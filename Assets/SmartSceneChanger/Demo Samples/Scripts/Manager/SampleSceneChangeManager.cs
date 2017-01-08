using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSCSample
{

    public class SampleSceneChangeManager : SSC.SceneChangeManager
    {

        [SerializeField]
        [Range(0, 1)]
        int m_index = 0;

        protected override SSC.NowLoadingBaseScript chooseNowLoading()
        {

            int count = this.refNowloadings.Count;

            if (count <= 0)
            {
                return null;
            }

            return this.refNowloadings[this.m_index];

        }

    }

}
