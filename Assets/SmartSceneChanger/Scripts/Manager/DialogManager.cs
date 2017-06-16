using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//using DialogInfos = System.Collections.Generic.Dictionary<string, System.Object>;

namespace SSC
{

    /// <summary>
    /// UI singleton manager
    /// </summary>
    public class DialogManager : SingletonMonoBehaviour<DialogManager>
    {

        /// <summary>
        /// Ok dialog Selectable
        /// </summary>
        public enum OkDialogSelectable
        {
            None,
            Ok,
        }

        /// <summary>
        /// Yesno dialog Selectable
        /// </summary>
        public enum YesNoDialogSelectable
        {
            None,
            Yes,
            No,
        }

        /// <summary>
        /// Reference to input blocker UiControllerScript
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to input blocker UiControllerScript")]
        protected UiControllerScript m_refInputBlocker;

        /// <summary>
        /// Reference to ok dialog UiControllerScript
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to ok dialog UiControllerScript")]
        protected DialogUiControllerScript m_refOkDialog;

        /// <summary>
        /// Reference to yes no dialog UiControllerScript
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to yes no dialog UiControllerScript")]
        protected DialogUiControllerScript m_refYesNoDialog;

        /// <summary>
        /// Reference to OK button
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to OK button")]
        protected Selectable m_refOkButtonSelectable;

        /// <summary>
        /// Reference to Yes button
        /// </summary>
        [Tooltip("Reference to Yes button")]
        [SerializeField]
        protected Selectable m_refYesButtonSelectable;

        /// <summary>
        /// Reference to No button
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to No button")]
        protected Selectable m_refNoButtonSelectable;

        /// <summary>
        /// The number of error stack
        /// </summary>
        [SerializeField]
        [Tooltip("The number of error stack")]
        protected int m_numberOfErrorStack = 10;

        /// <summary>
        /// Ok button callback
        /// </summary>
        protected Action m_okButtonCallback = null;

        /// <summary>
        /// Yes button callback
        /// </summary>
        protected Action m_yesButtonCallback = null;

        /// <summary>
        /// No button callback
        /// </summary>
        protected Action m_noButtonCallback = null;

        /// <summary>
        /// If now showing dialog
        /// </summary>
        protected bool m_nowShowing = false;

        /// <summary>
        /// Error stack
        /// </summary>
        protected List<System.Object> m_errorDialogMessagesStack = new List<System.Object>();

        /// <summary>
        /// pause state before dialog showing
        /// </summary>
        protected bool m_previousPauseState = false;

        /// <summary>
        /// Reference to Selectable before showing dialog
        /// </summary>
        protected Selectable m_refSelectableBeforeShowingDialog = null;

        // ----------------------------------------------------------------------------------------

        /// <summary>
        /// If now showing dialog
        /// </summary>
        public bool nowShowing { get { return this.m_nowShowing; } }

        /// <summary>
        /// Error stack
        /// </summary>
        public List<System.Object> errorDialogMessagesStack { get { return this.m_errorDialogMessagesStack; } }

        // ----------------------------------------------------------------------------------------

        /// <summary>
        /// Called in Awake
        /// </summary>
        // ----------------------------------------------------------------------------------------
        protected override void initOnAwake()
        {

#if UNITY_EDITOR

            if (!this.m_refInputBlocker)
            {
                Debug.LogWarning("(#if UNITY_EDITOR) m_refInputBlocker is null");
            }

            if (!this.m_refOkDialog)
            {
                Debug.LogWarning("(#if UNITY_EDITOR) m_refOkDialog is null");
            }

            if (!this.m_refYesNoDialog)
            {
                Debug.LogWarning("(#if UNITY_EDITOR) m_refYesNoDialog is null");
            }

#endif

        }

        /// <summary>
        /// Finish dialog
        /// </summary>
        // ----------------------------------------------------------------------------------------
        public void finishDialog(Action finishDoneCallback)
        {

            if (!this.m_nowShowing)
            {
                return;
            }

            // ---------------

            if (this.m_refInputBlocker)
            {
                this.m_refInputBlocker.startHiding(() =>
                {

                    this.m_nowShowing = false;
                    this.resumePauseSignalIfNeeded();

                    if (finishDoneCallback != null)
                    {
                        finishDoneCallback();
                    }

                });
            }

            else
            {

                this.m_nowShowing = false;
                this.resumePauseSignalIfNeeded();

                if (finishDoneCallback != null)
                {
                    finishDoneCallback();
                }

            }

            // resume selectable
            {

                if(this.m_refSelectableBeforeShowingDialog)
                {
                    EventSystem.current.SetSelectedGameObject(this.m_refSelectableBeforeShowingDialog.gameObject);
                }

                else
                {
                    EventSystem.current.SetSelectedGameObject(null);
                }

            }

        }

        /// <summary>
        /// Add error
        /// </summary>
        /// <param name="messages">System.Object</param>
        // ----------------------------------------------------------------------------------------
        protected virtual void addErrorStackIfError(System.Object messages)
        {

            if (messages == null)
            {
                return;
            }

            // ---------------------------

            DialogMessages dialogMessages = messages as DialogMessages;

            if (dialogMessages == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("(#if UNITY_EDITOR) You should override this function if you want to store error messages");
#endif
                return;
            }

            // ---------------------------

            if (dialogMessages.category != DialogMessages.MessageCategory.Error)
            {
                return;
            }

#if UNITY_EDITOR
            Debug.LogError("(#if UNITY_EDITOR) " +
                dialogMessages.urlIfNeeded + " | " +
                dialogMessages.mainMessage + " | " +
                dialogMessages.subMessage + " | " +
                dialogMessages.anyMessage);
#endif

            if (this.m_numberOfErrorStack <= 0)
            {
                return;
            }

            // --------------------------

            if(this.m_errorDialogMessagesStack.Count >= this.m_numberOfErrorStack)
            {
                this.m_errorDialogMessagesStack.RemoveAt(0);
            }

            this.m_errorDialogMessagesStack.Add(messages);

        }

        /// <summary>
        /// Send pause state if needed
        /// </summary>
        // -------------------------------------------------------------------------------------
        protected void sendPauseSignalIfNeeded()
        {

            var pState = SimpleReduxManager.Instance.PauseStateWatcher.state();

            this.m_previousPauseState = pState.pause;

            if (pState.pause == false)
            {
                pState.setState(SimpleReduxManager.Instance.PauseStateWatcher, true);
            }

        }

        /// <summary>
        /// Resume pause state if needed
        /// </summary>
        // -------------------------------------------------------------------------------------
        protected void resumePauseSignalIfNeeded()
        {
            if (this.m_previousPauseState == false)
            {
                var pState = SimpleReduxManager.Instance.PauseStateWatcher.state();
                pState.setState(SimpleReduxManager.Instance.PauseStateWatcher, false);
            }
        }

        /// <summary>
        /// Show ok dialog
        /// </summary>
        /// <param name="messages">System.Object</param>
        /// <param name="okCallback">ok button callback</param>
        /// <returns>suuccess</returns>
        // ----------------------------------------------------------------------------------------
        public virtual bool showOkDialog(System.Object messages, Action okCallback, OkDialogSelectable selectable = OkDialogSelectable.Ok)
        {

            if (this.m_nowShowing)
            {
                return false;
            }

            // set
            {

                this.m_nowShowing = true;

                this.m_okButtonCallback = okCallback;
                this.m_yesButtonCallback = null;
                this.m_noButtonCallback = null;

                // m_refSelectableBeforeShowingDialog
                {

                    Selectable currentSelectable = null;
                    GameObject selected = EventSystem.current.currentSelectedGameObject;

                    if (selected)
                    {
                        currentSelectable = selected.GetComponent<Selectable>();
                    }

                    this.m_refSelectableBeforeShowingDialog = currentSelectable;

                }

            }

            // addErrorStackIfError
            {
                this.addErrorStackIfError(messages);
            }

            // sendPauseSignalIfNeeded
            {
                this.sendPauseSignalIfNeeded();
            }

            // show
            {

                if (this.m_refInputBlocker)
                {
                    this.m_refInputBlocker.startShowing();
                }

                if (this.m_refOkDialog)
                {

                    this.m_refOkDialog.setMessages(messages);

                    this.m_refOkDialog.startShowing(() =>
                    {

#if !(UNITY_IOS || UNITY_ANDROID)

                        if (this.m_refOkButtonSelectable && selectable == OkDialogSelectable.Ok)
                        {
                            this.m_refOkButtonSelectable.Select();
                        }

#endif

                    });

                }

            }

            return true;

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
        public virtual bool showYesNoDialog(
            System.Object messages,
            Action yesCallback,
            Action noCallback,
            YesNoDialogSelectable selectable = YesNoDialogSelectable.No
            )
        {

            if (this.m_nowShowing)
            {
                return false;
            }

            // set
            {

                this.m_nowShowing = true;

                this.m_okButtonCallback = null;
                this.m_yesButtonCallback = yesCallback;
                this.m_noButtonCallback = noCallback;


                // m_refSelectableBeforeShowingDialog
                {

                    Selectable currentSelectable = null;
                    GameObject selected = EventSystem.current.currentSelectedGameObject;

                    if(selected)
                    {
                        currentSelectable = selected.GetComponent<Selectable>();
                    }

                    this.m_refSelectableBeforeShowingDialog = currentSelectable;

                }

            }

            // addErrorStackIfError
            {
                this.addErrorStackIfError(messages);
            }

            // sendPauseSignalIfNeeded
            {
                this.sendPauseSignalIfNeeded();
            }

            // show
            {

                if (this.m_refInputBlocker)
                {
                    this.m_refInputBlocker.startShowing();
                }

                if (this.m_refYesNoDialog)
                {

                    this.m_refYesNoDialog.setMessages(messages);

                    this.m_refYesNoDialog.startShowing(() =>
                    {

#if !(UNITY_IOS || UNITY_ANDROID)

                        if (this.m_refNoButtonSelectable && selectable == YesNoDialogSelectable.No)
                        {
                            this.m_refNoButtonSelectable.Select();
                        }

                        else if (this.m_refYesButtonSelectable && selectable == YesNoDialogSelectable.Yes)
                        {
                            this.m_refYesButtonSelectable.Select();
                        }

#endif

                    });

                }

            }

            return true;

        }

        /// <summary>
        /// Ok button function
        /// </summary>
        // ----------------------------------------------------------------------------------------
        public void onClickOkButton()
        {

            if (this.m_refOkDialog)
            {

                this.m_refOkDialog.startHiding(() =>
                {

                    this.finishDialog(() =>
                    {
                        if (this.m_okButtonCallback != null)
                        {
                            this.m_okButtonCallback();
                        }
                    });

                });

            }

        }

        /// <summary>
        /// Yes button function
        /// </summary>
        // ----------------------------------------------------------------------------------------
        public void onClickYesButton()
        {

            if (this.m_refYesNoDialog)
            {

                this.m_refYesNoDialog.startHiding(() =>
                {

                    this.finishDialog(() =>
                    {
                        if (this.m_yesButtonCallback != null)
                        {
                            this.m_yesButtonCallback();
                        }
                    });

                });

            }

        }

        /// <summary>
        /// No button function
        /// </summary>
        // ----------------------------------------------------------------------------------------
        public void onClickNoButton()
        {

            if (this.m_refYesNoDialog)
            {

                this.m_refYesNoDialog.startHiding(() =>
                {

                    this.finishDialog(() =>
                    {
                        if (this.m_noButtonCallback != null)
                        {
                            this.m_noButtonCallback();
                        }
                    });

                });

            }

        }

    }


}

