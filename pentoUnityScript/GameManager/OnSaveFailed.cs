using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// セーブの後、画面転換 (Button Component)
/// </summary>
public class OnSaveFailed : MonoBehaviour {

    /// <summary>
    /// セーブの後、画面転換
    /// </summary>
	public void OnClick()
	{
        //  ゲームの各Menu画面に戻る
        switch (SceneManager.GetActiveScene().buildIndex)
		{
			case 6:
                //  Sceneの名前を使ってScene Change
                GameController.Instance.SN = "StoryScrollView";
				break;
			case 7:
                //  Sceneの名前を使ってScene Change
                GameController.Instance.SN = "AllCollectionScrollView";
				break;
			case 8:
                //  Sceneの名前を使ってScene Change
                GameController.Instance.SN = "LevelScrollView";
				break;
			case 9:
                //  Sceneの名前を使ってScene Change
                GameController.Instance.SN = "MainMenu";
				break;
			case 10:
                //  Sceneの名前を使ってScene Change
                GameController.Instance.SN = "FreeModeMenu";
				break;
		}
    }

}