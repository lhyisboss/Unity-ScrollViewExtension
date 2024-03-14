using System.Collections;
using System.Collections.Generic;
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
            // 一気に20個追加の場合(20じゃなくても1個1個追加するのもできる)
            // for (var i = 0; i < 1; i++)
            // {
            //     var item = test.CreateItem(new Vector2(50, 50));
            //     item.num = i;
            // }
            //
            // for (var i = 1; i < 2; i++)
            // {
            //     var item = test.CreateItem(new Vector2(50, 80));
            //     item.num = i;
            // }
            test.Show();
            indexText.text = "next index: " + nextIndex;
            barText.text = "next bar: " + nextBar;
        }
        
        /// <summary>
        /// 複数のアイテムを追加する例
        /// </summary>
        public void OnClickAddButton()
        {
            for (int i = 0; i < 1; i++)
            {
                var item = test.CreateItem(new Vector2(100, 40));
                item.num = item.Index;
            }
            
            //今のBar位置は一番下にいる場合は一番下を表示する、でないと今の位置のままで表示する
            // if (test.GetScrollBarPos() == 0)
            // { 
            //     test.Show(0.0f);
            // }
            // else
            // { 
            //     test.Show();
            // }
            test.Show();
        }
        
        public void OnClickRemoveButton()
        {
            test.RemoveItem();
            test.Show();
        }

        public void OnClickIndexButton()
        {
            test.Show(nextIndex);
            nextIndex = Random.Range(0, 20);
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
            StartCoroutine(InitAndShow());
        }

        private IEnumerator InitAndShow()
        {
            yield return null;

            // var list = new List<DynamicScrollTestItem.Base>();
            // for (var i = 0; i < 5000; i++)
            // {
            //     list.Add(new DynamicScrollTestItem.Base(new Vector2(200, 50), i));
            // }

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