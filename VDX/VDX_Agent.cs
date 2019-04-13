using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VDX_Agent : MonoBehaviour
{
    #region Constructor Inspector

    [SerializeField, Tooltip("視野の半径(m)")]
    //視界の範囲
    public float _viewRadius = 10000;

    [SerializeField, Tooltip("視野角(度)")]
    //視界の視野角
    public float _viewAngle = 170;

    [SerializeField, Tooltip("シード値")]
    //乱数のシード値
    public int _randomSeed = 10000;

    [SerializeField, Tooltip("視野角の分割数")]
    //視野角の分割数
    public int _bin = 49;

    [SerializeField, Tooltip("エージェントの速度(m/秒)")]
    //エージェントの移動スピード
    public float _agentSpeed = 1.0f;

    [SerializeField, Tooltip("視線射出実行間隔(秒)")]
    //隔秒数ごとに実行する時間間隔
    public float _timeOut = 3.0f;

    [SerializeField, Tooltip("リセットルールの適用距離(m)")]
    //エージェントと壁との距離の閾値
    public float _stopThre = 0.5f;

    [SerializeField, Tooltip("視線をpow乗(default:2)")]
    //エージェントと壁との距離の閾値
    public float _lenPow = 2;

    [SerializeField, Tooltip("ゲーム内速度の変更")]
    //エージェントと壁との距離の閾値
    public float _timeScale = 1;
    #endregion

    #region InspectorOnly
    public bool showLine;
    #endregion

    #region
    private float _timeElapsed;
    #endregion

    /// コンストラクタ
    #region constructor
    VDX_Alg alg = new VDX_Alg();
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        alg.Agent = this.gameObject;
        alg.ViewRadius = _viewRadius;
        alg.ViewAngle = _viewAngle;
        alg.RandomSeed = _randomSeed;
        alg.Bin = _bin;
        alg.AgentSpeed = _agentSpeed;
        alg.TimeOut = _timeOut;
        alg.StopThre = _stopThre;
        alg.LenPow = _lenPow;
        alg.TimeScale = _timeScale;

        #region AddComponets
        TrailRenderer trail = gameObject.AddComponent<TrailRenderer>();
        trail.time = Mathf.Infinity;
        NavMeshAgent navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        #endregion
    }

    void Update()
    {

        Time.timeScale = _timeScale;

        System.Random r = new System.Random(_randomSeed);
        _randomSeed++;

        double rdm = r.NextDouble();

        alg.WalkAlong(Time.deltaTime);
        _timeElapsed += Time.deltaTime;

        ////線を表示
        //if (showLine)
        //{
        //    for (int i = 0; i < lnviewPoints.Count; i++)
        //    {
        //        Debug.DrawLine(agent.transform.position, lnviewPoints[i], Color.red);
        //    }
        //}

        ////initailSettings
        //if (Time.time == 0) {
        //    Debug.Log("Initialize");
        //    double init_rdm = r.NextDouble();
        //    bDir360();
        //    bRotateObject(_vagentDir);
        //}

        //衝突しそうなときの判定
        if (alg.RayLength() < _stopThre)
        {
            _timeElapsed = 0;
            if (alg.SideRay(transform.right, rdm) == true)
            {
                Debug.Log("////////////////////");
                Debug.Log("Side step");
                alg.RotateObject(transform.right);
            }
            else
            {
                //360度方向へ視線を飛ばす
                Debug.Log("////////////////////");
                Debug.Log("360 degree Reset");
                //_dirに視線をセット
                //bNavTrmIso();
                alg.RotateObject(alg.Dir360());
            }
        }

        //timeOutごとの処理
        if (_timeElapsed > _timeOut)
        {
            Debug.Log("////////////////////");
            Debug.Log("Normal");
            alg.NavCutDir();

            //視線リストから確率で選択
            alg.RotateObject(alg.SelectDir(rdm));
            _timeElapsed = 0;
        }
    }
}
