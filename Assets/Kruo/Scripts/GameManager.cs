using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static public GameManager _instance;//单实例


    //下面为各项数值
    //金钱相关
    public int money = 5000;
    public int moneyImproveRate = 5;
    //时间相关
    float lastTime;
    float deltaTime;
    public float TimeScale=1;
    //游戏整体控制的变量
    public bool isStart=false;
    public int spacersCoolingTime=4;//路障冷却时间
    public int areaTime=20;//消毒区持续时间
    public int CameraMovingSpeed =2;
    //界面相关
    public GameObject items;
        

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        lastTime = Time.time;
    }

    void Update()
    {
        if(isStart==true)
        MoneyIncrease();

    }

    void MoneyIncrease()
    {
        deltaTime +=  Time.deltaTime;       
        if(deltaTime >1)
        {
            money += moneyImproveRate;
            deltaTime = 0;
        }
        
    }
    //控制时间流逝速度
    public void IncreaseTimeScale()
    {
        TimeScale = Mathf.Clamp(TimeScale + 0.5f,0.5f,2.5f);
        Time.timeScale = TimeScale;
    }
    public void DecreaseTimeScale()
    {
        TimeScale = Mathf.Clamp(TimeScale - 0.5f, 0.5f, 2.5f);
        Time.timeScale = TimeScale;
    }
    //暂停
    public void Pause()
    {
        Time.timeScale = 0;
        items.SetActive(true);
    }
    public void Continue()
    {
        Time.timeScale = 1;
        items.SetActive(false);
    }
    public void Back()
    {
        Time.timeScale = 1;
        items.SetActive(false);
        SceneManager.LoadSceneAsync(0);//重新载入场景
    }
}
