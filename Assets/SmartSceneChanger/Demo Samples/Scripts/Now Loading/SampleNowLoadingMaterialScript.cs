using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SSCSample
{

    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasGroup))]
    public class SampleNowLoadingMaterialScript : SSC.UiControllerScript
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

        [SerializeField]
        Image m_refImage = null;

        /// <summary>
        /// Reference to RectTransform
        /// </summary>
        protected RectTransform m_refRectTransform = null;

        /// <summary>
        /// Reference to CanvasGroup
        /// </summary>
        protected CanvasGroup m_refCanvasGroup = null;


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

        //Material m_refMaterial = null;

        //readonly float _AlphaUvScaleVal = 0.0f;
        float _LimitAlphaUvScale = 10.0f;

        readonly string _AlphaUvScale = "_AlphaUvScale";

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

            this.m_refImage.material = new Material(this.m_refImage.material);
            this._LimitAlphaUvScale = this.m_refImage.material.GetFloat("_LimitAlphaUvScale");

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
                this.m_refCanvasGroup.alpha = 1.0f;
            }

            // show
            {

                float timeCounter = 0.0f;

                while (timeCounter < this.m_transitionSeconds)
                {

                    this.m_refImage.material.SetFloat(
                        this._AlphaUvScale,
                        Mathf.Lerp(0.0f, this._LimitAlphaUvScale, (timeCounter / this.m_transitionSeconds))
                        );

                    timeCounter += Time.deltaTime;

                    yield return null;

                }

            }

            // finish
            {

                this.m_refImage.material.SetFloat(
                        this._AlphaUvScale,
                        this._LimitAlphaUvScale
                        );

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

                float timeCounter = 0.0f;

                float firstAlpha = this.m_refCanvasGroup.alpha;

                while (timeCounter < this.m_transitionSeconds)
                {

                    this.m_refImage.material.SetFloat(
                        this._AlphaUvScale,
                        Mathf.Lerp(this._LimitAlphaUvScale, 0.0f, (timeCounter / this.m_transitionSeconds))
                        );

                    timeCounter += Time.deltaTime;

                    yield return null;

                }

            }

            // finish
            {

                this.m_refImage.material.SetFloat(
                        this._AlphaUvScale,
                        0.0f
                        );

                this.m_refCanvasGroup.blocksRaycasts = false;
                this.m_refCanvasGroup.alpha = 0.0f;

                this.m_eventAtHidingFinished.Invoke();
            }

        }

        // ------------------------------------------------------------------------------------------
        protected virtual void OnDestroy()
        {
            Destroy(this.m_refImage.material);
        }

    }

}
