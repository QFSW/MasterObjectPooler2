using UnityEngine;

namespace QFSW.MOP2
{
    /// <summary>
    /// Automatically releases an object after the specified amount of time has surpassed.
    /// </summary>
    [DisallowMultipleComponent]
    public class AutoPool : PoolableMonoBehaviour
    {
        [Tooltip("The duration of time to wait before releasing the object to the pool.")]
        [SerializeField] private float _poolTimer = 1;

        [Tooltip("Whether to use scaled or unscaled time.")]
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
