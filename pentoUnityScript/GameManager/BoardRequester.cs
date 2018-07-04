using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;
using UnityEngine;

/// <summary>
/// ボードに情報や機能の実行を要請
/// </summary>
public class BoardRequester : MonoBehaviour, IListener
{
    //  ボードとコネクト中かどうか
    private bool isEntering = false;

    //  座標データを受信中かどうか
    private bool isReading = false;

    //  Arduino Object //4, 6
    private SerialPort intoARD = new SerialPort("COM6", 9600, Parity.None, 8, StopBits.One);
    
    //  ボードから受信した座標のデータ

    private int[,] readXY = new int[15, 15];

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static BoardRequester Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static BoardRequester instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("보드 리퀘스터 실행");
#endif

        //  sceneにinstanceが存在するかを検査
        //  存在する場合消滅させる。
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
        //  WRONG_BLOCK イベントを受信するためにlistner登録
        EventManager.Instance.AddListener(EVENT_TYPE.WRONG_BLOCK, this);
        //  PUZZLE_COMPLETE イベントを受信するためにlistner登録
        EventManager.Instance.AddListener(EVENT_TYPE.PUZZLE_COMPLETE, this);
        //  CREATE_COMPLETE イベントを受信するためにlistner登録
        EventManager.Instance.AddListener(EVENT_TYPE.CREATE_COMPLETE, this);
    }

    //----------------------------------------------
    /// <summary>
    /// arduinoにコネクトする
    /// </summary>
    /// <returns></returns>
    public bool EnterTheArduino()
    {
        if (isEntering == false)
        {
            isEntering = true;
            try 
            {
                intoARD.Open();

                Debug.Log("------------ボードコネクト完了----------");

                isEntering = false;

                return true;
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());

                Debug.Log("------------ボードコネクト失敗----------");

                isEntering = false;

                return false;
            }
        }
        else
        {
            return false;
        }
        
    }

    //--------------------------------------------------------
    /// <summary>
    /// arduinoとのコネクトを切る
    /// </summary>
    public void OutTheArduino()
    {      
        if (IsConnect()) //현재 연결 상태를 확인 후
        {
            //아두이노 클로즈
            intoARD.Close();

            Debug.Log("------------コネクトを切る----------");
        }
    }

    //----------------------------------------------
    /// <summary>
    /// arduinoとのコネクトを確認
    /// </summary>
    /// <returns></returns>
    public Boolean IsConnect()
    {
        return intoARD.IsOpen;
    }

    //--------------------------------------------------
    /// <summary>
    /// ボードのシリアル番号を受信する
    /// </summary>
    public IEnumerator GetSerial_Num()
    {
        if (IsConnect())
        {
            intoARD.Write("N");
            yield return new WaitForSeconds(3.5f);

            byte[] rcvBuff = new byte[1024];
            int Len = intoARD.Read(rcvBuff, 0, 1024);
            String sTemp = Encoding.Default.GetString(rcvBuff, 0, Len);

            //  SERIAL_NUM_ARRIVED イベントを受信するためにlistner登録
            EventManager.Instance.PostNotification(EVENT_TYPE.SERIAL_NUM_ARRIVED, this, sTemp);
        }
        else
        {
            //  SERIAL_NUM_ARRIVED イベントを受信するためにlistner登録
            EventManager.Instance.PostNotification(EVENT_TYPE.SERIAL_NUM_ARRIVED, this,"false");
        }
    }

    //------------------------------------------------
    /// <summary>
    /// WRONG_BLOCKイベントをボードに転送
    /// </summary>
    private void failed()
    {
        if (IsConnect())
        {
            intoARD.Write("F");
        }
        else
        {
            GameController.Instance.IBC = -1;
        }
    }

    //---------------------------------------------
    /// <summary>
    /// PUZZLE_COMPLETE、CREATE_COMPLETEイベントをボードに転送
    /// </summary>
    private void success()
    {
        if (IsConnect())
        {
            intoARD.Write("S");
        }
        else
        {
            GameController.Instance.IBC = -1;
        }
    }

    /// <summary>
    /// 座標のデータをarduinoに転送
    /// </summary>
    /// <param name="arrLED">座標のデータのarray</param>
    public void WriteLED(int[,] arrLED)
    {
        String str = "L";

        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                str += arrLED[i, j];
            }
        }

        intoARD.Write(str);
    }

    //-----------------------------------------------
    /// <summary>
    /// 'ReadCensor'Threadを実行
    /// </summary>
    /// <returns>ボードの座標のデータ</returns>
    public void StartRead()
    {
        //  コネクト状態をチェックした後、座標のデータを受信する
        if (IsConnect())
        {
            StartCoroutine(ReadCensor());
        }
        else
        {
            GameController.Instance.IBC = -1;
        }
    }

    //-----------------------------------------------
    /// <summary>
    /// センサーのデータを受信する
    /// </summary>
    /// <returns>ボードの座標のデータ</returns>
    IEnumerator ReadCensor()
    {
        isReading = true;

        intoARD.Write("C");

        yield return new WaitForSeconds(1f);

        byte[] rcvBuff = new byte[1024];
        int Len = intoARD.Read(rcvBuff, 0, 1024);
        String sTemp = Encoding.Default.GetString(rcvBuff, 0, Len);

        char[] arrParse = sTemp.ToCharArray();

        //  ボードからもらった座標データをarrayに変換する
        int j = 0;
        for (int i = 0; i < 225; i++)
        {
            if (i != 0 && i % 15 == 0)
            {
                ++j;
            }
            readXY[j, i % 15] = (int)arrParse[i] == 48 ? 0 : 1;
        }

        isReading = false;

        yield return null;

        EventManager.Instance.PostNotification(EVENT_TYPE.BOARD_RESPONSE, this, readXY);
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
            case EVENT_TYPE.WRONG_BLOCK:
                failed();
                break;
            case EVENT_TYPE.PUZZLE_COMPLETE:
                success();
                break;
            case EVENT_TYPE.CREATE_COMPLETE:
                success();
                break;
        }
    }
}
