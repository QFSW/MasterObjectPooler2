using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFSW.MOP2
{
    public class MasterObjectPooler : MonoBehaviour
    {
        [SerializeField] private ObjectPool[] _pools = new ObjectPool[0];

        private void Start()
        {
            foreach (ObjectPool pool in _pools)
            {
                pool.Initialize();
            }
        }
    }
}
