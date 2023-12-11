using ScrollViewExtension.Scripts.Adapter.DTO;
using ScrollViewExtension.Scripts.Core.Entity.Interface;
using ScrollViewExtension.Scripts.Core.Service.Interface;
using UnityEngine;

namespace ScrollViewExtension.Scripts.Core.Service
{
    public class FindIndex<TData> : IFindIndex<TData> where TData : ScrollItemBase, new()
    {
        private static FindIndex<TData> instance;

        public static FindIndex<TData> CreateInstance()
        {
            return instance ??= new FindIndex<TData>();
        }

        /// <summary>
        /// 二分探索法で索引を探します
        /// </summary>
        /// <param name="position"></param>
        /// <param name="viewEntity"></param>
        /// <returns></returns>
        public int ByPosition(Vector3 position, IScrollViewEntity<TData> viewEntity)
        {
            var left = 0;
            var right = viewEntity.Data.Count - 1;
            var pos = viewEntity.IsVertical ? position.y : -position.x;

            while (left <= right)
            {
                var mid = left + (right - left) / 2;
                
                var itemPosition = viewEntity.IsVertical
                    ? -viewEntity.Data[mid].Position.y
                    : viewEntity.Data[mid].Position.x;
                
                // 次のアイテムの位置を計算する
                var nextItemPosition = CalculateNextItemPosition(mid, viewEntity);

                // 目的の位置がアイテムの範囲内にあるかどうかをチェックする
                if (InRange(position, nextItemPosition, mid, viewEntity))
                {
                    return mid;
                }

                if ( itemPosition < pos)
                {
                    left = mid + 1; // 目标が右侧
                }
                else
                {
                    right = mid - 1; // 目标が左侧
                }
            }

            // アイテムが見つからなかった場合
            return -1;
        }

        public void Dispose()
        {
            instance = null;
        }

        private static Vector2 CalculateNextItemPosition(int i, IScrollViewEntity<TData> viewEntity)
        {
            // 次の項目が存在すれば、その位置を返します。
            if (i + 1 < viewEntity.Data.Count) return viewEntity.Data[i + 1].Position;

            // 最初の項目（インデックス0）の場合、位置は現在の項目のサイズとデフォルトのパディングに基づきます。
            if (viewEntity.Data[i].Index == 0)
            {
                return new Vector2(
                    viewEntity.Data[i].Position.x + viewEntity.Data[i].Size.x + viewEntity.DefaultPadding.left,
                    viewEntity.Data[i].Position.y - viewEntity.Data[i].Size.y - viewEntity.DefaultPadding.top);
            }

            // それ以外の項目は、位置が現在の項目のサイズとスペーシングに基づきます。
            return new Vector2(
                viewEntity.Data[i].Position.x + viewEntity.Data[i].Size.x + viewEntity.Spacing,
                viewEntity.Data[i].Position.y - viewEntity.Data[i].Size.y - viewEntity.Spacing);
        }

        private static bool InRange(Vector3 position, Vector2 temp, int index, IScrollViewEntity<TData> viewEntity)
        {
            return viewEntity.IsVertical
                ? CheckVerticalRange(viewEntity.Data[index].Position.y, temp.y, -position.y)
                : CheckHorizontalRange(viewEntity.Data[index].Position.x, temp.x, position.x);
        }

        private static bool CheckVerticalRange(float yPos, float tempY, float positionY)
        {
            return yPos >= positionY && tempY < positionY;
        }

        private static bool CheckHorizontalRange(float xPos, float tempX, float positionX)
        {
            return xPos <= -positionX && tempX > -positionX;
        }

        private FindIndex()
        {
        }
    }
}