using System.Collections;
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

        private bool HasMaxSize => _maxSize > 0;
        private bool HasPooledObjects => _pooledObjects.Count > 0;

        private readonly List<GameObject> _pooledObjects = new List<GameObject>();
        private readonly Dictionary<int, GameObject> _aliveObjects = new Dictionary<int, GameObject>();

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

        private void Awake()
        {
            if (string.IsNullOrWhiteSpace(_name)) { _name = _template.name; }
        }

        public GameObject Resurrect() { return Resurrect(_template.transform.position); }
        public GameObject Resurrect(Vector3 position) { return Resurrect(position, _template.transform.rotation); }
        public GameObject Resurrect(Vector3 position, Quaternion rotation)
        {
            GameObject obj;
            if (HasPooledObjects)
            {
                obj = _pooledObjects[_pooledObjects.Count - 1];
                _pooledObjects.RemoveAt(_pooledObjects.Count - 1);

                if (!obj)
                {
                    Debug.LogWarning(string.Format("Object in pool '{0}' was null or destroyed; it may have been destroyed externally. Attempting to retrieve a new object", _name));
                    return Resurrect(position, rotation);
                }
            }
            else
            {
                obj = Instantiate(_template, position, rotation);
                obj.name = _template.name;
            }

            obj.transform.position = position;
            obj.transform.rotation = rotation;

            obj.SetActive(true);

            _aliveObjects.Add(obj.GetInstanceID(), obj);
            return obj;
        }

        public void Destroy(IEnumerable<GameObject> objs)
        {
            foreach (GameObject obj in objs)
            {
                Destroy(obj);
            }
        }

        public void Destroy(GameObject obj)
        {
            _aliveObjects.Remove(obj.GetInstanceID());
            Destroy(obj);
        }

        public void Purge()
        {
            foreach (GameObject obj in _pooledObjects) { Destroy(obj); }
            foreach (GameObject obj in _aliveObjects.Values) { Destroy(obj); }
            _pooledObjects.Clear();
            _aliveObjects.Clear();
        }
    }
}
