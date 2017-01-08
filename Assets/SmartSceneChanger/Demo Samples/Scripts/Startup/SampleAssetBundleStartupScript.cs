using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSCSample
{

    public class SampleAssetBundleStartupScript : SSC.AssetBundleStartupScript
    {

        protected override void initOnStart()
        {

        }

        public override void success(AssetBundle assetBundle)
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
            }

        }

        public override void failed(WWW www)
        {
            Debug.LogError(www.url + " " + www.error);
        }

        public override void progress(WWW www)
        {

        }

    }

}
