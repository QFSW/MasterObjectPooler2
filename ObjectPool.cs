using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFSW.MOP2
{
    public class ObjectPool : ScriptableObject
    {
        [SerializeField] private string _name = string.Empty;
        [SerializeField] private GameObject _template = null;
        [SerializeField] private int _defaultSize;
        [SerializeField] private int _maxSize = -1;

        private readonly List<GameObject> _pooledObjects = new List<GameObject>();
        private readonly Dictionary<int, GameObject> _aliveObjects = new Dictionary<int, GameObject>();

        public static ObjectPool Create(GameObject template, int defaultSize = 0, int maxSize = -1)
        {
            string name = template.name;
            return Create(template, name, defaultSize, maxSize);
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
