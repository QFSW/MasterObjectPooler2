using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QFSW.MOP2
{
    /// <summary>
    /// MasterObjectPooler manages various ObjectPools. By using a MasterObjectPooler, you can perform a large variety of pool operations with a named string reference
    /// instead of requiring an object reference to the ObjectPool. Furthermore, initialization of the pools is handled by the MOP.
    /// Pools can be added either via the inspector or at runtime.
    /// </summary>
    public class MasterObjectPooler : MonoBehaviour
    {
        [Tooltip("Forces the MOP into singleton mode. This means the MOP will be made scene persistent and will not be destroyed when new scenes are loaded.")]
        [SerializeField] private bool _singletonMode = false;
        [SerializeField] private ObjectPool[] _pools = new ObjectPool[0];

        /// <summary>
        /// Singleton reference to the MOP. Only valid and set if the singleton option is enabled for the MOP.
        /// </summary>
        public static MasterObjectPooler Instance { get; private set; }

        private readonly Dictionary<string, ObjectPool> _poolTable = new Dictionary<string, ObjectPool>();

        #region Initialization
        private void Awake()
        {
            if (_singletonMode)
            {
                if (Instance == null)
                {
                    Instance = this;

                    if (transform.parent == null)
                    {
                        DontDestroyOnLoad(gameObject);
                    }
                    else
                    {
                        Debug.LogWarning($"Singleton mode enabled for the Master Object Pooler '{name}' which is not a root GameObject; this means it cannot be made scene persistent");
                    }
                }
                else
                {
                    Object.Destroy(gameObject);
                }
            }
        }

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
            Object.Destroy(pool.ObjectParent.gameObject);
        }
        #endregion

        #region PoolManagement
        /// <summary>
        /// Adds an ObjectPool to the MasterObjectPooler and initializes it.
        /// </summary>
        /// <param name="pool">The ObjectPool to add to the MasterObjectPooler.</param>
        public void AddPool(ObjectPool pool) { AddPool(pool.PoolName, pool); }

        /// <summary>
        /// Adds an ObjectPool to the MasterObjectPooler and initializes it.
        /// </summary>
        /// <param name="poolName">Override for the named string reference to use for this pool. By default uses the ObjectPool's name.</param>
        /// <param name="pool">The ObjectPool to add to the MasterObjectPooler.</param>
        public void AddPool(string poolName, ObjectPool pool)
        {
            pool.Initialize();
            pool.ObjectParent.parent = transform;

            if (_poolTable.ContainsKey(poolName))
            {
                Debug.LogWarning($"{poolName} could not be added to the pool table as a pool with the same name already exists");
            }
            else
            {
                _poolTable.Add(poolName, pool);
            }
        }

        /// <summary>
        /// Retrieves a pool.
        /// </summary>
        /// <param name="poolName">The name of the pool to retrieve.</param>
        /// <returns>The retrieved pool.</returns>
        public ObjectPool GetPool(string poolName)
        {
            if (_poolTable.ContainsKey(poolName))
            {
                return _poolTable[poolName];
            }

            throw new ArgumentException($"Cannot get pool {poolName} as it is not present in the pool table");
        }

        /// <summary>
        /// Retrieves/adds a pool.
        /// </summary>
        /// <param name="poolName">The name of the pool to retrieve/add.</param>
        /// <returns>The retrieved pool.</returns>
        public ObjectPool this[string poolName]
        {
            get => GetPool(poolName);
            set
            {
                _poolTable.Remove(poolName);
                AddPool(poolName, value);
            }
        }

        /// <summary>
        /// Destroys every pool, purging all of their contents then removing them from the MasterObjectPooler.
        /// </summary>
        public void DestroyAllPools()
        {
            foreach (ObjectPool pool in _poolTable.Values)
            {
                DestroyPoolInternal(pool);
            }

            _poolTable.Clear();
        }

        /// <summary>
        /// Destroys a specified pool, purging its contents and removing it from the MasterObjectPooler.
        /// </summary>
        /// <param name="poolName">The pool to destroy.</param>
        public void DestroyPool(string poolName)
        {
            ObjectPool pool = GetPool(poolName);
            DestroyPoolInternal(pool);
            _poolTable.Remove(poolName);
        }
        #endregion

        #region GetObject/Component
        /// <summary>
        /// Gets an object from the specified pool.
        /// </summary>
        /// <param name="poolName">The name of the pool to get an object from.</param>
        /// <returns>The retrieved object.</returns>
        public GameObject GetObject(string poolName)
        {
            return GetPool(poolName).GetObject();
        }

        /// <summary>
        /// Gets an object from the specified pool.
        /// </summary>
        /// <param name="poolName">The name of the pool to get an object from.</param>
        /// <param name="position">The position to set the object to.</param>
        /// <returns>The retrieved object.</returns>
        public GameObject GetObject(string poolName, Vector3 position)
        {
            return GetPool(poolName).GetObject(position);
        }

        /// <summary>
        /// Gets an object from the specified pool.
        /// </summary>
        /// <param name="poolName">The name of the pool to get an object from.</param>
        /// <param name="position">The position to set the object to.</param>
        /// <param name="rotation">The rotation to set the object to.</param>
        /// <returns>The retrieved object.</returns>
        public GameObject GetObject(string poolName, Vector3 position, Quaternion rotation)
        {
            return GetPool(poolName).GetObject(position, rotation);
        }

        /// <summary>
        /// Gets an object from the specified pool, and then retrieves the specified component using a cache to improve performance.
        /// Note: this should not be used if multiple components of the same type exist on the object, or if the component will be dynamically removed/added at runtime.
        /// </summary>
        /// <typeparam name="T">The component type to get.</typeparam>
        /// <param name="poolName">The name of the pool to get the component from.</param>
        /// <returns>The retrieved component.</returns>
        public T GetObjectComponent<T>(string poolName) where T : class
        {
            return GetPool(poolName).GetObjectComponent<T>();
        }

        /// <summary>
        /// Gets an object from the specified pool, and then retrieves the specified component using a cache to improve performance.
        /// Note: this should not be used if multiple components of the same type exist on the object, or if the component will be dynamically removed/added at runtime.
        /// </summary>
        /// <typeparam name="T">The component type to get.</typeparam>
        /// <param name="poolName">The name of the pool to get the component from.</param>
        /// <param name="position">The position to set the object to.</param>
        /// <returns>The retrieved component.</returns>
        public T GetObjectComponent<T>(string poolName, Vector3 position) where T : class
        {
            return GetPool(poolName).GetObjectComponent<T>(position);
        }

        /// <summary>
        /// Gets an object from the specified pool, and then retrieves the specified component using a cache to improve performance.
        /// Note: this should not be used if multiple components of the same type exist on the object, or if the component will be dynamically removed/added at runtime.
        /// </summary>
        /// <typeparam name="T">The component type to get.</typeparam>
        /// <param name="poolName">The name of the pool to get the component from.</param>
        /// <param name="position">The position to set the object to.</param>
        /// <param name="rotation">The rotation to set the object to.</param>
        /// <returns>The retrieved component.</returns>
        public T GetObjectComponent<T>(string poolName, Vector3 position, Quaternion rotation) where T : class
        {
            return GetPool(poolName).GetObjectComponent<T>(position, rotation);
        }
        #endregion

        #region Release/Destroys
        /// <summary>
        /// Releases an object and returns it back to the specified pool, effectively 'destroying' it from the scene.
        /// Pool equivalent of Destroy.
        /// </summary>
        /// <param name="obj">The object to release.</param>
        /// <param name="poolName">The name of the pool to return the object to.</param>
        public void Release(GameObject obj, string poolName)
        {
            GetPool(poolName).Release(obj);
        }

        /// <summary>
        /// Releases a collection of objects and returns them back to the specified pool, effectively 'destroying' them from the scene.
        /// </summary>
        /// <param name="objs">the objects to release.</param>
        /// <param name="poolName">The name of the pool to return the objects to.</param>
        public void Release(IEnumerable<GameObject> objs, string poolName)
        {
            GetPool(poolName).Release(objs);
        }

        /// <summary>
        /// Releases every active object in the specified pool.
        /// </summary>
        /// <param name="poolName">The name of the pool.</param>
        public void ReleaseAll(string poolName)
        {
            GetPool(poolName).ReleaseAll();
        }

        /// <summary>
        /// Forcibly destroys the object and does not return it to a pool.
        /// </summary>
        /// <param name="obj">The object to destroy.</param>
        public void Destroy(GameObject obj) { Destroy(obj, obj.name); }

        /// <summary>
        /// Forcibly destroys the object and does not return it to a pool.
        /// </summary>
        /// <param name="obj">The object to destroy.</param>
        /// <param name="poolName">The name of the pool that the object belonged to.</param>
        public void Destroy(GameObject obj, string poolName)
        {
            ObjectPool pool = GetPool(poolName);
            if (pool) { pool.Destroy(obj); }
            else { Object.Destroy(obj); }
        }

        /// <summary>
        /// Forcibly destroys a collection of objects and does not return them to a pool.
        /// </summary>
        /// <param name="objs">The objects to destroy.</param>
        /// <param name="poolName">The name of the pool that the objects belonged to.</param>
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

        /// <summary>
        /// Releases every active object in every pool.
        /// </summary>
        public void ReleaseAllInAllPools()
        {
            foreach (ObjectPool pool in _poolTable.Values)
            {
                pool.ReleaseAll();
            }
        }
        #endregion

        #region Miscellaneous
        /// <summary>
        /// Populates the specified pool with the specified number of objects, so that they do not need instantiating later.
        /// </summary>
        /// <param name="poolName">The name of the pool to populate.</param>
        /// <param name="quantity">The number of objects to populate it with.</param>
        /// <param name="method">The population mode.</param>
        public void Populate(string poolName, int quantity, PopulateMethod method = PopulateMethod.Set)
        {
            GetPool(poolName).Populate(quantity, method);
        }

        /// <summary>
        /// Destroys every object in the specified pool, both alive and pooled.
        /// </summary>
        /// <param name="poolName">The name of the pool to populate.</param>
        public void Purge(string poolName)
        {
            GetPool(poolName).Purge();
        }

        /// <summary>
        /// Destroys every object in every pool, both alive and pooled.
        /// </summary>
        public void PurgeAll()
        {
            foreach (ObjectPool pool in _poolTable.Values)
            {
                pool.Purge();
            }
        }

        /// <summary>
        /// Gets all active objects in the specified pool.
        /// </summary>
        /// <param name="poolName">The name of the pool to populate.</param>
        /// <returns>The active objects.</returns>
        public IEnumerable<GameObject> GetAllActiveObjects(string poolName)
        {
            return GetPool(poolName).GetAllActiveObjects();
        }
        #endregion
    }
}
