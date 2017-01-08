using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSC
{

    /// <summary>
    /// Class for simple redux pattern
    /// </summary>
    public class SimpleReduxManager : SingletonMonoBehaviour<SimpleReduxManager>
    {

        /// <summary>
        /// StateWatcher for SceneChangeState
        /// </summary>
        StateWatcher<SceneChangeState> m_sceneStateWatcher = new StateWatcher<SceneChangeState>();

        /// <summary>
        /// StateWatcher for WwwStartupState
        /// </summary>
        StateWatcher<WwwStartupState> m_wwwStartupWatcher = new StateWatcher<WwwStartupState>();

        /// <summary>
        /// StateWatcher for AssetBundleStartupState
        /// </summary>
        StateWatcher<AssetBundleStartupState> m_abStartupWatcher = new StateWatcher<AssetBundleStartupState>();

        /// <summary>
        /// StateWatcher for YesNoDialogState
        /// </summary>
        StateWatcher<YesNoDialogState> m_yesnoDialogWatcher = new StateWatcher<YesNoDialogState>();

        /// <summary>
        /// StateWatcher for OkDialogState
        /// </summary>
        StateWatcher<OkDialogState> m_okDialogWatcher = new StateWatcher<OkDialogState>();

        /// <summary>
        /// StateWatcher for IEnumeratorStartupState
        /// </summary>
        StateWatcher<IEnumeratorStartupState> m_ieStartupWatcher = new StateWatcher<IEnumeratorStartupState>();





        /// <summary>
        /// StateWatcher<SceneChangeState> getter
        /// </summary>
        public StateWatcher<SceneChangeState> SceneChangeStateWatcher { get { return this.m_sceneStateWatcher; } }

        /// <summary>
        /// StateWatcher<WwwStartupState> getter
        /// </summary>
        public StateWatcher<WwwStartupState> WwwStartupStateWatcher { get { return this.m_wwwStartupWatcher; } }

        /// <summary>
        /// StateWatcher<AssetBundleStartupState> getter
        /// </summary>
        public StateWatcher<AssetBundleStartupState> AssetBundleStartupStateWatcher { get { return this.m_abStartupWatcher; } }

        /// <summary>
        /// StateWatcher<AssetBundleStartupState> getter
        /// </summary>
        public StateWatcher<IEnumeratorStartupState> IEnumeratorStartupStateWatcher { get { return this.m_ieStartupWatcher; } }

        /// <summary>
        /// StateWatcher<YesNoDialogState> getter
        /// </summary>
        public StateWatcher<YesNoDialogState> YesNoDialogStateWatcher { get { return this.m_yesnoDialogWatcher; } }

        /// <summary>
        /// StateWatcher<OkDialogState> getter
        /// </summary>
        public StateWatcher<OkDialogState> OkDialogStateWatcher { get { return this.m_okDialogWatcher; } }

        /// <summary>
        /// override
        /// </summary>
        protected override void initOnAwake()
        {

        }

    }

}
