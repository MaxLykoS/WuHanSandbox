using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniGame.UI;
using UnityEngine.UI;

namespace MiniGame.UI
{
    public class HospitalPanel : PanelBase
    {
        private Text TextDayCounter;
        private Text TextBedCounter;

        private Text TextHealthyCnt;
        private Text TextLatentCnt;
        private Text TextIllCnt;
        private Text TextInHospitalCnt;
        private Text TextCuredCnt;
        private Text TextDeadCnt;

        private bool bUpdate = false;

        #region 生命周期
        public override void Init(params object[] args)
        {
            base.Init(args);

            skinPath = "MaxLykoS/Prefabs/UI/Panel/HospitalPanel";//设置具体的Prefab路径
            layer = PanelLayer.Panel;  //定义Layer
        }


        public override void OnShowing()//在这里查找赋值
        {
            base.OnShowing();

            Transform skinTrans = skin.transform;
            TextDayCounter = skinTrans.Find("PanelStats/TextDayCounter").GetComponent<Text>();
            TextBedCounter = skinTrans.Find("PanelStats/TextBedCounter").GetComponent<Text>();

            TextHealthyCnt = skinTrans.Find("PanelStats/TextHealthyCnt").GetComponent<Text>();
            TextLatentCnt = skinTrans.Find("PanelStats/TextLatentCnt").GetComponent<Text>();
            TextIllCnt = skinTrans.Find("PanelStats/TextIllCnt").GetComponent<Text>();
            TextInHospitalCnt = skinTrans.Find("PanelStats/TextInHospitalCnt").GetComponent<Text>();
            TextCuredCnt = skinTrans.Find("PanelStats/TextCuredCnt").GetComponent<Text>();
            TextDeadCnt = skinTrans.Find("PanelStats/TextDeadCnt").GetComponent<Text>();
        }

        public override void OnShowed()
        {
            base.OnShowed();

            StartUpdate();
        }

        public override void OnClosing()
        {
            base.OnClosing();

            StopUpdate();
        }
        void StopUpdate()
        {
            bUpdate = false;
        }

        void StartUpdate()
        {
            bUpdate = true;
            StartCoroutine(UpdateCounter());
        }
        #endregion
        bool bFirstInfected = false;
        bool bHalfInfected = false;
        IEnumerator UpdateCounter()
        {
            while (bUpdate)
            {
                //update stats
                TextDayCounter.text = (((int)CitizenMgr.DaysCnt)/5).ToString();
                TextBedCounter.text = CitizenMgr.CurBedsCnt.ToString() + "/" + CitizenMgr.BedsCntTotal.ToString() + " + " + CitizenMgr.WaitingCnt.ToString();

                TextHealthyCnt.text = CitizenMgr.HealthyCnt.ToString();
                TextLatentCnt.text = CitizenMgr.LatentCnt.ToString();
                TextIllCnt.text = CitizenMgr.IllCnt.ToString();
                TextInHospitalCnt.text = CitizenMgr.InHospitalCnt.ToString();
                TextCuredCnt.text = CitizenMgr.CuredCnt.ToString();
                TextDeadCnt.text = CitizenMgr.DeadCnt.ToString();

                if (!bFirstInfected && CitizenMgr.IllCnt >= 1)
                {
                    bFirstInfected = true;
                    Broadcast.Instance.AddString("发现了首批感染者。", Color.yellow);
                }
                if (!bHalfInfected && CitizenMgr.LatentCnt + CitizenMgr.IllCnt > CitizenMgr.Population / 2)
                {
                    bHalfInfected = true;
                    Broadcast.Instance.AddString("感染的形式不容乐观。", Color.red);
                }

                if (CitizenMgr.LatentCnt + CitizenMgr.IllCnt + CitizenMgr.InHospitalCnt == 0)
                {
                    StopUpdate();
                    PanelMgr.Instance.OpenPanel<TipGameEnding>("", "游戏结束");
                    Broadcast.Instance.AddString("我们成功的战胜了这次疫情。", Color.green);    
                }

                yield return new WaitForFixedUpdate();
            }
            yield return 0;
        }
    }
}