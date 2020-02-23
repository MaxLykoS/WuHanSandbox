using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CitizenStatusType
{ 
    MovingToHosp,
    WaitingToHosp,
    Dying,
    Healthy,
    Latent,
    Ill
}
public class CitizenStatus : MonoBehaviour
{
    static readonly string DyingPath = "MaxLykoS/Sprites/Dying";
    static readonly string MovingPath = "MaxLykoS/Sprites/MoveingToHosp";
    static readonly string WaitingPath = "MaxLykoS/Sprites/WaitingToHosp";
    static readonly string HealthyPath = "doudou/Sprites/HealthyStatus";
    static readonly string LatentPath = "doudou/Sprites/LatentStatus";
    static readonly string IllPath = "doudou/Sprites/IllStatus";
    static readonly float PositionYOffset = 1.0f;

    static Transform Canvas;

    RectTransform statusRoot;
    GameObject statusPrefab;
    Dictionary<CitizenStatusType, GameObject> statusDict;
    Transform citizenTrans;
    public void Init(Transform citizen)
    {
        statusDict = new Dictionary<CitizenStatusType, GameObject>();
        statusPrefab = Resources.Load<GameObject>("MaxLykoS/Prefabs/Status/Status");
        citizenTrans = citizen;
        if(!Canvas)
            Canvas = GameObject.Find("Canvas").transform;
    }

    public void Hide()
    {
        if(statusRoot)
            statusRoot.gameObject.SetActive(false);
    }

    public void Display()
    {
        if(statusRoot)
            statusRoot.gameObject.SetActive(true);
    }

    public void AddStatus(CitizenStatusType type)
    {
        if (!statusRoot)
        {
            statusRoot = Instantiate<GameObject>(Resources.Load<GameObject>("MaxLykoS/Prefabs/Status/StatusBar"), Canvas).GetComponent<RectTransform>();
            statusRoot.position = Camera.main.WorldToScreenPoint(citizenTrans.position + new Vector3(0, PositionYOffset, 0));
            statusRoot.transform.SetAsFirstSibling();
        }
        if (statusDict.ContainsKey(type))
            return;
        statusDict[type] = Instantiate<GameObject>(statusPrefab, statusRoot);
        Image image = statusDict[type].GetComponent<Image>();
        string targetPath = DyingPath;
        switch (type)
        {
            case CitizenStatusType.Dying:targetPath = DyingPath;break;
            case CitizenStatusType.MovingToHosp:targetPath = MovingPath;break;
            case CitizenStatusType.WaitingToHosp:targetPath = WaitingPath;break;
            case CitizenStatusType.Healthy:targetPath = HealthyPath;break;
            case CitizenStatusType.Ill:targetPath = IllPath;break;
            case CitizenStatusType.Latent:targetPath = LatentPath;break;
        }
        image.sprite = Resources.Load<Sprite>(targetPath);
    }

    public void RemoveStatus(CitizenStatusType type)
    {
        GameObject status;
        if (statusDict.TryGetValue(type, out status))
        {
            statusDict.Remove(type);
            Destroy(status);
        }
    }

    public void ClearAll(bool bDestroy)
    {
        if (statusDict.Count == 0 && !statusRoot)
            return;
        foreach (CitizenStatusType type in System.Enum.GetValues(typeof(CitizenStatusType)))
        {
            RemoveStatus(type);
        }
        if(bDestroy)
            Destroy(statusRoot.gameObject);
    }

    private void OnGUI()
    {
        if (statusRoot)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(citizenTrans.position + new Vector3(0, PositionYOffset, 0));
            statusRoot.position = screenPos;
        }
    }
}
