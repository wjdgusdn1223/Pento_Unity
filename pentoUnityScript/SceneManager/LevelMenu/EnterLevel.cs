using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-----------------------------------------------
/// <summary>
/// 選択したLevelに入場 
/// </summary>
public class EnterLevel : MonoBehaviour {

    //-------------------------------------------
	public void OnClick()
    {
        //  レベルの番号をObjectの名前から分離
        int LevelNum = Convert.ToInt32(gameObject.name.Split('_')[1]);

        //  レベル番号をセーブ
        GameData.LevelNum = LevelNum;

        //  UserLevelと選択したレベルをチェック
        LevelMenuController.Instance.EL = LevelNum;
    }

}
