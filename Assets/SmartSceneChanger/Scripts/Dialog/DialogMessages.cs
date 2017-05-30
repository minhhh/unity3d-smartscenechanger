using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSC
{

    /// <summary>
    /// Dialog messages
    /// </summary>
    public class DialogMessages
    {

        /// <summary>
        /// Message category
        /// </summary>
        public enum MessageCategory
        {
            Confirmation,
            Warning,
            Error,
            Other,
        }

        /// <summary>
        /// Category
        /// </summary>
        public MessageCategory category = MessageCategory.Confirmation;

        /// <summary>
        /// Url if needed
        /// </summary>
        public string urlIfNeeded = "";

        /// <summary>
        /// Title
        /// </summary>
        public string title = "";

        /// <summary>
        /// Main message
        /// </summary>
        public string mainMessage = "";

        /// <summary>
        /// Sub message
        /// </summary>
        public string subMessage = "";

    }

}
