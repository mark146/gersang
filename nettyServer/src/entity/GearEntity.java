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
    Map<String, String> sendData = new HashMap<>();

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

    public List<String> GearOpen (String result) {
        //System.out.println("GearOpen: "+result);
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

//                System.out.println("Slot_1: " + rs.getString("Head"));
//                System.out.println("Slot_2: " + rs.getString("Weapon"));
//                System.out.println("Slot_3: " + rs.getString("Body"));
//                System.out.println("Slot_4: " + rs.getString("Waist"));
//                System.out.println("Slot_5: " + rs.getString("Ring_left"));
//                System.out.println("Slot_6: " + rs.getString("Shoes"));
//                System.out.println("Slot_7: " + rs.getString("ring_right"));
            }
        } catch (SQLException e) {
            e.printStackTrace();
        } finally {
            // 6. 연결 해제
            JDBCUtil.close(rs, stmt, conn);
        }
        return gear;
    }

    public Map<String, String> GearUpdate(String userID, List<String> gearList) {
        System.out.println("GearUpdate: "+userID);
        Map<String, String> sendData = new HashMap<String, String>();

        try {
            // 2. Connection 연결(획득)
            conn = JDBCUtil.getConnection();

            // 3. Statement 생성
            // 쿼리 참고: https://sas-study.tistory.com/160
            String sql = "update Gear set Head=?, Weapon=?, "
                    + "Body=?, Waist=?, Ring_left=?, Shoes=?, ring_right=? where UserID=?";
            stmt = conn.prepareStatement(sql);
            for(int i =0; i <gearList.size();i++) {
                stmt.setString(i+1, gearList.get(i).trim());
            }
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
