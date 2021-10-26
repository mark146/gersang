using RTS;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Diagnostics;
using UnityEngineInternal;

namespace RTS
{
    public class BattleScene : MonoBehaviour
    {
        [SerializeField] Collider[] selections { get; set; }
        [SerializeField] Box box;
        private Vector3 startPos, dragPos;

        private Camera cam;
        private Ray ray;
        private List<Unit> unitsSelected;

        // 선택 영역 - 참고: https://www.youtube.com/watch?v=vsdIhyLKgjc
        [SerializeField]
        private RectTransform selectSquareImage;
        private Vector3 selectStart, selectEnd;

        //플레이어 유닛 목록
        public List<GameObject> unitList { get; set; }

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

        //참고: https://qastack.kr/gamedev/116455/how-to-properly-differentiate-single-clicks-and-double-click-in-unity3d-using-c
        float clickCount;
        float draTimeLimit = 1.0f;

        void Start()
        {
            unitsSelected = new List<Unit>();
            cam = Camera.main;

            // 선택 상자 비활성화
            selectSquareImage.gameObject.SetActive(false);


            //플레이어 유닛 정보 조회 후 생성
            unitList = new List<GameObject>();

            GameObject unit = Managers.Resource.Instantiate($"UnitList/Cube_1");
            Stat stat = unit.GetComponent<Stat>();
            stat.Job = "플레이어";
            unitList.Add(unit);

            unit = Managers.Resource.Instantiate($"UnitList/Cube_2");
            stat = unit.GetComponent<Stat>();
            stat.Job = "검투사";
            unitList.Add(unit);

            unit = Managers.Resource.Instantiate($"UnitList/Cube_3");
            stat = unit.GetComponent<Stat>();
            stat.Job = "마법사";
            Unit range = unit.GetComponent<Unit>();
            range.AttackRange = 8;
            unitList.Add(unit);

            // 유닛 정보 표시해주는 창 객체로 생성
            infoPanel = GameObject.Find("UnitInfoPanel");

            //몬스터 유닛 정보 조회 후 생성
            monsterList = new List<GameObject>();

            for (int i = 0; i < 6; i++)
            {
                unit = Managers.Resource.Instantiate($"UnitList/Monster_{i + 1}");
                monsterList.Add(unit);
            }
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.A) && unitsSelected.Count != 0)
            {
                Debug.Log("A키를 누를 경우");

                // 땅을 선택할 경우: 목적지로 이동 하면서 적이 있을 경우 적을 공격
                // 몬스터를 선택할 경우: 몬스터 선택
                RaycastHit hit;
                ray = cam.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out hit, 100f);
                Debug.Log($"선택된 유닛: {hit.transform.tag}");
            }

            if (Input.GetKey(KeyCode.H) && unitsSelected.Count != 0)
            {
                Debug.Log("H키를 누를 경우");
                foreach (var unit in unitsSelected)
                {
                    unit.HoldUnit();
                }
            }

            if (Input.GetKey(KeyCode.S) && unitsSelected.Count != 0)
            {
                Debug.Log("S키를 누를 경우");
                foreach (var unit in unitsSelected)
                {
                    unit.StopUnit();
                }
            }

            if (Input.GetKey(KeyCode.R) && unitsSelected.Count == 1 && rangeSkill == null)
            {
                Debug.Log("유닛을 선택한 상태에서 R키를 누를 경우");

                // 마나가 부족할 경우 실행 불가
                // 땅을 선택할 경우: 목적지로 이동 하면서 적이 있을 경우 적을 공격
                // 몬스터를 선택할 경우:  
                RaycastHit hit;
                ray = cam.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out hit, 100f);


                //스킬 범위 값 지정
                Vector3 boxSize = new Vector3(10, 10, 10);
                // OverlapBox 파라미터값: 구체의 반경, 상자의 절반 크기
                // Physics.OverlapBox: 모든 접촉한 콜라이더나 내부의 구체(sphere)와 함께 배열을 반환합니다.
                //transform.position에서 사이즈 (1,1)에 회전안한(0) 상자에 충돌한 콜라이더를 반환한다
                selections = Physics.OverlapBox(hit.point, boxSize * 0.5f, Quaternion.identity);
                foreach (var obj in selections)
                {
                    if (obj.gameObject.layer != 0)
                    {
                        Debug.Log($"스킬 범위 안에 존재하는 유닛: {obj.gameObject.name}");
                    }
                }

                //스킬범위 표시하는 프래팹 생성
                if (rangeSkill == null)
                {
                    rangeSkill = Managers.Resource.Instantiate("rangeSkill");
                    isRangeSkill = true;
                }
            }

        }


        private void LateUpdate()
        {

            if (isRangeSkill == true)
            {
                RaycastHit hit;
                ray = cam.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out hit, 100f);
                if (rangeSkill != null)
                {
                    rangeSkill.transform.position = new Vector3(hit.point.x, 1f, hit.point.z);
                }
            }

            //마우스 오른쪽 버튼을 클릭할 경우
            // [수정사항] 선택한 유닛이 죽고 다시 선택상자를 표시하려하면 에러 발생(버그 예상 위치)
            if (Input.GetMouseButtonDown(1) && unitsSelected.Count != 0)
            {
                Debug.Log("마우스 오른쪽 버튼을 클릭할 경우");
                Debug.Log($"선택된 유닛 수: {unitsSelected.Count}");

                RaycastHit hit;
                ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100f))
                {
                    foreach (var unit in unitsSelected)
                    {
                        // 선택된 유닛들 이동
                        unit.MoveUnit(hit.point);
                    }
                }
            }


            //마우스 버튼을 누른 경우
            if (Input.GetMouseButtonDown(0))
            {
                clickCount = 0;

                RaycastHit hit;
                ray = cam.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out hit, 100f);

                Debug.Log($"hit.name: {hit.transform.gameObject.name}");
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
                } else
                {
                    selectEnd = startPos;
                    Vector3 squareStart = Camera.main.WorldToScreenPoint(selectStart);
                    squareStart.z = 0f;
                    Vector3 selectcentre = squareStart;
                    selectSquareImage.position = selectcentre;

                    float sizeX = Mathf.Abs(squareStart.x);
                    float sizeY = Mathf.Abs(squareStart.y);

                    selectSquareImage.sizeDelta = new Vector2(sizeX, sizeY);
                }
            }

            //마우스 버튼을 눌렀다 때는 경우
            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log($"마우스 버튼을 눌렀다 때는 경우 - clickCount: {clickCount}");

                //범위 스킬을 사용하지 않을 경우
                // 캐릭터를 선택할 경우
                if (isRangeSkill == false || unitsSelected.Count >= 1)
                {
                    // 기존에 이미 컨트롤 중인 유닛 선택 해제
                    foreach (var unit in unitsSelected)
                    {
                        unit.Selected(false);
                        UnitInfo(unit);
                    }
                    unitsSelected.Clear();
                    selectSquareImage.gameObject.SetActive(false);
                }

                // 1. 드래그 앤 드롭으로 선택한 유닛 추가
                RaycastHit hit;
                ray = cam.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out hit, 100f);

                selections = Physics.OverlapBox(box.Center, box.Extents, Quaternion.identity);
                foreach (var obj in selections)
                {
                    //Debug.Log($"layer: {obj.gameObject.layer}");
                    if (obj.gameObject.layer == 8)
                    {
                        //Debug.Log($"선택한 유닛 활성화: {obj.transform.name}");
                        // Debug.Log(obj + " looped");
                        Unit unit = obj.GetComponent<Unit>();
                        unit.Selected(true);
                        unitsSelected.Add(unit);
                        UnitInfo(true, obj);
                    }
                }



                // 범위 스킬
                if (rangeSkill != null && isRangeSkill == true)
                {
                    Debug.Log($"범위 스킬을 클릭할 경우 - isRangeSkill: {isRangeSkill}");
                    // 범위 밖일 경우 -> 범위 안으로 이동 후 스킬 시전
                    // 범위 안일 경우 -> 바로 스킬 실행

                    RaycastHit skillHit;
                    ray = cam.ScreenPointToRay(Input.mousePosition);
                    Physics.Raycast(ray, out skillHit, 100f);

                    explosion = Managers.Resource.Instantiate("Skill/Explosion");
                    explosion.transform.position = skillHit.point;
                    Managers.Resource.Destroy(rangeSkill);
                    isRangeSkill = false;
                    Invoke("DestroyObject", 0.5f);
                }
            }


            if (unitList.Count == 0)
            {
                Debug.Log($"전투 종료 - 플레이어 유닛이 전부 죽을 경우");
            }

            if (monsterList.Count == 0)
            {
                Debug.Log($"전투 종료 - 몬스터 유닛이 전부 죽을 경우");
            }
        }

        // 선택한 유닛 정보 생성
        void UnitInfo(bool isSelected, Collider obj)
        {
            //Debug.Log($"obj.GetComponent<Stat>().Job: {obj.GetComponent<Stat>().Job}");
            switch (obj.GetComponent<Stat>().Job)
            {
                case "검투사":
                    if (warriorInfo == null)
                    {
                        warriorInfo = Managers.Resource.Instantiate("UnitInfo/WarriorInfo");
                        warriorInfo.transform.SetParent(infoPanel.transform);
                        warriorInfo.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);//이미지 스케일 조정
                    }
                    break;
                case "마법사":
                    if (healerInfo == null)
                    {
                        healerInfo = Managers.Resource.Instantiate("UnitInfo/HealerInfo");
                        healerInfo.transform.SetParent(infoPanel.transform);
                        healerInfo.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);//이미지 스케일 조정
                    }
                    break;
                case "플레이어":
                    if (playerInfo == null)
                    {
                        playerInfo = Managers.Resource.Instantiate("UnitInfo/PlayerInfo");
                        playerInfo.transform.SetParent(infoPanel.transform);
                        playerInfo.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);//이미지 스케일 조정
                    }
                    break;
                default:
                    break;
            }
        }

        // 선택한 유닛 정보 제거
        void UnitInfo(Unit unit)
        {
            Debug.Log($"unit: {unit.GetComponent<Stat>().Job}");
            switch (unit.GetComponent<Stat>().Job)
            {
                case "검투사":
                    if (warriorInfo != null)
                    {
                        Managers.Resource.Destroy(warriorInfo);
                    }
                    break;
                case "마법사":
                    if (healerInfo != null)
                    {
                        Managers.Resource.Destroy(healerInfo);
                    }
                    break;
                case "플레이어":
                    if (playerInfo != null)
                    {
                        Managers.Resource.Destroy(playerInfo);
                    }
                    break;
                default:
                    break;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(box.Center, box.Size);
        }

        private void DestroyObject()
        {

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