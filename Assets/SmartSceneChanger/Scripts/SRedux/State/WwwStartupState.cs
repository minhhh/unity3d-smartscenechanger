namespace SSC
{

    /// <summary>
    /// WwwStartupState class
    /// </summary>
    public class WwwStartupState
    {

        /// <summary>
        /// enum state
        /// </summary>
        public enum StateEnum
        {
            Start,
            Restart,
            Error,
            Done,
            Clear,
        }

        /// <summary>
        /// Current state
        /// </summary>
        public StateEnum stateEnum = StateEnum.Done;

        /// <summary>
        /// Current error message
        /// </summary>
        public string error = "";

        /// <summary>
        /// Current error url
        /// </summary>
        public string url = "";

        /// <summary>
        /// Set state
        /// </summary>
        /// <param name="watcher">watcher</param>
        /// <param name="_stateEnum">stateEnum</param>
        /// <param name="_error">error</param>
        /// <param name="_url">url</param>
        public void setState(StateWatcher<WwwStartupState> watcher, StateEnum _stateEnum, string _error, string _url)
        {
            this.stateEnum = _stateEnum;
            this.error = _error;
            this.url = _url;

            watcher.sendState();
        }

    }

}
