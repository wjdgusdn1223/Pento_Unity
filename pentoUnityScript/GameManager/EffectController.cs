using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// Effectを制御する
/// </summary>
public class EffectController : MonoBehaviour {

    //-------------------------------------------------------------
    //  Effect Array
    private GameObject[] ParticleExamples;

    //  画面にあるEffect List
    private List<GameObject> onScreenParticles = new List<GameObject>();

    //-----------------------------------------------------
    /// <summary>
    /// singleton instance
    /// read only
    /// </summary>
    public static EffectController Instance
    {
        get
        {
            return instance;
        }
        set { }
    }

    //-----------------------------------------------------
    private static EffectController instance = null;
    //---------------------------------------------------
    // Use this for initialization
    void Awake()
    {
#if SHOW_DEBUG_MESSAGES
        Debug.Log("EffectControllerを実行");
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

        //  Effectをarrayにセーブする
        List<GameObject> particleExampleList = new List<GameObject>();
        int nbChild = this.transform.childCount;
        for (int i = 0; i < nbChild; i++)
        {
            GameObject child = this.transform.GetChild(i).gameObject;
            particleExampleList.Add(child);
        }
        particleExampleList.Sort(delegate (GameObject o1, GameObject o2) { return o1.name.CompareTo(o2.name); });
        ParticleExamples = particleExampleList.ToArray();

        StartCoroutine("CheckForDeletedParticles");
    }

    //---------------------------------------------------
    // Use this for initialization
    private void Start()
    {

    }

    //---------------------------------------------------
    /// <summary>
    /// puzzle complete Effectを実行
    /// </summary>
    /// <param name="effectNum">Effectの番号</param>
    /// <param name="target">変更するターゲットの名前</param>
    /// <param name="scale">Effectの大きさ</param>
    /// <param name="point">Effectを実行する座標</param>
    /// <param name="puzzleSize">Puzzleの大きさ</param>
    /// <param name="blockSize">Blockの大きさ</param>
    public IEnumerator CompleteEffect(int effectNum, string target, float scale, Vector3 point, int puzzleSize, float blockSize)
    {
        //  delay time 変数を設定
        float time = 0f;

        // random scale
        float rdScale = scale;

        for (int i = 0; i < 20; i++)
        {
            //  Effectを実行する座標を計算
            Vector3 targetPoint = point + Random.insideUnitSphere * (puzzleSize * blockSize / 2f);

            //  大きさを設定
            rdScale = Random.Range(scale * 0.5f, scale * 1f);

            //  Effectを実行
            EffectController.Instance.SpawnParticle(6, "", rdScale, targetPoint);

            // delay time 設定
            time = Random.Range(0.1f,0.4f);

            // delay time
            yield return new WaitForSeconds(time);
        }
    }

    //---------------------------------------------------
    /// <summary>
    /// Effectを実行
    /// </summary>
    /// <param name="effectNum">Effectの番号</param>
    /// <param name="target">変更するターゲットの名前</param>
    /// <param name="scale">Effectの大きさ</param>
    /// <param name="point">Effectを実行する座標</param>
    /// <returns></returns>
    public void SpawnParticle(int effectNum, string target, float scale, Vector3 point)
    {
        GameObject particles = (GameObject)Instantiate(ParticleExamples[effectNum]);

        //  座標を設定
        particles.transform.localPosition = point;
        
        if (target.Equals("")) 
        {
            //particles.gameObject.GetComponent<ParticleSystemRenderer>().maxParticleSize = scale;
            particles.transform.localScale *= scale;
        }
        else
        {
            //particles.transform.Find(target).gameObject.GetComponent<ParticleSystemRenderer>().maxParticleSize = scale;
            particles.transform.Find(target).transform.localScale *= scale;
        }
        
        //  Layer sorting 設定
        particles.GetComponent<ParticleSystemRenderer>().sortingLayerName = "Foreground";

        particles.GetComponent<ParticleSystemRenderer>().sortingOrder = 2;

        //  objectを活性化
        particles.SetActive(true);

        ParticleSystem ps = particles.GetComponent<ParticleSystem>();

        if (ps != null)
        {
            var main = ps.main;
            if (main.loop)
            {
                ps.gameObject.AddComponent<CFX_AutoStopLoopedEffect>();
                ps.gameObject.AddComponent<CFX_AutoDestructShuriken>();
            }
        }

        onScreenParticles.Add(particles);
    }

    /// <summary>
    /// 消滅させるEffectをチェック
    /// </summary>
    IEnumerator CheckForDeletedParticles()
    {
        while (true)
        {
            yield return new WaitForSeconds(5.0f);
            for (int i = onScreenParticles.Count - 1; i >= 0; i--)
            {
                if (onScreenParticles[i] == null)
                {
                    onScreenParticles.RemoveAt(i);
                }
            }
        }
    }

    /// <summary>
    /// Effectを消滅させる
    /// </summary>
    public void DestroyParticles()
    {
        for (int i = onScreenParticles.Count - 1; i >= 0; i--)
        {
            if (onScreenParticles[i] != null)
            {
                GameObject.Destroy(onScreenParticles[i]);
            }

            onScreenParticles.RemoveAt(i);
        }
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
