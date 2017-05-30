using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSC
{

    /// <summary>
    /// Class for WWW startup
    /// </summary>
    public abstract class WwwStartupScript : MonoBehaviour
    {

        /// <summary>
        /// Url
        /// </summary>
        [SerializeField]
        [Tooltip("Url")]
        protected string m_url = "";

        /// <summary>
        /// Called in Start()
        /// </summary>
        protected abstract void initOnStart();

        /// <summary>
        /// Success function
        /// </summary>
        /// <param name="www">WWW</param>
        public abstract void success(WWW www);

        /// <summary>
        /// Failed function
        /// </summary>
        /// <param name="www">WWW</param>
        public abstract void failed(WWW www);

        /// <summary>
        /// Progress function
        /// </summary>
        /// <param name="www">WWW</param>
        public abstract void progress(WWW www);

        /// <summary>
        /// Start()
        /// </summary>
        protected virtual void Start()
        {

            this.initOnStart();

            WwwStartupManager.Instance.addSceneStartupWww(this.m_url, this.success, this.failed, this.progress);

        }

    }

}
