using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ボードとコネクトする。
/// </summary>
public class BoardConnect : MonoBehaviour {

    /// <summary>
    /// ボードとコネクトする
    /// </summary>
	public void OnClick()
    {
        //  警告のウィンドウを非活性化する。
        transform.parent.gameObject.SetActive(false);

        //  連結の後、確認する。
        if (BoardRequester.Instance.EnterTheArduino())
        {
            if (BoardRequester.Instance.IsConnect())
            {
                //  コネクト完了のイベントを転送する。
                GameController.Instance.IBC = 1;

                return;
            }
        }

        //  コネクト失敗のイベントを転送する。
        GameController.Instance.IBC = -1;
    }

}
