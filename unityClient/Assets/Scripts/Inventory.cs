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
    NetworkManager networkManager;
    GameObject cancel;
    static GameObject Inven;
    GameObject character;
    GameObject selectItem;
    GameObject beforeSelectItemParent;
    GameObject beforeSelectItem;
    static GameObject gear;
    static GameObject head;
    static GameObject weapon;
    static GameObject body;
    static GameObject waist;
    static GameObject ring_left;
    static GameObject shoes;
    static GameObject ring_right;
    static bool isInvan = false;
    int before_invenSlot_index = 0;
    GameObject before_invenSlot = null;
    int after_invenSlot_index = 0;
    GameObject after_invenSlot = null;
    RectTransform isSlotRect = null;
    int isSlotIndex = 0;

    public static Dictionary<int, GameObject> invenSlot { get; set; }
    public static Dictionary<RectTransform, int> invenSlotRect { get; set; }

    public static Dictionary<int, GameObject> gearSlot { get; set; }
    public static Dictionary<RectTransform, int> gearSlotRect { get; set; }

    private void Awake()
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


        // 데이터베이스에서 실제 인벤토리 정보 참고해서 생성
        GameObject network = GameObject.Find("@NetworkManager");
        networkManager = network.GetComponent<NetworkManager>();
        networkManager.InventoryOpen();
        networkManager.GearOpen();

        if (networkManager.player[2].Equals("남자 캐릭터") == true)
        {
            transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/player_male") as Sprite;
        }
        else
        {
            transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/player_female") as Sprite;
        }

        transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = "레벨: " + networkManager.player[3];
        transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>().text = "체력: " + networkManager.player[6]+" / " + networkManager.player[7];
        transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(2).GetComponent<Text>().text = "마력: " + networkManager.player[8] + " / " + networkManager.player[9];
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

        // 인벤토리에 슬롯 추가
        for (int i = 0; i < recv.Count(); i++)
        {
            //Debug.Log($"recvGear - gearSlot {i}번째 값:{recv[i]}");
            if (recv[i].Equals("없음") == true)
            {
                gearSlot.Add(i, null);
            }
            else
            {
                GameObject Info = Managers.Resource.Instantiate("UI/SubItem/Info");
                Info.name = "Info";
                InvenItem item = Info.AddComponent<InvenItem>();
                item.ItemName = recv[i];
                Info.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/Item/{recv[i]}") as Sprite;

                if (i == 0)
                {
                    Info.transform.SetParent(head.transform);
                }
                else if (i == 1)
                {
                    Info.transform.SetParent(weapon.transform);
                }
                else if (i == 2)
                {
                    Info.transform.SetParent(body.transform);
                }
                else if (i == 3)
                {
                    Info.transform.SetParent(waist.transform);
                }
                else if (i == 4)
                {
                    Info.transform.SetParent(ring_left.transform);
                }
                else if (i == 5)
                {
                    Info.transform.SetParent(shoes.transform);
                }
                else if (i == 6)
                {
                    Info.transform.SetParent(ring_right.transform);
                }

                Info.AddComponent<InvenItem>().ItemName = recv[i];
                RectTransform rt = (RectTransform)Info.transform;
                rt.anchoredPosition = new Vector3(0f, 0f);
                gearSlot.Add(i, Info);

            }
        }
    }

    public static void recvInven(List<string> recv)
    {
        invenSlot = new Dictionary<int, GameObject>();
        invenSlotRect = new Dictionary<RectTransform, int>();


        // 인벤토리에 슬롯 추가
        for (int i = 0; i < recv.Count(); i++)
        {
            //Debug.Log($"recvInven - inven {i}번째 값:{recv[i]}");

            // 데이터베이스에 저장된 아이템 정보에 따라 값을 다르게 슬롯안 아이템 지정
            if(recv[i].Equals("없음") == true)
            {
                GameObject item = Managers.Resource.Instantiate("UI/SubItem/Slot_Empty");
                item.transform.SetParent(Inven.transform);// 너의 부모는 누구다 라고 지정하는 함수
                item.name = $"slot_{i}"; // 오브젝트 이름 변경
                invenSlot.Add(i, null);
                invenSlotRect.Add(item.transform as RectTransform, i);
            } else
            {
                GameObject item = Managers.Resource.Instantiate("UI/SubItem/Slot");
                item.transform.SetParent(Inven.transform);// 너의 부모는 누구다 라고 지정하는 함수
                item.name = $"slot_{i}"; // 오브젝트 이름 변경
                //Debug.Log("recv[i]: "+ recv[i]);
                
                GameObject Info = item.transform.Find("Info").gameObject;
                Info.transform.SetParent(item.transform);
                Info.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/Item/{recv[i]}") as Sprite;
                Info.AddComponent<InvenItem>().ItemName = recv[i];
                invenSlot.Add(i, Info);
                invenSlotRect.Add(item.transform as RectTransform, i);
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
        //드래그 시작 전 부모 클래스 저장


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

    public void OnEndDrag(PointerEventData eventData)
    {
        if (selectItem != null)
        {
            // 스크린상의 특정 포인트가 지정된 사각 범위 꼭지점 안에 포함되는지 결과를 리턴(인벤토리 밖인지 안인지 체크)
            isInvan = RectTransformUtility.RectangleContainsScreenPoint(Inven.transform.parent as RectTransform, eventData.position);

            // 인벤토리 밖일 경우 true, 안일 경우 false
            if (isInvan == false)
            {
                // 경고창 출력 true, false 처리
                Debug.Log($"인벤토리 밖");

                //원래대로 이동
                selectItem.transform.SetParent(beforeSelectItemParent.transform);
                selectItem.transform.SetSiblingIndex(0);
                //GameObject result = beforeSelectItemParent.GetChild(0).transform;
                RectTransform rt = (RectTransform)beforeSelectItemParent.transform.GetChild(0).transform;
                rt.anchoredPosition = new Vector3(0f, 0f);


                // 인벤 슬롯 정보 조회 후 제거
                foreach (KeyValuePair<int, GameObject> invenSlotResult in invenSlot)
                {
                    if (invenSlotResult.Value == selectItem)
                    {
                        Debug.Log($"인벤 슬롯 정보 조회 - 키: {invenSlotResult.Key} 값: {invenSlotResult.Value}");
                        Managers.Resource.Destroy(invenSlotResult.Value);
                        invenSlot[invenSlotResult.Key] = null;
                        break;
                    }
                }

            }
            else
            {
                bool isGear = RectTransformUtility.RectangleContainsScreenPoint(gear.transform as RectTransform, eventData.position);
                bool isInven = RectTransformUtility.RectangleContainsScreenPoint(Inven.transform as RectTransform, eventData.position);

                // 범위 값 조회 검색 결과 저장 목적
                RectTransform gearResult = null;
                int gearIndex = 0;

                if (isGear == true)
                {
                    Debug.Log("장비 슬롯 안일 경우");

                    foreach (KeyValuePair<RectTransform, int> gearValue in gearSlotRect)
                    {
                        bool isGearSlot = RectTransformUtility.RectangleContainsScreenPoint(gearValue.Key, eventData.position);
                        if (isGearSlot == true)
                        {
                            Debug.Log("마우스커서 위치가 어떤 장비 슬롯 위에 있는지 조회");
                            Debug.Log($"isGearSlot: 키: {gearValue.Key} 값: {gearValue.Value}");
                            gearIndex = gearValue.Value;
                            gearResult = gearValue.Key;
                        }
                    }

                    if (gearResult != null)
                    {
                        int invenValue = 0;
                        GameObject gearInfo = null;
                        if (gearSlot.TryGetValue(gearIndex, out gearInfo) == true)
                        {
                            //장비칸에 아무것도 장착하지 않을 경우 null
                            if (gearInfo == null)
                            {
                                Debug.Log($"목적지 장비 슬롯 정보 - 값: null");
                            }
                            else
                            {
                                Debug.Log($"목적지 장비 슬롯 정보 - 값: {gearInfo.name}");
                            }

                            // 인벤 슬롯 정보 제거하고 장비 슬롯 정보로 이동
                            Debug.Log($"선택한 아이템 슬롯 값: {beforeSelectItemParent.transform.name}");

                            Debug.Log($"목적지 장비 슬롯 정보: 키: {gearIndex} 값: {gearResult}");

                            // 인벤 슬롯 정보 조회
                            foreach (KeyValuePair<int, GameObject> invenSlotResult in invenSlot)
                            {
                                if (invenSlotResult.Value != null && invenSlotResult.Value.transform.parent.name.Equals("CharacterInfo(Clone)") == true)
                                {
                                    Debug.Log($"인벤 슬롯 정보 일치 - 키: {invenSlotResult.Key} 값: {invenSlotResult.Value.transform.parent.name}");
                                    invenValue = invenSlotResult.Key;
                                }
                           
                            }

                            selectItem.transform.SetParent(gearResult.transform);
                            selectItem.transform.SetSiblingIndex(0);
                            RectTransform rt = (RectTransform)gearResult.GetChild(0).transform;
                            rt.anchoredPosition = new Vector3(0f, 0f);

                            //인벤 슬롯에 있는 아이템을 장비 슬롯 아이템위치로 이동, 슬롯 장비에는 null 값 추가
                            if (gearSlot[gearIndex] == null)
                            {
                                Debug.Log("장비칸에 장비가 없을 경우");
                                Debug.Log("gearSlot[gearIndex]: " + gearSlot[gearIndex]);
                                gearSlot[gearIndex] = selectItem;
                                invenSlot[invenValue] = null;

                                // 장비 슬롯 정보 조회
                                foreach (KeyValuePair<int, GameObject> gearSlotResult in gearSlot)
                                {
                                    if (gearSlotResult.Value != null)
                                    {
                                        Debug.Log($"장비 슬롯 정보 조회 - 키: {gearSlotResult.Key} 값: {gearSlotResult.Value.name}");
                                    }
                                }

                                // 인벤 슬롯 정보 조회
                                foreach (KeyValuePair<int, GameObject> invenSlotResult in invenSlot)
                                {

                                    if (invenSlotResult.Value == null)
                                    {
                                        Debug.Log($"인벤 슬롯 정보 일치 - 키: {invenSlotResult.Key} 값: null");
                                    }
                                }
                            }
                            else
                            {
                                Debug.Log("장비칸에 장비가 존재할 경우");
                                Debug.Log($"선택한 장비 슬롯 정보: 키: {gearIndex} 값: {gearResult.name}");
                                Debug.Log("선택한 장비 슬롯 정보 정보: " + gearSlot[gearIndex].transform.name);
                                Debug.Log($"invenSlot[invenValue] 값: {invenSlot[invenValue].transform.parent.name}");

                                gearSlot[gearIndex].transform.SetParent(beforeSelectItemParent.transform);
                                RectTransform rect = (RectTransform)beforeSelectItemParent.transform.GetChild(0).transform;
                                rect.anchoredPosition = new Vector3(0f, 0f);

                                //기존 아이템이 이쪽 부모로 이동하게 설정 후 데이터값 변경
                                invenSlot[invenValue] = gearSlot[gearIndex];
                                gearSlot[gearIndex] = selectItem;
                            }


                            //// 장비 슬롯 정보 조회
                            //foreach (KeyValuePair<int, GameObject> gearSlotResult in gearSlot)
                            //{
                            //    if (gearSlotResult.Value != null)
                            //    {
                            //        Debug.Log($"장비 슬롯 정보 조회 - 키: {gearSlotResult.Key} 값: {gearSlotResult.Value.name}");
                            //    }
                            //    else
                            //    {
                            //        Debug.Log($"장비 슬롯 정보 조회 - 키: {gearSlotResult.Key} 값: null");
                            //    }
                            //}

                            //// 인벤 슬롯 정보 조회
                            //foreach (KeyValuePair<int, GameObject> invenSlotResult in invenSlot)
                            //{

                            //    if (invenSlotResult.Value == null)
                            //    {
                            //        Debug.Log($"인벤 슬롯 정보 조회 - 키: {invenSlotResult.Key} 값: null");
                            //    }
                            //    else
                            //    {
                            //        Debug.Log($"인벤 슬롯 정보 조회 - 키: {invenSlotResult.Key}  값: {invenSlotResult.Value}");
                            //    }
                            //}

                            //gearResult = null;
                        }
                        else
                        {
                            Debug.Log("목적지 장비 슬롯이 아닌 곳에 드래그한 경우");
                            //원래대로 이동
                            selectItem.transform.SetParent(beforeSelectItemParent.transform);
                            selectItem.transform.SetSiblingIndex(0);
                            //GameObject result = beforeSelectItemParent.GetChild(0).transform;
                            RectTransform rt = (RectTransform)beforeSelectItemParent.transform.GetChild(0).transform;
                            rt.anchoredPosition = new Vector3(0f, 0f);
                        }
                    }
                    else
                    {
                        Debug.Log("장비창 내 슬롯이 아닌 곳에 드래그한 경우");
                        //원래대로 이동
                        selectItem.transform.SetParent(beforeSelectItemParent.transform);
                        selectItem.transform.SetSiblingIndex(0);
                        //GameObject result = beforeSelectItemParent.GetChild(0).transform;
                        RectTransform rt = (RectTransform)beforeSelectItemParent.transform.GetChild(0).transform;
                        rt.anchoredPosition = new Vector3(0f, 0f);
                    }

                }
                else if (isInven == true)
                {
                    Debug.Log("인벤토리 슬롯 안 일 경우 ");

                    foreach (KeyValuePair<RectTransform, int> pair in invenSlotRect)
                    {
                        bool isItemSlot = RectTransformUtility.RectangleContainsScreenPoint(pair.Key, eventData.position);
                        if (isItemSlot == true)
                        {
                            Debug.Log($"마우스커서 위치가 어떤 인벤토리 슬롯 위에 있는지 조회: {pair.Key},  {pair.Value}");
                            isSlotRect = pair.Key;
                            isSlotIndex = pair.Value;
                        }
                    }

                    if (isSlotRect != null)
                    {
                        Debug.Log($"인벤토리 슬롯 위에 커서가 있을 경우 처음 선택한 아이템 위치랑 현재 위치한 아이템 위치 변경");
                        Debug.Log($"드래그한 아이템 정보: {beforeSelectItemParent.transform.name}");
                        Debug.Log($"목적지 슬롯 위치 정보: {isSlotRect.transform.name}");
         

                        GameObject result = null;
                        if (invenSlot.TryGetValue(isSlotIndex, out result) == true)
                        {
                            if(result == null)
                            {
                                Debug.Log($"인벤토리 슬롯에 아이템이 없을 경우");

                                //장비 처리

                                // 처음으로 선택한 아이템이 목적지 아이템 위치로 이동
                                selectItem.transform.SetParent(isSlotRect.transform);
                                int nCount = isSlotRect.transform.childCount;
                                isSlotRect.transform.GetChild(nCount - 1).transform.SetSiblingIndex(0);
                                RectTransform rect = (RectTransform)isSlotRect.transform.GetChild(0).transform;
                                rect.anchoredPosition = new Vector3(0f, 0f);

                                //인벤토리 슬롯 값 변경
                                invenSlot[isSlotIndex] = selectItem;
                                Debug.Log("invenSlot[isSlotIndex].GetComponent<InvenItem>().ItemName: " + invenSlot[isSlotIndex].GetComponent<InvenItem>().ItemName);

                                //인벤 기존값 조회 후 제거
                                foreach (KeyValuePair<RectTransform, int> pair in invenSlotRect)
                                {
                                    if(beforeSelectItemParent.transform == pair.Key)
                                    {
                                        Debug.Log($"드래그한 아이템, 기존값 일치할 경우: {pair.Key},  {pair.Value}");
                                        invenSlot[pair.Value] = null;
                                    }
                                }

                                // 장비 슬롯 기존값 조회 후 제거
                                foreach (KeyValuePair<RectTransform, int> gearSlotResult in gearSlotRect)
                                {
                                    if (beforeSelectItemParent.transform == gearSlotResult.Key) { 
                                        Debug.Log($"장비 슬롯 기존값 조회 후 제거 - 키: {gearSlotResult.Key} 값: {gearSlotResult.Value}");
                                        gearSlot[gearSlotResult.Value] = null;
                                    }
                                }
                            }
                            else
                            {
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
                                foreach (KeyValuePair<RectTransform, int> pair in invenSlotRect)
                                {
                                    if (beforeSelectItemParent.transform == pair.Key)
                                    {
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
                    }
                    else
                    {
                        Debug.Log("인벤토리 슬롯 내 슬롯이 아닌 곳에 드래그한 경우");
                        selectItem.transform.SetParent(beforeSelectItemParent.transform);
                        selectItem.transform.SetSiblingIndex(0);
                        RectTransform rt = (RectTransform)beforeSelectItemParent.transform.GetChild(0).transform;
                        rt.anchoredPosition = new Vector3(0f, 0f);
                    }
                }
                else
                {
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

    void OnDestroy()
    {
        Debug.Log("Inventory - OnDestroy");

        // 인벤토리 정보, 장비 정보 데이터 베이스에 전송
        networkManager.InventoryUpdate(invenSlot);
        networkManager.GearUpdate(gearSlot);
    }
}