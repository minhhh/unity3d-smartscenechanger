using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSCSample
{

    public class SampleIEnumeratorStartupScript : SSC.IEnumeratorStartupScript
    {

        [SerializeField]
        bool m_usePause = true;

        [SerializeField]
        bool m_errorForTest = false;

        SSC.SceneChangeState m_refSceneChangeState = null;

        bool m_pause = false;

        protected override void initOnStart()
        {
            this.m_refSceneChangeState = SSC.SimpleReduxManager.Instance.SceneChangeStateWatcher.state();
            SSC.SimpleReduxManager.Instance.addPauseStateReceiver(this.onPauseState);
        }

        void onPauseState(SSC.PauseState pState)
        {
            this.m_pause = pState.pause;
        }

        public override IEnumerator startup()
        {

            int sign = (this.m_beforeAfter == SSC.IEnumeratorStartupManager.BeforeAfter.Before) ? 1 : -1;

            for (int i = 0; i < 60; i++)
            {
                this.transform.Rotate(Vector3.one * sign);
                this.setProgress((float)i / 60.0f);
                yield return null;
            }

            if (this.m_errorForTest)
            {
                this.setError("This is Dummy Error");
                this.m_errorForTest = false;
            }

            this.setProgress(1.0f);

            yield return new WaitForSeconds(0.5f);

        }

        void Update()
        {

            if(this.m_usePause && this.m_pause)
            {
                return;
            }

            if (
                this.m_refSceneChangeState.stateEnum == SSC.SceneChangeState.StateEnum.NowLoadingOutro ||
                this.m_refSceneChangeState.stateEnum == SSC.SceneChangeState.StateEnum.ScenePlaying
                )
            {
                this.transform.Rotate(Vector3.one);
            }

        }


    }

}
