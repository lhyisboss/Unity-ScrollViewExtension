using System.Collections.Generic;
using System.Linq;
using ScrollViewExtension.Scripts.Adapter.DTO;
using ScrollViewExtension.Scripts.Core.Entity.Interface;
using ScrollViewExtension.Scripts.Core.Service.Interface;
using ScrollViewExtension.Scripts.Core.UseCase.Interface;
using UnityEngine;

namespace ScrollViewExtension.Scripts.Core.UseCase
{
    internal class ScrollViewDataHandler<TData> : UseCaseBase<TData>, IScrollViewDataHandler<TData> where TData : ScrollItemBase, new()
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
        
        public void CreateMultipleItems(List<TData> data)
        {
            viewEntity.Data = data;
            UpdatePositionsFromIndex(0);
        }

        public void RemoveItem(int index)
        {
            viewEntity.RemoveItem(index);
            UpdatePositionsFromIndex(index);
        }

        public void RemoveBottomItem()
        {
            RemoveItem(viewEntity.Data[^1].Index);
        }
        
        public List<TData> GetRange(int index, int count)
        {
            if (viewEntity.Data.Count <= 0 || viewEntity.Data is null)
                return null;
            
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
            foreach (var item in viewEntity.Data.Skip(index))
            {
                var pos = viewEntity.CalculateItemPosition(item.Index);
                viewEntity.UpdateItemPosition(item.Index, pos);
            }
        }

        public bool IsDataBulkGreaterThanInstance(int genNum)
        {
            return viewEntity.Data.Count > genNum;
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