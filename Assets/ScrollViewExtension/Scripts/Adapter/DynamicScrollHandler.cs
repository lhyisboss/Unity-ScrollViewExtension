/*
 * ***********************************************
 *
 * 機能名: DynamicScrollView
 * バージョン: 0.2.4
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
 * 配置流れ:
 * 1.このクラスを継承して具体的なTDataとTItemを指定する
 * 2.継承したクラスを目標とするScrollViewオブジェクトにアタッチしてフィールドを設定する
 * 3.ScrollViewのContentにVerticalLayoutGroupもしくはHorizontalLayoutGroupをアタッチする(ContentSizeFitterなし)
 *
 * 使用流れ(二つに分ける):
 * 1.
 * 事前にlistを用意してInitializeの時にlistを渡してShowを呼ぶだけ
 *
 * 2.
 * Initializeをパラメータなしで実行し、その後CreateItemを呼んで新しいアイテムを追加して最後はShowを呼ぶ
 *
 * ※使用例はTestClient.cs、DynamicScrollViewExample.unityにあるので、ご参考ください
 *
 * ***********************************************
 */

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ScrollViewExtension.Scripts.Adapter.DTO;
using ScrollViewExtension.Scripts.Common;
using ScrollViewExtension.Scripts.Configuration;
using ScrollViewExtension.Scripts.Core.Entity;
using ScrollViewExtension.Scripts.Core.Entity.Interface;
using ScrollViewExtension.Scripts.Core.Service;
using ScrollViewExtension.Scripts.Core.UseCase;
using ScrollViewExtension.Scripts.Core.UseCase.Interface;
using UnityEngine;
using UnityEngine.UI;

[assembly: InternalsVisibleTo("Tests")]
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

        [SerializeField] private bool isVertical;

        [Tooltip("Show()の際に現在表示内容をリフレッシュするかどうか")]
        [SerializeField] private bool needRefreshView;

        [Tooltip("前もってノードをロードする\n" +
                 "ただインスタンス生成数が上昇する\n" +
                 "ノードの内容が重くない場合はfalse推奨(最小生成数で表示していく)\n" +
                 "非同期処理や重い内容の場合はtrue推奨(一定程度表示の際に緩和できる)")]
        [SerializeField] private bool needPreLoadNode;

        private GroupLayoutConfig config;
        
        private ScrollRect scrollRect;

        private IScrollViewCalculator calculator;

        private IScrollViewDataHandler<TData> dataHandler;

        private IScrollViewEntity<TData> viewEntity;
        
        private List<TItem> list;
        
        private SimpleObjectPool<TItem> pool;

        private RectOffset defaultPadding;
        
        private Vector4 lastPadding;

        private bool isInitialized;

        public TItem ItemPrefab => itemPrefab;

        /// <summary>
        /// データの数が変更なしなら、このまま呼んでいいですが、
        /// 変更あった場合はresethandlerを先に呼んで再初期化してください。
        /// </summary>
        /// <param name="startIndex"></param>
        public void Show(int startIndex)
        {
            if (IsItemDataListNull())
            {
                UpdateContentSize();
                return;
            }
            
            //item生成
            var count = calculator.CalculateInstanceNumber(needPreLoadNode);
            list = list.Count < count ? pool.Get(count).ToList() : list;
            
            //表示する
            var range = dataHandler.GetRange(startIndex, count);
            for (var i = 0; i < range.Count; i++)
            {
                list[i].UpdateData(range[i]);
            }
            
            UpdateContentSize();
            
            //bar位置設置
            var barPosition = calculator.CalculateBarPosition(startIndex);
            scrollRect.normalizedPosition = new Vector2(barPosition, barPosition);
            
            //padding設置
            var v4 = calculator.CalculateOffset(list[0].Data.Index, list.Count, scrollRect.content.anchoredPosition);
            UpdatePadding(v4);
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
        }

        public void Show(float barPosition)
        {
            if (IsItemDataListNull())
            {
                UpdateContentSize();
                return;
            }
            
            //item生成
            var count = calculator.CalculateInstanceNumber(needPreLoadNode);
            list = list.Count < count ? pool.Get(count).ToList() : list;
            
            UpdateContentSize();
            
            //bar位置設置
            scrollRect.normalizedPosition = new Vector2(barPosition, barPosition);
            
            //表示する
            var range = dataHandler.GetRange(scrollRect.content.anchoredPosition, count);
            for (var i = 0; i < range.Count; i++)
            {
                list[i].UpdateData(range[i]);
            }
            
            //padding設置
            var v4 = calculator.CalculateOffset(list[0].Data.Index, list.Count, scrollRect.content.anchoredPosition);
            UpdatePadding(v4);
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
        }
        
        public void Show()
        {
            if (IsItemDataListNull())
            {
                UpdateContentSize();
                return;
            }
            
            //item生成
            var count = calculator.CalculateInstanceNumber(needPreLoadNode);
            var needUpData = list.Count < count;
            if(needUpData)
                list = pool.Get(count).ToList();
            
            UpdateContentSize();

            if (needUpData)
            {
                //表示する
                var range = dataHandler.GetRange(scrollRect.content.anchoredPosition, count);
                for (var i = 0; i < range.Count; i++)
                {
                    list[i].UpdateData(range[i]);
                }
                
                //padding設置
                var v4 = calculator.CalculateOffset(list[0].Data.Index, list.Count, scrollRect.content.anchoredPosition);
                UpdatePadding(v4);
            }
            else if (needRefreshView)
            {
                //表示する
                var range = dataHandler.GetRange(list[0].Data.Index, count);
                for (var i = 0; i < range.Count; i++)
                {
                    list[i].UpdateData(range[i]);
                }
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
        }

        public void OnValueChanged(Vector2 pos)
        {
            if (!isInitialized || list == null || IsItemDataListNull())
            {
                scrollRect.velocity = Vector2.zero; //OnValueChangedはなるべく呼ばないためにゼロにした
                
                if (isVertical)
                    scrollRect.verticalScrollbar.size = 1; //barがAutohideになっていない場合の対策
                else
                    scrollRect.horizontalScrollbar.size = 1;
                
                return;
            }

            //padding設置
            var v4 = calculator.CalculateOffset(list[0].Data.Index, list.Count, scrollRect.content.anchoredPosition, needPreLoadNode);
            
            var count = calculator.CalculateRolling(lastPadding, v4, list[0].Data.Index);
            
            UpdatePadding(v4);
            
            StartRoll(count);
        }
        
        /// <summary>
        /// 新しいアイテムを作る、作ったアイテムがリストの一番下に追加する
        /// <para>動的新しいアイテムを追加した場合は必ず<see cref="UpdateContentSize"/>が呼ばれる必要がある</para>
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public TData CreateItem(Vector2 size)
        {
            var item = dataHandler.CreateItem(size);
            item.OnItemSizeChanged += OnItemSizeChanged;
            return item;
        }
        
        private bool IsItemDataListNull()
        {
            return dataHandler.GetRange(0, 1) is null;
        }

        /// <summary>
        /// 現在のitemリストを基にContentSizeを更新する
        /// </summary>
        public void UpdateContentSize()
        {
            //contentサイズ設置
            scrollRect.content.sizeDelta = calculator.CalculateContentSize();
        }

        /// <summary>
        /// 今の該当する方向のBar位置取得する
        /// </summary>
        /// <returns></returns>
        public float GetScrollBarPos()
        {
            return isVertical ? scrollRect.normalizedPosition.y : scrollRect.normalizedPosition.x;
        }

        /// <summary>
        /// 基本は一回だけ呼べば行ける
        /// <para>アイテムリストが大きいな変更があった場合は<see cref="ResetHandler"/>して再初期化する必要がある(現段階)</para>
        /// それ以外は例えばアイテムリストの一番下に新しいアイテムを追加したい場合は<see cref="CreateItem"/>呼べば行ける
        /// </summary>
        /// <param name="data"></param>
        public void Initialize(List<TData> data = null)
        {
            if(isInitialized) return;
            
            scrollRect = GetComponent<ScrollRect>();
            scrollRect.onValueChanged.AddListener(OnValueChanged);
            
            // listがnullの場合は初期化する
            list ??= new List<TItem>();
            pool ??= new SimpleObjectPool<TItem>(new InstanceGameObj<TData, TItem>(itemPrefab, scrollRect.content));

            defaultPadding = new RectOffset(group.padding.left, group.padding.right, group.padding.top, group.padding.bottom);
            
            var rect = scrollRect.viewport.rect;
            var viewLength = new Vector2(rect.width, rect.height);
            viewEntity = ScrollViewEntity<TData>.CreateInstance(defaultPadding,
                group.spacing,
                viewLength,
                scrollRect.content.sizeDelta,
                isVertical);

            config = Resources.Load<GroupLayoutConfig>(scrollRect.horizontal
                ? Const.HorizontalPath
                : Const.VerticalPath);

            SetLayoutWithConfig(config);
            
            var findIndex = FindIndex<TData>.CreateInstance();
            calculator = ScrollViewCalculator<TData>.CreateInstance(viewEntity, findIndex);
            dataHandler = ScrollViewDataHandler<TData>.CreateInstance(viewEntity, findIndex);
            
            if(data is {Count: > 0})
                SetItems(data);

            isInitialized = true;
        }

        public void ResetHandler(bool needRelease = false)
        {
            if(!isInitialized || list == null) return;

            group.padding = defaultPadding;
            
            list.ForEach(x => pool.Return(x));
            list.Clear();
            
            scrollRect.onValueChanged.RemoveListener(OnValueChanged);
            scrollRect.content.sizeDelta = Vector2.zero;
            
            if (needRelease)
            {
                pool.Clear();
            }
            
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

        private void OnItemSizeChanged(ScrollItemBase obj)
        {
            scrollRect.content.sizeDelta = calculator.CalculateContentSize();

            dataHandler.UpdatePositionsFromIndex(obj.Index + 1);
        }

        private void SetLayoutWithConfig(GroupLayoutConfig layoutConfig)
        {
            group.reverseArrangement = layoutConfig.reverseArrangement;
            group.childAlignment = layoutConfig.childAlignment;
            group.childForceExpandWidth = layoutConfig.childForceExpandWidth;
            group.childForceExpandHeight = layoutConfig.childForceExpandHeight;
            group.childControlWidth = layoutConfig.childControlWidth;
            group.childControlHeight = layoutConfig.childControlHeight;
            scrollRect.content.anchorMin = layoutConfig.minAnchor;
            scrollRect.content.anchorMax = layoutConfig.maxAnchor;
            scrollRect.content.pivot = layoutConfig.pivot;
            scrollRect.horizontal = !isVertical;
            scrollRect.vertical = isVertical;
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
                        item.UpdateData(range[i], rollingCount - i > list.Count); // アイテムを表示
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
                        item.UpdateData(range[i], rollingCount - i - 1 < rollingCount - list.Count); // アイテムを表示
                    }

                    break;
            }
        }
    }
}