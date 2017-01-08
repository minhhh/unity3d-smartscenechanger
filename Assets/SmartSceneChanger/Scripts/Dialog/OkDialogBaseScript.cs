using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSC
{

    /// <summary>
    /// Class for OK button dialog
    /// </summary>
    public abstract class OkDialogBaseScript : MonoBehaviour
    {

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
        /// Called when OK button Clicked
        /// </summary>
        protected abstract void onClickOk();

        /// <summary>
        /// OkDialogState reference
        /// </summary>
        protected OkDialogState refState = null;

        /// <summary>
        /// Show Dialog
        /// </summary>
        /// <param name="state"></param>
        // -------------------------------------------------------------------------
        public void showDialog(OkDialogState state)
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
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------
        IEnumerator startOutro()
        {

            yield return this.outro();

            var ok_action = this.refState.okAction;

            // state
            {
                var state = SimpleReduxManager.Instance.OkDialogStateWatcher.state();
                state.setState(
                    SimpleReduxManager.Instance.OkDialogStateWatcher,
                    OkDialogState.StateEnum.Done,
                    "",
                    null
                    );
            }

            // action callback
            {
                if (ok_action != null)
                {
                    ok_action();
                }

                else
                {
                    Debug.LogError("No Callback Action");
                }
            }



        }

        /// <summary>
        /// Ok Button function
        /// </summary>
        // -------------------------------------------------------------------------
        public void onClickOkBase()
        {
            this.onClickOk();
            StartCoroutine(this.startOutro());
        }

    }

}