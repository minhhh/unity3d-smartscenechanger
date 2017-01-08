namespace SSC
{

    /// <summary>
    /// SceneChangeState class
    /// </summary>
    public class SceneChangeState
    {

        /// <summary>
        /// enum state
        /// </summary>
        public enum StateEnum
        {
            ScenePlaying,
            NowLoadingIntro,
            NowLoadingMain,
            NowLoadingOutro,
            InnerChange,
        }

        /// <summary>
        /// Current state
        /// </summary>
        public StateEnum stateEnum = StateEnum.ScenePlaying;

        /// <summary>
        /// Next scene name
        /// </summary>
        public string nextSceneName = "";

        /// <summary>
        /// Set state
        /// </summary>
        /// <param name="watcher">watcher</param>
        /// <param name="_stateEnum">stateEnum</param>
        /// <param name="_nextSceneName">nextSceneName</param>
        public void setState(StateWatcher<SceneChangeState> watcher, StateEnum _stateEnum, string _nextSceneName)
        {
            this.stateEnum = _stateEnum;
            this.nextSceneName = _nextSceneName;

            watcher.sendState();
        }

    }

}
