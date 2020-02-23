using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public abstract class FSMBaseState
{
    public FSMStateID mStateID { get; set; }    //状态ID
    public FSMSystem mFSMSystem { get; set; }   //该对象属于在哪个状态机

    public Dictionary<FSMTransition, FSMStateID> mFSMStateIdDic = new Dictionary<FSMTransition, FSMStateID>();

    protected Citizen mCitizen;
    protected Animator mAnimator;
    public FSMBaseState(FSMSystem fsmSystem, FSMStateID stateID)
    {
        this.mFSMSystem = fsmSystem;
        this.mStateID = stateID;
    }

    public void Init(Citizen citizen,Animator animator)
    {
        mCitizen = citizen;
        mAnimator = animator;
    }

    public void AddTransition(FSMTransition transition, FSMStateID stateID)
    {
        if (mFSMStateIdDic.ContainsKey(transition))
        {
            Debug.Log("本状态已经包含了该转换条件");
            return;
        }
        mFSMStateIdDic.Add(transition, stateID);
    }

    public void DeleteTransition(FSMTransition transition)
    {
        if (!mFSMStateIdDic.ContainsKey(transition))
        {
            Debug.Log("容器中没有该转换条件");
            return;
        }
        mFSMStateIdDic.Remove(transition);
    }

    public FSMStateID GetStateIdByTransition(FSMTransition transition)
    {
        if (!mFSMStateIdDic.ContainsKey(transition))
        {
            Debug.Log("容器内没有该转换条件，无法获取状态");
            return FSMStateID.NullFSMStateID;
        }

        return mFSMStateIdDic.FirstOrDefault(q => q.Key == transition).Value;
    }

    public abstract void StateStart();
    public virtual void StateUpdate() { }
    public virtual void StateFixUpdate() { }
    public virtual void StateEnd() { }
    //转化状态条件
    public abstract void TransitionReason();
}