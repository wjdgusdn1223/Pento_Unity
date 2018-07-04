using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// サーバーにコネクトする (Button Component)
/// </summary>
public class ServerConnect : MonoBehaviour {

    /// <summary>
    /// サーバーにコネクトする
    /// </summary>
	public void OnClick()
    {
        //  警告のウィンドウを活性化
        transform.parent.gameObject.SetActive(false);

        //  Server Connectのイベントを転送
        GameController.Instance.ISC = 1;
    }

}
