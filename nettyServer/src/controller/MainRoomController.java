package controller;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import entity.GearEntity;
import entity.InventoryEntity;
import entity.MercenaryEntity;
import entity.UserEntity;
import io.netty.buffer.ByteBuf;
import io.netty.buffer.Unpooled;
import io.netty.channel.ChannelHandlerContext;
import service.MainRoomService;
import vo.Chat;
import vo.Player;
import java.util.*;


public class MainRoomController {
    private static Gson gson = new GsonBuilder().setLenient().setPrettyPrinting().create();
    private static Map<String, String> sendData = new HashMap<>();
    private static InventoryEntity inventoryEntity = new InventoryEntity();
    private static GearEntity gearEntity = new GearEntity();
    private static UserEntity userEntity = new UserEntity();
    private static MercenaryEntity mercenaryEntity = new MercenaryEntity();
    private static MainRoomService mainRoomService = new MainRoomService();
    String result = null;


    public String controller(ChannelHandlerContext ctx, int key, Object value) {
        result = null;
        sendData.clear();

        switch (key) {
            case 8: // 메인 룸에 접속해 있는 유저 정보 조회
                Map<String, List> sendData = new HashMap<>();
                List<Chat> chatList = new ArrayList<Chat>();

                // 클라이언트에서 온 값을 리스트로 변환
                List<String> userInfo = gson.fromJson(String.valueOf(value), List.class);

                // 채팅 내용 비교하기 위해 이동, 채팅에 추가할 내용이 있다면 map에 추가할 내용 저장
                chatList = mainRoomService.ChatChek(userInfo, chatList);
                sendData.put("8", mainRoomService.Spawn()); // 유저 정보 조회
                sendData.put("23", chatList); // 유저 정보 조회

                // 서버로 보내기 전
                // Set<Map.Entry<String, List>> entries = sendData.entrySet();
//                for (Map.Entry<String, List> entry : entries) {
//                    System.out.print("key: "+ entry.getKey());
//                    System.out.println(", Value: "+ entry.getValue());
//                }

                String jdata = gson.toJson(sendData);
                ByteBuf messageBuffer = Unpooled.buffer();

                // io.netty.util.ResourceLeakDetector reportTracedLeak 에러 발생
                messageBuffer.writeBytes(jdata.getBytes());
                ctx.writeAndFlush(messageBuffer);
                break;
            case 4: // 캐릭터가 이동할 경우 호출되며, 캐릭터 위치값 저장
                Player player = gson.fromJson(String.valueOf(value), Player.class);
                mainRoomService.UserMove(player);
                break;
            case 5: // 캐릭터가 메인 룸에 처음 입장할 경우, 전투 후 메인 룸에 입장할 경우 실행
                player = gson.fromJson(String.valueOf(value), Player.class);
                // System.out.println("5 - Value: "+ value);
                mainRoomService.EnterRoom(player);
                break;
            case 6: // 인벤토리를 열때 호출 - 인벤토리 정보, 장비창 정보 조회 후 전달
                // Object -> String 변환
                String recvData = gson.fromJson(String.valueOf(value), String.class);
                List<String> invenInfo = inventoryEntity.InventoryOpen(recvData);
                List<String> gear = gearEntity.GearOpen(recvData);


                Map<String, List<String>> sendList = new HashMap();
                sendList.put("6", invenInfo);
                sendList.put("7", gear);


                // Map -> String 변환 후 전달
                jdata = gson.toJson(sendList);
                messageBuffer = Unpooled.buffer();
                messageBuffer.writeBytes(jdata.getBytes());
                ctx.writeAndFlush(messageBuffer);
                break;
            case 9: // 인벤토리를 열때 호출 - 장비창 정보, 장비창 내용이 변경할 경우 실행
                // System.out.println("9 - value: " + value);

                // 클라이언트에서 온 값을 리스트로 변환 후 데이터 수정
                List<String> gearList = gson.fromJson(String.valueOf(value), List.class);
                String userId = gearList.get(7);
                gearList.remove(7);

                // 데이터 확인
//                for (int i = 0; i < gearList.size(); i++){
//                    System.out.println("gearList.get("+i+"): "+gearList.get(i));
//                }

                gearEntity.GearUpdate(userId, gearList);
                break;
            case 14: //인벤토리창 내용이 변경할 경우 실행, 인벤토리 정보 업데이트
                // System.out.println("14 - value: " + value);
                String playerId = null;
                List<String> invenList = new ArrayList<String>();
                Map<String, Object> recvInven = gson.fromJson(String.valueOf(value), Map.class);

                // 모든 key 조회
                Set<Map.Entry<String, Object>> invenEntries = recvInven.entrySet();
                for (Map.Entry<String, Object> entry : invenEntries) {
                    // System.out.print("key: "+ entry.getKey()+", Value: "+ entry.getValue());
                    playerId = entry.getKey();
                    invenList = (List<String>) entry.getValue();
                }

                inventoryEntity.InventoryUpdate(playerId, invenList);
                break;
            case 15: // 인벤토리를 닫을때 호출
                System.out.println("15 - value: " + value);
                break;
            case 16: // 플레이어가 아이템을 구매할 경우 실행, 데이터 수정해야할 정보: 플레이어 인벤토리, 돈
                System.out.println("16 - value: " + value);

                // 클라이언트에서 온 값을 리스트로 변환 후 데이터 수정
                List<String> tradeList = gson.fromJson(String.valueOf(value), List.class);
                // System.out.println("tradeList.size(): "+tradeList.size());

                // 서버에서 온 데이터가 정상적으로 올경우 true, 예외사항이 발생해서 다시 전송해 올 경우 false 실행
                if(tradeList.size() == 4) {

                    // 인벤토리 슬롯 정보 수정
                    inventoryEntity.InventorySlotUpdate(tradeList);

                    // 플레이어 소지금 수정
                    userEntity.PlayerGoldUpdate(tradeList);
                } else {
//                    for(int i =0; i < tradeList.size(); i++) {
//                        System.out.println("i: ["+i+"] 값: "+tradeList.get(i));
//                    }
                }
                break;
            case 17: // 플레이어가 아이템을 구매할 경우 실행, 데이터 수정해야할 정보: 플레이어 인벤토리, 돈
                // System.out.println("17 - value: " + value);

                //용병 정보 조회
                List<String> unitList = mercenaryEntity.MercenaryCheck(String.valueOf(value));
                // System.out.println("MercenaryCheck 조회 후: "+unitList.size());

                sendList = new HashMap();
                sendList.put("17", unitList);

                // Map -> String 변환 후 전달
                jdata = gson.toJson(sendList);
                messageBuffer = Unpooled.buffer();
                messageBuffer.writeBytes(jdata.getBytes());
                ctx.writeAndFlush(messageBuffer);
                break;
            case 18: // 훈련소에서 용병을 구매할 경우 호출 (전달 받을 정보: 플레이어 id, 돈, 용병 이름)
                System.out.println("18 - value: " + value);

                // 클라이언트에서 온 값을 리스트로 변환 후 데이터 수정
                List<String> mercenaryList = gson.fromJson(String.valueOf(value), List.class);

                // 용병 테이블에 정보 추가
                List<String> buyResult = mercenaryEntity.MercenaryBuy(mercenaryList.get(0), mercenaryList.get(2));

                // 플레이어 소지금 수정
                userEntity.PlayerGoldUpdate(mercenaryList);

                //용병 정보 조회
                sendList = new HashMap();
                sendList.put("18", buyResult);

                // Map -> String 변환 후 전달
                jdata = gson.toJson(sendList);
                messageBuffer = Unpooled.buffer();
                messageBuffer.writeBytes(jdata.getBytes());
                ctx.writeAndFlush(messageBuffer);
                break;
            case 19: // 훈련소에서 용병을 구매할 경우 호출 (전달 받을 정보: 플레이어 id, 돈, 용병 이름)
                System.out.println("19 - value: " + value);

                // 클라이언트에서 온 값을 리스트로 변환 후 데이터 수정
                mercenaryList = gson.fromJson(String.valueOf(value), List.class);

                // 용병 테이블에 정보 추가
                mercenaryEntity.MercenarySell(mercenaryList.get(0), mercenaryList.get(2));

                // 플레이어 소지금 수정
                userEntity.PlayerGoldUpdate(mercenaryList);

                List<String> sellResult = new ArrayList<>();
                sellResult.add("용병이 존재하지 않습니다.");

                //용병 정보 조회
                sendList = new HashMap();
                sendList.put("19", sellResult);

                // Map -> String 변환 후 전달
                jdata = gson.toJson(sendList);
                messageBuffer = Unpooled.buffer();
                messageBuffer.writeBytes(jdata.getBytes());
                ctx.writeAndFlush(messageBuffer);
                break;
            case 20: // 유저가 채팅을 보낼 경우 실행
                System.out.println("20 - value: " + value);

                // 클라이언트에서 온 값을 Chat 클래스로 변환
                Chat chatInfo = gson.fromJson(String.valueOf(value), Chat.class);

                // 채팅 내용 추가
                mainRoomService.ChatUpdate(chatInfo);
                break;
            case 22: // 유저가 장비 장착을 할 경우 전송
                // System.out.println("22 - value: " + value);

                // 클라이언트에서 온 값을 리스트로 변환
                List<String> playerInfo = gson.fromJson(String.valueOf(value), List.class);

                // 플레이어 정보 수정
                userEntity.PlayerUpdate(playerInfo);
                break;
            default:
                System.out.println("[MainRoomController] 기타 - key: "+key+", value: " + value);
                break;
        }
        return null;
    }
}