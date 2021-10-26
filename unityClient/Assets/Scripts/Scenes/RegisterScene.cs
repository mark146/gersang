using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class RegisterScene : MonoBehaviour
{
    public InputField id;
    public InputField passowrd;
    public InputField passwordConfirm;
    object _lock = new object();
    static bool isId = false;
    static bool isPassword = false;
    static bool isPasswordConfirm = false;
    GameObject register;

    //매개변수: 프로토콜id, 어떤 작업을 할 것인가?
    Dictionary<int, Action<object>> _handler = new Dictionary<int, Action<object>>();

    public RegisterScene()
    {
        Register();
    }

    public void Register()
    {
        //매개변수: 프로토콜ID, 무엇을 할 것인가?
        _handler.Add(2, RegisterRecv);
        _handler.Add(3, RegisterRecv);
    }

    private void Awake()
    {
        // 이벤트 추가
        GameObject back = GameObject.Find("BackButton").gameObject;
        back.GetComponent<Button>().onClick.AddListener(BackAction);

        register = GameObject.Find("RegisterButton").gameObject;
        register.GetComponent<Button>().onClick.AddListener(RegisterAction);
        // register.GetComponent<Button>().interactable = false; // 버튼 클릭을 비활성
        //btn.interactable = true; // 버튼 클릭을 

        // 참고: https://docs.unity3d.com/kr/530/ScriptReference/UI.InputField-onEndEdit.html
        // 플레이어가 기본 입력 필드 편집을 마치면 "LockInput"메서드를 호출하는 리스너를 추가합니다.
        // "LockInput"이 호출 될 때 기본 입력 필드를 메소드에 전달
        id = GameObject.Find("IdInputField").GetComponent<InputField>();
        id.onValueChanged.AddListener(delegate { IdInput(id); });

        passowrd = GameObject.Find("PasswordInputField").GetComponent<InputField>();
        passowrd.onValueChanged.AddListener(delegate { PasswordInput(passowrd); });

        passwordConfirm = GameObject.Find("PasswordConfirmInputField").GetComponent<InputField>();
        passwordConfirm.onValueChanged.AddListener(delegate { PasswordConfirmInput(passwordConfirm); });

        //NetworkManager 파일이 없다면 생성
        GameObject go = GameObject.Find("@NetworkManager");
        if (go == null)
        {
            go = new GameObject { name = "@NetworkManager" };
            go.AddComponent<NetworkManager>();
        }
    }

    private void LateUpdate()
    {
        //Debug.Log($"isId: {isId} - isPassword: {isPassword} - isPasswordConfirm: {isPasswordConfirm}");
        if(isId == true && isPassword == true && isPasswordConfirm == true)
        {
            register = GameObject.Find("RegisterButton").gameObject;
            register.GetComponent<Button>().interactable = true; // 버튼 클릭을 활성화
        }
        else
        {
            register = GameObject.Find("RegisterButton").gameObject;
            register.GetComponent<Button>().interactable = false; // 버튼 클릭을 비활성
        }

        List<Dictionary<int, object>> list = Queue.Instance.PopAll();// 꺼내옴
        foreach (Dictionary<int, object> result in list)
        {
            foreach (int Key in result.Keys)
            {
                //Debug.Log("Key: " + Key);
                object value = result[Key];
                _handler[Key].Invoke(value);
            }
        }
    }

    private void RegisterAction()
    {
        //Debug.Log("등록 버튼 클릭");
        //Debug.Log($"id: {id.text} password: {passowrd.text}");

        User register = new User();
        register.Id = id.text;
        register.Password = passowrd.text;

        if (register.Id == "")
        {
            register.Id = "null";
        }

        if (register.Password == "")
        {
            register.Password = "null";
        }

        // 네트워크 매니저 send 메소드 접근
        GameObject go = GameObject.Find("@NetworkManager");
        NetworkManager network = go.GetComponent<NetworkManager>();

        Dictionary<int, User> player = new Dictionary<int, User>();
        player.Add(3, register);

        // 서버에 데이터 전송
        network.RegisterSend(player);
    }

    private void BackAction()
    {
        //Debug.Log("뒤로가기 클릭");
        SceneManager.LoadScene("LoginScene");
    }

    public void RegisterRecv(object result)
    {
        string value = result.ToString();
        //Debug.Log("RegisterScene - result: " + value);
        if(value == "회원가입이 완료되었습니다.")
        {
            //소켓 연결 닫기 후 로그인 화면으로 이동
            GameObject.Destroy(GameObject.Find("@NetworkManager"));
            SceneManager.LoadScene("LoginScene");

        } else
        {

            StartCoroutine(Delay(value));
        }
    }

    IEnumerator Delay(string result)
    {
        yield return new WaitForSeconds(0.1f);

        lock (_lock) {
            Text name = GameObject.Find("IdResult").GetComponent<Text>();
            if (result == "사용가능한 닉네임 입니다.")
            {
                name.text = result;
                isId = true;
            }
            else if (result == "존재하는 닉네임 입니다.")
            {
                name.text = result;
                isId = false;
            }
            else
            {
                isId = false;
                name.text = "";
            }
        }
    }


    #region 아이디,비밀번호 검증
    void IdInput(InputField input)
    {
        if (input.text.Length > 0)
        {
            //Debug.Log("IdInput: " + input.text);

            // 네트워크 매니저 send 메소드 접근
            GameObject go = GameObject.Find("@NetworkManager");
            NetworkManager network = go.GetComponent<NetworkManager>();

            Dictionary<int, string> player = new Dictionary<int, string>();
            player.Add(2, input.text);

            // 서버에 데이터 전송
            network.Verification(player);
        }
        else if (input.text.Length == 0)
        {
            Text id = GameObject.Find("IdResult").GetComponent<Text>();
            id.text = "";
            //Debug.Log("Main Input Empty");
            isId = false;
        }
    }

    void PasswordInput(InputField input)
    {
        Text password = GameObject.Find("PasswordResult").GetComponent<Text>();
        if (input.text.Length > 0)
        {
            Regex regex = new Regex("^(?=.*[a-zA-Z])(?=.*[0-9]).{9,15}$");
            Match m = regex.Match(input.text);
            if (m.Success)
            {
                // Debug.Log(string.Format("비밀번호 일치 - {0}:{1}", m.Index, m.Value));
                password.text = "사용가능한 비밀번호 입니다.";
                //    Debug.Log("비밀번호는 9~15 글자 입력하세요.");
                isPassword = true;
            }
            else
            {
                // Debug.Log(string.Format(@"미일치 - {0} does not match with Regex(""{1}"")", input.text, regex.ToString()));
                password.text = "비밀번호는 9~15 글자 입력하세요.";
                //    Debug.Log("비밀번호는 9~15 글자 입력하세요.");
                isPassword = false;
            }
        }
        else if (input.text.Length == 0)
        {
            password.text = "";
            //  Debug.Log("Main Input Empty");
            isPasswordConfirm = false;
        }
    }

    void PasswordConfirmInput(InputField input)
    {
        Text passwordConfirm = GameObject.Find("PasswordConfirmResult").GetComponent<Text>();
        if (input.text.Length > 0)
        {
            Regex regex = new Regex(passowrd.text);
            Match m = regex.Match(input.text);
            if (m.Success)
            {
                //Debug.Log(string.Format("비밀번호 일치 - {0}:{1}", m.Index, m.Value));
                passwordConfirm.text = "비밀번호 일치";
                //   Debug.Log("비밀번호 일치");
                isPasswordConfirm = true;
            }
            else
            {
                // Debug.Log(string.Format(@"미일치 - {0} does not match with Regex(""{1}"")", input.text, regex.ToString()));
                passwordConfirm.text = "비밀번호 미일치";
                //    Debug.Log("비밀번호 미일치");
                isPasswordConfirm = false;
            }
        }
        else if (input.text.Length == 0)
        {
            passwordConfirm.text = "";
            //  Debug.Log("Main Input Empty");
            isPasswordConfirm = false;
        }
    }
    #endregion
}