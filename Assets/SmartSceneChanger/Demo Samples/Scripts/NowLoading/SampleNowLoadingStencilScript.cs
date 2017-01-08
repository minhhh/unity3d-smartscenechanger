using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SSCSample
{

    public class SampleNowLoadingStencilScript : SSC.NowLoadingBaseScript
    {

        [SerializeField]
        float m_fadeSeconds = 1.0f;

        [SerializeField]
        Image refImage;

        [SerializeField]
        Image refRingImage;

        [SerializeField]
        Image refRingImageBack;

        Material refMat = null;

        readonly float _AlphaUvScaleVal = 0.0f;
        readonly float _LimitAlphaUvScaleVal = 10.0f;

        readonly string _AlphaUvScale = "_AlphaUvScale";

        void Awake()
        {
            this.refImage.material = new Material(this.refImage.material);
            this.refMat = this.refImage.material;
            this.refRingImage.fillAmount = 0.0f;
            this.refRingImageBack.gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            Destroy(this.refMat);
        }

        IEnumerator introOrOutro(bool is_intro)
        {

            yield return null;

            float time_counter = 0.0f;

            //-------------------

            // stencil
            {

                while (time_counter < this.m_fadeSeconds)
                {

                    if (is_intro)
                    {
                        this.refMat.SetFloat(this._AlphaUvScale, Mathf.Lerp(this._AlphaUvScaleVal, this._LimitAlphaUvScaleVal, (time_counter / this.m_fadeSeconds)));
                    }

                    else
                    {
                        this.refMat.SetFloat(this._AlphaUvScale, Mathf.Lerp(this._AlphaUvScaleVal, this._LimitAlphaUvScaleVal, 1.0f - (time_counter / this.m_fadeSeconds)));
                    }

                    time_counter += Time.deltaTime;

                    yield return null;

                }

            }

            // finish
            {
                this.refMat.SetFloat(this._AlphaUvScale, (is_intro) ? this._LimitAlphaUvScaleVal + 0.1f : this._AlphaUvScaleVal);
            }

            yield return null;

        }

        protected override IEnumerator intro()
        {
            this.refMat.SetFloat(this._AlphaUvScale, this._AlphaUvScaleVal);
            yield return this.introOrOutro(true);
            this.refRingImage.fillAmount = 0.0f;
        }

        protected override IEnumerator outro()
        {
            this.refRingImage.fillAmount = 0.0f;
            yield return this.introOrOutro(false);
        }

        protected override IEnumerator mainLoop()
        {

            float numerator = 0.0f;
            float denominator = 0.0f;
            SSC.SceneChangeState state = SSC.SimpleReduxManager.Instance.SceneChangeStateWatcher.state();

            this.refRingImageBack.gameObject.SetActive(true);

            while (state.stateEnum == SSC.SceneChangeState.StateEnum.NowLoadingMain)
            {

                numerator = SSC.SceneChangeManager.Instance.progressNumerator(true, true, true);
                denominator = SSC.SceneChangeManager.Instance.progressDenominator(true, true, true);

                if (denominator > 0)
                {
                    this.refRingImage.fillAmount = numerator / denominator;
                }

                yield return null;

            }

            numerator = SSC.SceneChangeManager.Instance.progressNumerator(true, true, true);
            denominator = SSC.SceneChangeManager.Instance.progressDenominator(true, true, true);

            this.refRingImage.fillAmount = numerator / denominator;

            yield return new WaitForSeconds(0.5f);

            this.refRingImageBack.gameObject.SetActive(false);
            this.refRingImage.fillAmount = 0.0f;

            yield return null;

        }

    }

}
