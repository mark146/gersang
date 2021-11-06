using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//참고: https://mrbinggrae.tistory.com/102?category=818359
// https://m.blog.naver.com/PostView.nhn?blogId=dj3630&logNo=221551100952&proxyReferer=https:%2F%2Fwww.google.com%2F
// https://docs.unity3d.com/kr/2018.4/ScriptReference/EventSystems.PointerEventData-pointerPressRaycast.html
public class Inventory : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    static GameObject Inven;
    static GameObject gear;
    static GameObject head;
    static GameObject weapon;
    static GameObject body;
    static GameObject waist;
    static GameObject ring_left;
    static GameObject shoes;
    static GameObject ring_right;
    static bool isInvan = false;
    NetworkManager networkManager;
    GameObject cancel;
    GameObject character;
    GameObject selectItem;
    GameObject beforeSelectItemParent;
    GameObject beforeSelectItem;
    RectTransform isSlotRect = null;
    int isSlotIndex = 0;

    public static Dictionary<int, GameObject> invenSlot { get; set; }
    public static Dictionary<RectTransform, int> invenSlotRect { get; set; }
    public static Dictionary<int, GameObject> gearSlot { get; set; }
    public static Dictionary<RectTransform, int> gearSlotRect { get; set; }

    private void Start()
    {
        //창닫기 버튼에 이벤트 추가
        cancel = GameObject.Find("InfoExitButton").gameObject;
        cancel.GetComponent<Button>().onClick.AddListener(Cancel);
        Inven = transform.GetChild(0).GetChild(1).transform.gameObject;
        character = transform.GetChild(0).GetChild(0).transform.gameObject;
        gear = character.transform.GetChild(1).transform.gameObject;
        head = gear.transform.GetChild(1).gameObject;
        weapon = gear.transform.GetChild(3).gameObject;
        body = gear.transform.GetChild(4).gameObject;
        waist = gear.transform.GetChild(5).gameObject;
        ring_left = gear.transform.GetChild(6).gameObject;
        shoes = gear.transform.GetChild(7).gameObject;
        ring_right = gear.transform.GetChild(8).gameObject;

        // 서버에서 장비, 인벤토리 정보 호출 후 반영
        GameObject network = GameObject.Find("@NetworkManager");
        networkManager = network.GetComponent<NetworkManager>();
        
        //Debug.Log("networkManager.InventoryInit() 실행");
        networkManager.InventoryInit();
        //networkManager.InventoryOpen();
        //networkManager.GearOpen();

        // 인벤토리 캐릭터 이미지 설정
        switch (networkManager.player[2])
        {
            case "남자 캐릭터":
                transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/player_male") as Sprite;
                break;
            case "여자 캐릭터":
                transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/player_female") as Sprite;
                break;
        }

        transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = "레벨: " + networkManager.player[3];
        transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>().text = "체력: " + networkManager.player[6]+" / " + networkManager.player[7];
        transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(2).GetComponent<Text>().text = "마력: " + networkManager.player[8] + " / " + networkManager.player[9];

        // 인벤토리 정보 최신화 시켜주는 함수 코루틴으로 실행 - 참고: https://solution94.tistory.com/7
        // InvokeRepeating("playerUpdate", 0f, 0.25f);
    }


    void playerUpdate() {

        // 인벤토리 창이 열려있을 경우에만 실행
        if(networkManager.player_gear != null) {

            //UI 체크 후 수정된 정보로 업데이트,  캐릭터 상태창이 열려 있다면 실행
            if (GameObject.Find("CharacterState(Clone)") != null) {
                GameObject playerStat = GameObject.Find("CharacterState(Clone)");

                // Debug.Log($"CONPanel: {playerStat.transform.GetChild(0).GetChild(9).transform.name}," + $" CON_Value: {playerStat.transform.GetChild(0).GetChild(9).GetChild(1).transform.GetComponent<Text>().text} / CON_Plus_Value: {playerStat.transform.GetChild(0).GetChild(9).GetChild(4).transform.GetComponent<Text>().text}");
                // Debug.Log($"WISPanel: {playerStat.transform.GetChild(0).GetChild(10).transform.name}," +  $" WIS_Value: {playerStat.transform.GetChild(0).GetChild(10).GetChild(1).transform.GetComponent<Text>().text} / WIS_Plus_Value: {playerStat.transform.GetChild(0).GetChild(10).GetChild(4).transform.GetComponent<Text>().text}");

                for (int i = 0; i < networkManager.player_gear.Count; i++) {
                    //Debug.Log($"장비 정보 조회 - 키: {i} 값: {networkManager.player_gear[i]}");

                    switch (i) {
                        case 0:// 0: 머리,  힘 - 아이템을 장착할 경우 증가
                            playerStat.transform.GetChild(0).GetChild(7).GetChild(1).transform.GetComponent<Text>().text = networkManager.player[14];
                            Debug.Log($"STR_Value: {playerStat.transform.GetChild(0).GetChild(7).GetChild(1).transform.GetComponent<Text>().text} / STR_Plus_Value: {playerStat.transform.GetChild(0).GetChild(7).GetChild(4).transform.GetComponent<Text>().text}");

                            if (networkManager.player_gear[i].Trim().Equals("head_1") == true) {
                                playerStat.transform.GetChild(0).GetChild(7).GetChild(4).transform.GetComponent<Text>().text = "5";
                            } else if (networkManager.player_gear[i].Trim().Equals("head_2") == true) {
                                playerStat.transform.GetChild(0).GetChild(7).GetChild(4).transform.GetComponent<Text>().text = "10";
                            } else if (networkManager.player_gear[i].Trim().Equals("head_3") == true) {
                                playerStat.transform.GetChild(0).GetChild(7).GetChild(4).transform.GetComponent<Text>().text = "15";
                            } else {
                                playerStat.transform.GetChild(0).GetChild(7).GetChild(4).transform.GetComponent<Text>().text = "0";
                            }
                            break;
                        case 1:// 1. 무기,  공격력 - 아이템을 장착할 경우 증가
                            playerStat.transform.GetChild(0).GetChild(5).GetChild(1).transform.GetComponent<Text>().text = networkManager.player[12];
                            Debug.Log($"Attack_Value: {playerStat.transform.GetChild(0).GetChild(5).GetChild(1).transform.GetComponent<Text>().text} / Attack_Plus_Value: {playerStat.transform.GetChild(0).GetChild(5).GetChild(4).transform.GetComponent<Text>().text}");

                            if (networkManager.player_gear[i].Trim().Equals("weapon_1") == true) {
                                playerStat.transform.GetChild(0).GetChild(5).GetChild(4).transform.GetComponent<Text>().text = "5";
                            } else if (networkManager.player_gear[i].Trim().Equals("weapon_2") == true) {
                                playerStat.transform.GetChild(0).GetChild(5).GetChild(4).transform.GetComponent<Text>().text = "10";
                            } else if (networkManager.player_gear[i].Trim().Equals("weapon_3") == true) {
                                playerStat.transform.GetChild(0).GetChild(5).GetChild(4).transform.GetComponent<Text>().text = "15";
                            } else {
                                playerStat.transform.GetChild(0).GetChild(5).GetChild(4).transform.GetComponent<Text>().text = "0";
                            }
                            break;
                        case 2:// 2: 갑옷,  방어력 - 아이템을 장착할 경우 증가
                            // GameObject playerStat = GameObject.Find("CharacterState(Clone)");
                            Debug.Log($" Defensive_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(1).transform.GetComponent<Text>().text} / Defensive_Plus_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text}");
                            playerStat.transform.GetChild(0).GetChild(6).GetChild(1).transform.GetComponent<Text>().text = networkManager.player[13];

                            if (networkManager.player_gear[i].Trim().Equals("body_1") == true) {
                                playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text = "5";
                            } else if (networkManager.player_gear[i].Trim().Equals("body_2") == true) {
                                playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text = "10";
                            } else if (networkManager.player_gear[i].Trim().Equals("body_3") == true) {
                                playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text = "15";
                            } else {
                                playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text = "0";
                            }
                            break;
                        case 3:// 3: 허리, 생명력 - 아이템을 장착할 경우 증가
                            Debug.Log($"HP: {playerStat.transform.GetChild(0).GetChild(3).GetChild(1).transform.GetComponent<Text>().text} / MaxHP: {playerStat.transform.GetChild(0).GetChild(3).GetChild(3).transform.GetComponent<Text>().text}");

                            playerStat.transform.GetChild(0).GetChild(3).GetChild(1).transform.GetComponent<Text>().text = networkManager.player[6];
                            playerStat.transform.GetChild(0).GetChild(3).GetChild(3).transform.GetComponent<Text>().text = networkManager.player[7];
                            break;
                        case 5:// 5: 신발, 민첩성
                            Debug.Log($" Defensive_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(1).transform.GetComponent<Text>().text} / Defensive_Plus_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text}");
                            playerStat.transform.GetChild(0).GetChild(6).GetChild(1).transform.GetComponent<Text>().text = networkManager.player[15];

                            if (networkManager.player_gear[i].Trim().Equals("shoes_1") == true) {
                                playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text = "5";
                            } else if (networkManager.player_gear[i].Trim().Equals("shoes_2") == true) {
                                playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text = "10";
                            } else if (networkManager.player_gear[i].Trim().Equals("shoes_3") == true) {
                                playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text = "15";
                            } else {
                                playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text = "0";
                            }

                            // 민첩성 - 아이템을 장착할 경우 증가
                            // characterState.transform.GetChild(0).GetChild(8).GetChild(1).GetComponent<Text>().text = network.player[15];
                            // characterState.transform.GetChild(0).GetChild(8).GetChild(4).GetComponent<Text>().text = "0";
                            break;
                        case 4: case 6:// 4: 왼쪽 반지, 6: 오른쪽 반지, 마력 - 아이템을 장착할 경우 증가
                            Debug.Log($" MP: {playerStat.transform.GetChild(0).GetChild(4).GetChild(1).transform.GetComponent<Text>().text} / MaxMP: {playerStat.transform.GetChild(0).GetChild(4).GetChild(3).transform.GetComponent<Text>().text}");

                            playerStat.transform.GetChild(0).GetChild(4).GetChild(1).transform.GetComponent<Text>().text = networkManager.player[8];
                            playerStat.transform.GetChild(0).GetChild(4).GetChild(3).transform.GetComponent<Text>().text = networkManager.player[9];
                            break;
                        default:
                            break;
                    }
                }

            }

            /**
            캐릭터 장비 조회 후 능력치 수정
            0: 머리
            1: 무기
            2: 갑옷
            3: 허리
            4: 왼쪽 반지
            5: 신발
            6: 오른쪽 반지
          */

            /**
            // 캐릭터 능력치 업데이트(클라 수정 -> 서버 수정)
            for (int i = 0; i < networkManager.player_gear.Count; i++)
            {
                switch (networkManager.player_gear[i])
                {
                    case "body_1":
                        Debug.Log($"장비 정보[body_1] - 키: {i} 값: {networkManager.player_gear[i]}");

                        // 장비에 따라 능력치 수정
                        int player_str = Int32.Parse(networkManager.player[13]);
                        int gear_str = 5;
                        int result_str = player_str + gear_str;
                        Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                        // int -> string 변환 후 플레이어 정보 저장
                        networkManager.player[13] = result_str.ToString();

                        Debug.Log($"플레이어 스텟 정보[Defense] 값: {networkManager.player[13]}");
                        break;
                    case "body_2":
                        Debug.Log($"장비 정보[body_2] - 키: {i} 값: {networkManager.player_gear[i]}");

                        // 장비에 따라 능력치 수정
                        player_str = Int32.Parse(networkManager.player[13]);
                        gear_str = 10;
                        result_str = player_str + gear_str;
                        Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                        // int -> string 변환 후 플레이어 정보 저장
                        networkManager.player[13] = result_str.ToString();

                        Debug.Log($"플레이어 스텟 정보[Defense] 값: {networkManager.player[13]}");
                        break;
                    case "body_3":
                        Debug.Log($"장비 정보[body_3] - 키: {i} 값: {networkManager.player_gear[i]}");

                        // 장비에 따라 능력치 수정
                        player_str = Int32.Parse(networkManager.player[13]);
                        gear_str = 15;
                        result_str = player_str + gear_str;
                        Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                        // int -> string 변환 후 플레이어 정보 저장
                        networkManager.player[13] = result_str.ToString();

                        Debug.Log($"플레이어 스텟 정보[Defense] 값: {networkManager.player[13]}");
                        break;
                    case "head_1":
                        Debug.Log($"장비 정보[head_1] - 키: {i} 값: {networkManager.player_gear[i]}");

                        // 장비에 따라 능력치 수정
                        player_str = Int32.Parse(networkManager.player[14]);
                        gear_str = 5;
                        result_str = player_str + gear_str;
                        Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                        // int -> string 변환 후 플레이어 정보 저장
                        networkManager.player[14] = result_str.ToString();

                        Debug.Log($"플레이어 스텟 정보[STR] 값: {networkManager.player[14]}");
                        break;
                    case "head_2":
                        Debug.Log($"장비 정보[head_2] - 키: {i} 값: {networkManager.player_gear[i]}");

                        // 장비에 따라 능력치 수정
                        player_str = Int32.Parse(networkManager.player[14]);
                        gear_str = 10;
                        result_str = player_str + gear_str;
                        Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                        // int -> string 변환 후 플레이어 정보 저장
                        networkManager.player[14] = result_str.ToString();

                        Debug.Log($"플레이어 스텟 정보[STR] 값: {networkManager.player[14]}");
                        break;
                    case "head_3":
                        Debug.Log($"장비 정보[head_3] - 키: {i} 값: {networkManager.player_gear[i]}");

                        // 장비에 따라 능력치 수정
                        player_str = Int32.Parse(networkManager.player[14]);
                        gear_str = 15;
                        result_str = player_str + gear_str;
                        Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                        // int -> string 변환 후 플레이어 정보 저장
                        networkManager.player[14] = result_str.ToString();

                        Debug.Log($"플레이어 스텟 정보[STR] 값: {networkManager.player[14]}");
                        break;
                    case "ring_1":
                        Debug.Log($"장비 정보[ring_1] - 키: {i} 값: {networkManager.player_gear[i]}");

                        // 장비에 따라 능력치 수정
                        player_str = Int32.Parse(networkManager.player[9]); // MaxMp 값
                        gear_str = 50;
                        result_str = player_str + gear_str;
                        Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                        // int -> string 변환 후 플레이어 정보 저장
                        networkManager.player[8] = result_str.ToString();
                        networkManager.player[9] = result_str.ToString();

                        Debug.Log($"플레이어 스텟 정보[마력] 값: {networkManager.player[8]}/{networkManager.player[9]}");
                        break;
                    case "ring_2":
                        Debug.Log($"장비 정보[ring_2] - 키: {i} 값: {networkManager.player_gear[i]}");

                        // 장비에 따라 능력치 수정
                        player_str = Int32.Parse(networkManager.player[9]); // MaxMp 값
                        gear_str = 100;
                        result_str = player_str + gear_str;
                        Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                        // int -> string 변환 후 플레이어 정보 저장
                        networkManager.player[8] = result_str.ToString();
                        networkManager.player[9] = result_str.ToString();

                        Debug.Log($"플레이어 스텟 정보[마력] 값: {networkManager.player[8]}/{networkManager.player[9]}");
                        break;
                    case "ring_3":
                        Debug.Log($"장비 정보[ring_3] - 키: {i} 값: {networkManager.player_gear[i]}");

                        // 장비에 따라 능력치 수정
                        player_str = Int32.Parse(networkManager.player[9]); // MaxMp 값
                        gear_str = 150;
                        result_str = player_str + gear_str;
                        Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                        // int -> string 변환 후 플레이어 정보 저장
                        networkManager.player[8] = result_str.ToString();
                        networkManager.player[9] = result_str.ToString();

                        Debug.Log($"플레이어 스텟 정보[마력] 값: {networkManager.player[8]}/{networkManager.player[9]}");
                        break;
                    case "shoes_1":
                        Debug.Log($"장비 정보[shoes_1] - 키: {i} 값: {networkManager.player_gear[i]}");

                        // 장비에 따라 능력치 수정
                        player_str = Int32.Parse(networkManager.player[15]);
                        gear_str = 5;
                        result_str = player_str + gear_str;
                        Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                        // int -> string 변환 후 플레이어 정보 저장
                        networkManager.player[15] = result_str.ToString();

                        Debug.Log($"플레이어 스텟 정보[DEX] 값: {networkManager.player[15]}");
                        break;
                    case "shoes_2":
                        Debug.Log($"장비 정보[shoes_2] - 키: {i} 값: {networkManager.player_gear[i]}");

                        // 장비에 따라 능력치 수정
                        player_str = Int32.Parse(networkManager.player[15]);
                        gear_str = 10;
                        result_str = player_str + gear_str;
                        Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                        // int -> string 변환 후 플레이어 정보 저장
                        networkManager.player[15] = result_str.ToString();

                        Debug.Log($"플레이어 스텟 정보[DEX] 값: {networkManager.player[15]}");
                        break;
                    case "shoes_3":
                        Debug.Log($"장비 정보[shoes_3] - 키: {i} 값: {networkManager.player_gear[i]}");

                        // 장비에 따라 능력치 수정
                        player_str = Int32.Parse(networkManager.player[15]);
                        gear_str = 15;
                        result_str = player_str + gear_str;
                        Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                        // int -> string 변환 후 플레이어 정보 저장
                        networkManager.player[15] = result_str.ToString();

                        Debug.Log($"플레이어 스텟 정보[DEX] 값: {networkManager.player[15]}");
                        break;
                    default:
                        //  Debug.Log($"장비 정보[기타] - 키: {i} 값: {networkManager.player_gear[i]}");
                        break;
                }
            }
              */
        }



        /**
         
        // 아이템 정보 전송 - 캐릭터 능력치 업데이트(클라 수정 -> 서버 수정)
        switch (isSlotRect.transform.GetChild(0).GetComponent<InvenItem>().ItemName)
        {
            case "body_1":
                Debug.Log($"장비 정보[body_1] - 이동한 아이템 정보 정보: {isSlotRect.transform.GetChild(0).GetComponent<InvenItem>().ItemName}");

                // 아이템에 따라 능력치 다르게 증가
                int player_str = Int32.Parse(networkManager.player[13]);
                int gear_str = 5;
                int result_str = player_str - gear_str;

                Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                // int -> string 변환 후 플레이어 정보 저장
                networkManager.player[13] = result_str.ToString();

                Debug.Log($"플레이어 스텟 정보[Defense] 값: {networkManager.player[13]}");
                break;
            case "body_2":
                Debug.Log($"장비 정보[body_2] - 이동한 아이템 정보 정보: {isSlotRect.transform.GetChild(0).GetComponent<InvenItem>().ItemName}");

                // 아이템에 따라 능력치 다르게 증가
                player_str = Int32.Parse(networkManager.player[13]);
                gear_str = 10;
                result_str = player_str - gear_str;

                Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                // int -> string 변환 후 플레이어 정보 저장
                networkManager.player[13] = result_str.ToString();

                Debug.Log($"플레이어 스텟 정보[Defense] 값: {networkManager.player[13]}");
                break;
            case "body_3":
                Debug.Log($"장비 정보[body_3] - 이동한 아이템 정보 정보: {isSlotRect.transform.GetChild(0).GetComponent<InvenItem>().ItemName}");

                // 아이템에 따라 능력치 다르게 증가
                player_str = Int32.Parse(networkManager.player[13]);
                gear_str = 15;
                result_str = player_str - gear_str;

                Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                // int -> string 변환 후 플레이어 정보 저장
                networkManager.player[13] = result_str.ToString();

                Debug.Log($"플레이어 스텟 정보[Defense] 값: {networkManager.player[13]}");
                break;
            case "head_1":
                Debug.Log($"장비 정보[head_1] - 이동한 아이템 정보 정보: {isSlotRect.transform.GetChild(0).GetComponent<InvenItem>().ItemName}");
                Debug.Log($"플레이어 스텟 정보[STR] 값: {networkManager.player[14]}");
                break;
            case "head_2":
                Debug.Log($"장비 정보[head_2] - 이동한 아이템 정보 정보: {isSlotRect.transform.GetChild(0).GetComponent<InvenItem>().ItemName}");
                Debug.Log($"플레이어 스텟 정보[STR] 값: {networkManager.player[14]}");
                break;
            case "head_3":
                Debug.Log($"장비 정보[head_3] - 이동한 아이템 정보 정보: {isSlotRect.transform.GetChild(0).GetComponent<InvenItem>().ItemName}");
                Debug.Log($"플레이어 스텟 정보[STR] 값: {networkManager.player[14]}");
                break;
            case "ring_1":
                Debug.Log($"장비 정보[ring_1] - 이동한 아이템 정보 정보: {isSlotRect.transform.GetChild(0).GetComponent<InvenItem>().ItemName}");
                Debug.Log($"플레이어 스텟 정보[마력] 값: {networkManager.player[8]}/{networkManager.player[9]}");
                break;
            case "ring_2":
                Debug.Log($"장비 정보[ring_2] - 이동한 아이템 정보 정보: {isSlotRect.transform.GetChild(0).GetComponent<InvenItem>().ItemName}");
                Debug.Log($"플레이어 스텟 정보[마력] 값: {networkManager.player[8]}/{networkManager.player[9]}");
                break;
            case "ring_3":
                Debug.Log($"장비 정보[ring_3] - 이동한 아이템 정보 정보: {isSlotRect.transform.GetChild(0).GetComponent<InvenItem>().ItemName}");
                Debug.Log($"플레이어 스텟 정보[마력] 값: {networkManager.player[8]}/{networkManager.player[9]}");
                break;
            case "shoes_1":
                Debug.Log($"장비 정보[shoes_1] - 이동한 아이템 정보 정보: {isSlotRect.transform.GetChild(0).GetComponent<InvenItem>().ItemName}");
                Debug.Log($"플레이어 스텟 정보[DEX] 값: {networkManager.player[15]}");
                break;
            case "shoes_2":
                Debug.Log($"장비 정보[shoes_2] - 이동한 아이템 정보 정보: {isSlotRect.transform.GetChild(0).GetComponent<InvenItem>().ItemName}");
                Debug.Log($"플레이어 스텟 정보[DEX] 값: {networkManager.player[15]}");
                break;
            case "shoes_3":
                Debug.Log($"장비 정보[shoes_3] - 이동한 아이템 정보 정보: {isSlotRect.transform.GetChild(0).GetComponent<InvenItem>().ItemName}");
                Debug.Log($"플레이어 스텟 정보[DEX] 값: {networkManager.player[15]}");
                break;
            default:
                //  Debug.Log($"장비 정보[기타] - 키: {i} 값: {networkManager.player_gear[i]}");
                break;
        }

      
         */
    }

    public static void recvGear(List<string> recv)
    {
        gearSlot = new Dictionary<int, GameObject>();
        gearSlotRect = new Dictionary<RectTransform, int>();
        gearSlotRect.Add(head.transform as RectTransform, 0);
        gearSlotRect.Add(weapon.transform as RectTransform, 1);
        gearSlotRect.Add(body.transform as RectTransform, 2);
        gearSlotRect.Add(waist.transform as RectTransform, 3);
        gearSlotRect.Add(ring_left.transform as RectTransform, 4);
        gearSlotRect.Add(shoes.transform as RectTransform, 5);
        gearSlotRect.Add(ring_right.transform as RectTransform, 6);

        //Debug.Log($"recvGear - recv.Count: {recv.Count}");

        // 인벤토리에 슬롯 추가
        for (int i = 0; i < recv.Count(); i++)
        {
            switch (recv[i])
            {
                case "없음":
                    gearSlot.Add(i, null);
                    break;
                default:
                    GameObject Info = Managers.Resource.Instantiate("UI/SubItem/Info");
                    Info.name = "Info";
                    InvenItem item = Info.AddComponent<InvenItem>();
                    item.ItemName = recv[i];
                    Info.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/Item/{recv[i]}") as Sprite;

                    switch (i)
                    {
                        case 0:
                            Info.transform.SetParent(head.transform);
                            break;
                        case 1:
                            Info.transform.SetParent(weapon.transform);
                            break;
                        case 2:
                            Info.transform.SetParent(body.transform);
                            break;
                        case 3:
                            Info.transform.SetParent(waist.transform);
                            break;
                        case 4:
                            Info.transform.SetParent(ring_left.transform);
                            break;
                        case 5:
                            Info.transform.SetParent(shoes.transform);
                            break;
                        case 6:
                            Info.transform.SetParent(ring_right.transform);
                            break;
                        default:
                            break;
                    }

                    Info.AddComponent<InvenItem>().ItemName = recv[i];
                    RectTransform rt = (RectTransform)Info.transform;
                    rt.anchoredPosition = new Vector3(0f, 0f);
                    gearSlot.Add(i, Info);
                    break;
            }
        }
    }

    public static void recvInven(List<string> recv)
    {

        // Debug.Log($"recvInven - recv.Count: {recv.Count}");

        invenSlot = new Dictionary<int, GameObject>();
        invenSlotRect = new Dictionary<RectTransform, int>();


        // 인벤토리에 슬롯 추가
        for (int i = 0; i < recv.Count(); i++)
        {
            //Debug.Log($"recvInven - inven {i}번째 값:{recv[i]}");
            // 데이터베이스에 저장된 아이템 정보에 따라 값을 다르게 슬롯안 아이템 지정
            switch (recv[i])
            {
                case "없음":
                    GameObject item = Managers.Resource.Instantiate("UI/SubItem/Slot_Empty");
                    item.transform.SetParent(Inven.transform);// 너의 부모는 누구다 라고 지정하는 함수
                    item.name = $"slot_{i}"; // 오브젝트 이름 변경
                    invenSlot.Add(i, null);
                    invenSlotRect.Add(item.transform as RectTransform, i);
                    break;
                default:
                    item = Managers.Resource.Instantiate("UI/SubItem/Slot");
                    item.transform.SetParent(Inven.transform);// 너의 부모는 누구다 라고 지정하는 함수
                    item.name = $"slot_{i}"; // 오브젝트 이름 변경, Debug.Log("recv[i]: "+ recv[i]);

                    GameObject Info = item.transform.Find("Info").gameObject;
                    Info.transform.SetParent(item.transform);
                    Info.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/Item/{recv[i]}") as Sprite;
                    Info.AddComponent<InvenItem>().ItemName = recv[i];
                    invenSlot.Add(i, Info);
                    invenSlotRect.Add(item.transform as RectTransform, i);
                    break;
            }
        }
    }

    public void Cancel()
    {
        Debug.Log("Inventory - Cancel");
        Managers.Resource.Destroy(transform.gameObject);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 아이템 이미지를 선택했을 경우에만 드래그 가능하게 조건 설정
        if (eventData.pointerCurrentRaycast.gameObject.name.Equals("Info") == true)
        {
            //선택한 아이템 정보 저장
            selectItem = eventData.pointerCurrentRaycast.gameObject;
            beforeSelectItem = eventData.pointerCurrentRaycast.gameObject;
            beforeSelectItemParent = eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject;

            //선택한 아이템 부모 변경
            selectItem.transform.SetParent(Inven.transform.parent.parent.transform);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 선택한 아이템이 없다면 무시하게 설정
        if (selectItem != null)
        {
            // 선택한 아이템 위치 이동
            // eventData.position: 드래그 하고 있는 마우스위치
            selectItem.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (selectItem != null) {
            // 스크린상의 특정 포인트가 지정된 사각 범위 꼭지점 안에 포함되는지 결과를 리턴(인벤토리 밖인지 안인지 체크)
            isInvan = RectTransformUtility.RectangleContainsScreenPoint(Inven.transform.parent as RectTransform, eventData.position);

            // 인벤토리 밖일 경우 true, 안일 경우 false
            if (isInvan == false) {
                // 경고창 출력 true, false 처리
                Debug.Log($"인벤토리 밖");

                // 드래그한 아이템 위치 원래대로 이동
                selectItem.transform.SetParent(beforeSelectItemParent.transform);
                selectItem.transform.SetSiblingIndex(0);
                RectTransform rt = (RectTransform)beforeSelectItemParent.transform.GetChild(0).transform;
                rt.anchoredPosition = new Vector3(0f, 0f);

                // 인벤 슬롯 정보 조회 후 제거
                foreach (KeyValuePair<int, GameObject> invenSlotResult in invenSlot)
                {
                    if (invenSlotResult.Value == selectItem)
                    {
                        //Debug.Log($"인벤 슬롯 정보 조회 - 키: {invenSlotResult.Key} 값: {invenSlotResult.Value}");

                        // 아이템을 놓는 곳이 상점창 안이냐 밖이냐에 따라서 분기 결정
                        // 마우스 위치 판별 어떻게?
                        // RectangleContainsScreenPoint: 스크린상의 특정 포인트가 지정된 사각 범위 꼭지점 안에 포함되는지 결과를 리턴한다.
                        // true면 포인트가 그 영역내이고, false
                        // 밖에다 버릴 경우:
                        // 상점에 팔 경우: 
                        GameObject shop = GameObject.Find("ShopUI(Clone)");

                        // 상점창이 생성 안되어 있으면 null 발생
                        if (shop == null)
                        {
                            Debug.Log($"상점창이 생성되어 있지 않을 경우");
                        } else {
                            bool isShop = RectTransformUtility.RectangleContainsScreenPoint(shop.transform as RectTransform, eventData.position);
                            int item_money = 0;
                            Debug.Log($"isShop: {isShop}");

                            // 상점창이 생성 되어 있으면 true 발생
                            if (isShop == true)
                            {
                                Debug.Log($"인벤 슬롯 정보 조회 - 인벤토리 슬롯 정보: {invenSlotResult.Key} 값: {invenSlotResult.Value.GetComponent<InvenItem>().ItemName}");

                                // 아이템을 판매할 경우
                                switch (invenSlotResult.Value.GetComponent<InvenItem>().ItemName) {
                                    case "body_1": case "head_1": case "ring_1": case "shoes_1":
                                        // 1000 원
                                        item_money = 500;
                                        break;
                                    case "body_2": case "ring_2": case "head_2": case "shoes_2":
                                        // 2000 원
                                        item_money = 1000;
                                        break;
                                    case "body_3": case "head_3": case "ring_3": case "shoes_3":
                                        // 3000 원
                                        item_money = 1500;
                                        break;
                                    default:
                                        Debug.Log($"아이템을 판매할 경우 - 예외 상황");
                                        break;
                                }

                                // 아이템 가격의 반만큼 소지금 증가
                                int player_money = int.Parse(networkManager.player[5]);
                               
                                // Debug.Log($"계산 전 - network.player[5]: {networkManager.player[5]},  price: {item_money}");
                                networkManager.player[5] = (player_money + item_money).ToString();
                                // Debug.Log($"계산 후 - network.player[5]: {networkManager.player[5]}");

                                // 인벤토리에서 아이템 제거 [UI]
                                Managers.Resource.Destroy(invenSlotResult.Value);
                                invenSlot[invenSlotResult.Key] = null;

                                // 인벤토리에서 아이템 제거 [Data]
                                networkManager.inven[invenSlotResult.Key] = "없음"; 

                                // 아이템 판매 결과 서버로 전송 - 매개 변수: 인벤토리 슬롯 번호, 아이템 명, 아이템 판매 후 계산된 플레이어 돈
                                networkManager.ShopSellUpdate(invenSlotResult.Key, "없음", int.Parse(networkManager.player[5]));
                            }

                        }


                        break;
                    }
                }
            } else {
                bool isGear = RectTransformUtility.RectangleContainsScreenPoint(gear.transform as RectTransform, eventData.position);
                bool isInven = RectTransformUtility.RectangleContainsScreenPoint(Inven.transform as RectTransform, eventData.position);

                // 범위 값 조회 검색 결과 저장 목적
                RectTransform gearResult = null;
                int gearIndex = 0;

                if (isGear == true) {
                    // Debug.Log("장비 슬롯 안일 경우");

                    foreach (KeyValuePair<RectTransform, int> gearValue in gearSlotRect) {
                        bool isGearSlot = RectTransformUtility.RectangleContainsScreenPoint(gearValue.Key, eventData.position);
                        if (isGearSlot == true) {
                            //Debug.Log("마우스커서 위치가 어떤 장비 슬롯 위에 있는지 조회");
                            //Debug.Log($"isGearSlot: 키: {gearValue.Key} 값: {gearValue.Value}");
                            gearIndex = gearValue.Value;
                            gearResult = gearValue.Key;
                        }
                    }

                    if (gearResult != null) {
                        int invenValue = 0;
                        GameObject gearInfo = null;
                        if (gearSlot.TryGetValue(gearIndex, out gearInfo) == true) {
                            //장비칸에 아무것도 장착하지 않을 경우 null
                            //if (gearInfo == null) {
                            //    Debug.Log($"목적지 장비 슬롯 정보 - 값: null");
                            //} else {
                            //    Debug.Log($"목적지 장비 슬롯 정보 - 값: {gearInfo.name}");
                            //}

                            // 인벤 슬롯 정보 제거하고 장비 슬롯 정보로 이동
                            //Debug.Log($"선택한 아이템 슬롯 값: {beforeSelectItemParent.transform.name}");
                            //Debug.Log($"목적지 장비 슬롯 정보: 키: {gearIndex} 값: {gearResult}");

                            // 인벤 슬롯 정보 조회
                            foreach (KeyValuePair<int, GameObject> invenSlotResult in invenSlot) {
                                if (invenSlotResult.Value != null && invenSlotResult.Value.transform.parent.name.Equals("CharacterInfo(Clone)") == true) {
                                    // Debug.Log($"인벤 슬롯 정보 일치 - 키: {invenSlotResult.Key} 값: {invenSlotResult.Value.transform.parent.name}");
                                    invenValue = invenSlotResult.Key;
                                }
                            }

                            selectItem.transform.SetParent(gearResult.transform);
                            selectItem.transform.SetSiblingIndex(0);
                            RectTransform rt = (RectTransform)gearResult.GetChild(0).transform;
                            rt.anchoredPosition = new Vector3(0f, 0f);

                            //인벤 슬롯에 있는 아이템을 장비 슬롯 아이템위치로 이동, 슬롯 장비에는 null 값 추가
                            if (gearSlot[gearIndex] == null) {
                                // Debug.Log("장비칸에 장비가 없을 경우");
                                // Debug.Log("gearSlot[gearIndex]: " + gearSlot[gearIndex]);
                                gearSlot[gearIndex] = selectItem;
                                invenSlot[invenValue] = null;

                                // 장비 슬롯 정보 조회
                                // Debug.Log("장비 슬롯 아이템 장작 후 [UI]");
                                foreach (KeyValuePair<int, GameObject> gearSlotResult in gearSlot) {                  
                                    if (gearSlotResult.Value != null) {
                                        if(gearSlotResult.Value.name.Equals("없음") != true) {
                                            InvenItem itemName = gearSlotResult.Value.GetComponent<InvenItem>();
                                            // Debug.Log($"장비 슬롯 정보 조회 - 키: {gearSlotResult.Key} 값: {itemName.ItemName}");

                                            networkManager.player_gear[gearSlotResult.Key] = itemName.ItemName;
                                        } else  {
                                            networkManager.player_gear[gearSlotResult.Key] = gearSlotResult.Value.name;
                                        }
                                    } else {
                                        networkManager.player_gear[gearSlotResult.Key] = "없음";
                                    }
                                }

                                // 인벤토리, 장비창 정보 수정
                                Debug.Log($"선택한 아이템 정보: {selectItem.GetComponent<InvenItem>().ItemName}");
                                Debug.Log("장비 슬롯 아이템 장작 후 [Data]");

                                // 캐릭터 능력치 업데이트(클라 수정 -> 서버 수정)
                                for (int i = 0; i < networkManager.player_gear.Count; i++) {
                                    Debug.Log($"장비 정보 - 키: {i} 값: {networkManager.player_gear[i]}");
                                    switch (i) {
                                        case 0:// 머리, 선택한 아이템 비교 후 능력치 수정
                                            switch (networkManager.player_gear[i]) {
                                                case "head_1":
                                                    // 이동한 장비가 맞다면 능력치 증가
                                                    if(selectItem.GetComponent<InvenItem>().ItemName.Equals(networkManager.player_gear[i]) == true) {
                                                        // 장비에 따라 능력치 수정
                                                        int player_str = Int32.Parse(networkManager.player[14]);
                                                        int gear_str = 5;
                                                        int result_str = player_str + gear_str;
                                                        Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                                                        // int -> string 변환 후 플레이어 정보 저장
                                                        networkManager.player[14] = result_str.ToString();

                                                        Debug.Log($"플레이어 스텟 정보[STR] 값: {networkManager.player[14]}");
                                                    }
                                                    break;
                                                case "head_2":
                                                    // 이동한 장비가 맞다면 능력치 증가
                                                    if (selectItem.GetComponent<InvenItem>().ItemName.Equals(networkManager.player_gear[i]) == true) {
                                                        // 장비에 따라 능력치 수정
                                                        int player_str = Int32.Parse(networkManager.player[14]);
                                                        int gear_str = 10;
                                                        int result_str = player_str + gear_str;
                                                        Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                                                        // int -> string 변환 후 플레이어 정보 저장
                                                        networkManager.player[14] = result_str.ToString();

                                                        Debug.Log($"플레이어 스텟 정보[STR] 값: {networkManager.player[14]}");
                                                    }
                                                    break;
                                                case "head_3":
                                                    // 이동한 장비가 맞다면 능력치 증가
                                                    if (selectItem.GetComponent<InvenItem>().ItemName.Equals(networkManager.player_gear[i]) == true) {
                                                        // 장비에 따라 능력치 수정
                                                        int player_str = Int32.Parse(networkManager.player[14]);
                                                        int gear_str = 15;
                                                        int result_str = player_str + gear_str;
                                                        Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                                                        // int -> string 변환 후 플레이어 정보 저장
                                                        networkManager.player[14] = result_str.ToString();

                                                        Debug.Log($"플레이어 스텟 정보[STR] 값: {networkManager.player[14]}");
                                                    }
                                                    break;
                                                default:
                                                      Debug.Log($"head 장비 정보[기타] - 키: {i} 값: {networkManager.player_gear[i]}");
                                                    break;
                                            }
                                            break;
                                        case 1:// 무기
                                            Debug.Log($"case 1 장비 정보[기타] - 키: {i} 값: {networkManager.player_gear[i]}");
                                            break;
                                        case 2:// 갑옷
                                            switch (networkManager.player_gear[i]) {
                                                case "body_1":
                                                    // 이동한 장비가 맞다면 장비에 따라 능력치 수정
                                                    if (selectItem.GetComponent<InvenItem>().ItemName.Equals(networkManager.player_gear[i]) == true) {
                                                        
                                                        int player_str = Int32.Parse(networkManager.player[13]);
                                                        int gear_str = 5;
                                                        int result_str = player_str + gear_str;
                                                        // Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                                                        // int -> string 변환 후 플레이어 정보 저장
                                                        networkManager.player[13] = result_str.ToString();

                                                        Debug.Log($"캐릭터 상태창 UI정보 수정 - 플레이어 스텟 정보[Defense] 값: {networkManager.player[13]}");
                                                        GameObject playerStat = GameObject.Find("CharacterState(Clone)");
                                                        if (playerStat != null)
                                                        {
                                                            // Debug.Log($" Defensive_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(1).transform.GetComponent<Text>().text} / Defensive_Plus_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text}");
                                                            playerStat.transform.GetChild(0).GetChild(6).GetChild(1).transform.GetComponent<Text>().text = networkManager.player[13];
                                                            playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text = "5";
                                                        }
                                                    }
                                                    break;
                                                case "body_2":
                                                    // 이동한 장비가 맞다면 장비에 따라 능력치 수정
                                                    if (selectItem.GetComponent<InvenItem>().ItemName.Equals(networkManager.player_gear[i]) == true) {

                                                        int player_str = Int32.Parse(networkManager.player[13]);
                                                        int gear_str = 10;
                                                        int result_str = player_str + gear_str;
                                                        // Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                                                        // int -> string 변환 후 플레이어 정보 저장
                                                        networkManager.player[13] = result_str.ToString();

                                                        Debug.Log($"캐릭터 상태창 UI정보 수정 - 플레이어 스텟 정보[Defense] 값: {networkManager.player[13]}");
                                                        GameObject playerStat = GameObject.Find("CharacterState(Clone)");
                                                        if (playerStat != null)
                                                        {
                                                            // Debug.Log($" Defensive_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(1).transform.GetComponent<Text>().text} / Defensive_Plus_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text}");
                                                            playerStat.transform.GetChild(0).GetChild(6).GetChild(1).transform.GetComponent<Text>().text = networkManager.player[13];
                                                            playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text = "5";
                                                        }
                                                    }
                                                    break;
                                                case "body_3":
                                                    // 이동한 장비가 맞다면 장비에 따라 능력치 수정
                                                    if (selectItem.GetComponent<InvenItem>().ItemName.Equals(networkManager.player_gear[i]) == true) {

                                                        int player_str = Int32.Parse(networkManager.player[13]);
                                                        int gear_str = 15;
                                                        int result_str = player_str + gear_str;
                                                        // Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                                                        // int -> string 변환 후 플레이어 정보 저장
                                                        networkManager.player[13] = result_str.ToString();

                                                        Debug.Log($"캐릭터 상태창 UI정보 수정 - 플레이어 스텟 정보[Defense] 값: {networkManager.player[13]}");
                                                        GameObject playerStat = GameObject.Find("CharacterState(Clone)");
                                                        if (playerStat != null)
                                                        {
                                                            // Debug.Log($" Defensive_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(1).transform.GetComponent<Text>().text} / Defensive_Plus_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text}");
                                                            playerStat.transform.GetChild(0).GetChild(6).GetChild(1).transform.GetComponent<Text>().text = networkManager.player[13];
                                                            playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text = "5";
                                                        }
                                                    }
                                                    break;
                                                default:
                                                    //  Debug.Log($"장비 정보[기타] - 키: {i} 값: {networkManager.player_gear[i]}");
                                                    break;
                                            }
                                            break;
                                        case 3:// 허리
                                            Debug.Log($"case 3 - 장비 정보[기타] - 키: {i} 값: {networkManager.player_gear[i]}");
                                            break;
                                        case 4:// 왼쪽 링
                                            Debug.Log($"case 4 - 장비 정보 - 키: {i} 값: {networkManager.player_gear[i]}. networkManager.player[9]: {networkManager.player[9]}");

                                            // 장비에 따라 능력치 수정
                                            switch (networkManager.player_gear[i]) {
                                                case "ring_1":
                                                    // 이동한 장비가 맞다면 장비에 따라 능력치 수정
                                                    if (selectItem.GetComponent<InvenItem>().ItemName.Equals(networkManager.player_gear[i]) == true) {
                                                        int player_str = Int32.Parse(networkManager.player[9]); // MaxMp 값
                                                        int gear_str = 50;
                                                        //int result_str = player_str + gear_str + 50;
                                                        int result_str = player_str + gear_str;
                                                        // Debug.Log($"networkManager.player_gear[6] - ring_1 - player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                                                        // int -> string 변환 후 플레이어 정보 저장
                                                        networkManager.player[8] = result_str.ToString();
                                                        networkManager.player[9] = result_str.ToString();

                                                        Debug.Log($"캐릭터 상태창 UI정보 수정 - 플레이어 스텟 정보[마력] 값: {networkManager.player[8]}/{networkManager.player[9]}");
                                                        GameObject playerStat = GameObject.Find("CharacterState(Clone)");
                                                        if (playerStat != null)
                                                        {
                                                            // Debug.Log($" Defensive_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(1).transform.GetComponent<Text>().text} / Defensive_Plus_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text}");
                                                            playerStat.transform.GetChild(0).GetChild(4).GetChild(1).transform.GetComponent<Text>().text = networkManager.player[8];
                                                            playerStat.transform.GetChild(0).GetChild(4).GetChild(3).transform.GetComponent<Text>().text = networkManager.player[9];
                                                        }

                                                        // 인벤토리 내 마력 정보 수정
                                                        character.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Text>().text = "마력: " + networkManager.player[8] + " / " + networkManager.player[9];
                                                    }

                                                    break;
                                                case "ring_2":
                                                    // 이동한 장비가 맞다면 장비에 따라 능력치 수정
                                                    if (selectItem.GetComponent<InvenItem>().ItemName.Equals(networkManager.player_gear[i]) == true)
                                                    {
                                                        int player_str = Int32.Parse(networkManager.player[9]); // MaxMp 값
                                                        int gear_str = 100;
                                                        //int result_str = player_str + gear_str + 50;
                                                        int result_str = player_str + gear_str;
                                                        // Debug.Log($"networkManager.player_gear[6] - ring_1 - player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                                                        // int -> string 변환 후 플레이어 정보 저장
                                                        networkManager.player[8] = result_str.ToString();
                                                        networkManager.player[9] = result_str.ToString();

                                                        Debug.Log($"캐릭터 상태창 UI정보 수정 - 플레이어 스텟 정보[마력] 값: {networkManager.player[8]}/{networkManager.player[9]}");
                                                        GameObject playerStat = GameObject.Find("CharacterState(Clone)");
                                                        if (playerStat != null)
                                                        {
                                                            // Debug.Log($" Defensive_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(1).transform.GetComponent<Text>().text} / Defensive_Plus_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text}");
                                                            playerStat.transform.GetChild(0).GetChild(4).GetChild(1).transform.GetComponent<Text>().text = networkManager.player[8];
                                                            playerStat.transform.GetChild(0).GetChild(4).GetChild(3).transform.GetComponent<Text>().text = networkManager.player[9];
                                                        }

                                                        // 인벤토리 내 마력 정보 수정
                                                        character.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Text>().text = "마력: " + networkManager.player[8] + " / " + networkManager.player[9];
                                                    }
                                                    break;
                                                case "ring_3":
                                                    // 이동한 장비가 맞다면 장비에 따라 능력치 수정
                                                    if (selectItem.GetComponent<InvenItem>().ItemName.Equals(networkManager.player_gear[i]) == true)
                                                    {
                                                        int player_str = Int32.Parse(networkManager.player[9]); // MaxMp 값
                                                        int gear_str = 150;
                                                        //int result_str = player_str + gear_str + 50;
                                                        int result_str = player_str + gear_str;
                                                        // Debug.Log($"networkManager.player_gear[6] - ring_1 - player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                                                        // int -> string 변환 후 플레이어 정보 저장
                                                        networkManager.player[8] = result_str.ToString();
                                                        networkManager.player[9] = result_str.ToString();

                                                        Debug.Log($"캐릭터 상태창 UI정보 수정 - 플레이어 스텟 정보[마력] 값: {networkManager.player[8]}/{networkManager.player[9]}");
                                                        GameObject playerStat = GameObject.Find("CharacterState(Clone)");
                                                        if (playerStat != null)
                                                        {
                                                            // Debug.Log($" Defensive_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(1).transform.GetComponent<Text>().text} / Defensive_Plus_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text}");
                                                            playerStat.transform.GetChild(0).GetChild(4).GetChild(1).transform.GetComponent<Text>().text = networkManager.player[8];
                                                            playerStat.transform.GetChild(0).GetChild(4).GetChild(3).transform.GetComponent<Text>().text = networkManager.player[9];
                                                        }

                                                        // 인벤토리 내 마력 정보 수정
                                                        character.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Text>().text = "마력: " + networkManager.player[8] + " / " + networkManager.player[9];
                                                    }
                                                    break;
                                                default:
                                                    //  Debug.Log($"장비 정보[기타] - 키: {i} 값: {networkManager.player_gear[i]}");
                                                    break;
                                            }
                                            break;
                                        case 5:// 신발
                                            switch (networkManager.player_gear[i]) {
                                                case "shoes_1":
                                                    Debug.Log($"장비 정보[shoes_1] - 키: {i} 값: {networkManager.player_gear[i]}");

                                                    // 장비에 따라 능력치 수정
                                                    int player_str = Int32.Parse(networkManager.player[15]);
                                                    int gear_str = 5;
                                                    int result_str = player_str + gear_str;
                                                    Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                                                    // int -> string 변환 후 플레이어 정보 저장
                                                    networkManager.player[15] = result_str.ToString();

                                                    Debug.Log($"플레이어 스텟 정보[DEX] 값: {networkManager.player[15]}");
                                                    break;
                                                case "shoes_2":
                                                    Debug.Log($"장비 정보[shoes_2] - 키: {i} 값: {networkManager.player_gear[i]}");

                                                    // 장비에 따라 능력치 수정
                                                    player_str = Int32.Parse(networkManager.player[15]);
                                                    gear_str = 10;
                                                    result_str = player_str + gear_str;
                                                    Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                                                    // int -> string 변환 후 플레이어 정보 저장
                                                    networkManager.player[15] = result_str.ToString();

                                                    Debug.Log($"플레이어 스텟 정보[DEX] 값: {networkManager.player[15]}");
                                                    break;
                                                case "shoes_3":
                                                    Debug.Log($"장비 정보[shoes_3] - 키: {i} 값: {networkManager.player_gear[i]}");

                                                    // 장비에 따라 능력치 수정
                                                    player_str = Int32.Parse(networkManager.player[15]);
                                                    gear_str = 15;
                                                    result_str = player_str + gear_str;
                                                    Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                                                    // int -> string 변환 후 플레이어 정보 저장
                                                    networkManager.player[15] = result_str.ToString();

                                                    Debug.Log($"플레이어 스텟 정보[DEX] 값: {networkManager.player[15]}");
                                                    break;
                                                default:
                                                    //  Debug.Log($"장비 정보[기타] - 키: {i} 값: {networkManager.player_gear[i]}");
                                                    break;
                                            }
                                            break;
                                        case 6:// 오른쪽 링
                                            Debug.Log($"case 6 - 장비 정보 - 키: {i} 값: {networkManager.player_gear[i]}. networkManager.player[9]: {networkManager.player[9]}");

                                            // 장비에 따라 능력치 수정
                                            switch (networkManager.player_gear[i]) {
                                                case "ring_1":
                                                    // 이동한 장비가 맞다면 장비에 따라 능력치 수정
                                                    if (selectItem.GetComponent<InvenItem>().ItemName.Equals(networkManager.player_gear[i]) == true) {
                                                        int player_str = Int32.Parse(networkManager.player[9]); // MaxMp 값
                                                        int gear_str = 50;
                                                        //int result_str = player_str + gear_str + 50;
                                                        int result_str = player_str + gear_str;
                                                        // Debug.Log($"networkManager.player_gear[6] - ring_1 - player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                                                        // int -> string 변환 후 플레이어 정보 저장
                                                        networkManager.player[8] = result_str.ToString();
                                                        networkManager.player[9] = result_str.ToString();

                                                        Debug.Log($"캐릭터 상태창 UI정보 수정 - 플레이어 스텟 정보[마력] 값: {networkManager.player[8]}/{networkManager.player[9]}");
                                                        GameObject playerStat = GameObject.Find("CharacterState(Clone)");
                                                        if (playerStat != null)
                                                        {
                                                            // Debug.Log($" Defensive_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(1).transform.GetComponent<Text>().text} / Defensive_Plus_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text}");
                                                            playerStat.transform.GetChild(0).GetChild(4).GetChild(1).transform.GetComponent<Text>().text = networkManager.player[8];
                                                            playerStat.transform.GetChild(0).GetChild(4).GetChild(3).transform.GetComponent<Text>().text = networkManager.player[9];
                                                        }

                                                        // 인벤토리 내 마력 정보 수정
                                                        character.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Text>().text = "마력: " + networkManager.player[8] + " / " + networkManager.player[9];
                                                    }

                                                    break;
                                                case "ring_2":
                                                    // 이동한 장비가 맞다면 장비에 따라 능력치 수정
                                                    if (selectItem.GetComponent<InvenItem>().ItemName.Equals(networkManager.player_gear[i]) == true) {
                                                        int player_str = Int32.Parse(networkManager.player[9]); // MaxMp 값
                                                        int gear_str = 100;
                                                        //int result_str = player_str + gear_str + 50;
                                                        int result_str = player_str + gear_str;
                                                        // Debug.Log($"networkManager.player_gear[6] - ring_1 - player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                                                        // int -> string 변환 후 플레이어 정보 저장
                                                        networkManager.player[8] = result_str.ToString();
                                                        networkManager.player[9] = result_str.ToString();

                                                        Debug.Log($"캐릭터 상태창 UI정보 수정 - 플레이어 스텟 정보[마력] 값: {networkManager.player[8]}/{networkManager.player[9]}");
                                                        GameObject playerStat = GameObject.Find("CharacterState(Clone)");
                                                        if (playerStat != null)
                                                        {
                                                            // Debug.Log($" Defensive_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(1).transform.GetComponent<Text>().text} / Defensive_Plus_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text}");
                                                            playerStat.transform.GetChild(0).GetChild(4).GetChild(1).transform.GetComponent<Text>().text = networkManager.player[8];
                                                            playerStat.transform.GetChild(0).GetChild(4).GetChild(3).transform.GetComponent<Text>().text = networkManager.player[9];
                                                        }

                                                        // 인벤토리 내 마력 정보 수정
                                                        character.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Text>().text = "마력: " + networkManager.player[8] + " / " + networkManager.player[9];
                                                    }
                                                    break;
                                                case "ring_3":
                                                    // 이동한 장비가 맞다면 장비에 따라 능력치 수정
                                                    if (selectItem.GetComponent<InvenItem>().ItemName.Equals(networkManager.player_gear[i]) == true) {
                                                        int player_str = Int32.Parse(networkManager.player[9]); // MaxMp 값
                                                        int gear_str = 150;
                                                        //int result_str = player_str + gear_str + 50;
                                                        int result_str = player_str + gear_str;
                                                        // Debug.Log($"networkManager.player_gear[6] - ring_1 - player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                                                        // int -> string 변환 후 플레이어 정보 저장
                                                        networkManager.player[8] = result_str.ToString();
                                                        networkManager.player[9] = result_str.ToString();

                                                        Debug.Log($"캐릭터 상태창 UI정보 수정 - 플레이어 스텟 정보[마력] 값: {networkManager.player[8]}/{networkManager.player[9]}");
                                                        GameObject playerStat = GameObject.Find("CharacterState(Clone)");
                                                        if (playerStat != null)
                                                        {
                                                            // Debug.Log($" Defensive_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(1).transform.GetComponent<Text>().text} / Defensive_Plus_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text}");
                                                            playerStat.transform.GetChild(0).GetChild(4).GetChild(1).transform.GetComponent<Text>().text = networkManager.player[8];
                                                            playerStat.transform.GetChild(0).GetChild(4).GetChild(3).transform.GetComponent<Text>().text = networkManager.player[9];
                                                        }

                                                        // 인벤토리 내 마력 정보 수정
                                                        character.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Text>().text = "마력: " + networkManager.player[8] + " / " + networkManager.player[9];
                                                    }
                                                    break;
                                                default:
                                                    //  Debug.Log($"장비 정보[기타] - 키: {i} 값: {networkManager.player_gear[i]}");
                                                    break;
                                            }
                                            break;
                                        default:
                                            Debug.Log($"장비 정보[기타] - 키: {i} 값: {networkManager.player_gear[i]}");
                                            break;
                                    }

                                                              
                                }


                                // 인벤 슬롯 정보 조회
                                List<string> invenInfo = new List<string>();
                                foreach (KeyValuePair<int, GameObject> invenSlotResult in invenSlot) {
                                    if(invenSlotResult.Value == null) {
                                        // Debug.Log($"인벤 정보 조회 - Key: {invenSlotResult.Key} ,  Value: null");
                                        invenInfo.Add("없음");
                                    } else {
                                        // Debug.Log($"인벤 정보 조회 - Key: {invenSlotResult.Key} ,  Value: {invenSlotResult.Value.GetComponent<InvenItem>().ItemName}");
                                        invenInfo.Add(invenSlotResult.Value.GetComponent<InvenItem>().ItemName);
                                    }
                                }

                                //for (int i = 0; i < networkManager.player_gear.Count; i++) {
                                //    Debug.Log($"장비 정보 조회 - 키: {i} 값: {networkManager.player_gear[i]}");
                                //}

                                // 서버에 장비, 인벤, 플레이어 정보 전송
                                Debug.Log("장비칸에 장비 장착 후 인벤, 장비 정보 서버로 전송");
                                networkManager.PlayerStatUpdate();
                                networkManager.GeadInfoUpdate(networkManager.player_gear);
                                networkManager.InventoryInfoUpdate(invenInfo);
                            } else {
                                //Debug.Log("장비칸에 장비가 존재할 경우");
                                //Debug.Log($"선택한 장비 슬롯 정보: 키: {gearIndex} 값: {gearResult.name}");
                                //Debug.Log("선택한 장비 슬롯 정보 정보: " + gearSlot[gearIndex].transform.name);
                                //Debug.Log($"invenSlot[invenValue] 값: {invenSlot[invenValue].transform.parent.name}");

                                gearSlot[gearIndex].transform.SetParent(beforeSelectItemParent.transform);
                                RectTransform rect = (RectTransform)beforeSelectItemParent.transform.GetChild(0).transform;
                                rect.anchoredPosition = new Vector3(0f, 0f);

                                //기존 아이템이 이쪽 부모로 이동하게 설정 후 데이터값 변경
                                invenSlot[invenValue] = gearSlot[gearIndex];
                                gearSlot[gearIndex] = selectItem;
                            }
                        } else {
                            Debug.Log("목적지 장비 슬롯이 아닌 곳에 드래그한 경우");
                            //원래대로 이동
                            selectItem.transform.SetParent(beforeSelectItemParent.transform);
                            selectItem.transform.SetSiblingIndex(0);
                            //GameObject result = beforeSelectItemParent.GetChild(0).transform;
                            RectTransform rt = (RectTransform)beforeSelectItemParent.transform.GetChild(0).transform;
                            rt.anchoredPosition = new Vector3(0f, 0f);
                        }
                    } else {
                        Debug.Log("장비창 내 슬롯이 아닌 곳에 드래그한 경우");
                        //원래대로 이동
                        selectItem.transform.SetParent(beforeSelectItemParent.transform);
                        selectItem.transform.SetSiblingIndex(0);
                        //GameObject result = beforeSelectItemParent.GetChild(0).transform;
                        RectTransform rt = (RectTransform)beforeSelectItemParent.transform.GetChild(0).transform;
                        rt.anchoredPosition = new Vector3(0f, 0f);
                    }
                } else if (isInven == true) {
                   // Debug.Log("드래그 한 아이템 위치가 인벤토리 내부일 경우 - 작업 전");

                    // 인벤토리 내부 정보를 조회
                    foreach (KeyValuePair<RectTransform, int> pair in invenSlotRect) {

                        // 인벤토리 내부 슬롯 위치 조회
                        bool isItemSlot = RectTransformUtility.RectangleContainsScreenPoint(pair.Key, eventData.position);
                        if (isItemSlot == true) {
                            // Debug.Log($"드래그 한 아이템 위치가 어떤 인벤토리 슬롯 위에 있는지 조회 - Key: {pair.Key} ,  Value: {pair.Value}");
                            isSlotRect = pair.Key;
                            isSlotIndex = pair.Value;
                        }
                    }

                    if (isSlotRect != null) {
                         //Debug.Log($"인벤토리 슬롯 위에 커서가 있을 경우 처음 선택한 아이템 위치랑 현재 위치한 아이템 위치 변경");
                         //Debug.Log($"드래그한 아이템 정보: {beforeSelectItemParent.transform.name}");
                         //Debug.Log($"목적지 슬롯 위치 정보: {isSlotRect.transform.name}");
                         
                        GameObject result = null;
                        if (invenSlot.TryGetValue(isSlotIndex, out result) == true) {
                            if(result == null) {
                                // Debug.Log($"인벤토리 슬롯에 아이템이 없을 경우");

                                //장비 처리

                                // 처음으로 선택한 아이템이 목적지 아이템 위치로 이동
                                selectItem.transform.SetParent(isSlotRect.transform);
                                int nCount = isSlotRect.transform.childCount;
                                isSlotRect.transform.GetChild(nCount - 1).transform.SetSiblingIndex(0);
                                RectTransform rect = (RectTransform)isSlotRect.transform.GetChild(0).transform;
                                rect.anchoredPosition = new Vector3(0f, 0f);

                                //인벤토리 슬롯 값 변경
                                invenSlot[isSlotIndex] = selectItem;
                                // Debug.Log("드래그한 아이템 이름: " + invenSlot[isSlotIndex].GetComponent<InvenItem>().ItemName);

                                //인벤 기존값 조회 후 제거
                                foreach (KeyValuePair<RectTransform, int> pair in invenSlotRect) {
                                    if(beforeSelectItemParent.transform == pair.Key) {
                                        Debug.Log($"드래그한 아이템, 기존값 일치할 경우: {pair.Key},  {pair.Value}");
                                        invenSlot[pair.Value] = null;
                                    }
                                }

                                // 장비 슬롯 기존값 조회 후 제거
                                foreach (KeyValuePair<RectTransform, int> gearSlotResult in gearSlotRect) {
                                    if (beforeSelectItemParent.transform == gearSlotResult.Key) { 
                                        // Debug.Log($"장비 슬롯 기존값 조회 후 제거 - 키: {gearSlotResult.Key} 값: {gearSlotResult.Value}");
                                        gearSlot[gearSlotResult.Value] = null;
                                    }
                                }

                                // 아이템 정보 전송 - 캐릭터 능력치 업데이트(클라 수정 -> 서버 수정), 선택한 아이템 정보 조회 후 능력치 수정
                                Debug.Log($"선택한 아이템 정보 조회 후 능력치 수정");
                                switch (isSlotRect.transform.GetChild(0).GetComponent<InvenItem>().ItemName) {
                                    case "body_1":
                                        Debug.Log($"장비 정보[body_1] - 이동한 아이템 정보 정보: {isSlotRect.transform.GetChild(0).GetComponent<InvenItem>().ItemName}");
                                        // 아이템에 따라 능력치 다르게 증가
                                        
                                        int player_str = Int32.Parse(networkManager.player[13]);
                                        int gear_str = 5;
                                        int result_str = player_str - gear_str;

                                        Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                                        // int -> string 변환 후 플레이어 정보 저장
                                        networkManager.player[13] = result_str.ToString();

                                        Debug.Log($"플레이어 스텟 정보[Defense] 값: {networkManager.player[13]}");
                                        GameObject playerStat = GameObject.Find("CharacterState(Clone)");
                                        if(playerStat != null) { 
                                            playerStat.transform.GetChild(0).GetChild(6).GetChild(1).transform.GetComponent<Text>().text = networkManager.player[13];
                                            playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text = "0"; 
                                        }
                                        break;
                                    case "body_2":
                                        Debug.Log($"장비 정보[body_2] - 이동한 아이템 정보 정보: {isSlotRect.transform.GetChild(0).GetComponent<InvenItem>().ItemName}");

                                        // 아이템에 따라 능력치 다르게 증가
                                        player_str = Int32.Parse(networkManager.player[13]);
                                        gear_str = 10;
                                        result_str = player_str - gear_str;

                                        Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                                        // int -> string 변환 후 플레이어 정보 저장
                                        networkManager.player[13] = result_str.ToString();

                                        Debug.Log($"플레이어 스텟 정보[Defense] 값: {networkManager.player[13]}");
                                        playerStat = GameObject.Find("CharacterState(Clone)");
                                        if (playerStat != null) {
                                            playerStat.transform.GetChild(0).GetChild(6).GetChild(1).transform.GetComponent<Text>().text = networkManager.player[13];
                                            playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text = "0";
                                        }
                                        break;
                                    case "body_3":
                                        Debug.Log($"장비 정보[body_3] - 이동한 아이템 정보 정보: {isSlotRect.transform.GetChild(0).GetComponent<InvenItem>().ItemName}");

                                        // 아이템에 따라 능력치 다르게 증가
                                        player_str = Int32.Parse(networkManager.player[13]);
                                        gear_str = 15;
                                        result_str = player_str - gear_str;

                                        Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                                        // int -> string 변환 후 플레이어 정보 저장
                                        networkManager.player[13] = result_str.ToString();

                                        Debug.Log($"플레이어 스텟 정보[Defense] 값: {networkManager.player[13]}");
                                        playerStat = GameObject.Find("CharacterState(Clone)");
                                        if (playerStat != null) {
                                            playerStat.transform.GetChild(0).GetChild(6).GetChild(1).transform.GetComponent<Text>().text = networkManager.player[13];
                                            playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text = "0";
                                        }
                                        break; 
                                    case "head_1":
                                        Debug.Log($"장비 정보[head_1] - 이동한 아이템 정보 정보: {isSlotRect.transform.GetChild(0).GetComponent<InvenItem>().ItemName}");
                                        Debug.Log($"플레이어 스텟 정보[STR] 값: {networkManager.player[14]}");
                                        break; 
                                    case "head_2":
                                        Debug.Log($"장비 정보[head_2] - 이동한 아이템 정보 정보: {isSlotRect.transform.GetChild(0).GetComponent<InvenItem>().ItemName}");
                                        Debug.Log($"플레이어 스텟 정보[STR] 값: {networkManager.player[14]}");
                                        break;
                                    case "head_3":
                                        Debug.Log($"장비 정보[head_3] - 이동한 아이템 정보 정보: {isSlotRect.transform.GetChild(0).GetComponent<InvenItem>().ItemName}");
                                        Debug.Log($"플레이어 스텟 정보[STR] 값: {networkManager.player[14]}");
                                        break;
                                    case "ring_1":
                                        //Debug.Log($"장비 정보[ring_1] - 이동한 아이템 정보 정보: {isSlotRect.transform.GetChild(0).GetComponent<InvenItem>().ItemName}");
                                        //Debug.Log($"플레이어 스텟 정보[마력] 값: {networkManager.player[8]}/{networkManager.player[9]}");

                                        // 장비에 따라 능력치 수정
                                        player_str = Int32.Parse(networkManager.player[9]); // MaxMp 값
                                        gear_str = 50;
                                        result_str = player_str - gear_str;
                                        // Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                                        // int -> string 변환 후 플레이어 정보 저장
                                        networkManager.player[8] = result_str.ToString();
                                        networkManager.player[9] = result_str.ToString();

                                        Debug.Log($"캐릭터 상태창 UI정보 수정 - 플레이어 스텟 정보[마력] 값: {networkManager.player[8]}/{networkManager.player[9]}");
                                        playerStat = GameObject.Find("CharacterState(Clone)");
                                        if (playerStat != null)
                                        {
                                            // Debug.Log($" Defensive_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(1).transform.GetComponent<Text>().text} / Defensive_Plus_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text}");
                                            playerStat.transform.GetChild(0).GetChild(4).GetChild(1).transform.GetComponent<Text>().text = networkManager.player[8];
                                            playerStat.transform.GetChild(0).GetChild(4).GetChild(3).transform.GetComponent<Text>().text = networkManager.player[9];
                                        }

                                        // 인벤토리 내 마력 정보 수정
                                        transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(2).GetComponent<Text>().text = "마력: " + networkManager.player[8] + " / " + networkManager.player[9];

                                        break;
                                    case "ring_2":
                                        Debug.Log($"장비 정보[ring_2] - 이동한 아이템 정보 정보: {isSlotRect.transform.GetChild(0).GetComponent<InvenItem>().ItemName}");
                                        Debug.Log($"플레이어 스텟 정보[마력] 값: {networkManager.player[8]}/{networkManager.player[9]}");

                                        // 장비에 따라 능력치 수정
                                        player_str = Int32.Parse(networkManager.player[9]); // MaxMp 값
                                        gear_str = 100;
                                        result_str = player_str - gear_str;
                                        Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                                        // int -> string 변환 후 플레이어 정보 저장
                                        networkManager.player[8] = result_str.ToString();
                                        networkManager.player[9] = result_str.ToString();

                                        Debug.Log($"플레이어 스텟 정보[마력] 값: {networkManager.player[8]}/{networkManager.player[9]}");

                                        Debug.Log($"캐릭터 상태창 UI정보 수정");
                                        playerStat = GameObject.Find("CharacterState(Clone)");
                                        if (playerStat != null) {
                                            // Debug.Log($" Defensive_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(1).transform.GetComponent<Text>().text} / Defensive_Plus_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text}");
                                            playerStat.transform.GetChild(0).GetChild(4).GetChild(1).transform.GetComponent<Text>().text = networkManager.player[8];
                                            playerStat.transform.GetChild(0).GetChild(4).GetChild(3).transform.GetComponent<Text>().text = networkManager.player[9];
                                        }

                                        // 인벤토리 내 마력 정보 수정
                                        transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(2).GetComponent<Text>().text = "마력: " + networkManager.player[8] + " / " + networkManager.player[9];

                                        break;
                                    case "ring_3":
                                        Debug.Log($"장비 정보[ring_3] - 이동한 아이템 정보 정보: {isSlotRect.transform.GetChild(0).GetComponent<InvenItem>().ItemName}");
                                        Debug.Log($"플레이어 스텟 정보[마력] 값: {networkManager.player[8]}/{networkManager.player[9]}");

                                        // 장비에 따라 능력치 수정
                                        player_str = Int32.Parse(networkManager.player[9]); // MaxMp 값
                                        gear_str = 150;
                                        result_str = player_str - gear_str;
                                        Debug.Log($"player_str 값: {player_str}, gear_str 값: {gear_str}, result_str 값: {result_str}");

                                        // int -> string 변환 후 플레이어 정보 저장
                                        networkManager.player[8] = result_str.ToString();
                                        networkManager.player[9] = result_str.ToString();

                                        Debug.Log($"플레이어 스텟 정보[마력] 값: {networkManager.player[8]}/{networkManager.player[9]}");

                                        Debug.Log($"캐릭터 상태창 UI정보 수정");
                                        playerStat = GameObject.Find("CharacterState(Clone)");
                                        if (playerStat != null)
                                        {
                                            // Debug.Log($" Defensive_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(1).transform.GetComponent<Text>().text} / Defensive_Plus_Value: {playerStat.transform.GetChild(0).GetChild(6).GetChild(4).transform.GetComponent<Text>().text}");
                                            playerStat.transform.GetChild(0).GetChild(4).GetChild(1).transform.GetComponent<Text>().text = networkManager.player[8];
                                            playerStat.transform.GetChild(0).GetChild(4).GetChild(3).transform.GetComponent<Text>().text = networkManager.player[9];
                                        }

                                        // 인벤토리 내 마력 정보 수정
                                        transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(2).GetComponent<Text>().text = "마력: " + networkManager.player[8] + " / " + networkManager.player[9];

                                        break;
                                    case "shoes_1":
                                        Debug.Log($"장비 정보[shoes_1] - 이동한 아이템 정보 정보: {isSlotRect.transform.GetChild(0).GetComponent<InvenItem>().ItemName}");
                                        Debug.Log($"플레이어 스텟 정보[DEX] 값: {networkManager.player[15]}");
                                        break;
                                    case "shoes_2":
                                        Debug.Log($"장비 정보[shoes_2] - 이동한 아이템 정보 정보: {isSlotRect.transform.GetChild(0).GetComponent<InvenItem>().ItemName}");
                                        Debug.Log($"플레이어 스텟 정보[DEX] 값: {networkManager.player[15]}");
                                        break;
                                    case "shoes_3":
                                        Debug.Log($"장비 정보[shoes_3] - 이동한 아이템 정보 정보: {isSlotRect.transform.GetChild(0).GetComponent<InvenItem>().ItemName}");
                                        Debug.Log($"플레이어 스텟 정보[DEX] 값: {networkManager.player[15]}");
                                        break;
                                    default:
                                        //  Debug.Log($"장비 정보[기타] - 키: {i} 값: {networkManager.player_gear[i]}");
                                        break;
                                    }
                            } else {
                                Debug.Log($"인벤토리 슬롯에 아이템이 있을 경우");

                                // 처음으로 선택한 아이템이 목적지 아이템 위치로 이동
                                selectItem.transform.SetParent(isSlotRect.transform);
                                int nCount = isSlotRect.transform.childCount;
                                isSlotRect.transform.GetChild(nCount - 1).transform.SetSiblingIndex(0);
                                RectTransform rect = (RectTransform)isSlotRect.transform.GetChild(0).transform;
                                rect.anchoredPosition = new Vector3(0f, 0f);

                                //인벤토리 슬롯 값 변경
                                invenSlot[isSlotIndex] = selectItem;
                                Debug.Log("invenSlot[isSlotIndex].GetComponent<InvenItem>().ItemName: " + invenSlot[isSlotIndex].GetComponent<InvenItem>().ItemName);

                                //기존값 조회 후 변경
                                foreach (KeyValuePair<RectTransform, int> pair in invenSlotRect) {
                                    if (beforeSelectItemParent.transform == pair.Key) {
                                        Debug.Log($"드래그한 아이템, 기존값 일치할 경우: {pair.Key},  {pair.Value}");
                                        // 목적지 아이템 정보를 처음 선택한 아이템 위치로 이동
                                        nCount = isSlotRect.transform.childCount;
                                        isSlotRect.transform.GetChild(nCount - 1).SetParent(beforeSelectItemParent.transform);
                                        rect = (RectTransform)beforeSelectItemParent.transform.GetChild(0).transform;
                                        rect.anchoredPosition = new Vector3(0f, 0f);
                                        invenSlot[pair.Value] = beforeSelectItemParent.transform.GetChild(0).gameObject;
                                    }
                                }
                            }
                        }
                    } else {
                        Debug.Log("인벤토리 슬롯 내 슬롯이 아닌 곳에 드래그한 경우");
                        selectItem.transform.SetParent(beforeSelectItemParent.transform);
                        selectItem.transform.SetSiblingIndex(0);
                        RectTransform rt = (RectTransform)beforeSelectItemParent.transform.GetChild(0).transform;
                        rt.anchoredPosition = new Vector3(0f, 0f);
                    }

                    // 여기서 서버에 전송해야할 듯?
                    // Debug.Log("드래그 한 아이템 위치가 인벤토리 내부일 경우 - 작업 후");

                    //인벤토리 UI 정보 대로 인벤토리 Data 수정
                    // Debug.Log("인벤토리 UI 정보");
                    foreach (KeyValuePair<int, GameObject> pair in invenSlot) {
                        if(pair.Value == null) {
                            // Debug.Log($"인벤 슬롯 번호: {pair.Key}  아이템 명e: null");
                            networkManager.inven[pair.Key] = "없음";
                        } else {
                            // Debug.Log($"인벤 슬롯 번호: {pair.Key}  아이템 명: {pair.Value.GetComponent<InvenItem>().ItemName}");
                            networkManager.inven[pair.Key] = pair.Value.GetComponent<InvenItem>().ItemName;
                        }
                    }

                    //Debug.Log("인벤토리 Data 정보");
                    //for (int i =0; i < networkManager.inven.Count;i++) {
                    //    Debug.Log($"i: {i}  Value: {networkManager.inven[i]}");
                    //}

                    // 서버에 전송할 정보: 인벤토리 정보
                    // 인벤토리 정보 재생성 후 전송
                    Debug.Log("수정된 인벤토리 정보 서버 전송");
                    Dictionary<string, List<string>> invenList = new Dictionary<string, List<string>>();
                    invenList.Add(networkManager.player[1], networkManager.inven);
                    networkManager.ReInventoryUpdate(invenList);

                    // 장비 정보 조회
                    // GeadInfoUpdate(List<string> gearList
                    List<string> gearList = new List<string>();
                    foreach (KeyValuePair<int, GameObject> gearValue in gearSlot) {
                        if(gearValue.Value != null) {
                            // Debug.Log($"장비 정보: 키: {gearValue.Key} 값: {gearValue.Value.GetComponent<InvenItem>().ItemName}");
                            gearList.Add(gearValue.Value.GetComponent<InvenItem>().ItemName);
                        } else {
                            // Debug.Log($"장비 정보: 키: {gearValue.Key} 값: 없음");
                            gearList.Add("없음");
                        }
                    }

                    //장비 정보도 전송
                    Debug.Log("수정된 장비 정보 서버 전송");
                    networkManager.GeadInfoUpdate(gearList);

                    //플레이어 능력치도 전송
                }
                else {
                    Debug.Log("인벤토리 슬롯 내 슬롯이 아닌 곳에 드래그한 경우");
                    selectItem.transform.SetParent(beforeSelectItemParent.transform);
                    selectItem.transform.SetSiblingIndex(0);
                    RectTransform rt = (RectTransform)beforeSelectItemParent.transform.GetChild(0).transform;
                    rt.anchoredPosition = new Vector3(0f, 0f);
                }
            }

            selectItem = null;
            beforeSelectItemParent = null;
            beforeSelectItem = null;
        }
    }
}