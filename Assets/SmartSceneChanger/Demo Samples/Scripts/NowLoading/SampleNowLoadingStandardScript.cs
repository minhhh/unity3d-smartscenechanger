using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SSCSample
{

    public class SampleNowLoadingStandardScript : SSC.NowLoadingBaseScript
    {

        [SerializeField]
        CanvasGroup refCanvasGroup;

        [SerializeField]
        Text refText;

        [SerializeField]
        float m_fadeSeconds = 1.0f;

        IEnumerator introOrOutro(bool is_intro)
        {

            //-------------------

            float time_counter = 0.0f;

            //-------------------

            // fade
            {

                while (time_counter < this.m_fadeSeconds)
                {

                    if (is_intro)
                    {
                        this.refCanvasGroup.alpha = (time_counter / this.m_fadeSeconds);
                    }

                    else
                    {
                        this.refCanvasGroup.alpha = 1.0f - (time_counter / this.m_fadeSeconds);
                    }

                    time_counter += Time.deltaTime;

                    yield return null;

                }

            }

            // finish
            {
                this.refCanvasGroup.alpha = (is_intro) ? 1.0f : 0.0f;
            }

            yield return null;

        }

        protected override IEnumerator intro()
        {
            yield return this.introOrOutro(true);
        }

        protected override IEnumerator outro()
        {
            yield return this.introOrOutro(false);
            this.refText.text = "0.000 : 0";
        }

        protected override IEnumerator mainLoop()
        {

            SSC.SceneChangeState state = SSC.SimpleReduxManager.Instance.SceneChangeStateWatcher.state();

            while (state.stateEnum == SSC.SceneChangeState.StateEnum.NowLoadingMain)
            {
                this.refText.text =
                    SSC.SceneChangeManager.Instance.progressNumerator(true, true, true).ToString("F3") +
                    " : " +
                    SSC.SceneChangeManager.Instance.progressDenominator(true, true, true)
                    ;
                yield return null;
            }

        }

    }

}
