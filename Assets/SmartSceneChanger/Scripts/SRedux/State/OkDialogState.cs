using System;

namespace SSC
{

    /// <summary>
    /// OkDialogState class
    /// </summary>
    public class OkDialogState
    {

        /// <summary>
        /// enum state
        /// </summary>
        public enum StateEnum
        {
            Show,
            Done,
        }

        /// <summary>
        /// Current state
        /// </summary>
        public StateEnum stateEnum = StateEnum.Done;

        /// <summary>
        /// message
        /// </summary>
        public string message = "";

        /// <summary>
        /// OK button function
        /// </summary>
        public Action okAction = null;

        /// <summary>
        /// Set state
        /// </summary>
        /// <param name="watcher">watcher</param>
        /// <param name="_stateEnum">stateEnum</param>
        /// <param name="_message">message</param>
        /// <param name="_okAction">okAction</param>
        public void setState(
            StateWatcher<OkDialogState> watcher,
            StateEnum _stateEnum,
            string _message,
            Action _okAction
            )
        {
            this.stateEnum = _stateEnum;
            this.message = _message;
            this.okAction = _okAction;

            watcher.sendState();
        }

    }

}
