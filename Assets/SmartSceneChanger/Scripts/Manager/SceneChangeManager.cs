using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SSC
{

    /// <summary>
    /// Singleton class for changing scene
    /// </summary>
    public class SceneChangeManager : SingletonMonoBehaviour<SceneChangeManager>
    {

#if UNITY_EDITOR

        /// <summary>
        /// Title scene object for failure
        /// </summary>
        [SerializeField]
        [Tooltip("Title scene object for failure")]
        UnityEngine.Object m_titleScene;

#endif

        /// <summary>
        /// Title scene name of m_titleScene
        /// </summary>
        [HideInInspector]
        [SerializeField]
        string m_titleSceneName = "";

        /// <summary>
        /// Object list for Now Loading
        /// </summary>
        [SerializeField]
        [Tooltip("NowLoadingBaseScript reference list")]
        protected List<NowLoadingBaseScript> refNowloadings;

        /// <summary>
        /// Current Now Loading object
        /// </summary>
        NowLoadingBaseScript refCurrentNl = null;

        /// <summary>
        /// override
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        protected override void initOnAwake()
        {
            SimpleReduxManager.Instance.SceneChangeStateWatcher.addAction(this.onSceneChangeState);
            SimpleReduxManager.Instance.WwwStartupStateWatcher.addAction(this.onWwwStartupState);
            SimpleReduxManager.Instance.AssetBundleStartupStateWatcher.addAction(this.onAssetBundleStartupState);
            SimpleReduxManager.Instance.IEnumeratorStartupStateWatcher.addAction(this.onIEnumeratorStartupState);
        }

        /// <summary>
        /// Load next scene
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        public void loadNextScene(string sceneName)
        {

            SceneChangeState state = SimpleReduxManager.Instance.SceneChangeStateWatcher.state();

            if (state.stateEnum != SceneChangeState.StateEnum.ScenePlaying)
            {
                Debug.LogWarning("Discard loadNextScene for bad state : " + sceneName + " : " + state.stateEnum);
                return;
            }

            // set state
            {
                state.setState(
                    SimpleReduxManager.Instance.SceneChangeStateWatcher,
                    SceneChangeState.StateEnum.NowLoadingIntro,
                    sceneName
                    );
            }

        }

        /// <summary>
        /// Start Now Loading
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        void startNowLoading()
        {

            this.refCurrentNl = this.chooseNowLoading();

            if (!this.refCurrentNl)
            {
                Debug.LogError("no refNowloadings");
                return;
            }

            this.refCurrentNl.gameObject.SetActive(true);

            StartCoroutine(this.refCurrentNl.startIntro());

        }

        /// <summary>
        /// Finish Now Loading
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        void finishNowLoading()
        {

            if (this.refCurrentNl)
            {
                this.refCurrentNl.gameObject.SetActive(false);
            }

        }

        /// <summary>
        /// Action on SceneChangeStateWatcher
        /// </summary>
        /// <param name="state">current state</param>
        // -------------------------------------------------------------------------------------------------------
        void onSceneChangeState(SceneChangeState state)
        {

            if (state.stateEnum == SceneChangeState.StateEnum.NowLoadingIntro)
            {
                this.startNowLoading();
            }

            else if (state.stateEnum == SceneChangeState.StateEnum.NowLoadingMain)
            {
                StartCoroutine(this.refCurrentNl.startMainLoop());
                StartCoroutine(this.loadNextScene_IE(state.nextSceneName));
            }

            else if (state.stateEnum == SceneChangeState.StateEnum.NowLoadingOutro)
            {
                StartCoroutine(this.refCurrentNl.startOutro());
            }

            else if (state.stateEnum == SceneChangeState.StateEnum.InnerChange)
            {
                StartCoroutine(this.loadNextScene_IE(state.nextSceneName));
            }

            else if (state.stateEnum == SceneChangeState.StateEnum.ScenePlaying)
            {
                this.finishNowLoading();
            }

        }

        /// <summary>
        /// Action on WwwStartupStateWatcher
        /// </summary>
        /// <param name="ws_state">current state</param>
        // -------------------------------------------------------------------------------------------------------
        void onWwwStartupState(WwwStartupState ws_state)
        {

            if (ws_state.stateEnum == WwwStartupState.StateEnum.Done)
            {
                this.startIEnumeratorAfterStartup();
            }

            else if (ws_state.stateEnum == WwwStartupState.StateEnum.Error)
            {

                var ynd_state = SimpleReduxManager.Instance.YesNoDialogStateWatcher.state();

                ynd_state.setState(
                    SimpleReduxManager.Instance.YesNoDialogStateWatcher,
                    YesNoDialogState.StateEnum.Show,
                    ws_state.error,
                    ws_state.url,
                    this.restartWwwStartup,
                    this.cancelStartup
                    );
            }

        }

        /// <summary>
        /// Action on AssetBundleStartupStateWatcher
        /// </summary>
        /// <param name="ab_state">current state</param>
        // -------------------------------------------------------------------------------------------------------
        void onAssetBundleStartupState(AssetBundleStartupState ab_state)
        {

            if (ab_state.stateEnum == AssetBundleStartupState.StateEnum.Done)
            {
                this.startWwwStartup();
            }

            else if (ab_state.stateEnum == AssetBundleStartupState.StateEnum.Error)
            {

                var ynd_state = SimpleReduxManager.Instance.YesNoDialogStateWatcher.state();

                ynd_state.setState(
                    SimpleReduxManager.Instance.YesNoDialogStateWatcher,
                    YesNoDialogState.StateEnum.Show,
                    ab_state.error,
                    ab_state.url,
                    this.restartAssetBundleStartup,
                    this.cancelStartup
                    );
            }

        }

        /// <summary>
        /// Action on IEnumeratorStartupStateWatcher
        /// </summary>
        /// <param name="ie_state">current state</param>
        // -------------------------------------------------------------------------------------------------------
        void onIEnumeratorStartupState(IEnumeratorStartupState ie_state)
        {

            if (ie_state.stateEnum == IEnumeratorStartupState.StateEnum.DoneBefore)
            {
                this.startAssetBundleStartup();
            }

            else if (ie_state.stateEnum == IEnumeratorStartupState.StateEnum.DoneAfter)
            {

                SceneChangeState sc_state = SimpleReduxManager.Instance.SceneChangeStateWatcher.state();
                sc_state.setState(
                    SimpleReduxManager.Instance.SceneChangeStateWatcher,
                    SceneChangeState.StateEnum.NowLoadingOutro,
                    sc_state.nextSceneName
                    );

            }

            else if (ie_state.stateEnum == IEnumeratorStartupState.StateEnum.ErrorBefore)
            {

                var ynd_state = SimpleReduxManager.Instance.YesNoDialogStateWatcher.state();

                ynd_state.setState(
                    SimpleReduxManager.Instance.YesNoDialogStateWatcher,
                    YesNoDialogState.StateEnum.Show,
                    ie_state.error,
                    "",
                    this.restartIEnumeratorBeforeStartup,
                    this.cancelStartup
                    );
            }

            else if (ie_state.stateEnum == IEnumeratorStartupState.StateEnum.ErrorAfter)
            {

                var ynd_state = SimpleReduxManager.Instance.YesNoDialogStateWatcher.state();

                ynd_state.setState(
                    SimpleReduxManager.Instance.YesNoDialogStateWatcher,
                    YesNoDialogState.StateEnum.Show,
                    ie_state.error,
                    "",
                    this.restartIEnumeratorAfterStartup,
                    this.cancelStartup
                    );
            }

        }

        /// <summary>
        /// Start WWW Startup (No. 3 / 4)
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        void startWwwStartup()
        {
            var state = SimpleReduxManager.Instance.WwwStartupStateWatcher.state();
            state.setState(
                SimpleReduxManager.Instance.WwwStartupStateWatcher,
                WwwStartupState.StateEnum.Start,
                "",
                ""
                );
        }

        /// <summary>
        /// Restart WWW Startup
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        void restartWwwStartup()
        {
            var state = SimpleReduxManager.Instance.WwwStartupStateWatcher.state();
            state.setState(
                SimpleReduxManager.Instance.WwwStartupStateWatcher,
                WwwStartupState.StateEnum.Restart,
                "",
                ""
                );
        }

        /// <summary>
        /// Start AssetBundle Startup (No. 2 / 4)
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        void startAssetBundleStartup()
        {
            var state = SimpleReduxManager.Instance.AssetBundleStartupStateWatcher.state();
            state.setState(
                SimpleReduxManager.Instance.AssetBundleStartupStateWatcher,
                AssetBundleStartupState.StateEnum.Start,
                "",
                ""
                );
        }

        /// <summary>
        /// Restart AssetBundle Startup
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        void restartAssetBundleStartup()
        {
            var state = SimpleReduxManager.Instance.AssetBundleStartupStateWatcher.state();
            state.setState(
                SimpleReduxManager.Instance.AssetBundleStartupStateWatcher,
                AssetBundleStartupState.StateEnum.Restart,
                "",
                ""
                );
        }

        /// <summary>
        /// Start IEnumerator Startup (No. 1 / 4)
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        void startIEnumeratorBeforeStartup()
        {
            var state = SimpleReduxManager.Instance.IEnumeratorStartupStateWatcher.state();
            state.setState(
                SimpleReduxManager.Instance.IEnumeratorStartupStateWatcher,
                IEnumeratorStartupState.StateEnum.StartBefore,
                ""
                );
        }

        /// <summary>
        /// Retart IEnumerator Startup
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        void restartIEnumeratorBeforeStartup()
        {
            var state = SimpleReduxManager.Instance.IEnumeratorStartupStateWatcher.state();
            state.setState(
                SimpleReduxManager.Instance.IEnumeratorStartupStateWatcher,
                IEnumeratorStartupState.StateEnum.RestartBefore,
                ""
                );
        }

        /// <summary>
        /// Start IEnumerator Startup (No. 4 / 4)
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        void startIEnumeratorAfterStartup()
        {
            var state = SimpleReduxManager.Instance.IEnumeratorStartupStateWatcher.state();
            state.setState(
                SimpleReduxManager.Instance.IEnumeratorStartupStateWatcher,
                IEnumeratorStartupState.StateEnum.StartAfter,
                ""
                );
        }

        /// <summary>
        /// Retart IEnumerator Startup
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        void restartIEnumeratorAfterStartup()
        {
            var state = SimpleReduxManager.Instance.IEnumeratorStartupStateWatcher.state();
            state.setState(
                SimpleReduxManager.Instance.IEnumeratorStartupStateWatcher,
                IEnumeratorStartupState.StateEnum.RestartAfter,
                ""
                );
        }

        /// <summary>
        /// Cancel startup
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        void cancelStartup()
        {

            OkDialogState state = SimpleReduxManager.Instance.OkDialogStateWatcher.state();

            state.setState(
                SimpleReduxManager.Instance.OkDialogStateWatcher,
                OkDialogState.StateEnum.Show,
                "Back to Title",
                this.backToTitle
                );

        }

        /// <summary>
        /// Back to title scene
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        void backToTitle()
        {

            SceneChangeState state = SimpleReduxManager.Instance.SceneChangeStateWatcher.state();

            state.setState(
                SimpleReduxManager.Instance.SceneChangeStateWatcher,
                SceneChangeState.StateEnum.InnerChange,
                this.m_titleSceneName
                );

        }

        /// <summary>
        /// Clear all startup contents
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        void clearStartupContents()
        {

            // AssetBundleStartupStateWatcher
            {
                var state = SimpleReduxManager.Instance.AssetBundleStartupStateWatcher.state();
                state.setState(
                    SimpleReduxManager.Instance.AssetBundleStartupStateWatcher,
                    AssetBundleStartupState.StateEnum.Clear,
                    "",
                    ""
                    );
            }

            // WwwStartupStateWatcher
            {
                var state = SimpleReduxManager.Instance.WwwStartupStateWatcher.state();
                state.setState(
                    SimpleReduxManager.Instance.WwwStartupStateWatcher,
                    WwwStartupState.StateEnum.Clear,
                    "",
                    ""
                    );
            }

            // IEnumeratorStartupStateWatcher
            {
                var state = SimpleReduxManager.Instance.IEnumeratorStartupStateWatcher.state();
                state.setState(
                    SimpleReduxManager.Instance.IEnumeratorStartupStateWatcher,
                    IEnumeratorStartupState.StateEnum.Clear,
                    ""
                    );
            }

        }

        /// <summary>
        /// Load next scene IEnumerator
        /// </summary>
        /// <param name="scene_name">next scene name</param>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------------------------------------
        IEnumerator loadNextScene_IE(string sceneName)
        {

            // clear state
            {
                this.clearStartupContents();
            }

            // unload
            {

                AsyncOperation ao = Resources.UnloadUnusedAssets();

                while (!ao.isDone)
                {
                    yield return null;
                }

                System.GC.Collect();

            }

            // load
            {
                
                AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);

                while (!ao.isDone)
                {
                    yield return null;
                }

            }

            // startWwwStartup
            {
                this.startIEnumeratorBeforeStartup();
            }

        }

        /// <summary>
        /// Denominator of total progresses (be careful with zero)
        /// </summary>
        /// <param name="includeAssetBundle">AssetBundle startup progress</param>
        /// <param name="includeWww">WWW startup progress</param>
        /// <param name="includeIEnumerator">IEnumerator startup progress</param>
        /// <returns>progress</returns>
        // -------------------------------------------------------------------------------------------------------
        public int progressDenominator(bool includeAssetBundle, bool includeWww, bool includeIEnumerator)
        {

            int ret = 0;

            if(includeAssetBundle)
            {
                ret += AssetBundleStartupManager.Instance.progressDenominator();
            }

            if (includeWww)
            {
                ret += WwwStartupManager.Instance.progressDenominator();
            }

            if (includeAssetBundle)
            {
                ret += IEnumeratorStartupManager.Instance.progressDenominator();
            }

            return ret;

        }

        /// <summary>
        /// Numerator of total progresses
        /// </summary>
        /// <param name="includeAssetBundle">AssetBundle startup progress</param>
        /// <param name="includeWww">WWW startup progress</param>
        /// <param name="includeIEnumerator">IEnumerator startup progress</param>
        /// <returns>progress</returns>
        // -------------------------------------------------------------------------------------------------------
        public float progressNumerator(bool includeAssetBundle, bool includeWww, bool includeIEnumerator)
        {

            float ret = 0.0f;

            if (includeAssetBundle)
            {
                ret += AssetBundleStartupManager.Instance.progressNumerator();
            }

            if (includeWww)
            {
                ret += WwwStartupManager.Instance.progressNumerator();
            }

            if (includeAssetBundle)
            {
                ret += IEnumeratorStartupManager.Instance.progressNumerator();
            }

            return ret;

        }

        /// <summary>
        /// Select NowLoading object
        /// </summary>
        /// <returns>Selected NowLoading object</returns>
        // -------------------------------------------------------------------------------------------------------
        protected virtual NowLoadingBaseScript chooseNowLoading()
        {

            if (this.refNowloadings.Count <= 0)
            {
                return null;
            }

            return this.refNowloadings[0];

        }

        /// <summary>
        /// Dummy of OnValidate
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        protected virtual void DummyOnValidate()
        {

        }

#if UNITY_EDITOR

        /// <summary>
        /// OnValidate
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        void OnValidate()
        {

            this.DummyOnValidate();

            if (this.m_titleScene && !string.IsNullOrEmpty(this.m_titleScene.name))
            {
                this.m_titleSceneName = this.m_titleScene.name;
            }

        }

#endif

    }

}
