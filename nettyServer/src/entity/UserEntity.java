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

            // 3. Statement 생성 - 쿼리 참고: https://sas-study.tistory.com/160
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
        User user = new User();
        user.setId(name);
        System.out.println("UserEntity - Verification - id: " + user.getId().trim());

        try {
            // 2. Connection 연결(획득)
            conn = JDBCUtil.getConnection();

            // 3. Statement 생성 - 쿼리 참고: https://sas-study.tistory.com/160
            String sqlplus = "select * from User where Name=?";
            stmt = conn.prepareStatement(sqlplus);
            stmt.setString(1, user.getId().trim());

            // 4. SQL 실행
            rs = stmt.executeQuery();

            // 5. 검색 결과 처리
            while (rs.next()) {
                System.out.println("Name: " + rs.getString("Name"));
                System.out.println("Password: " + rs.getString("Password"));
                afterName = rs.getString("Name");
                isResult = true;
            }

            System.out.println("isResult: " + isResult+", afterName: "+afterName+", Verification - id: " + user.getId().trim());
            if (isResult) {
                System.out.println("--- 검색 후 --");
                System.out.println("id: " + afterName);
                System.out.println("-----------------");

                if (name.equals(afterName) == true) {
//                    System.out.println("존재하는 닉네임 입니다.");
                    sendData.put("2", "존재하는 닉네임 입니다.");
                } else {
//                    System.out.println("사용가능한 닉네임 입니다.");
                    sendData.put("2", "사용가능한 닉네임 입니다.");
                }
            } else {
//                System.out.println("사용가능한 닉네임 입니다.");
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

            // 3. Statement 생성 - 쿼리 참고: https://sas-study.tistory.com/160
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

            // 3. Statement 생성 - 쿼리 참고: https://sas-study.tistory.com/160
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

            // 3. Statement 생성 - 쿼리 참고: https://sas-study.tistory.com/160
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

            // 3. Statement 생성 - 쿼리 참고: https://sas-study.tistory.com/160
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

            // 3. Statement 생성 - 쿼리 참고: https://sas-study.tistory.com/160
            String sql = "delete from Player where Name=?";
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


    // 22: 플레이어 정보 수정
    public void PlayerUpdate(List<String> playerInfo) {
        /**
         [player 정보] - 캐릭터 정보 연동
         UserID - 번호: 0 , 값: test
         Name - 번호: 1 , 값: test1
         Gender - 번호: 2 , 값: 남자 캐릭터
         Lv - 번호: 3 , 값: 1
         MaxLv - 번호: 4 , 값: 250
         Money - 번호: 5 , 값: 10000
         HP - 번호: 6 , 값: 200
         MaxHP - 번호: 7 , 값: 200
         MP - 번호: 8 , 값: 100
         MaxMP - 번호: 9 , 값: 100
         EXP - 번호: 10 , 값: 0
         MaxEXP - 번호: 11 , 값: 100
         Attack - 번호: 12 , 값: 10
         Defense - 번호: 13 , 값: 5
         STR - 번호: 14 , 값: 10
         DEX - 번호: 15 , 값: 10
         CON - 번호: 16 , 값: 10
         WIS - 번호: 17 , 값: 10
         */
//        for (int i = 0; i < playerInfo.size(); i++){
//            System.out.println("playerInfo["+i+"] - "+playerInfo.get(i));
//        }

        try {
            // 2. Connection 연결(획득)
            conn = JDBCUtil.getConnection();

            // 3. Statement 생성 - 쿼리 참고: https://allg.tistory.com/23
            String sql = "update Player set Lv=?, MaxLv=?, HP=?, MaxHP=?, MP=?, MaxMP=?, EXP=?, MaxEXP=?,Attack=?,Defense=?, STR=?, DEX=?, CON=?, WIS=? where Name=?";

            stmt = conn.prepareStatement(sql);
            stmt.setString(1, playerInfo.get(3).trim());
            stmt.setString(2, playerInfo.get(4).trim());
            stmt.setString(3, playerInfo.get(6).trim());
            stmt.setString(4, playerInfo.get(7).trim());
            stmt.setString(5, playerInfo.get(8).trim());
            stmt.setString(6, playerInfo.get(9).trim());
            stmt.setString(7, playerInfo.get(10).trim());
            stmt.setString(8, playerInfo.get(11).trim());
            stmt.setString(9, playerInfo.get(12).trim());
            stmt.setString(10, playerInfo.get(13).trim());
            stmt.setString(11, playerInfo.get(14).trim());
            stmt.setString(12, playerInfo.get(15).trim());
            stmt.setString(13, playerInfo.get(16).trim());
            stmt.setString(14, playerInfo.get(17).trim());
            stmt.setString(15, playerInfo.get(1).trim());

            // 4. SQL 실행
            stmt.executeUpdate();

            System.out.println("플레이어 정보 수정 완료");
        } catch (SQLException e) {
            e.printStackTrace();
        } finally {
            // 6. 연결 해제
            JDBCUtil.close(rs, stmt, conn);
        }
    }
}