using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MPBar : MonoBehaviour
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
        transform.position = parent.position + Vector3.up * (parent.GetComponent<BoxCollider>().bounds.size.y);

        // 로테이션을 카메라의 로테이션이랑 맞춰줌
        transform.rotation = Camera.main.transform.rotation;


        // 체력바 설정
        float ratio = _stat.Mp / (float)_stat.MaxMp;
        SetMpRatio(ratio);
    }

    //hp바 수정
    public void SetMpRatio(float ratio)
    {
        transform.GetChild(0).GetComponent<Slider>().value = ratio;
    }
}
