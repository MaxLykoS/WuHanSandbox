using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame.UI
{
    public enum PanelLayer
    {
        Panel,        //面板
        Tip,         //提示
    }
    public class PanelBase : MonoBehaviour
    {
        //皮肤路径
        public string skinPath;
        //皮肤
        public GameObject skin;
        //层级
        public PanelLayer layer;
        //面板参数
        public object[] args;


        #region 生命周期
        public virtual void Init(params object[] args)   //自定义参数
        {
            this.args = args;
        }


        //开始面板前
        public virtual void OnShowing() { }


        //显示面板后
        public virtual void OnShowed() { }


        //帧更新
        public virtual void Update() { }


        //关闭前
        public virtual void OnClosing() { }


        //关闭后
        public virtual void OnClosed() { }
        #endregion


        #region 操作
        protected virtual void Close()
        {
            string name = this.GetType().ToString();   //反射
            PanelMgr.Instance.ClosePanel(name);
        }
        #endregion
    }
}