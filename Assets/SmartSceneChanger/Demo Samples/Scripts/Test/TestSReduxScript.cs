using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSCSample
{

    public class TestSReduxScript : MonoBehaviour
    {

        void Start()
        {
            SSC.SimpleReduxManager.Instance.addSceneChangeStateReceiver(this.onSceneChangeState);
            SSC.SimpleReduxManager.Instance.addPauseStateReceiver(this.onPauseStateState);
        }

        void onSceneChangeState(SSC.SceneChangeState scState)
        {
            print("onSceneChangeState : " + scState.stateEnum.ToString());
        }

        void onPauseStateState(SSC.PauseState pState)
        {
            print("onPauseStateState : " + pState.pause.ToString());
        }

    }

}
