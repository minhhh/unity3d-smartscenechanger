using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSC
{

    /// <summary>
    /// DontDestroyOnLoad
    /// </summary>
    public class DontDestroyScript : MonoBehaviour
    {

        /// <summary>
        /// Awake()
        /// </summary>
        void Awake()
        {
            DontDestroyOnLoad(this);
        }

    }

}
