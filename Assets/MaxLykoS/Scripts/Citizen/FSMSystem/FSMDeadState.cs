using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMDeadState : FSMBaseState
{
    public FSMDeadState(FSMSystem fsmSystem) : base(fsmSystem, FSMStateID.DeadFSMStateID) { }

    public override void StateStart()
    {
        mCitizen.CitizenType = CitizenType.Dead;
        mCitizen.name = "Dead";
        mCitizen.SetActive(false);
        mCitizen.DestroySelf();

        CitizenMgr.DeadCnt++;
        Broadcast.Instance.AddString("疫情造成患者死亡。", Color.red);
    }

    public override void TransitionReason()
    {
        
    }
}
