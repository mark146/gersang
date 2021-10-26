package network;

import io.netty.bootstrap.ServerBootstrap;
import io.netty.channel.*;
import io.netty.channel.nio.NioEventLoopGroup;
import io.netty.channel.socket.SocketChannel;
import io.netty.channel.socket.nio.NioServerSocketChannel;


public class EchoServer {
    static final int PORT = Integer.parseInt(System.getProperty("port", "8007"));
    final static EchoServerHandler serverHandler = new EchoServerHandler();


    public static void main(String[] args) throws Exception {

        // EventLoopGroup: I / O 작업을 처리하는 멀티 스레드 이벤트 루프
        // '보스'라고 불리는 첫 번째 것은 들어오는 연결을 수락합니다.
        // bossGroup 클라이언트의 연결을 수락하는 부모 스레드 그룹
        // NioEventLoopGroup(인수) 스레드 그룹 내에서 생성할 최대 스레드 수 1이므로 단일 스레드
        // NioEventLoopGroup 클래스 생성자의 인수로 사용된 숫자는 스레드 그룹 내에서 생성할 최대 스레드 수를 의미한다.
        // boss 그룹의 스레드 갯수가 1이라면 포트를 여러 개 열었다고 해서 스레드를 여러 개 생성하지 않습니다. 하나의 스레드가 두 개의 포트를 listen 합니다.
        // 만약 boss 그룹의 스레드 갯수를 2로 지정했다면 포트 하나당 스레드 하나를 사용하게 되겠지요.
        EventLoopGroup bossGroup = new NioEventLoopGroup(1);

        // 연결된 클라이언트 소켓으로부터 데이터 입출력(I/O) 및 이벤트처리를 담당하는 자식 쓰레드 그룹
        // 생성자에 인수가 없으므로 CPU 코어 수에 따른 쓰레드의 수가 설정된다.
        // '작업자'라고하며, 보스가 연결을 수락하고 승인 된 연결을 작업자에게 등록하면 허용 된 연결의 트래픽을 처리합니다.
        EventLoopGroup workerGroup = new NioEventLoopGroup();

        // 사용되는 스레드 수와 생성 된 스레드에 매핑되는 방법 Channel은 EventLoopGroup구현에 따라 다르며 생성자를 통해 구성 할 수도 있습니다.
        // NioEventLoopGroup 클래스의 인수 없는 생성자를 호출했다.
        // 인수 없는 생성자는 사용할 스레드 수를 서버 애플리케이션이 동작하는 하드웨어 코어 수를 기준으로 결정한다.
        try {
            // ServerBootstrap: 서버를 설정하는 도우미 클래스
            // 부트 스트랩 객체 생성
            ServerBootstrap b = new ServerBootstrap();
            b.group(bossGroup, workerGroup)// 스레드 그룹 초기화
                    // group, Channel과 같은 메서드로 객체를 초기화
                    // b.group ServerBootStrap 객체에 서버 어플리케이션이 사용할 두 스레드 그룹을 설정
                    // 첫번째 인수는 부모 스레드이다. 부모 스레드는 클라이언트 연결 요청의 수락을 담당한다.
                    // 두번째 인수는 연결된 소켓에 대한 I/O 처리를 담당하는 자식 스레드이다.
                    .channel(NioServerSocketChannel.class)
                    // 여기서는 들어오는 연결을 수락하기 NioServerSocketChannel위해 새 인스턴스를 만드는 데 사용되는 클래스 를 사용하도록 지정, 채널 초기화
                    // 서버 소켓이 사용할 네트워크 입출력 모드를 설정한다. 여기서는 NioServerSocketChannel 클래스로 설정했기 때문에 NIO 모드로 동작한다.
                    .option(ChannelOption.CONNECT_TIMEOUT_MILLIS, 3000)// 자동 연결 해제 시간 설정
                    .option(ChannelOption.SO_BACKLOG, 300) // 동시에 수용할 클라이언트의 연결 요청 수
                    .childOption(ChannelOption.TCP_NODELAY, true) // [5] 반응속도를 높이기 위해 Nagle 알고리즘을 비활성화 합니다.
                    .childOption(ChannelOption.SO_LINGER, 0) // [6] 소켓이 close될 때 신뢰성있는 종료를 위해 4way-handshake가 발생하고 이때 TIME_WAIT로 리소스가 낭비됩니다. 이를 방지하기 위해 0으로 설정합니다.
                    .childOption(ChannelOption.SO_KEEPALIVE, true) // [7] Keep-alive를 켭니다.
                    .childOption(ChannelOption.SO_REUSEADDR, true) // [8] SO_LINGER설정이 있으면 안해도 되나 혹시나병(!)으로 TIME_WAIT걸린 포트를 재사용할 수 있도록 설정합니다.
                    .childHandler(new ChannelInitializer<SocketChannel>() {
                        // 여기에 지정된 핸들러는 항상 새로 승인 된 것으로 평가됩니다. ChannelInitializer: 사용자 구성에 새로운 도움을 작정하는 특별한 핸들러, 자식 채널의 초기화
                        //ChannelInitializer는 클라이언트로부터 연결된 채널이 초기화 될 때의 기본 동작이 지정된 추상 클래스다.
                        // initChannel은 클라이언트 소켓 채널이 생성될 때 호출됨
                        @Override
                        public void initChannel(SocketChannel ch) throws Exception {
                            System.out.println("initChannel");
                            // 채널 파이프라인의 객체 생성, initChannel은 클라이언트 소켓 채널이 생성될 때 호출됨
                            // initChannel 메서드는 클라이언트 소켓 채널이 생성될 때 자동으로 호출 된다. 이때 채널 파이프라인의 설정을 수행하게 된다.
                            // 채널 파이프라인에 EchoServerHandler 클래스를 동록하여 클라이언트와 연결이 되었을 때 데이터 처리를 담당한다.
                            // EchoServerHandler 클래스는 이후에 클라이언트의 연결이 생성되었을 때 데이터 처리를 담당한다.
                            // 접속된 클라이언트로부터 수신된 데이터를 처리할 핸들러를 지정한다.
                            ChannelPipeline p = ch.pipeline();
                            p.addLast(serverHandler);
                        }
                    });

            // 서버를 비동기식으로 바인딩후 채널의 closeFuture 을 얻고 완료될때까지 현재 스레드를 블록킹.
            // 들어오는 연결을 바인딩하고 수락하기 시작합니다.
            // 8007 머신의 모든 NIC (네트워크 인터페이스 카드) 포트 에 바인딩 합니다.
            // 부트스트랩 클래스의 bind 메서드로 접속할 포트를 지정한다.
            ChannelFuture future = b.bind(PORT).sync();
            future.channel().closeFuture().sync(); // 서버 소켓이 닫힐 때까지 기다립니다.
        }  catch (Exception e) {
            e.printStackTrace();
        } finally {
            // Shut down all event loops to terminate all threads.
            bossGroup.shutdownGracefully();
            workerGroup.shutdownGracefully();
        }
    }
}