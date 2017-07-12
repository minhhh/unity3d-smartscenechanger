using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SSC
{

    [InitializeOnLoad]
    public class BuildTargetChanged
    {
        static BuildTargetChanged()
        {
            EditorUserBuildSettings.activeBuildTargetChanged -= onBuildTargetChanged;
            EditorUserBuildSettings.activeBuildTargetChanged += onBuildTargetChanged;
        }
        static void onBuildTargetChanged()
        {
            Debug.LogWarning("Don't forget to change Editor's Graphics API");
        }
    }

    /// <summary>
    /// Class for Build AssetBundles Window
    /// </summary>
    public class BuildAssetBundlesWindow : EditorWindow
    {

        /// <summary>
        /// BuildPlatforms
        /// </summary>
        [Serializable]
        public class BuildPlatforms
        {

            /// <summary>
            /// Build windows AssetBundles
            /// </summary>
            public bool buildWindows = true;

            /// <summary>
            /// Build android AssetBundles
            /// </summary>
            public bool builAndroid = true;

            /// <summary>
            /// Build ios AssetBundles
            /// </summary>
            public bool buildIos = true;

            /// <summary>
            /// Windows manifest name
            /// </summary>
            public string windowsManifestName = "windows.unity3d";

            /// <summary>
            /// Windows manifest name
            /// </summary>
            public string androidManifestName = "android.unity3d";

            /// <summary>
            /// Windows manifest name
            /// </summary>
            public string iosManifestName = "ios.unity3d";

            /// <summary>
            /// Windows manifest name
            /// </summary>
            public string windowsEncryptedManifestName = "windows.encrypted.unity3d";

            /// <summary>
            /// Windows manifest name
            /// </summary>
            public string androidEncryptedManifestName = "android.encrypted.unity3d";

            /// <summary>
            /// Windows manifest name
            /// </summary>
            public string iosEncryptedManifestName = "ios.encrypted.unity3d";

            /// <summary>
            /// Reset
            /// </summary>
            public void reset()
            {

                this.buildWindows = true;
                this.builAndroid = true;
                this.buildIos = true;

                this.windowsManifestName = "windows.unity3d";
                this.androidManifestName = "android.unity3d";
                this.iosManifestName = "ios.unity3d";

                this.windowsEncryptedManifestName = "windows.encrypted.unity3d";
                this.androidEncryptedManifestName = "android.encrypted.unity3d";
                this.iosEncryptedManifestName = "ios.encrypted.unity3d";

            }

        }

        /// <summary>
        /// EncryptionInfo
        /// </summary>
        [Serializable]
        public class EncryptionInfo
        {

            /// <summary>
            /// Temp AssetBundles folder for encryption
            /// </summary>
            public string tempAssetBundlesFolderPath = "Assets/AssetBundles";

            /// <summary>
            /// Temp AssetBundles folder for encryption
            /// </summary>
            public string tempEncryptedFolderPath = "Assets/Encrypted";

            /// <summary>
            /// Prefix of AssetBundleNames
            /// </summary>
            public string encryptedAssetBundlePrefix = "encrypted/";

            /// <summary>
            /// Use Encryption
            /// </summary>
            public bool useEncryption = true;

            /// <summary>
            /// password
            /// </summary>
            public string password = "PassworDPassworD";

            /// <summary>
            /// Reset
            /// </summary>
            public void reset()
            {
                this.tempAssetBundlesFolderPath = "Assets/AssetBundles";
                this.tempEncryptedFolderPath = "Assets/Encrypted";

                this.encryptedAssetBundlePrefix = "encrypted/";
                this.useEncryption = true;
                this.password = "PassworDPassworD";
            }

            /// <summary>
            /// Validate params
            /// </summary>
            public void validate()
            {

                if (this.tempAssetBundlesFolderPath.EndsWith("/"))
                {
                    this.tempAssetBundlesFolderPath = this.tempAssetBundlesFolderPath.Remove(this.tempAssetBundlesFolderPath.Length - 1);
                }

                if (this.tempEncryptedFolderPath.EndsWith("/"))
                {
                    this.tempEncryptedFolderPath = this.tempEncryptedFolderPath.Remove(this.tempEncryptedFolderPath.Length - 1);
                }

                if (this.tempAssetBundlesFolderPath != "Assets" && !this.tempAssetBundlesFolderPath.StartsWith("Assets/"))
                {
                    this.tempAssetBundlesFolderPath = "Assets/" + this.tempAssetBundlesFolderPath;
                }

                if (this.tempEncryptedFolderPath != "Assets" && !this.tempEncryptedFolderPath.StartsWith("Assets/"))
                {
                    this.tempEncryptedFolderPath = "Assets/" + this.tempEncryptedFolderPath;
                }

            }

        }
        /// <summary>
        /// OtherInfo
        /// </summary>
        [Serializable]
        public class OtherInfo
        {

            /// <summary>
            /// Convert variant to name
            /// </summary>
            public bool useFakeAssetBundleNames = true;

            /// <summary>
            /// Force rebuild
            /// </summary>
            public bool forceRebuild = true;

            /// <summary>
            /// Reset
            /// </summary>
            public void reset()
            {
                this.useFakeAssetBundleNames = true;
                this.forceRebuild = true;
            }

        }

        /// <summary>
        /// Output path
        /// </summary>
        protected string m_buildPath = "";

        /// <summary>
        /// Scroll pos
        /// </summary>
        protected Vector2 m_scrollPos = Vector2.zero;


        /// <summary>
        /// EncryptionInfo
        /// </summary>
        protected EncryptionInfo m_encryptionInfo = new EncryptionInfo();

        /// <summary>
        /// BuildPlatforms
        /// </summary>
        protected BuildPlatforms m_buildPlatforms = new BuildPlatforms();

        /// <summary>
        /// OtherInfo
        /// </summary>
        protected OtherInfo m_otherInfo = new OtherInfo();

        /// <summary>
        /// AssetBundles label and variant
        /// </summary>
        protected class LabelAndVariant
        {

            /// <summary>
            /// AssetBundle name
            /// </summary>
            public string assetBundleName = "";

            /// <summary>
            /// AssetBundle variant
            /// </summary>
            public string assetBundleVariant = "";

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="_assetBundleName">name</param>
            /// <param name="_assetBundleVariant">variant</param>
            public LabelAndVariant(string _assetBundleName, string _assetBundleVariant)
            {
                this.assetBundleName = _assetBundleName;
                this.assetBundleVariant = _assetBundleVariant;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        public BuildAssetBundlesWindow()
        {

        }

        /// <summary>
        /// Reset params
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        protected void resetSettings()
        {

            this.m_encryptionInfo.reset();
            this.m_buildPlatforms.reset();
            this.m_otherInfo.reset();

        }

        /// <summary>
        /// Return manifest name and BuildTarget 
        /// </summary>
        /// <returns>Dictionary</returns>
        // -----------------------------------------------------------------------------------------------
        protected virtual Dictionary<BuildTarget, string> manifestNameAndTarget()
        {

            Dictionary<BuildTarget, string> ret = new Dictionary<BuildTarget, string>();

            if (this.m_buildPlatforms.buildWindows)
            {
                ret.Add(BuildTarget.StandaloneWindows,
                    (this.m_encryptionInfo.useEncryption) ?
                    this.m_buildPlatforms.windowsEncryptedManifestName :
                    this.m_buildPlatforms.windowsManifestName
                    );
            }

            if (this.m_buildPlatforms.builAndroid)
            {
                ret.Add(BuildTarget.Android,
                    (this.m_encryptionInfo.useEncryption) ?
                    this.m_buildPlatforms.androidEncryptedManifestName :
                    this.m_buildPlatforms.androidManifestName
                    );
            }

            if (this.m_buildPlatforms.buildIos)
            {
                ret.Add(BuildTarget.iOS,
                    (this.m_encryptionInfo.useEncryption) ?
                    this.m_buildPlatforms.iosEncryptedManifestName :
                    this.m_buildPlatforms.iosManifestName
                    );
            }

            return ret;

        }

        /// <summary>
        /// Build AssetBundles by params
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        protected void build()
        {

            string buildPath = EditorUtility.OpenFolderPanel("Build AssetBundles", this.m_buildPath, "");

            if (string.IsNullOrEmpty(buildPath))
            {
                return;
            }

            // m_buildPath
            {
                this.m_buildPath = buildPath;
            }

            if (!this.ready())
            {
                return;
            }

            // -------------------------------

            bool ok = true;

            Dictionary<string, LabelAndVariant> fakes = null;

            if (this.m_otherInfo.useFakeAssetBundleNames)
            {
                fakes = this.setFakeAssetBundleInfos();
            }

            if (this.m_encryptionInfo.useEncryption)
            {

                ok = this.buildAssetBundles(this.m_encryptionInfo.tempAssetBundlesFolderPath, false);

                if (ok)
                {

                    ok = this.createEncryptedFiles();

                    if (ok)
                    {
                        this.buildAssetBundles(buildPath, true);
                    }

                }

            }

            else
            {
                ok = this.buildAssetBundles(buildPath, false);
            }

            if (this.m_otherInfo.useFakeAssetBundleNames)
            {
                this.revertAssetBundleInfos(fakes);
            }

            // savePrefs
            {
                this.savePrefs();
            }

            // forceRefresh
            {
                this.forceRefresh();
            }

            //EditorUtility.DisplayDialog("Build AssetBundles", ok ? "Success\n\nDon't forget to clear editor's cache if you needed." : "Failed", "OK");
            EditorUtility.DisplayDialog("Build AssetBundles", ok ? "Success" : "Failed", "OK");

        }

        /// <summary>
        /// Show confirmation window if needed 
        /// </summary>
        /// <returns></returns>
        // -----------------------------------------------------------------------------------------
        protected bool ready()
        {

            string overwrite = "";
            string remove_folder = "";
            string remove_unused = "■Remove Unused AssetBundleNames ? \n\n";

            if (this.m_encryptionInfo.useEncryption && Directory.Exists(this.m_encryptionInfo.tempAssetBundlesFolderPath))
            {
                overwrite = "■Overwrite ? \n" + this.m_encryptionInfo.tempAssetBundlesFolderPath + "\n\n";
            }

            if (Directory.Exists(this.m_encryptionInfo.tempEncryptedFolderPath))
            {
                remove_folder = "■Move Folder to Trash ? \n" + this.m_encryptionInfo.tempEncryptedFolderPath + "\n\n";
            }

            if (!EditorUtility.DisplayDialog("Build AssetBundles", overwrite + remove_folder + remove_unused, "Yes", "Cancel"))
            {
                return false;
            }

            // ----------------

            if (!string.IsNullOrEmpty(remove_folder))
            {
                if (!AssetDatabase.MoveAssetToTrash(m_encryptionInfo.tempEncryptedFolderPath))
                {
                    return false;
                }
                AssetDatabase.Refresh();
            }

            AssetDatabase.RemoveUnusedAssetBundleNames();

            return true;

        }

        /// <summary>
        /// Create encrypted AssetBundle files
        /// </summary>
        /// <returns>success</returns>
        // -----------------------------------------------------------------------------------------
        protected bool createEncryptedFiles()
        {

            try
            {

                foreach (string file in Directory.GetFiles(m_encryptionInfo.tempAssetBundlesFolderPath, "*", SearchOption.AllDirectories))
                {

                    if (file.EndsWith(".meta") || file.EndsWith(".manifest"))
                    {
                        continue;
                    }

                    if (!this.createEncryptedFile(file))
                    {
                        return false;
                    }

                }

                return true;

            }

            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            return false;

        }



        /// <summary>
        /// Convert variant to name
        /// </summary>
        /// <returns>Results to revert</returns>
        // -----------------------------------------------------------------------------------------------
        protected Dictionary<string, LabelAndVariant> setFakeAssetBundleInfos()
        {

            Dictionary<string, LabelAndVariant> ret = new Dictionary<string, LabelAndVariant>();

            AssetImporter ai = null;

            string[] paths = AssetDatabase.GetAllAssetPaths();

            foreach (string path in paths)
            {

                ai = AssetImporter.GetAtPath(path);

                if (!string.IsNullOrEmpty(ai.assetBundleVariant))
                {
                    ret.Add(path, new LabelAndVariant(ai.assetBundleName, ai.assetBundleVariant));
                    ai.assetBundleVariant = "";
                }

            }

            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.Refresh();

            foreach (string key in new List<string>(ret.Keys))
            {
                ai = AssetImporter.GetAtPath(key);
                ai.assetBundleName = ret[key].assetBundleName + "." + ret[key].assetBundleVariant;
            }

            AssetDatabase.Refresh();

            return ret;

        }

        /// <summary>
        /// Revert AssetBundles name and variant
        /// </summary>
        /// <param name="fakes">from setFakeAssetBundleInfos</param>
        // -----------------------------------------------------------------------------------------------
        protected void revertAssetBundleInfos(Dictionary<string, LabelAndVariant> fakes)
        {

            AssetImporter ai = null;

            foreach (var kv in fakes)
            {
                ai = AssetImporter.GetAtPath(kv.Key);
                ai.assetBundleName = kv.Value.assetBundleName;
            }

            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.Refresh();

            foreach (var kv in fakes)
            {
                ai = AssetImporter.GetAtPath(kv.Key);
                ai.assetBundleVariant = kv.Value.assetBundleVariant;
            }

            AssetDatabase.Refresh();

        }

        /// <summary>
        /// Encrypt binary data
        /// </summary>
        /// <param name="data">data to encrypt</param>
        /// <returns>encrypted data</returns>
        // -----------------------------------------------------------------------------------------------
        protected virtual byte[] encryptBinaryData(byte[] data)
        {
            return Funcs.EncryptBinaryData(data, this.m_encryptionInfo.password);
        }

        /// <summary>
        /// Create encrypted file
        /// </summary>
        /// <param name="originFilePath">origin AssetBundle file</param>
        /// <returns>success</returns>
        // -----------------------------------------------------------------------------------------------
        protected bool createEncryptedFile(string originFilePath)
        {

            try
            {

                originFilePath = originFilePath.Replace("\\", "/");

                byte[] data = File.ReadAllBytes(Path.GetFullPath(originFilePath));

                data = this.encryptBinaryData(data);

                if (data.Length > 0)
                {

                    string assetBundleName = "";

                    assetBundleName = originFilePath.Replace(m_encryptionInfo.tempAssetBundlesFolderPath + "/", "");
                    assetBundleName = this.m_encryptionInfo.encryptedAssetBundlePrefix + assetBundleName.Remove(0, assetBundleName.IndexOf("/") + 1).ToLower();

                    string filepath = m_encryptionInfo.tempEncryptedFolderPath + "/" + originFilePath.Replace(m_encryptionInfo.tempAssetBundlesFolderPath + "/", "") + ".bytes";

                    if (!Directory.Exists(Path.GetDirectoryName(filepath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(filepath));
                        AssetDatabase.Refresh();
                    }

                    File.WriteAllBytes(filepath, data);

                    AssetDatabase.Refresh();

                    AssetImporter ai = AssetImporter.GetAtPath(filepath);

                    ai.assetBundleName = assetBundleName;

                }

                else
                {
                    Debug.LogError("Empty Data " + originFilePath);
                    return false;
                }

            }

            catch (Exception e)
            {
                Debug.LogError(originFilePath);
                Debug.LogError(e.Message);
                throw (e);
            }

            return true;

        }

        /// <summary>
        /// Build AssetBundles
        /// </summary>
        /// <param name="buildPath">output path</param>
        /// <param name="encryptedOnly">encrypted only</param>
        /// <returns>success</returns>
        // -----------------------------------------------------------------------------------------------
        protected bool buildAssetBundles(string buildPath, bool encryptedOnly)
        {

            Dictionary<BuildTarget, string> targets = this.manifestNameAndTarget();
            List<AssetBundleBuild> list = null;

            foreach (var kv in targets)
            {

                if (encryptedOnly)
                {

                    list = this.listAssetBundleBuilds(
                        this.m_encryptionInfo.tempEncryptedFolderPath + "/" + kv.Value
                        );

                    if (!this.buildAssetBundles(buildPath, kv.Key, kv.Value, list))
                    {
                        return false;
                    }

                }

                else
                {
                    if (!this.buildAssetBundles(buildPath, kv.Key, kv.Value))
                    {
                        return false;
                    }
                }

                AssetDatabase.Refresh();

            }

            return true;

        }

        /// <summary>
        /// Build AssetBundles
        /// </summary>
        /// <param name="buildPath">output path</param>
        /// <param name="target">BuildTarget</param>
        /// <param name="manifestName">manifest name</param>
        /// <param name="builds">list of AssetBundleBuild</param>
        /// <returns>success</returns>
        // -----------------------------------------------------------------------------------------------
        protected bool buildAssetBundles(string buildPath, BuildTarget target, string manifestName, List<AssetBundleBuild> builds = null)
        {

            BuildAssetBundleOptions opt =
                BuildAssetBundleOptions.ChunkBasedCompression |
                BuildAssetBundleOptions.StrictMode
                ;

            if(this.m_otherInfo.forceRebuild)
            {
                opt = opt | BuildAssetBundleOptions.ForceRebuildAssetBundle;
            }

            // buildPath
            {

                buildPath += buildPath.EndsWith("/") ? "" : "/";
                buildPath += manifestName;

                if (!Directory.Exists(buildPath))
                {
                    Directory.CreateDirectory(buildPath);
                }

            }

            if (builds != null)
            {
                return BuildPipeline.BuildAssetBundles(buildPath, builds.ToArray(), opt, target);
            }

            else
            {
                return BuildPipeline.BuildAssetBundles(buildPath, opt, target);
            }

        }

        /// <summary>
        /// List AssetBundleBuild
        /// </summary>
        /// <param name="folderPath">folder path</param>
        /// <returns>list of AssetBundleBuild</returns>
        // -----------------------------------------------------------------------------------------------
        protected List<AssetBundleBuild> listAssetBundleBuilds(string folderPath)
        {

            AssetImporter ai = null;
            List<AssetBundleBuild> ret = new List<AssetBundleBuild>();

            foreach (string file in Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories))
            {

                if (file.EndsWith(".meta") || file.EndsWith(".manifest"))
                {
                    continue;
                }

                ai = AssetImporter.GetAtPath(file);

                AssetBundleBuild abb = new AssetBundleBuild();

                abb.assetNames = new string[] { ai.assetPath };
                abb.assetBundleName = ai.assetBundleName;

                ret.Add(abb);

            }

            return ret;

        }

        /// <summary>
        /// OnGUI
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        protected virtual void OnGUI()
        {

            this.m_scrollPos = EditorGUILayout.BeginScrollView(this.m_scrollPos);

            bool dummy = true;

            //
            {

                EditorGUIUtility.labelWidth = 250;

                GUILayout.Label("AssetBundle Options", EditorStyles.boldLabel);

                // m_encryptionInfo
                {

                    this.m_encryptionInfo.useEncryption = EditorGUILayout.BeginToggleGroup("Encryption", this.m_encryptionInfo.useEncryption);

                    this.m_encryptionInfo.encryptedAssetBundlePrefix = EditorGUILayout.TextField(
                        new GUIContent("Prefix of encrypted AssetBundles Name", "prefix"),
                        this.m_encryptionInfo.encryptedAssetBundlePrefix
                        );

                    this.m_encryptionInfo.tempAssetBundlesFolderPath = EditorGUILayout.TextField(
                        new GUIContent("Temp AssetBundles Folder", "Starts with Assets/"),
                        this.m_encryptionInfo.tempAssetBundlesFolderPath
                        );

                    this.m_encryptionInfo.tempEncryptedFolderPath = EditorGUILayout.TextField(
                        new GUIContent("Temp encrypted Folder", "Starts with Assets/"),
                        this.m_encryptionInfo.tempEncryptedFolderPath
                        );

                    this.m_encryptionInfo.password = EditorGUILayout.TextField(
                        new GUIContent("Password (Length = 16, 24, or 32)", "Password length must be 16, 24, or 32"),
                        this.m_encryptionInfo.password
                        );

                    EditorGUILayout.EndToggleGroup();

                }

                GUILayout.Space(30.0f);

                // m_buildPlatforms
                {

                    GUILayout.Label("Select Platforms", EditorStyles.boldLabel);

                    // buildWindows
                    {

                        this.m_buildPlatforms.buildWindows = EditorGUILayout.BeginToggleGroup("Windows", this.m_buildPlatforms.buildWindows);

                        if (this.m_encryptionInfo.useEncryption)
                        {
                            this.m_buildPlatforms.windowsEncryptedManifestName = EditorGUILayout.TextField(
                               new GUIContent("Windows Encrypted Manifest Name", "Windows Encrypted Manifest Name"),
                               this.m_buildPlatforms.windowsEncryptedManifestName
                               );
                        }

                        else
                        {
                            this.m_buildPlatforms.windowsManifestName = EditorGUILayout.TextField(
                                new GUIContent("Windows Manifest Name", "Windows Manifest Name"),
                                this.m_buildPlatforms.windowsManifestName
                                );
                        }

                        EditorGUILayout.EndToggleGroup();

                    }

                    // builAndroid
                    {

                        this.m_buildPlatforms.builAndroid = EditorGUILayout.BeginToggleGroup("Android", this.m_buildPlatforms.builAndroid);

                        if(this.m_encryptionInfo.useEncryption)
                        {
                            this.m_buildPlatforms.androidEncryptedManifestName = EditorGUILayout.TextField(
                                new GUIContent("Android Encrypted Manifest Name", "Android Encrypted Manifest Name"),
                                this.m_buildPlatforms.androidEncryptedManifestName
                                );
                        }

                        else
                        {
                            this.m_buildPlatforms.androidManifestName = EditorGUILayout.TextField(
                                new GUIContent("Android Manifest Name", "Android Manifest Name"),
                                this.m_buildPlatforms.androidManifestName
                                );
                        }

                        EditorGUILayout.EndToggleGroup();

                    }

                    // buildWindows
                    {

                        this.m_buildPlatforms.buildIos = EditorGUILayout.BeginToggleGroup("iOS", this.m_buildPlatforms.buildIos);

                        if (this.m_encryptionInfo.useEncryption)
                        {
                            this.m_buildPlatforms.iosEncryptedManifestName = EditorGUILayout.TextField(
                                new GUIContent("iOS Encrypted Manifest Name", "iOS Encrypted Manifest Name"),
                                this.m_buildPlatforms.iosEncryptedManifestName
                                );
                        }

                        else
                        {
                            this.m_buildPlatforms.iosManifestName = EditorGUILayout.TextField(
                                new GUIContent("iOS Manifest Name", "iOS Manifest Name"),
                                this.m_buildPlatforms.iosManifestName
                                );
                        }

                        EditorGUILayout.EndToggleGroup();

                    }

                }

                GUILayout.Space(30.0f);

                // m_otherInfo
                {

                    GUILayout.Label("Other Options", EditorStyles.boldLabel);

                    this.m_otherInfo.forceRebuild = EditorGUILayout.Toggle(
                        new GUIContent("Force rebuild AssetBundles", "Force rebuild AssetBundles"),
                        this.m_otherInfo.forceRebuild
                        );

                    this.m_otherInfo.useFakeAssetBundleNames = EditorGUILayout.Toggle(
                        new GUIContent("(Convert variant to name)", "If you got an error 'already loaded' in LoadFromMemoryAsync, try this"),
                        this.m_otherInfo.useFakeAssetBundleNames
                        );

                }

                // button
                {

                    GUILayout.Space(30.0f);

                    EditorGUILayout.BeginHorizontal();

                    GUILayout.Space(250.0f);

                    if (GUILayout.Button("Reset Settings", GUILayout.MinHeight(30)))
                    {

                        GUI.FocusControl("");

                        if (EditorUtility.DisplayDialog("Reset Settings", "Reset ?", "Yes", "No"))
                        {
                            this.resetSettings();
                        }
                            
                    }

                    EditorGUILayout.EndHorizontal();

                }

            }

            // validate
            {
                this.m_encryptionInfo.validate();
            }

            // build
            {

                GUILayout.FlexibleSpace();

                EditorGUILayout.BeginHorizontal(GUI.skin.box);

                if (GUILayout.Button("Clear Editor Cache", GUILayout.MinHeight(30), GUILayout.MaxWidth(150)))
                {

                    if (EditorUtility.DisplayDialog("Clear Editor AssetBundle Cache", "Clear Editor Cache ?", "Yes", "No"))
                    {
                        EditorUtility.DisplayDialog("Clear Editor AssetBundle Cache", Caching.CleanCache() ? "Success" : "Failed", "Ok");
                    }

                }

                GUILayout.Space(150.0f);

                if (GUILayout.Button("Build AssetBundles", GUILayout.MinHeight(30)))
                {
                    this.build();
                    dummy = false;
                }

                EditorGUILayout.EndHorizontal();

            }

            if (dummy)
            {
                EditorGUILayout.EndScrollView();
            }

        }

        /// <summary>
        /// OnEnable
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        protected virtual void OnEnable()
        {
            this.loadPrefs();
        }

        /// <summary>
        /// OnDisable
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        protected virtual void OnDisable()
        {
            this.savePrefs();
        }

        /// <summary>
        /// Load prefs
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        protected void loadPrefs()
        {

            MonoScript script = MonoScript.FromScriptableObject(this);
            string scriptpath = AssetDatabase.GetAssetPath(script);
            string sopath = Path.GetDirectoryName(scriptpath) + "/" + Path.GetFileNameWithoutExtension(scriptpath) + "Prefs.asset";

            BuildAssetBundlesWindowPrefs prefs = AssetDatabase.LoadAssetAtPath<BuildAssetBundlesWindowPrefs>(sopath);

            if (prefs)
            {
                this.m_buildPlatforms = prefs.buildPlatforms;
                this.m_encryptionInfo = prefs.encryptionInfo;
            }

        }

        /// <summary>
        /// Save prefs
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        protected virtual void savePrefs()
        {

            MonoScript script = MonoScript.FromScriptableObject(this);
            string scriptpath = AssetDatabase.GetAssetPath(script);
            string sopath = Path.GetDirectoryName(scriptpath) + "/" + Path.GetFileNameWithoutExtension(scriptpath) + "Prefs.asset";

            BuildAssetBundlesWindowPrefs prefs = AssetDatabase.LoadAssetAtPath<BuildAssetBundlesWindowPrefs>(sopath);

            if (!prefs)
            {
                prefs = CreateInstance<BuildAssetBundlesWindowPrefs>();
                AssetDatabase.CreateAsset(prefs, sopath);
            }

            // save
            {
                prefs.buildPlatforms = this.m_buildPlatforms;
                prefs.encryptionInfo = this.m_encryptionInfo;

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

        }

        /// <summary>
        /// Dirty way to refresh editor
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        protected void forceRefresh()
        {

            MonoScript script = MonoScript.FromScriptableObject(this);
            string scriptpath = AssetDatabase.GetAssetPath(script);
            string sopath = Path.GetDirectoryName(scriptpath) + "/dummy.cs";

            if(File.Exists(sopath))
            {
                return;
            }

            using (StreamWriter stream = new StreamWriter(sopath))
            {
                stream.WriteLine("using System;");
            }

            AssetDatabase.Refresh();

            AssetDatabase.DeleteAsset(sopath);

            AssetDatabase.Refresh();

        }

    }


}