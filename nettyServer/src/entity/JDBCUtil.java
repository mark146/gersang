package entity;

import java.sql.*;


public class JDBCUtil {

    public static Connection getConnection() {
        try {
            // 1. 드라이버 로딩
            Class.forName("com.mysql.cj.jdbc.Driver");

            // 2. Connection 연결(획득)
            String jdbcUrl="jdbc:mysql://192.168.56.1:3306/gersang?characterEncoding=UTF-8&serverTimezone=UTC";
            String id="root";
            String password="root";
            return DriverManager.getConnection(jdbcUrl, id, password);
        } catch (Exception e) {
            e.printStackTrace();
        }
        return null;
    }


    // 커넥션 연결 해제
    public static void close(ResultSet rs, Statement stmt, Connection conn) {
        try {
            if(rs != null)
                rs.close();
        } catch (SQLException e) {
            e.printStackTrace();
        } finally {
            rs = null;
        }

        try {
            if(stmt != null)
                stmt.close();
        } catch (SQLException e) {
            e.printStackTrace();
        } finally {
            stmt = null;
        }

        try {
            if(!conn.isClosed() && conn != null)
                conn.close();
        } catch (SQLException e) {
            e.printStackTrace();
        } finally {
            conn = null;
        }
    }
}