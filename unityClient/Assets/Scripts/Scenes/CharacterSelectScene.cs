using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 알파값 변경 참고: http://devkorea.co.kr/bbs/board.php?bo_table=m03_qna&wr_id=75422
public class CharacterSelectScene : MonoBehaviour
{
    NetworkManager network;
    List<string> players;
    GameObject slot1;
    GameObject slot2;
    bool isCheck = false;
    List<string> selectPlayer1;
    List<string> selectPlayer2;

    void Start()
    {
        players = new List<string>();

        //버튼에 이벤트 추가
        GameObject connect = GameObject.Find("StartButton").gameObject;
        connect.GetComponent<Button>().onClick.AddListener(ConnectAction);

        GameObject back = GameObject.Find("BackButton").gameObject;
        back.GetComponent<Button>().onClick.AddListener(BackAction);

        GameObject create = GameObject.Find("CreateButton").gameObject;
        create.GetComponent<Button>().onClick.AddListener(CreateAction);

        GameObject delete = GameObject.Find("DeleteButton").gameObject;
        delete.GetComponent<Button>().onClick.AddListener(DeleteAction);

        slot1 = GameObject.Find("CharacterSlot1").gameObject;
        slot1.GetComponent<Button>().onClick.AddListener(Slot1Action);

        slot2 = GameObject.Find("CharacterSlot2").gameObject;
        slot2.GetComponent<Button>().onClick.AddListener(Slot2Action);

        //NetworkManager 파일이 없다면 생성
        GameObject net = GameObject.Find("@NetworkManager");

        // 네트워크 매니저 send 메소드 접근
        network = net.GetComponent<NetworkManager>();
        Debug.Log("CharacterSelectScene - network.Id: " + network.Id);

        Dictionary<int, string> sendData = new Dictionary<int, string>();
        string send = network.Id as string;
        sendData.Add(12, send);
        network.CharacterCheck(sendData);
    }

    private void Slot1Action()
    {
        //Debug.Log("Slot1Action 클릭");
        isCheck = true;
        RawImage slot2Image = slot2.GetComponent<RawImage>();
        RawImage slot1Image = slot1.GetComponent<RawImage>();
        selectPlayer1 = new List<string>();
        selectPlayer2 = null;

        if (players.Count >= 16)
        {
            Debug.Log("slot1 - 닉네임 정보: " + slot1.transform.GetChild(1).GetChild(0).GetComponent<Text>().text);
            Debug.Log("slot1 - 캐릭터 이미지 정보: " + slot1.transform.GetChild(0).GetComponent<Image>().sprite.name);
            Debug.Log("slot1 - 레벨 정보: " + slot1.transform.GetChild(2).GetChild(0).GetComponent<Text>().text);

            selectPlayer1.Add(slot1.transform.GetChild(1).GetChild(0).GetComponent<Text>().text);
            selectPlayer1.Add(slot1.transform.GetChild(0).GetComponent<Image>().sprite.name);
            selectPlayer1.Add(slot1.transform.GetChild(2).GetChild(0).GetComponent<Text>().text);
        }

        if (slot2Image.color.a == 255)
        {
            slot2Image.color = new Color(slot2Image.color.r, slot2Image.color.g, slot2Image.color.b, 0);
        }

        if (slot1Image.color.a != 255)
        {
            slot1Image.color = new Color(slot1Image.color.r, slot1Image.color.g, slot1Image.color.b, 255.0f);
        }
    }

    private void Slot2Action()
    {
        //Debug.Log("Slot2Action 클릭");
        isCheck = true;
        RawImage slot1Image = slot1.GetComponent<RawImage>();
        RawImage slot2Image = slot2.GetComponent<RawImage>();
        selectPlayer1 = null;
        selectPlayer2 = new List<string>();

        if (players.Count == 32)
        {
            Debug.Log("slot2 - 닉네임 정보: " + slot2.transform.GetChild(1).GetChild(0).GetComponent<Text>().text);
            Debug.Log("slot2 - 캐릭터 이미지 정보: " + slot2.transform.GetChild(0).GetComponent<Image>().sprite.name);
            Debug.Log("slot2 - 레벨 정보: " + slot2.transform.GetChild(2).GetChild(0).GetComponent<Text>().text);

            selectPlayer2.Add(slot2.transform.GetChild(1).GetChild(0).GetComponent<Text>().text);
            selectPlayer2.Add(slot2.transform.GetChild(0).GetComponent<Image>().sprite.name);
            selectPlayer2.Add(slot2.transform.GetChild(2).GetChild(0).GetComponent<Text>().text);
        }

        if (slot1Image.color.a == 255)
        {
            slot1Image.color = new Color(slot1Image.color.r, slot1Image.color.g, slot1Image.color.b, 0);
        }

        if(slot2Image.color.a != 255) {
            slot2Image.color = new Color(slot2Image.color.r, slot2Image.color.g, slot2Image.color.b, 255.0f);
        }
    }

    private void LateUpdate()
    {
        List<Dictionary<int, object>> list = Queue.Instance.PopAll();// 꺼내옴
        foreach (Dictionary<int, object> result in list)
        {
            foreach (int Key in result.Keys)
            {
                object value = result[Key];
                Debug.Log("Key: " + Key);
                Debug.Log("value: " + value);

                if(Key != 13)
                {
                    IEnumerator e = result.Values.GetEnumerator();
                    while (e.MoveNext())
                    {
                        object current = e.Current;
                        //Debug.Log("12 - current: " + current);
                        players = JsonConvert.DeserializeObject<List<string>>(current.ToString());
                        PlayerCheck(players);
                    }
                } else
                {

                }

            }
        }
    }

    void PlayerCheck(List<string> players)
    {
        Debug.Log("players.Count: " + players.Count);

        if (players.Count == 16)
        {
            Debug.Log("플레이어 캐릭터가 한 개 일 경우");

            //닉네임 변경
            slot1.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = players[1];

            //캐릭터 이미지 정보
            if (players[2].Equals("남자 캐릭터") == true)
            {
                slot1.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/player_male") as Sprite;
            }
            else
            {
                slot1.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/player_female") as Sprite;
            }

            slot1.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = "level: " + players[3];
            slot1.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "소지금: " + players[5];
        }
        else if (players.Count == 32)
        {
            Debug.Log("플레이어 캐릭터가 2개 일 경우");

            //닉네임 변경
            slot1.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = players[1];

            //캐릭터 이미지 정보
            if (players[2].Equals("남자 캐릭터") == true)
            {
                slot1.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/player_male") as Sprite;
            } else {
                slot1.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/player_female") as Sprite;
            }

            slot1.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = "level: " + players[3]; 
            slot1.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "소지금: " + players[5];

            //닉네임 변경
            slot2.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = players[17];

            //캐릭터 이미지 정보
            if (players[18].Equals("남자 캐릭터") == true)
            {
                slot2.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/player_male") as Sprite;
            }
            else
            {
                slot2.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/player_female") as Sprite;
            }

            // 레벨
            slot2.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = "level: " + players[19];
            slot2.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "소지금: " + players[21];
        } else
        {
            Debug.Log("플레이어 캐릭터가 없을 경우");
        }

    }

    void ConnectAction()
    {
        Debug.Log("게임 접속 버튼 클릭");

        if (selectPlayer1 == null && selectPlayer2 != null)
        {
            Debug.Log("selectPlayer2.Count 캐릭터를 선택한 경우: " + selectPlayer2.Count);

            if (selectPlayer2.Count == 0)
            {
                Debug.Log("선택한 캐릭터 이미지에 정보가 없을 경우");
            }
            else
            {
                List<string> select = new List<string>();
                
                for (int i = 16; i < players.Count; i++)
                {
                    //Debug.Log($"players 번호: {i} , 값: {players[i]}");
                    select.Add(players[i]);
                }

                network.player = select;
                Debug.Log("network.player.Count(저장후): " + network.player.Count);
                SceneManager.LoadScene("MainScene");
            }
        }
        else if (selectPlayer1 != null && selectPlayer2 == null)
        {
            Debug.Log("selectPlayer1.Count 캐릭터를 선택한 경우: " + selectPlayer1.Count);

            if (selectPlayer1.Count == 0)
            {
                Debug.Log("선택한 캐릭터 이미지에 정보가 없을 경우");
            }
            else
            {
                List<string> select = new List<string>();
                // 경고창 출력한 후 확인 누를시에만 삭제되게 설정

                for (int i = 0; i < 16; i++)
                {
                   // Debug.Log($"players 번호: {i} , 값: {players[i]}");
                    select.Add(players[i]);
                }

                network.player = select;
                //Debug.Log("network.player.Count(저장후): " + network.player.Count);
                SceneManager.LoadScene("MainScene");
            }
        }
        else
        {
            Debug.Log("캐릭터를 선택 안한 경우");
        }

    }

    void BackAction()
    {
        // Debug.Log("뒤로가기 클릭");

        //소켓 연결 닫기 후 로그인 화면으로 이동
        GameObject.Destroy(GameObject.Find("@NetworkManager"));
        SceneManager.LoadScene("LoginScene");
    }

    void CreateAction()
    {
        if (selectPlayer2 != null && selectPlayer2.Count != 0) {
            Debug.Log("캐릭터를 생성할 수 없습니다.");
        } else
        {
            SceneManager.LoadScene("CharacterCreateScene");
        }
    }

    void DeleteAction()
    {
        Debug.Log("캐릭터 제거 클릭");

        if(selectPlayer1 == null && selectPlayer2 != null)
        {
            Debug.Log("selectPlayer2.Count 캐릭터를 선택한 경우: " + selectPlayer2.Count);

            if (selectPlayer2.Count == 0)
            {
                Debug.Log("선택한 캐릭터 이미지에 정보가 없을 경우");
            }
            else
            {
                // 경고창 출력한 후 확인 누를시에만 삭제되게 설정

                for (int i = 0; i < selectPlayer2.Count; i++)
                {
                    Debug.Log($"selectPlayer 번호: {i} , 값: {selectPlayer2[i]}");
                }

                Dictionary<int, List<string>> sendData = new Dictionary<int, List<string>>();

                selectPlayer2.Add(network.Id);
                sendData.Add(13, selectPlayer2);
                network.CharacterDelete(sendData);

                //닉네임 변경
                slot2.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "";

                //캐릭터 이미지 정보
                slot2.transform.GetChild(0).GetComponent<Image>().sprite = null;

                // 레벨
                slot2.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = "";
                slot2.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "";

                selectPlayer2 = null;
            }
        }
        else if (selectPlayer1 != null && selectPlayer2 == null)
        {
            Debug.Log("selectPlayer1.Count 캐릭터를 선택한 경우: " + selectPlayer1.Count);

            if(selectPlayer1.Count == 0)
            {
                Debug.Log("선택한 캐릭터 이미지에 정보가 없을 경우");
            } else
            {
                // 경고창 출력한 후 확인 누를시에만 삭제되게 설정

                for (int i = 0; i < selectPlayer1.Count; i++)
                {
                    Debug.Log($"selectPlayer1 번호: {i} , 값: {selectPlayer1[i]}");
                }

                Dictionary<int, List<string>> sendData = new Dictionary<int, List<string>>();

                selectPlayer1.Add(network.Id);
                sendData.Add(13, selectPlayer1);
                network.CharacterDelete(sendData);

                //닉네임 변경
                slot1.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "";

                //캐릭터 이미지 정보
                slot1.transform.GetChild(0).GetComponent<Image>().sprite = null;

                // 레벨
                slot1.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = "";
                slot1.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "";

                selectPlayer1 = null;
            }
        }
        else
        {
            Debug.Log("캐릭터를 선택 안한 경우");
        }
    }
}