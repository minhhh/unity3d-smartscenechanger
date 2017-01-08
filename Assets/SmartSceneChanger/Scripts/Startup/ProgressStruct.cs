using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSC
{

    /// <summary>
    /// Class for progress
    /// </summary>
    public class ProgressStruct
    {

        /// <summary>
        /// Progress of each coroutine 
        /// </summary>
        public List<float> progressOfCo = new List<float>();

        /// <summary>
        /// Progress denominator
        /// </summary>
        public int progressDenominator = 0;

        /// <summary>
        /// Progress numerator
        /// </summary>
        public float progressNumerator = 0.0f;

        /// <summary>
        /// Clear params
        /// </summary>
        /// <param name="numberOfCo"></param>
        public void clear(int numberOfCo)
        {
            this.progressOfCo = new List<float>(new float[numberOfCo]);
            this.progressDenominator = 0;
            this.progressNumerator = 0;
        }

    }

}
