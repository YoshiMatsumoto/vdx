using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Vd alg.
/// </summary>

public partial class VDX_Alg : MonoBehaviour
{
    /// <summary>
    /// inspectorから入力する必要があるフィールドの定義
    /// 単語の先頭を大文字にする
    /// </summary>

    public GameObject Agent { get; set; }
    // 視野限界、Rayの半径
    public float ViewRadius { get; set; }
    // 視野角
    public float ViewAngle { get; set; }
    // Agentの動く速さ
    public float AgentSpeed { get; set; }
    //　乱数のシード値
    public int RandomSeed { get; set; }
    // Bin数
    public int Bin { get; set; }
    // 隔秒数ごとに実行する時間間隔
    public float TimeOut { get; set; }
    // エージェントと壁との距離の閾値
    public float StopThre { get; set; }
    // 視野の係数
    public float LenPow { get; set; }
    // エージェントと壁との距離の閾値
    public float TimeScale { get; set; }
    public bool debug { get; set; }
    public bool showLine { get; set; }
    public bool showDir { get; set; }


    /// <summary>
    /// クラス内で完結させる変数
    /// 変数名の最初にアンダースコアをかき、先頭を小文字にする
    /// </summary>

    //時間のカウント
    private float _timeElapsed;
    //エージェントの選択されたベクトルを格納
    private Vector3 _agentDir;

    private List<Vector3> _lnviewPoints = new List<Vector3>();

    //Navmeshへの衝突判定
    private UnityEngine.AI.NavMeshHit _hit;
    private bool _blocked = false;
    private List<Vector3> _viewPointsVector = new List<Vector3>();
}
