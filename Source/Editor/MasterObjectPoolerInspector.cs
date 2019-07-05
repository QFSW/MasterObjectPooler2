using UnityEditor;
using UnityEngine;

namespace QFSW.MOP2.Editor
{
    [CustomEditor(typeof(MasterObjectPooler))]
    public class MasterObjectPoolerInspector : UnityEditor.Editor
    {
        [SerializeField] private Texture2D _banner = null;

        public override void OnInspectorGUI()
        {
            EditorHelpers.DrawBanner(_banner);
            base.OnInspectorGUI();
        }
    }
}
