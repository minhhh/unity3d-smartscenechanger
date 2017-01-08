namespace SSC
{

    /// <summary>
    /// IEnumeratorStartupState class
    /// </summary>
    public class IEnumeratorStartupState
    {

        /// <summary>
        /// enum state
        /// </summary>
        public enum StateEnum
        {
            StartBefore,
            RestartBefore,
            StartAfter,
            RestartAfter,
            ErrorBefore,
            ErrorAfter,
            DoneBefore,
            DoneAfter,
            Clear,
        }

        /// <summary>
        /// Current state
        /// </summary>
        public StateEnum stateEnum = StateEnum.DoneAfter;

        /// <summary>
        /// Current error message
        /// </summary>
        public string error = "";

        /// <summary>
        /// Set state
        /// </summary>
        /// <param name="watcher">watcher</param>
        /// <param name="_stateEnum">stateEnum</param>
        /// <param name="_error">error</param>
        public void setState(StateWatcher<IEnumeratorStartupState> watcher, StateEnum _stateEnum, string _error)
        {
            this.stateEnum = _stateEnum;
            this.error = _error;

            watcher.sendState();
        }

    }

}
