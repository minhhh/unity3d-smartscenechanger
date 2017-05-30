using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSC
{

    /// <summary>
    /// Scene UI Manager
    /// </summary>
    public class SceneUiManager : UiManager
    {

        /// <summary>
        /// Singleton instance
        /// </summary>
        static SceneUiManager instance;

        /// <summary>
        /// static SceneUiManager instance
        /// </summary>
        public static SceneUiManager Instance
        {
            get
            {
                if (instance == null && !isQuitting)
                {
                    instance = (SceneUiManager)FindObjectOfType(typeof(SceneUiManager));
                }

                return instance;
            }
        }

        //------------------------------------

        /// <summary>
        /// Now quitting
        /// </summary>
        static bool isQuitting = false;

        //------------------------------------

        /// <summary>
        /// Awake()
        /// </summary>
        //-----------------------------------------------------------------------
        protected override void Awake()
        {

            base.Awake();

            if (!this.checkInstance())
            {
                return;
            }

        }

        /// <summary>
        /// If an instance already exists, destroy new one.
        /// </summary>
        /// <returns>if instance is this, return true.</returns>
        //-----------------------------------------------------------------------
        bool checkInstance()
        {
            if (instance == null)
            {
                instance = (SceneUiManager)this;
                return true;
            }
            else if (Instance == this)
            {
                return true;
            }


#if UNITY_EDITOR
            Debug.LogWarning("(#if UNITY_EDITOR) Destroy the object because an instance alresdy exists : " + this.gameObject.name);
#endif
            Destroy(this.gameObject);

            return false;
        }

        /// <summary>
        /// OnApplicationQuit
        /// </summary>
        //-----------------------------------------------------------------------
        protected virtual void OnApplicationQuit()
        {
            isQuitting = true;
        }


    }

}
