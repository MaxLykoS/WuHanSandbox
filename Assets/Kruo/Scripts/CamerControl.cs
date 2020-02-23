using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerControl : MonoBehaviour
{
    GameObject myCamera;    
    Vector2 mousePosition;
    float screen_X;
    float screen_Y;
    public float Offset_X;
    public float Offset_Y;
    public float zoomSpeed;//缩放速度
    float currentZoom;

    void Start()
    {
        myCamera = this.gameObject;
        screen_X = Screen.width;
        screen_Y = Screen.height;
    }

    void Update()
    {
        mousePosition = Input.mousePosition;
        //前后左右移动
        if(mousePosition.x > screen_X - Offset_X)
        {
            myCamera.transform.Translate(new Vector3(0, 0, GameManager._instance.CameraMovingSpeed*0.1f),Space.World);
        }
        if (mousePosition.x <0 + Offset_X)
        {
            myCamera.transform.Translate(new Vector3(0, 0, -1* GameManager._instance.CameraMovingSpeed*0.1f), Space.World);
        }
        if (mousePosition.y > screen_Y - Offset_Y)
        {
            myCamera.transform.Translate(new Vector3(-1 * GameManager._instance.CameraMovingSpeed*0.1f, 0,0), Space.World);
        }
        if (mousePosition.y < 0 + Offset_Y)
        {
            myCamera.transform.Translate(new Vector3(GameManager._instance.CameraMovingSpeed*0.1f, 0, 0), Space.World);
        }
        //摄像机fov        
        currentZoom = Input.mouseScrollDelta.y * zoomSpeed  * -1;
        myCamera.GetComponent<Camera>().fieldOfView = Mathf.Clamp((myCamera.GetComponent<Camera>().fieldOfView + currentZoom),40,80);
        
    }
}
