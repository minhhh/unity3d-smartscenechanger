using System;

namespace SSC
{

    /// <summary>
    /// YesNoDialogState class
    /// </summary>
    public class YesNoDialogState
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
        /// Current message
        /// </summary>
        public string message = "";

        /// <summary>
        /// Url if needed
        /// </summary>
        public string urlForIfNeeded = "";

        /// <summary>
        /// Yes button function
        /// </summary>
        public Action yesAction = null;

        /// <summary>
        /// No button function
        /// </summary>
        public Action noAction = null;

        /// <summary>
        /// Set state
        /// </summary>
        /// <param name="watcher">watcher</param>
        /// <param name="_stateEnum">stateEnum</param>
        /// <param name="_message">message</param>
        /// <param name="_urlForIfNeeded">urlForIfNeeded</param>
        /// <param name="_yesAction">yesAction</param>
        /// <param name="_noAction">noAction</param>
        public void setState(
            StateWatcher<YesNoDialogState> watcher,
            StateEnum _stateEnum,
            string _message,
            string _urlForIfNeeded,
            Action _yesAction,
            Action _noAction
            )
        {
            this.stateEnum = _stateEnum;
            this.message = _message;
            this.urlForIfNeeded = _urlForIfNeeded;
            this.yesAction = _yesAction;
            this.noAction = _noAction;

            watcher.sendState();
        }

    }

}
