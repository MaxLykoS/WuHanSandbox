using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum CitizenType
{
    Healthy,
    Latent,
    Ill,
    InHospital,
    Cured,
    Dead
}
public class Citizen : MonoBehaviour
{
    public int ID;
    public CitizenType CitizenType;
    public float timeAlive;
    public bool bWearingMask;
    public bool bMovingToHosp;
    public bool bWaitingForHosp;
    public bool bRecalculatePathKey;
    public bool bCheckAmbulanceKey;  //只有true生效

    public float CureTimeTotal;
    public float LatentTimeTotal;
    public float ToHospTimeTotal;
    public float AliveTimeTotal;//从Ill之后开始计算，Ill时timeAlive会从0累计
    public float InfectRate;

    public Hospital Hospital
    {
        get { if (CitizenType == CitizenType.InHospital) return hospital; else return null; }
        set { hospital = value; }
    }

    private SpriteRenderer spriteRenderer;
    private NavMeshAgent navAgent;
    private Animator animator;
    private CitizenStatus citizenStatus;
    private FSMSystem fsmSystem;
    private BoxCollider boxCollider;
    private Hospital hospital;
    public void Init(int id, CitizenType citizenType)
    {
        #region 初始化参数
        timeAlive = 0;
        ID = id;
        CitizenType = citizenType;
        bWearingMask = false;
        bRecalculatePathKey = false;
        bCheckAmbulanceKey = false;

        CureTimeTotal = CitizenMgr.GenerateCureTime();
        LatentTimeTotal = CitizenMgr.GenerateLatentTime();
        ToHospTimeTotal = CitizenMgr.GenerateToHospitalTime();
        AliveTimeTotal = CitizenMgr.GenerateAliveTime();
        InfectRate = CitizenMgr.GenerateInfectRate();
        #endregion

        #region 初始化组件
        spriteRenderer = GetComponent<SpriteRenderer>();
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        citizenStatus = GetComponent<CitizenStatus>();
        boxCollider = GetComponent<BoxCollider>();

        navAgent.updateRotation = false;
        citizenStatus.Init(transform);
        #endregion

        #region 初始化状态机
        fsmSystem = new FSMSystem();

        FSMHealthyState healthyState = new FSMHealthyState(fsmSystem);
        healthyState.AddTransition(FSMTransition.Infected, FSMStateID.LatentFSMStateID);

        FSMLatentState latentState = new FSMLatentState(fsmSystem);
        latentState.AddTransition(FSMTransition.AfterLatent, FSMStateID.IllFSMStateID);
        latentState.AddTransition(FSMTransition.TookByAmbulance, FSMStateID.ToHospitalFSMStateID);
        latentState.AddTransition(FSMTransition.CantTakeAmbulance, FSMStateID.WaitToHospFSMStateID);

        FSMIllState illState = new FSMIllState(fsmSystem);
        illState.AddTransition(FSMTransition.WantHealing, FSMStateID.WaitToHospFSMStateID);
        illState.AddTransition(FSMTransition.ToLate, FSMStateID.DeadFSMStateID);
        illState.AddTransition(FSMTransition.TookByAmbulance, FSMStateID.ToHospitalFSMStateID);
        illState.AddTransition(FSMTransition.CantTakeAmbulance, FSMStateID.WaitToHospFSMStateID);

        FSMWaitHospState waitToHospitalState = new FSMWaitHospState(fsmSystem);
        waitToHospitalState.AddTransition(FSMTransition.BedsAvailable, FSMStateID.ToHospitalFSMStateID);
        waitToHospitalState.AddTransition(FSMTransition.ToLate, FSMStateID.DeadFSMStateID);

        FSMToHospitalState toHospitalState = new FSMToHospitalState(fsmSystem);
        toHospitalState.AddTransition(FSMTransition.ReachHospital, FSMStateID.InHospitalFSMStateID);
        toHospitalState.AddTransition(FSMTransition.ToLate, FSMStateID.DeadFSMStateID);

        FSMInHospitalState inHospitalState = new FSMInHospitalState(fsmSystem);
        inHospitalState.AddTransition(FSMTransition.Healed, FSMStateID.CuredFSMStateID);
        inHospitalState.AddTransition(FSMTransition.HealingFailed, FSMStateID.DeadFSMStateID);
        inHospitalState.AddTransition(FSMTransition.ToLate, FSMStateID.DeadFSMStateID);

        FSMCuredState curedState = new FSMCuredState(fsmSystem);
        FSMDeadState deadState = new FSMDeadState(fsmSystem);

        List<FSMBaseState> statesList = new List<FSMBaseState>{ healthyState, latentState, illState, waitToHospitalState,
                                                                toHospitalState, inHospitalState,curedState, deadState };
        foreach (FSMBaseState state in statesList)
        {
            state.Init(this, animator);
        }
        switch (citizenType)
        {
            case CitizenType.Healthy: fsmSystem.AddFSMSate(healthyState); CitizenType = CitizenType.Healthy; break;
            case CitizenType.Latent: fsmSystem.AddFSMSate(latentState); CitizenType = CitizenType.Latent; break;
            case CitizenType.Ill: fsmSystem.AddFSMSate(illState); CitizenType = CitizenType.Ill; break;
            case CitizenType.InHospital: fsmSystem.AddFSMSate(inHospitalState); CitizenType = CitizenType.InHospital; break;
            case CitizenType.Cured: fsmSystem.AddFSMSate(curedState); CitizenType = CitizenType.Cured; break;
            case CitizenType.Dead: fsmSystem.AddFSMSate(deadState); CitizenType = CitizenType.Dead; break;
        }
        foreach (FSMBaseState state in statesList)
        {
            fsmSystem.AddFSMSate(state);
        }
        #endregion

        #region 初始化动画
        int controllerIndex = Random.Range(0, 4);
        AnimatorOverrideController controller = new AnimatorOverrideController();
        switch (controllerIndex)
        {
            case 0:
                controller.runtimeAnimatorController = CitizenMgr.Instance.F1;
                break;
            case 1:
                controller.runtimeAnimatorController = CitizenMgr.Instance.F2;
                break;
            case 2:
                controller.runtimeAnimatorController = CitizenMgr.Instance.M1;
                break;
            case 3:
                controller.runtimeAnimatorController = CitizenMgr.Instance.M2;
                break;
            default:
                controller.runtimeAnimatorController = CitizenMgr.Instance.F1;
                break;
        }
        animator.runtimeAnimatorController = controller;
        #endregion
    }

    public void AddStatus(CitizenStatusType citizenStatusType)
    {
        citizenStatus.AddStatus(citizenStatusType);
    }

    public void RemoveStatus(CitizenStatusType citizenStatusType)
    {
        citizenStatus.RemoveStatus(citizenStatusType);
    }

    public void ClearAllStatus(bool bDestroy)
    {
        citizenStatus.ClearAll(bDestroy);
    }

    public bool CheckReachDest()
    {
        if (navAgent.enabled && !navAgent.pathPending)
        {
            if (navAgent.remainingDistance <= navAgent.stoppingDistance)
            {
                if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool TryMove(Vector3 pos, float stoppingDist)
    {
        NavMeshPath path = new NavMeshPath();
        if (navAgent.CalculatePath(pos, path))
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                navAgent.SetDestination(pos);
                return true;
            }
        return false;
    }

    public void Move(Vector3 pos, float stoppingDist)
    {
        if (navAgent.enabled)
        {
            navAgent.stoppingDistance = stoppingDist;
            navAgent.SetDestination(pos);
        }
    }

    public void CheckReavel()
    {
        if (CitizenType == CitizenType.Latent)
        {
            citizenStatus.RemoveStatus(CitizenStatusType.Healthy);
            citizenStatus.AddStatus(CitizenStatusType.Latent);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Building"))
        {
            GetComponent<SpriteRenderer>().enabled = false;
            citizenStatus.Hide();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Building"))
        {
            GetComponent<SpriteRenderer>().enabled = true;
            citizenStatus.Display();
        }
    }

    int CompareByDist(HospitalNavInfo x, HospitalNavInfo y)//从小到大排序器
    {
        if (x == null)
        {
            if (y == null)
            {
                return 0;
            }
            return 1;
        }
        if (y == null)
        {
            return -1;
        }

        int retval = x.Dist.CompareTo(y.Dist);
        return retval;
    }
    public HospitalNavInfo GetNearestReachableHospital()
    {
        List<HospitalNavInfo> hospitalsSort = new List<HospitalNavInfo>();
        for (int i = 0; i < CitizenMgr.HospitalsList.Count; i++)
        {
            Hospital h = CitizenMgr.HospitalsList[i];
            hospitalsSort.Add(new HospitalNavInfo(h, Vector3.Distance(transform.position, h.CheckPoint.position)));
        }
        hospitalsSort.Sort(CompareByDist);
        NavMeshPath path = new NavMeshPath();
        foreach (HospitalNavInfo hd in hospitalsSort)
        {
            if (navAgent.CalculatePath(hd.Hospital.CheckPoint.position, path))
            {
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    hd.Reachable = true;
                    return hd;
                }
            }
        }
        return hospitalsSort[0];
    }

    public bool CheckAlive() => timeAlive < AliveTimeTotal;
    public bool CheckDying()// => timeAlive + CitizenMgr.DyingHintDelay > AliveTimeTotal;
    {
        bool ans = timeAlive + CitizenMgr.DyingHintDelay > AliveTimeTotal;
        if(ans)
            Broadcast.Instance.AddString("有患者濒临死亡，请尽快救治。", Color.red);
        return ans;
    }

    public void SetActive(bool t)
    {
        spriteRenderer.enabled = t;
        boxCollider.enabled = t;
        navAgent.enabled = t;
        if(t)
            citizenStatus.Display();
        else
            citizenStatus.Hide();
    }

    private void FixedUpdate()
    {
        timeAlive += Time.fixedDeltaTime;
        fsmSystem.FixUpdateSystem();
        UpdateRotation();
        UpdateMovingAnimation();
    }

    void UpdateRotation()
    {
        if (!navAgent)
            return;
        transform.LookAt(Camera.main.transform.position);
        Vector3 v = transform.eulerAngles;
        if (navAgent.velocity.z > 0.1f)
            v.y = 90;
        else
            v.y = -90;
        transform.eulerAngles = v;
    }

    void UpdateMovingAnimation()
    {
        if (navAgent.velocity == Vector3.zero)
            animator.SetBool("Moving", false);
        else
            animator.SetBool("Moving", true);
    }

    public void DestroySelf()
    {
        ClearAllStatus(true);
        CitizenMgr.CitizensDict.Remove(ID);
        Destroy(gameObject);
    }

    public bool MoveToHosp(float stoppingDist)
    {
        HospitalNavInfo info = GetNearestReachableHospital();
        Move(info.Hospital.CheckPoint.position, stoppingDist);
        Hospital = info.Hospital;
        return info.Reachable;
    }

    public bool CheckReachDestHosp()
    {
        if (!hospital)
            return false;
        float dist = Vector3.Distance(hospital.CheckPoint.transform.position, transform.position);
        bool res = Mathf.Abs(dist - navAgent.stoppingDistance)<0.1f;
        return res;
    }
}
