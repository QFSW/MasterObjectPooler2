using UnityEngine;

namespace QFSW.MOP2.Demo
{
    public class TriggerSpawner : MonoBehaviour
    {
        [SerializeField] ObjectPool _triggerPool = null;
        [SerializeField] float _spawnRate = 2;
        [SerializeField] float _spawnSpeed = 3;
        [SerializeField] float _spawnAngularSpeed = 6;

        private bool ShouldSpawn => Time.time > _lastSpawned + 1 / _spawnRate;

        private float _lastSpawned;

        private void Start()
        {
            _triggerPool.Initialize();
            _triggerPool.ObjectParent.parent = transform;
        }

        private void Update()
        {
            if (ShouldSpawn)
            {
                _lastSpawned = Time.time;
                Rigidbody rb = _triggerPool.GetObjectComponent<Rigidbody>(transform.position);
                rb.angularVelocity = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)) * _spawnAngularSpeed;
                rb.velocity = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)) * _spawnSpeed;
            }
        }
    }
}
