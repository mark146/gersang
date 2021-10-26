using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum UIEvent
    {
        Click,
        Drag,
    }

    public enum MouseEvent
    {
        Press,
        PointerDown, // 맨처음 땐 상태에서 누름
        PointerUp, // 마우스를 한번 누른 후 땐 상태
        Click,
        Drag,
    }

    // state 패턴, 플레이어 상태 저장
    public enum State
    {
        Die,
        Moving,
        Idle,
        Skill,
        Hold,
    }

    public enum Layer
    {
        Ground = 8,
        Wall = 9,
        Monster = 10,
        Store = 11,
    }

    public enum CameraMode
    {
        QuarterView,
    }
}
