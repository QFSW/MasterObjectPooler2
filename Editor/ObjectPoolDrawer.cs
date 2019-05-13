using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace QFSW.MOP2.Editor
{
    //[CustomPropertyDrawer(typeof(ObjectPool), true)]
    public class ObjectPoolDrawer : PropertyDrawer
    {
        private bool _expanded = false;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return -2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);
        }
    }
}
