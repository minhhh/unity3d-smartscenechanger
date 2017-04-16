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
        class AbStruct
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

        }

        /// <summary>
        /// Class for error handling and reloading
        /// </summary>
        class InnerState
        {

            /// <summary>
            /// url for error
            /// </summary>
            public string url = "";

            /// <summary>
            /// error message
            /// </summary>
            public string error = "";

            /// <summary>
            /// reloading flag for new manifest detected
            /// </summary>
            public bool newManifestDetected = false;

            /// <summary>
            /// reloading flag for additive scene detected
            /// </summary>
            public bool sceneAdditiveDetected = false;

            /// <summary>
            /// Clear params
            /// </summary>
            public void clear()
            {
                this.url = "";
                this.error = "";
                this.newManifestDetected = false;
                this.sceneAdditiveDetected = false;
            }

        }

        /// <summary>
        /// AssetBundles dependencies
        /// </summary>
        Dictionary<string, AssetBundle> m_dependencies = new Dictionary<string, AssetBundle>();

        /// <summary>
        /// AbStruct list
        /// </summary>
        List<AbStruct> m_absList = new List<AbStruct>();

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
        /// InnerState instance
        /// </summary>
        InnerState m_is = new InnerState();

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
        /// Progress information
        /// </summary>
        protected ProgressStruct m_progress = new ProgressStruct();

        /// <summary>
        /// override
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        protected override void initOnAwake()
        {

            this.m_numberOfCo = Math.Max(1, this.m_numberOfCo);
            this.m_progress.clear(this.m_numberOfCo);

            SimpleReduxManager.Instance.AssetBundleStartupStateWatcher.addAction(this.onAbStartupState);
            SimpleReduxManager.Instance.SceneChangeStateWatcher.addAction(this.onSceneChangeState);

#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)

            if(!SystemInfo.graphicsDeviceType.ToString().ToLower().Contains("opengl"))
            {
                Debug.LogWarning("Use OpenGLES, or you will see pink shader, perhaps.");
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

            Debug.LogWarning("You must override this function as you want.");

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
        /// Get AssetBundle from already loaded dependencies
        /// </summary>
        /// <param name="nameDotVariant">AssetBundle name</param>
        /// <returns>if found, return AssetBundle, if not, return null</returns>
        // -------------------------------------------------------------------------------------------------------
        AssetBundle assetBundleFromDependencies(string nameDotVariant)
        {
            if (this.m_dependencies.ContainsKey(nameDotVariant))
            {
                return this.m_dependencies[nameDotVariant];
            }
            return null;
        }

        /// <summary>
        /// Action on AssetBundleStartupStateWatcher
        /// </summary>
        /// <param name="state">current state</param>
        // -------------------------------------------------------------------------------------------------------
        void onAbStartupState(AssetBundleStartupState state)
        {

            if (state.stateEnum == AssetBundleStartupState.StateEnum.Start)
            {
                this.startAbStartup(false);
            }

            else if (state.stateEnum == AssetBundleStartupState.StateEnum.Restart)
            {
                this.startAbStartup(true);
            }

            else if (state.stateEnum == AssetBundleStartupState.StateEnum.Clear)
            {
                this.clearAll(true);
            }

            else if (state.stateEnum == AssetBundleStartupState.StateEnum.Done)
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
        /// <param name="clearAssetBundle">clear already loaded AssetBundles</param>
        // -------------------------------------------------------------------------------------------------------
        void clearAll(bool clearAssetBundles)
        {

            // m_is
            {
                this.m_is.clear();
            }

            // m_absList
            {

                if(clearAssetBundles)
                {

                    foreach (string key in new List<string>(this.m_dependencies.Keys))
                    {

                        if (this.m_dependencies[key])
                        {
                            this.m_dependencies[key].Unload(false);
                        }

                    }

                    this.m_dependencies.Clear();
                    this.m_absList.Clear();

                }

            }

        }

        /// <summary>
        /// If manifest is available
        /// </summary>
        /// <returns>manifest is available</returns>
        // -------------------------------------------------------------------------------------------------------
        bool hasManifest()
        {
            return this.m_manifest;
        }

        /// <summary>
        /// If error string is not empty
        /// </summary>
        /// <returns>error string is not empty</returns>
        // -------------------------------------------------------------------------------------------------------
        bool hasError()
        {
            return !string.IsNullOrEmpty(this.m_is.error);
        }

        /// <summary>
        /// Set error message
        /// </summary>
        /// <param name="url">url if needed</param>
        /// <param name="error">error message</param>
        // -------------------------------------------------------------------------------------------------------
        void setError(string url, string error)
        {

            if(string.IsNullOrEmpty(error))
            {
                this.m_is.url = url;
                this.m_is.error = error;
            }

            else if(string.IsNullOrEmpty(this.m_is.error))
            {
                this.m_is.url = url;
                this.m_is.error = error;
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
                this.m_progress.progressDenominator =  this.m_dependencies.Count + this.m_absList.Count;
            }

            if (updateOnlyDenominator)
            {
                return;
            }

            // numerator
            {

                float numerator = 0.0f;

                foreach (var kv in this.m_dependencies)
                {
                    if(kv.Value)
                    {
                        numerator += 1.0f;
                    }
                }

                foreach (AbStruct abs in this.m_absList)
                {
                    if (abs.doneSuccess)
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
        IEnumerator hasDifferenceOfOldAndNewManifest(Dictionary<string, Hash128> currentManifest, Dictionary<string, Hash128> newManifest)
        {

            if (currentManifest.Count <= 0 || newManifest.Count <= 0)
            {
                this.m_is.newManifestDetected = false;
                yield break;
            }

            // check length
            {

                if (currentManifest.Count != currentManifest.Count)
                {
                    this.m_is.newManifestDetected = true;
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
                        this.m_is.newManifestDetected = true;
                        yield break;
                    }

                    if (counter++ % 50 == 0)
                    {
                        yield return null;
                    }

                }

            }

            this.m_is.newManifestDetected = false;

        }

        /// <summary>
        /// Start startup
        /// </summary>
        /// <param name="restart">restart flag</param>
        // -------------------------------------------------------------------------------------------------------
        void startAbStartup(bool restart)
        {

            if (this.m_absList.Count <= 0)
            {
                var state = SimpleReduxManager.Instance.AssetBundleStartupStateWatcher.state();
                state.setState(
                    SimpleReduxManager.Instance.AssetBundleStartupStateWatcher,
                    AssetBundleStartupState.StateEnum.Done,
                    "",
                    ""
                    );
                return;
            }

            // ------------------------

            // clear
            {
                this.clearAll(false);
            }

            // startStarter
            {
                StartCoroutine(this.startStarter(restart));
            }

        }

        /// <summary>
        /// Restart loading
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        void restartAbStartup()
        {
            this.startAbStartup(true);
        }

        /// <summary>
        /// Start loading starter
        /// </summary>
        /// <param name="restart">restart flag</param>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------------------------------------
        IEnumerator startStarter(bool restart)
        {

            if (!restart)
            {
                this.setManifestFileAndFolderUrl();
            }

            if(!this.hasManifest() || !restart)
            {
                yield return this.updateManifest(false);
            }

            if (!this.hasError())
            {
                yield return this.solveDependenciesKeys();
            }

            if (!this.hasError())
            {

                int counter = 0;
                List<string> depend_keys = new List<string>(this.m_dependencies.Keys);

                for (int i = 0; i < this.m_numberOfCo; i++)
                {
                    StartCoroutine(this.startEachCoForDependencies(i, depend_keys, () =>
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

            }

            //
            if (!this.hasError())
            {

                Dictionary<string, List<AbStruct>> dict = this.createCombinedAbList();
                List<string> keys = new List<string>(dict.Keys);

                int counter = 0;
                for (int i = 0; i < this.m_numberOfCo; i++)
                {
                    StartCoroutine(this.startEachCoForMain(i, keys, dict, () =>
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

            }

            if (!this.hasError() && this.m_checkManifestAfterLoading && !this.m_is.sceneAdditiveDetected)
            {
                yield return this.updateManifest(true);
            }

            this.calcProgress(false);

            yield return null;

            this.funcAtDone();

        }

        /// <summary>
        /// Start parallel loading of dependencies
        /// </summary>
        /// <param name="startIndex">coroutine index</param>
        /// <param name="dependKeys">dependencies urls</param>
        /// <param name="doneCallback">called when done</param>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------------------------------------
        IEnumerator startEachCoForDependencies(int startIndex, List<string> dependKeys, Action doneCallback)
        {

            yield return null;

            int size = dependKeys.Count;

            for (int i = startIndex; i < size; i += this.m_numberOfCo)
            {

                if (this.hasError())
                {
                    break;
                }

                yield return this.loadDependency(dependKeys[i], this.m_dependencies[dependKeys[i]], startIndex);

            }

            doneCallback();

        }

        /// <summary>
        /// Start parallel loading of main AssetBundle
        /// </summary>
        /// <param name="startIndex">coroutine inde</param>
        /// <param name="keys">urls</param>
        /// <param name="combined">to avoid duplicated loading</param>
        /// <param name="doneCallback">called when done</param>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------------------------------------
        IEnumerator startEachCoForMain(int startIndex, List<string> keys, Dictionary<string, List<AbStruct>> combined, Action doneCallback)
        {

            yield return null;

            int size = combined.Count;

            for (int i = startIndex; i < size; i += this.m_numberOfCo)
            {

                if (this.hasError() || this.m_is.sceneAdditiveDetected)
                {
                    break;
                }

                yield return this.startAssetBundleLoading(keys[i], combined[keys[i]], startIndex, () =>
                {
                    this.m_is.sceneAdditiveDetected = true;
                });

            }

            doneCallback();

        }

        /// <summary>
        /// Detect duplicated AssetBundles to avoid duplicated loading
        /// </summary>
        /// <returns>combined list</returns>
        // -------------------------------------------------------------------------------------------------------
        Dictionary<string, List<AbStruct>> createCombinedAbList()
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
                    Debug.LogWarning("Duplicated AssetBundle loading is not recommended (but, no problem) : " + kv.Key);
                }

            }

#endif

            return ret;

        }

        /// <summary>
        /// Called when all loadings have done with any reason
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        void funcAtDone()
        {

            var ab_state = SimpleReduxManager.Instance.AssetBundleStartupStateWatcher.state();

            if (this.hasError())
            {
                ab_state.setState(
                    SimpleReduxManager.Instance.AssetBundleStartupStateWatcher,
                    AssetBundleStartupState.StateEnum.Error,
                    this.m_is.error,
                    this.m_is.url
                    );
            }

            else if (this.m_is.sceneAdditiveDetected)
            {
                Invoke("restartAbStartup", 0.1f);
            }

            else if (this.m_is.newManifestDetected)
            {

                SceneChangeState sc_state = SimpleReduxManager.Instance.SceneChangeStateWatcher.state();

                sc_state.setState(
                    SimpleReduxManager.Instance.SceneChangeStateWatcher,
                    SceneChangeState.StateEnum.InnerChange,
                    sc_state.nextSceneName
                    );
            }

            else
            {

                ab_state.setState(
                    SimpleReduxManager.Instance.AssetBundleStartupStateWatcher,
                    AssetBundleStartupState.StateEnum.Done,
                    "",
                    ""
                    );

            }

        }

        /// <summary>
        /// Decrypt binary data
        /// </summary>
        /// <param name="textAsset">binary data</param>
        /// <returns>decrypted binary data</returns>
        // -------------------------------------------------------------------------------------------------------
        protected virtual byte[] decryptBinaryData(TextAsset textAsset)
        {

            if(!textAsset)
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
        IEnumerator decryptAssetBundle(AssetBundle assetBundle, Action<AssetBundle> ret)
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

                if(decrypted != null && decrypted.Length > 0)
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
        IEnumerator updateManifest(bool check)
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

                    foreach (string str in this.m_manifest.GetAllAssetBundles())
                    {
                        if (!old_key_hash.ContainsKey(str))
                        {
                            old_key_hash.Add(str, this.m_manifest.GetAssetBundleHash(str));
                        }
                    }

                    this.m_manifestAssetBundle.Unload(true);
                    yield return null;
                }

            }
            
            using (WWW www = new WWW(this.m_assetBundleManifestFileUrl))
            {

                while (!www.isDone)
                {
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

                        if(this.m_manifestAssetBundle)
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
            if (check)
            {
                yield return this.hasDifferenceOfOldAndNewManifest(old_key_hash, new_key_hash);
            }

        }

        /// <summary>
        /// Get all dependencies names
        /// </summary>
        /// <param name="ret">return list</param>
        /// <param name="nameDotVariant">origin</param>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------------------------------------
        IEnumerator allDependenciesNames(List<string> ret, string nameDotVariant)
        {

            yield return null;

            if (!this.m_manifest || this.hasError())
            {
                yield break;
            }

            foreach (string depend in this.m_manifest.GetAllDependencies(nameDotVariant))
            {
                if (!ret.Contains(depend))
                {
                    ret.Add(depend);
                    yield return this.allDependenciesNames(ret, depend);
                }
            }

            yield return null;

        }

        /// <summary>
        /// List dependencies
        /// </summary>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------------------------------------
        IEnumerator solveDependenciesKeys()
        {

            yield return null;

            if (!this.m_manifest || this.hasError())
            {
                yield break;
            }

            foreach (AbStruct abs in this.m_absList)
            {

                List<string> dependencies = new List<string>();
                yield return this.allDependenciesNames(dependencies, abs.nameDotVariant);

                foreach (string depend in dependencies)
                {
                    if (!this.m_dependencies.ContainsKey(depend))
                    {
                        this.m_dependencies.Add(depend, null);
                    }
                }

            }

        }

        /// <summary>
        /// Download dependencies
        /// </summary>
        /// <param name="key">AssetBundle name of dependency</param>
        /// <param name="assetBundle">AssetBundle if already loaded</param>
        /// <param name="coNumber">coroutine index</param>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------------------------------------
        IEnumerator loadDependency(string key, AssetBundle assetBundle, int coNumber)
        {

            yield return null;

            if (assetBundle || this.hasError())
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

            using (WWW dependwww = WWW.LoadFromCacheOrDownload(this.createAssetBundleUrl(key), this.m_manifest.GetAssetBundleHash(key)))
            {

                while (!dependwww.isDone)
                {
                    this.m_progress.progressOfCo[coNumber] = dependwww.progress * 0.999f;
                    yield return null;
                }

                if (string.IsNullOrEmpty(dependwww.error))
                {

                    yield return this.decryptAssetBundle(dependwww.assetBundle, (ab) =>
                    {
                        this.m_progress.progressOfCo[coNumber] = 0.0f;
                        this.m_dependencies[key] = ab;
                    }
                    );

                }

                else
                {
                    this.setError(dependwww.url, dependwww.error);
                }

            }

        }

        /// <summary>
        /// Download main AssetBundle
        /// </summary>
        /// <param name="keyNameDotVariant">AssetBundle name</param>
        /// <param name="absCombined">combined list to avoid duplicated loading</param>
        /// <param name="coNumber">coroutine index</param>
        /// <param name="sceneAdditiveCallback">callback when scene additive detected</param>
        /// <returns>IEnumerator</returns>
        // -------------------------------------------------------------------------------------------------------
        IEnumerator startAssetBundleLoading(string keyNameDotVariant, List<AbStruct> absCombined, int coNumber, Action sceneAdditiveCallback)
        {

            yield return null;

            if (absCombined.Count <= 0)
            {
                yield break;
            }

            AbStruct last = absCombined.Last();

            if (last.doneSuccess || this.hasError())
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

            AssetBundle from_dependencies = this.assetBundleFromDependencies(keyNameDotVariant);

            if (from_dependencies)
            {

                foreach (var abs in absCombined)
                {

                    if (abs.successFunc != null && !abs.doneSuccess)
                    {
                        abs.successFunc(from_dependencies);
                    }

                    this.m_progress.progressOfCo[coNumber] = 0.0f;
                    abs.doneSuccess = true;

                }

            }

            else
            {
                
                using (WWW abwww = WWW.LoadFromCacheOrDownload(this.createAssetBundleUrl(keyNameDotVariant), this.m_manifest.GetAssetBundleHash(keyNameDotVariant)))
                {

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

                            this.m_progress.progressOfCo[coNumber] = abwww.progress * 0.999f;

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

                            if(decrypted)
                            {

                                if (decrypted.isStreamedSceneAssetBundle)
                                {

                                    sceneAdditiveCallback();

                                    foreach (string str in decrypted.GetAllScenePaths())
                                    {
                                        var ao = SceneManager.LoadSceneAsync(Path.GetFileNameWithoutExtension(str), LoadSceneMode.Additive);
                                        while (!ao.isDone)
                                        {
                                            yield return null;
                                        }
                                    }

#if UNITY_EDITOR
                                    if(absCombined.Count >= 2)
                                    {
                                        Debug.LogWarning("Duplicated scene was not supported.");
                                    }
#endif

                                }

                                foreach (var abs in absCombined)
                                {
                                    if (abs.successFunc != null && !abs.doneSuccess)
                                    {
                                        abs.successFunc(decrypted);
                                    }
                                }

                                decrypted.Unload(false);

                            }

                            else
                            {
                                this.setError(abwww.url, this.FailedToDecryptAssetBundle);
                                yield break;
                            }

                        }

                        // fail
                        else
                        {

                            foreach (var abs in absCombined)
                            {
                                if (abs.failedFunc != null && !abs.doneSuccess)
                                {
                                    abs.failedFunc(abwww);
                                }
                            }

                            if (!this.m_ignoreErrorExceptManifest)
                            {
                                this.setError(abwww.url, abwww.error);
                                yield break;
                            }

                        }

                    } // success or fail

                    foreach (var abs in absCombined)
                    {
                        this.m_progress.progressOfCo[coNumber] = 0.0f;
                        abs.doneSuccess = true;
                    }

                } // using

            } // else

        } // startAssetBundleLoading

    }

}
