using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SSC
{

    /// <summary>
    /// Progress dialog UI controller
    /// </summary>
    public abstract class ProgressDialogUiControllerScript : DialogUiControllerScript
    {

        /// <summary>
        /// Set progress value
        /// </summary>
        /// <param name="val">progress value</param>
        public abstract void setProgress(float val);

    }

}
