using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hospital : MonoBehaviour
{
    [HideInInspector]public Transform CheckPoint;
    public void Init()
    {
        CheckPoint = transform.Find("CheckPoint");

        foreach (var keyValue in CitizenMgr.CitizensDict)
        {
            Citizen c = keyValue.Value;
            if (c.bMovingToHosp||c.bWaitingForHosp)
                keyValue.Value.bRecalculatePathKey = true;
        }
    }

    public Vector3 GenerateSpawnPoint()
    {
        GameObject go = CheckPoint.gameObject;
        Renderer r = go.GetComponent<Renderer>();
        float xLenD2 = r.bounds.size.x / 2;
        float zLenD2 = r.bounds.size.z / 2;
        float xMax = go.transform.position.x + xLenD2;
        float xMin = go.transform.position.x - xLenD2;
        float zMax = go.transform.position.z + zLenD2;
        float zMin = go.transform.position.z - zLenD2;
        float newX = Random.Range(xMin, xMax);
        float newZ = Random.Range(zMin, zMax);
        return new Vector3(newX, 0.141f, newZ);
    }
}

public class HospitalNavInfo
{
    public Hospital Hospital;
    public float Dist;
    public bool Reachable;

    public HospitalNavInfo(Hospital hospital, float v)
    {
        Hospital = hospital;
        Dist = v;
        Reachable = false;
    }
}