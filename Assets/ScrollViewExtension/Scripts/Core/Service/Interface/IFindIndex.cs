using System;
using ScrollViewExtension.Scripts.Core.Entity.Interface;
using ScrollViewExtension.Scripts.DTO;
using UnityEngine;

namespace ScrollViewExtension.Scripts.Core.Service.Interface
{
    public interface IFindIndex<TData> : IDisposable where TData : ScrollItemBase, new()
    {
        int ByPosition(Vector3 position, IScrollViewEntity<TData> viewEntity);
    }
}