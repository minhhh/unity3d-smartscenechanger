using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SSC
{

    /// <summary>
    /// Simple UI controller
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasGroup))]
    public class SimpleUiControllerScript : UiControllerScript
    {

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
        /// Target alpha
        /// </summary>
        [SerializeField]
        [Tooltip("Target alpha")]
        protected float m_targetAlpha = 1.0f;

        /// <summary>
        /// From and to alpha
        /// </summary>
        [SerializeField]
        [Tooltip("From and to alpha")]
        protected float m_fromAndToAlpha = 0.0f;

        /// <summary>
        /// Target possition
        /// </summary>
        [SerializeField]
        [Tooltip("Target possition")]
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

        /// <summary>
        /// Target alpha
        /// </summary>
        public float targetAlpha { get { return this.m_targetAlpha; } set { this.m_targetAlpha = value; } }

        /// <summary>
        /// From and to alpha
        /// </summary>
        public float fromAndToAlpha { get { return this.m_fromAndToAlpha; } set { this.m_fromAndToAlpha = value; } }

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

                this.m_refRectTransform.anchoredPosition = this.m_targetPos + this.m_transitionRelativeTo;

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
                float firstAlpha = this.m_fromAndToAlpha;

                while (timeCounter < this.m_transitionSeconds)
                {

                    val = timeCounter / this.m_transitionSeconds;

                    this.m_refRectTransform.anchoredPosition = Vector2.Lerp(from, this.m_targetPos, 1 - (Mathf.Pow(val - 1, 2)));

                    this.m_refCanvasGroup.alpha = Mathf.Lerp(firstAlpha, this.m_targetAlpha, val);

                    timeCounter += Time.deltaTime;

                    yield return null;

                }

            }
            
            // finish
            {

                this.m_refRectTransform.anchoredPosition = this.m_targetPos;
                this.m_refCanvasGroup.alpha = this.m_targetAlpha;

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

                    this.m_refCanvasGroup.alpha = Mathf.Lerp(firstAlpha, this.m_fromAndToAlpha, val);

                    timeCounter += Time.deltaTime;

                    yield return null;

                }

            }
            
            // finish
            {

                this.m_refRectTransform.anchoredPosition = this.m_targetPos + this.m_transitionRelativeTo;
                this.m_refCanvasGroup.alpha = this.m_fromAndToAlpha;

                this.m_refCanvasGroup.blocksRaycasts = false;

                this.m_eventAtHidingFinished.Invoke();
            }

        }

    }

}
