using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CDControl : MonoBehaviour
{
    //这个类用来控制技能冷却
    bool isFinished = true;
    bool isStarted = false;
    float deltaTime;
    float coolingTime;
    float decreaseTime = 0;
    //MaskImage
    public Image mask;
    public void Start()
    {
        
    }
    private void Update()
    {
        if(isStarted==true)
        {
            if (deltaTime > coolingTime)
            {
                mask.fillAmount = 0;
                isFinished = true;
                isStarted = false;
                deltaTime = 0;
            }
            else
            {
                deltaTime += Time.deltaTime;
                mask.fillAmount = 1 - deltaTime / coolingTime;
            }
                               
        }                    
    }
    public void startCooling(float coolingTime)
    {
        if(isFinished == true && coolingTime!=0)
        {
            isStarted = true;
            isFinished = false;
            this.coolingTime = coolingTime -decreaseTime;           
        }
    }
    public bool isCoolingDown()
    {
        return isFinished;
    }
    public void DecreaseCoolingTime(float value)
    {
        decreaseTime = value; 
    }
}
