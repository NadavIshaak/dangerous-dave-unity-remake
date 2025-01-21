using System.Collections.Generic;
using UnityEngine;

public class MonoPool<T> : MonoSingleton<MonoPool<T>> where T : MonoBehaviour, IPoolable
{
    [SerializeField] private T _prefab;
    [SerializeField] private int _initialSize;
    [SerializeField] private Transform _parent;
    private Stack<T> _available;

    protected override void Awake()
    {
        base.Awake();
        _available = new Stack<T>();
        addItemsToPool();
    }

    public virtual T Get()
    {
        if (_available.Count == 0)
            addItemsToPool();

        var item = _available.Pop();
        item.Reset();
        item.gameObject.SetActive(true);
        return item;
    }

    public virtual void Return(T item)
    {
        item.gameObject.SetActive(false);
        _available.Push(item);
    }

    public virtual void ImmediateReturn(T item)
    {
        //an return that wont be changed by the further implementation
        item.gameObject.SetActive(false);
        _available.Push(item);
    }

    private void addItemsToPool()
    {
        for (var i = 0; i < _initialSize; i++)
        {
            var item = Instantiate(_prefab, _parent, true);
            item.gameObject.SetActive(false);
            _available.Push(item);
        }
    }
}