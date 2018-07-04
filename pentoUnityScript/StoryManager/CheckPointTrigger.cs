using UnityEngine;
using System.Collections;

/// <summary>
/// チェックポイントをチェック
/// </summary>
public class CheckPointTrigger : MonoBehaviour
{
    //-------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        //  CHARACTER_ARRIVED　イベントをListenerに転送
        EventManager.Instance.PostNotification(EVENT_TYPE.CHARACTER_ARRIVED, this, other.name);
    }
}
