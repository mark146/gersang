package vo;

import java.io.Serializable;

public class Player implements Serializable {
    private String Id;
    private String Gender;
    private String State;

    //캐릭터 목적지 정보
    private float DestPos_x;
    private float DestPos_y;
    private float DestPos_z;

    //캐릭터 현재 위치 정보
    private float x;
    private float y;
    private float z;


    public String getId() {
        return Id;
    }

    public void setId(String id) {
        Id = id;
    }

    public String getGender() {
        return Gender;
    }

    public void setGender(String gender) {
        Gender = gender;
    }

    public String getState() {
        return State;
    }

    public void setState(String state) {
        State = state;
    }

    public float getDestPos_x() {
        return DestPos_x;
    }

    public void setDestPos_x(float destPos_x) {
        DestPos_x = destPos_x;
    }

    public float getDestPos_y() {
        return DestPos_y;
    }

    public void setDestPos_y(float destPos_y) {
        DestPos_y = destPos_y;
    }

    public float getDestPos_z() {
        return DestPos_z;
    }

    public void setDestPos_z(float destPos_z) {
        DestPos_z = destPos_z;
    }

    public float getX() {
        return x;
    }

    public void setX(float x) {
        this.x = x;
    }

    public float getY() {
        return y;
    }

    public void setY(float y) {
        this.y = y;
    }

    public float getZ() {
        return z;
    }

    public void setZ(float z) {
        this.z = z;
    }
}
