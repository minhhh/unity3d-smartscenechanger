using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SSC
{

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {

        /// <summary>
        /// OnGUI
        /// </summary>
        /// <param name="position">Rect</param>
        /// <param name="property">SerializedProperty</param>
        /// <param name="label">GUIContent</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }

    }

}
