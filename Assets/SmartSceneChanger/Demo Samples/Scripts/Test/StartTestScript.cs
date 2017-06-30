using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSCSample
{

    public class StartTestScript : MonoBehaviour
    {

#if UNITY_EDITOR

        [SerializeField]
        UnityEngine.Object m_nextScene;

#endif

        [HideInInspector]
        [SerializeField]
        string m_nextSceneName = "";

        [SerializeField]
        int m_fps = 60;

        void Start()
        {

            Application.targetFrameRate = this.m_fps;
            Invoke("loadNextScene", 1.0f);

        }

        void loadNextScene()
        {

            // SSC.SceneChangeManager.Instance.currentNowLoadingIdentifier = "NowLoading1";

            // SSC.SceneChangeManager.Instance.loadNextScene(this.m_nextSceneName);
            // SSC.SceneChangeManager.Instance.loadNextScene("AAA");

            SSC.SceneChangeManager.Instance.loadNextScene(this.m_nextSceneName, true, "", "All");

        }

        void OnValidate()
        {

#if UNITY_EDITOR

            if (this.m_nextScene && !string.IsNullOrEmpty(this.m_nextScene.name))
            {
                this.m_nextSceneName = this.m_nextScene.name;
            }

            else
            {
                this.m_nextSceneName = "";
            }
#endif

        }

    }

}
