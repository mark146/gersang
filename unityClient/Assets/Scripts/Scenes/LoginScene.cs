using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Scene 전환을 하기 위해 필요
using UnityEngine.UI;

// 참고: https://docs.microsoft.com/ko-kr/dotnet/csharp/programming-guide/classes-and-structs/using-properties
// JSON 사용법 참고: https://www.youtube.com/watch?v=faCEtCeiv0o
public class LoginScene : MonoBehaviour
{
    GameObject idField;
    GameObject passwordField;
    GameObject net;
    GameObject lodingView;
    GameObject alertView;
    User user;

    private void Awake()
    {
        //로그인 버튼에 이벤트 추가
        GameObject login = GameObject.Find("LoginButton").gameObject;
        login.GetComponent<Button>().onClick.AddListener(LoginActive);

        //회원가입 버튼에 이벤트 추가
        GameObject register = GameObject.Find("RegisterButton").gameObject;
        register.GetComponent<Button>().onClick.AddListener(RegisterActive);

        idField = GameObject.Find("ID");
        passwordField = GameObject.Find("Password");

        // test
        //net = GameObject.Find("@NetworkManager");
        //if (net == null)
        //{
        //    net = new GameObject { name = "@NetworkManager" };
        //    net.AddComponent<NetworkManager>();
        //}

        //// 네트워크 매니저 send 메소드 접근
        //network = net.GetComponent<NetworkManager>();
    }

    /*
        //// 받는다.
        //byte[] recvBuff = new byte[65535];
        //int recvBytes = socket.Receive(recvBuff);
        //string recvData = Encoding.Default.GetString(recvBuff, 0, recvBytes);
        //Debug.Log("[From Server]" + recvData);

        ////쫒아낸다.
        //socket.Shutdown(SocketShutdown.Both);
        //socket.Close();
        */
    public void LoginActive()
    {
        try
        {
            // 로딩뷰 프리팹을 찾은 후 읽어서 실행
            lodingView = Managers.Resource.Instantiate("LodingView");

            //NetworkManager 파일이 없다면 생성
            net = GameObject.Find("@NetworkManager");
            if (net == null)
            {
                net = new GameObject { name = "@NetworkManager" };
                net.AddComponent<NetworkManager>();
            }
  
            // 네트워크 매니저 send 메소드 접근
            NetworkManager network = net.GetComponent<NetworkManager>();
            string id = idField.GetComponent<InputField>().text;
            string password = passwordField.GetComponent<InputField>().text;

            user = new User();
            user.Id = id;
            user.Password = password;


            if (user.Id == "")
            {
                user.Id = "null";
            }

            if (user.Password == "")
            {
                user.Password = "null";
            }

            ///* register - 1 */
            //Dictionary<int, User> player = new Dictionary<int, User>();
            //player.Add(1, user);

            // 서버에 데이터 전송
            network.LoginSend(user);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void RegisterActive()
    {
        //씬 이동
        SceneManager.LoadScene("RegisterScene");
    }

    private void FixedUpdate()
    {
        List<Dictionary<int, object>> list = Queue.Instance.PopAll();// 꺼내옴
        foreach (Dictionary<int, object> result in list)
        {
            foreach (int Key in result.Keys)
            {
                //Debug.Log("LoginScene - Key: " + Key);
                //Debug.Log("LoginScene - result[Key]: " + result[Key]);

                //Newtonsoft.Json 라이브러리 사용해서 json 직렬화 처리
                string value = JsonConvert.SerializeObject(result[Key]);
                string value2 = JsonConvert.DeserializeObject<string>(value);
                //Debug.Log("LoginScene - value: " + value2);
                RecvRegister(value2.Trim());
            }
        }
    }

    public void RecvRegister(string result)
    {
        StartCoroutine(Delay(result));
    }

    IEnumerator Delay(string result)
    {
        yield return new WaitForSeconds(0.5f);
//        Debug.Log($"RecvRegister - result: {result}");

        if (result == "아이디가 존재하지 않습니다.")
        {
            // 알림창 프리팹을 찾은 후 읽어서 실행
            alertView = Managers.Resource.Instantiate("AlertView");

            //내용 변경
            Text title = GameObject.Find("Message").GetComponent<Text>();
            title.text = result;

            // 네트워크 매니저 제거
            NetworkManager network = net.GetComponent<NetworkManager>();
            network.Disconnect();
            GameObject.Destroy(net);

            //버튼 이벤트 추가
            GameObject ok = GameObject.Find("Ok Button").gameObject;
            ok.GetComponent<Button>().onClick.AddListener(() => {
                Managers.Resource.Destroy(alertView);
            }); ;

        }
        else if (result == "비밀번호가 일치하지 않습니다.")
        {
            // 알림창 프리팹을 찾은 후 읽어서 실행
            alertView = Managers.Resource.Instantiate("AlertView");

            //내용 변경
            Text title = GameObject.Find("Message").GetComponent<Text>();
            title.text = result;

            // 네트워크 매니저 제거
            NetworkManager network = net.GetComponent<NetworkManager>();
            network.Disconnect();
            GameObject.Destroy(net);

            //버튼 이벤트 추가
            GameObject ok = GameObject.Find("Ok Button").gameObject;
            ok.GetComponent<Button>().onClick.AddListener(() => {
                Managers.Resource.Destroy(alertView);
            }); ;

        }
        else if (result == "로그인에 성공하였습니다.")
        {
            //아이디 저장 후 다른 씬으로 이동
            NetworkManager network = net.GetComponent<NetworkManager>();
            network.Id = user.Id;

            DontDestroyOnLoad(net);
            SceneManager.LoadScene("CharacterSelectScene");
        }
        else {
            Debug.Log($"기타 버그");
        }

        // 프리팹 제거
        Managers.Resource.Destroy(lodingView); 
    }

    private void OnDestroy()
    {
        StopCoroutine("Delay");
        // Debug.Log($"LoginScene OnDestroy 실행");   
    }
}