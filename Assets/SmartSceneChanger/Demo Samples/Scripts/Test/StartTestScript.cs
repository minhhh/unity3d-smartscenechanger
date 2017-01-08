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

            // print warning message
            if(UnityEngine.Random.value > 10.0f) // always false
            {
                Invoke("loadNextScene", 1.5f);
            }

        }

        void loadNextScene()
        {
            SSC.SceneChangeManager.Instance.loadNextScene(this.m_nextSceneName);
        }

#if UNITY_EDITOR

        void OnValidate()
        {

            if (this.m_nextScene && !string.IsNullOrEmpty(this.m_nextScene.name))
            {
                this.m_nextSceneName = this.m_nextScene.name;
            }

        }

#endif

    }

}
