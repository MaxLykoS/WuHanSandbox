using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MiniGame.UI;

public class Broadcast : PanelBase
{
    private static int MessageCntsTotal = 10;
    private static int CurMessageCnts;
    private static float ComboPeriod = 2.0f;
    private static Broadcast _instance;
    public static Broadcast Instance
    {
        get 
        {
            if (_instance == null)
            {
                PanelMgr.Instance.OpenPanel<Broadcast>("");   
            }
            return _instance;
        }
    }

    GameObject textPrefab;
    Transform messageContainer;

    public override void Init(params object[] args)
    {
        base.Init(args);

        skinPath = "MaxLykoS/Prefabs/UI/Panel/BroadcastPanel";//设置具体的Prefab路径
        layer = PanelLayer.Panel;  //定义Layer
        _instance = this;
        CurMessageCnts = 0;
        comboCnt = 0;
        bComboing = false;
        timer = 0;
    }

    public override void OnShowing()//在这里查找赋值
    {
        base.OnShowing();

        Transform skinTrans = skin.transform;
        textPrefab = skinTrans.Find("TextPrefab").gameObject;
        messageContainer = skinTrans.Find("MessagePanel");
    }

    public override void OnShowed()
    {
        base.OnShowed();

        textPrefab.SetActive(false);
        AddString("ZZTV开始为您播报！",Color.white);
    }

    public override void OnClosing()
    {
        base.OnClosing();

        _instance = null;
    }

    string curString;
    int comboCnt;
    public void AddString(string s, Color color)
    {
        if (curString != s || !bComboing)
        {
            Text newMessage = Instantiate<GameObject>(textPrefab, messageContainer).GetComponent<Text>();
            newMessage.transform.SetAsLastSibling();
            if (++CurMessageCnts > MessageCntsTotal)
            {
                DestroyImmediate(messageContainer.GetChild(0).gameObject);
                CurMessageCnts--;
            }
            newMessage.text = s;
            newMessage.color = color;
            newMessage.gameObject.SetActive(true);
            comboCnt = 1;
        }
        else if (curString == s && bComboing)
        {
            comboCnt++;
            Text curMessage = messageContainer.GetChild(messageContainer.childCount - 1).gameObject.GetComponent<Text>();
            int spaceIndex = curMessage.text.IndexOf(" ");
            if (spaceIndex>=0)
                curMessage.text = curMessage.text.Substring(0, spaceIndex);
            curMessage.text += comboCnt >= 2 ? " X" + comboCnt.ToString() : "";
        }
        StopAllCoroutines();
        StartCoroutine(IUpdateCombo());
        curString = s;
    }

    bool bComboing;
    float timer;
    IEnumerator IUpdateCombo()
    {
        bComboing = true;
        timer = 0;
        while (bComboing&&timer < ComboPeriod)
        {
            timer += Time.deltaTime;

            yield return 0;
        }
        bComboing = false;
    }
}
