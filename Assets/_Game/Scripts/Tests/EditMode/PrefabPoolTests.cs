using System;
using Game.Core;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Tests.EditMode
{
    public class PrefabPoolTests
    {
        private GameObject _prefab;
        private Transform _parent;
        private PrefabPool<Transform> _pool;

        [SetUp]
        public void SetUp()
        {
            _prefab = new GameObject("PoolTestPrefab");
            _parent = new GameObject("PoolTestParent").transform;
            _pool = new PrefabPool<Transform>(_prefab.transform, _parent, defaultCapacity: 4, maxSize: 2);
        }

        [TearDown]
        public void TearDown()
        {
            _pool.Dispose();
            Object.DestroyImmediate(_parent.gameObject); // also removes instances still active
            Object.DestroyImmediate(_prefab);
        }

        [Test]
        public void Get_ReturnsActiveInstanceUnderParent()
        {
            Transform item = _pool.Get();

            Assert.That(item, Is.Not.Null);
            Assert.That(item, Is.Not.SameAs(_prefab.transform));
            Assert.That(item.gameObject.activeSelf, Is.True);
            Assert.That(item.parent, Is.SameAs(_parent));
            Assert.That(_pool.CountActive, Is.EqualTo(1));
        }

        [Test]
        public void Get_WithPose_PlacesInstance()
        {
            var position = new Vector3(1f, 0f, 2f);
            Quaternion rotation = Quaternion.Euler(0f, 90f, 0f);

            Transform item = _pool.Get(position, rotation);

            Assert.That(Vector3.Distance(item.position, position), Is.LessThan(1e-5f));
            Assert.That(Quaternion.Angle(item.rotation, rotation), Is.LessThan(1e-3f));
        }

        [Test]
        public void Release_DeactivatesAndNextGetReusesInstance()
        {
            Transform first = _pool.Get();
            _pool.Release(first);

            Assert.That(first.gameObject.activeSelf, Is.False);
            Assert.That(_pool.CountInactive, Is.EqualTo(1));

            Transform second = _pool.Get();

            Assert.That(second, Is.SameAs(first));
            Assert.That(second.gameObject.activeSelf, Is.True);
        }

        [Test]
        public void Release_Twice_Throws()
        {
            Transform item = _pool.Get();
            _pool.Release(item);

            Assert.Throws<InvalidOperationException>(() => _pool.Release(item));
        }

        [Test]
        public void Release_BeyondMaxSize_DestroysExtraInstances()
        {
            Transform a = _pool.Get();
            Transform b = _pool.Get();
            Transform c = _pool.Get();

            _pool.Release(a);
            _pool.Release(b);
            _pool.Release(c); // maxSize is 2, so this one is destroyed

            Assert.That(_pool.CountInactive, Is.EqualTo(2));
            Assert.That(c == null, Is.True);
        }

        [Test]
        public void Prewarm_CreatesInactiveInstances()
        {
            _pool.Prewarm(2);

            Assert.That(_pool.CountInactive, Is.EqualTo(2));
            Assert.That(_pool.CountActive, Is.EqualTo(0));
            Assert.That(_parent.childCount, Is.EqualTo(2));
        }

        [Test]
        public void Clear_DestroysInactiveInstances()
        {
            _pool.Prewarm(2);
            _pool.Clear();

            Assert.That(_pool.CountInactive, Is.EqualTo(0));
            Assert.That(_parent.childCount, Is.EqualTo(0));
        }

        [Test]
        public void Constructor_NullPrefab_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new PrefabPool<Transform>(null));
        }
    }
}
