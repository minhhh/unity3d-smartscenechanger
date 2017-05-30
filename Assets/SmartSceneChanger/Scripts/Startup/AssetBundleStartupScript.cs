using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSC
{

    /// <summary>
    /// Class for AssetBundle startup
    /// </summary>
    public abstract class AssetBundleStartupScript : MonoBehaviour
    {

        /// <summary>
        /// AssetBundle name
        /// </summary>
        [SerializeField]
        [Tooltip("AssetBundle name")]
        protected string m_assetBundleName = "";

        /// <summary>
        /// AssetBundle variant
        /// </summary>
        [SerializeField]
        [Tooltip("AssetBundle variant")]
        protected string m_variant = "";

        /// <summary>
        /// Called in Start()
        /// </summary>
        protected abstract void initOnStart();

        /// <summary>
        /// Success function
        /// </summary>
        /// <param name="ab">Result AssetBundle</param>
        public abstract void success(AssetBundle ab);

        /// <summary>
        /// Failed function
        /// </summary>
        /// <param name="www">failed WWW</param>
        public abstract void failed(WWW www);

        /// <summary>
        /// Progress function
        /// </summary>
        /// <param name="www">progress WWW</param>
        public abstract void progress(WWW www);

        /// <summary>
        /// Start()
        /// </summary>
        protected virtual void Start()
        {

            this.initOnStart();

            AssetBundleStartupManager.Instance.addSceneStartupAssetBundle(this.m_assetBundleName, this.m_variant, this.success, this.failed, this.progress);

        }

    }

}
