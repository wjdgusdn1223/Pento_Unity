using UnityEngine;
using System.Collections;

//------------------------------------------------------
/// <summary>
/// ゲーム終了
/// </summary>
public class GameEnd : MonoBehaviour
{

    // Use this for initialization
    public void OnClick()
    {
        //  ゲーム終了のイベントを転送
        GameController.Instance.IP = -1;
    }

}
