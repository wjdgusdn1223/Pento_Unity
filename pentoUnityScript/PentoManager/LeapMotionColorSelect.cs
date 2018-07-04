using UnityEngine;
using System.Collections;

/// <summary>
/// 手で色を選択する時実行される
/// </summary>
public class LeapMotionColorSelect : MonoBehaviour
{
    //  手で色を選択する時実行される
    public void OnTriggerEnter(Collider other)
    {
        if (other.name.Equals("touch"))
        {
            //  COLOR_SELECT　イベントをListenerに転送
            EventManager.Instance.PostNotification(EVENT_TYPE.COLOR_SELECT, this, gameObject.name);

            Debug.Log(gameObject.name);

            //  COLOR_SELECT　Effectを実行
            EffectController.Instance.SpawnParticle(39, "", 1f * PentoGameController.Instance.BS, other.transform.position);
        }
    }
}
