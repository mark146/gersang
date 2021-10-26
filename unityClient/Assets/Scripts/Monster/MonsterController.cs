using RTS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MonsterController : BaseController
{
    Stat _stat;

    [SerializeField]
    float _scanRange = 10;

    [SerializeField]
    float _attackRange = 2;
    [SerializeField]
    float timer;

    [SerializeField]
    float waitingTime;

    GameObject DamageText;

    public override void Init()
    {
        timer = 0.0f;
        waitingTime = 0.8f;

        _stat = gameObject.GetComponent<Stat>();
        //HPBar가 없을 경우 생성
        if (transform != null)
        {
            //Debug.Log($"transform.name: {transform.name}");
            GameObject go = Managers.Resource.Instantiate($"UI_HPBar");
            go.transform.SetParent(this.transform);// 나에게 붙어서  hp바 생성
        }
    }


    private void UpdateDie()
    {
        Debug.Log($"UpdateDie 실행 - {transform.name}");
    }

    // 가만히 있다가 주변에 플레이어가 있는지 서칭 발견하면 이동
    protected override void UpdateIdle()
    {
        // 참고: https://cru6548.tistory.com/5
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        if (player != null)
        {
            for (int i = 0; i < player.Length; i++)
            {
                //Debug.Log($"UpdateIdle - player[{i}]: {player[i]}");
                float distance = (player[i].transform.position - transform.position).magnitude;
                if (distance <= _scanRange)
                {
                    _lockTarget = player[i];
                    State = Define.State.Moving;
                    return;
                }
            }
        }

    }

    // 플레이어에게 다가가는 함수
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
                NavMeshAgent nma = gameObject.GetComponent<NavMeshAgent>();
                nma.SetDestination(transform.position);// 상대 목표 지정시 자동 이동
                State = Define.State.Skill;
                return;
            }
        }

        // 플레이어가 이동해야할 위치 방향 벡터값 (목적지 - 플레이어 위치)
        Vector3 dir = _destPos - transform.position;
        dir.y = 0.5f;

        // 목적지에 도달하면 멈춤
        if (dir.magnitude < 0.1f) //0.0001f 벡터 계산은 오차 범위가 있어서 0이 아닌 숫자로 설정
        {
            State = Define.State.Idle;
        }
        else
        {
            // 아니면 계속 이동
            // 이동 강의 참고, NavMeshAgent: 길찾기 지원 클래스
            NavMeshAgent nma = gameObject.GetComponent<NavMeshAgent>();
            nma.SetDestination(_destPos);// 상대 목표 지정시 자동 이동
            nma.speed = _stat.MoveSpeed;

            //transform.position += dir.normalized * moveDist;// 속도 * 시간
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
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

        //미사일 생성 후 시전
        timer += Time.deltaTime;

        if (timer > waitingTime)
        {
            timer = 0.0f;
            OnHitEvent();
        }
    }

    void OnHitEvent()
    {
        // 체력을 깎는 부분
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();

            //체력 깎는 텍스트 표시
            int damage = Mathf.Max(0, _stat.Attack - targetStat.Defense);
            if (damage <= 0)
            {
                damage = 0;
            }

            //참고: https://lesslate.github.io/unity/%EC%9C%A0%EB%8B%88%ED%8B%B0-%ED%94%8C%EB%A1%9C%ED%8C%85-%EB%8D%B0%EB%AF%B8%EC%A7%80-%ED%85%8D%EC%8A%A4%ED%8A%B8/
            DamageText = Managers.Resource.Instantiate("DamageText");
            DamageText.transform.SetParent(_lockTarget.transform);            
            DamageText.transform.position = new Vector3(_lockTarget.transform.position.x, 2.0f, _lockTarget.transform.position.z); // 표시될 위치
            DamageText.GetComponent<TextMesh>().text = "-"+damage.ToString(); // 데미지 전달
            DamageText.GetComponent<TextMesh>().color = Color.red;
            Invoke("DestroyObject", 0.5f);

            targetStat.OnAttacked(_stat);//매개변수:  내스텟 


            if (targetStat.Hp <= 0)
            {
                //Debug.Log("플레이어가 죽을 경우");
                GameObject.Destroy(targetStat.gameObject);
                //  Managers.Game.Despwan(targetStat.gameObject);

                //죽은 유닛 리스트에서 제거
                BattleScene scene = GameObject.Find("@BattleScene").GetComponent<BattleScene>();
                scene.unitList.Remove(_lockTarget);

                _lockTarget = null;
                State = Define.State.Idle;
            }

            if (targetStat.Hp > 0)
            {
                // 거리 체크
                float distance = (_lockTarget.transform.position - transform.position).magnitude;
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
        } else {
            State = Define.State.Idle;
        }
    }
    private void DestroyObject()
    {
        Destroy(DamageText.gameObject);
    }

}