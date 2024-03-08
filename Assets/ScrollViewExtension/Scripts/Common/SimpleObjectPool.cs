using System.Collections.Generic;
using ScrollViewExtension.Scripts.Adapter;
using ScrollViewExtension.Scripts.Adapter.DTO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ScrollViewExtension.Scripts.Common
{
    internal interface IFactory<T>
    {
        T Create();
        
        void Destroy(T item);
        
        void Initialize(T item);
        
        void Reset(T item);
    }

    internal class InstanceGameObj<TData, TItem> : IFactory<TItem>
        where TData : ScrollItemBase, new()
        where TItem : DynamicScrollViewItem<TData>
    {
        private readonly TItem prefab;
        private readonly RectTransform content;
        
        public InstanceGameObj(TItem item, RectTransform content)
        {
            prefab = item;
            this.content = content;
        }

        public TItem Create()
        {
            return Object.Instantiate(prefab);
        }

        public void Initialize(TItem item)
        {
            item.Initialize();
            item.transform.SetParent(content, false);
            item.gameObject.SetActive(true);
        }

        public void Reset(TItem item)
        {
            item.gameObject.SetActive(false);
            item.transform.SetParent(content.parent, false);
        }

        public void Destroy(TItem item)
        {
            Object.Destroy(item.gameObject);
        }
    }

    internal class SimpleObjectPool<T>
    {
        private readonly Queue<T> objects;
        private readonly IFactory<T> factory;

        public SimpleObjectPool(IFactory<T> factory)
        {
            objects = new Queue<T>();
            this.factory = factory;
        }

        public IEnumerable<T> Get(int amount)
        {
            for (var i = 0; i < amount; i++)
            {
                T dequeue;
                if (objects.Count > 0)
                {
                    dequeue = objects.Dequeue();
                    factory.Initialize(dequeue);
                    yield return dequeue;
                }
                else
                {
                    dequeue = factory.Create();
                    factory.Initialize(dequeue);
                    yield return dequeue;
                }
            }
        }

        public T Get()
        {
            return objects.Count > 0 ? objects.Dequeue() : factory.Create();
        }

        public void Return(T item)
        {
            factory.Reset(item);
            objects.Enqueue(item);
        }

        public void Clear()
        {
            foreach (var o in objects)
            {
                factory.Destroy(o);
            }
            objects.Clear();
        }

        public int Count => objects.Count;
    }
}