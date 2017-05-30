using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SSC
{

    [RequireComponent(typeof(RectTransform))]
    public class LoadingSpinnerScript : MonoBehaviour
    {

        [SerializeField]
        float m_speed = 4f;

        RectTransform m_refRectTransform = null;

        void Awake()
        {
            this.m_refRectTransform = this.GetComponent<RectTransform>();

            this.m_refRectTransform.rotation = Quaternion.Euler(0.0f, 0.0f, UnityEngine.Random.Range(0.0f, 360.0f));

            this.enabled = false;
        }

        void Update()
        {

            float rot = this.m_refRectTransform.rotation.eulerAngles.z;

            rot = (rot + this.m_speed) % 360.0f;

            this.m_refRectTransform.rotation = Quaternion.Euler(0.0f, 0.0f, rot);

        }

    }

}
