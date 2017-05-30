﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSCSample
{

    public class SampleAssetBundleStartupManager : SSC.AssetBundleStartupManager
    {

        [SerializeField]
        string m_decryptionPassword = "PassworDPassworD";

        [TextArea]
        [SerializeField]
        string m_iosManifestFileUrl = "http://localhost:50002/ios.encrypted.unity3d/encrypted/ios.encrypted.unity3d";

        [TextArea]
        [SerializeField]
        string m_androidManifestFileUrl = "http://localhost:50002/android.encrypted.unity3d/encrypted/android.encrypted.unity3d";

        [TextArea]
        [SerializeField]
        string m_winManifestFileUrl = "http://localhost:50002/windows.encrypted.unity3d/encrypted/windows.encrypted.unity3d";

        // -------------------------------------------------------------------------------------------------------

        public string iosManifestFileUrl { get { return this.m_iosManifestFileUrl; } set { this.m_iosManifestFileUrl = value; } }

        public string androidManifestFileUrl { get { return this.m_androidManifestFileUrl; } set { this.m_androidManifestFileUrl = value; } }

        public string winManifestFileUrl { get { return this.m_winManifestFileUrl; } set { this.m_winManifestFileUrl = value; } }

        // -------------------------------------------------------------------------------------------------------

        // -------------------------------------------------------------------------------------------------------
        protected override void setManifestFileAndFolderUrl()
        {

            string manifestFileUrl = "";

#if UNITY_IOS
        
            manifestFileUrl = this.m_iosManifestFileUrl;

#elif UNITY_ANDROID
        
            manifestFileUrl = this.m_androidManifestFileUrl;

#else

            manifestFileUrl = this.m_winManifestFileUrl;

#endif

            this.m_assetBundleManifestFileUrl = manifestFileUrl;
            this.m_assetBundleManifestFolderUrl = manifestFileUrl.Substring(0, manifestFileUrl.LastIndexOf('/') + 1);

            //// from streamingAssetsPath exsample
            //if (UnityEngine.Random.value > 10.0f) // always false
            //{
            //    this.m_assetBundleManifestFileUrl =
            //        "file:///" + Application.streamingAssetsPath + "/windows.encrypted.unity3d/encrypted/" + "windows.encrypted.unity3d"
            //        ;

            //    this.m_assetBundleManifestFolderUrl
            //        = "file:///" + Application.streamingAssetsPath + "/windows.encrypted.unity3d/encrypted/"
            //        ;
            //}

        }

        // -------------------------------------------------------------------------------------------------------
        protected override string createAssetBundleUrl(string nameDotVariant)
        {
            return this.m_assetBundleManifestFolderUrl + nameDotVariant;
        }

        // -------------------------------------------------------------------------------------------------------
        protected override byte[] decryptBinaryData(TextAsset textAsset)
        {

            if (!textAsset)
            {
                return new byte[] { };
            }

            return SSC.Funcs.DecryptBinaryData(textAsset.bytes, this.m_decryptionPassword);

        }

        /// <summary>
        /// Create error message for dialog
        /// </summary>
        /// <returns>error message object</returns>
        // -------------------------------------------------------------------------------------------------------
        public override System.Object createErrorMessage()
        {
            return base.createErrorMessage();
        }

    }

}
