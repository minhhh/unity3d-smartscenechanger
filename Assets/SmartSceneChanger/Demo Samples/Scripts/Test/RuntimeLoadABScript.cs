using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSCSample
{

    public class RuntimeLoadABScript : MonoBehaviour
    {

        [SerializeField]
        string m_assetBundleName = "";

        [SerializeField]
        string m_assetBundleVariant = "";

        public void startLoading()
        {

            SSC.AssetBundleStartupManager.Instance.loadAssetBundleInRuntime(
                this.m_assetBundleName,
                this.m_assetBundleVariant,
                successFunc,
                failedFunc,
                progressFunc
                );

        }

        void successFunc(AssetBundle assetBundle)
        {

            // do not Unload here

            if (assetBundle.isStreamedSceneAssetBundle)
            {
                return;
            }

            foreach (string name in assetBundle.GetAllAssetNames())
            {
                Object obj = assetBundle.LoadAsset(name);
                GameObject gobj = Instantiate(obj) as GameObject;
                
                gobj.transform.SetParent(this.transform, false);

                float valX = UnityEngine.Random.Range(-5.0f, 5.0f);
                float valY = UnityEngine.Random.Range(-5.0f, 5.0f);
                float valZ = UnityEngine.Random.Range(-5.0f, 5.0f);

                gobj.transform.localPosition = new Vector3(valX, valY, valZ);
            }

        }

        void failedFunc(WWW www)
        {
            Debug.LogError("failedFunc");
        }

        void progressFunc(WWW www)
        {
            // print("progressFunc");
        }

    }

}
