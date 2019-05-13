using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFSW.MOP2
{
    public class MasterObjectPooler : MonoBehaviour
    {
        [SerializeField] private ObjectPool[] _pools = new ObjectPool[0];

        private readonly Dictionary<string, ObjectPool> _poolTable = new Dictionary<string, ObjectPool>();

        private void Start()
        {
            foreach (ObjectPool pool in _pools)
            {
                AddPool(pool);
            }
        }

        public void AddPool(ObjectPool pool) { AddPool(pool.PoolName, pool); }
        public void AddPool(string poolName, ObjectPool pool)
        {
            pool.Initialize();
            pool.ObjectParent.parent = transform;

            if (_poolTable.ContainsKey(poolName))
            {
                Debug.LogWarning(string.Format("{0} could not be added to the pool table as a pool with the same name already exists", poolName));
            }
            else
            {
                _poolTable.Add(poolName, pool);
            }
        }

        public ObjectPool GetPool(string poolName)
        {
            if (_poolTable.ContainsKey(poolName))
            {
                return _poolTable[poolName];
            }
            else
            {
                throw new ArgumentException(string.Format("Cannot get pool {0} as it is not present in the pool table", poolName));
            }
        }

        public ObjectPool this[string poolName]
        {
            get
            {
                return GetPool(poolName);
            }
            set
            {
                _poolTable.Remove(poolName);
                AddPool(poolName, value);
            }
        }

        public void ReleaseAllInAllPools()
        {
            foreach (ObjectPool pool in _poolTable.Values)
            {
                pool.ReleaseAll();
            }
        }

        public void PurgeAll()
        {
            foreach (ObjectPool pool in _poolTable.Values)
            {
                pool.Purge();
            }
        }

        public void DestroyAllPools()
        {
            foreach (ObjectPool pool in _poolTable.Values)
            {
                DestroyPoolInternal(pool);
            }

            _poolTable.Clear();
        }

        public void DestroyPool(string poolName)
        {
            ObjectPool pool = GetPool(poolName);
            DestroyPoolInternal(pool);
            _poolTable.Remove(poolName);
        }

        private void DestroyPoolInternal(ObjectPool pool)
        {
            pool.Purge();
            Destroy(pool.ObjectParent);
        }
    }
}
