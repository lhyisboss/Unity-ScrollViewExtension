using System;
using ScrollViewExtension.Scripts.Common;
using ScrollViewExtension.Scripts.Core.Entity.Interface;
using ScrollViewExtension.Scripts.Core.Service.Interface;
using ScrollViewExtension.Scripts.Core.UseCase.Interface;
using ScrollViewExtension.Scripts.DTO;
using UnityEngine;

namespace ScrollViewExtension.Scripts.Core.UseCase
{
    public class ScrollViewCalculator<TData> : UseCaseBase<TData>, IScrollViewCalculator where TData : ScrollItemBase, new()
    {
        private const float Epsilon = Const.Epsilon;
        
        private static ScrollViewCalculator<TData> instance;

        private int lastIndex;
        private int lastCount;
        private float maxOffset;

        public static ScrollViewCalculator<TData> CreateInstance(IScrollViewEntity<TData> viewEntity, IFindIndex<TData> findIndex)
        {
            return instance ??= new ScrollViewCalculator<TData>(viewEntity, findIndex);
        }

        public int CalculateInstanceNumber()
        {
            var number = Mathf.CeilToInt(viewEntity.GetViewLength / viewEntity.GetItemMinLength()) + 1;

            return viewEntity.Data.Count >= number ? number : viewEntity.Data.Count;
        }

        public Vector2 CalculateContentSize()
        {
            var size = viewEntity.IsVertical
                ? new Vector2(viewEntity.ContentSize.x, viewEntity.GetContentLength(viewEntity.Data.Count))
                : new Vector2(viewEntity.GetContentLength(viewEntity.Data.Count), viewEntity.ContentSize.y);

            return size;
        }

        public float CalculateBarPosition(int index)
        {
            const float offset = 0.00001f; //unity barの計算はすごく小さいな誤差があるため、その対策です
            
            var vertical = Mathf.Clamp01(1 - viewEntity.GetContentLength(index) / GetScrollableRange() - offset);
         
            if(viewEntity.IsVertical) 
                return vertical;
            
            return  1 - vertical;
        }

        /// <summary>
        /// 現在のパディングと新しいパディングを元にローリングの計算を行う関数です。
        /// </summary>
        /// <param name="currentPadding">現在のパディング。</param>
        /// <param name="newPadding">新しいパディング。</param>
        /// <param name="startIndex">開始のインデックス。</param>
        /// <returns>計算されたローリングの値を返します。</returns>
        public int CalculateRolling(Vector4 currentPadding, Vector4 newPadding, int startIndex)
        {
            if (currentPadding.LessThan(Vector4.zero) || newPadding.LessThan(Vector4.zero))
                throw new ArgumentException("padding can not be negative");
            
            // パディングの新旧の点を決定する
            var newP = viewEntity.IsVertical ? newPadding.x : newPadding.z;
            var currentP = viewEntity.IsVertical ? currentPadding.x : currentPadding.z;

            var rolling = 0; // ローリングの値を初期化
            var diff = newP - currentP; // パディングの差分を計算
            
            // パディングの差分を元にローリングの値を計算
            switch (diff)
            {
                case > Epsilon:
                {
                    // 下方向へのローリングを計算
                    while (diff > Epsilon)
                    {
                        rolling++;
                        // アイテムのサイズを計算に反映
                        diff -= viewEntity.GetItemSize(startIndex);
                        startIndex++;
                    }

                    break;
                }
                case < -Epsilon:
                {
                    // 上方向へのローリングを計算
                    while (diff < -Epsilon)
                    {
                        rolling--;
                        // アイテムのサイズを計算に反映
                        diff += viewEntity.GetItemSize(startIndex - 1);
                        startIndex--;
                    }

                    break;
                }
            }

            return rolling;
        }

        /// <summary>
        /// フレームワーク内の特定の位置に基づいてOffsetを計算します。
        /// </summary>
        /// <param name="index">項目の開始インデックス</param>
        /// <param name="count">対象となる項目の数</param>
        /// <param name="contentPos">コンテンツ位置。XおよびY座標で指定します。</param>
        /// <returns>計算されたオフセットを表す4次元ベクトル</returns>
        /// <exception cref="ArgumentException">countが0の場合にスローされます</exception>
        public Vector4 CalculateOffset(int index, int count, Vector3 contentPos)
        {
            // カウントが0であることは許可されていません
            if (count <= 0)
                throw new ArgumentException("count can not smaller than 0");

            // コンテンツの長さを計算します
            var contentLength = viewEntity.GetContentLength(viewEntity.Data.Count);

            // 前回のカウントとインデックスが異なる場合、再計算します
            if (lastCount != count || lastIndex != index)
            {
                // 最大オフセットを更新します
                maxOffset = contentLength - viewEntity.GetContentLength(count, index);
                lastIndex = index;
                lastCount = count;
            }

            // コンテンツの位置が負の場合は、0にリセットします
            contentPos = Mathf.Min(-contentPos.x, contentPos.y) < 0 ? Vector3.zero : contentPos;

            // コンテンツの位置がコンテンツの長さを超える場合、最大オフセットにリセットします
            contentPos = Mathf.Max(-contentPos.x, contentPos.y) > contentLength
                ? (viewEntity.IsVertical ? maxOffset : -maxOffset) * Vector3.one
                : contentPos;

            // オフセットを計算します
            var offset = viewEntity.GetContentLength(findIndex.ByPosition(contentPos, viewEntity));

            // オフセットを含むベクトルを生成して返します
            return CreateVectorWithOffset(viewEntity.IsVertical,
                Mathf.Clamp(offset, 0, maxOffset));
        }

        public void Dispose()
        {
            instance = null;
        }

        private Vector4 CreateVectorWithOffset(bool isVertical, float offset)
        {
            return isVertical
                ? new Vector4(offset + viewEntity.DefaultPadding.top, 0, viewEntity.DefaultPadding.left, viewEntity.DefaultPadding.right)
                : new Vector4(viewEntity.DefaultPadding.top, viewEntity.DefaultPadding.bottom, offset + viewEntity.DefaultPadding.left, 0);
        }

        /// <summary>
        /// 移動できる長さ
        /// </summary>
        /// <returns></returns>
        private float GetScrollableRange()
        {
            return viewEntity.GetContentLength(viewEntity.Data.Count) - viewEntity.GetViewLength;
        }

        private ScrollViewCalculator(IScrollViewEntity<TData> viewEntity, IFindIndex<TData> findIndex) : base(viewEntity, findIndex)
        {
        }
    }
}