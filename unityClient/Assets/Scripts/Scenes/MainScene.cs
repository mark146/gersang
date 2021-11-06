using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScene : MonoBehaviour, IPointerEnterHandler {
    List<Player> players;
    Dictionary<string, OrderPlayer> _orderPlayers;
    GameObject TimePanel;
    Text moneyText;
    Text mainName;
    NetworkManager network;
    CameraController mainCamera;
    GameObject player;
    GameObject unitListUI;
    GameObject infoPopup;
    GameObject characterState;
    Boolean isInven = false;
    GameObject TrainingSchool;
    GameObject mainChat;
    ScrollRect scrollRect;
    RectTransform scrollTransform;
    Queue<Chat> chatQueue;

    void Start() {
        // Debug.Log("MainScene - Start 실행");

        players = new List<Player>();
        _orderPlayers = new Dictionary<string, OrderPlayer>();
        TimePanel = GameObject.Find("TimePanel");
        TimePanel = TimePanel.transform.GetChild(0).gameObject;
        moneyText = GameObject.Find("MoneyText").GetComponent<Text>();
        mainName = GameObject.Find("MainName").GetComponent<Text>();
        mainCamera = GameObject.Find("MainCamera").GetComponent<CameraController>();
        mainChat = GameObject.Find("MainChat");

        //이벤트 추가
        GameObject chatSend = mainChat.transform.GetChild(2).gameObject;
        chatSend.GetComponent<Button>().onClick.AddListener(chatAction);

        // 클라이언트가 메인 화면에 처음 접속한 경우
        // 전투가 끝나고 다시 메인 화면에 접속한 경우
        //채팅 정보 호출 - scrollRect 값 호출
        // Debug.Log($"mainChat.content: {mainChat.transform.GetChild(0).GetChild(0).GetChild(0).name}");

        
        // 스크롤바의 위치를 제일 아래로 내려준다 
        // 1.0이면 제일 위로 스크롤 0.0 이면 제일 아래로 스크롤이다
        Scrollbar scrollbar = mainChat.transform.GetChild(0).GetChild(1).GetComponent<Scrollbar>();
        scrollbar.value = 0.0f;

        ScrollRect scrollRect =  mainChat.transform.GetChild(0).GetComponent<ScrollRect>();
        
        //NetworkManager 검색 후 객체 생성
        GameObject net = GameObject.Find("@NetworkManager");
        network = net.GetComponent<NetworkManager>();

        chatQueue = network.chatQueue;
        // Debug.Log($"캐릭터 처음 접속 시간: {network.connectTime}");

        // Debug.Log($"채팅 로그 갯수 - chatQueue.Count: {chatQueue.Count}");
        
        //Debug.Log($"채팅 로그 갯수[UI] - chatQueue.Count: {mainChat.transform.GetChild(0).GetChild(0).GetChild(0).childCount}");
        if(mainChat.transform.GetChild(0).GetChild(0).GetChild(0).childCount == 0) {

            // 채팅 로그 전송
            Chat clientInfo = new Chat();

            // 클라이언트 마지막 채팅 로그 조회 및 저장
            foreach (Chat chatInfo in network.chatQueue)
            {
                clientInfo.playerId = chatInfo.playerId;
                clientInfo.content = chatInfo.content;
                clientInfo.time = chatInfo.time;


                // 채팅 내용 추가
                GameObject chat = Managers.Resource.Instantiate("UI/WhiteArea");
                chat.transform.SetParent(mainChat.transform.GetChild(0).GetChild(0).GetChild(0).transform);

                // 캐릭터 명
                chat.transform.GetChild(0).GetComponent<Text>().text = chatInfo.playerId;

                // 메시지 내용
                chat.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = chatInfo.content;

                // 메시지 시간
                chat.transform.GetChild(2).GetChild(2).GetComponent<Text>().text = chatInfo.time;

                RectTransform chat_rect = (RectTransform)chat.transform;
                chat_rect.anchoredPosition = new Vector3(0f, 0f);
                chat_rect.localScale = new Vector3(1f, 1f, 1f);
            }
        }

        
        // 용병창 리스트 UI 검색 후 객체로 생성
        unitListUI = GameObject.Find("UnitList");


        // 플레이어 정보를 용병창 UI에 반영
        /** 
        [player 정보] - 캐릭터 정보 연동
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
         */
        mainName.text = network.player[1] as string;

        // 플레이어 성별에 따라 다른 모델, 이미지 생성
        switch (network.player[2]) {
            case "남자 캐릭터":
                player = Managers.Resource.Instantiate("Player_Male");     
                unitListUI.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/player_male") as Sprite;
                break;
            case "여자 캐릭터":
                player = Managers.Resource.Instantiate("Player_Female");
                unitListUI.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/player_female") as Sprite;
                break;
        }

        //플레이어 이미지에 버튼 이벤트 추가
        unitListUI.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(PlayerInfo);

        //플레이어 체력바 설정
        unitListUI.transform.GetChild(0).GetChild(0).GetComponent<Slider>().maxValue = float.Parse(network.player[5]);
        unitListUI.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value = float.Parse(network.player[5]);

        //플레이어 마력바 설정
        unitListUI.transform.GetChild(0).GetChild(1).GetComponent<Slider>().maxValue = float.Parse(network.player[6]);
        unitListUI.transform.GetChild(0).GetChild(1).GetComponent<Slider>().value = float.Parse(network.player[6]);

        // 플레이어의 유닛 정보가 null이 아니라면 실행, 용병창 UI에 보유중인 유닛 수에 따라 다르게 표시하게 설정
        if (network.unitList != null) {
            Debug.Log($"network.unitList: {network.unitList.Count}");
            Debug.Log($"unitListUI.transform.GetChildCount(): {unitListUI.transform.childCount}");

            // 플레이어 보유 유닛 수에 따라서 분기 처리 진행
            // 용병 조회 후 분기 처리, 한마리 존재할 경우, 두마리 존재할 경우 처리
            switch (network.unitList.Count()) {
                case 0: // 플레이어 캐릭터에 용병이 없을 경우
                    Debug.Log("17 - unitList.Count(): " + network.unitList.Count());

                    GameObject mercenary_Slot_1_Empty1 = Managers.Resource.Instantiate($"Mercenary_Slot_1_Empty");
                    mercenary_Slot_1_Empty1.transform.SetParent(unitListUI.transform);
                    RectTransform rtc1 = (RectTransform)mercenary_Slot_1_Empty1.transform;
                    rtc1.anchoredPosition = new Vector3(0f, 0f);
                    rtc1.localScale = new Vector3(1f, 1f, 1f);

                    GameObject mercenary_Slot_2_Empty = Managers.Resource.Instantiate($"Mercenary_Slot_2_Empty");
                    mercenary_Slot_2_Empty.transform.SetParent(unitListUI.transform);
                    RectTransform rt2 = (RectTransform)mercenary_Slot_2_Empty.transform;
                    rt2.anchoredPosition = new Vector3(0f, 0f);
                    rt2.localScale = new Vector3(1f, 1f, 1f);
                    break;
                case 1: // 플레이어 캐릭터에 용병이 없을 경우
                    Debug.Log("17 - unitList.Count(): " + network.unitList.Count());

                    mercenary_Slot_1_Empty1 = Managers.Resource.Instantiate($"Mercenary_Slot_1_Empty");
                    mercenary_Slot_1_Empty1.transform.SetParent(unitListUI.transform);
                    rtc1 = (RectTransform)mercenary_Slot_1_Empty1.transform;
                    rtc1.anchoredPosition = new Vector3(0f, 0f);
                    rtc1.localScale = new Vector3(1f, 1f, 1f);

                    mercenary_Slot_2_Empty = Managers.Resource.Instantiate($"Mercenary_Slot_2_Empty");
                    mercenary_Slot_2_Empty.transform.SetParent(unitListUI.transform);
                    rt2 = (RectTransform)mercenary_Slot_2_Empty.transform;
                    rt2.anchoredPosition = new Vector3(0f, 0f);
                    rt2.localScale = new Vector3(1f, 1f, 1f);
                    break;
                case 16:
                    Debug.Log("용병이 한 명 있을 경우: " + network.unitList.Count());
                    //for (i =0; i< unitList.Count;i++)
                    //{
                    //    Debug.Log($"i: {i} - unitList: {unitList[i]}");
                    //}

                    // 용병 ui 생성
                    GameObject Mercenary_01 = Managers.Resource.Instantiate($"Mercenary_01");
                    Mercenary_01.transform.SetParent(unitListUI.transform);
                    rtc1 = (RectTransform)Mercenary_01.transform;
                    rtc1.anchoredPosition = new Vector3(0f, 0f);
                    rtc1.localScale = new Vector3(1f, 1f, 1f);

                    // Debug.Log($"unitList[1]: {network.unitList[1]}");

                    if (network.unitList[1].Equals("마법사") == true) {
                        Mercenary_01.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/magic") as Sprite;
                    } else {
                        Mercenary_01.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/ui_icon_hero_arthur") as Sprite;
                    }

                    //플레이어 체력바 설정
                    unitListUI.transform.GetChild(1).GetChild(0).GetComponent<Slider>().maxValue = float.Parse(network.unitList[5]);
                    unitListUI.transform.GetChild(1).GetChild(0).GetComponent<Slider>().value = float.Parse(network.unitList[4]);

                    //플레이어 마력바 설정
                    unitListUI.transform.GetChild(1).GetChild(1).GetComponent<Slider>().maxValue = float.Parse(network.unitList[7]);
                    unitListUI.transform.GetChild(1).GetChild(1).GetComponent<Slider>().value = float.Parse(network.unitList[6]);

                    // 빈 용병 ui 생성
                    mercenary_Slot_2_Empty = Managers.Resource.Instantiate($"Mercenary_Slot_2_Empty");
                    mercenary_Slot_2_Empty.transform.SetParent(unitListUI.transform);
                    rt2 = (RectTransform)mercenary_Slot_2_Empty.transform;
                    rt2.anchoredPosition = new Vector3(0f, 0f);
                    rt2.localScale = new Vector3(1f, 1f, 1f);
                    break;
                case 17:
                    Mercenary_01 = Managers.Resource.Instantiate($"Mercenary_01");
                    Mercenary_01.transform.SetParent(unitListUI.transform);
                    rtc1 = (RectTransform)Mercenary_01.transform;
                    rtc1.anchoredPosition = new Vector3(0f, 0f);
                    rtc1.localScale = new Vector3(1f, 1f, 1f);

                    // Debug.Log($"unitList[1]: {network.unitList[1]}");

                    if (network.unitList[2].Equals("마법사") == true) {
                        Mercenary_01.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/magic") as Sprite;
                    } else {
                        Mercenary_01.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/ui_icon_hero_arthur") as Sprite;
                    }

                    //플레이어 체력바 설정
                    unitListUI.transform.GetChild(1).GetChild(0).GetComponent<Slider>().maxValue = float.Parse(network.unitList[6]);
                    unitListUI.transform.GetChild(1).GetChild(0).GetComponent<Slider>().value = float.Parse(network.unitList[5]);

                    //플레이어 마력바 설정
                    unitListUI.transform.GetChild(1).GetChild(1).GetComponent<Slider>().maxValue = float.Parse(network.unitList[8]);
                    unitListUI.transform.GetChild(1).GetChild(1).GetComponent<Slider>().value = float.Parse(network.unitList[7]);
                    
                    // 빈 용병 ui 생성
                    mercenary_Slot_2_Empty = Managers.Resource.Instantiate($"Mercenary_Slot_2_Empty");
                    mercenary_Slot_2_Empty.transform.SetParent(unitListUI.transform);
                    rt2 = (RectTransform)mercenary_Slot_2_Empty.transform;
                    rt2.anchoredPosition = new Vector3(0f, 0f);
                    rt2.localScale = new Vector3(1f, 1f, 1f);
                    break;
                case 32:
                    Debug.Log("용병이 두 명 있을 경우: " + network.unitList.Count());
                    //for (i = 0; i < unitList.Count; i++)
                    //{
                    //    Debug.Log($"i: {i} - unitList: {unitList[i]}");
                    //}

                    Managers.Resource.Destroy(unitListUI.transform.GetChild(1).gameObject);
                    Managers.Resource.Destroy(unitListUI.transform.GetChild(2).gameObject);

                    // 용병 ui 생성
                    Mercenary_01 = Managers.Resource.Instantiate($"Mercenary_01");
                    Mercenary_01.transform.SetParent(unitListUI.transform);
                    rtc1 = (RectTransform)Mercenary_01.transform;
                    rtc1.anchoredPosition = new Vector3(0f, 0f);
                    rtc1.localScale = new Vector3(1f, 1f, 1f);

                    Debug.Log($"unitList[1]: {network.unitList[1]}");

                    if (network.unitList[1].Equals("마법사") == true) {
                        Mercenary_01.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/magic") as Sprite;
                    } else {
                        Mercenary_01.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/ui_icon_hero_arthur") as Sprite;
                    }

                    ////플레이어 체력바 설정
                    //unitListUI.transform.GetChild(1).GetChild(0).GetComponent<Slider>().maxValue = float.Parse(network.unitList[5]);
                    //unitListUI.transform.GetChild(1).GetChild(0).GetComponent<Slider>().value = float.Parse(network.unitList[4]);

                    ////플레이어 마력바 설정
                    //unitListUI.transform.GetChild(1).GetChild(1).GetComponent<Slider>().maxValue = float.Parse(network.unitList[7]);
                    //unitListUI.transform.GetChild(1).GetChild(1).GetComponent<Slider>().value = float.Parse(network.unitList[6]);


                    // 빈 용병 ui 생성
                    GameObject Mercenary_02 = Managers.Resource.Instantiate($"Mercenary_02");
                    Mercenary_02.transform.SetParent(unitListUI.transform);
                    rt2 = (RectTransform)Mercenary_02.transform;
                    rt2.anchoredPosition = new Vector3(0f, 0f);
                    rt2.localScale = new Vector3(1f, 1f, 1f);

                    // Debug.Log($"unitList[17]: {network.unitList[17]}");

                    if (network.unitList[17].Equals("마법사") == true) {
                        Mercenary_02.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/magic") as Sprite;
                    } else {
                        Mercenary_02.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/ui_icon_hero_arthur") as Sprite;
                    }

                    ////플레이어 체력바 설정
                    //unitListUI.transform.GetChild(2).GetChild(0).GetComponent<Slider>().maxValue = float.Parse(network.unitList[21]);
                    //unitListUI.transform.GetChild(2).GetChild(0).GetComponent<Slider>().value = float.Parse(network.unitList[20]);

                    ////플레이어 마력바 설정
                    //unitListUI.transform.GetChild(2).GetChild(1).GetComponent<Slider>().maxValue = float.Parse(network.unitList[23]);
                    //unitListUI.transform.GetChild(2).GetChild(1).GetComponent<Slider>().value = float.Parse(network.unitList[22]);
                    break;
                case 34:
                    Mercenary_01 = Managers.Resource.Instantiate($"Mercenary_01");
                    Mercenary_01.transform.SetParent(unitListUI.transform);
                    rtc1 = (RectTransform)Mercenary_01.transform;
                    rtc1.anchoredPosition = new Vector3(0f, 0f);
                    rtc1.localScale = new Vector3(1f, 1f, 1f);

                    // Debug.Log($"unitList[1]: {network.unitList[1]}");

                    if (network.unitList[2].Equals("마법사") == true) {
                        Mercenary_01.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/magic") as Sprite;
                    } else {
                        Mercenary_01.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/ui_icon_hero_arthur") as Sprite;
                    }

                    //플레이어 체력바 설정
                    unitListUI.transform.GetChild(1).GetChild(0).GetComponent<Slider>().maxValue = float.Parse(network.unitList[6]);
                    unitListUI.transform.GetChild(1).GetChild(0).GetComponent<Slider>().value = float.Parse(network.unitList[5]);

                    //플레이어 마력바 설정
                    unitListUI.transform.GetChild(1).GetChild(1).GetComponent<Slider>().maxValue = float.Parse(network.unitList[8]);
                    unitListUI.transform.GetChild(1).GetChild(1).GetComponent<Slider>().value = float.Parse(network.unitList[7]);

                    // 두 번째 용병 ui 생성
                    Mercenary_02 = Managers.Resource.Instantiate($"Mercenary_02");
                    Mercenary_02.transform.SetParent(unitListUI.transform);
                    rt2 = (RectTransform)Mercenary_02.transform;
                    rt2.anchoredPosition = new Vector3(0f, 0f);
                    rt2.localScale = new Vector3(1f, 1f, 1f);

                    // Debug.Log($"unitList[17]: {network.unitList[17]}");

                    if (network.unitList[19].Equals("마법사") == true) {
                        Mercenary_02.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/magic") as Sprite;
                    } else {
                        Mercenary_02.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/ui_icon_hero_arthur") as Sprite;
                    }

                    //플레이어 체력바 설정
                    unitListUI.transform.GetChild(2).GetChild(0).GetComponent<Slider>().maxValue = float.Parse(network.unitList[23]);
                    unitListUI.transform.GetChild(2).GetChild(0).GetComponent<Slider>().value = float.Parse(network.unitList[22]);

                    //플레이어 마력바 설정
                    unitListUI.transform.GetChild(2).GetChild(1).GetComponent<Slider>().maxValue = float.Parse(network.unitList[25]);
                    unitListUI.transform.GetChild(2).GetChild(1).GetComponent<Slider>().value = float.Parse(network.unitList[24]);
                    break;
                default:
                    Debug.Log("network.unitList.Count(): " + network.unitList.Count());
                    break;
            }
        }
        else {
            // 플레이어가 처음 메인 화면에 접속할 경우 null 발생
            // Debug.Log($"network.unitList: {network.unitList}");
        }

        // 플레이어에게 카메라 뷰 연결
        mainCamera.SetPlayer(player);

        // 룸 정보 최신화 시켜주는 함수 코루틴으로 실행 - 참고: https://solution94.tistory.com/7
        InvokeRepeating("roomUpdate", 0f, 0.25f);
        InvokeRepeating("playerUpdate", 0f, 0.20f);
    }

    //채팅 버튼 누를 경우 서버로 전송
    void chatAction() {
        Debug.Log("채팅 내용 서버로 전송");
        string chatContent = mainChat.transform.GetChild(1).GetComponent<InputField>().text;

        if (chatContent.Trim().Equals("") == true) {
            Debug.Log($"채팅 내용 비어있음");
        } else {
            mainChat.transform.GetChild(1).GetComponent<InputField>().text = "";

            /**
            Debug.Log($"서버 보내기 전");
		    Debug.Log($"유저 아이디: {sendMsg.playerId}");
		    Debug.Log($"유저 접속 시간: {sendMsg.playerConnectTime}");
		    Debug.Log($"유저 위치: {sendMsg.currentLocation}");
		    Debug.Log($"채팅 내용: {sendMsg.content}");
		    Debug.Log($"채팅 입력 시간: {sendMsg.time}");
             */
            Chat chat = new Chat();
            chat.playerId = network.player[1];
            chat.playerConnectTime = network.connectTime;
            chat.currentLocation = SceneManager.GetActiveScene().name;
            chat.content = chatContent;
            // chat.time = DateTime.Now.ToString(("hh:mm"), DateTimeFormatInfo.InvariantInfo);
            chat.time = DateTime.Now.ToString(("yyyy-MM-dd HH:mm:ss"));
           
            // 클라이언틑 채팅 리스트에 내용 추가 후 추가한 내용만 전송
            network.ChatSend(chat);
        }
    }

    void playerUpdate() {
        // 서버에서 온 데이터를 꺼낸 뒤 처리
        List<Dictionary<int, object>> list = Queue.Instance.PopAll();
            foreach (Dictionary<int, object> result in list) {
            //Debug.Log("MainScene - result.Keys: " + result.Keys);
            //Debug.Log("MainScene - result.Values: " + result.Values);

            foreach (int Key in result.Keys) {
                switch (Key) {
                    case 8: // 주변 유닛 정보 최신화
                        // enumerator 참고: https://justice-dev.tistory.com/1, https://jw-stroy-at30.tistory.com/7
                        IEnumerator enumerator = result.Values.GetEnumerator();
                        while (enumerator.MoveNext()) {
                            object current = enumerator.Current;
                          
                            players = JsonConvert.DeserializeObject<List<Player>>(current.ToString());
                        }
                       // Debug.Log("8 - result.Values: " + players.Count);
                        RoomInfo(players);
                        break;
                    case 6:// 인벤토리창 업데이트
                        string recv = null;

                        int i = 0;
                        enumerator = result.Values.GetEnumerator();
                        while (enumerator.MoveNext()) {
                            if (i == 0) {
                                object current = enumerator.Current;
                                recv = current.ToString();
                                i++;
                            } else {
                                i = 0;
                            }
                        }
                        List<string> inven = JsonConvert.DeserializeObject<List<string>>(recv);
                        //Debug.Log("6 - inven.Count(): " + inven.Count());
                        network.inven = inven;
                        Inventory.recvInven(inven);
                        break;
                    case 7:// 장비창 업데이트
                        string gearRecv = null;

                        // 참고: https://jw-stroy-at30.tistory.com/7
                        enumerator = result.Values.GetEnumerator();
                        while (enumerator.MoveNext()) {
                            object current = enumerator.Current;
                            //Debug.Log($"7 - enumerator.Current: {enumerator.Current}");
                            gearRecv = current.ToString();
                        }

                        List<string> gear = JsonConvert.DeserializeObject<List<string>>(gearRecv);
                        //Debug.Log("7 - gear.Count(): " + gear.Count());
                        network.player_gear = gear;
                        Inventory.recvGear(gear);
                        break;
                    case 14: // 서버에서 ArrayIndexOutOfBoundsException 발생시 호출, 인벤 정보 다시 전송
                        string message = null;
                        
                        enumerator = result.Values.GetEnumerator();
                        while (enumerator.MoveNext()) {
                            object current = enumerator.Current;
                            message = current.ToString();
                        }
                        Debug.Log($"14 - 서버에서온 메시지: {message}");

                        // 인벤토리 정보 재생성 후 전송
                        Dictionary<string, List<string>> invenList = new Dictionary<string, List<string>>();
                        invenList.Add(network.player[1], network.inven);

                        network.ReInventoryUpdate(invenList);
                        break;
                    case 16: // 서버에서 ArrayIndexOutOfBoundsException 발생시 호출, 인벤 정보 다시 전송
                        message = null;

                        enumerator = result.Values.GetEnumerator();
                        while (enumerator.MoveNext()) {
                            object current = enumerator.Current;
                            message = current.ToString();
                        }
                        Debug.Log($"16 - 서버에서온 메시지: {message}");

                        // 인벤토리 정보 재생성 후 전송
                        network.ShopBuyReUpdate();

                        break;
                    case 17:
                        string unitRecv = null;

                        // 참고: https://jw-stroy-at30.tistory.com/7
                        enumerator = result.Values.GetEnumerator();
                        while (enumerator.MoveNext()) {
                            object current = enumerator.Current;
                            unitRecv = current.ToString();
                        }
                        List<string> unitList = JsonConvert.DeserializeObject<List<string>>(unitRecv);
                        Debug.Log($"case 17 - unitList: {unitList.Count()}");
                        Debug.Log($"case 17 - unitListUI.transform.GetChildCount(): {unitListUI.transform.childCount}");
                        Debug.Log($"unitListUI.transform.GetChild(1).name: {unitListUI.transform.GetChild(1).name}");
                        Debug.Log($"unitListUI.transform.GetChild(2).name: {unitListUI.transform.GetChild(2).name}");

                        // 용병 조회 후 분기 처리, 한마리 존재할 경우, 두마리 존재할 경우 처리
                        switch (unitList.Count()) {
                            case 1: // 플레이어 캐릭터에 용병이 없을 경우
                                    // Debug.Log("17 - unitList.Count(): " + unitList.Count());

                                if (unitListUI.transform.GetChild(1).name.Equals("Mercenary_Slot_1_Empty") != true) {
                                    GameObject mercenary_Slot_1_Empty1 = Managers.Resource.Instantiate($"Mercenary_Slot_1_Empty");
                                    mercenary_Slot_1_Empty1.transform.SetParent(unitListUI.transform);
                                    RectTransform rtc1_1 = (RectTransform)mercenary_Slot_1_Empty1.transform;
                                    rtc1_1.anchoredPosition = new Vector3(0f, 0f);
                                    rtc1_1.localScale = new Vector3(1f, 1f, 1f);
                                }

                                if (unitListUI.transform.GetChild(2).name.Equals("Mercenary_Slot_2_Empty") != true) {
                                    GameObject mercenary_Slot_2_2_Empty = Managers.Resource.Instantiate($"Mercenary_Slot_2_Empty");
                                    mercenary_Slot_2_2_Empty.transform.SetParent(unitListUI.transform);
                                    RectTransform rt2_2 = (RectTransform)mercenary_Slot_2_2_Empty.transform;
                                    rt2_2.anchoredPosition = new Vector3(0f, 0f);
                                    rt2_2.localScale = new Vector3(1f, 1f, 1f);
                                }

                                break;
                            case 16:
                                Debug.Log("용병이 한 명 있을 경우: " + unitList.Count());
                                //for (i =0; i< unitList.Count;i++)
                                //{
                                //    Debug.Log($"i: {i} - unitList: {unitList[i]}");
                                //}

                                // 용병 ui 생성
                                GameObject Mercenary_01 = Managers.Resource.Instantiate($"Mercenary_01");
                                Mercenary_01.transform.SetParent(unitListUI.transform);
                                RectTransform rtc1 = (RectTransform)Mercenary_01.transform;
                                rtc1.anchoredPosition = new Vector3(0f, 0f);
                                rtc1.localScale = new Vector3(1f, 1f, 1f);

                                // Debug.Log($"unitList[1]: {unitList[1]}");

                                if (unitList[1].Equals("마법사") == true)
                                {
                                    Mercenary_01.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/magic") as Sprite;
                                } else{
                                    Mercenary_01.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/ui_icon_hero_arthur") as Sprite;
                                }

                                //플레이어 체력바 설정
                                unitListUI.transform.GetChild(1).GetChild(0).GetComponent<Slider>().maxValue = float.Parse(unitList[5]);
                                unitListUI.transform.GetChild(1).GetChild(0).GetComponent<Slider>().value = float.Parse(unitList[4]);

                                //플레이어 마력바 설정
                                unitListUI.transform.GetChild(1).GetChild(1).GetComponent<Slider>().maxValue = float.Parse(unitList[7]);
                                unitListUI.transform.GetChild(1).GetChild(1).GetComponent<Slider>().value = float.Parse(unitList[6]);

                                // 빈 용병 ui 생성
                                GameObject mercenary_Slot_2_Empty = Managers.Resource.Instantiate($"Mercenary_Slot_2_Empty");
                                mercenary_Slot_2_Empty.transform.SetParent(unitListUI.transform);
                                RectTransform rt2 = (RectTransform)mercenary_Slot_2_Empty.transform;
                                rt2.anchoredPosition = new Vector3(0f, 0f);
                                rt2.localScale = new Vector3(1f, 1f, 1f);

                                break;
                            case 32:
                                Debug.Log("용병이 두 명 있을 경우: " + unitList.Count());
                                //for (i = 0; i < unitList.Count; i++)
                                //{
                                //    Debug.Log($"i: {i} - unitList: {unitList[i]}");
                                //}

                                Debug.Log($"case 32 - unitListUI.transform.GetChildCount(): {unitListUI.transform.childCount}");
                                Managers.Resource.Destroy(unitListUI.transform.GetChild(1).gameObject);
                                Managers.Resource.Destroy(unitListUI.transform.GetChild(2).gameObject);
                            

                                // 용병 ui 생성
                                Mercenary_01 = Managers.Resource.Instantiate($"Mercenary_01");
                                Mercenary_01.transform.SetParent(unitListUI.transform);
                                rtc1 = (RectTransform)Mercenary_01.transform;
                                rtc1.anchoredPosition = new Vector3(0f, 0f);
                                rtc1.localScale = new Vector3(1f, 1f, 1f);

                                Debug.Log($"unitList[1]: {unitList[1]}");

                                if (unitList[1].Equals("마법사") == true) {
                                    Mercenary_01.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/magic") as Sprite;
                                } else {
                                    Mercenary_01.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/ui_icon_hero_arthur") as Sprite;
                                }

                                Debug.Log($"unitListUI.transform.GetChild(1).name: {unitListUI.transform.GetChild(1).name}");
                                if (unitListUI.transform.GetChild(1).name.Equals("Mercenary_Slot_1_Empty") != true) {
                                    Debug.Log($"unitListUI.transform.GetChild(1).name: {unitListUI.transform.GetChild(1).name}");
                                    Debug.Log($"unitList[5]: {unitList[5]}");
                                    //플레이어 체력바 설정
                                    unitListUI.transform.GetChild(1).GetChild(0).GetComponent<Slider>().maxValue = float.Parse(unitList[5]);
                                    unitListUI.transform.GetChild(1).GetChild(0).GetComponent<Slider>().value = float.Parse(unitList[4]);

                                    //플레이어 마력바 설정
                                    unitListUI.transform.GetChild(1).GetChild(1).GetComponent<Slider>().maxValue = float.Parse(unitList[7]);
                                    unitListUI.transform.GetChild(1).GetChild(1).GetComponent<Slider>().value = float.Parse(unitList[6]);
                                }

                                // 빈 용병 ui 생성
                                GameObject Mercenary_02 = Managers.Resource.Instantiate($"Mercenary_02");
                                Mercenary_02.transform.SetParent(unitListUI.transform);
                                rt2 = (RectTransform)Mercenary_02.transform;
                                rt2.anchoredPosition = new Vector3(0f, 0f);
                                rt2.localScale = new Vector3(1f, 1f, 1f);

                                Debug.Log($"unitListUI.transform.GetChild(2).name: {unitListUI.transform.GetChild(2).name}");
                                if (unitListUI.transform.GetChild(2).name.Equals("Mercenary_Slot_2_Empty") != true) {
                                    Debug.Log($"unitList[17]: {unitList[17]}");

                                    if (unitList[17].Equals("마법사") == true) {
                                        Mercenary_02.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/magic") as Sprite;
                                    } else {
                                        Mercenary_02.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/ui_icon_hero_arthur") as Sprite;
                                    }

                                    //플레이어 체력바 설정
                                    unitListUI.transform.GetChild(2).GetChild(0).GetComponent<Slider>().maxValue = float.Parse(unitList[21]);
                                    unitListUI.transform.GetChild(2).GetChild(0).GetComponent<Slider>().value = float.Parse(unitList[20]);

                                    //플레이어 마력바 설정
                                    unitListUI.transform.GetChild(2).GetChild(1).GetComponent<Slider>().maxValue = float.Parse(unitList[23]);
                                    unitListUI.transform.GetChild(2).GetChild(1).GetComponent<Slider>().value = float.Parse(unitList[22]);
                                }

                                break;
                            default:
                                Debug.Log("17 - unitList.Count(): " + unitList.Count());
                                break;
                        }

                        network.unitList = unitList;
                        break;
                    case 18:
                        unitRecv = null;

                        // 참고: https://jw-stroy-at30.tistory.com/7
                        enumerator = result.Values.GetEnumerator();
                        while (enumerator.MoveNext()) {
                            object current = enumerator.Current;
                            unitRecv = current.ToString();
                        }

                        //한개 있을 경우 제거
                        if (network.unitList.Count == 1) {
                            network.unitList.Clear();
                        }

                        // 플레이어 용병 정보 조회 후 처리
                        Debug.Log($"18 - unitRecv: {unitRecv}");
                        
                        // 데이터 저장
                        List<string> recvUnitInfo = JsonConvert.DeserializeObject<List<string>>(unitRecv);

                        // 고용한 용병 정보 플레이어 용병 리스트에 저장
                        for (i = 0; i < recvUnitInfo.Count; i++) {
                            network.unitList.Add(recvUnitInfo[i]);
                        }

                        Debug.Log($"용병 고용 결과 - 용병 데이터 처리 후 저장내용 확인: {network.unitList.Count}");

                        // 용병 UI 최신화
                        unitListUpdate();
                        break;
                    case 19:
                        unitRecv = null;

                        // 참고: https://jw-stroy-at30.tistory.com/7
                        enumerator = result.Values.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            object current = enumerator.Current;
                            unitRecv = current.ToString();
                        }

                        // 플레이어 용병 정보 조회 후 처리
                        Debug.Log($"19 - unitRecv: {unitRecv}");

                        // 훈련소 정보 호출 후 전송
                        GameObject train = GameObject.Find("Training_school(Clone)");
                        TrainingCenter Train = train.GetComponent<TrainingCenter>();

                        // 데이터 저장
                        List<string> recvUnit = JsonConvert.DeserializeObject<List<string>>(unitRecv);

                        // 예외 상황으로 전달한 경우 1 값이 옴 
                        if(recvUnit.Count == 1) {
                            Debug.Log($"21 - 예외처리 플레이어 - 해고한 용병 이름: {Train.selectList[2]}");
                        } else {
                            // 고용한 용병 정보 플레이어 용병 리스트에 저장
                            for (i = 0; i < recvUnit.Count; i++)
                            {
                                network.unitList.Add(recvUnit[i]);
                            }
                        }

                        Debug.Log($"용병 해고 결과 - 용병 데이터 처리 후 저장내용 확인: {network.unitList.Count}");

                        // 여기서 해고된 용병 데이터 처리
                        switch (network.unitList.Count) {
                            case 34:
                                Debug.Log($"Train.selectList[2]: {Train.selectList[2]}"); // 선택한 용병
                                Debug.Log($"network.unitList[2]: {network.unitList[2]}");
                                Debug.Log($"network.unitList[19]: {network.unitList[19]}");

                                // 선택한 캐릭터 정보
                                //for (i = 0; i < Train.selectList.Count; i++) {
                                //    Debug.Log($"선택한 용병 정보: i - {i}, 값 - {Train.selectList[i]}");
                                //}

                                // 용병 정보를 임시로 담을 리스트 생성
                                List<string> replaceUnitList = new List<string>();

                                // 용병 이름 조회 후 일치하는 용병 데이터값 제거 후 리스트 재생성
                                if (Train.selectList[2].Equals(network.unitList[2]) == true) {
                                    Debug.Log($"Train.selectList[2].Equals(network.unitList[2]): {Train.selectList[2].Equals(network.unitList[2])}");

                                    // 보유 중인 캐릭터 정보
                                    for (i = 17; i < network.unitList.Count; i++) {
                                        Debug.Log($"보유 중인 용병 정보: i - {i}, 값 - {network.unitList[i]}");
                                        replaceUnitList.Add(network.unitList[i]);
                                    }

                                } else if (Train.selectList[2].Equals(network.unitList[19]) == true) {
                                    Debug.Log($"Train.selectList[2].Equals(network.unitList[19]): {Train.selectList[2].Equals(network.unitList[19])}");

                                    // 보유 중인 캐릭터 정보
                                    for (i = 0; i < 17; i++) {
                                       // Debug.Log($"보유 중인 용병 정보: i - {i}, 값 - {network.unitList[i]}");
                                        replaceUnitList.Add(network.unitList[i]);
                                    }
                                }

                                // 최종 결과 리스트
                                //for (i = 0; i < replaceUnitList.Count; i++) {
                                //    Debug.Log($"최종 결과를 담은 용병 정보: i - {i}, 값 - {replaceUnitList[i]}");
                                //}

                                network.unitList.Clear();

                                // 해고 처리된 용병 정보를 리스트에 저장
                                network.unitList = replaceUnitList;
                                break;
                        }

                        // 용병 UI 최신화
                        unitListUpdate();
                        break;
                    case 20: // 서버에서 ArrayIndexOutOfBoundsException 발생시 호출, 인벤 정보 다시 전송
                        message = null;

                        enumerator = result.Values.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            object current = enumerator.Current;
                            message = current.ToString();

                        }
                        Debug.Log($"20- 서버에서온 메시지: {message}");

                        // 훈련소 정보 호출 후 전송
                        GameObject training = GameObject.Find("Training_school(Clone)");
                        TrainingCenter trainingCenter = training.GetComponent<TrainingCenter>();

                        // 매개변수: 플레이어 아이디, 소지금, 구매한 용병 이름
                        network.EmploymentEvent(network.player[1], network.player[5], trainingCenter.selectList[0]);

                        break;
                    case 21: // 서버에서 ArrayIndexOutOfBoundsException 발생시 호출, 인벤 정보 다시 전송
                        message = null;

                        enumerator = result.Values.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            object current = enumerator.Current;
                            message = current.ToString();

                        }
                        Debug.Log($"21 - 서버에서온 메시지: {message}");

                        // 훈련소 정보 호출 후 전송
                        training = GameObject.Find("Training_school(Clone)");
                        trainingCenter = training.GetComponent<TrainingCenter>();

                        Debug.Log($"21 - 예외처리 플레이어 - 아이디: {network.player[1]}, 돈: {network.player[5]}, 용병 이름: {trainingCenter.selectList[2]}");

                        // 매개변수: 플레이어 아이디, 소지금, 판매한 용병 이름
                        network.FireEvent(network.player[1], network.player[5], trainingCenter.selectList[2]);

                        //여기서 처리
                        Debug.Log($"21 - 예외처리 용병 리스트 UI 최신화: {network.unitList.Count}");

                        if(network.unitList[2].Equals(trainingCenter.selectList[2]) == true) {
                            Debug.Log($"21 - network.unitList[2]: {network.unitList[2]}");
                        } else if(network.unitList[19].Equals(trainingCenter.selectList[2]) == true) {
                            Debug.Log($"21 - network.unitList[19]: {network.unitList[19]}");
                        }

                        break;
                    case 9: //장비 에러
                        message = null;

                        enumerator = result.Values.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            object current = enumerator.Current;
                            message = current.ToString();

                        }
                        Debug.Log($"9 - 서버에서온 메시지: {message}");

                        // 서버에 장비 정보 전송
                        network.GeadInfoUpdate(network.player_gear);
                        network.InventoryInfoUpdate(network.inven);
                        break;
                    case 22: //채팅 보내기 에러
                        message = null;

                        enumerator = result.Values.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            object current = enumerator.Current;
                            message = current.ToString();

                        }
                        Debug.Log($"22 - 서버에서온 메시지: {message}");

                        // 서버에 채팅정보 재전송
                        network.ChatReSend();
                        break;
                    case 23: //서버 채팅 정보 호출
                        message = null;

                        enumerator = result.Values.GetEnumerator();
                        while (enumerator.MoveNext()) {
                            object current = enumerator.Current;
                            message = current.ToString();

                        }
                        // Debug.Log($"23 - 서버에서온 메시지: {message}");
                        
                        // Object -> List<Chat> 타입으로 변환
                        List<Chat> chatList = JsonConvert.DeserializeObject<List<Chat>>(message);


                        // 여기서 시간 비교?
                        // 클라이언트 마지막 채팅 로그랑 서버에서 온 채팅 로그 비교 한 뒤 추가
                        // 처음 메인 화면에 접속할 경우 갯수가 0
                        // Debug.Log($"채팅 로그 갯수 - chatQueue.Count: {chatQueue.Count}");
                        // Debug.Log($"현재 클라이언트 채팅 로그 내용 출력");
                        if (chatQueue.Count == 0) {

                            //메세지를 한개 보낼 경우?, 이전 로그랑 비교해서 같은 부분이 없다면 추가 [chatList = 서버에서 온 데이터]
                            for (i = 0; i < chatList.Count; i++)
                            {
                                Debug.Log($"chatList[{i}] - playerId: {chatList[i].playerId}, content: {chatList[i].content}, time: {chatList[i].time} ");

                                //Queue에 데이터 저장
                                chatQueue.Enqueue(chatList[i]);

                                // 채팅 내용 추가
                                GameObject chat = Managers.Resource.Instantiate("UI/WhiteArea");
                                chat.transform.SetParent(mainChat.transform.GetChild(0).GetChild(0).GetChild(0).transform);

                                // 캐릭터 명
                                chat.transform.GetChild(0).GetComponent<Text>().text = chatList[i].playerId;

                                // 메시지 내용
                                chat.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = chatList[i].content;

                                // yyyy-MM-dd HH:mm:ss
                                DateTime dt = DateTime.ParseExact(chatList[i].time, "yyyy-MM-dd HH:mm:ss", null);
                                // Debug.Log("오후 "+dt.Hour+":"+dt.Minute);
                                
                                // 메시지 시간
                                // chat.transform.GetChild(2).GetChild(2).GetComponent<Text>().text = chatList[i].time;
                                chat.transform.GetChild(2).GetChild(2).GetComponent<Text>().text = " 오후 "+dt.Hour+":"+dt.Minute;
                                
                                RectTransform chat_rect = (RectTransform)chat.transform;
                                chat_rect.anchoredPosition = new Vector3(0f, 0f);
                                chat_rect.localScale = new Vector3(1f, 1f, 1f);
                            }
                        } else {
                            Debug.Log($"채팅 로그 갯수[1개 이상일 경우] - chatQueue.Count: {chatQueue.Count}");

                            Chat clientInfo = new Chat();

                            // 클라이언트 마지막 채팅 로그 조회 및 저장
                            foreach (Chat chatInfo in chatQueue) {
                                clientInfo.playerId = chatInfo.playerId;
                                clientInfo.content = chatInfo.content;
                                clientInfo.time = chatInfo.time;
                            }


                            // Debug.Log($"클라이언트 큐 마지막 채팅 정보 - clientInfo: {clientInfo.playerId}, content: {clientInfo.content}, time: {clientInfo.time} ");

                            // 처음 접속 후 채팅 보낼 경우 리턴 값이 0 따라서 실행 안됨
                            // 참고: https://blog.naver.com/doghole/100117144255
                            DateTime t1 = DateTime.Parse(clientInfo.time.Trim());

                            for (i = 0; i < chatList.Count; i++) {
                                // Debug.Log($"서버 큐 정보 - chatList[{i}] - playerId: {chatList[i].playerId}, content: {chatList[i].content}, time: {chatList[i].time} ");

                                DateTime t2 = DateTime.Parse(chatList[i].time.Trim());
                 
                                if (DateTime.Compare(t1, t2) < 0) {
                                    // Debug.Log("t1 < t2");

                                    // 시간 비교 후 추가, Queue에 데이터 저장
                                    chatQueue.Enqueue(chatList[i]);

                                    // 채팅 내용 추가
                                    GameObject chat = Managers.Resource.Instantiate("UI/WhiteArea");
                                    chat.transform.SetParent(mainChat.transform.GetChild(0).GetChild(0).GetChild(0).transform);

                                    // 캐릭터 명
                                    chat.transform.GetChild(0).GetComponent<Text>().text = chatList[i].playerId;

                                    // 메시지 내용
                                    chat.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = chatList[i].content;

                                    // 메시지 시간
                                    chat.transform.GetChild(2).GetChild(2).GetComponent<Text>().text = chatList[i].time;

                                    RectTransform chat_rect = (RectTransform)chat.transform;
                                    chat_rect.anchoredPosition = new Vector3(0f, 0f);
                                    chat_rect.localScale = new Vector3(1f, 1f, 1f);
                                }
                            }
                        }

                        break;
                    case 24: //캐릭터 정보 호출
                        message = null;

                        enumerator = result.Values.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            object current = enumerator.Current;
                            message = current.ToString();

                        }
                        Debug.Log($"24 - 서버에서온 메시지: {message}");

                        // 플레이어 정보 재전송
                        network.GeadInfoUpdate(network.player_gear);
                        network.InventoryInfoUpdate(network.inven);
                        network.PlayerStatUpdate();
                        break;
                    default:
                        Debug.Log($"기타 상황: {Key}");
                        break;
                }
            }
        }


        //마우스 위치가 플레이어 이미지 위에 존재하는지 체크
        bool isPlayer = RectTransformUtility.RectangleContainsScreenPoint(unitListUI.transform.GetChild(0).transform as RectTransform, Input.mousePosition);
        if (isPlayer == true)
        {
            if (infoPopup == null)
            {
                infoPopup = Managers.Resource.Instantiate("InfoPopup");
                infoPopup.transform.GetChild(0).transform.position = new Vector3(unitListUI.transform.GetChild(0).transform.position.x + 110, unitListUI.transform.GetChild(0).transform.position.y, 0);

                //캐릭터명
                infoPopup.transform.GetChild(0).GetChild(0).GetChild(0).transform.GetComponent<Text>().text = network.player[1];

                //레벨
                infoPopup.transform.GetChild(0).GetChild(0).GetChild(2).transform.GetComponent<Text>().text = network.player[3];

                //체력
                infoPopup.transform.GetChild(0).GetChild(1).GetChild(1).transform.GetComponent<Text>().text = network.player[6];
                infoPopup.transform.GetChild(0).GetChild(1).GetChild(3).transform.GetComponent<Text>().text = network.player[7];

                //마력
                infoPopup.transform.GetChild(0).GetChild(2).GetChild(1).transform.GetComponent<Text>().text = network.player[8];
                infoPopup.transform.GetChild(0).GetChild(2).GetChild(3).transform.GetComponent<Text>().text = network.player[9];

                //경험치
                infoPopup.transform.GetChild(0).GetChild(3).GetChild(1).transform.GetComponent<Text>().text = network.player[10];
                infoPopup.transform.GetChild(0).GetChild(3).GetChild(3).transform.GetComponent<Text>().text = network.player[11];
            }
            //Debug.Log("infoPopup.transform.GetChild(0).transform.position: "+ infoPopup.transform.GetChild(0).transform.position);
            //Debug.Log("Input.mousePosition: " + Input.mousePosition);
        }
        else {
            if (infoPopup != null) {
                Managers.Resource.Destroy(infoPopup);
                infoPopup = null;
            }
        }
    }

    void unitListUpdate() {
        Debug.Log($"용병 데이터 처리 후 용병 리스트 UI 최신화: {network.unitList.Count}");

        // 훈련소 창 화면 정보를 가져와서 생성
        TrainingSchool = GameObject.Find("Training_school(Clone)");

        //Debug.Log($"unitListUI.transform.GetChildCount(): {unitListUI.transform.childCount}");
        Debug.Log($"TrainingSchool 버튼값: {TrainingSchool.transform.GetChild(0).GetChild(6).name}");

        // 플레이어 용병 정보에 따라 UI 처리 및 데이터 저장
        switch (network.unitList.Count)  {
            case 0: case 1:
                // 기존 빈 공간 제거
                // Debug.Log($"unitListUI.transform.GetChild(1).gameObject: {unitListUI.transform.GetChild(2).gameObject.name
                Managers.Resource.Destroy(unitListUI.transform.GetChild(1).gameObject);
                Managers.Resource.Destroy(unitListUI.transform.GetChild(2).gameObject);

                GameObject mercenary_Slot_1_Empty1 = Managers.Resource.Instantiate($"Mercenary_Slot_1_Empty");
                mercenary_Slot_1_Empty1.transform.SetParent(unitListUI.transform);
                RectTransform rtc1 = (RectTransform)mercenary_Slot_1_Empty1.transform;
                rtc1.anchoredPosition = new Vector3(0f, 0f);
                rtc1.localScale = new Vector3(1f, 1f, 1f);

                GameObject mercenary_Slot_2_Empty = Managers.Resource.Instantiate($"Mercenary_Slot_2_Empty");
                mercenary_Slot_2_Empty.transform.SetParent(unitListUI.transform);
                RectTransform rt2 = (RectTransform)mercenary_Slot_2_Empty.transform;
                rt2.anchoredPosition = new Vector3(0f, 0f);
                rt2.localScale = new Vector3(1f, 1f, 1f);

                break;
            case 17:// 한 명일 경우
                Debug.Log($"용병 데이터 처리 후 저장내용 확인 - network.unitList.Count: {network.unitList.Count}");

                Debug.Log($"unitListUI.transform.GetChild(1).transform.name: {unitListUI.transform.GetChild(1).transform.name}");

                // 기존 빈 공간 제거
                Managers.Resource.Destroy(unitListUI.transform.GetChild(1).gameObject);
                Managers.Resource.Destroy(unitListUI.transform.GetChild(2).gameObject);

                // 용병 ui 생성
                GameObject Mercenary_011 = Managers.Resource.Instantiate($"Mercenary_01");
                Mercenary_011.transform.SetParent(unitListUI.transform);
                Mercenary_011.transform.SetSiblingIndex(1);
                RectTransform rtc11 = (RectTransform)Mercenary_011.transform;
                rtc11.anchoredPosition = new Vector3(0f, 0f);
                rtc11.localScale = new Vector3(1f, 1f, 1f);

                Debug.Log($"unitList[1]: {network.unitList[1]}");

                if (network.unitList[1].Equals("마법사") == true) {
                    Mercenary_011.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/magic") as Sprite;
                } else {
                    Mercenary_011.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/ui_icon_hero_arthur") as Sprite;
                }

                //플레이어 체력바 설정
                unitListUI.transform.GetChild(1).GetChild(0).GetComponent<Slider>().maxValue = float.Parse(network.unitList[5]);
                unitListUI.transform.GetChild(1).GetChild(0).GetComponent<Slider>().value = float.Parse(network.unitList[4]);

                //플레이어 마력바 설정
                unitListUI.transform.GetChild(1).GetChild(1).GetComponent<Slider>().maxValue = float.Parse(network.unitList[7]);
                unitListUI.transform.GetChild(1).GetChild(1).GetComponent<Slider>().value = float.Parse(network.unitList[6]);


                //슬롯2에 다른 용병 이미지가 있을 경우에만 실행 
                //Debug.Log($"unitListUI.transform.GetChild(2).transform.name: {unitListUI.transform.GetChild(2).transform.name}");


                // 슬롯 2 빈공간 생성
                mercenary_Slot_2_Empty = Managers.Resource.Instantiate($"Mercenary_Slot_2_Empty");
                mercenary_Slot_2_Empty.transform.SetParent(unitListUI.transform);
                rt2 = (RectTransform)mercenary_Slot_2_Empty.transform;
                rt2.anchoredPosition = new Vector3(0f, 0f);
                rt2.localScale = new Vector3(1f, 1f, 1f);
                

                break;
            case 34:// 두 명째 구입할 경우
                Debug.Log($"용병 두 명일 경우");

                // 기존 빈 공간 제거
                Managers.Resource.Destroy(unitListUI.transform.GetChild(2).gameObject);

                // 용병 UI 생성
                GameObject Mercenary_012 = Managers.Resource.Instantiate($"Mercenary_02");
                Mercenary_012.transform.SetParent(unitListUI.transform);
                Mercenary_012.transform.SetSiblingIndex(2);
                RectTransform rtc12 = (RectTransform)Mercenary_012.transform;
                rtc12.anchoredPosition = new Vector3(0f, 0f);
                rtc12.localScale = new Vector3(1f, 1f, 1f);

                Debug.Log($"unitList[19]: {network.unitList[19]}");

                if (network.unitList[19].Equals("마법사") == true)
                {
                    Mercenary_012.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/magic") as Sprite;
                }
                else
                {
                    Mercenary_012.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/ui_icon_hero_arthur") as Sprite;
                }

                //플레이어 체력바 설정
                unitListUI.transform.GetChild(2).GetChild(0).GetComponent<Slider>().maxValue = float.Parse(network.unitList[23]);
                unitListUI.transform.GetChild(2).GetChild(0).GetComponent<Slider>().value = float.Parse(network.unitList[22]);

                //플레이어 마력바 설정
                unitListUI.transform.GetChild(2).GetChild(1).GetComponent<Slider>().maxValue = float.Parse(network.unitList[25]);
                unitListUI.transform.GetChild(2).GetChild(1).GetComponent<Slider>().value = float.Parse(network.unitList[24]);

                break;
            default:
                Debug.Log($"unitListUpdate - network.unitList.Count: {network.unitList.Count}");
                break;
        }
        Debug.Log($"unitListUpdate - 버튼명: {TrainingSchool.transform.GetChild(0).GetChild(6).name}");

        //훈련소 ui 수정
        if (TrainingSchool.transform.GetChild(0).GetChild(6).name.Equals("Buy_Button(Clone)") == true || TrainingSchool.transform.GetChild(0).GetChild(6).name.Equals("Buy_Button") == true) {
        } else {

            // 플레이어 용병 정보에 따라 UI 처리 및 데이터 저장
            switch (network.unitList.Count)
            {
                case 0:
                case 1:
                    // 용병 UI 기존 이미지 제거
                    Managers.Resource.Destroy(TrainingSchool.transform.GetChild(0).GetChild(4).GetChild(0).gameObject);
                    break;
                case 17:// 한 명일 경우
                    Debug.Log($"훈련소 ui - 용병 한 명일 경우");

                    Managers.Resource.Destroy(TrainingSchool.transform.GetChild(0).GetChild(4).GetChild(0).gameObject);
                    Managers.Resource.Destroy(TrainingSchool.transform.GetChild(0).GetChild(4).GetChild(1).gameObject);

                    // 기본 용병 생성, Employment_Slot_1 = 검투사
                    GameObject Employment_Slot_1 = Managers.Resource.Instantiate($"Employment_Slot_1");
                    Employment_Slot_1.GetComponent<Button>().onClick.AddListener(PlayerUnitSlot_1_Event);

                    if (network.unitList[1].Equals("마법사") == true) {
                        Employment_Slot_1.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/magic") as Sprite;
                    } else {
                        Employment_Slot_1.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/ui_icon_hero_arthur") as Sprite;
                    }

                    Employment_Slot_1.transform.SetParent(TrainingSchool.transform.GetChild(0).GetChild(4));
                    RectTransform rtc112 = (RectTransform)Employment_Slot_1.transform;
                    rtc112.anchoredPosition = new Vector3(0f, 0f);
                    rtc112.localScale = new Vector3(1f, 1f, 1f);
                    break;
                case 34:// 두 명째 구입할 경우
                    Debug.Log($"훈련소 ui - 용병 두 명일 경우");

                    break;
                default:
                    Debug.Log($"unitListUpdate - network.unitList.Count: {network.unitList.Count}");
                    break;
            }
        }

    }
    
    public void PlayerUnitSlot_1_Event()
    {
        Debug.Log($"PlayerUnitSlot_1_Event 실행 - network.unitList.Count: {network.unitList.Count}");
    }

    // 룸 정보 최신화 시켜주는 함수
    void roomUpdate() {
        TimePanel.GetComponent<Text>().text = DateTime.Now.ToString(("yyyy-MM-dd HH:mm:ss"));
        moneyText.text = network.player[5];
        network.Spawn();

        //Debug.Log($"unitListUI.transform.GetChild(1).name: {unitListUI.transform.GetChild(1).name}");
        if (unitListUI.transform.GetChild(1).name.Equals("Mercenary_Slot_1_Empty") != true) {
            //Debug.Log($"unitListUI.transform.GetChild(1).name: {unitListUI.transform.GetChild(1).name}");
            //Debug.Log($"unitList[5]: {network.unitList[5]}");
            //플레이어 체력바 설정
            unitListUI.transform.GetChild(1).GetChild(0).GetComponent<Slider>().maxValue = float.Parse(network.unitList[5]);
            unitListUI.transform.GetChild(1).GetChild(0).GetComponent<Slider>().value = float.Parse(network.unitList[4]);

            //플레이어 마력바 설정
            unitListUI.transform.GetChild(1).GetChild(1).GetComponent<Slider>().maxValue = float.Parse(network.unitList[7]);
            unitListUI.transform.GetChild(1).GetChild(1).GetComponent<Slider>().value = float.Parse(network.unitList[6]);
        }

        //Debug.Log($"unitListUI.transform.GetChild(2).name: {unitListUI.transform.GetChild(2).name}");
        if (unitListUI.transform.GetChild(2).name.Equals("Mercenary_Slot_2_Empty") != true) {
            // Debug.Log($"unitList[17]: {network.unitList[17]}");

            //플레이어 체력바 설정
            unitListUI.transform.GetChild(2).GetChild(0).GetComponent<Slider>().maxValue = float.Parse(network.unitList[21]);
            unitListUI.transform.GetChild(2).GetChild(0).GetComponent<Slider>().value = float.Parse(network.unitList[20]);

            //플레이어 마력바 설정
            unitListUI.transform.GetChild(2).GetChild(1).GetComponent<Slider>().maxValue = float.Parse(network.unitList[23]);
            unitListUI.transform.GetChild(2).GetChild(1).GetComponent<Slider>().value = float.Parse(network.unitList[22]);
        }
    }

    // 룸에 존재하는 플레이어 정보를 최신화 시켜주는 함수
    public void RoomInfo(List<Player> players) {
         //Debug.Log($"맵에 존재하는 플레이어 수: {players.Count}");
       
        for (int i =0; i < players.Count;i++) {
            // Debug.Log($"players[{i}].Id  값: {players[0].Id}");
            // 내 캐릭터일 경우 위치 정보 변경
            if (network.player[1] == players[i].Id) {
               // transform.position = new Vector3(players[i].x, players[i].y, players[i].z);
            } else {
                // 타 플레이어 일 경우 있으면 좌표 변경, 없으면 생성(다른 플레이어 프리팹 로드 후 클론 생성) 후 좌표 입력
                OrderPlayer order = null;
                // 생성된 유저 체크: 유저가 존재할 경우 true 실행, 좌표 수정
                if (_orderPlayers.TryGetValue(players[i].Id, out order))
                {
                  //  Debug.Log("다른 플레이어 프리팹이 이미 존재하는 경우");
                    order.state = players[i].State;
                    order.destPos = new Vector3(players[i].DestPos_x, players[i].DestPos_y, players[i].DestPos_z);
                    //order.transform.rotation = Quaternion.LookRotation(new Vector3(players[i].DestPos_x, players[i].DestPos_y, players[i].DestPos_z));
                    order.transform.position = new Vector3(players[i].x, players[i].y, players[i].z);
                } else {
                  //  Debug.Log("다른 플레이어 프리팹이 존재하지 않은 경우");
                    // 다른 플레이어 객체 정보 검색
                    // 있으면 좌표 변경, 없으면 생성 후 좌표 입력
                    // 다른 플레이어 프리팹 로드 후 클론 생성
                    // 프리팹 로드 후 클론 생성
                    if (players[i].Gender.Equals("남자 캐릭터") == true) {
                        GameObject orderPlayer = Managers.Resource.Instantiate("OrderPlayer_Male");
                        order = orderPlayer.AddComponent<OrderPlayer>();
                        order.PlayerId = players[i].Id;
                        order.state = players[i].State;
                        order.destPos = new Vector3(players[i].DestPos_x, players[i].DestPos_y, players[i].DestPos_z);
                        order.transform.position = new Vector3(players[i].x, players[i].y, players[i].z);

                        _orderPlayers.Add(order.PlayerId, order);
                    } else {
                        GameObject orderPlayer = Managers.Resource.Instantiate("OrderPlayer_Female");
                        order = orderPlayer.AddComponent<OrderPlayer>();
                        order.PlayerId = players[i].Id;
                        order.state = players[i].State;
                        order.destPos = new Vector3(players[i].DestPos_x, players[i].DestPos_y, players[i].DestPos_z);
                        order.transform.position = new Vector3(players[i].x, players[i].y, players[i].z);

                        _orderPlayers.Add(order.PlayerId, order);
                    }
                }
            }
        }

       
    }

    // 플레이어 캐릭터 이미지 누를 경우 실행, 캐릭터 정보 호출
    public void PlayerInfo() { 
        Debug.Log("PlayerInfo 클릭");
        if(characterState == null) {
            characterState = Managers.Resource.Instantiate("CharacterState");

            // 캐릭터 이름
            characterState.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>().text = network.player[1];

            // 레벨
            characterState.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Text>().text = network.player[3];

            //경험치, 최대 경험치
            characterState.transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<Text>().text = network.player[10];
            characterState.transform.GetChild(0).GetChild(2).GetChild(3).GetComponent<Text>().text = network.player[11];


            //캐릭터 장비 조회 후 능력치 상승
            // 캐릭터 능력치 업데이트(클라 수정 -> 서버 수정)
            /**
            0: 머리
            1: 무기
            2: 갑옷
            3: 허리
            4: 왼쪽 반지
            5: 신발
            6: 오른쪽 반지
             */
            for (int i = 0; i < network.player_gear.Count; i++) {
                // Debug.Log($"장비 정보 조회 - 키: {i} 값: {network.player_gear[i]}");

                switch (i) {
                    case 0:// 0: 머리,  힘 - 아이템을 장착할 경우 증가
                        characterState.transform.GetChild(0).GetChild(7).GetChild(1).GetComponent<Text>().text = network.player[14];

                        if (network.player_gear[i].Equals("head_1") == true) {
                            characterState.transform.GetChild(0).GetChild(7).GetChild(4).GetComponent<Text>().text = "5";
                        } else if (network.player_gear[i].Equals("head_2") == true) {
                            characterState.transform.GetChild(0).GetChild(7).GetChild(4).GetComponent<Text>().text = "10";
                        } else if (network.player_gear[i].Equals("head_3") == true) { 
                            characterState.transform.GetChild(0).GetChild(7).GetChild(4).GetComponent<Text>().text = "15";
                        } else { 
                            characterState.transform.GetChild(0).GetChild(7).GetChild(4).GetComponent<Text>().text = "0";
                        }
                        break;
                    case 1:// 1. 무기,  공격력 - 아이템을 장착할 경우 증가
                        characterState.transform.GetChild(0).GetChild(5).GetChild(1).GetComponent<Text>().text = network.player[12];

                        if (network.player_gear[i].Equals("weapon_1") == true) { 
                            characterState.transform.GetChild(0).GetChild(5).GetChild(4).GetComponent<Text>().text = "5";
                        } else if (network.player_gear[i].Equals("weapon_2") == true) {
                            characterState.transform.GetChild(0).GetChild(5).GetChild(4).GetComponent<Text>().text = "10";
                        } else if (network.player_gear[i].Equals("weapon_3") == true) {
                            characterState.transform.GetChild(0).GetChild(5).GetChild(4).GetComponent<Text>().text = "15";
                        } else {
                            characterState.transform.GetChild(0).GetChild(5).GetChild(4).GetComponent<Text>().text = "0";
                        }
                        break;
                    case 2:// 2: 갑옷,  방어력 - 아이템을 장착할 경우 증가
                        characterState.transform.GetChild(0).GetChild(6).GetChild(1).GetComponent<Text>().text = network.player[13];

                        if (network.player_gear[i].Equals("body_1") == true) {
                            characterState.transform.GetChild(0).GetChild(6).GetChild(4).GetComponent<Text>().text = "5";
                        } else if (network.player_gear[i].Equals("body_2") == true) {
                            characterState.transform.GetChild(0).GetChild(6).GetChild(4).GetComponent<Text>().text = "10";
                        } else if (network.player_gear[i].Equals("body_3") == true) {
                            characterState.transform.GetChild(0).GetChild(6).GetChild(4).GetComponent<Text>().text = "15";
                        } else {
                            characterState.transform.GetChild(0).GetChild(6).GetChild(4).GetComponent<Text>().text = "0";
                        }
                        break;
                    case 3:// 3: 허리, 생명력 - 아이템을 장착할 경우 증가
                        characterState.transform.GetChild(0).GetChild(9).GetChild(1).GetComponent<Text>().text = network.player[16];

                        if (network.player_gear[i].Equals("waist_1") == true) {
                            characterState.transform.GetChild(0).GetChild(9).GetChild(4).GetComponent<Text>().text = "5";
                        } else if (network.player_gear[i].Equals("waist_2") == true) {
                            characterState.transform.GetChild(0).GetChild(9).GetChild(4).GetComponent<Text>().text = "10";
                        } else if (network.player_gear[i].Equals("waist_3") == true) {
                            characterState.transform.GetChild(0).GetChild(9).GetChild(4).GetComponent<Text>().text = "15";
                        } else {
                            characterState.transform.GetChild(0).GetChild(9).GetChild(4).GetComponent<Text>().text = "0";
                        }
                        break;
                    case 5:// 5: 신발, 민첩성
                        characterState.transform.GetChild(0).GetChild(8).GetChild(1).GetComponent<Text>().text = network.player[15];

                        // 민첩성 - 아이템을 장착할 경우 증가
                        characterState.transform.GetChild(0).GetChild(8).GetChild(4).GetComponent<Text>().text = "0";
                        break;
                    case 4: case 6:// 4: 왼쪽 반지, 6: 오른쪽 반지, 마력 - 아이템을 장착할 경우 증가
                        characterState.transform.GetChild(0).GetChild(4).GetChild(1).GetComponent<Text>().text = network.player[8];
                        characterState.transform.GetChild(0).GetChild(4).GetChild(3).GetComponent<Text>().text = network.player[9];
                        break;
                    default:
                        break;
                }
            }

            // 장비 조회 후 능력치 증가
            // 레벨 별 기본스텟 조회 후 증가, 감소 진행
            //체력, 최대 체력
            // 허리: 체력
            characterState.transform.GetChild(0).GetChild(3).GetChild(1).GetComponent<Text>().text = network.player[6];
            characterState.transform.GetChild(0).GetChild(3).GetChild(3).GetComponent<Text>().text = network.player[7];


            // 지력
            characterState.transform.GetChild(0).GetChild(10).GetChild(1).GetComponent<Text>().text = network.player[17];

            // 지력 - 아이템을 장착할 경우 증가
            characterState.transform.GetChild(0).GetChild(10).GetChild(4).GetComponent<Text>().text = "0";

            //창닫기 버튼에 이벤트 추가
            characterState.transform.GetChild(0).GetChild(11).GetComponent<Button>().onClick.AddListener(Cancel);
        } else {
            Managers.Resource.Destroy(characterState);
        }
    }

    // 캐릭터 정보창 닫아주는 함수
    private void Cancel() {
        Managers.Resource.Destroy(characterState);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        Debug.Log("OnPointerEnter: "+eventData.pointerCurrentRaycast.gameObject.name);
    }

    void OnDestroy() { 
        CancelInvoke("roomUpdate");
    }

    // 누군가가 떠날때 실행
    //public void LeaveGame(S_BroadcastLeaveGame packet)
    //{
    //    if (_myPlayer.PlayerId == packet.playerId)//내가 나갈 경우
    //    {
    //        GameObject.Destroy(_myPlayer.gameObject);
    //        _myPlayer = null;
    //    }
    //    else
    //    {// 타 유저가 나갈 경우
    //        Player player = null;
    //        if (_players.TryGetValue(packet.playerId, out player))
    //        {
    //            GameObject.Destroy(player.gameObject);
    //            _players.Remove(packet.playerId);
    //        }
    //    }
    //}
}