using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CitizenMgr : Singletion<CitizenMgr>
{
    #region 可修改属性
    public static int DayTicks = 5;

    public static int Population = 45;  //人口数量 （每个1代表一个具体化的人，数值别太大
    public static int InitialInfectedCnt = 5;  //最初感染者数

    public static int BedsCntTotal = 5;  //病床数量

    public static double DeathRate = 0.1f;  //治疗死亡率

    public static float MinSafeDist = 0.1f;  //戴口罩的传播距离
    public static float MaxSafeDist = 0.5f;  //不戴口罩的传播距离

    public static float MinCureTime = 5;  //最短治愈时间
    public static float MaxCureTime = 13;  //最长治愈时间

    public static float MinLatentTime = 0;  //最短潜伏时间
    public static float MaxLatentTime = 14;  //最长潜伏时间

    public static float MinToHospTime = 1;  //最短就诊延迟
    public static float MaxToHospTime = 3;  //最长就诊延迟

    public static float MinAliveTime = 15;  //最短存活时间
    public static float MaxAliveTime = 30;  //最长存活时间

    public static float MinInfectRate = 0.7f;  //最小传染几率
    public static float MaxInfectRate = 0.9f;  //最大传染几率

    public static float MinMovePeriod = 1.0f;  //人物最短移动间隔
    public static float MaxMovePeriod = 10.0f; //人物最长移动间隔

    public static float DyingHintDelay = 3;   //Dying提示提前X天显示

    public static CitizenType InitInfectedType = CitizenType.Latent;
    #endregion

    #region 外部资源 
    public RuntimeAnimatorController F1;
    public RuntimeAnimatorController F2;
    public RuntimeAnimatorController M1;
    public RuntimeAnimatorController M2;
    static readonly string CITIZENPREFABPATH = "MaxLykoS/Prefabs/Citizen";
    #endregion

    public static Dictionary<int, Citizen> CitizensDict;
    public static List<GameObject> SpawnPlanesList;
    public static List<Hospital> HospitalsList;

    public static int CurBedsCnt;
    public static float DaysCnt;
    public static int HealthyCnt;
    public static int LatentCnt;
    public static int IllCnt;
    public static int InHospitalCnt;
    public static int WaitingCnt;
    public static int MoveingCnt;
    public static int CuredCnt;
    public static int DeadCnt;

    private Transform PersonsRoot;
    public bool Init()
    {
        CitizensDict = new Dictionary<int, Citizen>();
        SpawnPlanesList = new List<GameObject>();
        HospitalsList = new List<Hospital>();
        CurBedsCnt = BedsCntTotal;
        DaysCnt = 0;
        HealthyCnt = 0;
        LatentCnt = 0;
        IllCnt = 0;
        InHospitalCnt = 0;
        MoveingCnt = 0;
        WaitingCnt = 0;
        CuredCnt = 0;
        DeadCnt = 0;

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("SpawnPlane"))
            if (!SpawnPlanesList.Contains(go))
                SpawnPlanesList.Add(go);
        PersonsRoot = GameObject.Find("PersonsRoot").transform;

        foreach (Hospital hosp in GameObject.Find("Scene/HospitalsRoot").GetComponentsInChildren<Hospital>())
        {
            hosp.Init();
            HospitalsList.Add(hosp);
        }

        if (!Spawn(CitizenType.Healthy, Population - InitialInfectedCnt))
            return false;
        if (!Spawn(CitizenType.Latent, InitialInfectedCnt))
            return false;
        Debug.Log("初始化完毕！");
        return true;
    }

    bool Spawn(CitizenType type, int cnt)
    {
        HashSet<int> fullSpawnPlaneIndex = new HashSet<int>();
        while (cnt != 0)
        {
            int terrIndex = Random.Range(0, SpawnPlanesList.Count);
            if (fullSpawnPlaneIndex.Contains(terrIndex))
                continue;
            else
            {
                GameObject go = SpawnPlanesList[terrIndex];
                Renderer r = go.GetComponent<Renderer>();
                float xLenD2 = r.bounds.size.x / 2;
                float zLenD2 = r.bounds.size.z / 2;
                float xMax = go.transform.position.x + xLenD2;
                float xMin = go.transform.position.x - xLenD2;
                float zMax = go.transform.position.z + zLenD2;
                float zMin = go.transform.position.z - zLenD2;
                float newX = Random.Range(xMin, xMax);
                float newZ = Random.Range(zMin, zMax);
                int _loopTimes = 0;
                bool deadLoop = false;
                while (Physics.CheckSphere(new Vector3(newX, 0.141f, newZ), 0.3f, 1 << 8))
                {
                    newX = Random.Range(xMin, xMax);
                    newZ = Random.Range(zMin, zMax);
                    ++_loopTimes;
                    if (_loopTimes > 10)
                    {
                        Debug.Log("找不到刷新地点，换下一个地形");
                        fullSpawnPlaneIndex.Add(terrIndex);
                        deadLoop = true;
                        if (fullSpawnPlaneIndex.Count == SpawnPlanesList.Count)
                        {
                            Debug.Log("人数太多，无法放下!");
                            return false;
                        }
                        break;
                    }
                }
                if (deadLoop)
                    continue;
                GameObject prefab = Resources.Load<GameObject>(CITIZENPREFABPATH);
                Citizen newc = Instantiate<GameObject>(prefab, new Vector3(newX, prefab.transform.position.y, newZ), Quaternion.Euler(0, 90, 0), PersonsRoot).AddComponent<Citizen>();
                int id = CitizensDict.Count;
                CitizensDict[id] = newc;
                newc.Init(id, type);
                --cnt;
            }
        }
        return true;
    }

    public static bool CheckBedsAvailable() => CurBedsCnt - MoveingCnt  > 0;
    public static bool SafeDistance(Citizen p1, Citizen p2)
    {
        float dis = Vector3.Distance(p1.transform.position, p2.transform.position);
        if (p1.bWearingMask || p2.bWearingMask)
            return dis > MinSafeDist ? true : false;
        else
            return dis > MaxSafeDist ? true : false;
    }
    public static float GenerateCureTime() => Random.Range(MinCureTime, MaxCureTime)*DayTicks;
    public static float GenerateLatentTime() => Random.Range(MinLatentTime, MaxLatentTime)* DayTicks;
    public static float GenerateToHospitalTime() => Random.Range(MinToHospTime, MaxToHospTime)* DayTicks;
    public static float GenerateAliveTime() => Random.Range(MinAliveTime, MaxAliveTime)*DayTicks;
    public static float GenerateMovePeriod() => Random.Range(MinMovePeriod, MaxMovePeriod);
    public static float GenerateInfectRate() => Random.Range(MinInfectRate, MaxInfectRate);
    public static Vector3 GenerateMoveDest()
    {
        int planeIndex = Random.Range(0, SpawnPlanesList.Count);
        GameObject go = SpawnPlanesList[planeIndex];
        Renderer r = go.GetComponent<Renderer>();
        float xLenD2 = r.bounds.size.x / 2;
        float zLenD2 = r.bounds.size.z / 2;
        float xMax = go.transform.position.x + xLenD2;
        float xMin = go.transform.position.x - xLenD2;
        float zMax = go.transform.position.z + zLenD2;
        float zMin = go.transform.position.z - zLenD2;
        float newX = Random.Range(xMin, xMax);
        float newZ = Random.Range(zMin, zMax);
        return new Vector3(newX, go.transform.position.y + 0.0001f, newZ);
    }

    public static void ChangeInfectRateOverall(float offset)
    {
        foreach (var kv in CitizensDict)
            kv.Value.InfectRate += offset;
    }

    public static void ChangeCureTimeOverall(float offset)
    {
        foreach (var kv in CitizensDict)
            kv.Value.CureTimeTotal += offset;
    }

    public static void IncreaseBedsCnt(int cnt)
    {
        BedsCntTotal += cnt;
        CurBedsCnt += cnt;
    }

    private void FixedUpdate()
    {
        DaysCnt += Time.fixedDeltaTime;
    }
}
