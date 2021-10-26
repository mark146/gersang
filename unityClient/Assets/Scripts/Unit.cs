using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RTS
{
    public class Unit : BaseController
    {
        [SerializeField]
        float _scanRange = 10;

        [SerializeField]
        float _attackRange = 3;

        [SerializeField]
        float timer;

        [SerializeField]
        float waitingTime;

        [SerializeField]
        float arrowTimer;

        float arrowWaitingTime;

        Stat _stat;

        GameObject bullet;

        GameObject DamageText;

        GameObject isSelect;

        
        public float AttackRange { get { return _attackRange; } set { _attackRange = value; } }

        public override void Init()
        {            
            _stat = gameObject.GetComponent<Stat>();

            //체력, 마력바가 없을 경우 생성
            if (transform != null)
            {
                //Debug.Log($"transform.name: {transform.name}");
                GameObject go = Managers.Resource.Instantiate($"UI_HPBar");
                go.transform.SetParent(this.transform);// 나에게 붙어서  hp바 생성

                go = Managers.Resource.Instantiate($"UI_MPBar");
                go.transform.SetParent(this.transform);// 나에게 붙어서  hp바 생성
            }

            // 유닛 선택 표시 오브젝트 객체화
            isSelect = transform.GetChild(transform.childCount - 3).gameObject;

            timer = 0.0f;   
            waitingTime = 0.5f;
        }

        // 유닛 선택시 밑에 원 활성화, 비활성화 하는 기능
        public void Selected(bool select)
        {
            isSelect.SetActive(select);
        }

        public void StopUnit()
        {
            Debug.Log("StopUnit 실행");

            _lockTarget = null;
            State = Define.State.Idle;
        }

        public void HoldUnit()
        {
            State = Define.State.Hold;
        }

        protected override void UpdateIdle()
        {
            // 참고: https://cru6548.tistory.com/5
            GameObject[] monster = GameObject.FindGameObjectsWithTag("Monster");
            if (monster != null)
            {
                for (int i = 0; i < monster.Length; i++)
                {
                    //Debug.Log($"UpdateIdle - player[{i}]: {player[i]}");
                    float distance = (monster[i].transform.position - transform.position).magnitude;
                    if (distance <= _scanRange)
                    {
                        _lockTarget = monster[i];
                        State = Define.State.Moving;
                        // return;
                    }
                }
            }
        }

        protected override void UpdateMoving()
        {
            // 플레이어가 내 사정거리보다 가까우면 공격
            if (_lockTarget != null)
            {
                // 몬스터와 플레이어 간의 거리 값
                _destPos = _lockTarget.transform.position;
                float distance = (_destPos - transform.position).magnitude;

                // 플레이어와 몬스터 간의 거리가 가까우면 스킬 시전
                if (distance <= _attackRange)
                {
                    //agent.SetDestination(transform.position);// 상대 목표 지정시 자동 이동
                    State = Define.State.Skill;
                    return;
                }
            }

            // 플레이어가 이동해야할 위치 방향 벡터값 (목적지 - 플레이어 위치)
            Vector3 dir = _destPos - transform.position;
            dir.y = 0.5f;

            // 목적지에 도달하면 멈춤
            if (dir.magnitude < 0.7f) //0.0001f 벡터 계산은 오차 범위가 있어서 0이 아닌 숫자로 설정
            {
                State = Define.State.Idle;
            }
            else
            {
                // 이동 강의 참고, 아니면 계속 이동, NavMeshAgent: 길찾기 지원 클래스
                // NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
                // float moveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude); // moveDist 이 함수를 안 넣으면 목적지에서 혼자 부들부들 거리는 현상 발생
                // nma.Move(dir.normalized * moveDist);  // CalculatePath: 갈수 있는 거리 계산, 실제 크기까지 포함한 방향 벡터 입력
                // dir.normalized : normalized를 붙여야 1 이내로 나옴
                // moveDist 이 함수를 안 넣으면 목적지에서 혼자 부들부들 거리는 현상 발생
                float moveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude);
                transform.position += dir.normalized * moveDist;// 방향 * 거리
                                                                //transform.position += dir.normalized * moveDist;// 속도 * 시간
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
            }
        }

        protected override void UpdateSkill()
        {
            // 고개를 몬스터 쪽으로 돌려줌
            if (_lockTarget != null)
            {
                Vector3 dir = _lockTarget.transform.position - transform.position;
                Quaternion quat = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
            }

            //공격 속도 딜레이 지정
            timer += Time.deltaTime;
            if (timer > waitingTime)
            {
                timer = 0.0f;
                OnHitEvent();
            }

            if (bullet != null)
            {
                rangeAttack();
            }
        }

        protected override void UpdateDie()
        {
            Debug.Log($"죽음: {transform.name}");
        }

        // 체력을 깎는 부분
        void OnHitEvent()
        {
            if (_lockTarget != null)
            {
                if (_stat.Job.Equals("마법사") == true && bullet == null)
                {
                    bullet = Managers.Resource.Instantiate($"Shoot");
                    bullet.transform.SetParent(this.transform);
                    Vector3 dir = _lockTarget.transform.position - bullet.transform.position;
                    dir.y = 0;

                    // moveDist 이 함수를 안 넣으면 목적지에서 혼자 부들부들 거리는 현상 발생
                    float moveDist = Mathf.Clamp(10 * Time.deltaTime, 0, dir.magnitude);
                    bullet.transform.position = transform.position + dir.normalized;
                }


                if (_stat.Job.Equals("검투사") == true || _stat.Job.Equals("플레이어") == true)
                {
                    Stat targetStat = _lockTarget.GetComponent<Stat>();

                    //체력 깎는 텍스트 표시
                    //참고: https://lesslate.github.io/unity/%EC%9C%A0%EB%8B%88%ED%8B%B0-%ED%94%8C%EB%A1%9C%ED%8C%85-%EB%8D%B0%EB%AF%B8%EC%A7%80-%ED%85%8D%EC%8A%A4%ED%8A%B8/
                    int damage = Mathf.Max(0, _stat.Attack - targetStat.Defense);
                    DamageTextView(damage);
                    targetStat.OnAttacked(_stat);//매개변수:  내스텟 

                    if (targetStat.Hp <= 0)
                    {
                        //Debug.Log("플레이어가 죽을 경우");
                        Managers.Resource.Destroy(targetStat.gameObject);

                        //죽은 유닛 리스트에서 제거
                        BattleScene scene = GameObject.Find("@BattleScene").GetComponent<BattleScene>();
                        scene.monsterList.Remove(_lockTarget);

                        _lockTarget = null;
                        State = Define.State.Idle;
                    }

                    // 상대 체력 체크
                    if (targetStat.Hp > 0)
                    {
                        float distance = (_lockTarget.transform.position - transform.position).magnitude; // 거리 체크
                        if (distance <= _attackRange)
                        {
                            State = Define.State.Skill;
                        }
                        else
                        {
                            State = Define.State.Moving;
                        }
                    }
                    else
                    {
                        State = Define.State.Idle;
                    }
                }

            }
            else
            {
                State = Define.State.Idle;
            }

        }

        protected override void UpdateHold()
        {
            Debug.Log("UpdateHold");
        
            // 주변에 몬스터가 존재하는지 조회
            GameObject[] monster = GameObject.FindGameObjectsWithTag("Monster");
            if (monster != null)
            {
                for (int i = 0; i < monster.Length; i++)
                {
                    // 몬스터가 내 인식거리에 있을 경우 타겟 지정
                    float distance = (monster[i].transform.position - transform.position).magnitude;
                    if (distance <= _scanRange)
                    {
                        _lockTarget = monster[i];

                            // 몬스터와 플레이어 간의 거리 값
                            _destPos = _lockTarget.transform.position;
                            distance = (_destPos - transform.position).magnitude;

                            // 플레이어와 몬스터 간의 거리가 가까우면 공격
                            if (distance <= _attackRange)
                            {
                                Debug.Log("플레이어와 몬스터 간의 거리가 가까울 경우 공격");
                                // 고개를 몬스터 쪽으로 돌려줌
                                if (_lockTarget != null)
                                {
                                    Vector3 dir = _lockTarget.transform.position - transform.position;
                                    Quaternion quat = Quaternion.LookRotation(dir);
                                    transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
                                }

                                if (_stat.Job.Equals("검투사") == true || _stat.Job.Equals("플레이어") == true)
                                {
                                    //공격 쿨타임 계산
                                    timer += Time.deltaTime;
                                    if (timer > waitingTime)
                                    {
                                        timer = 0.0f;
                                        Stat targetStat = _lockTarget.GetComponent<Stat>();
                                        //체력 깎는 텍스트 표시
                                        //참고: https://lesslate.github.io/unity/%EC%9C%A0%EB%8B%88%ED%8B%B0-%ED%94%8C%EB%A1%9C%ED%8C%85-%EB%8D%B0%EB%AF%B8%EC%A7%80-%ED%85%8D%EC%8A%A4%ED%8A%B8/
                                        int damage = Mathf.Max(0, _stat.Attack - targetStat.Defense);
                                        DamageTextView(damage);
                                        targetStat.OnAttacked(_stat);//매개변수:  내스텟 

                                        if (targetStat.Hp <= 0)
                                        {
                                            //Debug.Log("플레이어가 죽을 경우");
                                            GameObject.Destroy(targetStat.gameObject);
                                            //  Managers.Game.Despwan(targetStat.gameObject);

                                            //죽은 유닛 리스트에서 제거
                                            BattleScene scene = GameObject.Find("@BattleScene").GetComponent<BattleScene>();
                                            scene.monsterList.Remove(_lockTarget);

                                            _lockTarget = null;
                                        }

                                    }

                                }

                                if (_stat.Job.Equals("마법사") == true)
                                {
                                    Debug.Log("마법사일 경우");

                                //공격속도 지정
                                arrowTimer += Time.deltaTime;
                                    if (bullet == null && arrowTimer > 3.0f)
                                    {
                                        try {
                                        arrowTimer = 0.0f;
                                            bullet = Managers.Resource.Instantiate($"Shoot");
                                            bullet.transform.SetParent(this.transform);

                                            bullet.transform.position = transform.position;
                                        } catch(Exception e)
                                        {
                                            Managers.Resource.Destroy(bullet);
                                        }
                                    } else
                                    {
                                        rangeAttack();
                                    }

                                }
                            }
                 
                    } else
                    {
                        //Debug.Log("범위 안에 몬스터가 존재하지 않을 경우");

                        _lockTarget = null;

                        // 미사일이 발사 중일 경우 미사일 오브젝트 제거
                        if (bullet != null)
                        {
                            Managers.Resource.Destroy(bullet);
                            bullet = null;
                        }
                    }

                }
            }


            if (bullet != null && monster.Length == 0)
            {
                Managers.Resource.Destroy(bullet.gameObject);
            }
        }

        public void MoveUnit(Vector3 position)
        {
            //Debug.Log("MoveUnit 실행");
            _lockTarget = null;
            //목적지 지정
            //agent.SetDestination(position);
            _destPos = position;
            State = Define.State.Moving;
        }

        public void AttackUnit(Vector3 position)
        {
            Debug.Log("AttackUnit 실행");
            // 플레이어가 내 사정거리보다 가까우면 공격
            if (_lockTarget != null)
            {
                // 몬스터와 플레이어 간의 거리 값
                _destPos = _lockTarget.transform.position;
                float distance = (_destPos - transform.position).magnitude;

                // 플레이어와 몬스터 간의 거리가 가까우면 스킬 시전
                if (distance <= _attackRange)
                {
                    //agent.SetDestination(transform.position);// 상대 목표 지정시 자동 이동
                    State = Define.State.Skill;
                    return;
                }
            }

        }

        public void rangeAttack()
        {
            Debug.Log($"rangeAttack");

            if (_lockTarget == null)
            {
                Managers.Resource.Destroy(bullet.gameObject);
            }
            else
            {

                try {
                    // 미사일이 이동해야할 위치 방향 벡터값 (목적지 - 플레이어 위치)
                    float shootDir = (_lockTarget.transform.position - bullet.transform.position).magnitude;
                    Debug.Log("AttackRange: "+AttackRange);

                    // 목적지에 도달할 경우 미사일 제거
                    if (shootDir < 1.5f)
                    {
                        Managers.Resource.Destroy(bullet.gameObject);

                        Stat targetStat = _lockTarget.GetComponent<Stat>();
                        int damage = Mathf.Max(0, _stat.Attack - targetStat.Defense);
                        DamageTextView(damage);
                        targetStat.OnAttacked(_stat);

                        if (targetStat.Hp <= 0)
                        {
                            //Debug.Log("플레이어가 죽을 경우");
                            Managers.Resource.Destroy(targetStat.gameObject);

                            //죽은 유닛 리스트에서 제거
                            BattleScene scene = GameObject.Find("@BattleScene").GetComponent<BattleScene>();
                            scene.monsterList.Remove(_lockTarget);

                            _lockTarget = null;
                        }
                    }
                    else
                    {
                        //대상이 존재할 경우에만 미사일 이동
                        if (_lockTarget != null)
                        {

                            try
                            {
                                // 플레이어가 이동해야할 위치 방향 벡터값 (목적지 - 플레이어 위치)
                                Vector3 dir = _lockTarget.transform.position - bullet.transform.position;
                                dir.y = 0;

                                float moveDist = Mathf.Clamp(15 * Time.deltaTime, 0, dir.magnitude);
                                bullet.transform.position += dir.normalized * moveDist;// 방향 * 거리
                                bullet.transform.rotation = Quaternion.Slerp(bullet.transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
                            }
                            catch (Exception e)
                            {
                                Managers.Resource.Destroy(bullet.gameObject);
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    Debug.Log("대상이 존재하지 않음");
                    _lockTarget = null;
                }


            
            }
        }

        //체력 깎는 텍스트 표시
        //참고: https://lesslate.github.io/unity/%EC%9C%A0%EB%8B%88%ED%8B%B0-%ED%94%8C%EB%A1%9C%ED%8C%85-%EB%8D%B0%EB%AF%B8%EC%A7%80-%ED%85%8D%EC%8A%A4%ED%8A%B8/
        public void DamageTextView(int damage)
        {
            if (damage <= 0)
            {
                damage = 0;
            }

            DamageText = Managers.Resource.Instantiate("DamageText");
            DamageText.transform.SetParent(_lockTarget.transform);
            DamageText.transform.position = new Vector3(_lockTarget.transform.position.x, 2.0f, _lockTarget.transform.position.z); // 표시될 위치
            DamageText.GetComponent<TextMesh>().text = damage.ToString(); // 데미지 전달
            DamageText.GetComponent<TextMesh>().color = Color.yellow;
            Invoke("DestroyObject", 0.5f);
        }

        private void DestroyObject()
        {
            Destroy(DamageText.gameObject);
        }
    }
}