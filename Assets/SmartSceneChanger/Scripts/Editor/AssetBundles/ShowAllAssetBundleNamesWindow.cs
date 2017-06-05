using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SSC
{

    /// <summary>
    /// Show all AssetBundle names window
    /// </summary>
    public class ShowAllAssetBundleNamesWindow : EditorWindow
    {



        /// <summary>
        /// Scroll pos
        /// </summary>
        Vector2 m_scrollPos = Vector2.zero;

        List<string> m_allAssetPaths = new List<string>();

        Dictionary<string, AssetImporter> m_pathAndAI = new Dictionary<string, AssetImporter>();

        /// <summary>
        /// OnGUI
        /// </summary>
        // -----------------------------------------------------------------------------------------
        void OnGUI()
        {

            this.m_scrollPos = EditorGUILayout.BeginScrollView(this.m_scrollPos);

            if (GUILayout.Button("Show All AssetBundle Names", GUILayout.MinHeight(30)))
            {

                this.m_allAssetPaths = new List<string>(AssetDatabase.GetAllAssetPaths());

                this.m_allAssetPaths.Sort();

                AssetImporter ai = null;

                foreach (string path in this.m_allAssetPaths)
                {

                    if(!path.StartsWith("Assets/"))
                    {
                        continue;
                    }

                    ai = AssetImporter.GetAtPath(path);

                    if(ai && !string.IsNullOrEmpty(ai.assetBundleName))
                    {
                        this.m_pathAndAI.Add(path, ai);
                    }
                    
                }

            }

            GUILayout.Space(30.0f);

            foreach (var kv in this.m_pathAndAI)
            {

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.SelectableLabel(kv.Key, GUILayout.MaxHeight(20));
                EditorGUILayout.SelectableLabel(kv.Value.assetBundleName, GUILayout.MaxHeight(20));
                EditorGUILayout.SelectableLabel(kv.Value.assetBundleVariant, GUILayout.MaxHeight(20));

                EditorGUILayout.EndHorizontal();

            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndScrollView();

        }

    }

}
