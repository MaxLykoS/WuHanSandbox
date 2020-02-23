using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMIllState : FSMBaseState
{
    public FSMIllState(FSMSystem fsmSystem) : base(fsmSystem, FSMStateID.IllFSMStateID) { }
    float MoveTimerTotal;

    public override void StateStart()
    {
        mCitizen.timeAlive = 0;
        mAnimator.SetBool("Healthy", false);
        MoveTimerTotal = CitizenMgr.GenerateMovePeriod();
        mCitizen.transform.name = "Ill";
        mCitizen.CitizenType = CitizenType.Ill;
        mCitizen.AddStatus(CitizenStatusType.Ill);

        CitizenMgr.IllCnt++;
    }

    public override void StateFixUpdate()
    {
        RandomMove();
        CheckDying();
    }

    public override void TransitionReason()
    {
        if (mCitizen.timeAlive > mCitizen.ToHospTimeTotal)
        {
            mFSMSystem.TransitionFSMState(FSMTransition.WantHealing);
            return;
        }
        else if (!mCitizen.CheckAlive())
        {
            mFSMSystem.TransitionFSMState(FSMTransition.ToLate);
            CitizenMgr.IllCnt--;
            return;
        }
        if (mCitizen.bCheckAmbulanceKey)
        {
            mCitizen.bCheckAmbulanceKey = false;
            if (CitizenMgr.CheckBedsAvailable())
                mFSMSystem.TransitionFSMState(FSMTransition.TookByAmbulance);
            else
                mFSMSystem.TransitionFSMState(FSMTransition.CantTakeAmbulance);
            return;
        }
    }

    public override void StateEnd()
    {
        
    }

    float currentMoveTimer = 0;
    void RandomMove()
    {
        currentMoveTimer += Time.fixedDeltaTime;
        if (currentMoveTimer > MoveTimerTotal)
        {
            currentMoveTimer = 0;
            MoveTimerTotal = CitizenMgr.GenerateMovePeriod();
            if (mCitizen.CheckReachDest())
            {
                int loopTimes = 0;
                while (loopTimes++ < 10)
                {
                    if (mCitizen.TryMove(CitizenMgr.GenerateMoveDest(),0.1f))
                        break;
                }
            }
        }
    }

    void CheckDying()
    {
        if (mCitizen.CheckDying())
            mCitizen.AddStatus(CitizenStatusType.Dying);
    }
}
