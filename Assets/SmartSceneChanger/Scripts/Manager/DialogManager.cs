using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSC
{

    public class DialogManager : SingletonMonoBehaviour<DialogManager>
    {

        /// <summary>
        /// YesNoDialogBaseScript reference
        /// </summary>
        [SerializeField]
        [Tooltip("YesNoDialogBaseScript reference")]
        YesNoDialogBaseScript refYesNoDialog;

        /// <summary>
        /// OkDialogBaseScript reference
        /// </summary>
        [SerializeField]
        [Tooltip("OkDialogBaseScript reference")]
        OkDialogBaseScript refOkDialog;

        /// <summary>
        /// override
        /// </summary>
        // -------------------------------------------------------------------------------------------------------
        protected override void initOnAwake()
        {
            SimpleReduxManager.Instance.YesNoDialogStateWatcher.addAction(this.onYesNoDialogState);
            SimpleReduxManager.Instance.OkDialogStateWatcher.addAction(this.onOkDialogState);
        }

        /// <summary>
        /// Action on YesNoDialogStateWatcher
        /// </summary>
        /// <param name="state">current state</param>
        // -------------------------------------------------------------------------------------------------------
        void onYesNoDialogState(YesNoDialogState state)
        {

            if (state.stateEnum == YesNoDialogState.StateEnum.Show)
            {
                this.refYesNoDialog.gameObject.SetActive(true);
                this.refYesNoDialog.showDialog(state);
            }

            else if (state.stateEnum == YesNoDialogState.StateEnum.Done)
            {
                this.refYesNoDialog.gameObject.SetActive(false);
            }

        }

        /// <summary>
        /// Action on OkDialogStateWatcher
        /// </summary>
        /// <param name="state">current state</param>
        // -------------------------------------------------------------------------------------------------------
        void onOkDialogState(OkDialogState state)
        {

            if (state.stateEnum == OkDialogState.StateEnum.Show)
            {
                this.refOkDialog.gameObject.SetActive(true);
                this.refOkDialog.showDialog(state);
            }

            else if (state.stateEnum == OkDialogState.StateEnum.Done)
            {
                this.refOkDialog.gameObject.SetActive(false);
            }

        }

    }

}
