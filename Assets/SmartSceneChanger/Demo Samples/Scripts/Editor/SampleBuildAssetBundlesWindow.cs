using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SSCSample
{

    public class SampleBuildAssetBundlesWindow : SSC.BuildAssetBundlesWindow
    {

        // -----------------------------------------------------------------------------------------------
        protected override Dictionary<BuildTarget, string> manifestNameAndTarget()
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

        // -----------------------------------------------------------------------------------------------
        protected override byte[] encryptBinaryData(byte[] data)
        {
            return SSC.Funcs.EncryptBinaryData(data, this.m_encryptionInfo.password);
        }

        // -----------------------------------------------------------------------------------------------
        protected override void OnGUI()
        {
            base.OnGUI();
        }

        // -----------------------------------------------------------------------------------------------
        protected override void OnEnable()
        {
            this.loadPrefs();
        }

        // -----------------------------------------------------------------------------------------------
        protected override void OnDisable()
        {
            this.savePrefs();
        }

    }

}
