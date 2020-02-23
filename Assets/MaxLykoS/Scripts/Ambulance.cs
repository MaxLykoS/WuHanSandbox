using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ambulance : MonoBehaviour
{
    public static float DetectDist = 3.0f;
    public static float AmbulanceStayedTimeTotal = 0.5f;

    private Hospital originalHosp;
    private Vector3 destPos;
    private NavMeshAgent navAgent;
    private bool bloaded;
    private bool bwaiting;
    private Dictionary<int, Citizen> patientsInCarDict;

    public void Init(Hospital start, Vector3 dest)
    {
        originalHosp = start;
        destPos = dest;
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.SetDestination(destPos);
        navAgent.updateRotation = false;
        bloaded = false;
        bwaiting = true;
        curTimer = 0;
        patientsInCarDict = new Dictionary<int, Citizen>();
    }

    bool CheckReachDest()
    {
        if (!navAgent.pathPending)
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

    float curTimer;
    void Wait()
    {
        if (bwaiting)
        {
            curTimer += Time.fixedDeltaTime;
            if (curTimer > AmbulanceStayedTimeTotal)
            {
                bwaiting = false;
                bloaded = true;
                navAgent.SetDestination(originalHosp.CheckPoint.position);
            }
        }
    }
    void Detect()
    {
        foreach (Collider c in Physics.OverlapSphere(transform.position, DetectDist, 1 << 8))
        {
            Citizen citizen = c.gameObject.GetComponent<Citizen>();
            if (citizen.CitizenType == CitizenType.Ill && citizen.bMovingToHosp)  //正在去医院的
            {
                patientsInCarDict[citizen.ID] = citizen;
                citizen.SetActive(false);
            }
            else if (citizen.CitizenType == CitizenType.Ill && citizen.timeAlive <= citizen.ToHospTimeTotal) //生病但不去医院的
            {
                citizen.bCheckAmbulanceKey = true;
            }
            else if (citizen.CitizenType == CitizenType.Latent)                //潜伏期的
            {
                citizen.bCheckAmbulanceKey = true;
            }
        }
    }

    void FirstBackToHosp()
    {
        if(bloaded)   //第一次返回医院
        {
            bloaded = false;
            foreach (var pair in patientsInCarDict)
                pair.Value.SetActive(true);
            patientsInCarDict.Clear();
            DestroyImmediate(gameObject);
        }
    }

    void FixedUpdate()
    {
        UpdateRotation();
        UpdatePosition();

        if (CheckReachDest() && !bloaded)
        {
            Detect();
            Wait();
        }
        if (!bwaiting && CheckReachDest() && bloaded)
        {
            FirstBackToHosp();
        }
    }

    void UpdateRotation()
    {
        if (!navAgent)
            return;
        Vector3 v = transform.eulerAngles;
        if (navAgent.velocity.z > 0.1f)
            v.y = 90;
        else
            v.y = -90;
        transform.eulerAngles = v;
    }

    void UpdatePosition()
    {
        if (patientsInCarDict.Count != 0)
        {
            foreach (var pair in patientsInCarDict)
            {
                Vector3 pos = transform.position;
                pos.y = pair.Value.transform.position.y;
                pair.Value.transform.position = pos;
            }
        }
    }
}
