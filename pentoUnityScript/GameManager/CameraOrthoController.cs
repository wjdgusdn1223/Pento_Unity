using UnityEngine;
using System.Collections;

/// <summary>
/// カメラのモードを変換
/// </summary>
public class CameraOrthoController : MonoBehaviour
{
    private Matrix4x4 ortho;
    
    private Matrix4x4 perspective;

    public float near = 0.001f;

    public float far = 1000f;

    private float aspect;

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static CameraOrthoController Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static CameraOrthoController instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake()
    {
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

    private void Start()
    {
        perspective = Camera.main.projectionMatrix;
    }

    //-------------------------------------
    /// <summary>
    /// カメラのモードを変える。
    /// </summary>
    public void StartMatrixBlender(float OrthoSize)
    {
        if (null != (Camera.main.transform.Find("Effect Camera")))
        {
            Camera effCamera = Camera.main.transform.Find("Effect Camera").gameObject.GetComponent<Camera>();

            if (OrthoSize != 0f)
            {
                effCamera.orthographic = true;
                effCamera.orthographicSize = OrthoSize;
            }
            else
            {
                effCamera.orthographic = false;
            }
        }

        aspect = (Screen.width + 0.0f) / (Screen.height + 0.0f);

        if (OrthoSize != 0f)
        {
            float vertical = OrthoSize;
            float horizontal = (vertical * 16f) / 9f;

            ortho = Matrix4x4.Ortho(-horizontal, horizontal, -vertical, vertical, near, far);

            BlendToMatrix(ortho, 1f);
        }
        else
        {
            BlendToMatrix(perspective, 1f);
        }
    }

    //---------------------------------------
    /// <summary>
    /// Matrix4x4をfromからtoまでかえる
    /// </summary>
    /// <param name="from">from</param>
    /// <param name="to">to</param>
    /// <param name="time">time</param>
    /// <returns></returns>
    private Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float time)
    {
        Matrix4x4 ret = new Matrix4x4();
        int i;
        for (i = 0; i < 16; i++)
            ret[i] = Mathf.Lerp(from[i], to[i], time);
        return ret;
    }

    //----------------------------------------
    /// <summary>
    /// メインカメラのMatrix4x4をsrcからdestまでかえる
    /// </summary>
    /// <param name="src">src</param>
    /// <param name="dest">dest</param>
    /// <param name="duration">duration</param>
    /// <returns></returns>
    IEnumerator LerpFromTo(Matrix4x4 src, Matrix4x4 dest, float duration)
    {
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            Camera.main.projectionMatrix = MatrixLerp(src, dest, (Time.time - startTime) / duration);
            yield return new WaitForSeconds(0f);
        }
        Camera.main.projectionMatrix = dest;
    }

    //-------------------------------------------------
    /// <summary>
    /// カメラのモードをtargetMatrixに変える。
    /// </summary>
    /// <param name="targetMatrix">目標のモード</param>
    /// <param name="duration">delay time</param>
    /// <returns></returns>
    private Coroutine BlendToMatrix(Matrix4x4 targetMatrix, float duration)
    {
        StopAllCoroutines();
        return StartCoroutine(LerpFromTo(Camera.main.projectionMatrix, targetMatrix, duration));
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