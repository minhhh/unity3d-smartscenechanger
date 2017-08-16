using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSC
{

    public enum UiType
    {
        CommonUi,
        SceneUi
    }

    [Serializable]
    public class UiTypeAndIdentifier
    {
        public UiType uiType = UiType.CommonUi;
        public string uiIdentifier = "";

        public UiTypeAndIdentifier()
        {

        }

        public UiTypeAndIdentifier(UiType _uiType, string _uiIdentifier)
        {
            this.uiType = _uiType;
            this.uiIdentifier = _uiIdentifier;
        }

    }

}
