using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace SSC
{

    /// <summary>
    /// Class for AssetBundle startup
    /// </summary>
    public class AssetBundleStartupManager : SingletonMonoBehaviour<AssetBundleStartupManager>
    {
        /// <summary>
        /// Class for loading AssetBundle
        /// </summary>
        protected class AbStruct
        {

            /// <summary>
            /// AssetBundle name
            /// </summary>
            public string nameDotVariant = "";

            /// <summary>
            /// Success function 
            /// </summary>
            public Action<AssetBundle> successFunc = null;

            /// <summary>
            /// Success detail function 
            /// </summary>
            public Action<AssetBundle, System.Object> successDetailFunc = null;

            /// <summary>
            /// Success detail function for asunc
            /// </summary>
            public Action<AssetBundle, System.Object, Action> successDetailFuncForAsync = null;

            /// <summary>
            /// Failed function
            /// </summary>
            public Action<WWW> failedFunc = null;

            /// <summary>
            /// Progress function
            /// </summary>
            public Action<WWW> progressFunc = null;

            /// <summary>
            /// Done with success flag
            /// </summary>
            public bool doneSuccess = false;

            /// <summary>
            /// Identifier for detail
            /// </summary>
            public System.Object identifierForDetail = null;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="_name">name</param>
            /// <param name="_variant">variant</param>
            /// <param name="_successFunc">successFunc</param>
            /// <param name="_failedFunc">failedFunc</param>
            /// <param name="_progressFunc">progressFunc</param>
            public AbStruct(
                string _name,
                string _variant,
                Action<AssetBundle> _successFunc,
                Action<WWW> _failedFunc,
                Action<WWW> _progressFunc
                )
            {

                this.nameDotVariant = string.IsNullOrEmpty(_variant) ? _name : _name + "." + _variant;
                this.successFunc = _successFunc;
                this.failedFunc = _failedFunc;
                this.progressFunc = _progressFunc;

            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="_name">name</param>
            /// <param name="_variant">variant</param>
            /// <param name="_successDetailFunc">successDetailFunc</param>
            /// <param name="_failedFunc">failedFunc</param>
            /// <param name="_progressFunc">progressFunc</param>
            /// <param name="_identifierForDetail">identifierForDetail</param>
            public AbStruct(
                string _name,
                string _variant,
                Action<AssetBundle, System.Object> _successDetailFunc,
                Action<WWW> _failedFunc,
                Action<WWW> _progressFunc,
                System.Object _identifierForDetail
                )
            {

                this.nameDotVariant = string.IsNullOrEmpty(_variant) ? _name : _name + "." + _variant;
                this.successDetailFunc = _successDetailFunc;
                this.failedFunc = _failedFunc;
                this.progressFunc = _progressFunc;
                this.identifierForDetail = _identifierForDetail;

            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="_name">name</param>
            /// <param name="_variant">variant</param>
            /// <param name="_successDetailFuncForAsync">successDetailFuncForAsync</param>
            /// <param name="_failedFunc">failedFunc</param>
            /// <param name="_progressFunc">progressFunc</param>
            /// <param name="_identifierForDetail">identifierForDetail</param>
            public AbStruct(
                string _name,
                string _variant,
                Action<AssetBundle, System.Object, Action> _successDetailFuncForAsync,
                Action<WWW> _failedFunc,
                Action<WWW> _progressFunc,
                System.Object _identifierForDetail
                )
            {

                this.nameDotVariant = string.IsNullOrEmpty(_variant) ? _name : _name + "." + _variant;
                this.successDetailFuncForAsync = _successDetailFuncForAsync;
                this.failedFunc = _failedFunc;
                this.progressFunc = _progressFunc;
                this.identifierForDetail = _identifierForDetail;

            }

        }

        /// <summary>
        /// Error message
        /// </summary>
        protected string m_error = "";

        /// <summary>
        /// Error url
        /// </summary>
        protected string m_errorUrl = "";

        /// <summary>
        /// AssetBundles dependencies
        /// </summary>
        protected Dictionary<string, AssetBundle> m_dependencies = new Dictionary<string, AssetBundle>();

        /// <summary>
        /// AbStruct list
        /// </summary>
        protected List<AbStruct> m_absList = new List<AbStruct>();

        /// <summary>
        /// The number of parallel loading coroutines
        /// </summary>
        [SerializeField]
        [Tooltip("The number of parallel loading coroutines")]
        protected int m_numberOfCo = 4;

        /// <summary>
        /// Ignore error except manifest
        /// </summary>
        [SerializeField]
        [Tooltip("Ignore error except manifest")]
        protected bool m_ignoreErrorExceptManifest = false;

        /// <summary>
        /// After loading done, redownload manifest and compare new and old.
        /// if deference detected, reload unity scene
        /// </summary>
        [SerializeField]
        [Tooltip("After loading done, reload manifest and compare new and old one, if deference detected, reload unity scene silently")]
        protected bool m_checkManifestAfterLoading = false;

        /// <summary>
        /// Use decryption
        /// </summary>
        [SerializeField]
        [Tooltip("Use decryption. If you changed this, don't forget to clear cache.")]
        protected bool m_useDecryption = false;

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
        /// Url for manifest file
        /// </summary>
        protected string m_assetBundleManifestFileUrl = "";

        /// <summary>
        /// Url for base url
        /// </summary>
        protected string m_assetBundleManifestFolderUrl = "";

        /// <summary>
        /// AssetBundle that has manifest
        /// </summary>
        protected AssetBundle m_manifestAssetBundle = null;

        /// <summary>
        /// Current AssetBundleManifest
        /// </summary>
        protected AssetBundleManifest m_manifest = null;

        /// <summary>
        /// Error message when loading manifest from AssetBundle failed
        /// </summary>
        protected readonly string FailedToGetAssetBundleManifest = "Failed to get AssetBundleManifest";

        /// <summary>
        /// Error message when decryption failed
        /// </summary>
        protected readonly string FailedToDecryptAssetBundle = "Failed to load AssetBundle";

        /// <summary>
        /// Connection Timeout message
        /// </summary>
        protected readonly string ConnectionTimeout = "Connection Timeout";

        /// <summary>
        /// Current co progress
        /// </summary>
        protected List<float> m_currentCoProgresses = new List<float>();

        /// <summary>
        /// Need to reload scene by new manifest
        /// </summary>
        protected bool m_needReloadScene = false;

        /// <summary>
        /// AbStruct list for runtime
        /// </summary>
        protected List<AbStruct> m_absListRuntime = new List<AbStruct>();

        /// <summary>
        /// IEnumerator for runtime loading
        /// </summary>
        protected IEnumerator m_runtimeLoading = null;

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

#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)

            if(!SystemInfo.graphicsDeviceType.ToString().ToLower().Contains("opengl"))
            {
                Debug.LogWarning("(#if UNITY_EDITOR) Use OpenGLES, or you will see pink shader, perhaps.");
            }

#endif

        }

        /// <summary>
        /// Set url for manifest file and base url
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        protected virtual void setManifestFileAndFolderUrl()
        {

#if UNITY_EDITOR

            Debug.LogWarning("(#if UNITY_EDITOR) You must override this function as you want.");

#endif
            // sample
            {

                string manifestName = "";

#if UNITY_ANDROID
                
                manifestName = (this.m_useDecryption) ? "android.encrypted.unity3d" : "android.unity3d";

#elif UNITY_IOS
                
                manifestName = (this.m_useDecryption) ? "ios.encrypted.unity3d" : "ios.unity3d";
                
#else

                manifestName = (this.m_useDecryption) ? "windows.encrypted.unity3d" : "windows.unity3d";

#endif

                // endsWith slash
                this.m_assetBundleManifestFolderUrl =
                    "http://localhost:50002/" +
                    manifestName +
                    ((this.m_useDecryption) ? "/encrypted/" : "/")
                    ;

                this.m_assetBundleManifestFileUrl = this.m_assetBundleManifestFolderUrl + manifestName;

            }

        }

        /// <summary>
        /// Add startup data
        /// </summary>
        /// <param name="assetBundleName">assetBundleName</param>
        /// <param name="variant">variant</param>
        /// <param name="success_func">success function</param>
        /// <param name="failed_func">failed function</param>
        /// <param name="progress_func">progress function</param>
        // -------------------------------------------------------------------------------------------------------
        public void addSceneStartupAssetBundle(string assetBundleName, string variant, Action<AssetBundle> success_func, Action<WWW> failed_func, Action<WWW> progress_func)
        {
            this.m_absList.Add(new AbStruct(assetBundleName, variant, success_func, failed_func, progress_func));
            this.m_detectedNewStartupObject = true;
        }

        /// <summary>
        /// Add startup data
        /// </summary>
        /// <param name="assetBundleName">assetBundleName</param>
        /// <param name="variant">variant</param>
        /// <param name="success_func">success function</param>
        /// <param name="failed_func">failed function</param>
        /// <param name="progress_func">progress function</param>
        // -------------------------------------------------------------------------------------------------------
        public void addSceneStartupAssetBundle(
            string assetBundleName,
            string variant,
            Action<AssetBundle, System.Object> success_func,
            Action<WWW> failed_func,
            Action<WWW> progress_func,
            System.Object identifierForDetail
            )
        {
            this.m_absList.Add(new AbStruct(assetBundleName, variant, success_func, failed_func, progress_func, identifierForDetail));
            this.m_detectedNewStartupObject = true;
        }

        /// <summary>
        /// Add startup data
        /// </summary>
        /// <param name="assetBundleName">assetBundleName</param>
        /// <param name="variant">variant</param>
        /// <param name="success_func">success function</param>
        /// <param name="failed_func">failed function</param>
        /// <param name="progress_func">progress function</param>
        // -------------------------------------------------------------------------------------------------------
        public void addSceneStartupAssetBundle(
            string assetBundleName,
            string variant,
            Action<AssetBundle, System.Object, Action> success_func,
            Action<WWW> failed_func,
            Action<WWW> progress_func,
            System.Object identifierForDetail
            )
        {
            this.m_absList.Add(new AbStruct(assetBundleName, variant, success_func, failed_func, progress_func, identifierForDetail));
            this.m_detectedNewStartupObject = true;
        }

        /// <summary>
        /// Denominator of progress
        /// </summary>
        /// <returns>value</returns>
        // -------------------------------------------------------------------------------------------------------
        public int progressDenominator()
        {
            return this.m_absList.Count + this.m_dependencies.Count;
        }

        /// <summary>
        /// Numerator of progress
        /// </summary>
        /// <returns>value</returns>
        // -------------------------------------------------------------------------------------------------------
        public float progressNumerator()
        {

            float ret = 0.0f;

            foreach (var abs in this.m_absList)
            {
                if (abs.doneSuccess)
                {
                    ret += 1.0f;
                }
            }

            foreach (var depend in this.m_dependencies)
            {
                if (depend.Value)
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
            messages.title = "AssetBundle Error";
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

            this.m_absList.Clear();

            StopAllCoroutines();

            // m_dependencies
            {

                foreach (var kv in this.m_dependencies)
                {

                    if (kv.Value)
                    {
                        kv.Value.Unload(false);
                    }

                }

                this.m_dependencies.Clear();

            }

            // m_currentCoProgresses
            {

                for (int i = this.m_currentCoProgresses.Count - 1; i >= 0; i--)
                {
                    this.m_currentCoProgresses[i] = 0.0f;
                }

            }

            // runtime
            {
                this.m_absListRuntime.Clear();
            }

            // m_detectedNewStartupObject
            {
                this.m_detectedNewStartupObject = false;
            }

        }

        /// <summary>
        /// Get AssetBundle from already loaded dependencies
        /// </summary>
        /// <param name="nameDotVariant">AssetBundle name</param>
        /// <returns>if found, return AssetBundle, if not, return null</returns>
        // -------------------------------------------------------------------------------------------------------
        protected AssetBundle assetBundleFromDependencies(string nameDotVariant)
        {
            if (this.m_dependencies.ContainsKey(nameDotVariant))
            {
                return this.m_dependencies[nameDotVariant];
            }
            return null;
        }

        /// <summary>
        /// If manifest is available
        /// </summary>
        /// <returns>manifest is available</returns>
        // -------------------------------------------------------------------------------------------------------
        protected bool hasManifest()
        {
            return this.m_manifest;
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

        /// <summary>
        /// Create AssetBundle url
        /// </summary>
        /// <param name="nameDotVariant">AssetBundle name</param>
        /// <returns>url</returns>
        // -------------------------------------------------------------------------------------------------------
        protected virtual string createAssetBundleUrl(string nameDotVariant)
        {
            return this.m_assetBundleManifestFolderUrl + nameDotVariant;
        }

        /// <summary>
        /// Compare current manifest with new manifest
        /// </summary>
        /// <param name="currentManifest">current manifest hashes</param>
        /// <param name="newManifest">new manifest hashes</param>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------------------------------------
        protected IEnumerator hasDifferenceOfOldAndNewManifest(
            Dictionary<string, Hash128> currentManifest,
            Dictionary<string, Hash128> newManifest,
            Action<bool> callback
            )
        {

            if (currentManifest.Count <= 0 || newManifest.Count <= 0)
            {
                callback(false);
                yield break;
            }

            // check length
            {

                if (currentManifest.Count != currentManifest.Count)
                {
                    callback(false);
                    yield break;
                }

            }

            // check hash
            {

                int counter = 0;

                foreach (var kv in currentManifest)
                {

                    if (!newManifest.ContainsKey(kv.Key) || currentManifest[kv.Key] != newManifest[kv.Key])
                    {
                        callback(true);
                        yield break;
                    }

                    if (counter++ % 50 == 0)
                    {
                        yield return null;
                    }

                }

            }

            callback(false);

        }

        /// <summary>
        /// Load all ABs internal function
        /// </summary>
        /// <param name="keyNameDotVariant">key.variant</param>
        /// <param name="absCombined">cobined ABs</param>
        /// <param name="coNumber">coroutine number</param>
        /// <param name="sceneAdditiveCallback">additive scene detected callback</param>
        /// <param name="doneCallback">callback when done</param>
        /// <returns></returns>
        // -------------------------------------------------------------------------------------------------------
        protected IEnumerator loadAllMainAssetBundleInternal(
            string keyNameDotVariant,
            List<AbStruct> absCombined,
            int coNumber,
            Action<AssetBundle> sceneAdditiveCallback,
            Action doneCallback
            )
        {

            yield return null;

            // Caching
            {
                while (!Caching.ready)
                {
                    yield return null;
                }
            }

            // from_dependencies
            {

                AssetBundle from_dependencies = this.assetBundleFromDependencies(keyNameDotVariant);

                if (from_dependencies)
                {

                    foreach (var abs in absCombined)
                    {

                        if (!abs.doneSuccess)
                        {

                            if (abs.successFunc != null)
                            {
                                abs.successFunc(from_dependencies);
                            }

                            else if (abs.successDetailFunc != null)
                            {
                                abs.successDetailFunc(from_dependencies, abs.identifierForDetail);
                            }

                            else if (abs.successDetailFuncForAsync != null)
                            {

                                bool finished = false; 

                                abs.successDetailFuncForAsync(from_dependencies, abs.identifierForDetail, () =>
                                {
                                    finished = true;
                                });

                                while(!finished)
                                {
                                    yield return null;
                                }

                            }

                        }

                        this.m_currentCoProgresses[coNumber] = 0.0f;
                        abs.doneSuccess = true;

                    }

                    doneCallback();
                    yield break;

                }

            }

            float noProgressTimer = 0.0f;
            float previousProgress = 0.0f;

            using (WWW abwww = WWW.LoadFromCacheOrDownload(this.createAssetBundleUrl(keyNameDotVariant), this.m_manifest.GetAssetBundleHash(keyNameDotVariant)))
            {

#if !UNITY_WEBGL
                abwww.threadPriority = this.m_threadPriority;
#endif

                // wait www done
                {

                    while (!abwww.isDone)
                    {

                        foreach (var abs in absCombined)
                        {
                            if (abs.progressFunc != null)
                            {
                                abs.progressFunc(abwww);
                            }
                        }

                        // m_currentCoProgresses
                        {
                            this.m_currentCoProgresses[coNumber] = abwww.progress * 0.999f;
                        }

                        // timeout
                        {

                            if (this.m_noProgressTimeOutSeconds > 0.0f)
                            {

                                if (previousProgress == abwww.progress)
                                {
                                    noProgressTimer += Time.deltaTime;
                                }

                                else
                                {
                                    noProgressTimer = 0.0f;
                                }

                                previousProgress = abwww.progress;

                                if (noProgressTimer >= this.m_noProgressTimeOutSeconds)
                                {
                                    this.setError(abwww.url, this.ConnectionTimeout);
                                    doneCallback();
                                    yield break;
                                }

                            }

                        }

                        yield return null;

                    }

                    foreach (var abs in absCombined)
                    {
                        if (abs.progressFunc != null)
                        {
                            abs.progressFunc(abwww);
                        }
                    }

                    yield return null;

                } // wait

                // success or fail
                {

                    // success
                    if (string.IsNullOrEmpty(abwww.error))
                    {

                        AssetBundle decrypted = null;

                        yield return this.decryptAssetBundle(abwww.assetBundle, (ab) =>
                        {
                            decrypted = ab;
                        }
                        );

                        if (decrypted)
                        {

                            if (decrypted.isStreamedSceneAssetBundle)
                            {

                                sceneAdditiveCallback(decrypted);

#if UNITY_EDITOR
                                if (absCombined.Count >= 2)
                                {
                                    Debug.LogWarning("(#if UNITY_EDITOR) Duplicated AssetBundle scene is not supported : " + keyNameDotVariant);
                                }
#endif

                            }

                            foreach (var abs in absCombined)
                            {

                                if (!abs.doneSuccess)
                                {

                                    if (abs.successFunc != null)
                                    {
                                        abs.successFunc(decrypted);
                                    }

                                    else if (abs.successDetailFunc != null)
                                    {
                                        abs.successDetailFunc(decrypted, abs.identifierForDetail);
                                    }

                                    else if (abs.successDetailFuncForAsync != null)
                                    {

                                        bool finished = false;

                                        abs.successDetailFuncForAsync(decrypted, abs.identifierForDetail, () =>
                                        {
                                            finished = true;
                                        });

                                        while (!finished)
                                        {
                                            yield return null;
                                        }

                                    }

                                }

                            }

                            if (!decrypted.isStreamedSceneAssetBundle)
                            {
                                decrypted.Unload(false);
                            }

                        }

                        else
                        {
                            this.setError(abwww.url, this.FailedToDecryptAssetBundle);
                            doneCallback();
                            yield break;
                        }

                    }

                    // fail
                    else
                    {

                        foreach (var abs in absCombined)
                        {
                            if (abs.failedFunc != null)
                            {
                                abs.failedFunc(abwww);
                            }
                        }

                        if (!this.m_ignoreErrorExceptManifest)
                        {
                            this.setError(abwww.url, abwww.error);
                            doneCallback();
                            yield break;
                        }

                    }

                } // success or fail

                // reset progress
                {
                    this.m_currentCoProgresses[coNumber] = 0.0f;
                }

                foreach (var abs in absCombined)
                {
                    abs.doneSuccess = true;
                }

            } // using

            doneCallback();

        }

        /// <summary>
        /// Load all ABs
        /// </summary>
        /// <param name="additiveScenes">Detected additive scene ABs</param>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------------------------------------
        protected IEnumerator loadAllMainAssetBundle(Action<List<AssetBundle>> additiveScenes)
        {

            yield return null;

            // clear
            Dictionary<string, List<AbStruct>> combined = this.createCombinedAbList();

            int workingCoCounter = 0;

            List<string> combinedKeys = new List<string>(combined.Keys);

            List<AssetBundle> detectedAdditiveScenes = new List<AssetBundle>();

            bool skip = true;

            int listCount = combinedKeys.Count;
            int listIndex = 0;

            while (listIndex < listCount)
            {

                if (this.hasError())
                {
                    break;
                }

                // -------------

                if (workingCoCounter < this.m_numberOfCo)
                {

                    string key = combinedKeys[listIndex++];

                    foreach (var val in combined[key])
                    {
                        if (!val.doneSuccess)
                        {
                            skip = false;
                            break;
                        }
                    }

                    if (!skip)
                    {

                        StartCoroutine(this.loadAllMainAssetBundleInternal(
                            key,
                            combined[key],
                            workingCoCounter++,
                            (additive) =>
                            {
                                detectedAdditiveScenes.Add(additive);
                            },
                            () =>
                            {
                                workingCoCounter--;
                            }
                            ));

                    }

                }

                yield return null;

            }

            while (workingCoCounter > 0)
            {
                yield return null;
            }

            additiveScenes(detectedAdditiveScenes);

        }

        /// <summary>
        /// Load dependency
        /// </summary>
        /// <param name="key">assetbundle name</param>
        /// <param name="coNumber">coroutine number</param>
        /// <param name="doneCallback">callback when done</param>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------------------------------------
        protected IEnumerator loadDependency(Dictionary<string, AssetBundle> dependenciesTarget, string key, int coNumber, Action doneCallback)
        {

            yield return null;

            if (!this.m_manifest)
            {
                doneCallback();
                yield break;
            }

            // Caching
            {
                while (!Caching.ready)
                {
                    yield return null;
                }
            }

            float noProgressTimer = 0.0f;
            float previousProgress = 0.0f;

            using (WWW dependwww = WWW.LoadFromCacheOrDownload(this.createAssetBundleUrl(key), this.m_manifest.GetAssetBundleHash(key)))
            {

#if !UNITY_WEBGL
                dependwww.threadPriority = this.m_threadPriority;
#endif

                while (!dependwww.isDone)
                {

                    // m_currentCoProgresses
                    {
                        this.m_currentCoProgresses[coNumber] = dependwww.progress * 0.999f;
                    }

                    // timeout
                    {

                        if (this.m_noProgressTimeOutSeconds > 0.0f)
                        {

                            if (previousProgress == dependwww.progress)
                            {
                                noProgressTimer += Time.deltaTime;
                            }

                            else
                            {
                                noProgressTimer = 0.0f;
                            }

                            previousProgress = dependwww.progress;

                            if (noProgressTimer >= this.m_noProgressTimeOutSeconds)
                            {
                                this.setError(dependwww.url, this.ConnectionTimeout);
                                doneCallback();
                                yield break;
                            }

                        }

                    }

                    yield return null;
                }

                if (string.IsNullOrEmpty(dependwww.error))
                {

                    yield return this.decryptAssetBundle(dependwww.assetBundle, (ab) =>
                    {
                        this.m_currentCoProgresses[coNumber] = 0.0f;
                        dependenciesTarget[key] = ab;
                    }
                    );

                }

                else
                {
                    this.setError(dependwww.url, dependwww.error);
                }

            }

            doneCallback();

        }

        /// <summary>
        /// Load all dependencies
        /// </summary>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------------------------------------
        protected IEnumerator loadAllDependencies(Dictionary<string, AssetBundle> dependenciesTarget)
        {

            yield return null;

            int workingCoCounter = 0;

            List<string> dependKeys = new List<string>(dependenciesTarget.Keys);

            int listCount = dependKeys.Count;
            int listIndex = 0;

            while (listIndex < listCount)
            {

                if (this.hasError())
                {
                    break;
                }

                // -------------

                if (workingCoCounter < this.m_numberOfCo)
                {

                    string key = dependKeys[listIndex++];

                    if (!dependenciesTarget[key])
                    {
                        StartCoroutine(this.loadDependency(dependenciesTarget, key, workingCoCounter++, () =>
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

        }

        // -------------------------------------------------------------------------------------------------------
        public IEnumerator startAbStartup(bool restartForAdditive, Action<List<AssetBundle>> additiveScenes, Action<bool> reloadScene)
        {

            yield return null;

            // m_detectedNewStartupObject
            {
                this.m_detectedNewStartupObject = false;
            }

            // clear error
            {
                this.setError("", "");
            }

            if (this.m_absList.Count <= 0)
            {
                this.setManifestFileAndFolderUrl();
                yield break;
            }

            // ------------------

            bool detectedAdditiveScene = false;

            //
            {

                if (!restartForAdditive)
                {
                    this.setManifestFileAndFolderUrl();
                }

                if (!this.hasManifest() || !restartForAdditive)
                {
                    yield return this.updateManifest(null);
                }

                if (this.hasError())
                {
                    yield break;
                }

                // solveStartupDependenciesKeys
                {

                    yield return this.solveStartupDependenciesKeys();

                    if (this.hasError())
                    {
                        yield break;
                    }

                }

                // loadAllDependencies
                {

                    yield return this.loadAllDependencies(this.m_dependencies);

                    if (this.hasError())
                    {
                        yield break;
                    }

                }

                // loadAllMainAssetBundle
                {

                    yield return this.loadAllMainAssetBundle((ret) =>
                    {
                        additiveScenes(ret);
                        detectedAdditiveScene = ret.Count > 0;
                    });

                    if (this.hasError())
                    {
                        yield break;
                    }

                }

            }

            if (this.m_checkManifestAfterLoading && !detectedAdditiveScene)
            {

                yield return this.updateManifest((ret) =>
                {
                    reloadScene(ret);
                });

            }

        }

        /// <summary>
        /// Detect duplicated AssetBundles to avoid duplicated loading
        /// </summary>
        /// <returns>combined list</returns>
        // -------------------------------------------------------------------------------------------------------
        protected Dictionary<string, List<AbStruct>> createCombinedAbList()
        {

            Dictionary<string, List<AbStruct>> ret = new Dictionary<string, List<AbStruct>>();

            foreach (AbStruct abs in this.m_absList)
            {

                if (!ret.ContainsKey(abs.nameDotVariant))
                {
                    ret[abs.nameDotVariant] = new List<AbStruct>();
                }

                ret[abs.nameDotVariant].Add(abs);

            }

#if UNITY_EDITOR

            foreach (var kv in ret)
            {

                if (kv.Value.Count >= 2)
                {
                    Debug.LogWarning("(#if UNITY_EDITOR) Duplicated AssetBundle loading is not recommended (but, no problem) : " + kv.Key);
                }

            }

#endif

            return ret;

        }

        /// <summary>
        /// Decrypt binary data
        /// </summary>
        /// <param name="textAsset">binary data</param>
        /// <returns>decrypted binary data</returns>
        // -------------------------------------------------------------------------------------------------------
        protected virtual byte[] decryptBinaryData(TextAsset textAsset)
        {

            if (!textAsset)
            {
                return new byte[] { };
            }

            return Funcs.DecryptBinaryData(textAsset.bytes, "PassworDPassworD");

        }

        /// <summary>
        /// Decrypt AssetBundle
        /// </summary>
        /// <param name="assetBundle">AssetBundle</param>
        /// <param name="ret">return function</param>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------------------------------------
        protected IEnumerator decryptAssetBundle(AssetBundle assetBundle, Action<AssetBundle> ret)
        {

            yield return null;

            if (!assetBundle)
            {
                yield break;
            }

            // ------------------

            if (this.m_useDecryption)
            {

                string[] names = assetBundle.GetAllAssetNames();
                string name = (names.Length > 0) ? assetBundle.GetAllAssetNames().First() : "";

                if (string.IsNullOrEmpty(name))
                {
                    ret(null);
                    yield break;
                }

                byte[] decrypted = this.decryptBinaryData(assetBundle.LoadAsset<TextAsset>(name));

                yield return null;

                if (decrypted != null && decrypted.Length > 0)
                {

                    AssetBundleCreateRequest abcr = AssetBundle.LoadFromMemoryAsync(decrypted);

                    if (abcr != null)
                    {
                        yield return abcr;
                        ret(abcr.assetBundle);
                        assetBundle.Unload(true);
                    }

                }

                else
                {
                    ret(null);
                    assetBundle.Unload(true);
                }

            }

            else
            {
                ret(assetBundle);
            }

        }

        /// <summary>
        /// Download manifest
        /// </summary>
        /// <param name="check">if true, compare old and new</param>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------------------------------------
        protected IEnumerator updateManifest(Action<bool> callback)
        {

            yield return null;

            // ---------------------

            Dictionary<string, Hash128> old_key_hash = new Dictionary<string, Hash128>();
            Dictionary<string, Hash128> new_key_hash = new Dictionary<string, Hash128>();

            // ---------------------

            // unload old
            {

                if (this.m_manifestAssetBundle)
                {

                    if (this.m_manifest)
                    {
                        foreach (string str in this.m_manifest.GetAllAssetBundles())
                        {
                            if (!old_key_hash.ContainsKey(str))
                            {
                                old_key_hash.Add(str, this.m_manifest.GetAssetBundleHash(str));
                            }
                        }
                    }

                    this.m_manifestAssetBundle.Unload(true);
                    yield return null;
                }

            }

            float noProgressTimer = 0.0f;
            float previousProgress = 0.0f;

            using (WWW www = new WWW(this.m_assetBundleManifestFileUrl))
            {

#if !UNITY_WEBGL
                www.threadPriority = this.m_threadPriority;
#endif

                while (!www.isDone)
                {

                    // timeout
                    {

                        if (this.m_noProgressTimeOutSeconds > 0.0f)
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

                            if (noProgressTimer >= this.m_noProgressTimeOutSeconds)
                            {
                                this.setError(www.url, this.ConnectionTimeout);
                                yield break;
                            }

                        }

                    }

                    yield return null;

                }

                if (string.IsNullOrEmpty(www.error))
                {

                    if (www.assetBundle)
                    {

                        yield return this.decryptAssetBundle(www.assetBundle, (ab) =>
                        {
                            this.m_manifestAssetBundle = ab;
                        }
                        );

                        if (this.m_manifestAssetBundle)
                        {

                            AssetBundleRequest request = this.m_manifestAssetBundle.LoadAssetAsync("AssetBundleManifest", typeof(AssetBundleManifest));

                            if (request != null)
                            {
                                yield return request;
                                this.m_manifest = request.asset as AssetBundleManifest;
                            }

                            if (this.m_manifest)
                            {
                                foreach (string str in this.m_manifest.GetAllAssetBundles())
                                {
                                    if (!new_key_hash.ContainsKey(str))
                                    {
                                        new_key_hash.Add(str, this.m_manifest.GetAssetBundleHash(str));
                                    }
                                }
                            }

                        }

                        else
                        {
                            this.setError(this.m_assetBundleManifestFileUrl, this.FailedToDecryptAssetBundle);
                            yield break;
                        }

                    }

                    if (!this.m_manifest)
                    {
                        this.setError(this.m_assetBundleManifestFileUrl, this.FailedToGetAssetBundleManifest);
                        yield break;
                    }

                }

                else
                {
                    this.setError(www.url, www.error);
                }

            } // using

            // check
            if (callback != null)
            {

                yield return this.hasDifferenceOfOldAndNewManifest(old_key_hash, new_key_hash, (ret) =>
                {
                    callback(ret);
                });

            }

        }

        /// <summary>
        /// Get all dependencies names
        /// </summary>
        /// <param name="ret">return list</param>
        /// <param name="nameDotVariant">origin</param>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------------------------------------
        protected IEnumerator allDependenciesNames(List<string> ret, string nameDotVariant, int counterForSleep)
        {

            if (counterForSleep++ % 100 == 0)
            {
                yield return null;
            }

            if (!this.m_manifest || this.hasError())
            {
                yield break;
            }

            foreach (string depend in this.m_manifest.GetAllDependencies(nameDotVariant))
            {
                if (!ret.Contains(depend))
                {
                    ret.Add(depend);
                    yield return this.allDependenciesNames(ret, depend, counterForSleep);
                }
            }

            yield return null;

        }

        /// <summary>
        /// List dependencies
        /// </summary>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------------------------------------
        protected IEnumerator solveStartupDependenciesKeys()
        {

            yield return null;

            if (!this.m_manifest || this.hasError())
            {
                yield break;
            }

            int counterForSleep = 0;

            foreach (AbStruct abs in this.m_absList)
            {

                List<string> dependenciesKeys = new List<string>();
                yield return this.allDependenciesNames(dependenciesKeys, abs.nameDotVariant, counterForSleep);

                foreach (string depend in dependenciesKeys)
                {
                    if (!this.m_dependencies.ContainsKey(depend))
                    {
                        this.m_dependencies.Add(depend, null);
                    }
                }

            }

        }


        /// <summary>
        /// Load AssetBundle in runtime
        /// </summary>
        /// <param name="abs">AbStruct</param>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------------------------------------
        protected IEnumerator loadAssetBundleInRuntimeInternal(AbStruct abs)
        {

            yield return null;

            if (!this.m_manifest)
            {
                yield break;
            }

            // Caching
            {
                while (!Caching.ready)
                {
                    yield return null;
                }
            }

            float noProgressTimer = 0.0f;
            float previousProgress = 0.0f;

            using (WWW abwww = WWW.LoadFromCacheOrDownload(this.createAssetBundleUrl(abs.nameDotVariant), this.m_manifest.GetAssetBundleHash(abs.nameDotVariant)))
            {

#if !UNITY_WEBGL
                abwww.threadPriority = this.m_threadPriority;
#endif

                // wait www done
                {

                    while (!abwww.isDone)
                    {

                        if (abs.progressFunc != null)
                        {
                            abs.progressFunc(abwww);
                        }

                        // timeout
                        {

                            if (this.m_noProgressTimeOutSeconds > 0.0f)
                            {

                                if (previousProgress == abwww.progress)
                                {
                                    noProgressTimer += Time.deltaTime;
                                }

                                else
                                {
                                    noProgressTimer = 0.0f;
                                }

                                previousProgress = abwww.progress;

                                if (noProgressTimer >= this.m_noProgressTimeOutSeconds)
                                {
                                    this.setError(abwww.url, this.ConnectionTimeout);
                                    yield break;
                                }

                            }

                        }

                        yield return null;

                    }

                    if (abs.progressFunc != null)
                    {
                        abs.progressFunc(abwww);
                    }

                    yield return null;

                } // wait

                // success or fail
                {

                    // success
                    if (string.IsNullOrEmpty(abwww.error))
                    {

                        AssetBundle decrypted = null;

                        yield return this.decryptAssetBundle(abwww.assetBundle, (ab) =>
                        {
                            decrypted = ab;
                        }
                        );

                        if (decrypted)
                        {

                            if (decrypted.isStreamedSceneAssetBundle)
                            {

                                foreach (string str in decrypted.GetAllScenePaths())
                                {

                                    var aoForAdditive = SceneManager.LoadSceneAsync(Path.GetFileNameWithoutExtension(str), LoadSceneMode.Additive);

                                    while (!aoForAdditive.isDone)
                                    {
                                        yield return null;
                                    }

                                }

                            }

                            // successFunc
                            {

                                if (abs.successFunc != null)
                                {
                                    abs.successFunc(decrypted);
                                }

                                else if (abs.successDetailFunc != null)
                                {
                                    abs.successDetailFunc(decrypted, abs.identifierForDetail);
                                }

                                else if (abs.successDetailFuncForAsync != null)
                                {

                                    bool finished = false;

                                    abs.successDetailFuncForAsync(decrypted, abs.identifierForDetail, () =>
                                    {
                                        finished = true;
                                    });

                                    while (!finished)
                                    {
                                        yield return null;
                                    }

                                }

                            }

                            abs.doneSuccess = true;

                            decrypted.Unload(false);

                        }

                        else
                        {

                            if (!this.m_ignoreErrorExceptManifest)
                            {
                                this.setError(abwww.url, this.FailedToDecryptAssetBundle);
                            }

                            if (abs.failedFunc != null)
                            {
                                abs.failedFunc(abwww);
                            }

                            yield break;

                        }

                    }

                    // fail
                    else
                    {

                        if (!this.m_ignoreErrorExceptManifest)
                        {
                            this.setError(abwww.url, abwww.error);
                        }

                        if (abs.failedFunc != null)
                        {
                            abs.failedFunc(abwww);
                        }

                        yield break;

                    }

                } // success or fail

            } // using

        }

        /// <summary>
        /// Resume runtime loading
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        protected void resumeLoadingAssetBundleInRuntime()
        {

            this.setError("", "");

            if (this.m_runtimeLoading == null)
            {
                this.m_runtimeLoading = this.loadAssetBundleInRuntimeIE();
                StartCoroutine(this.m_runtimeLoading);
            }

        }

        /// <summary>
        /// Add AbStruct ro list
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="variant">variant</param>
        /// <param name="successFunc">successFunc</param>
        /// <param name="failedFunc">failedFunc</param>
        /// <param name="progressFunc">progressFunc</param>
        // -------------------------------------------------------------------------------------------------------
        public void loadAssetBundleInRuntime(
            string name,
            string variant,
            Action<AssetBundle> successFunc,
            Action<WWW> failedFunc,
            Action<WWW> progressFunc
            )
        {

            this.m_absListRuntime.Add(new AbStruct(name, variant, successFunc, failedFunc, progressFunc));

            if (this.m_runtimeLoading == null && !this.hasError())
            {
                this.m_runtimeLoading = this.loadAssetBundleInRuntimeIE();
                StartCoroutine(this.m_runtimeLoading);
            }

        }

        /// <summary>
        /// Add AbStruct ro list
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="variant">variant</param>
        /// <param name="successDetailFunc">successDetailFunc</param>
        /// <param name="failedFunc">failedFunc</param>
        /// <param name="progressFunc">progressFunc</param>
        // -------------------------------------------------------------------------------------------------------
        public void loadAssetBundleInRuntime(
            string name,
            string variant,
            Action<AssetBundle, System.Object> successDetailFunc,
            Action<WWW> failedFunc,
            Action<WWW> progressFunc,
            System.Object identifierForDetail
            )
        {

            this.m_absListRuntime.Add(new AbStruct(name, variant, successDetailFunc, failedFunc, progressFunc, identifierForDetail));

            if (this.m_runtimeLoading == null && !this.hasError())
            {
                this.m_runtimeLoading = this.loadAssetBundleInRuntimeIE();
                StartCoroutine(this.m_runtimeLoading);
            }

        }

        /// <summary>
        /// Add AbStruct ro list
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="variant">variant</param>
        /// <param name="successDetailFuncForAsync">successDetailFuncForAsync</param>
        /// <param name="failedFunc">failedFunc</param>
        /// <param name="progressFunc">progressFunc</param>
        // -------------------------------------------------------------------------------------------------------
        public void loadAssetBundleInRuntime(
            string name,
            string variant,
            Action<AssetBundle, System.Object, Action> successDetailFuncForAsync,
            Action<WWW> failedFunc,
            Action<WWW> progressFunc,
            System.Object identifierForDetail
            )
        {

            this.m_absListRuntime.Add(new AbStruct(name, variant, successDetailFuncForAsync, failedFunc, progressFunc, identifierForDetail));

            if (this.m_runtimeLoading == null && !this.hasError())
            {
                this.m_runtimeLoading = this.loadAssetBundleInRuntimeIE();
                StartCoroutine(this.m_runtimeLoading);
            }

        }

        /// <summary>
        /// Load AssetBundle in runtime
        /// </summary>
        /// <param name="keyNameDotVariant"></param>
        /// <param name="successFunc"></param>
        /// <param name="failedFunc"></param>
        /// <param name="progressFunc"></param>
        /// <returns></returns>
            // -------------------------------------------------------------------------------------------------------
        public IEnumerator loadAssetBundleInRuntimeIE()
        {

            yield return null;

            if (!this.m_manifest)
            {

                yield return this.updateManifest(null);

                if (!this.m_manifest)
                {
                    this.m_runtimeLoading = null;
                    DialogManager.Instance.showYesNoDialog(
                        this.createErrorMessage(),
                        this.resumeLoadingAssetBundleInRuntime,
                        SceneChangeManager.Instance.backToTitleSceneWithOkDialog
                        );
                    yield break;
                }

            }

            // ----------------------

            while (this.m_absListRuntime.Count > 0)
            {

                AbStruct abs = this.m_absListRuntime[0];

                // ---------------

                List<string> dependenciesKeys = new List<string>();
                Dictionary<string, AssetBundle> dependenciesTarget = new Dictionary<string, AssetBundle>();

                // allDependenciesNames
                {
                    int counterForSleep = 0;
                    yield return this.allDependenciesNames(dependenciesKeys, abs.nameDotVariant, counterForSleep);
                }

                if (this.hasError() && !this.m_ignoreErrorExceptManifest)
                {
                    this.m_runtimeLoading = null;
                    DialogManager.Instance.showYesNoDialog(
                        this.createErrorMessage(),
                        this.resumeLoadingAssetBundleInRuntime,
                        SceneChangeManager.Instance.backToTitleSceneWithOkDialog
                        );
                    yield break;
                }

                foreach (string depend in dependenciesKeys)
                {
                    if (!dependenciesTarget.ContainsKey(depend))
                    {
                        dependenciesTarget.Add(depend, null);
                    }
                }

                // loadAllDependencies
                {
                    yield return this.loadAllDependencies(dependenciesTarget);
                }

                if (this.hasError() && !this.m_ignoreErrorExceptManifest)
                {
                    this.m_runtimeLoading = null;
                    DialogManager.Instance.showYesNoDialog(
                        this.createErrorMessage(),
                        this.resumeLoadingAssetBundleInRuntime,
                        SceneChangeManager.Instance.backToTitleSceneWithOkDialog
                        );
                    yield break;
                }

                // loadAssetBundleRuntimeInternal
                {
                    yield return this.loadAssetBundleInRuntimeInternal(abs);
                }

                // clear dependencies
                {

                    foreach (var kv in dependenciesTarget)
                    {

                        if (kv.Value)
                        {
                            kv.Value.Unload(false);
                        }

                    }

                    dependenciesTarget.Clear();
                }

                if (this.hasError() && !this.m_ignoreErrorExceptManifest)
                {
                    this.m_runtimeLoading = null;
                    DialogManager.Instance.showYesNoDialog(
                        this.createErrorMessage(),
                        this.resumeLoadingAssetBundleInRuntime,
                        SceneChangeManager.Instance.backToTitleSceneWithOkDialog
                        );
                    yield break;
                }

                // remove 0
                {
                    this.m_absListRuntime.RemoveAt(0);
                }

            }

            this.m_runtimeLoading = null;

        }

    }

}
