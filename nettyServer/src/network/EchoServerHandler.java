package network;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import controller.BattleRoomController;
import controller.LobbyController;
import controller.MainRoomController;
import io.netty.buffer.ByteBuf;
import io.netty.buffer.Unpooled;
import io.netty.channel.Channel;
import io.netty.channel.ChannelHandler;
import io.netty.channel.ChannelHandlerContext;
import io.netty.channel.ChannelInboundHandlerAdapter;
import io.netty.channel.group.ChannelGroup;
import io.netty.channel.group.DefaultChannelGroup;
import io.netty.util.ReferenceCountUtil;
import io.netty.util.concurrent.GlobalEventExecutor;

import java.nio.ByteBuffer;
import java.util.HashMap;
import java.util.Map;

/*
참고 사이트
https://devday.tistory.com/entry/ionettychannelChannelPipelineException-comizeyetestserviceConsolePrintingChannelInboundHandler-is-not-a-Sharable-handler-so-cant-be-added-or-removed-multiple-times
https://juyoung-1008.tistory.com/23
https://altongmon.tistory.com/503 [IOS를 Java]
https://jdm.kr/blog/151
https://github.com/jarvis-kim/chat-exam
https://m.blog.naver.com/PostView.nhn?blogId=lovesm135&logNo=220963148584&proxyReferer=https:%2F%2Fwww.google.com%2F
https://ko.coder.work/so/java/1921045
http://wonwoo.ml/index.php/post/553
https://netty.io/4.0/api/io/netty/channel/group/DefaultChannelGroup.html
http://blog.naver.com/rain483/220652729808
http://wonwoo.ml/index.php/post/category/netty
https://jyh1536.tistory.com/12
https://google.github.io/flatbuffers/flatbuffers_guide_tutorial.html
*/
//ChannelHandler를 여러 채널간에 안전하게 공유 할수 있음을 나타냄
// 클래스 설명: 클라이언트한테서 온 데이터를 처리하는 클래스
@ChannelHandler.Sharable
public class EchoServerHandler extends ChannelInboundHandlerAdapter {

    // 생성된 이름과 제공된 EventExecutor를 사용하여 새 그룹을 생성하여 ChannelGroupFutures에 알립니다.
    private static final ChannelGroup channelGroup = new DefaultChannelGroup(GlobalEventExecutor.INSTANCE);

    private static final LobbyController lobbyController = new LobbyController();
    private static final MainRoomController mainRoomController = new MainRoomController();
    private static final BattleRoomController battleRoomController = new BattleRoomController();

    private static Gson gson = new GsonBuilder().setLenient().setPrettyPrinting().create();

    // 1. 클라이언트가 로그인 할 경우 최초 한 번 실행
    @Override
    public void handlerAdded(ChannelHandlerContext ctx) throws Exception {
        System.out.println("[SERVER] handlerAdded 실행");
        Channel incoming = ctx.channel();
        // 채널 그룹에 등록
        channelGroup.add(incoming);
    }

    /**
     * 2 서버로 연결이 만들어질때, 채널 입출력 준비 완료 사용자가 들어왔을때. 실행
     * 사용자가 접속했을때 서버에 표시
     * 네티 api를 사용하여 채널 입출력을 수행할 상태다
     * 어떤 작업을 할 수 있을까
     * 클라이언트 연결 개수를 셀 때
     * 최초 연결 메세지
     * 연결된 상태에서 작업이 필요할 때 ==> 여기서 채팅방 나눠줘야 하나
     */
    @Override
    public void channelActive(ChannelHandlerContext ctx) throws Exception {
        System.out.println("[SERVER] channelActive 실행");
        System.out.println("서버 접속자 수: " + channelGroup.size());
    }

    /*
    3. 클라이언트로부터 새 데이터가 수신 될 때마다 수신 된 메시지와 함께 호출
    ChannelHandlerContext객체는 다양한 I / O 이벤트와 작업을 트리거 할 수 있도록 다양한 작업을 제공합니다.
    여기서, 우리 write(Object)는 수신 된 메시지를 그대로 쓰도록 호출 합니다.
    수신받은 데이터는 Object msg 형태로 받게 되는데, 이 Object는 ByteBuf에 넣어서 처리를 합니다.

    1. 바이트 버퍼를 선언
    2. 새로운 바이트 배열 선언
    3. 새로운 바이트 배열 초기화
    4. 뒤에서 부터 값 채우기
    5. 바이트 버퍼에 새로운 바이트 배열 맵핑
    6. Big Endian 으로 수정
    7. Int 형으로 변경

    참고
    http://hatemogi.github.io/netty-startup/3.html#/6/4
    https://parkhyeokjin.github.io/netty/2019/12/29/netty-chap3.html
    https://answerofgod.tistory.com/entry/ByteBuf
    https://dptablo.tistory.com/59
    https://netty.io/wiki/user-guide-for-4.x.html
    https://parkhyeokjin.github.io/netty/2019/12/29/netty-chap3.html
    */
    @Override
    public void channelRead(ChannelHandlerContext ctx, Object msg) {
        // System.out.println("[SERVER] channelRead 실행");
        int protocolId = 0;

        try {
            // 클라이언트한테 받은 값을 ByteBuf 형으로 캐스팅
            ByteBuf recvData = (ByteBuf) msg;

            // 읽을 수 있는 데이터 만큼 byte 배열 생성 
            byte[] recv = new byte[recvData.readableBytes()];

            // 받은 값을 생성한 byte 배열 안에 저장
            for (int i = 0; i < recvData.readableBytes(); i++) {
                recv[i] = recvData.getByte(i);
            }

            // 클라이언트한테 온 데이터를 flatbuffer 라이브러리를 사용해서 Message 클래스로 역직렬화
            // flatbuffer 라이브러리를 왜 사용하는가? 네트워크로 데이터를 전송할때 데이터가 클 수록 전송속도가 늘어나는데 이런 문제를 해결하기 위해서 라이브러리 사용(아파치 라이센스)
            // 남은 콘텐츠가 충분하지 않으면 IndexOutOfBoundsException 발생 합니다.
            // (고민) 데이터가 가끔씩 다 충분하지 않게 와서 에러가 발생하는데 이 문제를 어떻게 해결할 것인가?
            Message message = Message.getRootAsMessage(ByteBuffer.wrap(recv));

            protocolId = message.ProtocolId();

            // id 값에 따라 기능 처리
            switch (message.ProtocolId()) {
                /*
                8: 메인 룸에 접속해 있는 유저 정보 조회
                5: 캐릭터가 메인 룸에 처음 입장할 경우, 전투 후 메인 룸에 입장할 경우 실행
                4: 캐릭터가 이동할 경우 호출되며, 캐릭터 위치값 저장
                6: 인벤토리창을 열 떄 호출 되며 장비, 인벤토리 정보를 전달해줌
                9: 인벤토리를 열때 호출 - 장비창 정보, 장비창 내용이 변경할 경우 실행
                14: 인벤토리창 내용이 변경할 경우 실행
                15: 인벤토리창을 열 떄 호출 되며, 닫기전 장비, 인벤토리 정보 조회 후 변경 사항 있으면 변경
                16: 플레이어가 아이템을 구매할 경우 호출, 플레이어 돈, 인벤토리 정보 수정
                17: 메인 씬에 접속하기 전에 호출되며, 캐릭터의 용병 정보를 조회 후 전달해줌
                18: 훈련소에서 용병을 구매할 경우 호출
                19: 훈련소에서 용병을 판매할 경우 호출
                20: 클라이언트가 채팅메시지를 보낼 경우 호출
                */
                case 8: case 4: case 6: case 5: case 9: case 14: case 15: case 16: case 17: case 18: case 19: case 20:
                    mainRoomController.controller(ctx, message.ProtocolId(), message.content());
                    break;
                /*
                1: 로그인
                2: 회원가입 아이디 체크
                3: 회원가입
                10: 캐릭터 생성창 닉네임 중복 체크
                11: 캐릭터 생성
                12: 캐릭터 조회
                13: 캐릭터 삭제
                */
                case 1: case 2: case 3: case 10: case 11: case 12: case 13:
                    String sendData = lobbyController.controller(message.ProtocolId(), message.content());

                    // 처리한 값을 클라이언트에게 전송
                    ByteBuf messageBuffer = Unpooled.buffer();
                    messageBuffer.writeBytes(sendData.getBytes());

                    // ctx 는 ChannelHandlerContext w인터페이스의 객체로서 채널 파이프라인에 대한 이벤트를 처리한다.
                    ctx.writeAndFlush(messageBuffer);
                    break;

                    /**
                     21: 전투 씬에 들어갈 경우 실행
                     */
                case 21:
                    battleRoomController.controller(message.ProtocolId(), message.content());
                    break;
                default:
                    System.out.println("예외 사항 키: " + message.ProtocolId());
                    break;
            }
        } catch (IndexOutOfBoundsException e) {
            //System.out.println("ArrayIndexOutOfBoundsException 발생");
            Map<String, String> sendList = new HashMap();
            switch (protocolId) {
                case 4:// 이동
                    break;
                case 8:// 메인룸 정보 전달
                    break;
                case 9:
                    sendList.put("9", "ArrayIndexOutOfBoundsException 발생");
                    break;
                case 14: // 클라이언트에게 에러 메세지 전송
                    sendList.put("14", "ArrayIndexOutOfBoundsException 발생");
                    break;
                case 16: // 플레이어가 아이템을 구매할 경우 호출, 플레이어 돈, 인벤토리 정보 수정
                    sendList.put("16", "ArrayIndexOutOfBoundsException 발생");
                    break;
                case 18: // 18: 훈련소에서 용병을 구매할 경우 호출
                    sendList.put("20", "ArrayIndexOutOfBoundsException 발생");
                    break;
                case 19: // 18: 훈련소에서 용병을 판매할 경우 호출
                    sendList.put("21", "ArrayIndexOutOfBoundsException 발생");
                    break;
                case 20: // 20: 클라이언트가 채팅메시지를 보낼 경우 호출
                    sendList.put("22", "ArrayIndexOutOfBoundsException 발생");
                    break;
                default:
                    System.out.println("IndexOutOfBoundsException - protocolId: " + protocolId);
                    break;
            }

            // Map -> String 변환 후 전달
            String jdata = gson.toJson(sendList);
            ByteBuf messageBuffer = Unpooled.buffer();
            messageBuffer.writeBytes(jdata.getBytes());
            ctx.writeAndFlush(messageBuffer);
        } catch (Exception e) {
            System.out.println("Exception 발생");
            System.out.println("IndexOutOfBoundsException - protocolId: " + protocolId);
            System.out.println("e.toString(): " + e.toString());
        } finally {
            ReferenceCountUtil.release(msg);
        }
    }

    // 4, ctx.write(Object)메시지를 전선에 기록하지 않습니다. 내부에 버퍼링 된 다음에 의해 와이어로 플러시됩니다 ctx.flush(). 또는 ctx.writeAndFlush(msg)간결성을 요구할 수도 있습니다.
    @Override
    public void channelReadComplete(ChannelHandlerContext ctx) {
        // System.out.println("[SERVER] channelReadComplete 실행");
        ctx.flush();// 채널 파이프라인에 저장된 버퍼 전송
    }

    // 사용자가 나갔을 때 실행
    @Override
    public void channelInactive(ChannelHandlerContext ctx) throws Exception {
        System.out.println("[SERVER] channelInactive 실행");
        channelGroup.remove(ctx.channel());
        ctx.close();
    }

    // 예외처리, 예를 들어, 연결을 닫기 전에 오류 코드와 함께 응답 메시지를 보낼 수 있습니다.
    @Override
    public void exceptionCaught(ChannelHandlerContext ctx, Throwable cause) {
        // System.out.println("[SERVER] exceptionCaught 실행");
        // Close the connection when an exception is raised.

        channelGroup.remove(ctx.channel());
        cause.printStackTrace();
        ctx.close();
    }
}