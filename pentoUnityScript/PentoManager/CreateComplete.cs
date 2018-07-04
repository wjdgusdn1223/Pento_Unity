using UnityEngine;
using System.Collections;

/// <summary>
/// Puzzle Create Complete
/// </summary>
public class CreateComplete : MonoBehaviour
{
    //----------------------------------
    /// <summary>
    /// CREATE_COMPLETE　イベントをListenerに転送
    /// </summary>
    public void OnClick()
    {
        //  CREATE_COMPLETE　イベントをListenerに転送
        EventManager.Instance.PostNotification(EVENT_TYPE.CREATE_COMPLETE, this);
    }
}
