using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace SSC
{

    /// <summary>
    /// Start IEnumerator at each start of scene
    /// </summary>
    public class IEnumeratorStartupManager : SingletonMonoBehaviour<IEnumeratorStartupManager>
    {

        /// <summary>
        /// Before any startups, After any startups
        /// </summary>
        public enum BeforeAfter
        {
            Before,
            After
        }

        /// <summary>
        /// Class for IEnumerator startup
        /// </summary>
        protected class IeStruct
        {

            /// <summary>
            /// IEnumeratorStartupScript reference 
            /// </summary>
            public IEnumeratorStartupScript iess = null;

            /// <summary>
            /// Done with success flag
            /// </summary>
            public bool doneSuccess = false;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="_iess"></param>
            public IeStruct(IEnumeratorStartupScript _iess)
            {
                this.iess = _iess;
            }

        }

        /// <summary>
        /// List for brefore
        /// </summary>
        protected List<IeStruct> m_iesListBefore = new List<IeStruct>();

        /// <summary>
        /// List for After
        /// </summary>
        protected List<IeStruct> m_iesListAfter = new List<IeStruct>();

        /// <summary>
        /// The number of parallel loading coroutines
        /// </summary>
        [SerializeField]
        [Tooltip("The number of parallel loading coroutines")]
        protected int m_numberOfCo = 4;

        /// <summary>
        /// Ignore error
        /// </summary>
        [SerializeField]
        [Tooltip("Ignore error")]
        protected bool m_ignoreError = false;

        /// <summary>
        /// error message
        /// </summary>
        protected string m_error = "";

        /// <summary>
        /// Detected new startup object (before)
        /// </summary>
        protected bool m_detectedNewStartupObjectBefore = false;

        /// <summary>
        /// Detected new startup object (after)
        /// </summary>
        protected bool m_detectedNewStartupObjectAfter = false;

        // -------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Error message
        /// </summary>
        public string errorMessage { get { return this.m_error; } }

        // -------------------------------------------------------------------------------------------------------

        /// <summary>
        /// override
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        protected override void initOnAwake()
        {
            this.m_numberOfCo = Math.Max(1, this.m_numberOfCo);
        }

        /// <summary>
        /// Add startup data
        /// </summary>
        /// <param name="iess">IEnumeratorStartupScript</param>
        /// <param name="ba">before or after</param>
        // -------------------------------------------------------------------------------------------------------
        public void addSceneStartupIEnumerator(IEnumeratorStartupScript iess, BeforeAfter ba)
        {

            if(ba == BeforeAfter.Before)
            {
                this.m_iesListBefore.Add(new IeStruct(iess));
                this.m_detectedNewStartupObjectBefore = true;
            }

            else
            {
                this.m_iesListAfter.Add(new IeStruct(iess));
                this.m_detectedNewStartupObjectAfter = true;
            }

        }

        /// <summary>
        /// Denominator of progress
        /// </summary>
        /// <returns>value</returns>
        // -------------------------------------------------------------------------------------------------------
        public int progressDenominator()
        {
            return this.m_iesListBefore.Count + this.m_iesListAfter.Count;
        }

        /// <summary>
        /// Numerator of progress
        /// </summary>
        /// <returns>value</returns>
        // -------------------------------------------------------------------------------------------------------
        public float progressNumerator()
        {

            float ret = 0.0f;

            foreach(var ies in this.m_iesListBefore)
            {
                ret += (ies.doneSuccess) ? 1.0f : ies.iess.progress();
            }

            foreach (var ies in this.m_iesListAfter)
            {
                ret += (ies.doneSuccess) ? 1.0f : ies.iess.progress();
            }

            return ret;

        }

        /// <summary>
        /// Check if detected new startup
        /// </summary>
        /// <returns>detected</returns>
        // -------------------------------------------------------------------------------------------------------
        public bool checkIfDetectedNewStartup(BeforeAfter ba)
        {

            if(ba == BeforeAfter.Before)
            {
                return this.m_detectedNewStartupObjectBefore;
            }

            else
            {
                return this.m_detectedNewStartupObjectAfter;
            }
            
        }

        /// <summary>
        /// Create error message for dialog
        /// </summary>
        /// <returns>error message object</returns>
        // -------------------------------------------------------------------------------------------------------
        public virtual System.Object createErrorMessage()
        {

            DialogMessages messages = new DialogMessages();

            messages.category = DialogMessages.MessageCategory.Error;
            messages.title = "IEnumerator Error";
            messages.mainMessage = this.m_error;
            messages.subMessage = "Retry ?";

            return messages;
        }

        /// <summary>
        /// Clear contents
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        public void clearContents()
        {

            this.setError("");

            this.m_iesListBefore.Clear();
            this.m_iesListAfter.Clear();

        }

        /// <summary>
        /// Start internal IEnumerator startups
        /// </summary>
        /// <param name="ies">IeStruct</param>
        /// <param name="doneCallback">callback when done</param>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------------------------------------
        protected IEnumerator startIEnumeratorInternal(IeStruct ies, Action doneCallback)
        {

            yield return ies.iess.startup();

            if (string.IsNullOrEmpty(ies.iess.error()))
            {
                ies.doneSuccess = true;
            }

            else
            {
                if (!this.m_ignoreError)
                {
                    this.setError(ies.iess.error());
                }

                else
                {
                    ies.doneSuccess = true;
                }
            }

            doneCallback();

        }

        /// <summary>
        /// Start IEnumerator startups
        /// </summary>
        /// <param name="ba">BeforeAfter</param>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------------------------------------
        public IEnumerator startIEnumerator(BeforeAfter ba)
        {

            yield return null;

            // clear error
            {
                this.setError("");
            }

            List<IeStruct> list = (ba == BeforeAfter.Before) ? this.m_iesListBefore : this.m_iesListAfter;

            int listCount = list.Count;
            int listIndex = 0;
            int workingCoCounter = 0;

            while (listIndex < listCount)
            {

                if(this.hasError())
                {
                    break;
                }

                // -------------

                if (workingCoCounter < this.m_numberOfCo)
                {

                    IeStruct ies = list[listIndex++];

                    if(!ies.doneSuccess)
                    {

                        workingCoCounter++;

                        StartCoroutine(this.startIEnumeratorInternal(ies, () =>
                        {
                            workingCoCounter--;
                        }));

                    }

                }

                yield return null;

            }

            while (workingCoCounter > 0)
            {
                yield return null;
            }

            // m_detectedNewStartupObject
            {

                if (ba == BeforeAfter.Before)
                {
                    this.m_detectedNewStartupObjectBefore = false;
                }

                else
                {
                    this.m_detectedNewStartupObjectAfter = false;
                }

            }

        }

        /// <summary>
        /// If error string is not empty
        /// </summary>
        /// <returns>error string is not empty</returns>
        // -------------------------------------------------------------------------------------------------------
        public bool hasError()
        {
            return !string.IsNullOrEmpty(this.m_error);
        }

        /// <summary>
        /// Set error message
        /// </summary>
        /// <param name="error">error message</param>
        // -------------------------------------------------------------------------------------------------------
        protected void setError(string error)
        {

            if (string.IsNullOrEmpty(error))
            {
                this.m_error = error;
            }

            else if (string.IsNullOrEmpty(this.m_error))
            {
                this.m_error = error;
            }

        }

    }

}