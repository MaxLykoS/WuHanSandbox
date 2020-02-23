using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMHealthyState : FSMBaseState
{
    public FSMHealthyState(FSMSystem fsmSystem) : base(fsmSystem, FSMStateID.HealthyFSMStateID) { }
    float MoveTimerTotal;
    public override void StateStart()
    {
        mCitizen.CitizenType = CitizenType.Healthy;
        mCitizen.timeAlive = 0;
        mCitizen.bWearingMask = false;
        MoveTimerTotal = CitizenMgr.GenerateMovePeriod();
        mCitizen.transform.name = "Healthy";

        mAnimator.SetBool("Healthy", true);

        CitizenMgr.HealthyCnt++;
    }

    public override void StateFixUpdate()
    {
        RandomMove();
    }

    public override void TransitionReason()
    {
        CheckInfected();
    }

    public override void StateEnd()
    {
        mCitizen.timeAlive = 0;
        CitizenMgr.HealthyCnt--;
    }

    void CheckInfected()
    {
        var colls = Physics.OverlapSphere(mCitizen.transform.position, CitizenMgr.MaxSafeDist);
        foreach (Collider co in colls)
        {
            Citizen p = co.gameObject.GetComponent<Citizen>();
            if (p &&(p.CitizenType==CitizenType.Ill||p.CitizenType==CitizenType.Latent))
                if (p != null && !CitizenMgr.SafeDistance(mCitizen, p))
                {
                    if (Random.Range(0.0f, 1.0f) < mCitizen.InfectRate)
                    {
                        mFSMSystem.TransitionFSMState(FSMTransition.Infected);
                        return;
                    }
                }
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
