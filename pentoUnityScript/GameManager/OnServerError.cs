using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Server Error イベント (Button Component）
/// </summary>
public class OnServerError : MonoBehaviour {

    /// <summary>
    /// Server Error イベント
    /// </summary>
	public void OnClick()
	{
		if (SceneManager.GetActiveScene().buildIndex == 0)
		{
			//  ゲーム終了のイベントを転送
        	GameController.Instance.IP = -1;
		}
		else
		{
            //  Sceneの名前を使ってScene Change
            GameController.Instance.SN = "MainMenu";
		}
    }

}
