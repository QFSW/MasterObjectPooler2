using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QFSW.MOP2
{
    public class MasterObjectPooler : MonoBehaviour
    {
        [SerializeField] private ObjectPool[] _pools = new ObjectPool[0];

        private readonly Dictionary<string, ObjectPool> _poolTable = new Dictionary<string, ObjectPool>();

        #region Initialization
        private void Start()
        {
            foreach (ObjectPool pool in _pools)
            {
                AddPool(pool);
            }
        }
        #endregion

        #region Internal
        private void DestroyPoolInternal(ObjectPool pool)
        {
            pool.Purge();
            Destroy(pool.ObjectParent);
        }
        #endregion

        #region PoolManagement
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
        #endregion

        #region GetObject/Component
        public GameObject GetObject(string poolName)
        {
            return GetPool(poolName).GetObject();
        }

        public GameObject GetObject(string poolName, Vector3 position)
        {
            return GetPool(poolName).GetObject(position);
        }

        public GameObject GetObject(string poolName, Vector3 position, Quaternion rotation)
        {
            return GetPool(poolName).GetObject(position, rotation);
        }

        public T GetObjectComponent<T>(string poolName) where T : class
        {
            return GetPool(poolName).GetObjectComponent<T>();
        }

        public T GetObjectComponent<T>(string poolName, Vector3 position) where T : class
        {
            return GetPool(poolName).GetObjectComponent<T>(position);
        }

        public T GetObjectComponent<T>(string poolName, Vector3 position, Quaternion rotation) where T : class
        {
            return GetPool(poolName).GetObjectComponent<T>(position, rotation);
        }
        #endregion

        #region Release/Destroy
        public void Release(GameObject obj) { Release(obj, obj.name); }
        public void Release(GameObject obj, string poolName)
        {
            GetPool(poolName).Release(obj);
        }

        public void Release(IEnumerable<GameObject> objs, string poolName)
        {
            GetPool(poolName).Release(objs);
        }

        public void ReleaseAll(string poolName)
        {
            GetPool(poolName).ReleaseAll();
        }

        public void Destroy(GameObject obj) { Destroy(obj, obj.name); }
        public void Destroy(GameObject obj, string poolName)
        {
            ObjectPool pool = GetPool(poolName);
            if (pool) { pool.Destroy(obj); }
            else { Destroy(obj); }
        }

        public void Destroy(IEnumerable<GameObject> objs, string poolName)
        {
            ObjectPool pool = GetPool(poolName);
            if (pool) { pool.Destroy(objs); }
            else
            {
                foreach (GameObject obj in objs)
                {
                    Object.Destroy(obj);
                }
            }
        }

        public void ReleaseAllInAllPools()
        {
            foreach (ObjectPool pool in _poolTable.Values)
            {
                pool.ReleaseAll();
            }
        }
        #endregion

        #region Miscellaneous
        public void Populate(string poolName, int quantity, PopulateMethod method = PopulateMethod.Set)
        {
            GetPool(poolName).Populate(quantity, method);
        }

        public void Purge(string poolName)
        {
            GetPool(poolName).Purge();
        }

        public void PurgeAll()
        {
            foreach (ObjectPool pool in _poolTable.Values)
            {
                pool.Purge();
            }
        }
        #endregion
    }
}
