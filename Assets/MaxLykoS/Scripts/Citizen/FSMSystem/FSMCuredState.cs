using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMCuredState : FSMBaseState
{
    public FSMCuredState(FSMSystem fsmSystem) : base(fsmSystem, FSMStateID.CuredFSMStateID) { }
    float MoveTimerTotal;

    public override void StateFixUpdate()
    {
        RandomMove();
    }

    public override void StateStart()
    {
        MoveTimerTotal = CitizenMgr.GenerateMovePeriod();
        mCitizen.CitizenType = CitizenType.Cured;
        mCitizen.ClearAllStatus(false);
        mCitizen.AddStatus(CitizenStatusType.Healthy);
        mCitizen.name = "Cured";
        mAnimator.SetBool("Healthy", true);

        CitizenMgr.CuredCnt++;
    }

    public override void TransitionReason()
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
                    if (mCitizen.TryMove(CitizenMgr.GenerateMoveDest(), 0.1f))
                        break;
                }
            }
        }
    }
}
