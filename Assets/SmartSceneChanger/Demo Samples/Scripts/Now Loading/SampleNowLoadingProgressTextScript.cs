using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SSCSample
{

    [RequireComponent(typeof(Text))]
    public class SampleNowLoadingProgressTextScript : MonoBehaviour
    {

        Text m_refText = null;

        void Awake()
        {
            this.m_refText = this.GetComponent<Text>();
            this.enabled = false;
        }

        void OnEnable()
        {
            this.m_refText.text = "0.000 / 0";
        }

        void Update()
        {

            //if(Time.frameCount % 10 != 0)
            //{
            //    return;
            //}

            int denominator = SSC.SceneChangeManager.Instance.progressDenominator();

            if (denominator > 0)
            {
                this.m_refText.text =
                    SSC.SceneChangeManager.Instance.progressNumerator().ToString("0.000") +
                    " / " +
                    denominator.ToString("0");
            }

        }

    }

}
