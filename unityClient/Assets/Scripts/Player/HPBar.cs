using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    Stat _stat;
    Transform parent;

    void Start()
    {
        //Debug.Log($"transform.parent.name: {transform.parent.name}");
        _stat = transform.parent.GetComponent<Stat>();
        parent = transform.parent;
    }

    void Update()
    {
        // 내 부모 위치 기준으로 콜라이더 만큼을 위로 올려서 나를 위치
        transform.position = parent.position + new Vector3(0, 1.2f,0) * (parent.GetComponent<BoxCollider>().bounds.size.y);

        // 로테이션을 카메라의 로테이션이랑 맞춰줌
        transform.rotation = Camera.main.transform.rotation;


        // 체력바 설정
        float ratio = _stat.Hp / (float)_stat.MaxHp;
        SetHpRatio(ratio);
    }

    //hp바 수정
    public void SetHpRatio(float ratio)
    {
        transform.GetChild(0).GetComponent<Slider>().value = ratio;
    }
}
