using UnityEngine;

namespace QFSW.MOP2.Demo
{
    [RequireComponent(typeof(Collider))]
    public class ReleaseOnCollision : PoolableMonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            Release();
        }
    }
}
