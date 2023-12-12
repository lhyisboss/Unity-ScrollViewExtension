using System;
using ScrollViewExtension.Scripts.Common;
using UnityEngine;

namespace ScrollViewExtension.Scripts.Adapter.DTO
{
    public abstract class ScrollItemBase : IDisposable
    {
        public event Action<ScrollItemBase> OnItemSizeChanged;

        protected ScrollItemBase(Vector2 size)
        {
            this.size = size;
        }

        private int index;
        /// <summary>
        /// リスト中の位置
        /// </summary>
        public int Index
        {
            get => index;
            internal set
            {
                if (value < 0)
                    throw new Exception("Index can not smaller than 0");

                index = value;
            }
        }
        
        private Vector2 size;
        /// <summary>
        /// Itemのsize
        /// </summary>
        public Vector2 Size
        {
            get => size;
            set
            {
                if(value.x < 0 || value.y < 0)
                    throw new Exception("Size can not smaller than 0");
                
                size = value;
                
                OnItemSizeChanged?.Invoke(this);
            }
        }
        
        public Vector2 Position { get; internal set; }

        private Vector2 pivot = new(0, 1);
        /// <summary>
        /// 検証せずにどのpivotでも正しいPosが得られるのもありだが、ややこしいから
        /// 検証して左上に固定する
        /// </summary>
        public Vector2 Pivot
        {
            get => pivot; 
            internal set
            {
                if (value.x != 0 || Math.Abs(value.y - 1) > Const.Epsilon)
                    throw new Exception("Pivot only can be 0,1");

                pivot = value;
            }
        }

        public void Dispose()
        {
            OnItemSizeChanged = null;
        }
    }
}