using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueOpthions : MonoBehaviour
{

    public InputField[] inputFields;
    public GameObject OptionPanal;
    public GameObject MenuPanal;
    // Start is called before the first frame update
    void Start()
    {

    }
    

    public void GetVlues()
    {
        inputFields[0].text = CitizenMgr.Population.ToString();
        inputFields[1].text = CitizenMgr.InitialInfectedCnt.ToString();
        inputFields[2].text = CitizenMgr.BedsCntTotal.ToString();
        inputFields[3].text = CitizenMgr.MaxInfectRate.ToString();
        inputFields[4].text = CitizenMgr.DeathRate.ToString();
        inputFields[5].text = GameManager._instance.money.ToString();
        inputFields[6].text = GameManager._instance.moneyImproveRate.ToString();
        inputFields[7].text = GameManager._instance.areaTime.ToString();
        inputFields[8].text = GameManager._instance.spacersCoolingTime.ToString();
        inputFields[9].text = GameManager._instance.CameraMovingSpeed.ToString();
    }
    public void SetVlues()
    {
        CitizenMgr.Population = Mathf.Clamp(int.Parse(inputFields[0].text),0,500);
        CitizenMgr.InitialInfectedCnt = Mathf.Clamp(int.Parse(inputFields[1].text), 0, CitizenMgr.Population);
        CitizenMgr.BedsCntTotal = Mathf.Clamp(int.Parse(inputFields[2].text),0,500);
        CitizenMgr.MaxInfectRate = Mathf.Clamp(float.Parse(inputFields[3].text),0,1);
        CitizenMgr.DeathRate = Mathf.Clamp(float.Parse(inputFields[4].text),0,1);
        GameManager._instance.money = Mathf.Clamp(int.Parse(inputFields[5].text),0,10000000);
        GameManager._instance.moneyImproveRate = Mathf.Clamp(int.Parse(inputFields[6].text),0,1000);
        GameManager._instance.areaTime = Mathf.Clamp(int.Parse(inputFields[7].text),0,10000) ;
        GameManager._instance.spacersCoolingTime = Mathf.Clamp(int.Parse(inputFields[8].text ),0,10000);
        GameManager._instance.CameraMovingSpeed = Mathf.Clamp(int.Parse(inputFields[9].text),0,1000);
        OptionPanal.SetActive(false);
        MenuPanal.SetActive(true);
    }
}
