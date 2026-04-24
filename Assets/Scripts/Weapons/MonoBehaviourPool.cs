using System;
using UnityEngine;
using UnityEngine.Pool;

public class MonoBehaviourPool<T> where T : MonoBehaviour
{
    public IObjectPool<T> Pool { get; }

    private readonly Action<T> _onCreate;

    public MonoBehaviourPool(T prefab, int defaultCapacity, int maxSize, Transform parent, Action<T> onCreate = null)
    {
        _onCreate = onCreate;
        Pool = new ObjectPool<T>(() => Create(prefab, parent), OnGet, OnRelease, OnDestroy, collectionCheck: true,
            defaultCapacity: defaultCapacity, maxSize: maxSize
        );
    }

    public T Get()
    {
        return Pool.Get();
    }

    private T Create(T prefab, Transform parent)
    {
        T obj = UnityEngine.Object.Instantiate(prefab);
        obj.gameObject.SetActive(false);
        _onCreate?.Invoke(obj);
        return obj;
    }

    private static void OnGet(T obj)
    {
        obj.gameObject.SetActive(true);
    }

    private static void OnRelease(T obj)
    {
        obj.gameObject.SetActive(false);
    }

    private static void OnDestroy(T obj)
    {
        UnityEngine.Object.Destroy(obj.gameObject);
    }
}