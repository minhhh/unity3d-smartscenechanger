using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SSC
{

    public class SetAssetBundleNameWindow : EditorWindow
    {

        protected class BoolAndFilePath
        {

            public bool toggle = true;
            public string filePath = "";

            public BoolAndFilePath(bool _toggle, string _filePath)
            {
                this.toggle = _toggle;
                this.filePath = _filePath;
            }

        }

        /// <summary>
        /// Previous opened folder path
        /// </summary>
        string m_previousSelectedFolderPath = "";

        /// <summary>
        /// variant
        /// </summary>
        string m_variant = "";

        /// <summary>
        /// Toggle and target file path
        /// </summary>
        List<BoolAndFilePath> m_targetPaths = new List<BoolAndFilePath>();

        /// <summary>
        /// Scroll pos
        /// </summary>
        protected Vector2 m_scrollPos = Vector2.zero;

        /// <summary>
        /// OnGUI
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        protected virtual void OnGUI()
        {

            this.m_scrollPos = EditorGUILayout.BeginScrollView(this.m_scrollPos);

            // select folder
            {

                if (GUILayout.Button("Select Target Folder", GUILayout.MinHeight(30)))
                {

                    string path = EditorUtility.OpenFolderPanel("Select Target Folder", this.m_previousSelectedFolderPath, "");

                    if (!string.IsNullOrEmpty(path))
                    {

                        this.m_previousSelectedFolderPath = path;

                        if (path.StartsWith(Application.dataPath))
                        {
                            this.listTargetFilePaths(path);
                        }

                        else
                        {
                            Debug.LogError("Select a folder in this project");
                        }

                    }

                }

            }

            // select folder
            {

                GUILayout.Space(30.0f);

                this.m_variant = EditorGUILayout.TextField("Variant", this.m_variant);

                GUILayout.Space(30.0f);

            }

            // list label
            {

                GUILayout.Label("\n---------------------------------\n");

                if(this.m_targetPaths.Count <= 0)
                {
                    GUILayout.Label("(Empty)");
                }

                else
                {

                    int size = this.m_targetPaths.Count;

                    for(int i = 0; i < size; i++)
                    {

                        EditorGUILayout.BeginHorizontal(GUI.skin.box);

                        this.m_targetPaths[i].toggle = EditorGUILayout.Toggle(this.m_targetPaths[i].toggle, GUILayout.MaxWidth(30));

                        GUILayout.Label(this.m_targetPaths[i].filePath);

                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("S", GUILayout.MaxWidth(30)))
                        {
                            Selection.activeObject = AssetDatabase.LoadAssetAtPath(this.m_targetPaths[i].filePath, typeof(UnityEngine.Object));
                        }

                        EditorGUILayout.EndHorizontal();

                    }

                }

                GUILayout.Label("\n---------------------------------\n");

            }

            // start
            {

                GUILayout.FlexibleSpace();

                EditorGUILayout.BeginHorizontal(GUI.skin.box);

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Start", GUILayout.MinHeight(30), GUILayout.MaxWidth(150)))
                {

                    if(this.m_targetPaths.Count > 0)
                    {

                        if(EditorUtility.DisplayDialog("Confirmation", "You cannot undo this action.\n\nContinue ?", "OK", "Cancel"))
                        {
                            this.startSettingAssetBundleNames();
                            EditorUtility.DisplayDialog("Confirmation", "Done.", "OK");
                        }
                        
                    }

                    else
                    {
                        EditorUtility.DisplayDialog("Warning", "Empty targets", "OK");
                    }
                    
                }

                EditorGUILayout.EndHorizontal();

            }

            EditorGUILayout.EndScrollView();

        }

        /// <summary>
        /// List target file paths
        /// </summary>
        /// <param name="targetFolderPath">target folder</param>
        // -----------------------------------------------------------------------------------------------
        void listTargetFilePaths(string targetFolderPath)
        {

            this.m_targetPaths.Clear();

            try
            {

                string[] files = Directory.GetFiles(targetFolderPath, "*.*", SearchOption.AllDirectories);

                string temp = "";

                foreach (string path in files)
                {

                    temp = path.Replace(Application.dataPath, "Assets");

                    if (AssetImporter.GetAtPath(temp))
                    {
                        this.m_targetPaths.Add(new BoolAndFilePath(true, temp));
                    }

                }

            }

            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

        }

        /// <summary>
        /// Start
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        void startSettingAssetBundleNames()
        {

            AssetImporter ai = null;

            string temp = "";

            foreach (var val in this.m_targetPaths)
            {

                if (!val.toggle)
                {
                    continue;
                }

                ai = AssetImporter.GetAtPath(val.filePath);

                if(ai)
                {

                    temp = val.filePath.Remove(0, 7).ToLower().Replace(' ', '_');
                    temp = Path.ChangeExtension(temp, ".unity3d");

                    ai.assetBundleName = temp;
                    ai.assetBundleVariant = this.m_variant;

                }

            }

        }

    }

}
