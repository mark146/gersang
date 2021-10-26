package entity;

import java.sql.*;

//참고: http://blog.naver.com/PostView.nhn?blogId=50after&logNo=220912861796&parentCategoryNo=&categoryNo=&viewDate=&isShowPopularPosts=false&from=postView
public class JDBCUtil {

    public static Connection getConnection() {
        try {
            // 1. 드라이버 로딩
            // DriverManager.registerDriver(new Driver());
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

    public static void close(ResultSet rs, Statement stmt, Connection conn) {
        // 6. 연결 해제
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

    public static void close(Statement stmt, Connection conn) {
        // 6. 연결 해제
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
