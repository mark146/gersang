using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 유닛의 스텟 정보를 가져와서 반영
public class UnitHpBar : MonoBehaviour
{
    Stat _stat;
    
    void Start()
    {
        //Debug.Log($"transform.parent.name: {transform.parent.name}");
        _stat = transform.parent.GetComponent<Stat>();
    }

    
    void Update()
    {
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
