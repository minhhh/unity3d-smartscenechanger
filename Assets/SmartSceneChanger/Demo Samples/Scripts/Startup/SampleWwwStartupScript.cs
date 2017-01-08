using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSCSample
{

    [RequireComponent(typeof(MeshRenderer))]
    public class SampleWwwStartupScript : SSC.WwwStartupScript
    {

        MeshRenderer m_refRenderer = null;

        protected override void initOnStart()
        {
            this.m_refRenderer = this.GetComponent<MeshRenderer>();
        }

        public override void success(WWW www)
        {
            this.m_refRenderer.material.mainTexture = www.texture;
        }

        public override void failed(WWW www)
        {
            Debug.LogError(www.error);
        }

        public override void progress(WWW www)
        {

        }

        void OnDestroy()
        {
            if (this.m_refRenderer)
            {
                Destroy(this.m_refRenderer.material.mainTexture);
                Destroy(this.m_refRenderer.material);
            }
        }

    }

}
