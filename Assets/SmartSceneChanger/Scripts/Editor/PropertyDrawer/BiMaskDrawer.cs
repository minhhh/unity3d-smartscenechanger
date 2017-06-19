using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SSC
{

    [CustomPropertyDrawer(typeof(BiMaskAttribute))]
    public class BiMaskDrawer : PropertyDrawer
    {

        /// <summary>
        /// OnGUI
        /// </summary>
        /// <param name="position">Rect</param>
        /// <param name="property">SerializedProperty</param>
        /// <param name="label">GUIContent</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
        }

    }

}
