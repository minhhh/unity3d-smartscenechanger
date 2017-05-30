namespace SSC
{

    /// <summary>
    /// PauseState class
    /// </summary>
    public class PauseState : IResetStateOnSceneLoaded
    {

        /// <summary>
        /// Pause state
        /// </summary>
        public bool pause = false;

        /// <summary>
        /// Set state
        /// </summary>
        /// <param name="watcher">watcher</param>
        /// <param name="_pause">pause</param>
        // ----------------------------------------------------------------------------------------------
        public void setState(StateWatcher<PauseState> watcher, bool _pause)
        {
            this.pause = _pause;

            watcher.sendState();
        }

        /// <summary>
        /// Reset params on scene loaded
        /// </summary>
        // ----------------------------------------------------------------------------------------------
        public void resetOnSceneLevelLoaded()
        {
            this.pause = false;
        }

    }

}
