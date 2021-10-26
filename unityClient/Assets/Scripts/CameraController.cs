using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어 카메라 뷰 설정 클래스
public class CameraController : MonoBehaviour
{
    [SerializeField]
    Define.CameraMode _mode = Define.CameraMode.QuarterView;

    // 플레이어 시점
    [SerializeField]
    Vector3 _delta = new Vector3(0.0f, 6.0f, -5.0f);

    [SerializeField]
    GameObject _player = null;

    public void SetPlayer(GameObject player) { _player = player; }

    void LateUpdate()
    {
        if (_mode == Define.CameraMode.QuarterView)
        {

            //플레이어가 죽을 경우
            //if (_player.isVaild() == false)
            //{
            //    return;
            //}

            // 장애물에 카메라가 가려질 경우 카메라 위치 이동
            // 플레이어 위치, 카메라 위치
            RaycastHit hit;
            // LayerMask.GetMask("Block")
            if (Physics.Raycast(_player.transform.position, _delta, out hit, _delta.magnitude, 1 << (int)Define.Layer.Wall))
            {
                float dist = (hit.point - _player.transform.position).magnitude * 0.8f;
                transform.position = _player.transform.position + _delta.normalized * dist;
            }
            else
            {
                // 캐릭터 따라 카메라 이동
                transform.position = _player.transform.position + _delta;
                transform.LookAt(_player.transform);
            }
        }
    }

    public void SetQuaterView(Vector3 delta)
    {
        _mode = Define.CameraMode.QuarterView;
        _delta = delta;
    }
}
