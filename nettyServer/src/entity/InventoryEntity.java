package entity;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;


public class InventoryEntity {
    // JDBC 관련 변수
    Connection conn = null;
    PreparedStatement stmt = null;
    ResultSet rs = null;


    public void InventoryCreate(String nickName) {
        try {
            // 2. Connection 연결(획득)
            conn = JDBCUtil.getConnection();

            // 3. Statement 생성 - 쿼리 참고: https://sas-study.tistory.com/160
            String sql = "INSERT INTO Inventory (UserID, Slot_1, Slot_2, Slot_3, Slot_4, Slot_5, Slot_6, Slot_7, Slot_8, Slot_9, Slot_10, Slot_11, Slot_12) "
                    + "VALUES (?,'없음','없음','없음','없음','없음','없음','없음','없음','없음','없음','없음', '없음')";
            stmt = conn.prepareStatement(sql);
            stmt.setString(1, nickName.trim());

            // 4. SQL 실행
            stmt.executeUpdate();
            System.out.println(nickName+" 인벤토리가 생성 되었습니다.");
        } catch (SQLException e) {
            e.printStackTrace();
        } finally {
            // 6. 연결 해제
            JDBCUtil.close(rs, stmt, conn);
        }
    }


    public void InventoryDelete(String nickName) {
        try {
            // 2. Connection 연결(획득)
            conn = JDBCUtil.getConnection();

            // 3. Statement 생성 - 쿼리 참고: https://sas-study.tistory.com/160
            String sql = "delete from Inventory where UserID=?";
            stmt = conn.prepareStatement(sql);
            stmt.setString(1, nickName.trim());

            // 4. SQL 실행
            stmt.executeUpdate();

            System.out.println("인벤토리 삭제가 완료되었습니다.");
        } catch (SQLException e) {
            e.printStackTrace();
        } finally {
            // 6. 연결 해제
            JDBCUtil.close(rs, stmt, conn);
        }
    }


    public List<String> InventoryOpen(String userID) {
        List<String> inven = new ArrayList<String>();

        try {
            // 2. Connection 연결(획득)
            conn = JDBCUtil.getConnection();

            // 3. Statement 생성 - 쿼리 참고: https://sas-study.tistory.com/160
            String sql = "select * from Inventory where UserID=?";
            stmt = conn.prepareStatement(sql);
            stmt.setString(1, userID.trim());

            // 4. SQL 실행
            rs = stmt.executeQuery();

            // 5. 검색 결과 처리
            while (rs.next()) {
                inven.add(rs.getString("Slot_1"));
                inven.add(rs.getString("Slot_2"));
                inven.add(rs.getString("Slot_3"));
                inven.add(rs.getString("Slot_4"));
                inven.add(rs.getString("Slot_5"));
                inven.add(rs.getString("Slot_6"));
                inven.add(rs.getString("Slot_7"));
                inven.add(rs.getString("Slot_8"));
                inven.add(rs.getString("Slot_9"));
                inven.add(rs.getString("Slot_10"));
                inven.add(rs.getString("Slot_11"));
                inven.add(rs.getString("Slot_12"));
            }
        } catch (SQLException e) {
            e.printStackTrace();
        } finally {
            // 6. 연결 해제
            JDBCUtil.close(rs, stmt, conn);
        }
        return inven;
    }


    // 인벤토리 수정
    public Map<String, String> InventoryUpdate(String userID, List<String> invenList) {
        System.out.println("InventoryUpdate - 유저 아이디: "+userID);
        Map<String, String> sendData = new HashMap<String, String>();

        try {
            // 2. Connection 연결(획득)
            conn = JDBCUtil.getConnection();

            // 3. Statement 생성 - 쿼리 참고: https://sas-study.tistory.com/160
            String sql = "update Inventory set Slot_1=?, Slot_2=?, "
                    + "Slot_3=?, Slot_4=?, Slot_5=?, Slot_6=?, Slot_7=?, Slot_8=?, "
                    + "Slot_9=?, Slot_10=?, Slot_11=?, Slot_12=? where UserID=?";
            stmt = conn.prepareStatement(sql);
            for(int i =0; i <invenList.size();i++) {
                stmt.setString(i+1, invenList.get(i).trim());
            }
            stmt.setString(13, userID.trim());

            // 4. SQL 실행
            stmt.executeUpdate();

            sendData.put("8", "인벤토리가 수정되었습니다.");
        } catch (SQLException e) {
            e.printStackTrace();
        } finally {
            // 6. 연결 해제
            JDBCUtil.close(rs, stmt, conn);
        }
        return sendData;
    }


    // 인벤토리 슬롯 수정
    public void InventorySlotUpdate(List<String> sellList) {
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
            String sql = "update Inventory set ";
            switch (sellList.get(2)) {
                case "0":
                    sql += "Slot_1=?";
                    break;
                case "1":
                    sql += "Slot_2=?";
                    break;
                case "2":
                    sql += "Slot_3=?";
                    break;
                case "3":
                    sql += "Slot_4=?";
                    break;
                case "4":
                    sql += "Slot_5=?";
                    break;
                case "5":
                    sql += "Slot_6=?";
                    break;
                case "6":
                    sql += "Slot_7=?";
                    break;
                case "7":
                    sql += "Slot_8=?";
                    break;
                case "8":
                    sql += "Slot_9=?";
                    break;
                case "9":
                    sql += "Slot_10=?";
                    break;
                case "10":
                    sql += "Slot_11=?";
                    break;
                case "11":
                    sql += "Slot_12=?";
                    break;
                default:
                    System.out.println("인벤토리 슬롯 수정 예외사항: "+sellList.get(2));
                    break;
            }
            sql += " where UserID=?";
            stmt = conn.prepareStatement(sql);
            stmt.setString(1, sellList.get(3).trim());
            stmt.setString(2, sellList.get(0).trim());

            // 4. SQL 실행
            stmt.executeUpdate();

            System.out.println("인벤토리 슬롯 수정 완료");
            sql = null;
        } catch (Exception e) {
            e.printStackTrace();
        } finally {
            // 6. 연결 해제
            JDBCUtil.close(rs, stmt, conn);
        }
    }
}