using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// ゲームに入場 
/// </summary>
public class EnterCollection : MonoBehaviour
{

    //--------------------------------------------
    public void OnClick()
    {
        //  名前でパズルの番号を分離
        int CollectionNum = Convert.ToInt32(gameObject.name.Split('_')[1]);

        //  パズルの番号をセーブ
        GameData.CollectionNum = CollectionNum;

        //　ENTER_COLLECTION イベントをListenerに転送
        AllCollectionScrollViewController.Instance.EC = CollectionNum;
    }
    
}
