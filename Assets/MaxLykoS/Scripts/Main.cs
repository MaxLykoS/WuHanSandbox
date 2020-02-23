using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    void Start()
    {
        CitizenMgr.Instance.enabled = false;
        PanelMgr.Instance.OpenPanel<MiniGame.UI.MenuPanel>("");
    }
}
