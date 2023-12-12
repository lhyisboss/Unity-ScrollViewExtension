using System;
using ScrollViewExtension.Scripts.Adapter.DTO;
using ScrollViewExtension.Scripts.Core.Entity.Interface;
using UnityEngine;

namespace ScrollViewExtension.Scripts.Core.Service.Interface
{
    internal interface IFindIndex<TData> : IDisposable where TData : ScrollItemBase, new()
    {
        int ByPosition(Vector3 position, IScrollViewEntity<TData> viewEntity);
    }
}