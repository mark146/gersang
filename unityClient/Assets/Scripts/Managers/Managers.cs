using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 유니티 내 모든 오브젝트를 관리하는 클래스
public class Managers : MonoBehaviour
{
    static Managers s_instance; // 유일성이 보장

    // public static Managers Instance() { Init(); return s_instance; }// 유일한 매니저를 갖고 온다.
    static Managers Instance { get { Init(); return s_instance; } }// 유일한 매니저를 갖고 온다. null 이면 init() 함수 실행

    InputManager _input = new InputManager();
    ResourceManager _resource = new ResourceManager();

    public static InputManager Input { get { return Instance._input; } }
    public static ResourceManager Resource { get { return Instance._resource; } }

    void Start()
    {
        // 초기화
        Init();
    }

    void FixedUpdate()
    {
        // 마우스 or 키보드 이벤트 체크를 해줌. 이벤트가 발생하면 InputManager - OnUpdate() 로 이동
        _input.OnUpdate();
    }

    static void Init() {
        if (s_instance == null) {
            GameObject manager = GameObject.Find("@Managers");
            if (manager == null) {
                manager = new GameObject { name = "@Managers" }; // 게임 오브젝트 생성
                manager.AddComponent<Managers>();// Managers 스크립트 추가
            }

            DontDestroyOnLoad(manager);
            s_instance = manager.GetComponent<Managers>(); 
        }
    }
}