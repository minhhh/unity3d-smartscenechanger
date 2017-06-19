using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


public class SSC_EditorMenus
{

    ///<summary>
    /// Show Editor Scene Loader Window
    ///</summary>
    [MenuItem("Tools/SSC/Editor Scene Loader Window", false, 0)]
    static void EditorSceneLoader()
    {
        EditorWindow.GetWindow(typeof(SSC.EditorSceneLoaderWindow)).Show();
    }

    /// <summary>
    /// Show Build AssetBundles Window
    /// </summary>
    [MenuItem("Tools/SSC/Sample Build AssetBundles Window", false, 0)]
    static void ShowSampleBuildAssetBundlesWindow()
    {
        EditorWindow.GetWindow(typeof(SSCSample.SampleBuildAssetBundlesWindow)).Show();
    }

    /// <summary>
    /// Show Set AssetBundle Name Window
    /// </summary>
    [MenuItem("Tools/SSC/Set AssetBundle Name Window", false, 0)]
    static void ShowSetAssetBundleNameWindow()
    {
        EditorWindow.GetWindow(typeof(SSC.SetAssetBundleNameWindow)).Show();
    }

    /// <summary>
    /// Show Set AssetBundle Name Window
    /// </summary>
    [MenuItem("Tools/SSC/Show All AssetBundle Names Window", false, 0)]
    static void ShowAllAssetBundleNamesWindow()
    {
        EditorWindow.GetWindow(typeof(SSC.ShowAllAssetBundleNamesWindow)).Show();
    }

    /// <summary>
    /// Show Set AssetBundle Name Window
    /// </summary>
    [MenuItem("Tools/SSC/Create Starter Window", false, 0)]
    static void ShowCreateStarterWindow()
    {
        EditorWindow.GetWindow(typeof(SSC.CreateStarterWindow)).Show();
    }

}
