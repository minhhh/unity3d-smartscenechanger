using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSCSample
{

    public class RotateScript : MonoBehaviour
    {

        [SerializeField]
        [Range(0, 1)]
        int m_type = 0;

        SSC.SceneChangeState m_refSceneChangeState = null;

        void Start()
        {
            this.m_refSceneChangeState = SSC.SimpleReduxManager.Instance.SceneChangeStateWatcher.state();
        }

        void Update()
        {

            if (this.m_type == 0)
            {
                if (
                    this.m_refSceneChangeState.stateEnum == SSC.SceneChangeState.StateEnum.NowLoadingOutro ||
                    this.m_refSceneChangeState.stateEnum == SSC.SceneChangeState.StateEnum.ScenePlaying
                    )
                {
                    this.transform.Rotate(Vector3.one);
                }
            }

            else
            {
                if (
                    this.m_refSceneChangeState.stateEnum == SSC.SceneChangeState.StateEnum.NowLoadingOutro ||
                    this.m_refSceneChangeState.stateEnum == SSC.SceneChangeState.StateEnum.NowLoadingIntro ||
                    this.m_refSceneChangeState.stateEnum == SSC.SceneChangeState.StateEnum.ScenePlaying
                    )
                {
                    this.transform.Rotate(Vector3.one);
                }
            }

        }

    }

}
