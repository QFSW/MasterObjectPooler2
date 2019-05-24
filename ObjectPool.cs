using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace QFSW.MOP2
{
    [CreateAssetMenu(fileName = "Untitled Pool", menuName = "Master Object Pooler 2/Object Pool", order = 0)]
    public class ObjectPool : ScriptableObject
    {
        [SerializeField] private string _name = string.Empty;
        [SerializeField] private GameObject _template = null;
        [SerializeField] private int _defaultSize;
        [SerializeField] private int _maxSize = -1;
        [SerializeField] private bool _incrementalInstanceNames = false;


        public bool IncrementalInstanceNames
        {
            get => _incrementalInstanceNames;
            set => _incrementalInstanceNames = value;
        }
        public string PoolName => _name;

        public Transform ObjectParent
        {
            get
            {
                if (!_objectParent)
                {
                    _objectParent = new GameObject(string.Format("{0}Pool", _name)).transform;
                }

                return _objectParent;
            }
        }
        private Transform _objectParent;

        private bool HasMaxSize => _maxSize > 0;
        private bool HasPooledObjects => _pooledObjects.Count > 0;

        private int _instanceCounter = 0;

        #region Caches
        private readonly List<GameObject> _pooledObjects = new List<GameObject>();
        private readonly Dictionary<int, GameObject> _aliveObjects = new Dictionary<int, GameObject>();

        private readonly List<GameObject> _releaseAllBuffer = new List<GameObject>();
        private readonly Dictionary<Tuple2<int, Type>, object> _componentCache = new Dictionary<Tuple2<int, Type>, object>();
        #endregion

        #region Initialization/Creation
        public static ObjectPool Create(GameObject template, int defaultSize = 0, int maxSize = -1)
        {
            return Create(template, template.name, defaultSize, maxSize);
        }

        public static ObjectPool Create(GameObject template, string name, int defaultSize = 0, int maxSize = -1)
        {
            ObjectPool pool = CreateInstance<ObjectPool>();
            pool._name = name;
            pool._template = template;
            pool._defaultSize = defaultSize;
            pool._maxSize = maxSize;

            return pool;
        }

        private void OnEnable()
        {
            _instanceCounter = 0;
            SceneManager.sceneUnloaded += OnSceneUnload;
        }

        private void OnDisable()
        {
            SceneManager.sceneUnloaded -= OnSceneUnload;
        }

        public void Initialize()
        {
            if (string.IsNullOrWhiteSpace(_name))
            {
                _name = _template.name;
                ObjectParent.name = _name;
            }

            if (string.IsNullOrWhiteSpace(name)) { name = _name; }

            InitializeIPoolables();

            Populate(_defaultSize, PopulateMethod.Set);
        }

        private void InitializeIPoolables()
        {
            foreach (IPoolable poolable in _template.GetComponentsInChildren<IPoolable>())
            {
                poolable.InitializeTemplate(this);
            }
        }
        #endregion

        #region Internal
        private GameObject CreateNewObject()
        {
            GameObject newObj = Instantiate(_template);
            newObj.transform.parent = ObjectParent;

            if (_incrementalInstanceNames)
            {
                newObj.name = string.Format("{0}#{1:000}", _template.name, _instanceCounter);
            }
            else
            {
                newObj.name = _template.name;
            }

            _instanceCounter++;
            return newObj;
        }
        #endregion

        #region GetObject/Component
        public GameObject GetObject() { return GetObject(_template.transform.position); }
        public GameObject GetObject(Vector3 position) { return GetObject(position, _template.transform.rotation); }
        public GameObject GetObject(Vector3 position, Quaternion rotation)
        {
            GameObject obj;
            if (HasPooledObjects)
            {
                obj = _pooledObjects[_pooledObjects.Count - 1];
                _pooledObjects.RemoveAt(_pooledObjects.Count - 1);

                if (!obj)
                {
                    Debug.LogWarning(string.Format("Object in pool '{0}' was null or destroyed; it may have been destroyed externally. Attempting to retrieve a new object", _name));
                    return GetObject(position, rotation);
                }
            }
            else
            {
                obj = CreateNewObject();
            }

            obj.transform.position = position;
            obj.transform.rotation = rotation;

            obj.SetActive(true);

            _aliveObjects.Add(obj.GetInstanceID(), obj);
            return obj;
        }

        public T GetObjectComponent<T>() where T : class
        {
            return GetObjectComponent<T>(_template.transform.position);
        }

        public T GetObjectComponent<T>(Vector3 position) where T : class
        {
            return GetObjectComponent<T>(position, _template.transform.rotation);
        }

        public T GetObjectComponent<T>(Vector3 position, Quaternion rotation) where T : class
        {
            GameObject obj = GetObject(position, rotation);
            return GetObjectComponent<T>(obj);
        }

        public T GetObjectComponent<T>(GameObject obj) where T : class
        {
            Tuple2<int, Type> key = new Tuple2<int, Type>(obj.GetInstanceID(), typeof(T));
            T component;

            if (_componentCache.ContainsKey(key))
            {
                component = _componentCache[key] as T;
                if (component == null) { _componentCache.Remove(key); }
                else { return _componentCache[key] as T; }
            }

            component = obj.GetComponent<T>();
            if (component != null) { _componentCache[key] = component; }
            return component;
        }
        #endregion

        #region Release/Destroy
        public void Release(GameObject obj)
        {
            if (!_aliveObjects.Remove(obj.GetInstanceID()))
            {
                Debug.LogWarning(string.Format("Object '{0}' could not be found in pool '{1}'; it may have already been released.", obj, _name));
                return;
            }

            if (obj)
            {
                if (HasMaxSize && _pooledObjects.Count >= _maxSize)
                {
                    Object.Destroy(obj);
                }
                else
                {
                    _pooledObjects.Add(obj);
                    obj.SetActive(false);
                }
            }
        }

        public void Release(IEnumerable<GameObject> objs)
        {
            foreach (GameObject obj in objs)
            {
                Release(obj);
            }
        }

        public void ReleaseAll()
        {
            _releaseAllBuffer.Clear();
            _releaseAllBuffer.AddRange(_aliveObjects.Values);
            Release(_releaseAllBuffer);
        }

        public void Destroy(GameObject obj)
        {
            _aliveObjects.Remove(obj.GetInstanceID());
            Object.Destroy(obj);
        }

        public void Destroy(IEnumerable<GameObject> objs)
        {
            foreach (GameObject obj in objs)
            {
                Destroy(obj);
            }
        }
        #endregion

        #region Miscellaneous
        public void Populate(int quantity, PopulateMethod method = PopulateMethod.Set)
        {
            int newObjCount;
            switch (method)
            {
                case PopulateMethod.Set: newObjCount = quantity - _pooledObjects.Count; break;
                case PopulateMethod.Add: newObjCount = quantity; break;
                default: newObjCount = 0; break;
            }

            if (HasMaxSize) { newObjCount = Mathf.Min(newObjCount, _maxSize - _pooledObjects.Count); }
            if (newObjCount < 0) { newObjCount = 0; }

            for (int i = 0; i < newObjCount; i++)
            {
                GameObject newObj = CreateNewObject();
                newObj.SetActive(false);
                _pooledObjects.Add(newObj);
            }
        }

        public void Purge()
        {
            foreach (GameObject obj in _pooledObjects) { Object.Destroy(obj); }
            foreach (GameObject obj in _aliveObjects.Values) { Object.Destroy(obj); }
            _pooledObjects.Clear();
            _aliveObjects.Clear();
            _componentCache.Clear();
        }
        #endregion

        #region Callbacks
        private void OnSceneUnload(Scene scene)
        {
            if (!_objectParent)
            {
                _pooledObjects.Clear();
                _aliveObjects.Clear();
                _componentCache.Clear();
            }
            else
            {
                _pooledObjects.RemoveAll(x => !x);
            }
        }
        #endregion
    }
}
