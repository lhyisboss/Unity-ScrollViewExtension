using System;
using ScrollViewExtension.Scripts.DTO;
using ScrollViewExtension.Scripts.Entity.Interface;
using UnityEngine;

namespace ScrollViewExtension.Scripts.Service.Interface
{
    public interface IFindIndex<TData> : IDisposable where TData : ScrollItemBase, new()
    {
        int ByPosition(Vector3 position, IScrollViewEntity<TData> viewEntity);
    }
}