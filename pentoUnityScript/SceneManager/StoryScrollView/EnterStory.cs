using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Storyゲームに入場
/// </summary>
public class EnterStory : MonoBehaviour {

    //---------------------------------------------
	public void OnClick()
    {
        //  Storyの番号をObjectの名前から分離
        int StoryNum = Convert.ToInt32(gameObject.name.Split('_')[1]);

        //  Story番号をセーブ
        GameData.StoryNum = StoryNum;

        //  ENTER_STORY　イベントをListenerに転送
        StoryScrollViewController.Instance.ES = StoryNum;
    }
}
