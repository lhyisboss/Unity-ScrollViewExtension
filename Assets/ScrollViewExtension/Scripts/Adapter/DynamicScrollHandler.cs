/*
 * ***********************************************
 *
 * 機能名: DynamicScrollView
 * バージョン: 0.1.0
 * 作成日: 2023-12-06
 * 作者: lhyisboss
 * 言語: C#
 * 機能:
 * HorizontalOrVerticalLayoutGroupの上で、
 * 一列生成数の最適化かつ実行途中で動的サイズ変更できる処理を担っています。
 * Clean Architectureを基に構成されました。大まかな流れとしては以下に：
 *
 * Adapter => UseCase => Service => Entity
 *
 * Adapterは単に内部層にInputを提供し、返したOutputを使ってUIを更新します。
 * UseCaseはInputを受け取り、ServiceやEntityとやり取りし、Outputを生成します。
 * Entityは核となるロジック、検証やデータを管理するクラスです。
 *
 * ※：
 * VerticalとHorizontal両方向まとめて実装してはいますが、テストコードやテストなどを行ったのがVertical方向だけ。
 * Horizontal方向はテストしていないので、バグの可能性があり、次のバージョンでまたHorizontal方向のテストを行います。
 *
 * ***********************************************
 */

using System;
using System.Collections.Generic;
using ScrollViewExtension.Scripts.DTO;
using ScrollViewExtension.Scripts.Entity;
using ScrollViewExtension.Scripts.Entity.Interface;
using ScrollViewExtension.Scripts.Service;
using ScrollViewExtension.Scripts.UseCase;
using ScrollViewExtension.Scripts.UseCase.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace ScrollViewExtension.Scripts.Adapter
{
    /// <summary>
    /// Main入口かつAdapterクラスです。
    /// </summary>
    /// <typeparam name="TData">表示に使用するデータ</typeparam>
    /// <typeparam name="TItem">ビュー内のノード</typeparam>
    public abstract class DynamicScrollHandler<TData, TItem> : MonoBehaviour
        where TData : ScrollItemBase, new()
        where TItem : DynamicScrollViewItem<TData>
    {
        [SerializeField] private HorizontalOrVerticalLayoutGroup group;
        
        [SerializeField] private TItem itemPrefab;

        private ScrollRect scrollRect;

        private IScrollViewCalculator calculator;

        private IScrollViewDataHandler<TData> dataHandler;

        private IScrollViewEntity<TData> viewEntity;
        
        private List<TItem> list;

        private Vector4 lastPadding;

        private bool isInitialized;

        public TItem ItemPrefab => itemPrefab;

        public void Show(int startIndex = 0)
        {
            //item生成
            var count = calculator.CalculateInstanceNumber();
            list = GenerateItems(count);
            
            //表示する
            var range = dataHandler.GetRange(startIndex, count);
            for (var i = 0; i < range.Count; i++)
            {
                list[i].Show(range[i]);
            }
            
            //contentサイズ設置
            scrollRect.content.sizeDelta = calculator.CalculateContentSize();
            
            //bar位置設置
            scrollRect.verticalNormalizedPosition = calculator.CalculateBarPosition(startIndex);
            
            //padding設置
            var v4 = calculator.CalculateOffset(list[0].Data.Index, list.Count, scrollRect.content.localPosition);
            UpdatePadding(v4);
        }

        public void Show(float barPosition)
        {
            //item生成
            var count = calculator.CalculateInstanceNumber();
            list = GenerateItems(count);
            
            //contentサイズ設置
            scrollRect.content.sizeDelta = calculator.CalculateContentSize();
            
            //bar位置設置
            scrollRect.verticalNormalizedPosition = barPosition;
            
            //表示する
            var range = dataHandler.GetRange(scrollRect.content.localPosition, count);
            for (var i = 0; i < range.Count; i++)
            {
                list[i].Show(range[i]);
            }
            
            //padding設置
            var v4 = calculator.CalculateOffset(list[0].Data.Index, list.Count, scrollRect.content.localPosition);
            UpdatePadding(v4);
        }

        public void OnValueChanged(Vector2 pos)
        {
            if(!isInitialized) return;

            //padding設置
            var v4 = calculator.CalculateOffset(list[0].Data.Index, list.Count, scrollRect.content.localPosition);
            
            var count = calculator.CalculateRolling(lastPadding, v4, list[0].Data.Index);
            
            UpdatePadding(v4);
            
            StartRoll(count);
        }
        
        public TData CreateItem(Vector2 size)
        {
            var item = dataHandler.CreateItem(size);
            item.OnItemSizeChanged += OnItemSizeChanged;
            return item;
        }

        public void Initialize(List<TData> data = null)
        {
            if(isInitialized) return;
            
            scrollRect = GetComponent<ScrollRect>();
            scrollRect.onValueChanged.AddListener(OnValueChanged);
            
            viewEntity = ScrollViewEntity<TData>.CreateInstance(group.padding,
                group.spacing,
                GetComponent<RectTransform>().sizeDelta,
                scrollRect.content.sizeDelta,
                scrollRect.horizontal,
                scrollRect.vertical);
            
            GroupLayoutEntity.CreateInstance(viewEntity.IsVertical(),
                group.reverseArrangement,
                group.childForceExpandWidth,
                group.childForceExpandHeight,
                group.childControlWidth,
                group.childControlHeight,
                group.childAlignment,
                scrollRect.content.anchorMin,
                scrollRect.content.anchorMax);
            
            var findIndex = FindIndex<TData>.CreateInstance();
            calculator = ScrollViewCalculator<TData>.CreateInstance(viewEntity, findIndex);
            dataHandler = ScrollViewDataHandler<TData>.CreateInstance(viewEntity, findIndex);
            
            if(data is {Count: > 0})
                SetItems(data);

            isInitialized = true;
        }

        public void ResetHandler()
        {
            if(!isInitialized) return;
            
            list.ForEach(x => Destroy(x.gameObject));
            list.Clear();
            
            scrollRect.onValueChanged.RemoveListener(OnValueChanged);
            
            calculator.Dispose();
            dataHandler.Dispose();
            viewEntity.Dispose();
            
            calculator = null;
            dataHandler = null;
            viewEntity = null;
            
            isInitialized = false;
        }

        private void SetItems(List<TData> shell)
        {
            shell.ForEach(x =>
            {
                x.OnItemSizeChanged += OnItemSizeChanged;
            });

            dataHandler.SetScrollItems(shell);
        }

        private void UpdatePadding(Vector4 v4)
        {
            lastPadding = v4;
            
            group.padding.top = Mathf.RoundToInt(v4.x);
            group.padding.bottom = Mathf.RoundToInt(v4.y);
            group.padding.left = Mathf.RoundToInt(v4.z);
            group.padding.right = Mathf.RoundToInt(v4.w);
        }

        private List<TItem> GenerateItems(int count)
        {
            if (count <= 0)
            {
                throw  new ArgumentException("number of instances need greater than 0");
            }

            var items = new List<TItem>();
            
            for (var i = 0; i < count; i++)
            {
                var item = Instantiate(ItemPrefab, scrollRect.content);
                items.Add(item);
                
                item.Initialize();
            }
            
            return  items;
        }
        
        private void OnItemSizeChanged(ScrollItemBase obj)
        {
            scrollRect.content.sizeDelta = calculator.CalculateContentSize();

            dataHandler.UpdatePositionsFromIndex(obj.Index + 1);
        }

        /// <summary>
        /// リスト内のアイテムをローリングします。ローリングの数はパラメータ`rollingCount`によって決定されます。
        /// `rollingCount`が正の場合、リストの末尾からローリングを始めます。
        /// `rollingCount`が負の場合、リストの初めからローリングを始めます。
        /// </summary>
        /// <param name="rollingCount">ローリングを行うアイテムの数。正または負の数です。</param>
        private void StartRoll(int rollingCount)
        {
            // データ接点を保持する変数
            List<TData> range;
            switch (rollingCount)
            {
                // `rollingCount`が正の場合、リストの末尾からローリングを始めます。
                case > 0:
                    range = dataHandler.GetRange(list[^1].Data.Index + 1, rollingCount); // データ範囲を取得
                    for (var i = 0; i < rollingCount; i++)
                    {
                        list[0].transform.SetAsLastSibling(); // リストのアイテムを移動
                        var item = list[0]; // 現在のアイテムを取得
                        list.Remove(item); // リストからアイテムを削除
                        list.Add(item); // アイテムをリストの末尾に追加
                        item.Show(range[i]); // アイテムを表示
                    }

                    break;
                // `rollingCount`が負の場合、リストの初めからローリングを始めます。
                case < 0:
                    rollingCount = Mathf.Abs(rollingCount); // ローリング数を絶対値に変換
                    range = dataHandler.GetRange(list[0].Data.Index - rollingCount, rollingCount); // データ範囲を取得
                    for (var i = rollingCount - 1; i >= 0; i--)
                    {
                        list[^1].transform.SetAsFirstSibling(); // リストのアイテムを移動
                        var item = list[^1]; // 現在のアイテムを取得
                        list.Remove(item); // リストからアイテムを削除
                        list.Insert(0, item); // アイテムをリストの始めに追加
                        item.Show(range[i]); // アイテムを表示
                    }

                    break;
            }
        }
    }
}