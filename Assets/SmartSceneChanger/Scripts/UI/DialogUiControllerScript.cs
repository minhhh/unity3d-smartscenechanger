using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SSC
{

    /// <summary>
    /// Dialog UI controller
    /// </summary>
    public abstract class DialogUiControllerScript : UiControllerScript
    {

        /// <summary>
        /// Set DialogMessages
        /// </summary>
        /// <param name="messages">System.Object</param>
        public abstract void setMessages(System.Object messages);

    }

}
