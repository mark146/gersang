using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainScene : MonoBehaviour, IPointerEnterHandler
{
    List<Player> players;
    Dictionary<string, OrderPlayer> _orderPlayers;
    GameObject TimePanel;
    Text moneyText;
    Text mainName;
    NetworkManager network;
    CameraController mainCamera;
    GameObject player;
    GameObject unitList;
    GameObject infoPopup;
    GameObject characterState;
    public string money { get; set; }

    void Awake()
    {
        players = new List<Player>();
        _orderPlayers = new Dictionary<string, OrderPlayer>();
        TimePanel = GameObject.Find("TimePanel");
        TimePanel = TimePanel.transform.GetChild(0).gameObject;
        moneyText = GameObject.Find("MoneyText").GetComponent<Text>();
        mainName = GameObject.Find("MainName").GetComponent<Text>();
        mainCamera = GameObject.Find("MainCamera").GetComponent<CameraController>();
        //용병창 리스트 검색
        unitList = GameObject.Find("UnitList");

        //NetworkManager 검색
        GameObject net = GameObject.Find("@NetworkManager");
        network = net.GetComponent<NetworkManager>();
        //for (int i = 0; i < network.player.Count; i++)
        //{
        //    Debug.Log($"network.player[{i}] : {network.player[i]} ");
        //}

        // 캐릭터 정보 연동
        mainName.text = network.player[1] as string;

        if(network.player[2].Equals("남자 캐릭터") == true)
        {
            player = Managers.Resource.Instantiate("Player_Male");
            unitList.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/player_male") as Sprite;

        } else
        { 
            player = Managers.Resource.Instantiate("Player_Female");
            unitList.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/player_female") as Sprite;
        }

        //플레이어 이미지에 버튼 이벤트 추가
        unitList.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(PlayerInfo);

        //플레이어 체력바 설정
        unitList.transform.GetChild(0).GetChild(0).GetComponent<Slider>().maxValue = float.Parse(network.player[5]);
        unitList.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value = float.Parse(network.player[5]);

        //플레이어 마력바 설정
        unitList.transform.GetChild(0).GetChild(1).GetComponent<Slider>().maxValue = float.Parse(network.player[6]);
        unitList.transform.GetChild(0).GetChild(1).GetComponent<Slider>().value = float.Parse(network.player[6]);

        this.money = network.player[5];
        mainCamera.SetPlayer(player);// 플레이어에게 카메라 뷰 연결
    }

    void FixedUpdate()
    {
        TimePanel.GetComponent<Text>().text = DateTime.Now.ToString(("yyyy-MM-dd HH:mm:ss"));
        moneyText.text = money;

        List<Dictionary<int, object>> list = Queue.Instance.PopAll();// 꺼내옴
        foreach (Dictionary<int, object> result in list)
        {
            foreach (int Key in result.Keys)
            {
                //Debug.Log("MainScene - Key: " + Key);

                switch (Key)
                {
                    case 4:
                       // Debug.Log("4 - result.Values: " + result.Values);
                        IEnumerator e = result.Values.GetEnumerator();
                        while (e.MoveNext())
                        {
                            object current = e.Current;
                           // Debug.Log("4 - current: " + current);
                            players = JsonConvert.DeserializeObject<List<Player>>(current.ToString());
                        }
                        UserMove(players);
                        break;
                    case 5:
                        // 참고: https://jw-stroy-at30.tistory.com/7
                        e = result.Values.GetEnumerator();
                        while (e.MoveNext())
                        {
                            object current = e.Current;
                           // Debug.Log("5 - result.Values: " + current);
                            players = JsonConvert.DeserializeObject<List<Player>>(current.ToString());
                        }
                        UserAdd(players);
                        break;
                    case 6:
                        string recv = null;

                        // 참고: https://jw-stroy-at30.tistory.com/7
                        e = result.Values.GetEnumerator();
                        while (e.MoveNext())
                        {
                            object current = e.Current;
                            //Debug.Log("current.ToString(): " + current.ToString());
                            recv = current.ToString();
                        }

                        List<string> inven = JsonConvert.DeserializeObject<List<string>>(recv);
                       // Debug.Log("inven.Count(): " + inven.Count());
                        Inventory.recvInven(inven);
                        break;
                    case 7:
                        string gearRecv = null;

                        // 참고: https://jw-stroy-at30.tistory.com/7
                        e = result.Values.GetEnumerator();
                        while (e.MoveNext())
                        {
                            object current = e.Current;
                            //Debug.Log("current.ToString(): " + current.ToString());
                            gearRecv = current.ToString();
                        }

                        List<string> gear = JsonConvert.DeserializeObject<List<string>>(gearRecv);
                      //  Debug.Log("gear.Count(): " + gear.Count());
                        Inventory.recvGear(gear);
                        break;
                    default:

                        break;
                }
            }
        }

        //마우스 위치가 플레이어 이미지 위에 존재하는지 체크
        bool isPlayer = RectTransformUtility.RectangleContainsScreenPoint(unitList.transform.GetChild(0).transform as RectTransform, Input.mousePosition);
        //Debug.Log("isPlayer: "+ isPlayer);
        if(isPlayer == true)
        {
            if(infoPopup == null)
            {
                infoPopup = Managers.Resource.Instantiate("InfoPopup");
                infoPopup.transform.GetChild(0).transform.position = new Vector3(unitList.transform.GetChild(0).transform.position.x + 110, unitList.transform.GetChild(0).transform.position.y, 0);

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
        } else
        {
            if (infoPopup != null)
            {
                Managers.Resource.Destroy(infoPopup);
                infoPopup = null;
            }
        }

    }

    // 게임 접속할 때 플레이어 목록이 옴
    public void UserAdd(List<Player> players)
    {
        foreach (Player player in players)
        {
            if (network.Id == player.Id)// 내 캐릭터
            {
               // Debug.Log("MainScene - RecvRegister: " + player.Id);
                transform.position = new Vector3(player.x, player.y, player.z);// 위치 세팅
                //_myPlayer = myPlayer;
            }
            else// 타 플레이어
            {
                // 다른 플레이어 객체 정보 검색
                // 있으면 좌표 변경, 없으면 생성 후 좌표 입력
                // 다른 플레이어 프리팹 로드 후 클론 생성

                OrderPlayer order = null;
                // 생성된 유저 체크: 유저가 존재할 경우 true 실행, 좌표 수정
                if (_orderPlayers.TryGetValue(player.Id, out order))
                {
                    Debug.Log("다른 플레이어 프리팹이 이미 존재하는 경우");
                    order.transform.position = new Vector3(player.x, player.y, player.z);
                }
                else
                {
                    Debug.Log("다른 플레이어 프리팹이 존재하지 않은 경우");
                    // 프리팹 로드 후 클론 생성
                    GameObject orderPlayer = Managers.Resource.Instantiate("OrderPlayer");
                    order = orderPlayer.AddComponent<OrderPlayer>();
                    order.PlayerId = player.Id;
                    order.transform.position = new Vector3(player.x, player.y, player.z);

                    _orderPlayers.Add(order.PlayerId, order);
                }
            }
        }
    }

    // 이동하면 실행
    public void UserMove(List<Player> players)
    {
        Debug.Log("UserMove 실행");
        /*
         이동 동기화 까다롭다. 
         1. 서버 쪽에서 허락 패킷이 왔을때 이동하는 방법
         2. 일단 플레이어 이동 시킨 후 서버에서 응답이 오면 보정
         */
        foreach (Player player in players)
        {

            if (name.Equals(player.Id) == true)// 내가 이동할 때, 
            {
                transform.position = new Vector3(player.x, player.y, player.z);// 위치 세팅
            }
            else
            {// 타 유저가 이동할 때
                OrderPlayer order = null;
                if (_orderPlayers.TryGetValue(player.Id, out order))// 체크 후 player 호출, true시 실행, 좌표 수정
                {
                    Debug.Log("타 유저가 이동할 때 - Move: " + player.Id);
                    order.transform.position = new Vector3(player.x, player.y, player.z);// 위치 세팅
                }
            }
        }
    }

    public void PlayerInfo()
    {
        Debug.Log("PlayerInfo 클릭");
        if(characterState == null)
        {
            characterState = Managers.Resource.Instantiate("CharacterState");

            //닉네임
            characterState.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>().text = network.player[1];

            //레벨
            characterState.transform.GetChild(0).GetChild(1).GetChild(3).GetComponent<Text>().text = network.player[3];

            //경험치, 최대 경험치
            characterState.transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<Text>().text = network.player[10];
            characterState.transform.GetChild(0).GetChild(2).GetChild(3).GetComponent<Text>().text = network.player[11];

            //체력, 최대 체력
            characterState.transform.GetChild(0).GetChild(3).GetChild(1).GetComponent<Text>().text = network.player[6];
            characterState.transform.GetChild(0).GetChild(3).GetChild(3).GetComponent<Text>().text = network.player[7];

            //마력, 최대 마력
            characterState.transform.GetChild(0).GetChild(4).GetChild(1).GetComponent<Text>().text = network.player[8];
            characterState.transform.GetChild(0).GetChild(4).GetChild(3).GetComponent<Text>().text = network.player[9];

            //힘
            characterState.transform.GetChild(0).GetChild(7).GetChild(1).GetComponent<Text>().text = network.player[12];

            //민첩성
            characterState.transform.GetChild(0).GetChild(8).GetChild(1).GetComponent<Text>().text = network.player[13];

            //생명력
            characterState.transform.GetChild(0).GetChild(9).GetChild(1).GetComponent<Text>().text = network.player[14];

            //지력
            characterState.transform.GetChild(0).GetChild(10).GetChild(1).GetComponent<Text>().text = network.player[15];

            //창닫기 버튼에 이벤트 추가
            characterState.transform.GetChild(0).GetChild(11).GetComponent<Button>().onClick.AddListener(Cancel);
        }
        else
        {
            Managers.Resource.Destroy(characterState);
        }
    }

    private void Cancel()
    {
        Managers.Resource.Destroy(characterState);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("OnPointerEnter: "+eventData.pointerCurrentRaycast.gameObject.name);
    }

    //내가 접속한 상태서 누군가 새로 접속할때 실행
    //public void EnterGame(S_BroadcastEnterGame packet)
    //{
    //    if (packet.playerId == _myPlayer.PlayerId)
    //        return;

    //    Object obj = Resources.Load("Player");
    //    GameObject go = Object.Instantiate(obj) as GameObject;

    //    Player player = go.AddComponent<Player>();
    //    player.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);
    //    _players.Add(packet.playerId, player);
    //}


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