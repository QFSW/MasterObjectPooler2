using UnityEngine;

namespace QFSW.MOP2.Demo
{
    [RequireComponent(typeof(Collider))]
    public class ReleaseOnCollision : PoolableMonoBehaviour
    {
        [SerializeField] private LayerMask _collisionLayer = 0;
        [SerializeField] private ObjectPool _deathFX = null;

        private void OnCollisionEnter(Collision collision)
        {
            if ((_collisionLayer.value & 1 << collision.gameObject.layer) != 0)
            {
                Release();
                if (_deathFX)
                {
                    _deathFX.GetObject(transform.position);
                }
            }
        }
    }
}
