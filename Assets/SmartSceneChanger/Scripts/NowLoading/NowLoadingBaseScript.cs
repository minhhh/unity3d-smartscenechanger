using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSC
{

    /// <summary>
    /// Class for NowLoading
    /// </summary>
    public abstract class NowLoadingBaseScript : MonoBehaviour
    {

        /// <summary>
        /// Flag for main loop has finished
        /// </summary>
        bool m_mainLoopDone = true;

        /// <summary>
        /// Intro IEnumerator
        /// </summary>
        /// <returns>IEnumerator</returns>
        protected abstract IEnumerator intro();

        /// <summary>
        /// Outro IEnumerator
        /// </summary>
        /// <returns>IEnumerator</returns>
        protected abstract IEnumerator outro();

        /// <summary>
        /// Main loop IEnumerator
        /// </summary>
        /// <returns>IEnumerator</returns>
        protected abstract IEnumerator mainLoop();

        /// <summary>
        /// Start intro
        /// </summary>
        /// <returns>IEnumerator</returns>
        // --------------------------------------------------------------
        public IEnumerator startIntro()
        {

            // intro
            {
                yield return this.intro();
            }

            // state
            {
                var state = SimpleReduxManager.Instance.SceneChangeStateWatcher.state();
                state.setState(
                    SimpleReduxManager.Instance.SceneChangeStateWatcher,
                    SceneChangeState.StateEnum.NowLoadingMain,
                    state.nextSceneName
                    );
            }

            yield return null;

        }

        /// <summary>
        /// Start main loop
        /// </summary>
        /// <returns>IEnumerator</returns>
        // --------------------------------------------------------------
        public IEnumerator startMainLoop()
        {
            this.m_mainLoopDone = false;
            yield return this.mainLoop();
            this.m_mainLoopDone = true;
        }

        /// <summary>
        /// Start outro
        /// </summary>
        /// <returns>IEnumerator</returns>
        // --------------------------------------------------------------
        public IEnumerator startOutro()
        {

            // wait
            {
                while (!this.m_mainLoopDone)
                {
                    yield return null;
                }
            }

            // outro
            {
                yield return this.outro();
            }

            // state
            {
                var state = SimpleReduxManager.Instance.SceneChangeStateWatcher.state();
                state.setState(
                    SimpleReduxManager.Instance.SceneChangeStateWatcher,
                    SceneChangeState.StateEnum.ScenePlaying,
                    state.nextSceneName
                    );
            }

            yield return null;

        }

    }

}
