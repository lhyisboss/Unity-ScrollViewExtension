using System;
using System.Collections.Generic;
using System.Linq;
using ScrollViewExtension.Scripts.Core.Entity.Interface;
using ScrollViewExtension.Scripts.DTO;
using UnityEngine;

namespace ScrollViewExtension.Scripts.Core.Entity
{
    public class ScrollViewEntity<TData> : IScrollViewEntity<TData> where TData : ScrollItemBase, new()
    {
        private static ScrollViewEntity<TData> instance;
        
        private int instanceCount;

        public static ScrollViewEntity<TData> CreateInstance(RectOffset defaultPadding,
            float spacing,
            Vector2 viewLength,
            Vector2 contentSize,
            bool isVertical)
        {
            return instance ??= new ScrollViewEntity<TData>(defaultPadding,
                spacing,
                viewLength,
                contentSize,
                isVertical);
        }

        public bool IsVertical { get; set; }

        private Vector2 viewLength;

        /// <summary>
        /// viewportの長さ
        /// </summary>
        public Vector2 SetViewLength
        {
            set
            {
                if (value.x <= 0 || value.y <= 0)
                    throw new Exception("viewport Length must greater than 0");

                viewLength = value;
            }
        }

        public float GetViewLength => IsVertical ? viewLength.y : viewLength.x;

        private Vector2 contentSize;
        public Vector2 ContentSize
        {
            get => contentSize;
            set => contentSize = IsVertical ? new Vector2(value.x, 0) : new Vector2(0, value.y);
        }

        private RectOffset defaultPadding;

        /// <summary>
        /// DefaultPaddingの値
        /// </summary>
        public RectOffset DefaultPadding
        {
            get => defaultPadding;
            set
            {
                if (value.top < 0 || value.bottom < 0 || value.left < 0 || value.right < 0)
                    throw new Exception("padding can not smaller than 0");

                defaultPadding = value;
            }
        }

        private float spacing;

        /// <summary>
        /// LayoutGroupのSpacing
        /// </summary>
        public float Spacing
        {
            get => spacing;
            set
            {
                if (value < 0)
                    throw new Exception("spacing can not smaller than 0");

                spacing = value;
            }
        }

        private List<TData> data;

        /// <summary>
        /// 現時点の全てのdata
        /// </summary>
        public List<TData> Data
        {
            get { return data ??= new List<TData>(); }
            set
            {
                if(value.Count <= 0)
                        throw new Exception("data can not be empty");

                for (var i = 0; i < value.Count; i++)
                {
                    if(value[i].Size.x <= 0 || value[i].Size.y <= 0)
                        throw new Exception("size can not be smaller than 0");
                    
                    value[i].Index = i;
                }
                
                instanceCount = value.Count;
                data = value;
            }
        }

        /// <summary>
        /// アイテムを作成します
        /// </summary>
        /// <param name="size">アイテムサイズ</param>
        /// <param name="position"></param>
        /// <returns>作成したアイテム</returns>
        public TData CreateItem(Vector2 size, Vector2 position)
        {
            // 新しいTDataインスタンスを生成します
            var newItem = GenerateNewItem(size, position);

            // instanceCountを増加させます
            instanceCount++;

            // 新しいアイテムをデータリストに追加します
            Data.Add(newItem);

            // 作成したアイテムを戻します
            return newItem;
        }

        /// <summary>
        /// 指定範囲のデータを取得します。
        /// </summary>
        /// <param name="start">取得を開始するインデックス。</param>
        /// <param name="count">取得するデータの数。</param>
        /// <returns>指定範囲内のデータリスト。</returns>
        /// <exception cref="ArgumentException">スタートインデックスまたはカウントが無効な場合にスローされます。</exception>
        public List<TData> GetRange(int start, int count)
        {
            // start または count が 0 以下、または start + count が Data の数より大きい場合、
            // ArgumentException をスローします。
            if (start < 0 || count < 0 || start + count > Data.Count)
                throw new ArgumentException("start or count is invalid");

            // 指定した範囲のデータを取得して返します。
            return Data.GetRange(start, count);
        }

        /// <summary>
        /// アイテムの最小長を取得します。
        /// これは垂直方向か水平方向で変わります：垂直ならアイテムのY軸のサイズ、水平ならアイテムのX軸のサイズの中で、最小値を返します。
        /// </summary>
        /// <returns>アイテムの最小長</returns>
        public float GetItemMinLength()
        {
            // データから最小長を計算します
            var min = Data.Min(i => IsVertical ? i.Size.y : i.Size.x);
            return min;
        }

        /// <summary>
        /// 指定した範囲からコンテンツの長さを取得します。
        /// </summary>
        /// <param name="count">計算する複数ノードの数。</param>
        /// <param name="index">開始インデックス（既定値は0）。</param>
        /// <returns>コンテンツの長さ。</returns>
        public float GetContentLength(int count, int index = 0)
        {
            var length = 0f;

            // 選択された範囲の内容に対して長さを計算します。
            var list = GetRange(index, count);
            length += list.Sum(i => IsVertical ? i.Size.y : i.Size.x);

            // スペースとパディングを追加します。
            length += index == 0 ? (list.Count - 1) * Spacing : list.Count * Spacing;

            // 最初のインデックスの場合、デフォルトのパディングを追加します。
            if (index == 0)
                length += IsVertical ? DefaultPadding.top : DefaultPadding.left;

            return length;
        }

        /// <summary>
        /// 指定されたインデックスのアイテム位置を計算します。
        /// </summary>
        /// <param name="index">計算するアイテムのインデックス。</param>
        /// <returns>与えられたインデックスに対応するアイテムの座標を返します。</returns>
        public Vector2 CalculateItemPosition(int index)
        {
            Vector2 position; // アイテムの座標を格納します。

            if (index == 0) // インデックスが0の場合、座標は(0,0)です。
            {
                position = new Vector2(0, 0);
            }
            else // インデックスが0でない場合、以前のアイテムに基づいて座標を計算します。
            {
                var lastItem = Data[index - 1]; // 前のアイテムを取得します。
                // 以下はパディングオフセット（余白）の計算に用います。
                var topPaddingOffset = lastItem.Position.y == 0 ? DefaultPadding.top : 0;
                var leftPaddingOffset = lastItem.Position.x == 0 ? DefaultPadding.left : 0;
                var spacingOffset = index == 1 ? 0 : Spacing; // アイテム間のスペーシングを決定します。

                // 新しいアイテムの座標を計算します。
                position = new Vector2(lastItem.Position.x + leftPaddingOffset + lastItem.Size.x + spacingOffset,
                    lastItem.Position.y - lastItem.Size.y - spacingOffset - topPaddingOffset);
            }

            return position; // 計算されたアイテムの座標を返します。
        }
        
        public void UpdateItemPosition(int index, Vector2 newPos)
        {
            if(index < 0 || index >= Data.Count)
                throw new ArgumentException("index is invalid");

            Data[index].Position = newPos;
        }

        /// <summary>
        /// 指定されたインデックスのアイテムのサイズを取得します。
        /// 縦方向の場合はアイテムの高さ、横方向の場合はアイテムの幅を取得します。
        /// </summary>
        /// <param name="index">取得するアイテムのインデックス。範囲は0以上Data.Count未満でなければなりません。</param>
        /// <returns>
        /// 縦方向であればアイテムの高さ+パディングまたはスペーシング、
        /// 横方向であればアイテムの幅+パディングまたはスペーシングを返します。
        /// </returns>
        /// <exception cref="ArgumentException">引数のindexが無効な場合、この例外がスローされます。</exception>
        public float GetItemSize(int index)
        {
            // indexが範囲外の場合、例外をスロー
            if (index < 0 || index >= Data.Count)
                throw new ArgumentException("indexは無効です");

            // indexが0の場合、パディングを考慮したサイズを返す
            if (index == 0)
                return IsVertical
                    ? Data[index].Size.y + DefaultPadding.top // 縦方向の場合、アイテムの高さ＋パディング
                    : Data[index].Size.x + DefaultPadding.left; // 横方向の場合、アイテムの幅＋パディング

            // indexが1以上の場合、スペーシングを考慮したサイズを返す
            return IsVertical
                ? Data[index].Size.y + Spacing // 縦方向の場合、アイテムの高さ＋スペーシング
                : Data[index].Size.x + Spacing; // 横方向の場合、アイテムの幅＋スペーシング
        }

        public void Dispose()
        {
            instance = null;
            Data.ForEach(i => i.Dispose());
        }

        private TData GenerateNewItem(Vector2 size, Vector2 position)
        {
            return new TData
            {
                Index = instanceCount, // 索引を今のinstanceCountに 
                Size = size,
                Position = position,
                Pivot = new Vector2(0, 1)
            };
        }

        private ScrollViewEntity(RectOffset defaultPadding,
            float spacing,
            Vector2 viewLength,
            Vector2 contentSize,
            bool isVertical)
        {
            IsVertical = isVertical;
            SetViewLength = viewLength;
            ContentSize = contentSize;
            Spacing = spacing;
            DefaultPadding = defaultPadding;
            instanceCount = 0;
        }
    }
}