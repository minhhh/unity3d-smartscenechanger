using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SSC
{

    /// <summary>
    /// interface for onSceneLoaded
    /// </summary>
    public interface IResetStateOnSceneLoaded
    {
        void resetOnSceneLevelLoaded();
    }

    /// <summary>
    /// Class for simple redux pattern
    /// </summary>
    public class SimpleReduxManager : SingletonMonoBehaviour<SimpleReduxManager>
    {

        /// <summary>
        /// StateWatcher for SceneChangeState
        /// </summary>
        StateWatcher<SceneChangeState> m_sceneChangeStateWatcher = new StateWatcher<SceneChangeState>();

        /// <summary>
        /// StateWatcher for PauseState
        /// </summary>
        StateWatcher<PauseState> m_pauseStateWatcher = new StateWatcher<PauseState>();

        // ----------------------------------------------------------------------------------------------

        /// <summary>
        /// StateWatcher<SceneChangeState> getter
        /// </summary>
        public StateWatcher<SceneChangeState> SceneChangeStateWatcher { get { return this.m_sceneChangeStateWatcher; } }

        /// <summary>
        /// StateWatcher<PauseState> getter
        /// </summary>
        public StateWatcher<PauseState> PauseStateWatcher { get { return this.m_pauseStateWatcher; } }

        /// <summary>
        /// Called in Awake
        /// </summary>
        // ----------------------------------------------------------------------------------------------
        protected override void initOnAwake()
        {

            SceneManager.sceneLoaded += this.resetOnSceneLoaded;

        }

        /// <summary>
        /// Reset states on scene loaded
        /// </summary>
        /// <param name="scene">Scene</param>
        /// <param name="mode">LoadSceneMode</param>
        // ----------------------------------------------------------------------------------------------
        protected void resetOnSceneLoaded(Scene scene, LoadSceneMode mode)
        {

            if(mode != LoadSceneMode.Single)
            {
                return;
            }

            // ---------------

            this.m_sceneChangeStateWatcher.state().resetOnSceneLevelLoaded();
            this.m_pauseStateWatcher.state().resetOnSceneLevelLoaded();

        }


        /// <summary>
        /// Add scene change state receiver action
        /// </summary>
        /// <param name="action">PauseState</param>
        // ----------------------------------------------------------------------------------------------
        public void addSceneChangeStateReceiver(Action<SceneChangeState> action)
        {

            if (action != null)
            {
                this.m_sceneChangeStateWatcher.addAction(action);
            }

        }

        /// <summary>
        /// Add pause state receiver action
        /// </summary>
        /// <param name="action">PauseState</param>
        // ----------------------------------------------------------------------------------------------
        public void addPauseStateReceiver(Action<PauseState> action)
        {

            if(action != null)
            {
                this.m_pauseStateWatcher.addAction(action);
            }

        }

    }

}
