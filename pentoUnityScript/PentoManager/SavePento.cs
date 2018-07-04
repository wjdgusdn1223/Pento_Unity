using UnityEngine;

/// <summary>
/// Puzzle Save
/// </summary>
public class SavePento : MonoBehaviour
{
    //------------------------------
    public void OnClick()
    {
        //  パズルをセーブ
        EventManager.Instance.PostNotification(EVENT_TYPE.SAVE_PENTO, this);
    }
}