using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSC
{

    /// <summary>
    /// Class for IEnumerator startup
    /// </summary>
    public abstract class IEnumeratorStartupScript : MonoBehaviour
    {

        /// <summary>
        /// done flag
        /// </summary>
        [Obsolete("Not in use", true)]
        private bool m_done = false;

        /// <summary>
        /// error message
        /// </summary>
        private string m_error = "";

        /// <summary>
        /// progress
        /// </summary>
        private float m_progress = 0.0f;

        /// <summary>
        /// Called in Start()
        /// </summary>
        protected abstract void initOnStart();

        /// <summary>
        /// Main function is needed to implement
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator startup();

        /// <summary>
        /// Before any startups, After any startups
        /// </summary>
        [SerializeField]
        [Tooltip("Before any startups, After any startups")]
        protected IEnumeratorStartupManager.BeforeAfter m_beforeAfter = IEnumeratorStartupManager.BeforeAfter.After;

        /// <summary>
        /// Start()
        /// </summary>
        protected virtual void Start()
        {

            this.initOnStart();

            IEnumeratorStartupManager.Instance.addSceneStartupIEnumerator(this.startup(), this.progress, this.error, this.m_beforeAfter);

        }

        /// <summary>
        /// Call startup()
        /// </summary>
        /// <returns>IEnumerator</returns>
        [Obsolete("Not in use", true)]
        public IEnumerator startupBase()
        {

            this.m_done = false;
            this.m_error = "";

            yield return this.startup();

            this.m_done = true;
            this.m_progress = 0.0f;

            yield return null;

        }

        /// <summary>
        /// Return done flag
        /// </summary>
        /// <returns>done flag</returns>
        [Obsolete("Not in use", true)]
        public bool isDone()
        {
            return this.m_done;
        }

        /// <summary>
        /// Return error text
        /// </summary>
        /// <returns>error text</returns>
        public string error()
        {
            return this.m_error;
        }

        /// <summary>
        /// Set error message
        /// </summary>
        /// <param name="error"></param>
        protected void setError(string error)
        {
            this.m_error = error;
        }

        /// <summary>
        /// Progress
        /// </summary>
        /// <returns>progress</returns>
        public float progress()
        {
            return this.m_progress;
        }

        /// <summary>
        /// Set progress
        /// </summary>
        /// <param name="val">progress</param>
        protected void setProgress(float val)
        {
            this.m_progress = Mathf.Clamp01(val);
        }

    }

}
