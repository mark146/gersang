using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


/*
 게임 컨트롤 규모가 커지면 플레이어 컨트롤러에 넣을 수 없음 따라서 매니저 클래스를 만들어서 따로 관리해줘야함
 장점
 루프 마다 한번씩 체크 후 전파
 */
public class InputManager
{
    // InputManager 가 입력 체크하는데 입력이 있다면? 이벤트로 전파 해주는 형식(리스너 패턴)
    // 장점: 키보드 입력 역할 분할
    public Action KeyAction = null;
    public Action<Define.MouseEvent> MouseAction = null;

    //클릭 상태 저장
    bool _pressed = false;
    float _pressedTime = 0;

    // 마우스 드래그 상태도 여기에 추가


    public void OnUpdate()
    {

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // 키보드 이벤트 처리
        if (Input.anyKey && KeyAction != null)
        {
            KeyAction.Invoke();
        }

        // 1. 마우스 이벤트 처리
        if (MouseAction != null)
        {
            // 왼쪽 클릭은 0번
            if (Input.GetMouseButton(0))
            {
                if (!_pressed)
                {
                    MouseAction.Invoke(Define.MouseEvent.PointerDown);
                    _pressedTime = Time.time; // 유니티에서 관리하는 시간
                }

                MouseAction.Invoke(Define.MouseEvent.Press);
                _pressed = true;
            }
            else
            {
                if (_pressed)
                {

                    // 현재시간이 누른 시간 기준으로 클릭시 인정
                    if (Time.time < _pressedTime * 0.2f)
                    {
                        MouseAction.Invoke(Define.MouseEvent.Press);

                    }
                    MouseAction.Invoke(Define.MouseEvent.PointerUp);
                }
                _pressed = false;
                _pressedTime = 0; // 시간 초기화
            }
        }
    }

    // 씬 이동시 이전 정보 제거 목적
    public void Clear()
    {
        KeyAction = null;
        MouseAction = null;
    }
}
