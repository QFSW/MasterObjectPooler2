using UnityEditor;
using UnityEngine;

namespace QFSW.MOP2.Editor
{
    [CustomEditor(typeof(ObjectPool))]
    public class ObjectPoolInspector : MOPInspectorBase
    {
        private ObjectPool _objectPool;

        protected override void OnEnable()
        {
            base.OnEnable();

            _objectPool = (ObjectPool)target;
        }

        public override void OnInspectorGUI()
        {
            EditorHelpers.DrawBanner(Banner);
            
            if (Application.isPlaying)
            {
                if (!_objectPool.Initialized)
                {
                    EditorGUILayout.HelpBox("Pool has not been initialized: if you attempt to use the pool before initializing it, unintended behaviour may occur", MessageType.Warning);
                }
            }

            DrawDefaultInspector();
        }
    }
}
