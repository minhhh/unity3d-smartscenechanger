using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

namespace SSC
{

    /// <summary>
    /// Class for loading scene shortcut in editor
    /// </summary>
    public class EditorSceneLoaderWindow : EditorWindow
    {

        /// <summary>
        /// Scroll pos
        /// </summary>
        Vector2 m_scrollPos = Vector2.zero;

        /// <summary>
        /// show only available in build settings
        /// </summary>
        bool m_onlyAvailable = true;

        /// <summary>
        /// OnGUI
        /// </summary>
        // -----------------------------------------------------------------------------------------
        void OnGUI()
        {

            this.m_scrollPos = EditorGUILayout.BeginScrollView(this.m_scrollPos);

            // m_onlyAvailable
            {
                this.m_onlyAvailable = GUILayout.Toggle(this.m_onlyAvailable, "Only Available Scene");
            }

            // list
            {

                foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
                {

                    if (!this.m_onlyAvailable || (this.m_onlyAvailable && scene.enabled))
                    {

                        GUILayout.BeginHorizontal();

                        if (GUILayout.Button(Path.GetFileNameWithoutExtension(scene.path)))
                        {

                            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                            {
                                EditorSceneManager.OpenScene(scene.path);
                            }

                        }

                        if (GUILayout.Button("S", GUILayout.Width(40.0f)))
                        {
                            Selection.activeObject = AssetDatabase.LoadAssetAtPath(scene.path, typeof(Object));
                        }

                        GUILayout.EndHorizontal();

                    }

                }

            }

            EditorGUILayout.EndScrollView();

        }

    }

}
