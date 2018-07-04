using UnityEngine;
using System.Collections;

/// <summary>
/// Cube Objectを選択した時、実行される
/// </summary>
public class LeapMotionCubeSelect : MonoBehaviour
{

    public void OnTriggerEnter(Collider other)
    {
        if (other.name.Equals("touch"))
        {
            //  DimensionCube を選択した場合
            if (gameObject.name.Equals("dimensionCube"))
            {
                //  DIMENSION_CHANGE　イベントをListenerに転送
                EventManager.Instance.PostNotification(EVENT_TYPE.DIMENSION_CHANGE, this, gameObject);

                //  DIMENSION_CHANGE Effectを実行
                EffectController.Instance.SpawnParticle(15, "", 1f * PentoGameController.Instance.BS, other.transform.position);
            }
            else  //  PentoBlockを選択した場合
            {
                //  PAINT_COLOR　イベントをListenerに転送
                EventManager.Instance.PostNotification(EVENT_TYPE.PAINT_COLOR, this, gameObject);

                //  色を塗るモードだけEffectを実行させる
                if (LeapMotionController.Instance.isPainting)
                {
                    //  PAINT_COLOR Effectを実行
                    EffectController.Instance.SpawnParticle(18, "", 1f * PentoGameController.Instance.BS, other.transform.position);
                }
            }
        }
    }
}
