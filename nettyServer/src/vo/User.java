package vo;

import java.io.Serializable;

public class User implements Serializable {
    private String Id;
    private String Password;

    public String getId() {
        return Id;
    }

    public void setId(String id) {
        Id = id;
    }

    public String getPassword() {
        return Password;
    }

    public void setPassword(String password) {
        Password = password;
    }
}
