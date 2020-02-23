using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButterEnter : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    public GameObject Tip;
    public string introduction="no introduction";

    public void OnPointerEnter(PointerEventData eventData)
    {
        Tip.GetComponent<RectTransform>().position = gameObject.GetComponent<RectTransform>().position;
        Tip.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = introduction;
        Tip.SetActive(true);
    }
    //鼠标离开时关闭动画
    public void OnPointerExit(PointerEventData eventData)
    {
        Tip.SetActive(false);
    }

}
