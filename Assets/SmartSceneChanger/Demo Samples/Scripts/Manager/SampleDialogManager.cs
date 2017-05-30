﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSCSample
{

    public class SampleDialogManager : SSC.DialogManager
    {

        /// <summary>
        /// Add error
        /// </summary>
        /// <param name="messages">System.Object</param>
        // ----------------------------------------------------------------------------------------
        protected override void addErrorStackIfError(System.Object messages)
        {
            base.addErrorStackIfError(messages);
        }

        /// <summary>
        /// Show ok dialog
        /// </summary>
        /// <param name="messages">System.Object</param>
        /// <param name="okCallback">ok button callback</param>
        /// <returns>suuccess</returns>
        // ----------------------------------------------------------------------------------------
        public override bool showOkDialog(System.Object messages, Action okCallback, OkDialogSelectable selectable = OkDialogSelectable.Ok)
        {
            return base.showOkDialog(messages, okCallback, selectable);
        }

        /// <summary>
        /// Show yes no dialog
        /// </summary>
        /// <param name="messages">DialogMessages</param>
        /// <param name="yesCallback">yes button callback</param>
        /// <param name="noCallback">no button callback</param>
        /// <returns>suuccess</returns>
        /// <summary>
        // ----------------------------------------------------------------------------------------
        public override bool showYesNoDialog(
            System.Object messages,
            Action yesCallback,
            Action noCallback,
            YesNoDialogSelectable selectable = YesNoDialogSelectable.No
            )
        {
            return base.showYesNoDialog(messages, yesCallback, noCallback, selectable);
        }

    }

}
