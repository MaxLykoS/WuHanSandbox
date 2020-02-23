using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMToHospitalState : FSMBaseState
{
    public FSMToHospitalState(FSMSystem fsmSystem) : base(fsmSystem, FSMStateID.ToHospitalFSMStateID) { }

    public override void StateEnd()
    {
        mCitizen.RemoveStatus(CitizenStatusType.MovingToHosp);
        mCitizen.bMovingToHosp = false;
        mCitizen.SetActive(false);

        CitizenMgr.IllCnt--;
        CitizenMgr.MoveingCnt--;
    }

    public override void StateStart()
    {
        mCitizen.name = "ToHospital";
        mCitizen.bMovingToHosp = true;
        mCitizen.AddStatus(CitizenStatusType.MovingToHosp);
        mCitizen.RemoveStatus(CitizenStatusType.WaitingToHosp);
        mCitizen.CitizenType = CitizenType.Ill;

        CitizenMgr.MoveingCnt++;

        CheckRecalculatePath();
        CalculateHospital();
    }

    public override void TransitionReason()
    {
        if (mCitizen.CheckReachDest()&&mCitizen.CheckReachDestHosp())
        {
            mFSMSystem.TransitionFSMState(FSMTransition.ReachHospital);
            return;
        }
        else if (!mCitizen.CheckAlive())
            mFSMSystem.TransitionFSMState(FSMTransition.ToLate);
    }

    public override void StateFixUpdate()
    {
        CheckDying();
    }

    private void CheckDying()
    {
        if (mCitizen.CheckDying())
            mCitizen.AddStatus(CitizenStatusType.Dying);
    }

    void CheckRecalculatePath()
    {
        if (mCitizen.bRecalculatePathKey||mCitizen.bCheckAmbulanceKey)
        {
            mCitizen.bRecalculatePathKey = false;
            mCitizen.bCheckAmbulanceKey = false;
            CalculateHospital();
        }
    }

    void CalculateHospital()
    {
        if (!mCitizen.MoveToHosp(0.01f))
            Broadcast.Instance.AddString("有患者无法到达医院。", Color.red);
    }
}
