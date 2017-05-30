using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSC
{

    /// <summary>
    /// BuildAssetBundlesWindowPrefs
    /// </summary>
    public class BuildAssetBundlesWindowPrefs : ScriptableObject
    {

        /// <summary>
        /// EncryptionInfo
        /// </summary>
        [HideInInspector]
        public BuildAssetBundlesWindow.EncryptionInfo encryptionInfo = new BuildAssetBundlesWindow.EncryptionInfo();

        /// <summary>
        /// BuildPlatforms
        /// </summary>
        [HideInInspector]
        public BuildAssetBundlesWindow.BuildPlatforms buildPlatforms = new BuildAssetBundlesWindow.BuildPlatforms();

    }

}

