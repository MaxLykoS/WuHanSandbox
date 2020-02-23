using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMInHospitalState : FSMBaseState
{
    public FSMInHospitalState(FSMSystem fsmSystem) : base(fsmSystem, FSMStateID.InHospitalFSMStateID) { }
    float curCuredTime;

    public override void StateFixUpdate()
    {
        curCuredTime += Time.fixedDeltaTime;
    }

    public override void StateStart()
    {
        mCitizen.CitizenType = CitizenType.InHospital;
        curCuredTime = 0;
        mCitizen.name = "InHospital";
        CitizenMgr.CurBedsCnt--;
        CitizenMgr.InHospitalCnt++;
        mCitizen.bMovingToHosp = false;
    }

    public override void StateEnd()
    {
        mCitizen.transform.position = mCitizen.Hospital.GenerateSpawnPoint();
        mCitizen.SetActive(true);
        mCitizen.ClearAllStatus(false);
        CitizenMgr.CurBedsCnt++;
        CitizenMgr.InHospitalCnt--;
    }

    public override void TransitionReason()
    {
        if (curCuredTime > mCitizen.CureTimeTotal)
        {
            if (CheckHealingSuccess())
            {
                mFSMSystem.TransitionFSMState(FSMTransition.Healed);
            }
            else
            {
                mFSMSystem.TransitionFSMState(FSMTransition.HealingFailed);
            }
        }
        else if (!mCitizen.CheckAlive())
        {
            mFSMSystem.TransitionFSMState(FSMTransition.ToLate);
        }
    }

    bool CheckHealingSuccess()
    {
        return Random.Range(0.0f, 1.0f) > CitizenMgr.DeathRate;
    }
}
