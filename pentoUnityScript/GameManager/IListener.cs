using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-------------------------------------------
/// <summary>
/// イベントのタイプ
/// </summary>
public enum EVENT_TYPE
{
    SERIAL_NUM_CHECK,
    BOARD_CONNECTING_CHECK,
    SERVER_CONNECTING_CHECK,
    IS_PLAYING,
    SERVER_CONNECTED,
    BOARD_CONNECTED,
    SCENE_CHANGE,
    ENTER_LEVEL,
    ENTER_STORY,
    ENTER_COLLECTION,
    ENTER_PUZZLE,
    PUZZLE_COMPLETE,
    WRONG_BLOCK,
    BLOCK_CHANGED,
    SAVE_RECORD,
    SAVE_PENTO,
    CREATE_COMPLETE,
    CHARACTER_ARRIVED,
    STEP_COMPLETE,
    UI_STEP_COMPLETE,
    CAMERA_SETTING_COMPLETE,
    BOARD_RESPONSE,
    COLOR_SELECT,
    PAINT_COLOR,
    DIMENSION_CHANGE,
    SERVER_RESPONSE,
    SERVER_ERROR,
    TUTORIAL_STEP_COMPLETE,
    IMAGE_ARRIVED,
    SPECIAL_COORDINATE_RESPONSE,
    SERIAL_NUM_ARRIVED,
    LOGIN,
    SAVE_FAILED
}

//-------------------------------------------
/// <summary>
/// Listenerのインタペース
/// </summary>
public interface IListener
{
    //-------------------------------------------------
    /// <summary>
    /// イベントが発生する時、実行
    /// </summary>
    /// <param name="Event_Type">イベントのタイプ</param>
    /// <param name="Sender">受けるcomponent</param>
    /// <param name="Param">first parameter</param>
    /// <param name="Param2">second parameter</param>
    void OnEvent (EVENT_TYPE Event_Type, Component Sender, object Param = null, object Param2 = null);
}