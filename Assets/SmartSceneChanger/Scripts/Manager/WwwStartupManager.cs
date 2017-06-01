using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace SSC
{

    /// <summary>
    /// Class for WWW startup
    /// </summary>
    public class WwwStartupManager : SingletonMonoBehaviour<WwwStartupManager>
    {

        /// <summary>
        /// Class for loading WWW
        /// </summary>
        protected class WwwStruct
        {

            /// <summary>
            /// Url
            /// </summary>
            public string url = "";

            /// <summary>
            /// Success function
            /// </summary>
            public Action<WWW> successFunc = null;

            /// <summary>
            /// Failed function
            /// </summary>
            public Action<WWW> failedFunc = null;

            /// <summary>
            /// Progree function
            /// </summary>
            public Action<WWW> progressFunc = null;

            /// <summary>
            /// Done with success flag
            /// </summary>
            public bool doneSuccess = false;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="_url">url</param>
            /// <param name="_successFunc">successFunc</param>
            /// <param name="_failedFunc">failedFunc</param>
            /// <param name="_progressFunc">progressFunc</param>
            public WwwStruct(string _url, Action<WWW> _successFunc, Action<WWW> _failedFunc, Action<WWW> _progressFunc)
            {
                this.url = _url;
                this.successFunc = _successFunc;
                this.failedFunc = _failedFunc;
                this.progressFunc = _progressFunc;
            }

        }

        /// <summary>
        /// WwwStruct list
        /// </summary>
        protected List<WwwStruct> m_wwwsList = new List<WwwStruct>();

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
        /// ThreadPriority
        /// </summary>
        [SerializeField]
        [Tooltip("ThreadPriority")]
        protected UnityEngine.ThreadPriority m_threadPriority = UnityEngine.ThreadPriority.Low;

        /// <summary>
        /// Error seconds for timeout
        /// </summary>
        [SerializeField]
        [Tooltip("Error seconds for timeout")]
        protected float m_noProgressTimeOutSeconds = 0.0f;

        /// <summary>
        /// Error message
        /// </summary>
        protected string m_error = "";

        /// <summary>
        /// Error url
        /// </summary>
        protected string m_errorUrl = "";

        /// <summary>
        /// Connection Timeout message
        /// </summary>
        protected readonly string ConnectionTimeout = "Connection Timeout";

        /// <summary>
        /// Current co progress
        /// </summary>
        protected List<float> m_currentCoProgresses = new List<float>();

        /// <summary>
        /// Detected new startup object
        /// </summary>
        protected bool m_detectedNewStartupObject = false;

        // -------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Error url
        /// </summary>
        public string errorUrl { get { return this.m_errorUrl; } }

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
            this.m_currentCoProgresses = new List<float>(new float[this.m_numberOfCo]);
        }

        /// <summary>
        /// Add startup data
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="success_func">success function</param>
        /// <param name="failed_func">failed function</param>
        /// <param name="progress_func">progress function</param>
        // -------------------------------------------------------------------------------------------------------
        public void addSceneStartupWww(string url, Action<WWW> success_func, Action<WWW> failed_func, Action<WWW> progress_func)
        {
            this.m_wwwsList.Add(new WwwStruct(url, success_func, failed_func, progress_func));
            this.m_detectedNewStartupObject = true;
        }

        /// <summary>
        /// Denominator of progress
        /// </summary>
        /// <returns>value</returns>
        // -------------------------------------------------------------------------------------------------------
        public int progressDenominator()
        {
            return this.m_wwwsList.Count;
        }

        /// <summary>
        /// Numerator of progress
        /// </summary>
        /// <returns>value</returns>
        // -------------------------------------------------------------------------------------------------------
        public float progressNumerator()
        {

            float ret = 0.0f;

            foreach (var wwws in this.m_wwwsList)
            {
                if(wwws.doneSuccess)
                {
                    ret += 1.0f;
                }
            }

            foreach (var val in this.m_currentCoProgresses)
            {
                ret += val;
            }

            return ret;

        }

        /// <summary>
        /// Check if detected new startup
        /// </summary>
        /// <returns>detected</returns>
        // -------------------------------------------------------------------------------------------------------
        public bool checkIfDetectedNewStartup()
        {
            return this.m_detectedNewStartupObject;
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
            messages.title = "WWW Error";
            messages.urlIfNeeded = this.m_errorUrl;
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

            this.setError("", "");

            this.m_wwwsList.Clear();

            // m_currentCoProgresses
            {

                for (int i = this.m_currentCoProgresses.Count - 1; i >= 0; i--)
                {
                    this.m_currentCoProgresses[i] = 0.0f;
                }

            }

        }

        /// <summary>
        /// Start internal www loadings
        /// </summary>
        /// <param name="wwws">WwwStruct</param>
        /// <param name="coNumber">coroutine number</param>
        /// <param name="doneCallback">callback when done</param>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------------------------------------
        protected IEnumerator startWwwStartupInternal(WwwStruct wwws, int coNumber, Action doneCallback)
        {

            float noProgressTimer = 0.0f;
            float previousProgress = 0.0f;

            using (WWW www = new WWW(wwws.url))
            {

                www.threadPriority = this.m_threadPriority;

                // wait www done
                {

                    while (!www.isDone)
                    {

                        if (wwws.progressFunc != null)
                        {
                            wwws.progressFunc(www);
                        }

                        // m_currentCoProgresses
                        {
                            this.m_currentCoProgresses[coNumber] = www.progress * 0.999f;
                        }

                        // timeout
                        {

                            if(this.m_noProgressTimeOutSeconds > 0.0f)
                            {

                                if (previousProgress == www.progress)
                                {
                                    noProgressTimer += Time.deltaTime;
                                }

                                else
                                {
                                    noProgressTimer = 0.0f;
                                }

                                previousProgress = www.progress;

                                if(noProgressTimer >= this.m_noProgressTimeOutSeconds)
                                {
                                    this.setError(www.url, this.ConnectionTimeout);
                                    doneCallback();
                                    www.Dispose();
                                    yield break;
                                }

                            }

                        }

                        yield return null;

                    }

                    if (wwws.progressFunc != null)
                    {
                        wwws.progressFunc(www);
                    }

                    yield return null;

                }

                // success or fail
                {

                    // success
                    if (string.IsNullOrEmpty(www.error))
                    {

                        if (wwws.successFunc != null)
                        {
                            wwws.successFunc(www);
                        }

                    }

                    // fail
                    else
                    {


                        if (wwws.failedFunc != null)
                        {
                            wwws.failedFunc(www);
                        }

                        if (!this.m_ignoreError)
                        {
                            this.setError(www.url, www.error);
                            doneCallback();
                            www.Dispose();
                            yield break;
                        }

                    }

                }

            } // using

            // done
            {
                this.m_currentCoProgresses[coNumber] = 0.0f;
                wwws.doneSuccess = true;
            }

            doneCallback();

        }

        /// <summary>
        /// Start www loadings
        /// </summary>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------------------------------------
        public IEnumerator startWwwStartup()
        {

            yield return null;

            // clear error
            {
                this.setError("", "");
            }

            int listCount = this.m_wwwsList.Count;
            int listIndex = 0;
            int workingCoCounter = 0;

            while (listIndex < listCount)
            {

                if (this.hasError())
                {
                    break;
                }

                // -------------

                if (workingCoCounter < this.m_numberOfCo)
                {

                    WwwStruct wwws = this.m_wwwsList[listIndex++];

                    if (!wwws.doneSuccess)
                    {

                        StartCoroutine(this.startWwwStartupInternal(wwws, workingCoCounter++, () =>
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
                this.m_detectedNewStartupObject = false;
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
        /// <param name="url">url if needed</param>
        /// <param name="error">error message</param>
        // -------------------------------------------------------------------------------------------------------
        protected void setError(string url, string error)
        {

            if (string.IsNullOrEmpty(error))
            {
                this.m_errorUrl = url;
                this.m_error = error;
            }

            else if (string.IsNullOrEmpty(this.m_error))
            {
                this.m_errorUrl = url;
                this.m_error = error;
            }

        }

    }

}
