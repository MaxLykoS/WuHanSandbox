using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMLatentState : FSMBaseState
{
    public FSMLatentState(FSMSystem fsmSystem) : base(fsmSystem, FSMStateID.LatentFSMStateID) { }

    float MoveTimerTotal;

    public override void StateFixUpdate()
    {
        RandomMove();
    }

    public override void StateStart()
    {
        mAnimator.SetBool("Healthy", true);
        MoveTimerTotal = CitizenMgr.GenerateMovePeriod();
        mCitizen.timeAlive = 0;
        mCitizen.transform.name = "Latent";
        mCitizen.CitizenType = CitizenType.Latent;

        CitizenMgr.LatentCnt++;
    }

    public override void StateEnd()
    {
        mCitizen.timeAlive = 0;
        mAnimator.SetBool("Healthy", false);
        CitizenMgr.LatentCnt--;
    }

    public override void TransitionReason()
    {
        if (mCitizen.timeAlive > mCitizen.LatentTimeTotal)
        { 
            mFSMSystem.TransitionFSMState(FSMTransition.AfterLatent);
            return;
        }
        if (mCitizen.bCheckAmbulanceKey)
        {
            mCitizen.AliveTimeTotal += mCitizen.LatentTimeTotal - mCitizen.timeAlive;
            mCitizen.timeAlive = 0;
            if (CitizenMgr.CheckBedsAvailable())
                mFSMSystem.TransitionFSMState(FSMTransition.TookByAmbulance);
            else
                mFSMSystem.TransitionFSMState(FSMTransition.CantTakeAmbulance);
            CitizenMgr.IllCnt++;
            return;
        }
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
}
