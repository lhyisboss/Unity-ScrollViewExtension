using ScrollViewExtension.Scripts.Adapter.DTO;
using ScrollViewExtension.Scripts.Core.Entity.Interface;
using ScrollViewExtension.Scripts.Core.Service.Interface;

namespace ScrollViewExtension.Scripts.Core.UseCase
{
    internal abstract class UseCaseBase<TData> where TData : ScrollItemBase, new()
    {
        protected readonly IScrollViewEntity<TData> viewEntity;
        protected readonly IFindIndex<TData> findIndex;

        protected UseCaseBase(IScrollViewEntity<TData> viewEntity, IFindIndex<TData> findIndex)
        {
            this.viewEntity = viewEntity;
            this.findIndex = findIndex;
        }
    }
}