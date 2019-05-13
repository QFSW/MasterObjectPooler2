using UnityEngine;

namespace QFSW.MOP2
{
    public class AutoPool : MonoBehaviour, IPoolable
    {
        [SerializeField] private float _poolTimer = 1;
        [SerializeField] private bool _scaledTime = true;

        [SerializeField]
        [HideInInspector]
        private ObjectPool _parentPool;
        private float _elapsedTime;

        public void InitializeTemplate(ObjectPool pool)
        {
            _parentPool = pool;
        }

        private void OnEnable()
        {
            _elapsedTime = 0;
        }

        private void Update()
        {
            if (_scaledTime) { _elapsedTime += Time.deltaTime; }
            else { _elapsedTime += Time.unscaledDeltaTime; }

            if (_elapsedTime > _poolTimer && _parentPool) { _parentPool.Release(gameObject); }
        }
    }
}
