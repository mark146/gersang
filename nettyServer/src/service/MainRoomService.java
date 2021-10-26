package service;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import vo.Chat;
import vo.Player;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.*;

// io.netty.util.ResourceLeakDetector reportTracedLeak
public class MainRoomService {
    private static Gson gson = new GsonBuilder().setLenient().setPrettyPrinting().create();
    private static List<Player> userList = new ArrayList<Player>();
    private static int userCount = 0;
    private static Queue<Chat> chatQueue = new LinkedList<>();    //Queue의 인터페이스 구현체인 LinkedList를 사용
    private static SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");


    // 접속중인 모든 클라이언트에게 신규 유저가 접속한 사실을 전달
    // 유저 리스트에 존재하는지 체크, 플레이어가 서버에 처음 들어 올 경우 null 발생
    // 움직이는 유저 정보 검색 후 좌표 변경
    public static void EnterRoom(Player player) {
        boolean isUser = false;

        // 유저가 처음 접속할 경우 추가
        if (userList.size() == 0) {
            // System.out.println("유저 추가");
            userList.add(player);
        }

        // 접속 유저 조회 후 만약 유저 리스트에 있다면 true 변환
        for (int i = 0; i < userList.size(); i++) {
            if (userList.get(i).getId().equals(player.getId()) == true) {
                isUser = true;
            }
        }

        // 유저 리스트에 존재하지 않을 경우 유저 정보 추가
        if(isUser == false) {
            userList.add(player);
        }
    }


    // 1. 서버 쪽에서 허락 패킷이 왔을때 이동하는 방법 2. 일단 플레이어 이동 시킨 후 서버에서 응답이 오면 보정
    public static void UserMove(Player player) {
        //  System.out.println("4 - 현재 존재하는 유저 수: " + userList.size());

        //1. 움직이는 유저 정보 검색 후 좌표 변경
        for (int i = 0; i < userList.size(); i++) {
            if (userList.get(i).getId().equals(player.getId()) == true) {
                userList.get(i).setState(player.getState());
                userList.get(i).setGender(player.getGender());
                userList.get(i).setX(player.getX());
                userList.get(i).setY(player.getY());
                userList.get(i).setZ(player.getZ());
                userList.get(i).setDestPos_x(player.getDestPos_x());
                userList.get(i).setDestPos_y(player.getDestPos_y());
                userList.get(i).setDestPos_z(player.getDestPos_z());
            }
        }
    }


    //룸 정보 전송
    public static List<Player> Spawn() {
        if (userCount != userList.size()) {
            System.out.println("userList 유저 수: " + userList.size());
            userCount = userList.size();
        }

        return userList;
    }


    // 클라이언트한테서 온 채팅 정보 비교 및 조회 처리
    public static List<Chat> ChatChek(List<String> userInfo, List<Chat> chatList) {
        try {
            // 서버 채팅 로그 갯수가 한개이상 있을 경우에만 실행
            if (chatQueue.size() != 0) {

                // 비교 순서: 1. 캐릭터 접속 시간 비교, 2. 클라이언트 마지막 채팅 로그 비교(채팅 작성 시간 비교)
                // 클라이언트한테 마지막 채팅 내용이 존재하지 않을 경우, 큐 사용법 - 참고: https://velog.io/@agugu95/Java-Queue
                List<Chat> reList = (List<Chat>) chatQueue;
                //       System.out.println("채팅 큐에 있는 채팅 로그 갯수: " + reList.size());

                for (int i = 0; i < reList.size(); i++) {
                    // System.out.println("채팅 로그(" + i + ") PlayerId:" + reList.get(i).getPlayerId() + ", Content:" + reList.get(i).getContent() + ", PlayerConnectTime:" + reList.get(i).getPlayerConnectTime() + ", Time:" + reList.get(i).getTime());

                    // 시간 변환 참고: https://yuja-kong.tistory.com/26, https://codechacha.com/ko/java-compare-date-and-time/
                    Date recvDate = simpleDateFormat.parse(userInfo.get(1).toString().trim()); // 클라이언트 접속 시간
                    Date queueDate = simpleDateFormat.parse(reList.get(i).getTime());

                    // 1. 캐릭터 접속 시간 비교("before(): 인자보다 과거일 때 true가 리턴") (1차 분류) - 캐릭터 접속 이후 채팅 로그 출력
                    if (recvDate.before(queueDate)) {

                        // 2. 채팅 작성 시간 비교 (2차 분류) - 캐릭터 접속 이후 채팅 로그 출력
                        if(userInfo.size() != 4) {
                            recvDate = simpleDateFormat.parse(userInfo.get(7).toString().trim()); // 채팅 작성 마지막 시간
                            queueDate = simpleDateFormat.parse(reList.get(i).getTime());

                            // 클라이언트의 마지막 채팅 내용이랑 비교 해서 그 이후 값이 있다면 그 이후값만 저장 후 클라이언트에게 전송
                            if (recvDate.before(queueDate)) {
                                Chat info = new Chat();
                                info.setPlayerId(reList.get(i).getPlayerId());
                                info.setContent(reList.get(i).getContent());
                                info.setCurrentLocation(reList.get(i).getCurrentLocation());
                                info.setPlayerConnectTime(reList.get(i).getPlayerConnectTime());
                                info.setTime(reList.get(i).getTime());

                                chatList.add(info);
                            }
                        } else {
                            Chat info = new Chat();
                            info.setPlayerId(reList.get(i).getPlayerId());
                            info.setContent(reList.get(i).getContent());
                            info.setCurrentLocation(reList.get(i).getCurrentLocation());
                            info.setPlayerConnectTime(reList.get(i).getPlayerConnectTime());
                            info.setTime(reList.get(i).getTime());
                            chatList.add(info);
                        }
                    }
                }
            }
        } catch (ParseException e) {
            e.printStackTrace();
        }

        return chatList;
    }


    // 채팅 내용 업데이트
    public static void ChatUpdate(Chat chatInfo) {
        // 클라이언트한테서 온 내용 조회
//        System.out.println("ChatUpdate 실행 - chatInfo 내용");
//        System.out.println("유저 아이디: " + chatInfo.getPlayerId());
//        System.out.println("유저 접속 시간: " + chatInfo.getPlayerConnectTime());
//        System.out.println("유저 위치: " + chatInfo.getCurrentLocation());
//        System.out.println("채팅 내용: " + chatInfo.getContent());
//        System.out.println("채팅 입력 시간: " + chatInfo.getTime());

        // 큐에 추가 - 참고: https://hoho325.tistory.com/56
        chatQueue.offer(chatInfo);
    }


    //룸 나갈 경우
    public static void LeaveGame(Player player) {
        // 본인한테 정보 전송

        // 타인한테 정보 전송
    }
}