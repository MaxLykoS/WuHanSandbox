using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Build : MonoBehaviour
{


    //这个类通过摄像机发射射线来确定鼠标点的位置
    private Camera mainCamera;
    private Ray ray;
    private RaycastHit hit;
    private bool isBuilding = false;//是否是建造状态
    private bool isDeleting = false;//是否是拆除状态

    public GameObject quad;//显示在屏幕上的格子
    public GameObject quad_3x3;//显示在屏幕上的格子
    public GameObject quad_4x4;//显示在屏幕上的格子
    public GameObject quad_1x4;//显示在屏幕上的格子

    GameObject preBuildGameObject;// 将要建造的GameObject
    Vector3 offsetValue; //每个模型需要的偏离值
    int costValue;//每次的花费
    GameObject selectedObject;//按钮选中的技能
    float coolingTime;//选中物体的冷却时间
    GameObject requiredQuad;//所需要的格子

    /*处理在建造前点击其他建造按钮时的BUG,
    因为button事件会改变requiredQuad,
    导致之前的quad不能关闭，所以需要一个变量存储上一个使用的格子
    */
    GameObject lastQuad;//处理bug
    string needTag;// 射线照到需要的标签

    public GameObject hosipatal;
    public GameObject spacers;//路障
    public GameObject isolation;//隔离区
    public GameObject QuarStation;//检疫站
    public GameObject ScanArea;//检测区域
    public GameObject AmbulancePrefab;//救护车

    void Start()
    {
        mainCamera = gameObject.GetComponent<Camera>();
        lastQuad = quad;
    }

    void Update()
    {
        if (isBuilding)//点击了按钮
        {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction, Color.red);

            if (Physics.Raycast(ray, out hit, 100, ~(1 << 5) | ~(1 << 8) |~(1 << 11) | ~(1 << 12)))//射线检测,除去人和UI
            {
                if(isDeleting==true)//如果是删除状态
                {
                    requiredQuad.SetActive(true);
                    requiredQuad.transform.position = new Vector3(Round(hit.point.x, 0), 0.031f, Round(hit.point.z, 0));
                    
                    //quad.transform.position = hit.collider.gameObject.GetComponent<Transform>().position;                 
                    if (Input.GetMouseButtonDown(0))//删除物体
                    {
                        var cols = Physics.OverlapBox(requiredQuad.transform.position, new Vector3(0.5f, 0.5f, 0.5f), requiredQuad.transform.rotation);
                        GameObject obj=null;
                        foreach (var c in cols)
                        {
                            
                            if (c.gameObject.tag == "spacers")
                            {
                                obj = c.gameObject;
                            }
                        }
                        if(obj!=null)
                        Destroy(obj);

                        foreach (var keyvalue in CitizenMgr.CitizensDict)
                        {
                            Citizen c = keyvalue.Value;
                            if (c.bMovingToHosp || c.bWaitingForHosp)
                                c.bRecalculatePathKey = true;
                        }
                    }
                }
                //Debug.Log(hit.collider.gameObject.name);
                else if (hit.collider != null)//碰撞体不为空，打开格子
                {
                    requiredQuad.SetActive(true);
                    //quad.transform.position = hit.collider.gameObject.GetComponent<Transform>().position; 
                    requiredQuad.transform.position = new Vector3(Round(hit.point.x, 0), 0.031f, Round(hit.point.z, 0));
                    //Debug.Log(hit.collider.gameObject.name);
                    if (Input.GetMouseButtonDown(0))//点击左键生成物体 花费金钱 进入冷却
                    {
                        if (!UIControl.GetOverUI())//判断是否在建造前点击其他建造按钮
                        {

                            if (hit.collider.gameObject.tag == "SpawnPlane" || hit.collider.gameObject.tag == "Plane" || hit.collider.gameObject.tag == "road" || needTag == "all" )//如果已经有建筑
                            {
                                if (needTag == hit.collider.gameObject.tag || needTag == "all")//判断需要建造在路上还是plane
                                {
                                    GameObject[] objs = ScanAround(requiredQuad.transform.position);
                                    int[] dir = roadDir(objs);
                                    if (dir[0] == 1 && preBuildGameObject == spacers)//如果是十字路且是隔离带
                                    {
                                        Broadcast.Instance.AddString("十字路口不能建造隔离带",Color.red);
                                    }
                                    else if(dir[0] == -1 && preBuildGameObject == hosipatal)
                                    {
                                        Broadcast.Instance.AddString("医院旁需要道路", Color.red);
                                    }
                                    else if (selectedObject.GetComponent<CDControl>().isCoolingDown())//判断是否冷却完成
                                    {
                                        selectedObject.GetComponent<CDControl>().startCooling(coolingTime);
                                        if (preBuildGameObject == AmbulancePrefab)
                                        {
                                            //救护车代码  救助位置 requiredQuad.transform.position + offsetValue - new Vector3(0, 0.031f, 0)
                                           // Broadcast._instance.AddString("救护车出动",Color.black);
                                            Vector3 rescurePos = requiredQuad.transform.position + offsetValue - new Vector3(0, 0.031f, 0);
                                            float minDist = float.MaxValue;
                                            Hospital nearHospital = CitizenMgr.HospitalsList[0];
                                            foreach (Hospital hos in CitizenMgr.HospitalsList)
                                            {
                                                float dist = Vector3.Distance(rescurePos, hos.CheckPoint.position);
                                                if (minDist > dist)
                                                {
                                                    minDist = dist;
                                                    nearHospital = hos;
                                                }
                                            }
                                            Ambulance ambulance = Instantiate<GameObject>(Resources.Load<GameObject>("MaxLykoS/Prefabs/Ambulance"), nearHospital.CheckPoint.position, Quaternion.Euler(0, 90, 0)).AddComponent<Ambulance>();
                                            ambulance.Init(nearHospital, rescurePos);
                                        }
                                        else
                                        {
                                            var o = Instantiate(preBuildGameObject);
                                            o.GetComponent<Transform>().position = requiredQuad.transform.position + offsetValue - new Vector3(0, 0.031f, 0);//暂时调整y，之后用shader调整
                                            if (preBuildGameObject == spacers)//如果是隔离带，则需要判断方向
                                            {
                                                if (hit.collider.gameObject.GetComponent<Transform>().localScale.x > hit.collider.gameObject.GetComponent<Transform>().localScale.y)
                                                {
                                                    o.transform.eulerAngles = new Vector3(0, 90, 0);
                                                }
                                                foreach (var keyvalue in CitizenMgr.CitizensDict)
                                                {
                                                    Citizen c = keyvalue.Value;
                                                    if (c.bMovingToHosp || c.bWaitingForHosp)
                                                        c.bRecalculatePathKey = true;
                                                }
                                            }
                                            else if (preBuildGameObject == hosipatal)
                                            {
                                                o.transform.SetParent(GameObject.Find("Scene/HospitalsRoot").transform);
                                                Hospital newHosp = o.GetComponent<Hospital>();
                                                newHosp.Init();
                                                CitizenMgr.HospitalsList.Add(newHosp);
                                                //触发器绕医院旋转角度
                                                Vector3 point = Quaternion.AngleAxis(dir[1] * -1 * 90, Vector3.up) * (o.transform.GetChild(0).position - o.transform.position);
                                                o.transform.GetChild(0).position = o.transform.position + point;                                                
                                                //o.transform.eulerAngles = new Vector3(0, dir[1] * -1 * 90 + o.transform.eulerAngles.y, 0);
                                            }

                                        }
                                        if (GameManager._instance.money - costValue > 0)
                                        {
                                            GameManager._instance.money -= costValue;
                                        }
                                        else
                                        {
                                            Broadcast.Instance.AddString("金钱不足", Color.red);
                                        }
                                        requiredQuad.SetActive(false);
                                        isBuilding = false;
                                    }
                                    else//控制冷却
                                    {
                                        Broadcast.Instance.AddString("冷却未完成", Color.red);
                                    }
                                }
                                else//控制建造位置
                                {

                                    Broadcast.Instance.AddString("这里不能建造", Color.red);
                                }
                            }
                            else
                            {
                                Debug.Log(hit.collider.gameObject.tag);
                                Broadcast.Instance.AddString("这里不能建造",Color.red);
                            }

                        }

                    }
                }
                else//如果射线碰到物体时显示格子
                {
                    requiredQuad.SetActive(false);
                }
            }
            if (Input.GetMouseButtonDown(1))//右键取消建造
            {
                isBuilding = false;
                requiredQuad.SetActive(false);

            }
        }
        
    }

    public void BuildingHosipital()//医院
    {
        buildFunction(1000, hosipatal, new Vector3(0, 0.031f, 0), 20f, quad, "Plane");
    }
    public void BuildingIsolation()//消毒区
    {
        buildFunction(100, isolation, new Vector3(0, 0.031f, 0), 5f, quad_4x4, "all");
    }
    public void BuildingSpacer()//路障
    {
        buildFunction(100, spacers, new Vector3(0, 0, 0), GameManager._instance.spacersCoolingTime, quad, "road");
    }
    public void BuildQuarStation()//检疫站
    {
        buildFunction(500, QuarStation, new Vector3(0, 0.5f, 0), 20f, quad_3x3, "Plane");
    }
    public void Scan()//检测
    {
        buildFunction(100, ScanArea, new Vector3(0, 0.032f, 0), 4f, quad_4x4, "all");
    }
    public void Ambulance()//救护车
    {
        buildFunction(200, AmbulancePrefab, new Vector3(0, 0.5f, 0), 10f, quad, "road");
    }
    public void Delete()//删除
    {
        lastQuad.SetActive(false);
        isBuilding = true;
        this.requiredQuad = quad;//所需要的格子样式
        lastQuad = requiredQuad;
        isDeleting = true;
    }

    void buildFunction(int cost, GameObject buildObj, Vector3 offset, float coolingTime, GameObject requiredQuad, string tag)//建造方式
    {
        lastQuad.SetActive(false);
        costValue = cost;//花费
        isBuilding = true;
        preBuildGameObject = buildObj;//预制体
        offsetValue = offset;//位置偏移
        selectedObject = EventSystem.current.currentSelectedGameObject;
        Debug.Log(selectedObject.name);
        this.coolingTime = coolingTime;//冷却时间
        this.requiredQuad = requiredQuad;//所需要的格子样式
        lastQuad = requiredQuad;
        needTag = tag;
        isDeleting = false;
    }

    public static float Round(float value, int digit)//四舍五入
    {
        float vt = Mathf.Pow(10, digit);
        //1.乘以倍数 + 0.5
        float vx = value * vt + 0.5f;
        //2.向下取整
        float temp = Mathf.Floor(vx);
        //3.再除以倍数
        return (temp / vt);
    }
    public GameObject[] ScanAround(Vector3 position) //得到射线得到物体四个方向的物体
    {
        RaycastHit _hit;
        GameObject[] objs = new GameObject[4];
        Physics.Raycast(mainCamera.transform.position, position + new Vector3(1f, 0, 0) - mainCamera.transform.position, out _hit, 100, (1 << 9) | (1 << 10));
        objs[0] = _hit.collider.gameObject;

        Physics.Raycast(mainCamera.transform.position, position + new Vector3(0, 0, 1f) - mainCamera.transform.position, out _hit, 100, (1 << 9) | (1 << 10));
        objs[1] = _hit.collider.gameObject;

        Physics.Raycast(mainCamera.transform.position, position + new Vector3(-1f, 0, 0) - mainCamera.transform.position, out _hit, 100, (1 << 9) | (1 << 10));
        objs[2] = _hit.collider.gameObject;

        Physics.Raycast(mainCamera.transform.position, position + new Vector3(0, 0, -1f) - mainCamera.transform.position, out _hit, 100, (1 << 9) | (1 << 10));
        objs[3] = _hit.collider.gameObject;
        foreach (var o in objs)
        {
            //Debug.Log(o.name);
        }

        return objs;
    }
    public int[] roadDir(GameObject[] objs)
    {
        int[] dir = new int[4];//第一个索应代表是不是交叉路h或者没有路，23存储方向，0下，1右，2上，3左
        int index = 1;
        bool isHasRoad=false;
        dir[0] = 0;//默认不是十字路
        for (int i = 0; i < 4; i++)
        {
            if (objs[i].tag == "road")
            {
                //Debug.Log(objs[i].name);
                dir[index] = i;
                index++;
                isHasRoad = true;
            }
            if (index == 4)//说明是十字路
            {
                dir[0] = 1;
                break;
            }

        }
        if (isHasRoad == false)//周围没有路
        {
            dir[0] = -1;
        }
        return dir;
    }
}
