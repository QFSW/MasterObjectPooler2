using System.Collections.Generic;
using UnityEngine;

namespace QFSW.MOP2
{
    [CreateAssetMenu(fileName = "Untitled Pool", menuName = "Master Object Pooler 2/Object Pool", order = 0)]
    public class ObjectPool : ScriptableObject
    {
        [SerializeField] private string _name = string.Empty;
        [SerializeField] private GameObject _template = null;
        [SerializeField] private int _defaultSize;
        [SerializeField] private int _maxSize = -1;

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

        private readonly List<GameObject> _pooledObjects = new List<GameObject>();
        private readonly Dictionary<int, GameObject> _aliveObjects = new Dictionary<int, GameObject>();

        private readonly List<GameObject> _releaseAllBuffer = new List<GameObject>();

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

            pool.Initialize();
            return pool;
        }

        public void Initialize()
        {
            if (string.IsNullOrWhiteSpace(_name))
            {
                _name = _template.name;
                ObjectParent.name = _name;
            }

            Populate(_defaultSize, PopulateMethod.Set);
        }

        private GameObject CreateNewObject()
        {
            GameObject newObj = Instantiate(_template);
            newObj.name = _template.name;
            newObj.transform.parent = ObjectParent;
            return newObj;
        }

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

        public void Purge()
        {
            foreach (GameObject obj in _pooledObjects) { Object.Destroy(obj); }
            foreach (GameObject obj in _aliveObjects.Values) { Object.Destroy(obj); }
            _pooledObjects.Clear();
            _aliveObjects.Clear();
        }

        public T GetObject<T>()
        {
            return GetObject().GetComponent<T>();
        }

        public T GetObject<T>(Vector3 position)
        {
            return GetObject(position).GetComponent<T>();
        }

        public T GetObject<T>(Vector3 position, Quaternion rotation)
        {
            return GetObject(position, rotation).GetComponent<T>();
        }
    }
}
