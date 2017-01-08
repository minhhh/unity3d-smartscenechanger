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
        class WwwStruct
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
        List<WwwStruct> m_wwwsList = new List<WwwStruct>();

        /// <summary>
        /// The number of parallel loading coroutines
        /// </summary>
        [SerializeField]
        [Tooltip("The number of parallel loading coroutines")]
        int m_numberOfCo = 4;

        /// <summary>
        /// Ignore error
        /// </summary>
        [SerializeField]
        [Tooltip("Ignore error")]
        bool m_ignoreError = false;

        /// <summary>
        /// Error message
        /// </summary>
        string m_error = "";

        /// <summary>
        /// Error url
        /// </summary>
        string m_errorUrl = "";

        /// <summary>
        /// Progress information
        /// </summary>
        ProgressStruct m_progress = new ProgressStruct();

        /// <summary>
        /// override
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        protected override void initOnAwake()
        {

            this.m_numberOfCo = Math.Max(1, this.m_numberOfCo);
            this.m_progress.clear(this.m_numberOfCo);

            SimpleReduxManager.Instance.WwwStartupStateWatcher.addAction(this.onWwwStartupState);
            SimpleReduxManager.Instance.SceneChangeStateWatcher.addAction(this.onSceneChangeState);

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
            this.calcProgress(true);
        }

        /// <summary>
        /// Denominator of progress
        /// </summary>
        /// <returns>value</returns>
        // -------------------------------------------------------------------------------------------------------
        public int progressDenominator()
        {
            return this.m_progress.progressDenominator;
        }

        /// <summary>
        /// Numerator of progress
        /// </summary>
        /// <returns>value</returns>
        // -------------------------------------------------------------------------------------------------------
        public float progressNumerator()
        {
            return this.m_progress.progressNumerator;
        }

        /// <summary>
        /// Action on WwwStartupStateWatcher
        /// </summary>
        /// <param name="state">current state</param>
        // -------------------------------------------------------------------------------------------------------
        void onWwwStartupState(WwwStartupState state)
        {

            if (state.stateEnum == WwwStartupState.StateEnum.Start)
            {
                this.startWwwStartup();
            }

            else if (state.stateEnum == WwwStartupState.StateEnum.Restart)
            {
                this.startWwwStartup();
            }

            else if (state.stateEnum == WwwStartupState.StateEnum.Clear)
            {
                this.clearAll(true);
            }

            else if (state.stateEnum == WwwStartupState.StateEnum.Done)
            {
                this.clearAll(true);
            }

        }

        /// <summary>
        /// Action on SceneChangeStateWatcher
        /// </summary>
        /// <param name="state">current state</param>
        // -------------------------------------------------------------------------------------------------------
        void onSceneChangeState(SceneChangeState state)
        {

            if (state.stateEnum == SceneChangeState.StateEnum.ScenePlaying)
            {
                this.m_progress.clear(this.m_numberOfCo);
            }

        }

        /// <summary>
        /// Clear params
        /// </summary>
        /// <param name="clearList">clear list</param>
        // -------------------------------------------------------------------------------------------------------
        void clearAll(bool clearList)
        {

            this.setError("", "");

            if (clearList)
            {
                this.m_wwwsList.Clear();
            }

        }

        /// <summary>
        /// If error string is not empty
        /// </summary>
        /// <returns>error string is not empty</returns>
        // -------------------------------------------------------------------------------------------------------
        bool hasError()
        {
            return !string.IsNullOrEmpty(this.m_error);
        }

        /// <summary>
        /// Set error message
        /// </summary>
        /// <param name="url">url if needed</param>
        /// <param name="error">error message</param>
        // -------------------------------------------------------------------------------------------------------
        void setError(string url, string error)
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

        /// <summary>
        /// Calculate progress
        /// </summary>
        /// <param name="updateOnlyDenominator">update only denominator</param>
        // -------------------------------------------------------------------------------------------------------
        void calcProgress(bool updateOnlyDenominator)
        {

            // denominator
            {
                this.m_progress.progressDenominator =  this.m_wwwsList.Count;
            }

            if (updateOnlyDenominator)
            {
                return;
            }

            // numerator
            {

                float numerator = 0.0f;

                foreach (WwwStruct wwws in this.m_wwwsList)
                {
                    if (wwws.doneSuccess)
                    {
                        numerator += 1.0f;
                    }
                }

                foreach (float val in this.m_progress.progressOfCo)
                {
                    if (val < 1.0f)
                    {
                        numerator += val;
                    }
                }

                this.m_progress.progressNumerator = numerator;

            }

        }

        /// <summary>
        /// Start startup
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        void startWwwStartup()
        {

            // clear
            {
                this.clearAll(false);
            }

            // startStarter
            {
                StartCoroutine(this.startStarter());
            }

        }

        /// <summary>
        /// Start loading starter
        /// </summary>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------------------------------------
        IEnumerator startStarter()
        {

            yield return null;

            int counter = 0;

            for (int i = 0; i < this.m_numberOfCo; i++)
            {
                StartCoroutine(this.startEachCo(i, () =>
                {
                    Interlocked.Increment(ref counter);
                }
                ));
            }

            while (counter < this.m_numberOfCo)
            {
                this.calcProgress(false);
                yield return new WaitForSeconds(0.1f);
            }

            this.calcProgress(false);

            yield return null;

            this.funcAtDone();

        }

        /// <summary>
        /// Start parallel loading
        /// </summary>
        /// <param name="startIndex">coroutine index</param>
        /// <param name="doneCallback">called when done</param>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------------------------------------
        IEnumerator startEachCo(int startIndex, Action doneCallback)
        {

            yield return null;

            int size = this.m_wwwsList.Count;

            for (int i = startIndex; i < size; i += this.m_numberOfCo)
            {

                if (this.hasError())
                {
                    break;
                }

                yield return this.startWwwLoading(this.m_wwwsList[i], startIndex);

            }

            doneCallback();

        }

        /// <summary>
        /// Called when all loadings have done with any reason
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        void funcAtDone()
        {

            var state = SimpleReduxManager.Instance.WwwStartupStateWatcher.state();

            if (this.hasError())
            {
                state.setState(
                    SimpleReduxManager.Instance.WwwStartupStateWatcher,
                    WwwStartupState.StateEnum.Error,
                    this.m_error,
                    this.m_errorUrl
                    );
            }

            else
            {

                state.setState(
                    SimpleReduxManager.Instance.WwwStartupStateWatcher,
                    WwwStartupState.StateEnum.Done,
                    "",
                    ""
                    );

            }

        }

        /// <summary>
        /// Download main WWW
        /// </summary>
        /// <param name="wwws">WwwStruct</param>
        /// <param name="coNumber">coroutine index</param>
        /// <returns></returns>
        // -------------------------------------------------------------------------------------------------------
        IEnumerator startWwwLoading(WwwStruct wwws, int coNumber)
        {

            yield return null;

            if (wwws.doneSuccess || this.hasError())
            {
                yield break;
            }

            using (WWW www = new WWW(wwws.url))
            {

                // wait www done
                {

                    while (!www.isDone)
                    {

                        if (wwws.progressFunc != null)
                        {
                            wwws.progressFunc(www);
                        }

                        this.m_progress.progressOfCo[coNumber] = www.progress * 0.999f;

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
                            yield break;
                        }

                    }

                }

            } // using


            // done
            {
                this.m_progress.progressOfCo[coNumber] = 0.0f;
                wwws.doneSuccess = true;
            }

            yield return null;

        }

    }

}
