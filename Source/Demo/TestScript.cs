using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFSW.MOP2
{
    public class TestScript : MonoBehaviour
    {
        [SerializeField] private MasterObjectPooler _pooler = null;

        void Update()
        {
            if (Input.GetKey("i"))
            {
                Vector3 spawnPos = 3 * new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                SpriteRenderer rend = _pooler.GetObjectComponent<SpriteRenderer>("Test", spawnPos);
                rend.color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
            }
            if (Input.GetKeyDown("u"))
            {
                _pooler.ReleaseAll("Test");
            }
        }
    }
}
