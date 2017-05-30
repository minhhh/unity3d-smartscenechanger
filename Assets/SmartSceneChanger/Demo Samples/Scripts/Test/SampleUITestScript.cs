using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSCSample
{

    public class SampleUITestScript : MonoBehaviour
    {

        void Start()
        {
            SSC.SimpleReduxManager.Instance.SceneChangeStateWatcher.addAction(this.onSceneChangeState);
        }

        void onSceneChangeState(SSC.SceneChangeState scState)
        {

            if(scState.stateEnum == SSC.SceneChangeState.StateEnum.ScenePlaying)
            {
                if (SSC.SceneUiManager.Instance)
                {
                    SSC.SceneUiManager.Instance.showUi("All", true, false);
                }
            }

        }

        void Update()
        {

            if(Input.GetMouseButtonDown(0))
            {

                List<string> temp = SSC.CommonUiManager.Instance.currentShowingUiCopy;

                if (temp.Contains("Pause"))
                {
                    temp.Remove("Pause");
                    SSC.CommonUiManager.Instance.showUi(temp, true, false);
                }

            }

            else if (Input.GetMouseButtonDown(1))
            {

                if (SSC.SceneUiManager.Instance)
                {

                    var list = SSC.SceneUiManager.Instance.currentShowingUiAsReadOnly;

                    if(list.Count > 0)
                    {
                        SSC.SceneUiManager.Instance.showUi("", true, false);
                    }

                    else
                    {
                        SSC.SceneUiManager.Instance.showUi("All", true, false);
                    }
                    
                }

            }

        }

        public void onClickPauseButton()
        {
            
            List<string> temp = SSC.CommonUiManager.Instance.currentShowingUiCopy;

            if (!temp.Contains("Pause"))
            {
                temp.Add("Pause");
                SSC.CommonUiManager.Instance.showUi(temp, true, false);
            }

        }

        public void onClickReloadButton()
        {
            SSC.SceneChangeManager.Instance.loadNextScene(SSC.SceneChangeManager.Instance.nowLoadingSceneName);
        }

    }

}
