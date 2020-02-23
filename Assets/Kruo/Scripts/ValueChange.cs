using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueChange : MonoBehaviour
{
    public GameObject Ambulance;

    public void Reduceinfection()//降低感染概率
    {
        //相关数值变动写在这里
        CitizenMgr.ChangeInfectRateOverall(-0.3f);
        
        CostAndCoolingFunction(1500, 20f, "应政府宣传，人们都带上了口罩。");
    }

    public void AmbulanceFrequency()//救护车频率
    {
        CostAndCoolingFunction(1000, 200f, "医疗设备投入增加，救护车频率变高了。");
        Ambulance.GetComponent<CDControl>().DecreaseCoolingTime(5);
        //暂无  Max
    }

    public void SpeedOfTreatment() //医院治疗速度
    {
        CostAndCoolingFunction(1000, 20f, "对病毒研究有了新的进展，救治速度增加。");
        CitizenMgr.ChangeCureTimeOverall(-1);
    }
    public void ImproveHospitalSize()//增加床位规模
    {
        CostAndCoolingFunction(1500, 20f, "医院的规模增大，可以容纳更多的病患。");
        CitizenMgr.IncreaseBedsCnt(20);
    }
    void CostAndCoolingFunction(int cost, float coolingTime,string broadcast)//花费金钱、冷却时间和broadcast
    {
        var obj = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if (obj.GetComponent<CDControl>().isCoolingDown())//判断是否冷却完成
        {
            
            if (GameManager._instance.money - cost > 0)
            {
                obj.GetComponent<CDControl>().startCooling(coolingTime);
                Broadcast.Instance.AddString(broadcast, Color.white);
                GameManager._instance.money -= cost;
            }
            else
            {
                Broadcast.Instance.AddString("金钱不足。", Color.red);
            }
        }
        else
        {
            Broadcast.Instance.AddString("冷却未完成。", Color.black);
        }
       
    }
}
