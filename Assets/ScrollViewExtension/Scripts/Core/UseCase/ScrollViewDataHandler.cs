using System.Collections.Generic;
using ScrollViewExtension.Scripts.Adapter.DTO;
using ScrollViewExtension.Scripts.Core.Entity.Interface;
using ScrollViewExtension.Scripts.Core.Service.Interface;
using ScrollViewExtension.Scripts.Core.UseCase.Interface;
using UnityEngine;

namespace ScrollViewExtension.Scripts.Core.UseCase
{
    public class ScrollViewDataHandler<TData> : UseCaseBase<TData>, IScrollViewDataHandler<TData> where TData : ScrollItemBase, new()
    {
        private static ScrollViewDataHandler<TData> instance;

        /// <summary>
        /// インスタンスを作成または既存のインスタンスを返します。
        /// </summary>
        /// <param name="viewEntity">スクロールビューに表示するデータがあるエンティティ</param>
        /// <param name="findIndex">アイテムのインデックスを探す方法</param>
        public static ScrollViewDataHandler<TData> CreateInstance(IScrollViewEntity<TData> viewEntity, IFindIndex<TData> findIndex)
        {
            return instance ?? new ScrollViewDataHandler<TData>(viewEntity, findIndex);
        }

        public TData CreateItem(Vector2 size)
        {
            var position = viewEntity.CalculateItemPosition(viewEntity.Data.Count);
            return viewEntity.CreateItem(size, position);
        }

        public List<TData> GetRange(int index, int count)
        {
            // データのカウントを超えた場合、残りのアイテム全てを取得します
            return index + count > viewEntity.Data.Count
                ? viewEntity.GetRange(viewEntity.Data.Count - count, count)
                : viewEntity.GetRange(index, count);
        }

        public List<TData> GetRange(Vector3 contentPos, int count)
        {
            var index = findIndex.ByPosition(contentPos, viewEntity);
            return GetRange(index, count);
        }

        /// <summary>
        /// 指定されたインデックスから位置を更新します。
        /// </summary>
        /// <param name="index">開始するインデックス。</param>
        /// <returns>なし</returns>
        public void UpdatePositionsFromIndex(int index)
        {
            // 指定されたインデックスからデータカウントまでループします。
            for (var i = index; i < viewEntity.Data.Count; i++)
            {
                // アイテムの位置を計算します。
                var pos = viewEntity.CalculateItemPosition(i);

                // 計算された位置をデータに設定します。
                viewEntity.UpdateItemPosition(Mathf.Min(i, viewEntity.Data.Count - 1), pos);
            }
        }

        public void SetScrollItems(List<TData> data)
        {
            viewEntity.Data = data;
         
            UpdatePositionsFromIndex(0);
        }

        public void Dispose()
        {
            instance = null;
        }

        private ScrollViewDataHandler(IScrollViewEntity<TData> viewEntity, IFindIndex<TData> findIndex) : base(viewEntity, findIndex)
        {
        }
    }
}