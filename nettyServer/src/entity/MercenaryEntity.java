package entity;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;

public class MercenaryEntity {
    // JDBC 관련 변수
    Connection conn = null;
    PreparedStatement stmt = null;
    ResultSet rs = null;

    // 17: 캐릭터 용병 조회
    public List<String> MercenaryCheck(String name) {
        System.out.println("MercenaryCheck: "+name);
        List<String> unitList = new ArrayList<String>();

        try {
            // 2. Connection 연결(획득)
            conn = JDBCUtil.getConnection();

            // 3. Statement 생성
            // 쿼리 참고: https://sas-study.tistory.com/160
            String sql = "select * from Mercenary where PlayerID=?";
            stmt = conn.prepareStatement(sql);
            stmt.setString(1, name.trim());

            // 4. SQL 실행
            rs = stmt.executeQuery();

            // 5. 검색 결과 처리
            while (rs.next()) {
                unitList.add(rs.getString("Name"));
                unitList.add(rs.getString("Job"));
                unitList.add(rs.getString("Lv"));
                unitList.add(rs.getString("MaxLv"));
                unitList.add(rs.getString("HP"));
                unitList.add(rs.getString("MaxHP"));
                unitList.add(rs.getString("MP"));
                unitList.add(rs.getString("MaxMP"));
                unitList.add(rs.getString("EXP"));
                unitList.add(rs.getString("MaxEXP"));
                unitList.add(rs.getString("Attack"));
                unitList.add(rs.getString("Defense"));
                unitList.add(rs.getString("STR"));
                unitList.add(rs.getString("DEX"));
                unitList.add(rs.getString("CON"));
                unitList.add(rs.getString("WIS"));
            }

            // 용병이 존재하지 않을 경우
            if(unitList.size() == 0) {
                unitList.add("용병이 존재하지 않습니다.");
            }

        } catch (SQLException e) {
            e.printStackTrace();
        } finally {
            // 6. 연결 해제
            JDBCUtil.close(rs, stmt, conn);
        }

        return unitList;
    }


    public List<String> MercenaryBuy(String playerId, String mercenaryName) {
        List<String> unitList = new ArrayList<String>();

        // 전달 받을 정보: 플레이어 id, 돈, 용병 이름
        try {
            // 2. Connection 연결(획득)
            conn = JDBCUtil.getConnection();

            // 3. Statement 생성
            // 쿼리 참고: https://sas-study.tistory.com/160
            String sql = "insert into Mercenary(PlayerID, Name, Job, Lv, MaxLv, "
                    + "HP, MaxHP, MP, MaxMP, EXP, "
                    + "MaxEXP, Attack, Defense, STR, DEX, CON, WIS) "
                    + "values(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";

            stmt = conn.prepareStatement(sql);
            stmt.setString(1, playerId); // PlayerID
            stmt.setString(2, mercenaryName.trim()); // Name
            stmt.setString(3, mercenaryName.trim()); // Job
            stmt.setString(4, "1"); // Lv
            stmt.setString(5, "250"); // MaxLv
            stmt.setString(6, "200"); // HP
            stmt.setString(7, "200"); // MaxHP
            stmt.setString(8, "200"); // MP
            stmt.setString(9, "200"); // MaxMP
            stmt.setString(10, "0"); // EXP
            stmt.setString(11, "100"); // MaxEXP
            stmt.setString(12, "10"); // Attack
            stmt.setString(13, "5"); // Defense
            stmt.setString(14, "10"); // STR
            stmt.setString(15, "10"); // DEX
            stmt.setString(16, "10"); // CON
            stmt.setString(17, "10"); // WIS

            // 4. SQL 실행
            stmt.executeUpdate();
            System.out.println("용병 생성이 완료되었습니다.");
            
            // 용병 정보 저장
            unitList.add(playerId);
            unitList.add(mercenaryName);
            unitList.add(mercenaryName);
            unitList.add("1");
            unitList.add("250");
            unitList.add("200");
            unitList.add("200");
            unitList.add("200");
            unitList.add("200");
            unitList.add("0");
            unitList.add("100");
            unitList.add("10");
            unitList.add("5");
            unitList.add("10");
            unitList.add("10");
            unitList.add("10");
            unitList.add("10");
            
        } catch (SQLException e) {
            e.printStackTrace();
        } finally {
            // 6. 연결 해제
            JDBCUtil.close(rs, stmt, conn);
        }

        return unitList;
    }

    public void MercenarySell(String playerId, String mercenaryName) {
        try {
            // 2. Connection 연결(획득)
            conn = JDBCUtil.getConnection();

            // 3. Statement 생성
            // 쿼리 참고: https://sas-study.tistory.com/160
            String sql = "delete from Mercenary where PlayerID=? AND Job=? ";
            stmt = conn.prepareStatement(sql);
            stmt.setString(1, playerId.trim());
            stmt.setString(2, mercenaryName.trim());

            // 4. SQL 실행
            stmt.executeUpdate();
            System.out.println("캐릭터 삭제가 완료되었습니다.");

        } catch (SQLException e) {
            e.printStackTrace();
        } finally {
            // 6. 연결 해제
            JDBCUtil.close(rs, stmt, conn);
        }

    }
}
