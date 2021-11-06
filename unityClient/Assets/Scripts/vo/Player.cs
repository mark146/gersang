using System;


[Serializable]
public class Player {
    public string Id { get; set; }

    public string Gender { get; set; }

    public string State { get; set; }

    //목적지 정보
    public float DestPos_x { get; set; }

    public float DestPos_y { get; set; }

    public float DestPos_z { get; set; }

    //캐릭터 현재 위치 정보
    public float x { get; set; }

    public float y { get; set; }

    public float z { get; set; }
}
