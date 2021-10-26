package entity;

import vo.User;

import java.sql.*;
import java.time.ZonedDateTime;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

/*
DAO(Data Access Object) 클래스
JDBC(Java Data Base Connectivity)
1.드라이버 로딩
2.컨넥션 획득
3.Statement 객체 생성
4.쿼리전송
5.ResultSet 처리(select만 필요)
6.자원반납
*/
public class UserEntity {
    // JDBC 관련 변수
    Connection conn = null;
    PreparedStatement stmt = null;
    ResultSet rs = null;
    Map<String, String> sendData = new HashMap<>();

    // 1: 로그인
    public Map<String, String> Login(User user) {
        String id = user.getId();
        String password = user.getPassword();
        boolean isResult = false;

        try {
            // 2. Connection 연결(획득)
            conn = JDBCUtil.getConnection();

            // 3. Statement 생성
            // 쿼리 참고: https://sas-study.tistory.com/160
            String sql = "select * from User where Name=?";
            stmt = conn.prepareStatement(sql);
            stmt.setString(1, user.getId().trim());

            // 4. SQL 실행
            rs = stmt.executeQuery();

            // 5. 검색 결과 처리
            while (rs.next()) {
                user.setId(rs.getString("Name"));
                user.setPassword(rs.getString("Password"));
                System.out.println("Name: " + rs.getString("Name"));
                System.out.println("Password: " + rs.getString("Password"));
                isResult = true;
            }

            System.out.println("isResult: " + isResult);
            if (isResult) {
                if (id.equals(user.getId()) == true && password.equals(user.getPassword()) == false) {
                    System.out.println("비밀번호가 일치하지 않습니다.");
                    sendData.put("1", "비밀번호가 일치하지 않습니다.");
                } else if (id.equals(user.getId()) == true && password.equals(user.getPassword()) == true) {
                    System.out.println("로그인에 성공하였습니다.");
                    sendData.put("1", "로그인에 성공하였습니다.");
                }
            } else {
                System.out.println("아이디가 존재하지 않습니다.");
                sendData.put("1", "아이디가 존재하지 않습니다.");
            }

        } catch (SQLException e) {
            e.printStackTrace();
        } finally {
            // 6. 연결 해제
            JDBCUtil.close(rs, stmt, conn);
        }

        isResult = false;
        return sendData;
    }


    // 2: 회원가입창 - 유저 아이디 비교, 비밀번호 암호화 (적용 필요)
    public Map<String, String> Verification(String name) {
        boolean isResult = false;
        String afterName = null;
        System.out.println("UserEntity - Verification: " + name);

        try {
            // 2. Connection 연결(획득)
            conn = JDBCUtil.getConnection();

            // 3. Statement 생성
            // 쿼리 참고: https://sas-study.tistory.com/160
            String sql = "select * from User where Name=?";
            stmt = conn.prepareStatement(sql);
            stmt.setString(1, name.trim());

            // 4. SQL 실행
            rs = stmt.executeQuery();

            // 5. 검색 결과 처리
            while (rs.next()) {
                isResult = true;
                afterName = rs.getString("Name");
                System.out.println("Name: " + rs.getString("Name"));
            }

            System.out.println("isResult: " + isResult);
            if (isResult) {
                System.out.println("--- 검색 후 --");
                System.out.println("id: " + afterName);
                System.out.println("-----------------");

                if (name.equals(afterName) == true) {
                    System.out.println("존재하는 닉네임 입니다.");
                    sendData.put("2", "존재하는 닉네임 입니다.");
                } else {
                    System.out.println("사용가능한 닉네임 입니다.");
                    sendData.put("2", "사용가능한 닉네임 입니다.");
                }

            } else {
                System.out.println("사용가능한 닉네임 입니다.");
                sendData.put("2", "사용가능한 닉네임 입니다.");
            }

        } catch (SQLException e) {
            e.printStackTrace();
        } finally {
            // 6. 연결 해제
            JDBCUtil.close(rs, stmt, conn);
        }

        return sendData;
    }

    // 3: 회원가입창 - 회원가입
    public Map<String, String> Register(User user) {
        try {
            // 2. Connection 연결(획득)
            conn = JDBCUtil.getConnection();

            // 3. Statement 생성
            // 쿼리 참고: https://sas-study.tistory.com/160
            String sql = "insert into User(Name, Password) values(?,?)";
            stmt = conn.prepareStatement(sql);
            stmt.setString(1, user.getId().trim());
            stmt.setString(2, user.getPassword().trim());

            // 4. SQL 실행
            stmt.executeUpdate();
            System.out.println("회원가입이 완료되었습니다.");
            sendData.put("3", "회원가입이 완료되었습니다.");

        } catch (SQLException e) {
            e.printStackTrace();
        } finally {
            // 6. 연결 해제
            JDBCUtil.close(rs, stmt, conn);
        }

        return sendData;
    }


    // 10: 캐릭터 생성창 - 닉네임 중복 체크
    public Map<String, String> NickNameCheck(String playerId) {
        System.out.println("playerIdCheck: "+playerId);
        Map<String, String> sendData = new HashMap<String, String>();
        boolean isResult = false;
        String afterName = null;

        try {
            // 2. Connection 연결(획득)
            conn = JDBCUtil.getConnection();

            // 3. Statement 생성
            // 쿼리 참고: https://sas-study.tistory.com/160
            String sql = "select * from Player where Name=?";
            stmt = conn.prepareStatement(sql);
            stmt.setString(1, playerId.trim());

            // 4. SQL 실행
            rs = stmt.executeQuery();

            // 5. 검색 결과 처리
            while (rs.next()) {
                isResult = true;
                afterName = rs.getString("Name");
                System.out.println("Name: " + rs.getString("Name"));
            }

            System.out.println("isResult: " + isResult);
            if (isResult) {
                System.out.println("--- 검색 후 --");
                System.out.println("id: " + afterName);
                System.out.println("-----------------");

                if (playerId.trim().equals(afterName) == true) {
                    System.out.println("존재하는 닉네임 입니다.");
                    sendData.put("10", "존재하는 닉네임 입니다.");
                } else {
                    System.out.println("사용가능한 닉네임 입니다.");
                    sendData.put("10", "사용가능한 닉네임 입니다.");
                }

            } else {
                System.out.println("사용가능한 닉네임 입니다.");
                sendData.put("10", "사용가능한 닉네임 입니다.");
            }

        } catch (SQLException e) {
            e.printStackTrace();
        } finally {
            // 6. 연결 해제
            JDBCUtil.close(rs, stmt, conn);
        }

        return sendData;
    }

    // 11: 캐릭터 생성창 - 캐릭터 생성
    public Map<String, String> PlayerCreate(List<String> nameList) {
        System.out.println("PlayerCreate: "+nameList.get(0));
        Map<String, String> sendData = new HashMap<String, String>();
        ZonedDateTime zdateTime = ZonedDateTime.now();
        DateTimeFormatter formatter = DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss");
        System.out.println("formatter: "+zdateTime.format(formatter));

        try {
            // 2. Connection 연결(획득)
            conn = JDBCUtil.getConnection();

            // 3. Statement 생성
            // 쿼리 참고: https://sas-study.tistory.com/160
            String sql = "insert into Player(UserID, Name, Gender, Lv, MaxLv, "
                    + "Money, HP, MaxHP, MP, MaxMP, EXP, MaxEXP,Attack,Defense, STR, DEX, CON, WIS, CreateDate) "
                    + "values(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";

            stmt = conn.prepareStatement(sql);
            stmt.setString(1, nameList.get(0).trim());
            stmt.setString(2, nameList.get(1).trim());
            stmt.setString(3, nameList.get(2).trim());
            stmt.setString(4, "1");
            stmt.setString(5, "250");
            stmt.setString(6, "10000");
            stmt.setString(7, "200");
            stmt.setString(8, "200");
            stmt.setString(9, "100");
            stmt.setString(10, "100");
            stmt.setString(11, "0");
            stmt.setString(12, "100");
            stmt.setString(13, "10");
            stmt.setString(14, "5");
            stmt.setString(15, "10");
            stmt.setString(16, "10");
            stmt.setString(17, "10");
            stmt.setString(18, "10");
            stmt.setString(19, zdateTime.format(formatter));

            // 4. SQL 실행
            stmt.executeUpdate();
            System.out.println("캐릭터 생성이 완료되었습니다.");
            sendData.put("11", "캐릭터 생성이 완료되었습니다.");

        } catch (SQLException e) {
            e.printStackTrace();
        } finally {
            // 6. 연결 해제
            JDBCUtil.close(rs, stmt, conn);
        }

        return sendData;
    }

    // 12: 캐릭터 선택창 - 캐릭터 조회
    public Map<String, List<String>> PlayerCheck(String playerId) {
        System.out.println("PlayerCheck: "+playerId);
        Map<String, List<String>> sendData = new HashMap<String, List<String>>();
        List<String> player = new ArrayList<String>();
        boolean isResult = false;

        try {
            // 2. Connection 연결(획득)
            conn = JDBCUtil.getConnection();

            // 3. Statement 생성
            // 쿼리 참고: https://sas-study.tistory.com/160
            String sql = "select * from Player where UserID=?";
            stmt = conn.prepareStatement(sql);
            stmt.setString(1, playerId.trim());

            // 4. SQL 실행
            rs = stmt.executeQuery();

            // 5. 검색 결과 처리
            while (rs.next()) {
                isResult = true;

                player.add(rs.getString("UserID"));
                player.add(rs.getString("Name"));
                player.add(rs.getString("Gender"));
                player.add(rs.getString("Lv"));
                player.add(rs.getString("MaxLv"));
                player.add(rs.getString("Money"));
                player.add(rs.getString("HP"));
                player.add(rs.getString("MaxHP"));
                player.add(rs.getString("MP"));
                player.add(rs.getString("MaxMP"));
                player.add(rs.getString("EXP"));
                player.add(rs.getString("MaxEXP"));
                player.add(rs.getString("Attack"));
                player.add(rs.getString("Defense"));
                player.add(rs.getString("STR"));
                player.add(rs.getString("DEX"));
                player.add(rs.getString("CON"));
                player.add(rs.getString("WIS"));
            }

            System.out.println("isResult: " + isResult);
            if (isResult) {
                sendData.put("12", player);
            } else {
                player.add("유저 정보가 존재하지 않습니다.");
                sendData.put("12", player);
            }
        } catch (SQLException e) {
            e.printStackTrace();
        } finally {
            // 6. 연결 해제
            JDBCUtil.close(rs, stmt, conn);
        }

        return sendData;
    }
    
    // 13: 캐릭터 선택창 - 캐릭터 삭제
    public Map<String, String> PlayerDelete(List<String> playerList) {
        System.out.println("PlayerDelete: "+playerList.get(0));
        Map<String, String> sendData = new HashMap<String, String>();

        try {
            // 2. Connection 연결(획득)
            conn = JDBCUtil.getConnection();

            // 3. Statement 생성
            // 쿼리 참고: https://sas-study.tistory.com/160
            String sql = "delete from Player where NickName=?";
            stmt = conn.prepareStatement(sql);
            stmt.setString(1, playerList.get(0).trim());

            // 4. SQL 실행
            stmt.executeUpdate();
            sendData.put("13", "캐릭터 삭제가 완료되었습니다.");

        } catch (SQLException e) {
            e.printStackTrace();
        } finally {
            // 6. 연결 해제
            JDBCUtil.close(rs, stmt, conn);
        }

        return sendData;
    }

    // 16: 캐릭터 돈 수정
    public void PlayerGoldUpdate(List<String> sellList) {
       /*
        0: 플레이어 명
		1: 아이템 구매 후 계산된 플레이어 돈
		2: 인벤토리 슬롯 번호
		3: 아이템 명
       */
        try {
            // 2. Connection 연결(획득)
            conn = JDBCUtil.getConnection();

            // 3. Statement 생성 - 쿼리 참고: https://sas-study.tistory.com/160
            String sql = "update Player set Money=? where Name=?";

            stmt = conn.prepareStatement(sql);
            stmt.setString(1, sellList.get(1).trim());
            stmt.setString(2, sellList.get(0).trim());

            // 4. SQL 실행
            stmt.executeUpdate();

            System.out.println("플레이어 소지금 수정 완료");
        } catch (SQLException e) {
            e.printStackTrace();
        } finally {
            // 6. 연결 해제
            JDBCUtil.close(rs, stmt, conn);
        }
    }

}
