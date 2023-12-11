using System;
using ScrollViewExtension.Scripts.Adapter.DTO;
using ScrollViewExtension.Scripts.Common;
using UnityEngine;

namespace ScrollViewExtension.Scripts.Adapter
{
    public abstract class DynamicScrollViewItem<TData> : MonoBehaviour where TData : ScrollItemBase
    {
        private const float Tolerance = Const.Epsilon;

        private bool isInitialized;

        private bool canCheckSize;
        
        public RectTransform RectTransform { get; private set; }

        public TData Data { get; private set; }
        
        public void Initialize()
        {
            if (isInitialized) return;
            
            RectTransform ??= GetComponent<RectTransform>();
            
            isInitialized = true;
        }

        /// <summary>
        /// 必ずbase.showを読んでください。
        /// </summary>
        /// <param name="data"></param>
        public virtual void Show(TData data)
        {
            canCheckSize = false;
            
            Data = data;
            RectTransform.sizeDelta = data.Size;
            RectTransform.pivot = data.Pivot;
            
            canCheckSize = true;
        }

        public void Update()
        {
            if(Data == null || !canCheckSize) return;

            if(!IsSizeEqual(Data))
                Data.Size = RectTransform.sizeDelta;
        }

        private bool IsSizeEqual(TData data)
        {
            return Math.Abs(data.Size.x - RectTransform.sizeDelta.x) < Tolerance &&
                   Math.Abs(data.Size.y - RectTransform.sizeDelta.y) < Tolerance;
        }
    }
}