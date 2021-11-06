using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RTS   
{
    public class BattleScene : MonoBehaviour {
        [SerializeField] public Collider[] selections { get; set; }
        [SerializeField] Box box = null;
        private Vector3 startPos, dragPos;

        private Camera cam;
        private Ray ray;
        public List<Unit> unitsSelected { get; set; }

        // 선택 영역 - 참고: https://www.youtube.com/watch?v=vsdIhyLKgjc
        [SerializeField]
        private RectTransform selectSquareImage;
        private Vector3 selectStart, selectEnd;

        //플레이어 유닛 목록
        public List<GameObject> battleunitList = new List<GameObject>();

        //몬스터 유닛 목록
        public List<GameObject> monsterList { get; set; }
       
        //스킬 사용 유무
        bool isRangeSkill = false;
        GameObject rangeSkill;
        GameObject explosion;

        // 유닛 정보 표시 객체
        GameObject infoPanel;
        GameObject healerInfo;
        GameObject playerInfo;
        GameObject warriorInfo;


        //유닛별 스킬 정보 표시 객체
        GameObject menuPanel;
        GameObject playerSkill_Image;
        GameObject playerSkill_Text;
        GameObject healerSkill;
        GameObject warriorSkill;
        
        NetworkManager network;
        GameObject unit;

        GameObject battleChat;

        //참고: https://qastack.kr/gamedev/116455/how-to-properly-differentiate-single-clicks-and-double-click-in-unity3d-using-c
        float clickCount;
        float draTimeLimit = 1.0f;

        // 던전에 입장할 경우  어떤 데이터가 필요한가? 플레이어 정보, 용병 정보
        void Start() {
            unitsSelected = new List<Unit>();
            cam = Camera.main;
            battleChat = GameObject.Find("BattleChat");

            //NetworkManager 검색 후 객체 생성
            GameObject net = GameObject.Find("@NetworkManager");
            network = net.GetComponent<NetworkManager>();

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
                chat.transform.SetParent(battleChat.transform.GetChild(0).GetChild(0).GetChild(0).transform);

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


            // 선택 상자 비활성화
            selectSquareImage.gameObject.SetActive(false);

            /** 
            [player 정보] - 캐릭터 정보 연동
            UserID - 번호: 0 , 값: test
            Name - 번호: 1 , 값: test1
            Gender - 번호: 2 , 값: 남자 캐릭터
            Lv - 번호: 3 , 값: 1, MaxLv - 번호: 4 , 값: 250, 
            Money - 번호: 5 , 값: 10000
            HP - 번호: 6 , 값: 200, MaxHP - 번호: 7 , 값: 200
            MP - 번호: 8 , 값: 100, MaxMP - 번호: 9 , 값: 100
            EXP - 번호: 10 , 값: 0, MaxEXP - 번호: 11 , 값: 100
            Attack - 번호: 12 , 값: 10, Defense - 번호: 13 , 값: 5
            STR - [i: 14, 값: 10], DEX - [i: 15 , 값: 10], CON - [번호: 16, 값: 10], WIS - [번호: 17, 값: 10]
            
            //for (int i =0; i < network.player.Count; i++) {
            //    Debug.Log($"i: {i}, network.player: {network.player[i]}");
            //}

            //플레이어 유닛 정보 조회 후 생성
            // unitList = new List<GameObject>();
            */
            // GameObject unit = null;
            switch (network.player[2])
            {
                case "남자 캐릭터":
                    unit = Managers.Resource.Instantiate($"UnitList/Battle_Player_Male");
                    break;
                default:
                    unit = Managers.Resource.Instantiate($"UnitList/Battle_Player_Female");
                    break;
            }

            Stat stat = unit.GetComponent<Stat>();
            stat.Job = "플레이어";
            stat.Hp =  int.Parse(network.player[6]);
            stat.MaxHp = int.Parse(network.player[7]);
            stat.Mp = int.Parse(network.player[8]);
            stat.MaxMp = int.Parse(network.player[9]);
            stat.Attack = int.Parse(network.player[12]);
            stat.Defense = int.Parse(network.player[13]);
            battleunitList.Add(unit);


            /** 
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
            */
            switch (network.unitList.Count) {
                case 16:// 용병이 한 명일 경우
                    /**
                    [용병 정보]
                    Name: 0 - 마법사
                    Job: 1 - 마법사
                    Lv : 2 - 1, MaxLv: 3 - 250, 
                    HP: 4 - 200, MaxHP: 5 - 200
                    MP : 6 - 200, MaxMP: 7 - 100
                    EXP: 8 - 0, MaxEXP: 9 - 100
                    Attack:10 - 10, Defense: 11 - 5
                    STR: 12 - 10, DEX: 13 - 10, CON: 14 - 10, WIS: 15 - 10
                     */
                    if(network.unitList[1].Equals("마법사") == true) {
                        unit = Managers.Resource.Instantiate($"UnitList/Cube_3");
                        stat = unit.GetComponent<Stat>();
                        stat.Job = "마법사";
                        Unit range = unit.GetComponent<Unit>();
                        range.AttackRange = 8;

                    } else {
                        unit = Managers.Resource.Instantiate($"UnitList/Cube_2");
                        stat = unit.GetComponent<Stat>();
                        stat.Job = "검투사";
                    }

                    stat.Hp = int.Parse(network.unitList[4]);
                    stat.MaxHp = int.Parse(network.unitList[5]);
                    stat.Mp = int.Parse(network.unitList[6]);
                    stat.MaxMp = int.Parse(network.unitList[7]);
                    stat.Attack = int.Parse(network.unitList[10]);
                    stat.Defense = int.Parse(network.unitList[11]);
                    battleunitList.Add(unit);

                    break;
                case 17:// 용병이 한 명일 경우
                    if (network.unitList[2].Equals("마법사") == true) {
                        unit = Managers.Resource.Instantiate($"UnitList/Cube_3");
                        stat = unit.GetComponent<Stat>();
                        stat.Job = "마법사";
                        Unit range = unit.GetComponent<Unit>();
                        range.AttackRange = 8;
                    } else {
                        unit = Managers.Resource.Instantiate($"UnitList/Cube_2");
                        stat = unit.GetComponent<Stat>();
                        stat.Job = "검투사";
                    }

                    stat.Hp = int.Parse(network.unitList[5]);
                    stat.MaxHp = int.Parse(network.unitList[6]);
                    stat.Mp = int.Parse(network.unitList[7]);
                    stat.MaxMp = int.Parse(network.unitList[8]);
                    stat.Attack = int.Parse(network.unitList[11]);
                    stat.Defense = int.Parse(network.unitList[12]);
                    battleunitList.Add(unit);
                    break;
                case 32:// 용병이 두 명일 경우
                    if (network.unitList[1].Equals("마법사") == true) {
                        unit = Managers.Resource.Instantiate($"UnitList/Cube_3");
                        stat = unit.GetComponent<Stat>();
                        stat.Job = "마법사";
                        Unit range = unit.GetComponent<Unit>();
                        range.AttackRange = 8;
                    } else {
                        unit = Managers.Resource.Instantiate($"UnitList/Cube_2");
                        stat = unit.GetComponent<Stat>();
                        stat.Job = "검투사";
                    }

                    stat.Hp = int.Parse(network.unitList[4]);
                    stat.MaxHp = int.Parse(network.unitList[5]);
                    stat.Mp = int.Parse(network.unitList[6]);
                    stat.MaxMp = int.Parse(network.unitList[7]);
                    stat.Attack = int.Parse(network.unitList[10]);
                    stat.Defense = int.Parse(network.unitList[11]);
                    battleunitList.Add(unit);

                    /*
                    [용병 정보]
                    Name: 16 - 마법사
                    Job: 17 - 마법사
                    Lv : 18 - 1, MaxLv: 19 - 250, 
                    HP: 20 - 200, MaxHP: 21 - 200
                    MP : 22 - 200, MaxMP: 23 - 100
                    EXP: 24 - 0, MaxEXP: 25 - 100
                    Attack:26 - 10, Defense: 27 - 5
                    STR: 28 - 10, DEX: 29 - 10, CON: 30 - 10, WIS: 31 - 10
                    */

                    if (network.unitList[17].Equals("마법사") == true) {
                        unit = Managers.Resource.Instantiate($"UnitList/Cube_3");
                        stat = unit.GetComponent<Stat>();
                        stat.Job = "마법사";
                        Unit range = unit.GetComponent<Unit>();
                        range.AttackRange = 8;
                    } else {
                        unit = Managers.Resource.Instantiate($"UnitList/Cube_2");
                        stat = unit.GetComponent<Stat>();
                        stat.Job = "검투사";
                    }

                    stat.Hp = int.Parse(network.unitList[20]);
                    stat.MaxHp = int.Parse(network.unitList[21]);
                    stat.Mp = int.Parse(network.unitList[22]);
                    stat.MaxMp = int.Parse(network.unitList[23]);
                    stat.Attack = int.Parse(network.unitList[26]);
                    stat.Defense = int.Parse(network.unitList[27]);
                    battleunitList.Add(unit);
                    break;
                case 34:
                    if (network.unitList[2].Equals("마법사") == true) {
                        unit = Managers.Resource.Instantiate($"UnitList/Cube_3");
                        stat = unit.GetComponent<Stat>();
                        stat.Job = "마법사";
                        Unit range = unit.GetComponent<Unit>();
                        range.AttackRange = 8;
                    } else {
                        unit = Managers.Resource.Instantiate($"UnitList/Cube_2");
                        stat = unit.GetComponent<Stat>();
                        stat.Job = "검투사";
                    }

                    stat.Hp = int.Parse(network.unitList[5]);
                    stat.MaxHp = int.Parse(network.unitList[6]);
                    stat.Mp = int.Parse(network.unitList[7]);
                    stat.MaxMp = int.Parse(network.unitList[8]);
                    stat.Attack = int.Parse(network.unitList[11]);
                    stat.Defense = int.Parse(network.unitList[12]);
                    battleunitList.Add(unit);

                    /*
                    [용병 정보]
                    Name: 16 - 마법사
                    Job: 17 - 마법사
                    Lv : 18 - 1, MaxLv: 19 - 250, 
                    HP: 20 - 200, MaxHP: 21 - 200
                    MP : 22 - 200, MaxMP: 23 - 100
                    EXP: 24 - 0, MaxEXP: 25 - 100
                    Attack:26 - 10, Defense: 27 - 5
                    STR: 28 - 10, DEX: 29 - 10, CON: 30 - 10, WIS: 31 - 10
                    */

                    if (network.unitList[19].Equals("마법사") == true) {
                        unit = Managers.Resource.Instantiate($"UnitList/Cube_3");
                        stat = unit.GetComponent<Stat>();
                        stat.Job = "마법사";
                        Unit range = unit.GetComponent<Unit>();
                        range.AttackRange = 8;
                    } else {
                        unit = Managers.Resource.Instantiate($"UnitList/Cube_2");
                        stat = unit.GetComponent<Stat>();
                        stat.Job = "검투사";
                    }

                    stat.Hp = int.Parse(network.unitList[22]);
                    stat.MaxHp = int.Parse(network.unitList[23]);
                    stat.Mp = int.Parse(network.unitList[24]);
                    stat.MaxMp = int.Parse(network.unitList[25]);
                    stat.Attack = int.Parse(network.unitList[28]);
                    stat.Defense = int.Parse(network.unitList[29]);
                    battleunitList.Add(unit);
                    break;
                default:
                    Debug.Log($"용병 추가 기타 - network.unitList.Count: {network.unitList.Count}");
                    break;
            }

            // 유닛 정보 표시해주는 창 객체로 생성
            infoPanel = GameObject.Find("UnitInfoPanel");

            //몬스터 유닛 정보 조회 후 생성
            monsterList = new List<GameObject>();

            for (int i = 0; i < 6; i++) { 
                unit = Managers.Resource.Instantiate($"UnitList/Monster_{i + 1}");
                monsterList.Add(unit);
            }

            menuPanel = GameObject.Find("MenuPanel");

            // 룸 정보 최신화 시켜주는 함수 코루틴으로 실행 - 참고: https://solution94.tistory.com/7
            InvokeRepeating("roomUpdate", 0f, 0.25f);
        }

        void roomUpdate()
        {
            // 전투 정보 저장, 값 변경, 서버 전송
            for(int i =0; i< battleunitList.Count;i++)
            {
                Debug.Log($"battleunitList[{i}] "+
                    $"Job {battleunitList[i].GetComponent<Stat>().Job}, " +
                    $"Hp - {battleunitList[i].GetComponent<Stat>().Hp}, "+
                    $"Mp - {battleunitList[i].GetComponent<Stat>().Mp}");
            }

            // 플레이어 용병 정보 수정
            // network.unitList
            switch (network.unitList.Count)
            {
                case 0: // 플레이어 캐릭터에 용병이 없을 경우
                    Debug.Log("플레이어 캐릭터에 용병이 없을 경우");
                    break;
                case 1: // 플레이어 캐릭터에 용병이 없을 경우
                    Debug.Log("플레이어 캐릭터에 용병이 없을 경우");
                    break;
                case 16:
                    Debug.Log($"용병이 한 명 있을 경우: {network.unitList.Count}");
                    Debug.Log($"unitList[1]: {network.unitList[1]}, "+
                           $"HP[4]: {network.unitList[4]}, "+
                        $"maxHP[5]: {network.unitList[5]}" +
                           $"MP[6]: {network.unitList[6]}, " +
                        $"maxMP[7]: {network.unitList[7]}");
                    break;
                case 17:
                    Debug.Log($"용병이 한 명 있을 경우: {network.unitList.Count}");
                    Debug.Log($"unitList[2]: {network.unitList[2]}, " +
                           $"HP[5]: {network.unitList[5]}, " +
                        $"maxHP[6]: {network.unitList[6]}" +
                           $"MP[7]: {network.unitList[7]}, " +
                        $"maxMP[8]: {network.unitList[8]}");
                    break;
                case 32:
                    Debug.Log($"용병이 두 명 있을 경우: {network.unitList.Count} ");
                    Debug.Log($"unitList[1]: {network.unitList[1]}, " +
                           $"HP[4]: {network.unitList[4]}, " +
                        $"maxHP[5]: {network.unitList[5]}" +
                           $"MP[6]: {network.unitList[6]}, " +
                        $"maxMP[7]: {network.unitList[7]}");
                    Debug.Log($"unitList[17]: {network.unitList[17]}, " +
                         $"HP[20]: {network.unitList[20]}, " +
                         $"maxHP[21]: {network.unitList[21]}" +
                         $"MP[22]: {network.unitList[22]}, " +
                         $"maxMP[23]: {network.unitList[23]}");
                    break;
                case 34:
                    Debug.Log($"용병이 두 명 있을 경우: {network.unitList.Count} ");
                    Debug.Log($"unitList[2]: {network.unitList[2]}, " +
                           $"HP[5]: {network.unitList[5]}, " +
                        $"maxHP[6]: {network.unitList[6]}" +
                           $"MP[7]: {network.unitList[7]}, " +
                        $"maxMP[8]: {network.unitList[8]}");
                    Debug.Log($"unitList[19]: {network.unitList[19]}, " +
                         $"HP[22]: {network.unitList[22]}, " +
                         $"maxHP[23]: {network.unitList[23]}" +
                         $"MP[24]: {network.unitList[24]}, " +
                         $"maxMP[25]: {network.unitList[25]}");

                    break;
                default:
                    Debug.Log("network.unitList.Count(): " + network.unitList.Count);
                    break;
            }

            Debug.Log($"monsterList.Count - {monsterList.Count}");
        }


        // 스킬 컨트롤
        private void Update() {
            if (Input.GetKey(KeyCode.A) && unitsSelected.Count != 0) { 
                Debug.Log("A키를 누를 경우");

                // 땅을 선택할 경우: 목적지로 이동 하면서 적이 있을 경우 적을 공격
                // 몬스터를 선택할 경우: 몬스터 선택
                RaycastHit hit;
                ray = cam.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out hit, 100f);
                //Debug.Log($"선택된 유닛: {hit.transform.tag}");
            }

            if (Input.GetKey(KeyCode.H) && unitsSelected.Count != 0) { 
                Debug.Log("H키를 누를 경우");
                foreach (var unit in unitsSelected) {
                    unit.HoldUnit();
                }
            }

            if (Input.GetKey(KeyCode.S) && unitsSelected.Count != 0) {
                Debug.Log("S키를 누를 경우");
                foreach (var unit in unitsSelected) {
                    unit.StopUnit();
                }
            }

            if (Input.GetKey(KeyCode.R) && unitsSelected.Count == 1 && rangeSkill == null && unitsSelected[0].GetComponent<Stat>().Job.Equals("플레이어") == true
                && unitsSelected[0].GetComponent<Stat>().Mp >= 30) {
                Debug.Log("유닛을 선택한 상태에서 R키를 누를 경우");

                // 마나가 부족할 경우 실행 불가
                // 땅을 선택할 경우: 목적지로 이동 하면서 적이 있을 경우 적을 공격
                // 몬스터를 선택할 경우:  
                //Debug.Log($"플레이어 MP: {unitsSelected[0].GetComponent<Stat>().Mp}");
                //스킬범위 표시하는 프래팹 생성
                if (rangeSkill == null) {
                    rangeSkill = Managers.Resource.Instantiate("rangeSkill");
                    isRangeSkill = true;
                }
            }
        }

        // 유닛 컨트롤
        private void LateUpdate() {

            // 선택한 유닛 정보 표시해주는 기능
            for (int i =0; i < unitsSelected.Count; i++) {
                UnitInfo(unitsSelected[i].GetComponent<Stat>());
            }

            // 키코드 참고: https://docs.unity3d.com/kr/530/ScriptReference/KeyCode.html
            if(Input.GetKeyDown(KeyCode.Escape)) {
                // Debug.Log($"Input.inputString: {Input.inputString}");

                // 스킬 범위 표시 취소
                if(rangeSkill != null)
                {
                    Managers.Resource.Destroy(rangeSkill);
                    rangeSkill = null;
                    isRangeSkill = false;
                }
                
                // 메뉴 표시(예정)
            }
            

            // 스킬 범위 표시
            if (isRangeSkill == true) {
                RaycastHit hit;
                ray = cam.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out hit, 100f);
                if (rangeSkill != null) {
                    rangeSkill.transform.position = new Vector3(hit.point.x, 1f, hit.point.z);
                }
            }


            //마우스 오른쪽 버튼을 클릭할 경우
            // [수정사항] 선택한 유닛이 죽고 다시 선택상자를 표시하려하면 에러 발생(버그 예상 위치)
            if (Input.GetMouseButtonDown(1) && unitsSelected.Count != 0) {
                Debug.Log("마우스 오른쪽 버튼을 클릭할 경우");
                //Debug.Log($"선택된 유닛 수: {unitsSelected.Count}");

                RaycastHit hit;
                ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100f)) {
                    foreach (var unit in unitsSelected) {
                        // 선택된 유닛들 이동
                        unit.MoveUnit(hit.point);
                    }
                }
            }


            //마우스 버튼을 누른 경우
            if (Input.GetMouseButtonDown(0)) {
                clickCount = 0;

                RaycastHit hit;
                ray = cam.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out hit, 100f);

                // 땅을 찍을 경우 선택 초기화
                //Debug.Log($"hit.name: {hit.transform.gameObject.name}");

                //on drag start
                startPos = hit.point;
                box.baseMin = startPos;

                // 선택 사각형 시작
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
                {
                    selectStart = hit.point;
                }
            }

            //마우스 버튼을 누르는 중일 경우
            if (Input.GetMouseButton(0))
            {
                clickCount += Time.deltaTime;

                // 클릭하고 있는 시간이 0.3f 이상이면 선택 사각형 활성화 후 계산
                if (clickCount > 0.3f)
                {
                    if (!selectSquareImage.gameObject.activeInHierarchy)
                    {
                        selectSquareImage.gameObject.SetActive(true);
                    }

                    selectEnd = Input.mousePosition;
                    Vector3 squareStart = Camera.main.WorldToScreenPoint(selectStart);
                    squareStart.z = 0f;
                    Vector3 selectcentre = (squareStart + selectEnd) / 2f;
                    selectSquareImage.position = selectcentre;

                    float sizeX = Mathf.Abs(squareStart.x - selectEnd.x);
                    float sizeY = Mathf.Abs(squareStart.y - selectEnd.y);

                    selectSquareImage.sizeDelta = new Vector2(sizeX, sizeY);
                }
            }

            //마우스 버튼을 눌렀다 때는 경우
            if (Input.GetMouseButtonUp(0)) {
                //Debug.Log($"마우스 버튼을 눌렀다 때는 경우 - clickCount: {clickCount}, isRangeSkill: {isRangeSkill}");

                // 1. 드래그 앤 드롭으로 선택한 유닛 추가
                RaycastHit hit;
                ray = cam.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out hit, 100f);

                box.baseMax = hit.point;
                selections = Physics.OverlapBox(box.Center, box.Extents, Quaternion.identity);


                // 캐릭터를 선택 후 범위 스킬을 사용하지 않고 땅을 찍을 경우 유닛 선택 해제
                if (isRangeSkill == false && unitsSelected.Count >= 1 && clickCount > 0.4f || isRangeSkill == false && clickCount < 0.4) {
                    // 기존에 이미 컨트롤 중인 유닛 선택 해제
                    foreach (var unit in unitsSelected) {
                        unit.Selected(false);
                        UnitInfo(unit);
                    }
                    unitsSelected.Clear();
                    selectSquareImage.gameObject.SetActive(false);
                }

                foreach (var obj in selections) {
                    //Debug.Log($"layer: {obj.gameObject.layer}");
                    if (obj.gameObject.layer == 8) {
                        //Debug.Log($"선택한 유닛 활성화: {obj.transform.name}");
                        Unit unit = obj.GetComponent<Unit>();
                        unit.Selected(true);
                        unitsSelected.Add(unit);
                    }
                }


                // 범위 스킬
                if (rangeSkill != null && isRangeSkill == true) {
                    Debug.Log($"범위 스킬을 클릭할 경우 - isRangeSkill: {isRangeSkill}");
                    Debug.Log($"플레이어 Mp 소모전 : {unitsSelected[0].GetComponent<Stat>().Mp}");

                    // MP 감소
                    unitsSelected[0].GetComponent<Stat>().Mp = unitsSelected[0].GetComponent<Stat>().Mp - 30;
                    Debug.Log($"플레이어 Mp 소모후 : {unitsSelected[0].GetComponent<Stat>().Mp}");
                    // 범위 밖일 경우 -> 범위 안으로 이동 후 스킬 시전
                    // 범위 안일 경우 -> 바로 스킬 실행
                    // 마나가 부족할 경우 실행 불가
                    // 땅을 선택할 경우: 목적지로 이동 하면서 적이 있을 경우 적을 공격
                    // 몬스터를 선택할 경우:  
                    RaycastHit skillHit;
                    ray = cam.ScreenPointToRay(Input.mousePosition);
                    Physics.Raycast(ray, out skillHit, 100f);

                    //스킬 범위 값 지정
                    Vector3 boxSize = new Vector3(9, 9, 9);
                    // OverlapBox 파라미터값: 구체의 반경, 상자의 절반 크기
                    // Physics.OverlapBox: 모든 접촉한 콜라이더나 내부의 구체(sphere)와 함께 배열을 반환합니다.
                    //transform.position에서 사이즈 (1,1)에 회전안한(0) 상자에 충돌한 콜라이더를 반환한다
                    Collider[] targetList = Physics.OverlapBox(skillHit.point, boxSize * 0.4f, Quaternion.identity);
                    foreach (var obj in targetList) {
                        if (obj.gameObject.layer != 0) {
                            //Debug.Log($"스킬 범위 안에 존재하는 유닛: {obj.gameObject.name}");

                            Stat targetStat = obj.gameObject.GetComponent<Stat>();

                            //체력 깎는 텍스트 표시
                            //참고: https://lesslate.github.io/unity/%EC%9C%A0%EB%8B%88%ED%8B%B0-%ED%94%8C%EB%A1%9C%ED%8C%85-%EB%8D%B0%EB%AF%B8%EC%A7%80-%ED%85%8D%EC%8A%A4%ED%8A%B8/
                            int damage = Mathf.Max(0, (unitsSelected[0].GetComponent<Stat>().Attack * 5) - targetStat.Defense);
                            DamageTextView(damage, obj.gameObject);
                            targetStat.OnSkillAttacked(damage);

                            // 몬스터 체력이 0 이하일 경우 제거
                            if (targetStat.Hp <= 0) {
                                Managers.Resource.Destroy(obj.gameObject);

                                //죽은 유닛 리스트에서 제거
                                monsterList.Remove(obj.gameObject);
                            }
                        }
                    }

                    explosion = Managers.Resource.Instantiate("Skill/Explosion");
                    explosion.transform.position = skillHit.point;
                    Managers.Resource.Destroy(rangeSkill);
                    isRangeSkill = false;
                    Invoke("DestroyObject", 0.5f);
                }
            }

            // 몬스터, 플레이어 유닛이 전부 죽을 경우 메인 화면으로 이동
            // 전투가 종료할 경우 데이터 처리 어떻게 할 것인가?
            if (battleunitList.Count == 0) {
                Debug.Log($"전투 종료 - 플레이어 유닛이 전부 죽을 경우");

                // 경험치

                SceneManager.LoadScene("MainScene");
            } else if (monsterList.Count == 0) { 
                Debug.Log($"전투 종료 - 몬스터 유닛이 전부 죽을 경우");

                // 경험치, 보상

                SceneManager.LoadScene("MainScene");
            }
        }

        //체력 깎는 텍스트 표시
        //참고: https://lesslate.github.io/unity/%EC%9C%A0%EB%8B%88%ED%8B%B0-%ED%94%8C%EB%A1%9C%ED%8C%85-%EB%8D%B0%EB%AF%B8%EC%A7%80-%ED%85%8D%EC%8A%A4%ED%8A%B8/
        public void DamageTextView(int damage, GameObject target) {
            if (damage <= 0) {
                damage = 0;
            }

            GameObject DamageText = Managers.Resource.Instantiate("DamageText");
            DamageText.transform.SetParent(target.transform);
            DamageText.transform.position = new Vector3(target.transform.position.x, 2.0f, target.transform.position.z); // 표시될 위치
            DamageText.GetComponent<TextMesh>().text = damage.ToString(); // 데미지 전달
            DamageText.GetComponent<TextMesh>().color = Color.yellow;
            Invoke("DestroyObject", 0.5f);
        }

        // 선택한 유닛 정보 생성
        void UnitInfo(Stat stat) {
            //Debug.Log($"obj.GetComponent<Stat>().Job: {obj.GetComponent<Stat>().Job}");
            switch (stat.Job) {
                case "검투사":
                    if (warriorInfo == null) {
                        warriorInfo = Managers.Resource.Instantiate("UnitInfo/WarriorInfo");
                        warriorInfo.transform.SetParent(infoPanel.transform);
                        warriorInfo.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);//이미지 스케일 조정
                    } else {
                        // 1 :체력, 2:마력 설정
                        float hpBar = stat.Hp / (float)stat.MaxHp;
                        warriorInfo.transform.GetChild(1).GetComponent<Slider>().value = hpBar;

                        float mpBar = stat.Mp / (float)stat.MaxMp;
                        warriorInfo.transform.GetChild(2).GetComponent<Slider>().value = mpBar;
                    }
                    break;
                case "마법사":
                    if (healerInfo == null) {
                        healerInfo = Managers.Resource.Instantiate("UnitInfo/HealerInfo");
                        healerInfo.transform.SetParent(infoPanel.transform);
                        healerInfo.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);//이미지 스케일 조정
                    } else {
                        // 1 :체력, 2:마력 설정
                        float hpBar = stat.Hp / (float)stat.MaxHp;
                        healerInfo.transform.GetChild(1).GetComponent<Slider>().value = hpBar;

                        float mpBar = stat.Mp / (float)stat.MaxMp;
                        healerInfo.transform.GetChild(2).GetComponent<Slider>().value = mpBar;
                    }
                    break;
                case "플레이어":
                    if (playerInfo == null) {
                        playerInfo = Managers.Resource.Instantiate("UnitInfo/PlayerInfo");
                        playerInfo.transform.SetParent(infoPanel.transform);
                        playerInfo.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);//이미지 스케일 조정

                        if(unitsSelected.Count == 1) {
                            playerSkill_Image = Managers.Resource.Instantiate("UnitInfo/Explosion_Image");
                            playerSkill_Image.transform.SetParent(menuPanel.transform.GetChild(0).GetChild(4));
                            playerSkill_Image.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);//이미지 스케일 조정
                            playerSkill_Image.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, 0.0f, 0.0f);

                            playerSkill_Text = Managers.Resource.Instantiate("UnitInfo/Explosion_Text");
                            playerSkill_Text.transform.SetParent(menuPanel.transform.GetChild(0).GetChild(4));
                            playerSkill_Text.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);//이미지 스케일 조정
                            playerSkill_Text.GetComponent<RectTransform>().anchoredPosition = new Vector3(-10.0f, 26.5f, 0.0f);
                        }
                    } else {
                        // 1 :체력, 2:마력 설정
                        float hpBar = stat.Hp / (float)stat.MaxHp;
                        playerInfo.transform.GetChild(1).GetComponent<Slider>().value = hpBar;

                        float mpBar = stat.Mp / (float)stat.MaxMp;
                        playerInfo.transform.GetChild(2).GetComponent<Slider>().value = mpBar;
                    }
                    break;
                default:
                    break;
            }
        }

        // 선택한 유닛 정보 제거
        public void UnitInfo(Unit unit) {
            //Debug.Log($"unit: {unit.GetComponent<Stat>().Job}");
            switch (unit.GetComponent<Stat>().Job) {
                case "검투사":
                    if (warriorInfo != null) {
                        Managers.Resource.Destroy(warriorInfo);
                    }
                    break;
                case "마법사":
                    if (healerInfo != null) {
                        Managers.Resource.Destroy(healerInfo);
                    }
                    break;
                case "플레이어":
                    if (playerInfo != null) {
                        Managers.Resource.Destroy(playerInfo);

                        if(playerSkill_Image != null) {
                            Managers.Resource.Destroy(playerSkill_Image);
                            Managers.Resource.Destroy(playerSkill_Text);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void OnDrawGizmos() {
            // 기즈모 색 변경
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(box.Center, box.Size);
        }

        private void DestroyObject() {
            Destroy(explosion.gameObject);
        }
    }

    [System.Serializable]
    public class Box
    {
        public Vector3 baseMin, baseMax;

        public Vector3 Center
        {
            get
            {
                Vector3 center = baseMin + (baseMax - baseMin) * 0.5f;
                center.y = (baseMax - baseMin).magnitude * 0.5f;
                return center;
            }
        }

        public Vector3 Size
        {
            get
            {
                //시작 위치와 종료 위치에 따라 음수가 생성 될 수 있다. 그래서 항상 양수로 만들기 위해 절대값을 사용
                return new Vector3(Mathf.Abs(baseMax.x - baseMin.x), (baseMax - baseMin).magnitude, Mathf.Abs(baseMax.z - baseMin.z));
            }
        }

        public Vector3 Extents
        {
            get
            {
                return Size * 0.5f;
            }
        }
    }
}