package controller;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import entity.GearEntity;
import entity.InventoryEntity;
import entity.UserEntity;
import vo.User;
import java.util.HashMap;
import java.util.List;
import java.util.Map;


public class LobbyController {
    Gson gson = new GsonBuilder().setLenient().setPrettyPrinting().create();
    UserEntity userEntity = new UserEntity();
    InventoryEntity inventoryEntity = new InventoryEntity();
    GearEntity gearEntity = new GearEntity();
    String result = null;
    Map<String, String> sendData = new HashMap<>();


    public String controller(int key, Object value) {
        result = null;
        sendData.clear();

        switch (key) {
            case 1:
                User user = gson.fromJson(String.valueOf(value), User.class);
                String id = user.getId();
                String password = user.getPassword();

                if (id == null) {
                    sendData.put("1", "아이디가 존재하지 않습니다.");
                    // 값 클라이언트에게 전송
                    result = gson.toJson(sendData);
                } else if (password == null) {
                    sendData.put("1", "비밀번호가 일치하지 않습니다.");
                    // 값 클라이언트에게 전송
                    result = gson.toJson(sendData);
                } else {
                    result = gson.toJson(userEntity.Login(user));
                }
                break;
            case 2:
                String text = gson.fromJson(String.valueOf(value), String.class);
                System.out.println("case 2: gson.fromJson - text: "+text);

                sendData = userEntity.Verification(text);

                result = gson.toJson(sendData);
                break;
            case 3:
                user = gson.fromJson(String.valueOf(value), User.class);
                sendData = userEntity.Register(user);

                result = gson.toJson(sendData);
                break;
            case 10:
                //캐릭터 생성창 - 닉네임 중복 체크
                String playerId = (String) value;

                sendData = userEntity.NickNameCheck(playerId);

                result = gson.toJson(sendData);
                break;
            case 11: // 캐릭터 생성창 - 캐릭터 생성
                List<String> nameList = gson.fromJson(String.valueOf(value), List.class);
                /*
                0: 플레이어 아이디
                1: 캐릭터 이름
                2: 캐릭터 성별
                */
//                for(int i =0; i<nameList.size(); i++) {
//                    System.out.println("nameList.get(i): "+nameList.get(i));
//                }

                sendData = userEntity.PlayerCreate(nameList);

                // 장비, 인벤토리 생성
                String nickName = nameList.get(1);
                inventoryEntity.InventoryCreate(nickName);
                gearEntity.GearCreate(nickName);

                result = gson.toJson(sendData);
                break;
            case 12: // 캐릭터 선택창 - 캐릭터 조회
                // Object -> String으로 변환
                String userId = gson.fromJson(String.valueOf(value), String.class);

                // 캐릭터 선택창 - 캐릭터 조회
                Map<String, List<String>> playerCheck = userEntity.PlayerCheck(userId);

                result = gson.toJson(playerCheck);
                break;
            case 13: //캐릭터 선택창 - 캐릭터  삭제
                nameList = gson.fromJson(String.valueOf(value), List.class);
                // nameList = (List<String>) value;

//                for(int i =0; i<nameList.size(); i++) {
//                    System.out.println("nameList.get("+i+"): "+nameList.get(i));
//                }

                sendData = userEntity.PlayerDelete(nameList);

                // 장비, 인벤토리 삭제
                nickName = nameList.get(0).toString();
                inventoryEntity.InventoryDelete(nickName);
                gearEntity.GearDelete(nickName);

                // 값 클라이언트에게 전송
                result = gson.toJson(sendData);
                break;
            default:
                System.out.println("기타 요청 키 값: " + key);
                result = "null";
                break;
        }
        return result;
    }
}