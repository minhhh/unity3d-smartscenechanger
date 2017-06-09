using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSCSample
{

    public class SampleAssetBundleStartupAsyncScript : MonoBehaviour
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

        // --------------------------------------------------------------------------------
        void Start()
        {

            SSC.AssetBundleStartupManager.Instance.addSceneStartupAssetBundle(
                this.m_assetBundleName,
                this.m_variant,
                this.successAsync,
                null,
                null,
                "Any identifier or class"
                );

        }


        // --------------------------------------------------------------------------------
        void successAsync(AssetBundle assetBundle, System.Object info, Action finishCallback)
        {
            StartCoroutine(this.successAsyncIE(assetBundle, info, finishCallback));
        }

        // --------------------------------------------------------------------------------
        IEnumerator successAsyncIE(AssetBundle assetBundle, System.Object info, Action finishCallback)
        {

            yield return null;

            if(info != null && info is string)
            {
                print(info as string);
            }

            if (!assetBundle.isStreamedSceneAssetBundle)
            {

                foreach (string name in assetBundle.GetAllAssetNames())
                {

                    var request = assetBundle.LoadAssetAsync(name);

                    while(!request.isDone)
                    {
                        yield return null;
                    }

                    UnityEngine.Object obj = request.asset;
                    GameObject gobj = Instantiate(obj) as GameObject;
                    gobj.transform.SetParent(this.transform, false);

                }

            }

            finishCallback(); // IMPORTANT

        }

    }

}
