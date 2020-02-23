using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{

    public float scanFrequency=1f;//检测频率
    public float DestoryTime=5;//自我销毁时间
    float halfRange;
    float existTime=0;//已存在时间
    float deltaTime=0;
    public int type = 0; //1.用于消毒区 2.用于检测 3.用于检疫站

    private void Start()
    {
        if(type == 1)
        {
            DestoryTime = GameManager._instance.areaTime;
            halfRange = 2;
        }
        if(type == 2)
        {
            DestoryTime = 1;
            halfRange = 1.5f;
            scanFrequency = 0.2f;
        }
        if(type == 3)
        {
            scanFrequency = 2f;
            halfRange = 1.5f;
        }
    }

    private void Update()
    {
        
        if(deltaTime>scanFrequency)
        {
            var cols = Physics.OverlapBox(gameObject.transform.position, new Vector3(1.5f, 1.5f, 1.5f), transform.rotation);
            foreach(var c in cols)
            {
                Citizen citizen = c.GetComponent<Citizen>();
                if (c.gameObject.layer == 8 && citizen.CitizenType == CitizenType.Latent)
                {                   
                    //临时减少感染概率代码
                    if (type == 1)
                    {
                        citizen.InfectRate -= 0.5f;
                    }
                    //检测潜伏期的代码
                    else
                    {
                        citizen.CheckReavel();  
                    }
                }
                
            }
            deltaTime = 0;
        }
        deltaTime += Time.deltaTime;
        existTime += Time.deltaTime;
        if(existTime > DestoryTime && type!=3)
        {
            Destroy(gameObject);
        }

        
    }
    private void OnTriggerExit(Collider other)
    {
        //将感染概率恢复原状
        if(other!=null)
        other.gameObject.GetComponent<Citizen>().InfectRate += 0.5f;
        //
    }

}
