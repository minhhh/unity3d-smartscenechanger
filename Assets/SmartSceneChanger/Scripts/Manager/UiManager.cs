using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using UiIdentifiers = System.Collections.Generic.List<string>;

namespace SSC
{

    /// <summary>
    /// Ui group and its default Selectable
    /// </summary>
    [Serializable]
    public class UiListAndDefaultSelectable
    {

        /// <summary>
        /// UI identifier
        /// </summary>
        [Tooltip("UI identifier")]
        public string identifier = "";

        /// <summary>
        /// UI list
        /// </summary>
        [Tooltip("UI list")]
        public List<UiControllerScript> uiList = new List<UiControllerScript>();

        /// <summary>
        /// Default Selectable when showing
        /// </summary>
        [Tooltip("Default Selectable when showing")]
        public Selectable defaultSelectable;

        /// <summary>
        /// Send pause signal when showing
        /// </summary>
        [Tooltip("Send pause signal when showing")]
        public bool sendPauseSignal = false;

    }

    /// <summary>
    /// UI singleton manager
    /// </summary>
    public abstract class UiManager : MonoBehaviour
    {

        /// <summary>
        /// Current showing ui id
        /// </summary>
        protected UiIdentifiers m_currentShowingUi = new UiIdentifiers();

        /// <summary>
        /// Previous showing ui id
        /// </summary>
        protected UiIdentifiers m_previousShowingUi = new UiIdentifiers();

        /// <summary>
        /// UI group from inspector
        /// </summary>
        [SerializeField]
        [Tooltip("UI Group")]
        protected List<UiListAndDefaultSelectable> m_uiGroups = new List<UiListAndDefaultSelectable>();

        /// <summary>
        /// UI group Dictionary
        /// </summary>
        protected Dictionary<string, UiListAndDefaultSelectable> m_uiDictionary = new Dictionary<string, UiListAndDefaultSelectable>();

        /// <summary>
        /// There are any UIs in showing transition 
        /// </summary>
        protected bool m_nowInShowingTransition = false;

        /// <summary>
        /// There are any UIs in hiding transition 
        /// </summary>
        protected bool m_nowInHidingTransition = false;

        ///// <summary>
        ///// Current pause state
        ///// </summary>
        //protected bool m_currentPauseState = false;

        // ----------------------------------------------------------------------------------------

        /// <summary>
        /// m_currentShowingUi AsReadOnly
        /// </summary>
        public IList<string> currentShowingUiAsReadOnly { get { return this.m_currentShowingUi.AsReadOnly(); } }

        /// <summary>
        /// m_currentShowingUi Copy
        /// </summary>
        public List<string> currentShowingUiCopy { get { return new List<string>(this.m_currentShowingUi); } }

        /// <summary>
        /// m_previousShowingUi AsReadOnly
        /// </summary>
        public IList<string> previousShowingUiAsReadOnly { get { return this.m_previousShowingUi.AsReadOnly(); } }

        /// <summary>
        /// m_previousShowingUi Copy
        /// </summary>
        public List<string> previousShowingUiCopy { get { return new List<string>(this.m_previousShowingUi); } }

        /// <summary>
        /// Are there any UIs in showing transition
        /// </summary>
        public bool nowInShowingTransition { get { return this.m_nowInShowingTransition; } }

        /// <summary>
        /// Are there any UIs in hiding transition
        /// </summary>
        public bool nowInHidengTransition { get { return this.m_nowInHidingTransition; } }

        /// <summary>
        /// Are there any UIs in showing or hiding transition
        /// </summary>
        public bool nowInShowingOrHidingTransition { get { return (this.m_nowInShowingTransition || this.m_nowInHidingTransition); } }

        // ----------------------------------------------------------------------------------------

        /// <summary>
        /// Awake
        /// </summary>
        // ----------------------------------------------------------------------------------------
        protected virtual void Awake()
        {

            this.initUiDictionary();

        }

        /// <summary>
        /// Init m_uiDictionary
        /// </summary>
        // ----------------------------------------------------------------------------------------
        protected void initUiDictionary()
        {

            this.m_uiDictionary.Clear();

            foreach (UiListAndDefaultSelectable val in this.m_uiGroups)
            {

                if (string.IsNullOrEmpty(val.identifier))
                {

                    Debug.LogError("Empty UI identifier not allowed");
                    continue;
                }

                if (this.m_uiDictionary.ContainsKey(val.identifier))
                {
                    Debug.LogError("m_uiDictionary already contains a key : " + val.identifier);
                    continue;
                }

                this.m_uiDictionary.Add(val.identifier, val);

            }

        }

        /// <summary>
        /// Contains identifier
        /// </summary>
        /// <param name="id">UI identifier</param>
        /// <returns>contains</returns>
        // ----------------------------------------------------------------------------------------
        public bool containsIdentifier(string id)
        {
            return this.m_currentShowingUi.Contains(id);
        }

        /// <summary>
        /// Add element without duplicated
        /// </summary>
        /// <param name="currentList">current</param>
        /// <param name="targetList">result target</param>
        // ----------------------------------------------------------------------------------------
        protected void addElementWithoutDuplicated(List<UiControllerScript> currentList, List<UiControllerScript> targetList)
        {

            int size = targetList.Count;

            for (int i = 0; i < size; i++)
            {
                if (!currentList.Contains(targetList[i]))
                {
                    currentList.Add(targetList[i]);
                }
            }

        }

        // ----------------------------------------------------------------------------------------
        protected virtual List<UiControllerScript> listByIdentifier(UiIdentifiers identifiers, List<UiControllerScript> removeElements = null)
        {

            List<UiControllerScript> ret = new List<UiControllerScript>();

            foreach (string id in identifiers)
            {

                if (this.m_uiDictionary.ContainsKey(id))
                {
                    this.addElementWithoutDuplicated(ret, this.m_uiDictionary[id].uiList);
                }

            }

            if (removeElements != null)
            {
                for (int i = ret.Count - 1; i >= 0; i--)
                {
                    if (removeElements.Contains(ret[i]))
                    {
                        ret.RemoveAt(i);
                    }
                }
            }

            return ret;

        }

        /// <summary>
        /// Show UIs by list
        /// </summary>
        /// <param name="list">UiControllerScript list</param>
        /// <param name="restartShowing">restart showing ui if already showing</param>
        /// <param name="autoHideInvoke">Invoke hide function</param>
        /// <param name="showAllDoneCallback">callback when all showing done</param>
        // ----------------------------------------------------------------------------------------
        protected void showUiByList(
            List<UiControllerScript> list,
            bool restartShowing,
            float autoHideInvoke,
            Action showAllDoneCallback
            )
        {

            if (list.Count <= 0)
            {
                if (showAllDoneCallback != null)
                {
                    showAllDoneCallback();
                }
                return;
            }

            this.m_nowInShowingTransition = true;

            int counter = 0;

            for (int i = list.Count - 1; i >= 0; i--)
            {

                if (!list[i])
                {
                    list.RemoveAt(i);
                    continue;
                }

                // -------------

                list[i].startShowing(restartShowing, autoHideInvoke, () =>
                {

                    counter++;

                    if (counter >= list.Count)
                    {

                        this.m_nowInShowingTransition = false;

                        this.setSelectable(this.m_currentShowingUi);

                        if (showAllDoneCallback != null)
                        {
                            showAllDoneCallback();
                        }

                    }

                });

            }

        }

        /// <summary>
        /// Hide UIs by list
        /// </summary>
        /// <param name="list">UiControllerScript list</param>
        /// <param name="showAllDoneCallback">callback when all hiding done</param>
        // ----------------------------------------------------------------------------------------
        protected void hideUiByList(List<UiControllerScript> list, Action hideAllDoneCallback)
        {

            if (list.Count <= 0)
            {
                if (hideAllDoneCallback != null)
                {
                    hideAllDoneCallback();
                }
                return;
            }

            this.m_nowInHidingTransition = true;

            int counter = 0;

            for (int i = list.Count - 1; i >= 0; i--)
            {

                if (!list[i])
                {
                    list.RemoveAt(i);
                    continue;
                }

                // -------------

                list[i].startHiding(() =>
                {

                    counter++;

                    if (counter >= list.Count)
                    {

                        this.m_nowInHidingTransition = false;

                        if (hideAllDoneCallback != null)
                        {
                            hideAllDoneCallback();
                        }

                    }

                });

            }

        }

        /// <summary>
        /// Back button function
        /// </summary>
        // ----------------------------------------------------------------------------------------
        public void back(bool updateHistory)
        {
            this.showUi(this.previousShowingUiCopy, updateHistory, false);
        }

        /// <summary>
        /// Show Ui button function
        /// </summary>
        /// <param name="identifier">ui identifier</param>
        // ----------------------------------------------------------------------------------------
        public void showUiOnButtonClick(string identifier)
        {
            this.showUi(identifier, true, false);
        }

        /// <summary>
        /// Show Ui button function without updating previous ui identifier
        /// </summary>
        /// <param name="identifier">ui identifier</param>
        // ----------------------------------------------------------------------------------------
        public void showUiOnButtonClickWithoutUpdatingHistory(string identifier)
        {
            this.showUi(identifier, false, false);
        }

        /// <summary>
        /// Hide UI
        /// </summary>
        [Obsolete("Use showUi(\"\", ---)", true)]
        // ----------------------------------------------------------------------------------------
        public void hideUi()
        {
            
        }

        /// <summary>
        /// Show UI
        /// </summary>
        /// <param name="identifier">Ui identifier</param>
        /// <param name="updateUiHistory">update m_previousShowingUi</param>
        /// <param name="restartShowing">restart showing ui if already showing</param>
        /// <param name="autoHideInvoke">Invoke hide function</param>
        /// <param name="showAllDoneCallback">callback when all showing done</param>
        /// <param name="hideAllDoneCallback">callback when all hiding done</param>
        // ----------------------------------------------------------------------------------------
        public void showUi(
            string identifier,
            bool updateUiHistory,
            bool restartShowing,
            float autoHideInvoke = 0.0f,
            Action showAllDoneCallback = null,
            Action hideAllDoneCallback = null
            )
        {
            this.showUi(
                new List<string>() { identifier },
                updateUiHistory,
                restartShowing,
                autoHideInvoke,
                showAllDoneCallback,
                hideAllDoneCallback
                );
        }

        /// <summary>
        /// Show UI
        /// </summary>
        /// <param name="identifiers">UiIdentifiers</param>
        /// <param name="updateUiHistory">update m_previousShowingUi</param>
        /// <param name="restartShowing">restart showing ui if already showing</param>
        /// <param name="autoHideInvoke">Invoke hide function</param>
        /// <param name="showAllDoneCallback">callback when all showing done</param>
        /// <param name="hideAllDoneCallback">callback when all hiding done</param>
            // ----------------------------------------------------------------------------------------
        public void showUi(
            UiIdentifiers identifiers,
            bool updateUiHistory,
            bool restartShowing,
            float autoHideInvoke = 0.0f,
            Action showAllDoneCallback = null,
            Action hideAllDoneCallback = null
            )
        {

            this.showUiInternal(
                identifiers,
                updateUiHistory,
                restartShowing,
                autoHideInvoke,
                showAllDoneCallback,
                hideAllDoneCallback
                );

        }

        /// <summary>
        /// Should send pause state
        /// </summary>
        /// <returns>send or not</returns>
        // -------------------------------------------------------------------------------------
        public virtual bool shouldSendPauseState()
        {

            foreach(string id in this.m_currentShowingUi)
            {
                if(this.m_uiDictionary.ContainsKey(id))
                {
                    if(this.m_uiDictionary[id].sendPauseSignal)
                    {
                        return true;
                    }
                }
            }

            return false;

        }

        /// <summary>
        /// Send pause state if needed
        /// </summary>
        // -------------------------------------------------------------------------------------
        protected static void sendPauseSignalIfNeeded()
        {

            bool temp = false;

            temp = temp || CommonUiManager.Instance.shouldSendPauseState();

            if (SceneUiManager.Instance)
            {
                temp = temp || SceneUiManager.Instance.shouldSendPauseState();
            }

            var pState = SimpleReduxManager.Instance.PauseStateWatcher.state();

            if(temp != pState.pause)
            {
                pState.setState(SimpleReduxManager.Instance.PauseStateWatcher, temp);
            }

        }

        // -------------------------------------------------------------------------------------
        protected void showUiInternal(
            UiIdentifiers identifiers,
            bool updateUiHistory,
            bool restartShowing,
            float autoHideInvoke,
            Action showAllDoneCallback,
            Action hideAllDoneCallback
            )
        {

            for (int i = identifiers.Count - 1; i >= 0; i--)
            {

                if(string.IsNullOrEmpty(identifiers[i]))
                {
                    identifiers.RemoveAt(i);
                }

            }

#if UNITY_EDITOR

            foreach (string id in identifiers)
            {

                if (!this.m_uiDictionary.ContainsKey(id))
                {
                    Debug.LogWarning("(#if UNITY_EDITOR) m_uiDictionary not contain : " + id);
                }

            }

#endif

            List<UiControllerScript> showList = this.listByIdentifier(identifiers);
            List<UiControllerScript> hideList = this.listByIdentifier(this.m_currentShowingUi, showList);

            if (updateUiHistory)
            {
                this.m_previousShowingUi.Clear();
                this.m_previousShowingUi.AddRange(this.m_currentShowingUi);
            }

            // set m_currentShowingUi
            {
                this.m_currentShowingUi.Clear();
                this.m_currentShowingUi.AddRange(identifiers);
            }

            // sendPauseSignalIfNeeded
            {
                sendPauseSignalIfNeeded();
            }

            // show hide
            {
                this.hideUiByList(hideList, hideAllDoneCallback);
                this.showUiByList(showList, restartShowing, autoHideInvoke, showAllDoneCallback);
            }

        }

        /// <summary>
        /// Set Selectable
        /// </summary>
        /// <param name="identifiers">UiIdentifiers</param>
        // -------------------------------------------------------------------------------------
        protected void setSelectable(UiIdentifiers identifiers)
        {

#if UNITY_IOS || UNITY_ANDROID

            EventSystem.current.SetSelectedGameObject(null);
            return;
#else

            if (identifiers.Count <= 0)
            {
                EventSystem.current.SetSelectedGameObject(null);
                return;
            }

            // ----------------

            string id = identifiers[0];

            if (this.m_uiDictionary.ContainsKey(id))
            {

                if (this.m_uiDictionary[id].defaultSelectable)
                {
                    this.m_uiDictionary[id].defaultSelectable.Select();
                }

                else
                {
                    EventSystem.current.SetSelectedGameObject(null);
                }

            }

            else
            {
                EventSystem.current.SetSelectedGameObject(null);
            }

#endif

        }

    }

}
