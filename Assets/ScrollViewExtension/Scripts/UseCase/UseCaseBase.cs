using ScrollViewExtension.Scripts.DTO;
using ScrollViewExtension.Scripts.Entity.Interface;
using ScrollViewExtension.Scripts.Service.Interface;

namespace ScrollViewExtension.Scripts.UseCase
{
    public abstract class UseCaseBase<TData> where TData : ScrollItemBase, new()
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