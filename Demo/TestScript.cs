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
                SpriteRenderer rend = _pool.GetObjectComponent<SpriteRenderer>(3 * new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
                rend.color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
            }
            if (Input.GetKeyDown("u")) { _pool.ReleaseAll(); }
        }
    }
}
