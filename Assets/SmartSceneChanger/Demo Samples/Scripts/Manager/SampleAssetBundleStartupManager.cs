using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSCSample
{

    public class SampleAssetBundleStartupManager : SSC.AssetBundleStartupManager
    {

        [SerializeField]
        string m_encryptedManifestFileUrl = "http://localhost:50002/windows.encrypted.unity3d/encrypted/windows.encrypted.unity3d";

        [SerializeField]
        string m_manifestFileUrl = "http://localhost:50002/windows.unity3d/windows.unity3d";

        // -------------------------------------------------------------------------------------------------------
        protected override void setManifestFileAndFolderUrl()
        {

            Debug.LogWarning("You must override this function as you want. This sample code is only for one OS.");

            string manifestFileUrl = (this.m_useDecryption) ? this.m_encryptedManifestFileUrl : this.m_manifestFileUrl;

            this.m_assetBundleManifestFileUrl = manifestFileUrl;
            this.m_assetBundleManifestFolderUrl = manifestFileUrl.Substring(0, manifestFileUrl.LastIndexOf('/') + 1);

            // from streamingAssetsPath exsample
            if (UnityEngine.Random.value > 10.0f) // always false
            {
                this.m_assetBundleManifestFileUrl =
                    "file:///" + Application.streamingAssetsPath + "/windows.encrypted.unity3d/encrypted/" + "windows.encrypted.unity3d"
                    ;

                this.m_assetBundleManifestFolderUrl
                    = "file:///" + Application.streamingAssetsPath + "/windows.encrypted.unity3d/encrypted/"
                    ;
            }

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

            return SSC.Funcs.DecryptBinaryData(textAsset.bytes, "PassworDPassworD");

        }

    }

}
