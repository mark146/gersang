using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCamera : MonoBehaviour
{
    //카메라 구현 - 참고: https://www.youtube.com/watch?v=pxS14VJ_eXQ
    float spped;
    float zoomSpeed;
    float rotateSpeed;

    float maxHeight = 20f;
    float minHeight = 4f;
  
    void Start()
    {

    }


    void Update()
    {

        // 카메라 제어
        CameraControl();
    }

    
    void CameraControl()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            spped = 0.06f;
            zoomSpeed = 20.0f;
        }
        else
        {
            spped = 0.035f;
            zoomSpeed = 10.0f;
        }

        float hsp = transform.position.y * spped * Input.GetAxis("Horizontal");
        float vsp = transform.position.y * spped * Input.GetAxis("Vertical");
        float scrollSp = -zoomSpeed * Input.GetAxis("Mouse ScrollWheel");

        if ((transform.position.y >= maxHeight) && (scrollSp > 0))
        {
            scrollSp = 0;
        }
        else if ((transform.position.y <= minHeight) && (scrollSp < 0))
        {
            scrollSp = 0;
        }

        Vector3 verticalMove = new Vector3(0, scrollSp, 0);
        Vector3 lateralMove = hsp * transform.right;
        Vector3 forwardMove = transform.forward;
        forwardMove.y = 0;
        forwardMove.Normalize();
        forwardMove *= vsp;

        Vector3 move = verticalMove + lateralMove + forwardMove;

        transform.position += move;
    }
}