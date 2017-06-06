using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        protected UnityEngine.Object m_titleScene;

#endif

        /// <summary>
        /// Resume point from error
        /// </summary>
        protected enum ResumePoint
        {
            None,
            IEBefore,
            AssetBundle,
            Www,
            IEAfter,
        }

        /// <summary>
        /// Title scene name of m_titleScene
        /// </summary>
        [HideInInspector]
        [SerializeField]
        protected string m_titleSceneName = "";

        /// <summary>
        /// Current UI identifier for now loading
        /// </summary>
        [SerializeField]
        [Tooltip("Current UI identifier for now loading")]
        protected string m_currentNowLoadingUiIdentifier = "NowLoading";

        /// <summary>
        /// string for next scene
        /// </summary>
        protected string m_nowLoadingSceneName = "";

        /// <summary>
        /// Previous scene name
        /// </summary>
        protected string m_previousSceneName = "";

        /// <summary>
        /// UI identifiers after loading scene finished
        /// </summary>
        protected List<string> m_uiIdentifiersAfterLoadingScene = new List<string>();

        /// <summary>
        /// Progresses of loading scenes
        /// </summary>
        protected Dictionary<string, float> m_loadingScenesProgress = new Dictionary<string, float>();

        /// <summary>
        /// Resume point
        /// </summary>
        protected ResumePoint m_resumePoint = ResumePoint.None;

        /// <summary>
        /// Lock statements before loadings
        /// </summary>
        protected List<MonoBehaviour> m_lockBeforeLoadings = new List<MonoBehaviour>();

        /// <summary>
        /// Lock statements after loadings
        /// </summary>
        protected List<MonoBehaviour> m_lockAfterLoadings = new List<MonoBehaviour>();

        // -------------------------------------------------------------------------------------------------------

        /// <summary>
        /// string for next scene
        /// </summary>
        public string nowLoadingSceneName { get { return this.m_nowLoadingSceneName; } }

        /// <summary>
        /// Previous scene name
        /// </summary>
        public string previousSceneName { get { return this.m_previousSceneName; } }

        /// <summary>
        /// UI identifier for now loading
        /// </summary>
        public string currentNowLoadingUiIdentifier { get { return this.m_currentNowLoadingUiIdentifier; } set { this.m_currentNowLoadingUiIdentifier = value; } }

        // -------------------------------------------------------------------------------------------------------

        /// <summary>
        /// override
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        protected override void initOnAwake()
        {
        }

        /// <summary>
        /// Load next scene
        /// </summary>
        /// <param name="sceneName"></param>
        // -------------------------------------------------------------------------------------------------------
        public void loadNextScene(string sceneName)
        {
            this.loadNextScene(sceneName, true, new List<string>());
        }

        /// <summary>
        /// Load next scene
        /// </summary>
        /// <param name="sceneName"></param>
        // -------------------------------------------------------------------------------------------------------
        public void loadNextScene(string sceneName, bool updateHistory)
        {
            this.loadNextScene(sceneName, updateHistory, new List<string>());
        }

        /// <summary>
        /// Load next scene
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="afterLoadingSceneUiIdentifier"></param>
        // -------------------------------------------------------------------------------------------------------
        public void loadNextScene(string sceneName, bool updateHistory, string afterLoadingSceneUiIdentifier)
        {
            this.loadNextScene(sceneName, updateHistory, new List<string>() { afterLoadingSceneUiIdentifier });
        }

        /// <summary>
        /// Load next scene
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="uiIdentifiersAfterLoadingScene"></param>
        // -------------------------------------------------------------------------------------------------------
        public void loadNextScene(string sceneName, bool updateHistory, List<string> uiIdentifiersAfterLoadingScene)
        {

            var scState = SimpleReduxManager.Instance.SceneChangeStateWatcher.state();

            if (scState.stateEnum != SceneChangeState.StateEnum.ScenePlaying)
            {
#if UNITY_EDITOR
                Debug.LogWarning("(#if UNITY_EDITOR) Discard loadNextScene : " + sceneName);
#endif
                return;
            }

            //
            {

//                print(SceneManager.GetSceneByName(sceneName).buildIndex);

//                if (SceneManager.GetSceneByName(sceneName).buildIndex < 0)
//                {
//#if UNITY_EDITOR
//                    Debug.LogWarning("(#if UNITY_EDITOR) Not found scene : " + sceneName);
//#endif
//                    return;
//                }

            }

            // clear
            {
                this.clearContents();
            }
            
            // set
            {

                if (updateHistory)
                {
                    this.m_previousSceneName = this.m_nowLoadingSceneName;
                }

                this.m_nowLoadingSceneName = sceneName;

                //this.m_uiIdentifiersAfterLoadingScene.Clear();
                this.m_uiIdentifiersAfterLoadingScene.AddRange(uiIdentifiersAfterLoadingScene);

            }

            // SceneChangeStateWatcher
            {
                scState.setState(SimpleReduxManager.Instance.SceneChangeStateWatcher, SceneChangeState.StateEnum.NowLoadingIntro);
            }

            // show now loading ui
            {
                CommonUiManager.Instance.showUi(this.m_currentNowLoadingUiIdentifier, false, false, 0.0f, this.callbackForStartingNowLoading);
            }

        }

        /// <summary>
        /// Callback after showing ui done
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        protected void callbackForStartingNowLoading()
        {
            StartCoroutine(this.startNowLoadings(ResumePoint.None));
        }

        /// <summary>
        /// Clear contents
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        protected void clearContents()
        {

            this.m_loadingScenesProgress.Clear();

            this.m_uiIdentifiersAfterLoadingScene.Clear();

            this.m_resumePoint = ResumePoint.None;

            IEnumeratorStartupManager.Instance.clearContents();
            WwwStartupManager.Instance.clearContents();
            AssetBundleStartupManager.Instance.clearContents();

        }

        /// <summary>
        /// Retrt from resume pint
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        protected void retry()
        {
            StartCoroutine(this.startNowLoadings(this.m_resumePoint));
        }

        /// <summary>
        /// Back to title
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        public void backToTitleScene()
        {
            this.loadNextScene(this.m_titleSceneName, true, "");
        }

        /// <summary>
        /// Back to title with showing dialog
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        public void backToTitleSceneWithOkDialog()
        {
            DialogManager.Instance.showOkDialog(this.backToTitleMessages(), this.backToTitleScene);
        }

        /// <summary>
        /// Back to title messages
        /// </summary>
        /// <returns>message</returns>
        // -------------------------------------------------------------------------------------------------------
        public virtual System.Object backToTitleMessages()
        {

            DialogMessages messages = new DialogMessages();

            messages.category = DialogMessages.MessageCategory.Confirmation;
            messages.title = "Confirmation";
            messages.mainMessage = "Back to Title Scene";

            return messages;
        }

        // -------------------------------------------------------------------------------------------------------
        public void showBackToTitleOkDialog()
        {

            this.m_nowLoadingSceneName = this.m_titleSceneName;

            this.clearContents();

            DialogManager.Instance.showOkDialog(this.backToTitleMessages(), this.callbackForStartingNowLoading);

        }

        // -------------------------------------------------------------------------------------------------------
        protected IEnumerator startNowLoadings(ResumePoint resumePoint)
        {

            yield return null;

            // wait by lock
            {
                while(this.m_lockBeforeLoadings.Count > 0)
                {
                    yield return null;
                }
            }

            // unload
            if(resumePoint == ResumePoint.None)
            {

                AsyncOperation ao = Resources.UnloadUnusedAssets();

                while (!ao.isDone)
                {
                    yield return null;
                }

                System.GC.Collect();

            }


            // SceneChangeStateWatcher
            {
                var scState = SimpleReduxManager.Instance.SceneChangeStateWatcher.state();
                scState.setState(SimpleReduxManager.Instance.SceneChangeStateWatcher, SceneChangeState.StateEnum.NowLoadingMain);
            }

            // load
            if (resumePoint == ResumePoint.None)
            {

                if(!this.m_loadingScenesProgress.ContainsKey(this.m_nowLoadingSceneName))
                {
                    this.m_loadingScenesProgress.Add(this.m_nowLoadingSceneName, 0.0f);
                }

                AsyncOperation ao = SceneManager.LoadSceneAsync(this.m_nowLoadingSceneName);

                if(ao == null)
                {
                    Invoke("showBackToTitleOkDialog", 0.1f);
                    yield break;
                }

                while (!ao.isDone)
                {
                    this.m_loadingScenesProgress[this.m_nowLoadingSceneName] = ao.progress;
                    yield return null;
                }

                this.m_loadingScenesProgress[this.m_nowLoadingSceneName] = 1.0f;

            }

            // main
            {

                int loopCounter = 0;
                bool sceneAdditiveDetected = false;
                bool needToReload = false;

                System.Object messages = null;

                do
                {

                    sceneAdditiveDetected = false;

                    // IEnumeratorStartupManager Before
                    if (resumePoint <= ResumePoint.IEBefore)
                    {

                        yield return IEnumeratorStartupManager.Instance.startIEnumerator(IEnumeratorStartupManager.BeforeAfter.Before);

                        if (IEnumeratorStartupManager.Instance.hasError())
                        {
                            messages = IEnumeratorStartupManager.Instance.createErrorMessage();
                            this.m_resumePoint = ResumePoint.IEBefore;
                            break;
                        }

                    }

                    // AssetBundleStartupManager
                    if (resumePoint <= ResumePoint.AssetBundle)
                    {

                        List<AssetBundle> additiveScenes = new List<AssetBundle>();

                        yield return AssetBundleStartupManager.Instance.startAbStartup(
                            loopCounter++ > 0,
                            (detectedAdditiveScenes) =>
                            {
                                additiveScenes.AddRange(detectedAdditiveScenes);
                            },
                            (reload) =>
                            {
                                needToReload = reload;
                            });

                        if (AssetBundleStartupManager.Instance.hasError())
                        {
                            messages = AssetBundleStartupManager.Instance.createErrorMessage();
                            this.m_resumePoint = ResumePoint.AssetBundle;
                            break;
                        }

                        if (needToReload)
                        {
                            break;
                        }

                        if (additiveScenes.Count > 0)
                        {

                            sceneAdditiveDetected = true;

                            for (int i = additiveScenes.Count - 1; i >= 0; i--)
                            {

                                AssetBundle ab = additiveScenes[i];

                                foreach (string str in ab.GetAllScenePaths())
                                {

                                    if (!this.m_loadingScenesProgress.ContainsKey(str))
                                    {
                                        this.m_loadingScenesProgress.Add(str, 0.0f);
                                    }

                                    var aoForAdditive = SceneManager.LoadSceneAsync(Path.GetFileNameWithoutExtension(str), LoadSceneMode.Additive);

                                    while (!aoForAdditive.isDone)
                                    {
                                        this.m_loadingScenesProgress[str] = aoForAdditive.progress;
                                        yield return null;
                                    }

                                    this.m_loadingScenesProgress[str] = 1.0f;

                                }

                                ab.Unload(false);

                            }

                            continue;

                        }

                    }

                    // WwwStartupManager
                    if (resumePoint <= ResumePoint.Www)
                    {

                        yield return WwwStartupManager.Instance.startWwwStartup();

                        if (WwwStartupManager.Instance.hasError())
                        {
                            messages = WwwStartupManager.Instance.createErrorMessage();
                            this.m_resumePoint = ResumePoint.Www;
                            break;
                        }

                    }

                    // IEnumeratorStartupManager After
                    if (resumePoint <= ResumePoint.IEAfter)
                    {

                        yield return IEnumeratorStartupManager.Instance.startIEnumerator(IEnumeratorStartupManager.BeforeAfter.After);

                        if (IEnumeratorStartupManager.Instance.hasError())
                        {
                            messages = IEnumeratorStartupManager.Instance.createErrorMessage();
                            this.m_resumePoint = ResumePoint.IEAfter;
                            break;
                        }

                    }

                } while (sceneAdditiveDetected);

                if (messages != null)
                {
                    DialogManager.Instance.showYesNoDialog(messages, this.retry, this.showBackToTitleOkDialog);
                    yield break;
                }

                if (needToReload)
                {
                    Invoke("callbackWhenShowingNowLoadingDone", 0.1f);
                    yield break;
                }

                //
                {

                    if(IEnumeratorStartupManager.Instance.checkIfDetectedNewStartup(IEnumeratorStartupManager.BeforeAfter.Before))
                    {
                        this.m_resumePoint = ResumePoint.IEBefore;
                        Invoke("retry", 0.1f);
                        yield break;
                    }

                    else if (AssetBundleStartupManager.Instance.checkIfDetectedNewStartup())
                    {
                        this.m_resumePoint = ResumePoint.AssetBundle;
                        Invoke("retry", 0.1f);
                        yield break;
                    }

                    else if (WwwStartupManager.Instance.checkIfDetectedNewStartup())
                    {
                        this.m_resumePoint = ResumePoint.Www;
                        Invoke("retry", 0.1f);
                        yield break;
                    }

                    else if (IEnumeratorStartupManager.Instance.checkIfDetectedNewStartup(IEnumeratorStartupManager.BeforeAfter.After))
                    {
                        this.m_resumePoint = ResumePoint.IEAfter;
                        Invoke("retry", 0.1f);
                        yield break;
                    }

                }

            }

            // wait by lock
            {
                while (this.m_lockAfterLoadings.Count > 0)
                {
                    yield return null;
                }
            }

            // SceneChangeStateWatcher
            {
                var scState = SimpleReduxManager.Instance.SceneChangeStateWatcher.state();
                scState.setState(SimpleReduxManager.Instance.SceneChangeStateWatcher, SceneChangeState.StateEnum.NowLoadingOutro);
            }

            // show ui
            {
                CommonUiManager.Instance.showUi(this.m_uiIdentifiersAfterLoadingScene, true, false, 0, null, this.sendNowLoadingDoneSignal);
            }

            // clear
            {
                this.clearContents();
            }
            
        }

        /// <summary>
        /// Send now loading donw signal
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        protected void sendNowLoadingDoneSignal()
        {
            var scState = SimpleReduxManager.Instance.SceneChangeStateWatcher.state();
            scState.setState(SimpleReduxManager.Instance.SceneChangeStateWatcher, SceneChangeState.StateEnum.ScenePlaying);
        }

        /// <summary>
        /// All progress denominator
        /// </summary>
        /// <returns>progress denominator</returns>
        // -------------------------------------------------------------------------------------------------------
        public virtual int progressDenominator()
        {
            return this.progressDenominator(true, true, true, true);
        }

        /// <summary>
        /// Denominator of total progresses (be careful with zero)
        /// </summary>
        /// <param name="includeAssetBundle">AssetBundle startup progress</param>
        /// <param name="includeWww">WWW startup progress</param>
        /// <param name="includeIEnumerator">IEnumerator startup progress</param>
        /// <returns>progress</returns>
        // -------------------------------------------------------------------------------------------------------
        public virtual int progressDenominator(bool includeSceneProgress, bool includeAssetBundle, bool includeWww, bool includeIEnumerator)
        {

            int ret = 0;

            if (includeSceneProgress)
            {
                ret += this.m_loadingScenesProgress.Count;
            }

            if (includeAssetBundle)
            {
                ret += AssetBundleStartupManager.Instance.progressDenominator();
            }

            if (includeWww)
            {
                ret += WwwStartupManager.Instance.progressDenominator();
            }

            if (includeIEnumerator)
            {
                ret += IEnumeratorStartupManager.Instance.progressDenominator();
            }

            return ret;

        }

        /// <summary>
        /// All progress numerator
        /// </summary>
        /// <returns>progress numerator</returns>
        // -------------------------------------------------------------------------------------------------------
        public virtual float progressNumerator()
        {
            return this.progressNumerator(true, true, true, true);
        }

        /// <summary>
        /// Numerator of total progresses
        /// </summary>
        /// <param name="includeAssetBundle">AssetBundle startup progress</param>
        /// <param name="includeWww">WWW startup progress</param>
        /// <param name="includeIEnumerator">IEnumerator startup progress</param>
        /// <returns>progress</returns>
        // -------------------------------------------------------------------------------------------------------
        public virtual float progressNumerator(bool includeSceneProgress, bool includeAssetBundle, bool includeWww, bool includeIEnumerator)
        {

            float ret = 0.0f;

            if (includeSceneProgress)
            {

                foreach (var kv in this.m_loadingScenesProgress)
                {
                    ret += kv.Value;
                }
            }


            if (includeAssetBundle)
            {
                ret += AssetBundleStartupManager.Instance.progressNumerator();
            }

            if (includeWww)
            {
                ret += WwwStartupManager.Instance.progressNumerator();
            }

            if (includeIEnumerator)
            {
                ret += IEnumeratorStartupManager.Instance.progressNumerator();
            }

            return ret;

        }

        /// <summary>
        /// Add lock obj to before
        /// </summary>
        /// <param name="mb">obj</param>
        // -------------------------------------------------------------------------------------------------------
        public void addLockToBefore(MonoBehaviour mb)
        {

            if(mb && !this.m_lockBeforeLoadings.Contains(mb))
            {
                this.m_lockBeforeLoadings.Add(mb);
            }

        }

        /// <summary>
        /// Add lock obj to after
        /// </summary>
        /// <param name="mb">obj</param>
        // -------------------------------------------------------------------------------------------------------
        public void addLockToAfter(MonoBehaviour mb)
        {

            if (mb && !this.m_lockAfterLoadings.Contains(mb))
            {
                this.m_lockAfterLoadings.Add(mb);
            }

        }

        /// <summary>
        /// Remove lock obj from before
        /// </summary>
        /// <param name="mb">obj</param>
        // -------------------------------------------------------------------------------------------------------
        public void removeLockFromBefore(MonoBehaviour mb)
        {

#if UNITY_EDITOR

            if(!this.m_lockBeforeLoadings.Remove(mb))
            {
                Debug.LogWarning("(#if UNITY_EDITOR) Failed removeLockFromBefore");
            }

#else

            this.m_lockBeforeLoadings.Remove(mb);

#endif


        }

        /// <summary>
        /// Remove lock obj from after
        /// </summary>
        /// <param name="mb">obj</param>
        // -------------------------------------------------------------------------------------------------------
        public void removeLockFromAfter(MonoBehaviour mb)
        {

#if UNITY_EDITOR

            if (!this.m_lockAfterLoadings.Remove(mb))
            {
                Debug.LogWarning("(#if UNITY_EDITOR) Failed removeLockFromAfter");
            }

#else

            this.m_lockAfterLoadings.Remove(mb);

#endif

        }

        /// <summary>
        /// OnValidate
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        protected virtual void OnValidate()
        {

#if UNITY_EDITOR

            if (this.m_titleScene && !string.IsNullOrEmpty(this.m_titleScene.name))
            {
                this.m_titleSceneName = this.m_titleScene.name;
            }

            else
            {
                this.m_titleSceneName = "";
            }
#endif

        }

    }

}
