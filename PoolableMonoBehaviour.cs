using UnityEngine;

namespace QFSW.MOP2
{
    public class PoolableMonoBehaviour : MonoBehaviour, IPoolable
    {
        protected bool PoolReady => _parentPool;

        [SerializeField]
        [HideInInspector]
        private ObjectPool _parentPool;

        void IPoolable.InitializeTemplate(ObjectPool pool)
        {
            _parentPool = pool;
        }

        public void Release()
        {
            _parentPool.Release(gameObject);
        }
    }
}
