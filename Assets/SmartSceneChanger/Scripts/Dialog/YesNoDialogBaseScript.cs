using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSC
{

    /// <summary>
    /// Class for Yes No button dialog
    /// </summary>
    public abstract class YesNoDialogBaseScript : MonoBehaviour
    {

        /// <summary>
        /// Yes No state
        /// </summary>
        enum YesNo
        {
            Yes,
            No
        }

        /// <summary>
        /// Intro IEnumerator
        /// </summary>
        /// <returns>IEnumerator</returns>
        protected abstract IEnumerator intro();

        /// <summary>
        /// Outro IEnumerator
        /// </summary>
        /// <returns>IEnumerator</returns>
        protected abstract IEnumerator outro();

        /// <summary>
        /// Main Loop IEnumerator
        /// </summary>
        /// <returns>IEnumerator</returns>
        protected abstract IEnumerator mainLoop();

        /// <summary>
        /// Yes button function
        /// </summary>
        protected abstract void onClickYes();

        /// <summary>
        /// No button function
        /// </summary>
        protected abstract void onClickNo();

        /// <summary>
        /// YesNoDialogState reference
        /// </summary>
        protected YesNoDialogState refState = null;

        /// <summary>
        /// Dhow dialog
        /// </summary>
        /// <param name="state">yes or no</param>
        // -------------------------------------------------------------------------
        public void showDialog(YesNoDialogState state)
        {
            this.refState = state;
            StartCoroutine(this.startIntro());
        }

        /// <summary>
        /// Start intro
        /// </summary>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------
        IEnumerator startIntro()
        {
            yield return this.intro();
            StartCoroutine(this.mainLoop());
        }

        /// <summary>
        /// Start outro
        /// </summary>
        /// <param name="yesno">yes or no</param>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------
        IEnumerator startOutro(YesNo yesno)
        {

            yield return this.outro();

            var yes_action = this.refState.yesAction;
            var no_action = this.refState.noAction;

            // state
            {
                var state = SimpleReduxManager.Instance.YesNoDialogStateWatcher.state();
                state.setState(
                    SimpleReduxManager.Instance.YesNoDialogStateWatcher,
                    YesNoDialogState.StateEnum.Done,
                    "",
                    "",
                    null,
                    null
                    );
            }

            // action callback
            {
                if (yesno == YesNo.Yes && yes_action != null)
                {
                    yes_action();
                }

                else if (yesno == YesNo.No && no_action != null)
                {
                    no_action();
                }

                else
                {
                    Debug.LogError("No Callback Action");
                }
            }

        }

        /// <summary>
        /// Yes button function
        /// </summary>
        // -------------------------------------------------------------------------
        public void onClickYesBase()
        {
            this.onClickYes();
            StartCoroutine(this.startOutro(YesNo.Yes));
        }

        /// <summary>
        /// No button function
        /// </summary>
        // -------------------------------------------------------------------------
        public void onClickNoBase()
        {
            this.onClickNo();
            StartCoroutine(this.startOutro(YesNo.No));
        }

    }

}
