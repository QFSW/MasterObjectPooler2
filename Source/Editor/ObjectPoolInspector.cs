using UnityEditor;
using UnityEngine;

namespace QFSW.MOP2.Editor
{
    [CustomEditor(typeof(ObjectPool))]
    public class ObjectPoolInspector : UnityEditor.Editor
    {
        [SerializeField] private Texture2D _banner = null;

        public override void OnInspectorGUI()
        {
            EditorHelpers.DrawBanner(_banner);
            base.OnInspectorGUI();
        }
    }
}
