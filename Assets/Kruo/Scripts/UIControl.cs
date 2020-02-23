using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIControl : MonoBehaviour
{
    //这个类用于修改UI相关    
    public Text money;//金钱
    static PointerEventData pointerEventData;
    static GraphicRaycaster gr;
    GameObject canvas;
    void Start()
    {
        pointerEventData = new PointerEventData(EventSystem.current);
        canvas = this.gameObject;
        gr = canvas.GetComponent<GraphicRaycaster>();

    }

    void Update()
    {
        money.text =  GameManager._instance.money.ToString();
       
    }
    static public bool GetOverUI()//判断鼠标是否在UI上
    {
        pointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(pointerEventData, results);
        if (results.Count != 0)
        {
            Debug.Log(results[0]);
            return true;
        }
        else
        {
            //Debug.Log("false");
            return false;
        }

    }
}
