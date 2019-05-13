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
                pool.Initialize();
                if (_poolTable.ContainsKey(pool.PoolName))
                {
                    Debug.LogWarning(string.Format("{0} could not be added to the pool table as a pool with the same name already exists", pool.PoolName));
                }
                else
                {
                    _poolTable.Add(pool.PoolName, pool);
                }
            }
        }
    }
}
