using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSC
{

    /// <summary>
    /// Common UI Manager
    /// </summary>
    public class CommonUiManager : UiManager
    {

        /// <summary>
        /// Singleton instance
        /// </summary>
        static CommonUiManager instance;

        /// <summary>
        /// static CommonUiManager instance
        /// </summary>
        public static CommonUiManager Instance
        {
            get
            {
                if (instance == null && !isQuitting)
                {
                    instance = (CommonUiManager)FindObjectOfType(typeof(CommonUiManager));

                    if (instance == null)
                    {
                        Debug.LogError(typeof(CommonUiManager) + " Not Found. You Must Attach This to a GameObject");
                    }
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
        /// Is available
        /// </summary>
        /// <returns>available</returns>
        //-----------------------------------------------------------------------
        public static bool isAvailable()
        {
            return instance;
        }

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
                instance = (CommonUiManager)this;
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
