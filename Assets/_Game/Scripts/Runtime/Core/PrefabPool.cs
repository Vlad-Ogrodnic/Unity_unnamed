using System;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Game.Core
{
    /// <summary>
    /// Pool of instances of one prefab, built on <see cref="ObjectPool{T}"/>.
    /// Instances are active while taken and inactive while in the pool.
    /// The owner creates the pool and hands <see cref="Release"/> to the instances it spawns.
    /// </summary>
    public sealed class PrefabPool<T> : IDisposable where T : Component
    {
        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly ObjectPool<T> _pool;

        public int CountActive => _pool.CountActive;
        public int CountInactive => _pool.CountInactive;

        /// <param name="prefab">Prefab to instantiate.</param>
        /// <param name="parent">Optional parent for the instances, to keep the hierarchy tidy.</param>
        /// <param name="defaultCapacity">Initial size of the internal storage.</param>
        /// <param name="maxSize">Instances released beyond this many inactive ones are destroyed.</param>
        public PrefabPool(T prefab, Transform parent = null, int defaultCapacity = 16, int maxSize = 256)
        {
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            _prefab = prefab;
            _parent = parent;
            _pool = new ObjectPool<T>(
                Create,
                item => item.gameObject.SetActive(true),
                item => item.gameObject.SetActive(false),
                DestroyInstance,
                collectionCheck: true,
                defaultCapacity,
                maxSize);
        }

        public T Get() => _pool.Get();

        public T Get(Vector3 position, Quaternion rotation)
        {
            T item = _pool.Get();
            item.transform.SetPositionAndRotation(position, rotation);
            return item;
        }

        public void Release(T item) => _pool.Release(item);

        /// <summary>Creates instances up front so the first shots of a fight don't allocate.</summary>
        public void Prewarm(int count)
        {
            var buffer = new T[count];
            for (int i = 0; i < count; i++)
                buffer[i] = _pool.Get();
            for (int i = 0; i < count; i++)
                _pool.Release(buffer[i]);
        }

        /// <summary>Destroys the inactive instances. Active ones are left to their owners.</summary>
        public void Clear() => _pool.Clear();

        public void Dispose() => _pool.Dispose();

        private T Create()
        {
            T item = Object.Instantiate(_prefab, _parent);
            item.gameObject.SetActive(false);
            return item;
        }

        private static void DestroyInstance(T item)
        {
            if (item == null)
                return;

            // Edit Mode tests create pools outside Play Mode, where Destroy is not allowed.
            if (Application.isPlaying)
                Object.Destroy(item.gameObject);
            else
                Object.DestroyImmediate(item.gameObject);
        }
    }
}
