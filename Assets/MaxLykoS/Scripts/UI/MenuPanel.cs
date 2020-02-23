using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGame.UI
{
    public class MenuPanel : PanelBase
    {
        private List<GameObject> KruoUI;
        private Button btnStart;
        private Button btnOption;
        private Button btnAbout;
        private Button btnQuit;
        private Button btnMask;

        private GameObject optionPanel;
        private GameObject aboutPanel;
        private GameObject menuBtnPanel;
        public override void Init(params object[] args)
        {
            base.Init(args);

            skinPath = "MaxLykoS/Prefabs/UI/Panel/MenuPanel";//设置具体的Prefab路径
            layer = PanelLayer.Panel;  //定义Layer

            KruoUI = new List<GameObject>();
        }


        public override void OnShowing()//在这里查找赋值
        {
            base.OnShowing();

            Transform skinTrans = skin.transform;
            btnStart = skinTrans.Find("MenuBtnPanel/BtnStart").GetComponent<Button>();
            btnOption = skinTrans.Find("MenuBtnPanel/BtnHelp").GetComponent<Button>();
            btnAbout = skinTrans.Find("MenuBtnPanel/BtnAbout").GetComponent<Button>();
            btnQuit = skinTrans.Find("MenuBtnPanel/BtnQuit").GetComponent<Button>();
            btnMask = skinTrans.Find("BtnMask").GetComponent<Button>();
            menuBtnPanel = skinTrans.Find("MenuBtnPanel").gameObject;
            optionPanel = skinTrans.Find("OptionPanel").gameObject;
            aboutPanel = skinTrans.Find("AboutPanel").gameObject;

            KruoUI.Add(GameObject.Find("Canvas/Ability"));
            KruoUI.Add(GameObject.Find("Canvas/money"));
            KruoUI.Add(GameObject.Find("Canvas/加快时间"));
            KruoUI.Add(GameObject.Find("Canvas/减慢时间"));
            KruoUI.Add(GameObject.Find("Canvas/Pause"));
        }

        public override void OnShowed()
        {
            base.OnShowed();

            foreach (GameObject go in KruoUI)
            go.SetActive(false);
            optionPanel.SetActive(false);
            aboutPanel.SetActive(false);
            btnStart.onClick.AddListener(OnStartClick);
            btnOption.onClick.AddListener(OnOptionClick);
            btnAbout.onClick.AddListener(OnAboutClick);
            btnQuit.onClick.AddListener(OnQuitClick);
            btnMask.onClick.AddListener(OnMaskClick);

            Camera.main.GetComponent<CamerControl>().enabled = false;
            Camera.main.gameObject.AddComponent<CameraFilterPack_Blur_Movie>();
        }

        void OnStartClick()
        {
            foreach (GameObject go in KruoUI)
                go.SetActive(true);
            CitizenMgr.Instance.enabled = true;
            CitizenMgr.Instance.Init();
            PanelMgr.Instance.OpenPanel<MiniGame.UI.HospitalPanel>("");
            Camera.main.GetComponent<CamerControl>().enabled = true;
            Destroy(Camera.main.gameObject.GetComponent<CameraFilterPack_Blur_Movie>());
            PanelMgr.Instance.OpenPanel<Broadcast>("");
            GameManager._instance.isStart = true;
            Close();
        }

        void OnOptionClick()
        {
            menuBtnPanel.SetActive(false);
            optionPanel.SetActive(true);
            optionPanel.GetComponent<ValueOpthions>().GetVlues();
            aboutPanel.SetActive(false);
        }


        void OnAboutClick()
        {
            menuBtnPanel.SetActive(false);
            optionPanel.SetActive(false);
            aboutPanel.SetActive(true);
        }

        void OnQuitClick()
        {
            Application.Quit();
        }

        void OnMaskClick()
        {
            menuBtnPanel.SetActive(true);
            optionPanel.SetActive(false);
            aboutPanel.SetActive(false);
        }
    }
}
