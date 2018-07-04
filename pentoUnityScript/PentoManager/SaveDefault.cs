using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Default Save
/// </summary>
public class SaveDefault : MonoBehaviour{

    //-----------------------------------------
    public void OnClick()
    {
        //  記録をセーブ
        EventManager.Instance.PostNotification(EVENT_TYPE.SAVE_RECORD, this);
    }

}
