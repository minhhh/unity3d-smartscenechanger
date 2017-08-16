using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SSC
{

    [CustomPropertyDrawer(typeof(UiTypeAndIdentifier))]
    public class UiTypeAndIdentifierDrawer : PropertyDrawer
    {

        /// <summary>
        /// OnGUI
        /// </summary>
        /// <param name="position">Rect</param>
        /// <param name="property">SerializedProperty</param>
        /// <param name="label">GUIContent</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var leftRect = new Rect(position.xMin, position.y, position.width / 2, position.height);
            var rightRect = new Rect(position.xMin + (position.width / 2), position.y, position.width / 2, position.height);

            EditorGUI.PropertyField(leftRect, property.FindPropertyRelative("uiType"), GUIContent.none);
            EditorGUI.PropertyField(rightRect, property.FindPropertyRelative("uiIdentifier"), GUIContent.none);

            EditorGUI.EndProperty();

        }

    }

}
