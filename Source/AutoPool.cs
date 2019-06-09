using UnityEngine;

namespace QFSW.MOP2
{
    public class AutoPool : PoolableMonoBehaviour
    {
        [SerializeField] private float _poolTimer = 1;
        [SerializeField] private bool _scaledTime = true;

        private float _elapsedTime;

        private void OnEnable()
        {
            _elapsedTime = 0;
        }

        private void Update()
        {
            if (_scaledTime) { _elapsedTime += Time.deltaTime; }
            else { _elapsedTime += Time.unscaledDeltaTime; }

            if (_elapsedTime > _poolTimer && PoolReady) { Release(); }
        }
    }
}
