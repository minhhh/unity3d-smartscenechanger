using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SSCSample
{

    [RequireComponent(typeof(Slider))]
    public class SampleNowLoadingSliderScript : MonoBehaviour
    {

        Slider m_refSlider = null;

        void Awake()
        {
            this.m_refSlider = this.GetComponent<Slider>();
            this.enabled = false;
        }

        void OnEnable()
        {
            this.m_refSlider.value = 0.0f;
        }

        void Update()
        {

            int denominator = SSC.SceneChangeManager.Instance.progressDenominator();

            if(denominator <= 0)
            {
                // this.m_refSlider.value = 0.0f;
            }

            else
            {

                float value = Mathf.Clamp01(SSC.SceneChangeManager.Instance.progressNumerator() / (float)denominator);

                if(this.m_refSlider.value != value)
                {
                    this.m_refSlider.value = value;
                }
                
            }

        }

    }

}
