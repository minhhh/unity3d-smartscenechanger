using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SSC
{

    /// <summary>
    /// Create starter window
    /// </summary>
    public class CreateStarterWindow : EditorWindow
    {

        protected class TempInfo
        {
            public string newClassName = "";
            public TextAsset oriTextAsset = null;
            public string oriTextAssetPath = "";

            public TempInfo(string _newClassName, string _oriTextAssetPath)
            {
                this.newClassName = _newClassName;
                this.oriTextAssetPath = _oriTextAssetPath;
            }

        }

        /// <summary>
        /// Scroll pos
        /// </summary>
        protected Vector2 m_scrollPos = Vector2.zero;

        /// <summary>
        /// Previous folder path for dialog
        /// </summary>
        protected string m_previousFolderPath = "";

        /// <summary>
        /// Overwrite Yes to All
        /// </summary>
        protected bool m_overwriteYesTAll = false;

        /// <summary>
        /// namespace
        /// </summary>
        protected string m_namespace = "YourProject";

        /// <summary>
        /// AssetBundleStartupManager TempInfo
        /// </summary>
        protected TempInfo m_AssetBundleStartupManager = new TempInfo(
            "CustomAssetBundleStartupManager",
            "Assets/SmartSceneChanger/NotResources/TextAsset/SampleAssetBundleStartupManager.txt"
            );

        /// <summary>
        /// DialogManager TempInfo
        /// </summary>
        protected TempInfo m_DialogManager = new TempInfo(
            "CustomDialogManager",
            "Assets/SmartSceneChanger/NotResources/TextAsset/SampleDialogManager.txt"
            );

        /// <summary>
        /// AssetBundleStartupManager TempInfo
        /// </summary>
        protected TempInfo m_IEnumeratorStartupManager = new TempInfo(
            "CustomIEnumeratorStartupManager",
            "Assets/SmartSceneChanger/NotResources/TextAsset/SampleIEnumeratorStartupManager.txt"
            );

        /// <summary>
        /// SceneChangeManager TempInfo
        /// </summary>
        protected TempInfo m_SceneChangeManager = new TempInfo(
            "CustomSceneChangeManager",
            "Assets/SmartSceneChanger/NotResources/TextAsset/SampleSceneChangeManager.txt"
            );

        /// <summary>
        /// WwwStartupManager TempInfo
        /// </summary>
        protected TempInfo m_WwwStartupManager = new TempInfo(
            "CustomWwwStartupManager",
            "Assets/SmartSceneChanger/NotResources/TextAsset/SampleWwwStartupManager.txt"
            );


        /// <summary>
        /// Init
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        public void init()
        {

            this.m_AssetBundleStartupManager.oriTextAsset = AssetDatabase.LoadAssetAtPath(this.m_AssetBundleStartupManager.oriTextAssetPath, typeof(TextAsset)) as TextAsset;
            this.m_DialogManager.oriTextAsset = AssetDatabase.LoadAssetAtPath(this.m_DialogManager.oriTextAssetPath, typeof(TextAsset)) as TextAsset;
            this.m_IEnumeratorStartupManager.oriTextAsset = AssetDatabase.LoadAssetAtPath(this.m_IEnumeratorStartupManager.oriTextAssetPath, typeof(TextAsset)) as TextAsset;
            this.m_SceneChangeManager.oriTextAsset = AssetDatabase.LoadAssetAtPath(this.m_SceneChangeManager.oriTextAssetPath, typeof(TextAsset)) as TextAsset;
            this.m_WwwStartupManager.oriTextAsset = AssetDatabase.LoadAssetAtPath(this.m_WwwStartupManager.oriTextAssetPath, typeof(TextAsset)) as TextAsset;

            this.printErrorIfInitFailed(this.m_AssetBundleStartupManager);
            this.printErrorIfInitFailed(this.m_DialogManager);
            this.printErrorIfInitFailed(this.m_IEnumeratorStartupManager);
            this.printErrorIfInitFailed(this.m_SceneChangeManager);
            this.printErrorIfInitFailed(this.m_WwwStartupManager);

        }

        /// <summary>
        /// Print errors
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        protected void printErrorIfInitFailed(TempInfo tempInfo)
        {

            if(!tempInfo.oriTextAsset)
            {
                Debug.LogError("Not found : " + tempInfo.oriTextAssetPath);
            }

        }

        /// <summary>
        /// Start copy
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        protected void startCopy(string folderPathToCopy)
        {

            this.m_overwriteYesTAll = false;

            this.init();

            this.copyTextAsset(this.m_AssetBundleStartupManager, folderPathToCopy);
            this.copyTextAsset(this.m_DialogManager, folderPathToCopy);
            this.copyTextAsset(this.m_IEnumeratorStartupManager, folderPathToCopy);
            this.copyTextAsset(this.m_SceneChangeManager, folderPathToCopy);
            this.copyTextAsset(this.m_WwwStartupManager, folderPathToCopy);

            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Confirmation", "Done.", "OK");

        }

        // -----------------------------------------------------------------------------------------------
        protected void copyTextAsset(TempInfo tempInfo, string folderPathToCopy)
        {

            if(!tempInfo.oriTextAsset)
            {
                Debug.LogError("Empty TextAsset : " + tempInfo.oriTextAssetPath);
                return;
            }

            // -------------------------------

            string copyTargetPath = folderPathToCopy + "/" + tempInfo.newClassName + ".cs";

            if(File.Exists(copyTargetPath))
            {

                if(!this.m_overwriteYesTAll)
                {

                    int val = EditorUtility.DisplayDialogComplex("Confirmation", "Overwrite ?\n\n" + copyTargetPath, "Yes", "No", "Yes to All");

                    if (val == 1)
                    {
                        return;
                    }

                    else if (val == 2)
                    {
                        this.m_overwriteYesTAll = true;
                    }

                }

            }

            // WriteAllText
            {

                try
                {

                    string text = tempInfo.oriTextAsset.text;

                    text = text.Replace("#NAMESPACE#", this.m_namespace);

                    text = text.Replace("#CLASSNAME#", tempInfo.newClassName);

                    File.WriteAllText(copyTargetPath, text);

                }

                catch(Exception e)
                {
                    Debug.LogError(e.Message);
                }


            }

        }

        /// <summary>
        /// OnGUI
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        protected virtual void OnGUI()
        {

            this.m_scrollPos = EditorGUILayout.BeginScrollView(this.m_scrollPos);

            GUILayout.Space(30.0f);

            EditorGUILayout.HelpBox("Create custom SSC manager scripts", MessageType.Info);

            GUILayout.Space(30.0f);

            //
            {

                float temp = EditorGUIUtility.labelWidth;

                EditorGUIUtility.labelWidth = 300.0f;

                this.m_namespace = EditorGUILayout.TextField("namespace : ", this.m_namespace);

                GUILayout.Space(30.0f);

                this.m_AssetBundleStartupManager.newClassName = EditorGUILayout.TextField("AssetBundleStartupManager class name : ", this.m_AssetBundleStartupManager.newClassName);
                this.m_DialogManager.newClassName = EditorGUILayout.TextField("DialogManager class name : ", this.m_DialogManager.newClassName);
                this.m_IEnumeratorStartupManager.newClassName = EditorGUILayout.TextField("IEnumeratorStartupManager class name : ", this.m_IEnumeratorStartupManager.newClassName);
                this.m_SceneChangeManager.newClassName = EditorGUILayout.TextField("SceneChangeManager class name : ", this.m_SceneChangeManager.newClassName);
                this.m_WwwStartupManager.newClassName = EditorGUILayout.TextField("WwwStartupManager class name : ", this.m_WwwStartupManager.newClassName);

                EditorGUIUtility.labelWidth = temp;

            }
            

            // create
            {

                GUILayout.FlexibleSpace();

                EditorGUILayout.BeginHorizontal(GUI.skin.box);

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Create", GUILayout.MinHeight(30), GUILayout.MaxWidth(150)))
                {

                    string path = EditorUtility.OpenFolderPanel("Folder to save", this.m_previousFolderPath, "");

                    if (!string.IsNullOrEmpty(path))
                    {

                        this.m_previousFolderPath = path;

                        if (path.StartsWith(Application.dataPath))
                        {
                            this.startCopy(path);
                        }

                        else
                        {
                            EditorUtility.DisplayDialog("Error", "Out of Assets folder", "OK");
                        }

                    }

                }

                EditorGUILayout.EndHorizontal();

            }

            EditorGUILayout.EndScrollView();

        }

    }

}