using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSCSample
{

    public class ClickSceneReloadScript : MonoBehaviour
    {

        void Update()
        {

            var state = SSC.SimpleReduxManager.Instance.SceneChangeStateWatcher.state();

            if (Input.GetMouseButtonDown(0) && state.stateEnum == SSC.SceneChangeState.StateEnum.ScenePlaying)
            {
                var sc_state = SSC.SimpleReduxManager.Instance.SceneChangeStateWatcher.state();
                SSC.SceneChangeManager.Instance.loadNextScene(sc_state.nextSceneName);
            }

        }

    }

}
