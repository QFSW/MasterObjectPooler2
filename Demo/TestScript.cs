using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFSW.MOP2
{
    public class TestScript : MonoBehaviour
    {
        [SerializeField] private ObjectPool _pool = null;

        private void Start()
        {
            _pool.Initialize();
        }

        void Update()
        {
            if (Input.GetKey("i"))
            {
                _pool.GetObject(3 * new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
            }
        }
    }
}
