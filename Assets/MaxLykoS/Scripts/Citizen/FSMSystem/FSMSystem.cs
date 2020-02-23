using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 状态ID
/// </summary>
public enum FSMStateID
{
    NullFSMStateID,
    HealthyFSMStateID, //健康状态
    LatentFSMStateID,  //潜伏状态
    IllFSMStateID,     //发病状态
    WaitToHospFSMStateID, //排队状态
    ToHospitalFSMStateID, //就诊状态
    InHospitalFSMStateID, //住院状态
    CuredFSMStateID, //治愈状态
    DeadFSMStateID   //死亡状态
}


/// <summary>
/// 状态转化条件
/// </summary>
public enum FSMTransition
{
    Infected,  //被感染
    AfterLatent,  //潜伏期之后
    TookByAmbulance,  //上了救护车（直接进入前往医院或等待医院状态）
    CantTakeAmbulance,  //救护车满了，进不去，直接进入等待医院状态
    ToLate,      //拖得时间太久
    WantHealing,  //希望就医
    BedsAvailable,  //预约床位
    ReachHospital,  //到达医院
    Healed,  //治愈成功
    HealingFailed  //治愈失败
}

public class FSMSystem
{
    private FSMStateID mCurrentStateID;
    private FSMBaseState mCurrentState;

    private Dictionary<FSMStateID, FSMBaseState> mFSMStateDic = new Dictionary<FSMStateID, FSMBaseState>();


    public void AddFSMSate(FSMBaseState state)
    {
        if (state == null)
        {
            Debug.Log("角色状态为空，无法添加");
            return;
        }
        if (mCurrentState == null)
        {
            //第一个添加的状态被作为系统首个运行的状态
            mCurrentStateID = state.mStateID;
            mCurrentState = state;
            mCurrentState.StateStart();
        }
        if (mFSMStateDic.ContainsValue(state))
        {
            //Debug.Log("容器内存在该状态");
            return;
        }
        mFSMStateDic.Add(state.mStateID, state);
    }

    public void DeleteFSMSate(FSMBaseState state)
    {
        if (state == null)
        {
            Debug.Log("角色状态为空，无法添加");
            return;
        }
        if (!mFSMStateDic.ContainsValue(state))
        {
            Debug.Log("容器内不存在该状态");
            return;
        }
        mFSMStateDic.Remove(state.mStateID);
    }

    //更新（执行）系统
    public void UpdateSystem()
    {
        if (mCurrentState != null)
        {
            mCurrentState.StateUpdate();
            mCurrentState.TransitionReason();
        }
    }

    //固定更新（执行系统）
    public void FixUpdateSystem()
    {
        if (mCurrentState != null)
        {
            mCurrentState.StateFixUpdate();
            mCurrentState.TransitionReason();
        }
    }

    //转换状态
    public void TransitionFSMState(FSMTransition transition)
    {
        FSMStateID stateID = mCurrentState.GetStateIdByTransition(transition);
        if (stateID != FSMStateID.NullFSMStateID)
        {
            mCurrentStateID = stateID;
            mCurrentState.StateEnd();
            //换状态
            mCurrentState = mFSMStateDic.FirstOrDefault(q => q.Key == stateID).Value;
            mCurrentState.StateStart();
        }
    }
}