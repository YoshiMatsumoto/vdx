using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.AI;
using System;

public partial class VDX_Alg : MonoBehaviour
{


    /// -------------------------------------------
    /// 2018/10/03 Matsumoto
    /// 与えられたベクトルの方向に正面を向く
    /// 
    /// 2019/04/12 Matsumoto
    /// カプセル化に伴い引数の変数名を変更 
    /// -------------------------------------------
    public virtual void RotateObject()
    {
        Vector3 newDir = Vector3.RotateTowards(Agent.transform.forward, _agentDir, ViewRadius, ViewAngle);
        newDir = new Vector3(newDir.x, 0, newDir.z);
        Agent.transform.rotation = Quaternion.LookRotation(newDir);
    }

    /// -------------------------------------------
    /// 2019/04/12 Matsumoto
    /// カプセル化に伴い引数の変数名を変更
    /// オーバーロード可能に 
    /// -------------------------------------------
    public virtual void RotateObject(Vector3 vec)
    {
        Vector3 newDir = Vector3.RotateTowards(Agent.transform.forward, vec, ViewRadius, ViewAngle);
        newDir = new Vector3(newDir.x, 0, newDir.z);
        Agent.transform.rotation = Quaternion.LookRotation(newDir);
    }

    /// -------------------------------------------
    /// 2018/10/03 Matsumoto
    /// エージェントの正面方向へ進む
    /// -------------------------------------------
    public virtual bool WalkAlong(float time)
    {
        Vector3 velocity = Agent.transform.rotation * new Vector3(0, 0, AgentSpeed);
        Agent.transform.position += velocity * time;

        return true;
    }

    /// -------------------------------------------
    /// 2018/10/03 Matsumoto
    /// 設定された視野角に、分割された複数の視線を返す関数
    /// 
    /// 2019/03/05 Matsumoto
    /// ViewCastを消し、内部で処理
    /// -------------------------------------------
    public virtual void NavCutDir()
    {
        _lnviewPoints.Clear();
        float stepAngleSize = ViewAngle / Bin;
        for (int i = 0; i < Bin; i++)
        {
            //各binの相対角度を算出
            float angle = Agent.transform.eulerAngles.y - ViewAngle / 2 + stepAngleSize / 2 + stepAngleSize * i;
            var angles = new Vector3(0, angle, 0);
            var direction = Quaternion.Euler(angles) * Vector3.forward;

            NavMesh.Raycast(Agent.transform.position, direction * ViewAngle, out _hit, NavMesh.AllAreas);
            _lnviewPoints.Add(_hit.position);
        }
    }


    /// -------------------------------------------
    /// 2018/10/03 Matsumoto
    /// 複数の視線から視線/Σ視線の確率で選択し、方向を返す
    /// 
    /// 2019/03/07 視線を任意の指数で指定可能に
    /// -------------------------------------------
    public virtual Vector3 SelectDir(double rdm_index)
    {
        //初期化
        //分割された視線の長さを格納
        var viewPointsLength = new List<double>();
        //分割された視線のベクトルを格納
        _viewPointsVector.Clear();


        //リストに追加
        for (int i = 0; i < _lnviewPoints.Count; i++)
        {
            viewPointsLength.Add(Vector3.Distance(_lnviewPoints[i], Agent.transform.position));
            _viewPointsVector.Add(_lnviewPoints[i] - Agent.transform.position);
        }

        // 視線の二乗
        var viewPointsSquare = viewPointsLength.Select(x => Math.Pow(x,LenPow));

        // 視線の二乗の総和
        var viewPointsSUM = viewPointsLength.Sum();

        // （視線の二乗）/（視線の二乗の総和）
        var viewPointsSelector = viewPointsLength.Select(x => x / viewPointsSUM).ToList();

        // 最大値インデックスの初期化
        var index = 0;

        // 確率スタックの初期化
        double tmp = viewPointsSelector[0];
        for (int i = 0; i < _lnviewPoints.Count - 1; i++)
        {
            // 累積確率から該当インデックスを取得
            //リストの頭から足していき、該当するインデックスの値範囲内に入ったら、indexとして返す
            if (rdm_index > tmp && rdm_index <= tmp + viewPointsSelector[i + 1])
            {
                index = i + 1;
                break;
            }
            else
            {
                tmp = tmp + viewPointsSelector[i + 1];
            }
        }
        Vector3 vec = _viewPointsVector[index];

        return vec;
    }

    /// -------------------------------------------
    /// 2018/10/03 Matsumoto
    /// 360度方向へ視線を飛ばす
    /// 
    /// 2019/03/05 Matsumoto
    /// ViewCastを消し、内部で処理
    /// -------------------------------------------
    public virtual Vector3 Dir360()
    {
        //初期化
        _lnviewPoints.Clear();
        _viewPointsVector.Clear();

        float stepAngleSize = ViewAngle / Bin;
        double stepCount = Math.Ceiling(360 / stepAngleSize);
        for (int i = 0; i < stepCount; i++)
        {
            float angle = transform.eulerAngles.y + stepAngleSize * i;
            var angles = new Vector3(0, angle, 0);
            var direction = Quaternion.Euler(angles) * Vector3.forward;

            NavMesh.Raycast(Agent.transform.position, direction * ViewRadius, out _hit, NavMesh.AllAreas);
            _lnviewPoints.Add(_hit.position);

        }

        for (int i = 0; i < _lnviewPoints.Count; i++)
        {
            _viewPointsVector.Add(_lnviewPoints[i] - Agent.transform.position);
        }

        var ind = UnityEngine.Random.Range(0, _viewPointsVector.Count);
        Vector3 vec = _viewPointsVector[ind];

        return vec;
    }

    /// -------------------------------------------
    /// 2018/10/03 Matsumoto
    /// エージェントの横方向へランダムに選択しレイを飛ばし、当たるかどうか
    /// -------------------------------------------
    public virtual bool SideRay(Vector3 vec, double rdm_sideRay)
    {

        if (rdm_sideRay <= 0.5)
        {
            vec = -vec;
        }
        return NavMesh.Raycast(Agent.transform.position, vec, out _hit, NavMesh.AllAreas);
    }

    /// -------------------------------------------
    /// 2018/10/03 Matsumoto
    /// 与えられた方向へサイドステップする
    /// -------------------------------------------
    public virtual void SideStep(Vector3 vec, double rdm_sideRay)
    {
        if (rdm_sideRay <= 0.5)
        {
            vec = -vec;
        }
        Agent.transform.position += vec;
    }

    /// -------------------------------------------
    /// 2019/03/05 Matsumoto
    /// 前方へNavmeshRayを射出し、見通し線の長さを取得する関数
    /// -------------------------------------------
    public virtual double RayLength()
    {
        NavMesh.Raycast(Agent.transform.position, Agent.transform.forward * ViewRadius, out _hit, NavMesh.AllAreas);
        var ln = (_hit.position - Agent.transform.position).magnitude - Agent.transform.localScale.y;
        if (showDir)
        {
            Debug.DrawLine(Agent.transform.position, _hit.position);
        }

        return ln;
    }
}
