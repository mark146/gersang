using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//몬스터, 플레이어의 공통기능을 모아둔 클래스
public abstract class BaseController : MonoBehaviour
{
    [SerializeField]
    protected Define.State _state = Define.State.Idle;

    [SerializeField]
    protected Vector3 _destPos;// 이동 목적지

    [SerializeField]
    protected GameObject _lockTarget;// 마우스로 찍은 대상

    public virtual Define.State State
    {
        get { return _state; }
        set
        {
            _state = value;

            Animator anim = GetComponent<Animator>();
            switch (_state)
            {
                case Define.State.Die:
                    break;
                case Define.State.Idle:
                    anim.CrossFade("Idle", 0.1f);
                    //anim.Play("Idle");
                    break;
                case Define.State.Moving:
                    //anim.Play("Run");
                    anim.CrossFade("Run", 0.1f);
                    break;
                case Define.State.Skill:
                    //Debug.Log("Skill");
                    anim.CrossFade("Attack", 0.1f, -1, 0);
                    break;
                case Define.State.Hold:
                    //Debug.Log("Hold");
                    anim.CrossFade("Idle", 0.1f);
                    //anim.CrossFade("Attack", 0.1f, -1, 0);
                    break;
            }
        }
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        switch (State)
        {
            case Define.State.Die:
                UpdateDie();
                break;
            case Define.State.Moving:
                UpdateMoving();
                break;
            case Define.State.Idle:
                UpdateIdle();
                break;
            case Define.State.Skill:
                UpdateSkill();
                break;
            case Define.State.Hold:
                UpdateHold();
                break;
            default:
                break;
        }
    }

    public abstract void Init();
    protected virtual void UpdateDie() { }
    protected virtual void UpdateMoving() { }
    protected virtual void UpdateIdle() { }
    protected virtual void UpdateSkill() { }
    protected virtual void UpdateHold() { }
}