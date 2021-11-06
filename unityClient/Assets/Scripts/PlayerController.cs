using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


// 오브젝트에 물리를 적용하기 위해선 리지드 바디를 적용해줘야함
public class PlayerController : MonoBehaviour {
    [SerializeField]
    float _speed = 10.0f;

    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Wall) | (1 << (int)Define.Layer.Store) | (1 << (int)Define.Layer.Training_school) | (1 << (int)Define.Layer.Clinic) | (1 << (int)Define.Layer.Dungeon);

    [SerializeField]
    protected Vector3 _destPos;// 이동 목적지

    [SerializeField]
    protected GameObject _lockTarget;// 마우스로 찍은 대상

    [SerializeField]
    protected Define.State _state = Define.State.Idle;

    bool _stopSkill = false;
    bool isInven = false;
    NetworkManager _network;
    GameObject characterInfo;
    Object _lock = new Object();
    Animator anim;

    public virtual Define.State State {
        get { return _state; }
        set {
            _state = value;
            // Debug.Log($"PlayerController - _network.player[2] {_network.player[2]}");
            if(_network.player[2].Equals("남자 캐릭터") == true) {
                GameObject gender = GameObject.Find("Player_Male(Clone)");
                if(gender != null) {
                    anim = gender.transform.GetComponent<Animator>();
                }
            } else {
                GameObject gender = GameObject.Find("Player_Female(Clone)");
                //anim = gender.transform.GetComponent<Animator>();
                if (gender != null) {
                    anim = gender.transform.GetComponent<Animator>();
                }
            }
            
            switch (_state) {
                case Define.State.Die:
                    break;
                case Define.State.Idle:
                    anim.CrossFade("Idle", 0.1f);
                    break;
                case Define.State.Moving:
                    anim.CrossFade("Run", 0.1f);
                    break;
                case Define.State.Skill:
                    anim.CrossFade("Attack", 0.1f, -1, 0);
                    break;
            }
        }
    }

    void Start() {
        // Debug.Log($"PlayerController - Start");

        //구독 신청: InputManager 한테 어떤 키보드 이벤트가 실행되면 OnKeyBoard 함수를 실행해주세요 하고 요청
        // -=: 실수로 다른 부분에서 구독 신청하면 두번 호출되는데 이 현상 방지 목적
        Managers.Input.KeyAction -= OnKeyBoard;
        Managers.Input.KeyAction += OnKeyBoard;

        // 2. 마우스 이벤트 OnMouseEvent 함수에 전달
        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;

        //NetworkManager 검색 후 객체 생성
        GameObject net = GameObject.Find("@NetworkManager");
        _network = net.GetComponent<NetworkManager>();

        if (_network.playerInfo == null) {
            // Debug.Log("_network.playerInfo null 일 경우");
            Player player = new Player();
            player.Id = _network.player[1];
            player.Gender = _network.player[2];
            player.State = _state.ToString();
            player.x = transform.position.x;
            player.y = transform.position.y;
            player.z = transform.position.z;

            // 플레이어 정보 서버에 전송
            _network.UserAdd(player);
        } else {
            Debug.Log("_network.playerInfo null 이 아닐 경우");
        }
    }

    // FixedUpdate 함수는 고정프레임마다 실행
    void FixedUpdate() {
        // 플레이어 상태에 따라 분기가 나뉘어짐
        switch (State) {
            case Define.State.Die:
                UpdateDie();
                break;
            case Define.State.Moving:
                UpdateMoving();

                lock (_lock) {
                    Player player = new Player();
                    player.Id = _network.player[1];
                    player.Gender = _network.player[2];
                    player.State = _state.ToString();
                    player.DestPos_x = _destPos.x;
                    player.DestPos_y = _destPos.y;
                    player.DestPos_z = _destPos.z;
                    player.x = transform.position.x;
                    player.y = transform.position.y;
                    player.z = transform.position.z;

                    // 서버에 캐릭터 이동 정보 전송
                    //Dictionary<int, Player> move = new Dictionary<int, Player>();
                    //move.Add(4, player);
                    _network.MoveSend(player);
                }
                break;
            case Define.State.Idle:
                UpdateIdle();
                break;
            case Define.State.Skill:
                UpdateSkill();
                break;
        }

        // I키를 누를 경우 인벤토리창 실행
        if (Input.GetKey(KeyCode.I)) {
            if (isInven == false) {
                isInven = true;
                StartCoroutine(InfoStart());
            }
        }
    }

    // 참고: https://hyunity3d.tistory.com/380
    private IEnumerator InfoStart() {
        if (characterInfo == null) {
            characterInfo = Managers.Resource.Instantiate("CharacterInfo");
            yield return new WaitForSeconds(0.2f);
            isInven = false;
        } else {
            Managers.Resource.Destroy(characterInfo);
            characterInfo = null;
            yield return new WaitForSeconds(0.2f);
            isInven = false;
        }
    }

    /*
    //+- delta, 특정 축으로 얼마만큼 회전
    transform.Rotate(new Vector3(0.0f, Time.deltaTime * 100.0f, 0.0f));

    //x,y,z 축(벡터3)으로만 만들면 짐벌록 문제가 일어남(회전 먹통 문제) -> 해결방법: Quaternion 사용
    transform.rotation = Quaternion.Euler(new Vector3(0.0f, Time.deltaTime * 100.0f, 0.0f));

    키보드 입력을 받아서 상,하,좌,우 움직이게 해주는 코드
    transform.position += new Vector3(0.0f, 0.0f, 1.0f)
    이슈: 플레이어가 빠르게 움직여짐 왜? Update()문이 한 프레임당 한번씩 호출되기 때문 그래서 이전 프레임과 지금 프레임의 시간 차이를 구해서 동작하게 해야함 (Time.deltaTime 사용)
    transform.position += new Vector3(0.0f, 0.0f, 1.0f) * Time.deltaTime
    이슈: 느리게 이동해지는 문제 발생 -> 해결방법: 다른 상수를 넣어줘야함(속도) = (시간 * 속도 = 거리)
    ransform.position += transform.TransformDirection(Vector3.forward * Time.deltaTime * _speed); // transform.TransformDirection() = 로컬좌표 -> 월드좌표로 변환 
    이슈: 원하는 방향으로 회전을 안함
    transform.rotation = Quaternion.LookRotation(Vector3.forward);
    이슈: 원하는 방향으로 볼수는 있으나 끊기는 느낌으로 회전함
    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward), 0.2f);
    transform.Translate(Vector3.forward * Time.deltaTime * _speed);
    이슈: 원하는 방향으로 보면서 이동은 하는데 좀 커브형으로 이동 및 튀는 현상 발생 -> 왜? Slerp() 눌러도 바로 이동하는게 아니라 쳐다보고 이동
    */
    void OnKeyBoard() {
        if (Input.GetKey(KeyCode.W)) {
            // 바라보는 방향과 상관없이 월드좌표 기준으로 이동
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward), 0.2f);
            transform.position += Vector3.forward * Time.deltaTime * _speed; // 예약어를 사용해서 처리
        }
        if (Input.GetKey(KeyCode.S)) {
            // transform.position -= new Vector3(0.0f, 0.0f, 1.0f) * Time.deltaTime * _speed;
            // transform.Translate(Vector3.back * Time.deltaTime * _speed);

            //원하는 방향으로 보면서 이동, null 이 아니라면 실행. 참고: https://answers.unity.com/questions/1015411/missingreferenceexception-the-object-of-type-text.html
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.back), 0.2f);
            transform.position += Vector3.back * Time.deltaTime * _speed;
        }
        if (Input.GetKey(KeyCode.A)) {
            // transform.position -= new Vector3(1.0f, 0.0f, 0.0f) * Time.deltaTime * _speed;
            // transform.Translate(Vector3.left * Time.deltaTime * _speed);

            //원하는 방향으로 보면서 이동
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.left), 0.2f);
            transform.position += Vector3.left * Time.deltaTime * _speed;
        }
        if (Input.GetKey(KeyCode.D)) {
            // transform.position += new Vector3(1.0f, 0.0f, 0.0f) * Time.deltaTime * _speed;
            // transform.Translate(Vector3.right * Time.deltaTime * _speed);

            //원하는 방향으로 보면서 이동
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right), 0.2f);
            transform.position += Vector3.right * Time.deltaTime * _speed;
        }
    }

    // 3. 마우스 이벤트 처리
    void OnMouseEvent(Define.MouseEvent evt) {
        // Debug.Log($"OnMouseEvent - State: {State}");
        switch (State) {
            case Define.State.Idle:
                OnMouseEvent_IdleRun(evt);
                break;
            case Define.State.Moving:
                OnMouseEvent_IdleRun(evt);
                break;
            case Define.State.Skill:
                {
                    // 한번이라도 떼면 다음 스킬을 안나가게 멈춰줌
                    if (evt == Define.MouseEvent.PointerUp)
                    {
                        _stopSkill = true;
                    }
                }
                break;
        }
    }

    void UpdateDie()
    {
        // 죽으면 아무것도 못함
    }

    // 4. 마우스 이벤트 처리
    void UpdateMoving() {
        // 몬스터가 내 사정거리보다 가까우면 공격
        if (_lockTarget != null) {
            _destPos = _lockTarget.transform.position; // 몬스터와 플레이어 간의 거리 값
            float distance = (_destPos - transform.position).magnitude;

            //Debug.Log($"UpdateMoving - _lockTarget.transform: " + _lockTarget.transform.name);
            // 플레이어와 상점 간의 거리가 가까우면 정지
            if (_lockTarget.transform.name.Equals("store") == true && distance <= 3) {
                Debug.Log($"PlayerController_상점 클릭시 실행 - {_lockTarget.transform.name}");
                Managers.Resource.Instantiate("ShopUI");
                isInven = true;
                StartCoroutine(InfoStart());
                State = Define.State.Idle;
                return;
            }

            // 플레이어와 의료소 간의 거리가 가까우면 정지
            if (_lockTarget.transform.name.Equals("clinic") == true && distance <= 3) {
                Debug.Log($"PlayerController_의료소 클릭시 실행 - {_lockTarget.transform.name}");

                State = Define.State.Idle;
                return;
            }

            if (_lockTarget.transform.name.Equals("training_school") == true && distance <= 3) {
                Debug.Log($"1. PlayerController_훈련소 클릭시 실행 - {_lockTarget.transform.name}");

                // 2.만들어둔 훈련소 화면을 호출해서 화면에 출력한다.
                Managers.Resource.Instantiate("Training_school");
                State = Define.State.Idle;
                return;
            }

            if (_lockTarget.transform.name.Equals("dungeon") == true && distance <= 3) {
                Debug.Log($"던전 클릭시 실행 - {_lockTarget.transform.name}");

                // 던전 들어가기 전 정보 수정할 부분 : 캐릭터 위치, 타 플레이어 에게 안보이게 설정

                // 전투 화면으로 이동
                SceneManager.LoadScene("BattleScene");
                State = Define.State.Idle;
                return;
            }
        }

        // 플레이어가 이동해야할 위치 방향 벡터값 (목적지 - 플레이어 위치)
        Vector3 dir = _destPos - transform.position;
        dir.y = 0;

        // 목적지에 도달하면 멈춤
        //0.0001f 벡터 계산은 오차 범위가 있어서 0이 아닌 숫자로 설정
        if (dir.magnitude < 0.1f) {
            State = Define.State.Idle;
        } else {
            // 이동 강의 참고, 아니면 계속 이동, NavMeshAgent: 길찾기 지원 클래스
            // NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
            // float moveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude); // moveDist 이 함수를 안 넣으면 목적지에서 혼자 부들부들 거리는 현상 발생
            // nma.Move(dir.normalized * moveDist);  // CalculatePath: 갈수 있는 거리 계산, 실제 크기까지 포함한 방향 벡터 입력

            // dir.normalized : normalized를 붙여야 1 이내로 나옴
            Debug.DrawRay(transform.position, dir.normalized, Color.green);

            //Raycast 파라미터: 내위치, 목적지, 감지 거리, 레이아웃 이름
            // transform.position + Vector3.up *0.5f : 플레이어 배꼽 위치
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, LayerMask.GetMask("Block"))) {
                // 마우스를 누르고 있다면 안변하게 설정
                if (Input.GetMouseButton(0) == false) {
                    State = Define.State.Idle;
                }
                return;
            }

            // moveDist 이 함수를 안 넣으면 목적지에서 혼자 부들부들 거리는 현상 발생
            float moveDist = Mathf.Clamp(_speed * Time.deltaTime, 0, dir.magnitude);
            transform.position += dir.normalized * moveDist;// 방향 * 거리
            //transform.position += dir.normalized * moveDist;// 속도 * 시간
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
        }
    }

    void UpdateSkill() {
        Debug.Log("UpdateSkill");

        // 고개를 몬스터 쪽으로 돌려줌
        if (_lockTarget != null) {
            Vector3 dir = _lockTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }

    void UpdateIdle() {
        State = Define.State.Idle;
        //애니메이션 처리
        //Animator anim = GetComponent<Animator>();
        // anim.SetFloat("speed", 0);
    }

    void OnMouseEvent_IdleRun(Define.MouseEvent evt) {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Physics.Raycast(): Ray를 투사해 조건에 맞는 객체가 닿게 되면 true 값을 반환
        bool raycastHit = Physics.Raycast(ray, out hit, 100.0f, _mask);

        // PointerDown
        // Debug.Log("OnMouseEvent_IdleRun - MouseEvent 값: " + evt);
        // Debug.Log("OnMouseEvent_IdleRun - raycastHit 값: " + raycastHit);
        switch (evt) {
            // 마우스를 땐 상태에서 처음 클릭할 경우
            case Define.MouseEvent.PointerDown:
                {
                    if (raycastHit)
                    {
                        //목적지 지정
                        _destPos = hit.point;
                        State = Define.State.Moving;
                        _stopSkill = false;

                        //Debug.Log("State 값 : " + State);
                        //Debug.Log("hit.layer 값 : " + hit.collider.gameObject.layer);

                        // 타겟 클릭시 대상 지정
                        switch (hit.collider.gameObject.layer)
                        {
                            case (int)Define.Layer.Monster:
                                _lockTarget = hit.collider.gameObject;
                                break;
                            case (int)Define.Layer.Store:
                                _lockTarget = hit.collider.gameObject;
                                break;
                            case (int)Define.Layer.Dungeon:
                                _lockTarget = hit.collider.gameObject;
                                break;
                            case (int)Define.Layer.Clinic:
                                _lockTarget = hit.collider.gameObject;
                                break;
                            case (int)Define.Layer.Training_school:
                                _lockTarget = hit.collider.gameObject;
                                break;
                            default:
                                _lockTarget = null;
                                break;
                        }
                    }
                }
                break;
            // 마우스로 누르고 있는 상태
            case Define.MouseEvent.Press:
                {
                    //Debug.Log("OnMouseEvent_IdleRun Press");
                    if (_lockTarget != null && raycastHit)
                    {
                        _destPos = hit.point;
                    }
                }
                break;
            case Define.MouseEvent.PointerUp:
                _stopSkill = true;
                //Debug.Log("OnMouseEvent_IdleRun PointerUp");
                break;
        }
    }

    void OnDisable()
    {
        Debug.Log($"PlayerController - OnDisable");
    }
    
    void OnDestroy()
    {
        Debug.Log($"PlayerController - OnDestroy");
    }
}