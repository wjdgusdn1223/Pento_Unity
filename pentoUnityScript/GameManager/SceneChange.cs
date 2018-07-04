using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Scene Change イベント (Button Component）
/// </summary>
public class SceneChange : MonoBehaviour {

    //----------------------------------------
    /// <summary>
    /// Objectの名前を使ってScene Change
    /// </summary>
    public void OnClick()
    {
#if SHOW_DEBUG_MESSAGES
            Debug.Log("Scene Change");
#endif

        //  Sceneの名前を使ってScene Change
        GameController.Instance.SN = gameObject.name.Split('_')[0];
    }

}
