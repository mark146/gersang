using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*
프로젝트가 커지면 소스 추적이 힘듬 따라서 한곳에서 모아서 관리
GameObject prefab = Resources.Load<GameObject>("Prefabs/Tank");// Resources 기준으로 프리팹 호출
GameObject tank = Instantiate(prefab); // 프리팹 실체화
Destroy(tank, 3.0f);// 객체 제거


GameObject tank = Managers.Resource.Instantiate("Tank");// 알아서 찾아서 로딩

Managers.Resource.Destroy(tank); // 제거
*/
public class ResourceManager
{
    // 원본을 찾는 역할, Resources 기준으로 프리팹 호출
    public T Load<T>(string path) where T : Object
    {

        return Resources.Load<T>(path);
    }

    // 프리팹 실체화
    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");

        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        // Object를 안붙이면 Instantiate를 재귀적으로 호출함
        return Object.Instantiate(original, parent);
    }

    // 객체 제거
    public void Destroy(GameObject go)
    {
        if (go == null)
        {
            return;
        }

        Object.Destroy(go);
    }
}