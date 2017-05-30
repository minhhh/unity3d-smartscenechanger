using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSC
{

    /// <summary>
    /// Singleton MonoBehaviour
    /// </summary>
    /// <typeparam name="T">class</typeparam>
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {

        /// <summary>
        /// Singleton instance
        /// </summary>
        static T instance;

        /// <summary>
        /// static T instance
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance == null && !isQuitting)
                {
                    instance = (T)FindObjectOfType(typeof(T));

                    if (instance == null)
                    {
                        Debug.LogError(typeof(T) + " Not Found. You Must Attach This to a GameObject");
                    }
                }

                return instance;
            }
        }

        //------------------------------------

        /// <summary>
        /// initOnAwake() is called after Awake().
        /// </summary>
        protected abstract void initOnAwake();

        /// <summary>
        /// Now quitting
        /// </summary>
        static bool isQuitting = false;

        //------------------------------------

        /// <summary>
        /// Awake()
        /// </summary>
        //-----------------------------------------------------------------------
        protected virtual void Awake()
        {

            if (!this.checkInstance())
            {
                return;
            }

            this.initOnAwake();

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
                instance = (T)this;
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
