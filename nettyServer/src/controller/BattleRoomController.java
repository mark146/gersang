package controller;

import java.util.HashMap;
import java.util.Map;


public class BattleRoomController {
    private static Map<String, String> sendData = new HashMap<>();
    String result = null;

    public String controller(int key, Object value) {
        result = null;
        sendData.clear();

        switch (key) {
            case 21: // 전투 장면에 들어갔을 경우 실행
                System.out.println("21 - value: " + value);
                break;
            default:
                System.out.println("[MainRoomController] 기타 - key: "+key+", value: " + value);
                break;
        }
        return null;
    }
}