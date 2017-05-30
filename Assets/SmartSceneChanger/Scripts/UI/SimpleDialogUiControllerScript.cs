using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SSC
{

    /// <summary>
    /// Simple dialog UI controller
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasGroup))]
    public class SimpleDialogUiControllerScript : DialogUiControllerScript
    {

        /// <summary>
        /// Reference to title text
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to title text")]
        protected Text m_refTitleText;

        /// <summary>
        /// Reference to main text
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to main text")]
        protected Text m_refMainText;

        /// <summary>
        /// Reference to sub text
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to sub text")]
        protected Text m_refSubText;

        /// <summary>
        /// Hide at Awake
        /// </summary>
        [SerializeField]
        [Tooltip("Hide at Awake")]
        protected bool m_hideAtAwake = true;

        /// <summary>
        /// Transition seconds
        /// </summary>
        [SerializeField]
        [Tooltip("Transition seconds")]
        protected float m_transitionSeconds = 0.5f;

        /// <summary>
        /// Main possition
        /// </summary>
        [SerializeField]
        [Tooltip("Main possition")]
        protected Vector2 m_targetPos = Vector2.zero;

        /// <summary>
        /// From position
        /// </summary>
        [SerializeField]
        [Tooltip("From position")]
        protected Vector2 m_transitionRelativeFrom = Vector2.zero;

        /// <summary>
        /// To position
        /// </summary>
        [SerializeField]
        [Tooltip("To position")]
        protected Vector2 m_transitionRelativeTo = Vector2.zero;

        /// <summary>
        /// UnityEvent at showing start
        /// </summary>
        [SerializeField]
        [Tooltip("UnityEvent at showing start")]
        protected UnityEvent m_eventAtShowingStart;

        /// <summary>
        /// UnityEvent at hiding finished
        /// </summary>
        [SerializeField]
        [Tooltip("UnityEvent at hiding finished")]
        protected UnityEvent m_eventAtHidingFinished;

        /// <summary>
        /// Reference to RectTransform
        /// </summary>
        protected RectTransform m_refRectTransform = null;

        /// <summary>
        /// Reference to CanvasGroup
        /// </summary>
        protected CanvasGroup m_refCanvasGroup = null;

        // ------------------------------------------------------------------------------------------

        /// <summary>
        /// Transition seconds
        /// </summary>
        public float transitionSeconds { get { return this.m_transitionSeconds; } set { this.m_transitionSeconds = value; } }

        /// <summary>
        /// Main possition
        /// </summary>
        public Vector2 targetPos { get { return this.m_targetPos; } set { this.m_targetPos = value; } }

        /// <summary>
        /// From position
        /// </summary>
        public Vector2 transitionRelativeFrom { get { return this.m_transitionRelativeFrom; } set { this.m_transitionRelativeFrom = value; } }

        /// <summary>
        /// To position
        /// </summary>
        public Vector2 transitionRelativeTo { get { return this.m_transitionRelativeTo; } set { this.m_transitionRelativeTo = value; } }

        // ------------------------------------------------------------------------------------------

        /// <summary>
        /// Called in Awake
        /// </summary>
        // ------------------------------------------------------------------------------------------
        protected virtual void Awake()
        {

            this.m_refRectTransform = this.GetComponent<RectTransform>();
            this.m_refCanvasGroup = this.GetComponent<CanvasGroup>();

            if (this.m_hideAtAwake)
            {
                this.m_refCanvasGroup.alpha = 0.0f;
                this.m_refCanvasGroup.interactable = false;
                this.m_refCanvasGroup.blocksRaycasts = false;
                this.m_shState = ShowHideState.NowHiding;
            }

        }

        /// <summary>
        /// Clear
        /// </summary>
        // ------------------------------------------------------------------------------------------
        protected virtual void clear()
        {

            if (this.m_refTitleText)
            {
                this.m_refTitleText.text = "Empty Title";
            }

            if (this.m_refMainText)
            {
                this.m_refMainText.text = "Empty Message";
            }

            if (this.m_refSubText)
            {
                this.m_refSubText.text = "";
            }

        }

        /// <summary>
        /// Set DialogMessages
        /// </summary>
        /// <param name="messages">System.Object</param>
        // ------------------------------------------------------------------------------------------
        public override void setMessages(System.Object messages)
        {

            if(messages == null)
            {
                this.clear();
                return;
            }

            // ----------------------------

            DialogMessages dialogMessages = messages as DialogMessages;

            if (dialogMessages == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("(#if UNITY_EDITOR) You should override this function");
#endif
                this.clear();
                return;
            }

            // ----------------------------

            if (this.m_refTitleText)
            {
                this.m_refTitleText.text = dialogMessages.title;
            }

            if (this.m_refMainText)
            {
                this.m_refMainText.text = dialogMessages.mainMessage;
            }

            if (this.m_refSubText)
            {
                this.m_refSubText.text = dialogMessages.subMessage;
            }

        }

        /// <summary>
        /// Show IEnumerator
        /// </summary>
        /// <returns></returns>
        // ------------------------------------------------------------------------------------------
        protected override IEnumerator show()
        {

            // init
            {
                this.m_refCanvasGroup.blocksRaycasts = true;
                this.m_eventAtShowingStart.Invoke();
            }

            // show
            {

                float val = 0.0f;
                float timeCounter = 0.0f;

                Vector2 from = this.m_targetPos + this.m_transitionRelativeFrom;
                float firstAlpha = this.m_refCanvasGroup.alpha;

                while (timeCounter < this.m_transitionSeconds)
                {

                    val = timeCounter / this.m_transitionSeconds;

                    this.m_refRectTransform.anchoredPosition = Vector2.Lerp(from, this.m_targetPos, 1 - (Mathf.Pow(val - 1, 2)));

                    this.m_refCanvasGroup.alpha = Mathf.Lerp(firstAlpha, 1.0f, val);

                    timeCounter += Time.deltaTime;

                    yield return null;

                }

            }

            // finish
            {

                this.m_refRectTransform.anchoredPosition = this.m_targetPos;
                this.m_refCanvasGroup.alpha = 1.0f;

                this.m_refCanvasGroup.interactable = true;

            }

        }

        /// <summary>
        /// Hide IEnumerator
        /// </summary>
        /// <returns></returns>
        // ------------------------------------------------------------------------------------------
        protected override IEnumerator hide()
        {

            // init
            {
                this.m_refCanvasGroup.interactable = false;
            }

            // hide
            {

                float val = 0.0f;
                float timeCounter = 0.0f;

                Vector2 firstPos = this.m_refRectTransform.anchoredPosition;
                Vector2 to = this.m_targetPos + this.m_transitionRelativeTo;
                float firstAlpha = this.m_refCanvasGroup.alpha;

                while (timeCounter < this.m_transitionSeconds)
                {

                    val = timeCounter / this.m_transitionSeconds;

                    this.m_refRectTransform.anchoredPosition = Vector2.Lerp(firstPos, to, 1 - (Mathf.Pow(val - 1, 2)));

                    this.m_refCanvasGroup.alpha = Mathf.Lerp(firstAlpha, 0.0f, val);

                    timeCounter += Time.deltaTime;

                    yield return null;

                }

            }

            // finish
            {

                this.m_refRectTransform.anchoredPosition = this.m_targetPos + this.m_transitionRelativeTo;
                this.m_refCanvasGroup.alpha = 0.0f;

                this.m_refCanvasGroup.blocksRaycasts = false;

                this.m_eventAtHidingFinished.Invoke();
            }

        }

    }

}
