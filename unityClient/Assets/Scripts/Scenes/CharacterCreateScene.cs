using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// UI 수정
public class CharacterCreateScene : MonoBehaviour {
    InputField nickname;
    GameObject detail;
    GameObject characterInfoImage;
    GameObject characterMale;
    GameObject characterFemale;
    GameObject networkManager;
    GameObject nicknameInputField;
    NetworkManager network;
    bool isname = false;

    void Start() {
        //이벤트 추가
        GameObject back = GameObject.Find("BackButton").gameObject;
        back.GetComponent<Button>().onClick.AddListener(BackAction);

        GameObject create = GameObject.Find("CreateButton").gameObject;
        create.GetComponent<Button>().onClick.AddListener(CreateAction);


        GameObject character_v1 = GameObject.Find("Character_v1").gameObject;
        character_v1.GetComponent<Button>().onClick.AddListener(V1Action);

        GameObject character_v2 = GameObject.Find("Character_v2").gameObject;
        character_v2.GetComponent<Button>().onClick.AddListener(V2Action);

        // 참고: https://docs.unity3d.com/kr/530/ScriptReference/UI.InputField-onEndEdit.html
        // 플레이어가 기본 입력 필드 편집을 마치면 "LockInput"메서드를 호출하는 리스너를 추가합니다.
        // "LockInput"이 호출 될 때 기본 입력 필드를 메소드에 전달
        nickname = GameObject.Find("NicknameInputField").GetComponent<InputField>();
        nickname.onValueChanged.AddListener(delegate { NicknameInput(nickname); });

        nicknameInputField = GameObject.Find("NicknameInputField");

        detail = GameObject.Find("CharacterDetail").gameObject;
        characterInfoImage = GameObject.Find("CharacterInfoImage").gameObject;

       
        networkManager = GameObject.Find("@NetworkManager");
        network = networkManager.GetComponent<NetworkManager>();

        //처음 입장시 남자 주인공 자동 클릭
        V1Action();
    }

    private void LateUpdate()
    {
        List<Dictionary<int, object>> list = Queue.Instance.PopAll();// 꺼내옴
        foreach (Dictionary<int, object> result in list)
        {
            foreach (int Key in result.Keys)
            {
                object value = result[Key];
                // Debug.Log($"Key: {Key} value: {value}");
                string recv = value as string;
                if (recv.Equals("캐릭터 생성이 완료되었습니다.") == true) {
                    SceneManager.LoadScene("CharacterSelectScene");
                } else {
                    StartCoroutine(Delay(recv));
                }
            }
        }
    
    }

    IEnumerator Delay(string result) {
        yield return new WaitForSeconds(0.1f);

        Text name = GameObject.Find("NameResult").GetComponent<Text>();

        //if (result == "사용가능한 닉네임 입니다.") {
        //    name.text = result;
        //    isname = true;
        //} else if (result == "존재하는 닉네임 입니다.") {
        //    name.text = result;
        //    isname = false;
        //}

        switch (result) {
            case "사용가능한 닉네임 입니다.":
                name.text = result;
                isname = true;
                break;
            case "존재하는 닉네임 입니다.":
                name.text = result;
                isname = false;
                break;
            default:
                break;
        }
    }

    private void V1Action() {
        Debug.Log("V1Action 클릭");
        detail.GetComponent<Text>().text = "남자 캐릭터";

        if (characterMale == null && characterFemale == null) {
            characterMale = Managers.Resource.Instantiate("CharacterMale");
            characterMale.transform.SetParent(characterInfoImage.transform);
            characterMale.SetActive(true);
            RectTransform rt = (RectTransform)characterMale.transform;
            rt.anchoredPosition = new Vector3(0f, 99f);
        } else if (characterMale == null && characterFemale != null) {
            Managers.Resource.Destroy(characterFemale);
            characterMale = Managers.Resource.Instantiate("CharacterMale");
            characterMale.transform.SetParent(characterInfoImage.transform);
            characterMale.SetActive(true);
            RectTransform rt = (RectTransform)characterMale.transform;
            rt.anchoredPosition = new Vector3(0f, 99f);
        }
    }

    private void V2Action()
    {
        Debug.Log("V2Action 클릭");
        detail.GetComponent<Text>().text = "여자 캐릭터";

        if (characterFemale == null && characterMale == null) {
            characterFemale = Managers.Resource.Instantiate("CharacterFemale");
            characterFemale.transform.SetParent(characterInfoImage.transform);
            characterFemale.SetActive(true);
            RectTransform rt = (RectTransform)characterFemale.transform;
            rt.anchoredPosition = new Vector3(0f, 99f);
        } else if (characterMale != null && characterFemale == null) {
            Managers.Resource.Destroy(characterMale);
            characterFemale = Managers.Resource.Instantiate("CharacterFemale");
            characterFemale.transform.SetParent(characterInfoImage.transform);
            characterFemale.SetActive(true);
            RectTransform rt = (RectTransform)characterFemale.transform;
            rt.anchoredPosition = new Vector3(0f, 99f);
        }
    }

    void BackAction()
    {
        Debug.Log("뒤로가기 클릭");
        SceneManager.LoadScene("CharacterSelectScene");
    }

    void CreateAction() {
        Debug.Log("캐릭터 생성 클릭");

        /*
         0: 플레이어 아이디
         1: 캐릭터 이름
         2: 캐릭터 성별
        */
        List<string> playerInfoList = new List<string>();
        playerInfoList.Add(network.Id.Trim()); 

        if (characterMale != null && characterFemale == null && isname == true) {
            //Debug.Log("남자 캐릭터");
            //Debug.Log($"name: {nicknameInputField.transform.GetChild(2).GetComponent<Text>().text}");
            playerInfoList.Add(nicknameInputField.transform.GetChild(2).GetComponent<Text>().text);
            playerInfoList.Add("남자 캐릭터");

            network.CharacterCreate(playerInfoList);
        } else if (characterMale == null && characterFemale != null && isname == true) {
            //Debug.Log("여자 캐릭터");
            //Debug.Log($"nickname: {nicknameInputField.transform.GetChild(2).GetComponent<Text>().text}");
            playerInfoList.Add(nicknameInputField.transform.GetChild(2).GetComponent<Text>().text);
            playerInfoList.Add("여자 캐릭터");

            network.CharacterCreate(playerInfoList);
        } else { 
            //알림창 출력
            Debug.Log("다시 확인 요청");
        }
    }

    // 닉네임 검증하는 기능
    void NicknameInput(InputField input) {
        int Length = input.text.Length;
        switch (Length) {
            case 0:
                Debug.Log("input.text.Length == 0 경우");
                break;
            default:
                //Debug.Log($"input.text.Length > 0 경우: {input.text}");
                network.NickNameVerification(input.text.Trim());
                break;
        }
    }
}