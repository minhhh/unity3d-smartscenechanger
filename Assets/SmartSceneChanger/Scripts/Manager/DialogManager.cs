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
        /// Reference to progress dialog UiControllerScript
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to progress dialog UiControllerScript")]
        protected ProgressDialogUiControllerScript m_refProgressDialog;

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
        /// Should send unpdause signal
        /// </summary>
        protected bool m_shouldUnpause = false;

        /// <summary>
        /// Reference to Selectable before showing dialog
        /// </summary>
        protected Selectable m_refSelectableBeforeShowingDialog = null;

        /// <summary>
        /// Consecutive showing dialogs flag
        /// </summary>
        protected bool m_consecutiveShowing = false;

        // ----------------------------------------------------------------------------------------

        /// <summary>
        /// If now showing dialog
        /// </summary>
        public bool nowShowing { get { return this.m_nowShowing; } }

        /// <summary>
        /// Error stack
        /// </summary>
        public List<System.Object> errorDialogMessagesStack { get { return this.m_errorDialogMessagesStack; } }

        /// <summary>
        /// Consecutive showing dialogs flag
        /// </summary>
        public bool consecutiveShowing { get { return this.m_consecutiveShowing; } set { this.m_consecutiveShowing = value; } }

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

                    // resume selectable
                    {

                        if (this.m_refSelectableBeforeShowingDialog)
                        {
                            EventSystem.current.SetSelectedGameObject(this.m_refSelectableBeforeShowingDialog.gameObject);
                        }

                        else
                        {
                            EventSystem.current.SetSelectedGameObject(null);
                        }

                    }

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

                // resume selectable
                {

                    if (this.m_refSelectableBeforeShowingDialog)
                    {
                        EventSystem.current.SetSelectedGameObject(this.m_refSelectableBeforeShowingDialog.gameObject);
                    }

                    else
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                    }

                }

                if (finishDoneCallback != null)
                {
                    finishDoneCallback();
                }

            }

            // m_consecutiveShowing
            {
                this.m_consecutiveShowing = false;
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

            if (pState.pause == false)
            {
                this.m_shouldUnpause = true;
                pState.setState(SimpleReduxManager.Instance.PauseStateWatcher, true);
            }

        }

        /// <summary>
        /// Resume pause state if needed
        /// </summary>
        // -------------------------------------------------------------------------------------
        protected void resumePauseSignalIfNeeded()
        {

            if (this.m_shouldUnpause == true)
            {
                var pState = SimpleReduxManager.Instance.PauseStateWatcher.state();
                pState.setState(SimpleReduxManager.Instance.PauseStateWatcher, false);
            }

            this.m_shouldUnpause = false;

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

            if (this.m_nowShowing && !this.m_consecutiveShowing)
            {
                return false;
            }

            // set
            {

                this.m_nowShowing = true;

                this.m_okButtonCallback = okCallback;
                this.m_yesButtonCallback = null;
                this.m_noButtonCallback = null;

                this.updateRefSelectableBeforeShowingDialog();

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
        // ----------------------------------------------------------------------------------------
        public virtual bool showYesNoDialog(
            System.Object messages,
            Action yesCallback,
            Action noCallback,
            YesNoDialogSelectable selectable = YesNoDialogSelectable.No
            )
        {

            if (this.m_nowShowing && !this.m_consecutiveShowing)
            {
                return false;
            }

            // set
            {

                this.m_nowShowing = true;

                this.m_okButtonCallback = null;
                this.m_yesButtonCallback = yesCallback;
                this.m_noButtonCallback = noCallback;

                this.updateRefSelectableBeforeShowingDialog();

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
        /// Show progress dialog
        /// </summary>
        /// <param name="messages">DialogMessages</param>
        /// <param name="showDoneCallback">callback when showing is done</param>
        /// <returns>suuccess</returns>
        // ----------------------------------------------------------------------------------------
        public virtual bool showProgressDialog(System.Object messages, Action showDoneCallback)
        {

            if (this.m_nowShowing && !this.m_consecutiveShowing)
            {
                return false;
            }

            // set
            {

                this.m_nowShowing = true;

                this.updateRefSelectableBeforeShowingDialog();

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

                if (this.m_refProgressDialog)
                {

                    this.m_refProgressDialog.setMessages(messages);
                    this.m_refProgressDialog.setProgress(0.0f);

                    this.m_refProgressDialog.startShowing(showDoneCallback);

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

                    if(this.m_consecutiveShowing)
                    {
                        if (this.m_okButtonCallback != null)
                        {
                            this.m_okButtonCallback();
                        }
                    }

                    else
                    {
                        this.finishDialog(() =>
                        {
                            if (this.m_okButtonCallback != null)
                            {
                                this.m_okButtonCallback();
                            }
                        });
                    }

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

                    if (this.m_consecutiveShowing)
                    {
                        if (this.m_yesButtonCallback != null)
                        {
                            this.m_yesButtonCallback();
                        }
                    }

                    else
                    {
                        this.finishDialog(() =>
                        {
                            if (this.m_yesButtonCallback != null)
                            {
                                this.m_yesButtonCallback();
                            }
                        });
                    }

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

                    if (this.m_consecutiveShowing)
                    {
                        if (this.m_noButtonCallback != null)
                        {
                            this.m_noButtonCallback();
                        }
                    }

                    else
                    {
                        this.finishDialog(() =>
                        {
                            if (this.m_noButtonCallback != null)
                            {
                                this.m_noButtonCallback();
                            }
                        });
                    }

                });

            }

        }

        /// <summary>
        /// Set progress value
        /// </summary>
        /// <param name="val">progress value</param>
        // ------------------------------------------------------------------------------------------
        public void setProgress(float val)
        {

            if (this.m_refProgressDialog)
            {
                this.m_refProgressDialog.setProgress(val);
            }

        }

        /// <summary>
        /// Finish progress dialog
        /// </summary>
        // ----------------------------------------------------------------------------------------
        public void finishProgressDialog(Action hideDoneCallback = null)
        {

            if (this.m_refProgressDialog)
            {

                this.m_refProgressDialog.setProgress(1.0f);

                this.m_refProgressDialog.startHiding(() =>
                {

                    if (this.m_consecutiveShowing)
                    {

                        if(hideDoneCallback != null)
                        {
                            hideDoneCallback();
                        }

                    }

                    else
                    {
                        this.finishDialog(hideDoneCallback);
                    }
                        
                });

            }

        }

        /// <summary>
        /// Update m_refSelectableBeforeShowingDialog
        /// </summary>
        // ----------------------------------------------------------------------------------------
        protected void updateRefSelectableBeforeShowingDialog()
        {

            Selectable currentSelectable = null;
            GameObject selected = EventSystem.current.currentSelectedGameObject;

            if (selected)
            {

                currentSelectable = selected.GetComponent<Selectable>();

                if (currentSelectable &&
                currentSelectable != this.m_refOkButtonSelectable &&
                currentSelectable != this.m_refYesButtonSelectable &&
                currentSelectable != this.m_refNoButtonSelectable
                )
                {
                    this.m_refSelectableBeforeShowingDialog = currentSelectable;
                }

            }

            else
            {
                this.m_refSelectableBeforeShowingDialog = null;
            }

        }

    }


}

