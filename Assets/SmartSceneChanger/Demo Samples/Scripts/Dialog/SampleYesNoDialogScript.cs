using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SSCSample
{

    [RequireComponent(typeof(RectTransform))]
    public class SampleYesNoDialogScript : SSC.YesNoDialogBaseScript
    {
        [SerializeField]
        protected CanvasGroup refCanvasGroup;

        [SerializeField]
        Text refText;

        [SerializeField]
        float m_fadeFeconds = 0.3f;

        [SerializeField]
        float m_movePosY = -100.0f;

        RectTransform refThisRect = null;

        Vector2 m_oriPos = Vector3.zero;

        void Awake()
        {
            this.refThisRect = this.GetComponent<RectTransform>();
            this.m_oriPos = this.refThisRect.anchoredPosition;
        }

        IEnumerator introOrOutro(bool is_intro)
        {

            //-------------------

            float time_counter = 0.0f;
            float val01 = 0.0f;
            float move = 0.0f;

            //-------------------

            // fade
            {

                while (time_counter < this.m_fadeFeconds)
                {

                    val01 = (time_counter / this.m_fadeFeconds);

                    // fade
                    {
                        this.refCanvasGroup.alpha = (is_intro) ? val01 : 1.0f - val01;
                    }

                    // pos
                    {
                        move = (is_intro) ? Mathf.Lerp(this.m_movePosY, 0.0f, val01) : Mathf.Lerp(0.0f, this.m_movePosY, val01);
                        this.refThisRect.anchoredPosition = this.m_oriPos + new Vector2(0.0f, move);
                    }

                    // timer
                    {
                        time_counter += Time.deltaTime;
                    }

                    yield return null;

                }

            }

            // finish
            {
                this.refCanvasGroup.alpha = (is_intro) ? 1.0f : 0.0f;
                this.refThisRect.anchoredPosition = this.m_oriPos + new Vector2(0.0f, (is_intro) ? 0.0f : this.m_movePosY);
            }

        }

        protected override IEnumerator intro()
        {

            // init
            {
                this.refCanvasGroup.alpha = 0.0f;
                this.refThisRect.anchoredPosition = this.m_oriPos;
                this.refText.text = this.refState.message;

                if (!string.IsNullOrEmpty(this.refState.urlForIfNeeded))
                {
                    this.refText.text += "\n" + this.refState.urlForIfNeeded.GetHashCode();
                    this.refText.text += "\n\n" + "Try Again ?";
                }

                Debug.LogError(this.refState.message + " | " + this.refState.urlForIfNeeded);
            }

            {
                yield return this.introOrOutro(true);
            }

            yield return null;

        }

        protected override IEnumerator outro()
        {

            // init
            {
                this.refCanvasGroup.alpha = 0.0f;
                this.refThisRect.anchoredPosition = this.m_oriPos;
            }

            {
                yield return this.introOrOutro(false);
            }

            yield return null;
        }

        protected override IEnumerator mainLoop()
        {
            yield return null;
        }

        protected override void onClickYes()
        {

        }

        protected override void onClickNo()
        {

        }

    }

}
