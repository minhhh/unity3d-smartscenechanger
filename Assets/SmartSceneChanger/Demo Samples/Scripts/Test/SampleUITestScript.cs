﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSCSample
{

    public class SampleUITestScript : MonoBehaviour
    {

        void Start()
        {
            SSC.SimpleReduxManager.Instance.addSceneChangeStateReceiver(this.onSceneChangeState);
        }

        void onSceneChangeState(SSC.SceneChangeState scState)
        {


            // You can use this function
            // SSC.SceneChangeManager.Instance.loadNextScene(this.m_nextSceneName, true, "", "All");
            // See StartTestScript.loadNextScene()

            // comment out
            //if(scState.stateEnum == SSC.SceneChangeState.StateEnum.ScenePlaying)
            //{
            //    if (SSC.SceneUiManager.isAvailable())
            //    {
            //        SSC.SceneUiManager.Instance.showUi("All", true, false);
            //    }
            //}

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

                if (SSC.SceneUiManager.isAvailable())
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
            SSC.SceneChangeManager.Instance.loadNextScene(SSC.SceneChangeManager.Instance.nowLoadingSceneName, true, "", "All");
        }

    }

}
