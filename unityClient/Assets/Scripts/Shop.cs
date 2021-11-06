using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    GameObject content;
    GameObject shopUI;
    Dictionary<int, GameObject> itemList;
    GameObject target;
    int targetIndex = 0;
    int clickCount = 0;
    GameObject alertView;
    bool isAlert = false;
    NetworkManager network;

    void Start() {
        // 네트워크 통신을 위해 NetworkManager 객체 검색 후 생성
        GameObject networkManager = GameObject.Find("@NetworkManager");
        network = networkManager.GetComponent<NetworkManager>();

        itemList = new Dictionary<int, GameObject>();

        //창닫기 버튼 이벤트를 추가하기 위한 객체 생성
        GameObject cancel = GameObject.Find("ShopClose");

        if (cancel != null) {

            // 창닫기 버튼에 이벤트 추가
            cancel.GetComponent<Button>().onClick.AddListener(ShopCancel);

            // 상점 창 화면 정보를 가져와서 생성
            shopUI = GameObject.Find("ShopUI(Clone)");
            content = shopUI.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).gameObject;

            //Debug.Log("content.transform.childCount: " + content.transform.childCount);
            //Debug.Log("content.transform.GetChild(0).name: " + content.transform.GetChild(0).name);

            // 상점창 목록에 물건 생성
            for (int i = 0; i < content.transform.childCount; i++) {
                switch (i) {
                    case 0:
                        GameObject item = content.transform.GetChild(i).gameObject;
                        item.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Item/body_1") as Sprite;
                        item.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "body_1";
                        item.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "상품 정보: 초보자용 갑옷";
                        item.transform.GetChild(1).GetChild(2).GetComponent<Text>().text = "가격: 1000원";
                        itemList.Add(i, item);
                        break;
                    case 1:
                        item = content.transform.GetChild(i).gameObject;
                        item.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Item/body_2") as Sprite;
                        item.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "body_2";
                        item.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "상품 정보: 갑옷 2단계";
                        item.transform.GetChild(1).GetChild(2).GetComponent<Text>().text = "가격: 2000원";
                        itemList.Add(i, item);
                        break;
                    case 2:
                        item = content.transform.GetChild(i).gameObject;
                        item.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Item/body_3") as Sprite;
                        item.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "body_3";
                        item.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "상품 정보: 갑옷 3단계";
                        item.transform.GetChild(1).GetChild(2).GetComponent<Text>().text = "가격: 3000원";
                        itemList.Add(i, item);
                        break;
                    case 3:
                        item = content.transform.GetChild(i).gameObject;
                        item.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Item/head_1") as Sprite;
                        item.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "head_1";
                        item.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "상품 정보: 초보자용 투구";
                        item.transform.GetChild(1).GetChild(2).GetComponent<Text>().text = "가격: 1000원";
                        itemList.Add(i, item);
                        break;
                    case 4:
                        item = content.transform.GetChild(i).gameObject;
                        item.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Item/head_2") as Sprite;
                        item.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "head_2";
                        item.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "상품 정보: 투구 2단계";
                        item.transform.GetChild(1).GetChild(2).GetComponent<Text>().text = "가격: 2000원";
                        itemList.Add(i, item);
                        break;
                    case 5:
                        item = content.transform.GetChild(i).gameObject;
                        item.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Item/head_3") as Sprite;
                        item.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "head_3";
                        item.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "상품 정보: 투구 3단계";
                        item.transform.GetChild(1).GetChild(2).GetComponent<Text>().text = "가격: 3000원";
                        itemList.Add(i, item);
                        break;
                    case 6:
                        item = content.transform.GetChild(i).gameObject;
                        item.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Item/ring_1") as Sprite;
                        item.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "ring_1";
                        item.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "상품 정보: 초보자용 반지";
                        item.transform.GetChild(1).GetChild(2).GetComponent<Text>().text = "가격: 1000원";
                        itemList.Add(i, item);
                        break;
                    case 7:
                        item = content.transform.GetChild(i).gameObject;
                        item.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Item/ring_2") as Sprite;
                        item.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "ring_2";
                        item.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "상품 정보: 금 반지";
                        item.transform.GetChild(1).GetChild(2).GetComponent<Text>().text = "가격: 2000원";
                        itemList.Add(i, item);
                        break;
                    case 8:
                        item = content.transform.GetChild(i).gameObject;
                        item.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Item/ring_3") as Sprite;
                        item.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "ring_3";
                        item.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "상품 정보: 금 반지";
                        item.transform.GetChild(1).GetChild(2).GetComponent<Text>().text = "가격: 3000원";
                        itemList.Add(i, item);
                        break;
                    case 9:
                        item = content.transform.GetChild(i).gameObject;
                        item.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Item/shoes_1") as Sprite;
                        item.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "shoes_1";
                        item.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "상품 정보: 초보자용 신발";
                        item.transform.GetChild(1).GetChild(2).GetComponent<Text>().text = "가격: 1000원";
                        itemList.Add(i, item);
                        break;
                    case 10:
                        item = content.transform.GetChild(i).gameObject;
                        item.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Item/shoes_2") as Sprite;
                        item.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "shoes_2";
                        item.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "상품 정보: 신발 2단계";
                        item.transform.GetChild(1).GetChild(2).GetComponent<Text>().text = "가격: 2000원";
                        itemList.Add(i, item);
                        break;
                    case 11:
                        item = content.transform.GetChild(i).gameObject;
                        item.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Item/shoes_3") as Sprite;
                        item.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "shoes_3";
                        item.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "상품 정보: 신발 3단계";
                        item.transform.GetChild(1).GetChild(2).GetComponent<Text>().text = "가격: 3000원";
                        itemList.Add(i, item);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void LateUpdate() {

        // 마우스 왼쪽 버튼 클릭시 실행
        if (Input.GetMouseButtonDown(0)) {

            if (isAlert == false) {

                for (int i = 0; i < itemList.Count; i++) {
                    if (RectTransformUtility.RectangleContainsScreenPoint(itemList[i].transform as RectTransform, Input.mousePosition) == true) {
                        //Debug.Log($"클릭 범위 체크 {i}: " + RectTransformUtility.RectangleContainsScreenPoint(itemList[i].transform as RectTransform, Input.mousePosition));

                        if (target == null) {
                            //Debug.Log($"처음 선택한 경우 {i}");
                            target = itemList[i];
                            targetIndex = i;
                            clickCount = 1;
                        } else if (target.name.Equals(itemList[i].name) == true) {
                            //Debug.Log($"동일한 아이템을 한번더 클릭한 경우 {i}");
                            targetIndex = i;
                            clickCount++;
                        } else {
                            Debug.Log($"다른 아이템을 선택한 경우 {i}");
                            target = itemList[i];
                            targetIndex = i;
                            clickCount = 1;
                        }
                    }
                }

                // 물건을 두번 클릭시 실행
                if (target != null && clickCount == 2) {
                    //플레이어 돈 확인 돈이 있을 경우 금액 차감 후 인벤 추가
                    //돈이 없을경우 경고창 출력
                    //Debug.Log("target: " + target.transform.name);
                    //Debug.Log("clickCount: " + clickCount);
                    //Debug.Log("target money: " + target.transform.GetChild(1).GetChild(2).transform.GetComponent<Text>().text);
                    //Debug.Log("money: " + mainScene.money);
                    clickCount = 0;

                    switch(target.transform.GetChild(1).GetChild(2).transform.GetComponent<Text>().text) {
                        case "가격: 1000원":
                            int price = 1000;
                            ShopProcess(price, target.transform);
                            break;
                        case "가격: 2000원":
                            price = 2000;
                            ShopProcess(price, target.transform);
                            break;
                        case "가격: 3000원":
                            price = 3000;
                            ShopProcess(price, target.transform);
                            break;
                        default:
                            Debug.Log("물건 구매 예외사항");
                            break;
                    }
                }
            }
        }
    }

    // 물건 구매시 실행
    void ShopProcess(int price, Transform transform)
    {
        int player_money = 0;
        if (Int32.TryParse(network.player[5], out player_money))
        {
            //Debug.Log("현재 보유중인 돈: " + player_money);
            //Debug.Log("물품 가격: " + price);

            //플레이어 소지금액이 가격보다 크거나 같을 경우에만 구매
            if (player_money >= price) {
                Dictionary<int, GameObject> invenSlot = Inventory.invenSlot;
                Dictionary<RectTransform, int> invenSlotRect = Inventory.invenSlotRect;

                // 구매 완료할 경우: 금액 차감 -> 인벤토리 추가 -> 데이터베이스에 인벤토리 정보, 금액 전송 
                // Debug.Log("invenSlot.Count: " + invenSlot.Count);
                for (int i = 0; i < invenSlot.Count; i++) {
                    if (invenSlot[11] != null) {
                        // 알림창 프리팹을 찾은 후 읽어서 실행
                        alertView = Managers.Resource.Instantiate("AlertView");
                        isAlert = true;

                        //내용 변경
                        Text title = GameObject.Find("Message").GetComponent<Text>();
                        title.text = "인벤토리 공간이 부족합니다.";

                        //버튼 이벤트 추가
                        GameObject ok = GameObject.Find("Ok Button").gameObject;
                        ok.GetComponent<Button>().onClick.AddListener(() => {
                            Managers.Resource.Destroy(alertView);
                            isAlert = false;
                        }); ;
                        break;
                    }

                    if (invenSlot[i] == null) {
                        // Debug.Log("인벤 정보: null");

                        //슬롯 정보 검색 후 인벤토리에 아이템 추가
                        foreach (KeyValuePair<RectTransform, int> invenSlotResult in invenSlotRect) {
                            if (invenSlotResult.Value == i) {
                                //Debug.Log($"invenSlotResult.Value [인벤토리 슬롯 번호]: {invenSlotResult.Value}");
                                // Debug.Log($"invenSlotResult.Key.transform [인벤토리 슬롯 이름]: {invenSlotResult.Key.transform.name}");
                                Debug.Log("구매한 물품: " + transform.GetChild(1).GetChild(0).transform.GetComponent<Text>().text);
                                
                                //아이템 정보를 가져와서 추가 [UI 부분]
                                GameObject Info = Managers.Resource.Instantiate("UI/SubItem/Info");
                                Info.name = "Info";
                                InvenItem item = Info.AddComponent<InvenItem>();
                                item.ItemName = transform.GetChild(1).GetChild(0).transform.GetComponent<Text>().text;
                                Info.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/Item/{transform.GetChild(1).GetChild(0).transform.GetComponent<Text>().text}") as Sprite;
                                Info.transform.SetParent(invenSlotResult.Key.transform);
                                RectTransform rt = (RectTransform)Info.transform;
                                rt.anchoredPosition = new Vector3(0f, 0f);
                                invenSlot[i] = Info;

                                Debug.Log("인벤토리 정보 최신화");
                                // 인벤토리 정보 수정 [Data 부분]
                                // 매개변수: 슬롯 번호, 구매한 아이템 이름
                                
                                // 물품 차감
                                int result = player_money - price;
                                network.player[5] = "" + result;

                                network.ShopBuyUpdate(invenSlotResult.Value, transform.GetChild(1).GetChild(0).transform.GetComponent<Text>().text, result);

                                Debug.Log("계산 결과: " + result);
                                Debug.Log("구매한 물품: " + transform.GetChild(1).GetChild(0).transform.GetComponent<Text>().text);
                                break;
                            }
                        }
                        break;
                    }
                }
            } else {
                // 플레이어가 가지고 있는 돈이 부족할 경우 실행
                alertView = Managers.Resource.Instantiate("AlertView");
                isAlert = true;

                //내용 변경
                Text title = GameObject.Find("Message").GetComponent<Text>();
                title.text = "금액이 부족합니다.";

                //버튼 이벤트 추가
                GameObject ok = GameObject.Find("Ok Button").gameObject;
                ok.GetComponent<Button>().onClick.AddListener(() => {
                    Managers.Resource.Destroy(alertView);
                    isAlert = false;
                });
            }
        }
    }

    public void ShopCancel()
    {
        Debug.Log("Shop - Cancel");
        Managers.Resource.Destroy(shopUI.gameObject);
    }
}