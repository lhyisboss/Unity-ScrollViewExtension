using TMPro;
using UnityEngine;

namespace ScrollViewExtension.Scripts
{
    /// <summary>
    /// 二種類のデータを初期化する方法がございます
    /// </summary>
    public class TestClient : MonoBehaviour
    {
        [SerializeField] private DynamicScrollTest test;
        
        [SerializeField] private TextMeshProUGUI indexText;

        [SerializeField] private TextMeshProUGUI barText;
        
        private int nextIndex;

        private float nextBar;
        
        public void Show()
        {
            for (var i = 0; i < 501; i++)
            {
                var item = test.CreateItem(new Vector2(50, 50));
                item.num = i;
            }
            
            test.Show(297);
            indexText.text = "next index: " + nextIndex;
            barText.text = "next bar: " + nextBar;
        }

        public void OnClickIndexButton()
        {
            test.Show(nextIndex);
            nextIndex = Random.Range(0, 500);
            indexText.text = "next index: " + nextIndex;
        }

        public void OnClickBarButton()
        {
            test.Show(nextBar);
            nextBar = Random.Range(0f, 1f);
            barText.text = "next bar: " + nextBar;
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