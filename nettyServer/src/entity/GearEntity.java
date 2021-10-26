package entity;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;


public class GearEntity {
    // JDBC 관련 변수
    Connection conn = null;
    PreparedStatement stmt = null;
    ResultSet rs = null;


    // 장비 생성
    public void GearCreate(String nickName) {
        try {
            // 2. Connection 연결(획득)
            conn = JDBCUtil.getConnection();

            // 3. Statement 생성
            // 쿼리 참고: https://sas-study.tistory.com/160
            String sql = "INSERT INTO Gear (UserID, Head, Weapon, Body, Waist, Ring_left, Shoes, ring_right) VALUES (?,'없음','없음','없음','없음','없음','없음','없음')";
            stmt = conn.prepareStatement(sql);
            stmt.setString(1, nickName.trim());

            // 4. SQL 실행
            stmt.executeUpdate();
            System.out.println(nickName+" 장비창이 생성 되었습니다.");
        } catch (SQLException e) {
            e.printStackTrace();
        } finally {
            // 6. 연결 해제
            JDBCUtil.close(rs, stmt, conn);
        }
    }


    // 장비 제거
    public void GearDelete(String nickName) {
        try {
            // 2. Connection 연결(획득)
            conn = JDBCUtil.getConnection();

            // 3. Statement 생성
            // 쿼리 참고: https://sas-study.tistory.com/160
            String sql = "delete from Gear where UserID=?";
            stmt = conn.prepareStatement(sql);
            stmt.setString(1, nickName.trim());

            // 4. SQL 실행
            stmt.executeUpdate();

            System.out.println("장비창 삭제가 완료되었습니다.");
        } catch (SQLException e) {
            e.printStackTrace();
        } finally {
            // 6. 연결 해제
            JDBCUtil.close(rs, stmt, conn);
        }
    }


    // 장비 정보 조회
    public List<String> GearOpen(String result) {
        List<String> gear = new ArrayList<String>();

        try {
            // 2. Connection 연결(획득)
            conn = JDBCUtil.getConnection();

            // 3. Statement 생성 - 쿼리 참고: https://sas-study.tistory.com/160
            String sql = "select * from Gear where UserID=?";
            stmt = conn.prepareStatement(sql);
            stmt.setString(1, result.trim());

            // 4. SQL 실행
            rs = stmt.executeQuery();

            // 5. 검색 결과 처리
            while (rs.next()) {
                gear.add(rs.getString("Head"));
                gear.add(rs.getString("Weapon"));
                gear.add(rs.getString("Body"));
                gear.add(rs.getString("Waist"));
                gear.add(rs.getString("Ring_left"));
                gear.add(rs.getString("Shoes"));
                gear.add(rs.getString("ring_right"));
            }
        } catch (SQLException e) {
            e.printStackTrace();
        } finally {
            // 6. 연결 해제
            JDBCUtil.close(rs, stmt, conn);
        }
        return gear;
    }


    // 장비 정보 수정
    public Map<String, String> GearUpdate(String userID, List<String> gearList) {
        System.out.println("GearUpdate: "+userID);
        Map<String, String> sendData = new HashMap<String, String>();

//        for(int i =0; i<gearList.size();i++) {
//            System.out.println("GearUpdate["+i+"]: "+gearList.get(i));
//        }

        try {
            // 2. Connection 연결(획득)
            conn = JDBCUtil.getConnection();

            // 3. Statement 생성 - 쿼리 참고: https://sas-study.tistory.com/160
            String sql = "update Gear set Head=?, Weapon=?, "
                    + "Body=?, Waist=?, Ring_left=?, Shoes=?, ring_right=? where UserID=?";
            stmt = conn.prepareStatement(sql);
            stmt.setString(1, gearList.get(0).trim());
            stmt.setString(2, gearList.get(1).trim());
            stmt.setString(3, gearList.get(2).trim());
            stmt.setString(4, gearList.get(3).trim());
            stmt.setString(5, gearList.get(4).trim());
            stmt.setString(6, gearList.get(5).trim());
            stmt.setString(7, gearList.get(6).trim());
            stmt.setString(8, userID.trim());

            // 4. SQL 실행
            stmt.executeUpdate();
            sendData.put("9", "장비 정보가 수정되었습니다.");
        } catch (SQLException e) {
            e.printStackTrace();
        } finally {
            // 6. 연결 해제
            JDBCUtil.close(rs, stmt, conn);
        }
        return sendData;
    }
}