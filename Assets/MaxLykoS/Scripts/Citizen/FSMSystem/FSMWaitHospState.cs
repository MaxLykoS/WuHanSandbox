using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMWaitHospState : FSMBaseState
{
    public FSMWaitHospState(FSMSystem fsmSystem) : base(fsmSystem, FSMStateID.WaitToHospFSMStateID) { }
    float jamTimer;

    public override void StateStart()
    {
        mCitizen.AddStatus(CitizenStatusType.WaitingToHosp);
        mCitizen.name = "WaitingToHosp";
        mCitizen.bMovingToHosp = false;
        mCitizen.bWaitingForHosp = true;
        mCitizen.CitizenType = CitizenType.Ill;

        jamTimer = 0;

        CitizenMgr.WaitingCnt++;

        CalculateHospital();
    }

    public override void StateEnd()
    {
        mCitizen.RemoveStatus(CitizenStatusType.WaitingToHosp);
        mCitizen.bWaitingForHosp = false;

        CitizenMgr.WaitingCnt--;
    }

    public override void TransitionReason()
    {
        if (!mCitizen.CheckAlive())
        {
            mFSMSystem.TransitionFSMState(FSMTransition.ToLate);
            CitizenMgr.IllCnt--;
        }
        else if (CitizenMgr.CheckBedsAvailable())
        {
            mCitizen.bMovingToHosp = true;
            mFSMSystem.TransitionFSMState(FSMTransition.BedsAvailable);
        }
    }

    public override void StateFixUpdate()
    {
        CheckDying();
        CheckRecalculatePath();
        CheckJam();
    }

    private void CheckDying()
    {
        if (mCitizen.CheckDying())
            mCitizen.AddStatus(CitizenStatusType.Dying);
    }

    void CheckRecalculatePath()
    {
        if (mCitizen.bRecalculatePathKey)
        {
            mCitizen.bRecalculatePathKey = false;
            CalculateHospital();
        }
    }

    void CalculateHospital()
    {
        if (!mCitizen.MoveToHosp(3.0f))
            Broadcast.Instance.AddString("有患者无法到达医院。", Color.red);
    }

    void CheckJam()
    {
        jamTimer += Time.fixedDeltaTime;
        if (jamTimer >= 2.0f)
        {
            Broadcast.Instance.AddString("医院收治患者出现紧张的情况。", Color.red);
            jamTimer = float.MinValue;
        }
    }
}