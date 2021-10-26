using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 참고: https://truecode.tistory.com/65
public class InvenItem : MonoBehaviour
{
    [SerializeField]
    public string ItemName { get; set; }

    enum ItemInfo
    {
        ItemIcon,
        ItemNameText,
        // Equipment,//장비
        // Consumption,//소모
        // Misc//기타
    }


}
