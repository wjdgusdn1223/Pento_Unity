using UnityEngine;

/// <summary>
/// Login Button 
/// </summary>
public class LoginButtonClick : MonoBehaviour
{
    //------------------------------
    public void OnClick()
    {
        //  LOGIN　イベントをListenerに転送
        EventManager.Instance.PostNotification(EVENT_TYPE.LOGIN, this);
    }
}