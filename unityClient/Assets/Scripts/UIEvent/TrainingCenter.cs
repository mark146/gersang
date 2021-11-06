using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingCenter : MonoBehaviour {
    public List<string> selectList { get; set; }
    NetworkManager network;
    GameObject TrainingSchool;
    List<string> gladiatorList;
    List<string> wizardList;
    GameObject Employment_Slot_1;
    GameObject Employment_Slot_2;

    void Start() {
        selectList = new List<string>();
        gladiatorList = new List<string>();
        wizardList = new List<string>();

        // 네트워크 통신을 위해 NetworkManager 객체 검색 후 생성
        GameObject networkManager = GameObject.Find("@NetworkManager");
        network = networkManager.GetComponent<NetworkManager>();

        // 게임 오브젝트를 검색 및 객체화 한 다음 버튼 이벤트를 추가
        GameObject Employment = GameObject.Find("Employment_Button");
        Employment.GetComponent<Button>().onClick.AddListener(Employment_Event);

        GameObject Fire = GameObject.Find("Fire_Button");
        Fire.GetComponent<Button>().onClick.AddListener(Fire_Event);

        GameObject Buy = GameObject.Find("Buy_Button");
        Buy.GetComponent<Button>().onClick.AddListener(Buy_Event);

        // 용병을 누를 경우
        Employment_Slot_1 = GameObject.Find("Employment_Slot_1");
        Employment_Slot_1.GetComponent<Button>().onClick.AddListener(Employment_Slot_1_Event);

        Employment_Slot_2 = GameObject.Find("Employment_Slot_2");
        Employment_Slot_2.GetComponent<Button>().onClick.AddListener(Employment_Slot_2_Event);

        // 창을 종료할 경우
        GameObject Training_Close = GameObject.Find("TrainingClose");
        Training_Close.GetComponent<Button>().onClick.AddListener(TrainingClose);

        // 훈련소 창 화면 정보를 가져와서 생성
        TrainingSchool = GameObject.Find("Training_school(Clone)");
        // Debug.Log($"TrainingSchool: {TrainingSchool}");

        // 용병 정보 저장(gladiatorList, wizardList)
        /**
         DB 정보: PlayerID, Name, Job, Lv, MaxLv, HP, MaxHP, MP, MaxMP, EXP, MaxEXP, Attack, Defense, STR, DEX, CON, WIS
        _level = 1;
        _hp = 100;
        _maxHp = 100;
        _mp = 100;
        _maxMp = 100;
        _attack = 10;
        _defense = 5; 
         */
        gladiatorList.Add("검투사"); // Name
        gladiatorList.Add("검투사"); // Job
        gladiatorList.Add("1"); // Lv
        gladiatorList.Add("250"); // MaxLv
        gladiatorList.Add("200"); // HP
        gladiatorList.Add("200"); // MaxHP
        gladiatorList.Add("200"); // MP
        gladiatorList.Add("200"); // MaxMP
        gladiatorList.Add("0"); // EXP
        gladiatorList.Add("100"); // MaxEXP
        gladiatorList.Add("10"); // Attack
        gladiatorList.Add("5"); // Defense
        gladiatorList.Add("10"); // STR
        gladiatorList.Add("10"); // DEX
        gladiatorList.Add("10"); // CON
        gladiatorList.Add("10"); // WIS

        wizardList.Add("마법사"); // Name
        wizardList.Add("마법사"); // Job
        wizardList.Add("1"); // Lv
        wizardList.Add("250"); // MaxLv
        wizardList.Add("200"); // HP
        wizardList.Add("200"); // MaxHP
        wizardList.Add("200"); // MP
        wizardList.Add("200"); // MaxMP
        wizardList.Add("0"); // EXP
        wizardList.Add("100"); // MaxEXP
        wizardList.Add("10"); // Attack
        wizardList.Add("5"); // Defense
        wizardList.Add("10"); // STR
        wizardList.Add("10"); // DEX
        wizardList.Add("10"); // CON
        wizardList.Add("10"); // WIS
    }
    
    public void Employment_Slot_1_Event() {
        Debug.Log("Employment_Slot_1_Event 실행");

        // 전사 정보: 검투사, 선택할 경우 캐릭터 정보 텍스트 표시 후 데이터 저장
        selectList.Clear();

        // 선택한 용병 정보를 리스트에 담아서 보관
        for(int i=0; i< gladiatorList.Count;i++) {
            selectList.Add(gladiatorList[i]);
        }

        // 훈련소 대화창 변경
        TrainingSchool.transform.GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text = "검투사 캐릭터 선택";
    }

    public void Employment_Slot_2_Event() {
        Debug.Log("Employment_Slot_2_Event 실행");

        // 마법사 정보: 마법사, 선택할 경우 캐릭터 정보 텍스트 표시 후 데이터 저장
        selectList.Clear();

        // 선택한 용병 정보를 리스트에 담아서 보관
        for (int i = 0; i < wizardList.Count; i++) {
            selectList.Add(wizardList[i]);
        }

        // 대화창 변경
        TrainingSchool.transform.GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text = "마법사 캐릭터 선택";
    }

    public void Employment_Event() {
        Debug.Log("Employment_Event 실행"); // 고용 버튼을 누를 경우, 

        switch (TrainingSchool.transform.GetChild(0).GetChild(4).childCount) {
            case 0:
                // 기본 용병 생성
                Employment_Slot_1 = Managers.Resource.Instantiate($"Employment_Slot_1");
                Employment_Slot_1.GetComponent<Button>().onClick.AddListener(Employment_Slot_1_Event);
                Employment_Slot_1.transform.SetParent(TrainingSchool.transform.GetChild(0).GetChild(4));
                RectTransform rtc1 = (RectTransform)Employment_Slot_1.transform;
                rtc1.anchoredPosition = new Vector3(0f, 0f);
                rtc1.localScale = new Vector3(1f, 1f, 1f);

                Employment_Slot_2 = Managers.Resource.Instantiate($"Employment_Slot_2");
                Employment_Slot_2.GetComponent<Button>().onClick.AddListener(Employment_Slot_2_Event);
                Employment_Slot_2.transform.SetParent(TrainingSchool.transform.GetChild(0).GetChild(4));
                RectTransform rt2 = (RectTransform)Employment_Slot_2.transform;
                rt2.anchoredPosition = new Vector3(0f, 0f);
                rt2.localScale = new Vector3(1f, 1f, 1f);
                break;
            case 1:
                Managers.Resource.Destroy(TrainingSchool.transform.GetChild(0).GetChild(4).GetChild(0).gameObject);

                // 기본 용병 생성
                Employment_Slot_1 = Managers.Resource.Instantiate($"Employment_Slot_1");
                Employment_Slot_1.GetComponent<Button>().onClick.AddListener(Employment_Slot_1_Event);
                Employment_Slot_1.transform.SetParent(TrainingSchool.transform.GetChild(0).GetChild(4));
                rtc1 = (RectTransform)Employment_Slot_1.transform;
                rtc1.anchoredPosition = new Vector3(0f, 0f);
                rtc1.localScale = new Vector3(1f, 1f, 1f);

                Employment_Slot_2 = Managers.Resource.Instantiate($"Employment_Slot_2");
                Employment_Slot_2.GetComponent<Button>().onClick.AddListener(Employment_Slot_2_Event);
                Employment_Slot_2.transform.SetParent(TrainingSchool.transform.GetChild(0).GetChild(4));
                rt2 = (RectTransform)Employment_Slot_2.transform;
                rt2.anchoredPosition = new Vector3(0f, 0f);
                rt2.localScale = new Vector3(1f, 1f, 1f);

                break;
            case 2:
                // 기존 이미지 제거
                Managers.Resource.Destroy(TrainingSchool.transform.GetChild(0).GetChild(4).GetChild(0).gameObject);
                Managers.Resource.Destroy(TrainingSchool.transform.GetChild(0).GetChild(4).GetChild(1).gameObject);

                // 기본 용병 생성, Employment_Slot_1 = 검투사
                Employment_Slot_1 = Managers.Resource.Instantiate($"Employment_Slot_1");
                Employment_Slot_1.GetComponent<Button>().onClick.AddListener(Employment_Slot_1_Event);
                Employment_Slot_1.transform.SetParent(TrainingSchool.transform.GetChild(0).GetChild(4));
                rtc1 = (RectTransform)Employment_Slot_1.transform;
                rtc1.anchoredPosition = new Vector3(0f, 0f);
                rtc1.localScale = new Vector3(1f, 1f, 1f);

                Employment_Slot_2 = Managers.Resource.Instantiate($"Employment_Slot_2");
                Employment_Slot_2.GetComponent<Button>().onClick.AddListener(Employment_Slot_2_Event);
                Employment_Slot_2.transform.SetParent(TrainingSchool.transform.GetChild(0).GetChild(4));
                rt2 = (RectTransform)Employment_Slot_2.transform;
                rt2.anchoredPosition = new Vector3(0f, 0f);
                rt2.localScale = new Vector3(1f, 1f, 1f);
                break;
        }

        Debug.Log($"TrainingSchool.transform.GetChild(0).GetChild(6).transform.name: {TrainingSchool.transform.GetChild(0).GetChild(6).transform.name}");

        // 버튼 제거 후 
        Managers.Resource.Destroy(TrainingSchool.transform.GetChild(0).GetChild(6).gameObject);

        // 기본 용병 생성, Employment_Slot_1 = 검투사
        GameObject buy_Button = Managers.Resource.Instantiate($"Buy_Button");
        buy_Button.GetComponent<Button>().onClick.AddListener(Buy_Event);
        buy_Button.transform.SetParent(TrainingSchool.transform.GetChild(0));
        RectTransform rt1 = (RectTransform)buy_Button.transform;
        rt1.anchoredPosition = new Vector3(53.8f, -135f);
        rt1.localScale = new Vector3(1f, 1f, 1f);
    }

    // 훈련소 UI 용병 해고 버튼을 누를 경우 실행
    // 보유 중인 용병 리스트를 출력
    public void Fire_Event() {
        Debug.Log("Fire_Event 실행 - network.unitList.Count: { network.unitList.Count}");
        switch(network.unitList.Count) {
            case 0:// 보유중인 용병이 존재하지 않을 경우
                // 기존 이미지 제거
                Managers.Resource.Destroy(TrainingSchool.transform.GetChild(0).GetChild(4).GetChild(0).gameObject);
                Managers.Resource.Destroy(TrainingSchool.transform.GetChild(0).GetChild(4).GetChild(1).gameObject);
                Employment_Slot_1 = null;
                Employment_Slot_2 = null;
                break;
            case 1:// 보유중인 용병이 존재하지 않을 경우
                // 기존 이미지 제거
                Managers.Resource.Destroy(TrainingSchool.transform.GetChild(0).GetChild(4).GetChild(0).gameObject);
                Managers.Resource.Destroy(TrainingSchool.transform.GetChild(0).GetChild(4).GetChild(1).gameObject);
                Employment_Slot_1 = null;
                Employment_Slot_2 = null;
                break;
            case 16:
                // 보유중인 용병이 한 명일 경우
                Debug.Log($"보유중인 용병이 한 명일 경우");
                Debug.Log($"TrainingSchool.transform.GetChild(4).childCount: {TrainingSchool.transform.GetChild(0).GetChild(4).childCount}");

                if (TrainingSchool.transform.GetChild(0).GetChild(4).childCount == 2)
                {
                    // 기존 이미지 제거
                    Managers.Resource.Destroy(TrainingSchool.transform.GetChild(0).GetChild(4).GetChild(0).gameObject);
                    Managers.Resource.Destroy(TrainingSchool.transform.GetChild(0).GetChild(4).GetChild(1).gameObject);
                    Employment_Slot_1 = null;
                    Employment_Slot_2 = null;

                    // 기본 용병 생성, Employment_Slot_1 = 검투사
                    Employment_Slot_1 = Managers.Resource.Instantiate($"Employment_Slot_1");
                    Employment_Slot_1.GetComponent<Button>().onClick.AddListener(PlayerUnitSlot_1_Event);

                    if (network.unitList[1].Equals("마법사") == true)
                    {
                        Employment_Slot_1.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/magic") as Sprite;
                    } else {
                        Employment_Slot_1.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/ui_icon_hero_arthur") as Sprite;
                    }

                    Employment_Slot_1.transform.SetParent(TrainingSchool.transform.GetChild(0).GetChild(4));
                    RectTransform rtc111 = (RectTransform)Employment_Slot_1.transform;
                    rtc111.anchoredPosition = new Vector3(0f, 0f);
                    rtc111.localScale = new Vector3(1f, 1f, 1f);
                }
                break;
            case 17:
                // 보유중인 용병이 한 명일 경우
                Debug.Log($"보유중인 용병이 한 명일 경우");
                Debug.Log($"TrainingSchool.transform.GetChild(4).childCount: {TrainingSchool.transform.GetChild(0).GetChild(4).childCount}");
                
                if(TrainingSchool.transform.GetChild(0).GetChild(4).childCount == 2)
                {
                    // 기존 이미지 제거
                    Managers.Resource.Destroy(TrainingSchool.transform.GetChild(0).GetChild(4).GetChild(0).gameObject);
                    Managers.Resource.Destroy(TrainingSchool.transform.GetChild(0).GetChild(4).GetChild(1).gameObject);
                    Employment_Slot_1 = null;
                    Employment_Slot_2 = null;

                    // 기본 용병 생성, Employment_Slot_1 = 검투사
                    Employment_Slot_1 = Managers.Resource.Instantiate($"Employment_Slot_1");
                    Employment_Slot_1.GetComponent<Button>().onClick.AddListener(PlayerUnitSlot_1_Event);

                    if (network.unitList[1].Equals("마법사") == true)
                    {
                        Employment_Slot_1.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/magic") as Sprite;
                    } else {
                        Employment_Slot_1.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/ui_icon_hero_arthur") as Sprite;
                    }

                    Employment_Slot_1.transform.SetParent(TrainingSchool.transform.GetChild(0).GetChild(4));
                    RectTransform rtc112 = (RectTransform)Employment_Slot_1.transform;
                    rtc112.anchoredPosition = new Vector3(0f, 0f);
                    rtc112.localScale = new Vector3(1f, 1f, 1f);
                }
                break;
            case 32:
                Debug.Log($"보유중인 용병이 두 명일 경우");
                Debug.Log($"TrainingSchool.transform.GetChild(4).childCount: {TrainingSchool.transform.GetChild(0).GetChild(4).childCount}");

                // 기존 이미지 제거
                Managers.Resource.Destroy(TrainingSchool.transform.GetChild(0).GetChild(4).GetChild(0).gameObject);
                Managers.Resource.Destroy(TrainingSchool.transform.GetChild(0).GetChild(4).GetChild(1).gameObject);
                Employment_Slot_1 = null;
                Employment_Slot_2 = null;

                //for (int i = 0; i < network.unitList.Count; i++)
                //{
                //    Debug.Log($"i: {i},  network.unitList: {network.unitList[i]}");
                //}

                // 첫번째 용병 생성
                Employment_Slot_1 = Managers.Resource.Instantiate($"Employment_Slot_1");
                Employment_Slot_1.GetComponent<Button>().onClick.AddListener(PlayerUnitSlot_1_Event);

                if (network.unitList[1].Equals("마법사") == true) {
                    Employment_Slot_1.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/magic") as Sprite;
                } else {
                    Employment_Slot_1.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/ui_icon_hero_arthur") as Sprite;
                }

                Employment_Slot_1.transform.SetParent(TrainingSchool.transform.GetChild(0).GetChild(4));
                RectTransform rtc1 = (RectTransform)Employment_Slot_1.transform;
                rtc1.anchoredPosition = new Vector3(0f, 0f);
                rtc1.localScale = new Vector3(1f, 1f, 1f);

                // 두번째 용병 생성
                Employment_Slot_2 = Managers.Resource.Instantiate($"Employment_Slot_2");
                Employment_Slot_2.GetComponent<Button>().onClick.AddListener(PlayerUnitSlot_2_Event);

                if (network.unitList[17].Equals("마법사") == true)
                {
                    Employment_Slot_2.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/magic") as Sprite;
                }
                else
                {
                    Employment_Slot_2.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/ui_icon_hero_arthur") as Sprite;
                }

                Employment_Slot_2.transform.SetParent(TrainingSchool.transform.GetChild(0).GetChild(4));
                RectTransform rtc2 = (RectTransform)Employment_Slot_2.transform;
                rtc2.anchoredPosition = new Vector3(0f, 0f);
                rtc2.localScale = new Vector3(1f, 1f, 1f);

                break;
            case 34:
                Debug.Log($"보유중인 용병이 두 명일 경우");
                Debug.Log($"TrainingSchool.transform.GetChild(4).childCount: {TrainingSchool.transform.GetChild(0).GetChild(4).childCount}");

                // 기존 이미지 제거
                Managers.Resource.Destroy(TrainingSchool.transform.GetChild(0).GetChild(4).GetChild(0).gameObject);
                Managers.Resource.Destroy(TrainingSchool.transform.GetChild(0).GetChild(4).GetChild(1).gameObject);
                Employment_Slot_1 = null;
                Employment_Slot_2 = null;

                //for(int i =0; i < network.unitList.Count;i++) {
                //    Debug.Log($"i: {i},  network.unitList: {network.unitList[i]}");
                //}

                // 첫번째 용병 생성
                Employment_Slot_1 = Managers.Resource.Instantiate($"Employment_Slot_1");
                Employment_Slot_1.GetComponent<Button>().onClick.AddListener(PlayerUnitSlot_1_Event);

                if (network.unitList[2].Equals("마법사") == true) {
                    Employment_Slot_1.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/magic") as Sprite;
                } else {
                    Employment_Slot_1.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/ui_icon_hero_arthur") as Sprite;
                }

                Employment_Slot_1.transform.SetParent(TrainingSchool.transform.GetChild(0).GetChild(4));
                RectTransform rtc11 = (RectTransform)Employment_Slot_1.transform;
                rtc11.anchoredPosition = new Vector3(0f, 0f);
                rtc11.localScale = new Vector3(1f, 1f, 1f);

                // 두번째 용병 생성
                Employment_Slot_2 = Managers.Resource.Instantiate($"Employment_Slot_2");
                Employment_Slot_2.GetComponent<Button>().onClick.AddListener(PlayerUnitSlot_2_Event);

                if (network.unitList[19].Equals("마법사") == true) {
                    Employment_Slot_2.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/magic") as Sprite;
                } else {
                    Employment_Slot_2.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/ui_icon_hero_arthur") as Sprite;
                }

                Employment_Slot_2.transform.SetParent(TrainingSchool.transform.GetChild(0).GetChild(4));
                RectTransform rtc22 = (RectTransform)Employment_Slot_2.transform;
                rtc22.anchoredPosition = new Vector3(0f, 0f);
                rtc22.localScale = new Vector3(1f, 1f, 1f);

                break;
        }

        Debug.Log($"TrainingSchool.transform.GetChild(0).GetChild(6).transform.name: {TrainingSchool.transform.GetChild(0).GetChild(6).transform.name}");
     
        // 버튼 제거 후 
        Managers.Resource.Destroy(TrainingSchool.transform.GetChild(0).GetChild(6).gameObject);

        // 기본 용병 생성, Employment_Slot_1 = 검투사
        GameObject fire_Button_1 = Managers.Resource.Instantiate($"Fire_Button_1");
        fire_Button_1.GetComponent<Button>().onClick.AddListener(Fire_Event_1);
        fire_Button_1.transform.SetParent(TrainingSchool.transform.GetChild(0));
        RectTransform rt1 = (RectTransform)fire_Button_1.transform;
        rt1.anchoredPosition = new Vector3(53.8f, -135f);
        rt1.localScale = new Vector3(1f, 1f, 1f);
    }

    // 보유중인 용병을 해고할 경우 실행, 선택한 용병 정보를 서버에 보내서 처리함
    public void Fire_Event_1() {
        Debug.Log("용병 해고 실행");
        
        // 소지금 증가
        // 17, 34
        Debug.Log($"선택한 용병 수: {selectList.Count}, 보유 유닛 정보 수: {network.unitList.Count}");
        /** 
        [player 정보]
        UserID - 번호: 0 , 값: test
        Name - 번호: 1 , 값: test1
        Gender - 번호: 2 , 값: 남자 캐릭터
        Lv - 번호: 3 , 값: 1
        MaxLv - 번호: 4 , 값: 250
        Money - 번호: 5 , 값: 10000
        HP - 번호: 6 , 값: 200
        MaxHP - 번호: 7 , 값: 200
        MP - 번호: 8 , 값: 100
        MaxMP - 번호: 9 , 값: 100
        EXP - 번호: 10 , 값: 0
        MaxEXP - 번호: 11 , 값: 100
        Attack - 번호: 12 , 값: 10
        Defense - 번호: 13 , 값: 5
        STR - 번호: 14 , 값: 10
        DEX - 번호: 15 , 값: 10
        CON - 번호: 16 , 값: 10
        WIS - 번호: 17 , 값: 10         

        [용병 정보]
        Name: 0 - 마법사
        Job: 1 - 마법사
        Lv : 2 - 1, MaxLv: 3 - 250, 
        HP: 4 - 200, MaxHP: 5 - 200
        MP : 6 - 200, MaxMP: 7 - 100
        EXP: 8 - 0, MaxEXP: 9 - 100
        Attack:10 - 10, Defense: 11 - 5
        STR: 12 - 10, DEX: 13 - 10, CON: 14 - 10, WIS: 15 - 10

        [용병 정보]
        Name: 16 - 마법사
        Job: 17 - 마법사
        Lv : 18 - 1, MaxLv: 19 - 250, 
        HP: 20 - 200, MaxHP: 21 - 200
        MP : 22 - 200, MaxMP: 23 - 100
        EXP: 24 - 0, MaxEXP: 25 - 100
        Attack:26 - 10, Defense: 27 - 5
        STR: 28 - 10, DEX: 29 - 10, CON: 30 - 10, WIS: 31 - 10

        // selectList 선택한 용병 정보
        for (int i = 0; i < selectList.Count; i++)
        {
            Debug.Log($"i: {i},  selectList[i]: {selectList[i]}");
        }
        */
        switch (network.unitList.Count) {
            case 16:
                Debug.Log($"보유중인 용병이 한 명일 경우 - PlayerId: {network.player[1]}, 돈: {network.player[5]}, 용병 이름: {selectList[1]}");

                // 매개변수: 플레이어 아이디, 소지금, 판매한 용병 이름
                network.FireEvent(network.player[1], network.player[5], selectList[1]);

                // UI 최신화, 해고한 용병 데이터 제거
                network.unitList.Clear();
                break;
            case 17:
                Debug.Log($"보유중인 용병이 한 명일 경우 - PlayerId: {network.player[1]}, 돈: {network.player[5]}, 용병 이름: {selectList[2]}");

                // 매개변수: 플레이어 아이디, 소지금, 판매한 용병 이름
                network.FireEvent(network.player[1], network.player[5], selectList[2]);

                // UI 최신화, 해고한 용병 데이터 제거
                network.unitList.Clear();
                break;
            case 32:
                Debug.Log($"보유중인 용병이 두 명일 경우 - PlayerId: {network.player[1]}, 돈: {network.player[5]}, 용병 이름: {selectList[1]}");

                // 매개변수: 플레이어 아이디, 소지금, 판매한 용병 이름
                network.FireEvent(network.player[1], network.player[5], selectList[1]);
                break;
            case 34:
                Debug.Log($"보유중인 용병이 두 명일 경우 - PlayerId: {network.player[1]}, 돈: {network.player[5]}, 용병 이름: {selectList[2]}");

                // 매개변수: 플레이어 아이디, 소지금, 판매한 용병 이름
                network.FireEvent(network.player[1], network.player[5], selectList[2]);

                // 여기서 UI 수정?
                break;
            default:
                Debug.Log($"선택한 용병이 없음");
            break;
        }   
    }

    // 훈련소 UI 해고 버튼을 누를 경우 실행, 보유중인 용병 리스트를 출력
    public void PlayerUnitSlot_1_Event() {
        Debug.Log($"PlayerUnitSlot_1_Event 실행 - network.unitList.Count: {network.unitList.Count}");

        // 리스트의 내용을 초기화 
        selectList.Clear();

        if(network.unitList.Count == 16) {
            selectList.Add(network.player[1]);
        }

        for (int i = 0; i < 17; i++) {
            selectList.Add(network.unitList[i]);
        }

        Debug.Log($"[선택한 용병 정보]: {selectList[2]} ");
        //for (int i = 0; i < selectList.Count; i++) {
        //    Debug.Log($"i: {i},  selectList: {selectList[i]}");
        //}

        //Debug.Log("[현재 보유 중인 용병 정보]");
        //for (int i = 0; i < 17; i++) {
        //    Debug.Log($"i: {i},  network.unitList: {network.unitList[i]}");
        //}
    }  

    public void PlayerUnitSlot_2_Event() {
        Debug.Log($"PlayerUnitSlot_2_Event 실행 - network.unitList.Count: {network.unitList.Count}");

        // 리스트의 내용을 초기화 
        selectList.Clear();

        for (int i = 17; i < network.unitList.Count; i++) {
            selectList.Add(network.unitList[i]);
        }

        Debug.Log($"[선택한 용병 정보]: {selectList[2]} ");
    }

    public void Buy_Event() {
        // 용병을 고용할 경우
        Debug.Log("Buy_Event 실행");

        if(selectList.Count == 0) {
            Debug.Log("용병을 선택 안함");
        } else {
            Debug.Log($"선택한 용병 정보 출력: {selectList[0]}");

            // 소지금 조회 후 돈이 있다면 실행 후 서버 전송, 전달할 정보: 플레이어 id, 돈, 용병 이름, 프로토콜 번호
            Debug.Log($"Name: {network.player[1]},  Money: {network.player[5]}");

            // 잔액에 따라 구매 여부 결정 

            // 매개변수: 플레이어 아이디, 소지금, 구매한 용병 이름
            network.EmploymentEvent(network.player[1], network.player[5], selectList[0]);
        }
    }
    
    public void TrainingClose() { 
        Debug.Log("창을 종료할 경우 - TrainingClose 실행");
        Managers.Resource.Destroy(TrainingSchool.gameObject);
    }
}