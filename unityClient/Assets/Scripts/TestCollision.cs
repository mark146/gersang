using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollision : MonoBehaviour
{

    // Collision 이벤트 발생 조건
    // 1) 나 혹은 상대한테 RigidBody 있어야 한다. (isKinematic : Off)
    // 2) 나한테 Collider가 있어야 한다. (isTrigger : Off)
    // 3) 상대한테 Collider가 있어야 한다. (isTrigger : Off)
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"collision @ {collision.gameObject.name} !");
    }

    // 1) 둘 다 collider가 있어야 한다.
    // 2) 둘 중 하나는 isTrigger : On
    // 3) 둘 중 하나는 RigidBody가 있어야 한다.
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger @ {other.gameObject.name} !");
    }

    void Start()
    {

    }

    void Update()
    {
        /*
        레이캐스팅 필요성?
        레이 : 광선, 캐스팅 : 쏘다.
        */
        Physics.Raycast(transform.position, Vector3.forward);

        //바닥에서 쏨
        Debug.DrawRay(transform.position, Vector3.forward, Color.red);


        // Local <-> World <-> (Viewport <-> Screen) (화면)

        // Debug.Log(Input.mousePosition); // Screen , 픽셀 좌표 표시

        // Debug.Log(Camera.main.ScreenToViewportPoint(Input.mousePosition)); // Viewport, 화면 비율 % 차지하는지 확인

        GameObject.FindGameObjectsWithTag("Monster");

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

            LayerMask mask = LayerMask.GetMask("Monster") | LayerMask.GetMask("Wall");
            // int mask = (1 << 8) | (1<<9);


            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100.0f, mask))
            {
                Debug.Log($"Raycast Camera @{hit.collider.gameObject.tag}");
            }


            // 카메라 기준 레이캐스팅 구현 (원리)
            //    Vector3 mousPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            //    Vector3 dir = mousPos - Camera.main.transform.position;
            //    dir = dir.normalized;

            //    Debug.DrawRay(Camera.main.transform.position, dir * 100.0f, Color.red, 1.0f);

            //    RaycastHit hit;
            //    if (Physics.Raycast(Camera.main.transform.position, dir, out hit, 100.0f))
            //    {
            //        Debug.Log($"Raycast Camera @{hit.collider.gameObject.name}");
            //    }
        }

    }
}
