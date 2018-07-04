using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// パズルゲームに入場
/// </summary>
public class EnterPuzzle : MonoBehaviour
{

    //-------------------------------------------
	public void OnClick()
    {
        //  パズルの番号をObjectの名前から分離
        int PuzzleNum = Convert.ToInt32(gameObject.name.Split('_')[1]);

        //  パズルの番号をセーブ
        GameData.PuzzleNum = PuzzleNum;

        //  ENTER_LEVEL　イベントをListenerに転送
        LevelScrollViewController.Instance.EP = PuzzleNum;
    }
}
