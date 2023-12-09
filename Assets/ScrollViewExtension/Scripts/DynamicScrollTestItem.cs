using ScrollViewExtension.Scripts.Adapter;
using ScrollViewExtension.Scripts.DTO;
using TMPro;
using UnityEngine;

namespace ScrollViewExtension.Scripts
{
    public class DynamicScrollTestItem : DynamicScrollViewItem<DynamicScrollTestItem.Base>
    {
        public class Base : ScrollItemBase
        {
            public int num;

            public Base(Vector2 size, int num) : base(size)
            {
                this.num = num;
            }

            public Base() : base(new Vector2(200,50))
            {
            }
        }

        [SerializeField] private TextMeshProUGUI text;

        public override void Show(Base @base)
        {
            base.Show(@base);
            
            text.text = @base.num.ToString();
            
            gameObject.SetActive(true);
        }
    }
}