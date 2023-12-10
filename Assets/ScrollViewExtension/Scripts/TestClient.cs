using System.Collections.Generic;
using UnityEngine;

namespace ScrollViewExtension.Scripts
{
    /// <summary>
    /// 二種類の初期化方法がございます
    /// </summary>
    public class TestClient : MonoBehaviour
    {
        [SerializeField] private DynamicScrollTest test;
        
        public void Show()
        {
            for (var i = 0; i < 501; i++)
            {
                var item = test.CreateItem(new Vector2(50, 200));
                item.num = i;
            }
            
            test.Show(0.4024f);
        }

        private void Start()
        {
            test.Initialize();
            Show();
        }

        
        // private void Start()
        // {
        //     var list = new List<DynamicScrollTestItem.Base>();
        //     list.Add(new DynamicScrollTestItem.Base(new Vector2(200, 50), 1));
        //     list.Add(new DynamicScrollTestItem.Base(new Vector2(200, 50), 2));
        //     list.Add(new DynamicScrollTestItem.Base(new Vector2(200, 50), 3));
        //     list.Add(new DynamicScrollTestItem.Base(new Vector2(200, 50), 4));
        //     list.Add(new DynamicScrollTestItem.Base(new Vector2(200, 50), 5));
        //     list.Add(new DynamicScrollTestItem.Base(new Vector2(200, 50), 6));
        //     list.Add(new DynamicScrollTestItem.Base(new Vector2(200, 50), 7));
        //     list.Add(new DynamicScrollTestItem.Base(new Vector2(200, 50), 8));
        //     list.Add(new DynamicScrollTestItem.Base(new Vector2(200, 50), 9));
        //     list.Add(new DynamicScrollTestItem.Base(new Vector2(200, 50), 10));
        //     
        //     test.Initialize(list);
        //     test.Show(0);
        // }
    }
}