using UnityEngine;

public class OrderPlayer : MonoBehaviour
{
    public string PlayerId { get; set; }

    // 이동 목적지
    public Vector3 destPos { get; set; }

    public static Vector3 beforePos { get; set; }

    public string state { get; set; }
    
    
    // Update is called once per frame
    void FixedUpdate()
    {
        Animator anim = GetComponent<Animator>();
        switch (state)
        {
            case "Idle":
                anim.CrossFade("Idle", 0.1f);
                if (beforePos != destPos)
                {
                    Rotate();
                }
                break;
            case "Moving":
                anim.CrossFade("Run", 0.1f);
                // 캐릭터 방향을 목적지 쪽으로 돌려줌
                if (beforePos != destPos)
                {
                    Rotate();
                }
                break;
        }
    }

    void Rotate()
    {
        //Debug.Log($"destPos 값: {destPos}");
        //Debug.Log($"beforePos 값: {beforePos}");

        beforePos = destPos;

        // 플레이어가 이동해야할 위치 방향 벡터값 (목적지 - 플레이어 위치)
        Vector3 dir = destPos - transform.position;

       // Debug.Log($"dir 값: {dir}");
        //order.transform.rotation = Quaternion.LookRotation(new Vector3(players[i].DestPos_x, players[i].DestPos_y, players[i].DestPos_z));
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 0.1f * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(dir.normalized);
    }
}
