using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ゲームのSoundを制御
/// </summary>
public class SoundController : MonoBehaviour {

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static SoundController Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static SoundController instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("Sound Controllerを実行");
#endif

        //  sceneにinstanceが存在するかを検査
        //  存在する場合,消滅させる。
        if (instance)
        {
            DestroyImmediate(gameObject);
            return;
        }

        //  このinstanceを唯一のobjectにする。
        instance = this;
    }

    //---------------------------------------------------
    // Use this for initialization
    private void Start()
    {

    }

    //-------------------------------------------------
    /// <summary>
    /// 選択したEffect Soundを出力
    /// </summary>
    /// <param name="soundName">Effect Soundの名前</param>
    public void PlayEffectSound(string soundName)
    {
        //  Sound play
        GameObject.Find(soundName).GetComponent<AudioSource>().Play();
    }

    //-------------------------------------------------
    /// <summary>
    /// 選択したSoundを出力
    /// </summary>
    /// <param name="soundName">Soundの名前</param>
    public void PlaySound(string soundName)
    {
        if (GameData.soundName.Equals(soundName))
        {
            return;
        }

        if (!GameData.soundName.Equals(string.Empty))
            StopSound();

        GameData.soundName = soundName;

        //  ボリュームを０に設定
        GameObject.Find(soundName).GetComponent<AudioSource>().volume = 0;

        //  Sound play
        GameObject.Find(soundName).GetComponent<AudioSource>().Play();

        //  Sound Fade Effectを実行
        StartCoroutine(SoundFadeEffect(soundName, 1));
    }

    //-------------------------------------------------
    /// <summary>
    /// 選択したSoundを一時停止
    /// </summary>
    public void PauseSound()
    {
        //  ボリュームを１に設定
        GameObject.Find(GameData.soundName).GetComponent<AudioSource>().volume = 1;

        //  Sound Fade Effectを実行
        StartCoroutine(SoundFadeEffect(GameData.soundName, 0, "Pause"));
    }

    //-------------------------------------------------
    /// <summary>
    /// 選択したSoundを停止
    /// </summary>
    public void StopSound()
    {
        //  ボリュームを１に設定
        GameObject.Find(GameData.soundName).GetComponent<AudioSource>().volume = 1;

        //  Sound Fade Effectを実行
        StartCoroutine(SoundFadeEffect(GameData.soundName, 0));
    }

    //-------------------------------------------------
    /// <summary>
    /// Sound Fade Effect
    /// </summary>
    /// <param name="soundName">Soundの名前</param>
    /// <param name="volume">目標にするボリューム</param>
    /// <param name="type">行動タイプ</param>
    IEnumerator SoundFadeEffect(string soundName, int volume, string type = "null")
    {
        while (GameObject.Find(soundName).GetComponent<AudioSource>().volume != volume)
        {
            //  Sound Fade Effectを適用
            if (volume == 1)
                GameObject.Find(soundName).GetComponent<AudioSource>().volume += 0.1f;
            else
                GameObject.Find(soundName).GetComponent<AudioSource>().volume -= 0.1f;

            //  delay time
            yield return new WaitForSeconds(0.1f);
        }

        //  fade outの時、Soundを停止
        if (volume != 1 && type.Equals("null"))
            GameObject.Find(soundName).GetComponent<AudioSource>().Stop();
        else if (type.Equals("Pause"))
            GameObject.Find(soundName).GetComponent<AudioSource>().Pause();
    }

    //-------------------------------------------------
    /// <summary>
    /// イベントが発生する時、実行
    /// </summary>
    /// <param name="Event_Type">イベントのタイプ</param>
    /// <param name="Sender">受けるcomponent</param>
    /// <param name="Param">first parameter</param>
    /// <param name="Param2">second parameter</param>
    public void OnEvent(EVENT_TYPE Event_Type, Component Sender, object Param = null, object Param2 = null)
    {
        switch (Event_Type)
        {

        }
    }
}
