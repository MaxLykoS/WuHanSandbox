using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniGame.UI;

public class PanelMgr : Singletion<PanelMgr>   //单例
{
    //画板
    private GameObject canvas;


    //各个面板
    public Dictionary<string, PanelBase> dict;


    //各个层级
    private Dictionary<PanelLayer, Transform> layerDict;


    //开始
    public void Awake()
    {
        InitLayer();
        dict = new Dictionary<string, PanelBase>();
    }


    //初始化层
    private void InitLayer()
    {
        //画布
        canvas = GameObject.Find("Canvas");
        if (canvas == null)
            Debug.LogError("PanelMgr.InitLayer fail,canvas is null");


        //各个层级
        layerDict = new Dictionary<PanelLayer, Transform>();


        foreach (PanelLayer pl in Enum.GetValues(typeof(PanelLayer))) // Canvas/Panel和Canvas/Tip 找到这两个物体，让UI挂在下面
        {
            string name = pl.ToString();
            Transform transform = canvas.transform.Find(name);
            layerDict.Add(pl, transform);
        }
    }


    //打开面板
    public void OpenPanel<T>(string skinPath, params object[] args) where T : PanelBase   //T必须是PanelBase的子类
    {
        //已经打开
        string name = typeof(T).ToString();
        if (dict.ContainsKey(name))
            return;


        //面板脚本
        PanelBase panel = canvas.AddComponent<T>();  //把脚本挂载Canvas下
        panel.Init(args);    //生命周期
        dict.Add(name, panel);
        //加载皮肤
        skinPath = skinPath != "" ? skinPath : panel.skinPath;   //skinPath也就是该Prefab的路径
        GameObject skin = Resources.Load<GameObject>(skinPath);
        if (skinPath == null)
            Debug.LogError("panelMgr.OpenPanel fail,skin is null,skinPath = " + skinPath);
        panel.skin = Instantiate(skin);//加载出来


        //坐标
        Transform skintrans = panel.skin.transform;   //得到该界面的Layer，然后从layerDict中找到对应的Layer的Transform，赋值过去成为子物体
        PanelLayer layer = panel.layer;
        Transform parent = layerDict[layer];
        skintrans.SetParent(parent, false); //层级


        //panel的生命周期
        panel.OnShowing();   //预留的面板动画
        //anm
        panel.OnShowed();   //加载结束时
    }


    //关闭面板
    //注意，name是该UI类的反射名，注意前面的命名空间，嵌套类记得写+号！
    public void ClosePanel(string name)
    {
        PanelBase panel;
        if (dict.ContainsKey(name))
        {
            panel = dict[name];
        }
        else
            return;


        panel.OnClosing();
        dict.Remove(name);
        panel.OnClosed();
        Destroy(panel.skin);
        Destroy(panel);
    }
    //这里有必要补充一下，假如是UI.Panel命名空间下的LobbyPanel类中的嵌套类AchieveTip，则应该调用ClosePanel("UI.Panel.LobbyPanel+AchieveTip");
}