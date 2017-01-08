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
        class IeStruct
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
        List<IeStruct> m_iesListBefore = new List<IeStruct>();

        /// <summary>
        /// List for After
        /// </summary>
        List<IeStruct> m_iesListAfter = new List<IeStruct>();

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
        /// error message
        /// </summary>
        string m_error = "";

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

            SimpleReduxManager.Instance.IEnumeratorStartupStateWatcher.addAction(this.onIEnumeratorStartupState);
            SimpleReduxManager.Instance.SceneChangeStateWatcher.addAction(this.onSceneChangeState);

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
            }
            else
            {
                this.m_iesListAfter.Add(new IeStruct(iess));
            }

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

            this.setError("");

            if (clearList)
            {
                this.m_iesListBefore.Clear();
                this.m_iesListAfter.Clear();
            }

        }

        /// <summary>
        /// Action on IEnumeratorStartupStateWatcher
        /// </summary>
        /// <param name="state"></param>
        // -------------------------------------------------------------------------------------------------------
        void onIEnumeratorStartupState(IEnumeratorStartupState state)
        {

            if (
                state.stateEnum == IEnumeratorStartupState.StateEnum.StartBefore ||
                state.stateEnum == IEnumeratorStartupState.StateEnum.RestartBefore
                )
            {
                this.startIEnumeratorStartup(BeforeAfter.Before);
            }

            else if (
                state.stateEnum == IEnumeratorStartupState.StateEnum.StartAfter ||
                state.stateEnum == IEnumeratorStartupState.StateEnum.RestartAfter
                )
            {
                this.startIEnumeratorStartup(BeforeAfter.After);
            }

            else if (state.stateEnum == IEnumeratorStartupState.StateEnum.Clear)
            {
                this.clearAll(true);
            }

            else if (state.stateEnum == IEnumeratorStartupState.StateEnum.DoneBefore)
            {
                this.clearAll(false);
            }

            else if (state.stateEnum == IEnumeratorStartupState.StateEnum.DoneAfter)
            {
                this.clearAll(true);
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
                this.m_progress.progressDenominator = this.m_iesListBefore.Count + this.m_iesListAfter.Count;
            }

            if (updateOnlyDenominator)
            {
                return;
            }

            // numerator
            {

                float numerator = 0.0f;

                foreach (var ies in this.m_iesListBefore)
                {
                    if (ies.iess.isDone())
                    {
                        numerator += 1.0f;
                    }
                }

                foreach (var ies in this.m_iesListAfter)
                {
                    if (ies.iess.isDone())
                    {
                        numerator += 1.0f;
                    }
                }

                foreach (float val in this.m_progress.progressOfCo)
                {
                    numerator += val;
                }

                this.m_progress.progressNumerator = numerator;

            }

        }

        /// <summary>
        /// Start startup
        /// </summary>
        /// <param name="ba">before or after</param>
        // -------------------------------------------------------------------------------------------------------
        void startIEnumeratorStartup(BeforeAfter ba)
        {

            // clear
            {
                this.clearAll(false);
            }

            // startStarter
            {
                StartCoroutine(this.startStarter(ba));
            }

        }

        /// <summary>
        /// Start loading starter
        /// </summary>
        /// <param name="restart">restart flag</param>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------------------------------------
        IEnumerator startStarter(BeforeAfter ba)
        {

            yield return null;

            List<IeStruct> list = (ba == BeforeAfter.Before) ? this.m_iesListBefore : this.m_iesListAfter;

            int counter = 0;

            for (int i = 0; i < this.m_numberOfCo; i++)
            {
                StartCoroutine(this.startEachCo(list, i, () =>
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

            this.funcAtDone(ba);

        }

        /// <summary>
        /// Start parallel loading
        /// </summary>
        /// <param name="list">list</param>
        /// <param name="startIndex">coroutine index</param>
        /// <param name="doneCallback">called when done</param>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------------------------------------
        IEnumerator startEachCo(List<IeStruct> list, int startIndex, Action doneCallback)
        {

            yield return null;

            int size = list.Count;

            for (int i = startIndex; i < size; i += this.m_numberOfCo)
            {

                if (!this.m_ignoreError && this.hasError())
                {
                    break;
                }

                yield return StartCoroutine(this.startIEnumerator(list[i], startIndex));

            }

            doneCallback();

        }

        /// <summary>
        /// Called when all loadings have done with any reason
        /// </summary>
        /// <param name="ba">before or after</param>
        // -------------------------------------------------------------------------------------------------------
        void funcAtDone(BeforeAfter ba)
        {

            var state = SimpleReduxManager.Instance.IEnumeratorStartupStateWatcher.state();

            if (this.hasError())
            {
                state.setState(
                    SimpleReduxManager.Instance.IEnumeratorStartupStateWatcher,
                    (ba == BeforeAfter.Before) ? IEnumeratorStartupState.StateEnum.ErrorBefore : IEnumeratorStartupState.StateEnum.ErrorAfter,
                    this.m_error
                    );
            }

            else
            {

                state.setState(
                    SimpleReduxManager.Instance.IEnumeratorStartupStateWatcher,
                    (ba == BeforeAfter.Before) ? IEnumeratorStartupState.StateEnum.DoneBefore : IEnumeratorStartupState.StateEnum.DoneAfter,
                    ""
                    );

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
        /// <param name="error">error message</param>
        // -------------------------------------------------------------------------------------------------------
        void setError(string error)
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

        /// <summary>
        /// Start IEnumerator startup
        /// </summary>
        /// <param name="ie">IeStruct</param>
        /// <param name="coNumber">coroutine index</param>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------------------------------------
        IEnumerator startIEnumerator(IeStruct ie, int coNumber)
        {

            yield return null;

            if(ie.doneSuccess)
            {
                yield break;
            }

            StartCoroutine(ie.iess.startupBase());

            while (!ie.iess.isDone())
            {
                yield return null;
                this.m_progress.progressOfCo[coNumber] = ie.iess.progress();
            }

            if (string.IsNullOrEmpty(ie.iess.error()))
            {
                ie.doneSuccess = true;
            }

            else
            {
                if(!this.m_ignoreError)
                {
                    this.setError(ie.iess.error());
                }
            }

        }

    }

}