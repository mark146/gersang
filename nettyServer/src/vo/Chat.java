package vo;

import java.io.Serializable;

public class Chat implements Serializable {
    private String playerId;
    private String content;
    private String playerConnectTime;
    private String currentLocation;
    private String time;


    public String getPlayerId() {
        return playerId;
    }

    public void setPlayerId(String playerId) {
        this.playerId = playerId;
    }

    public String getPlayerConnectTime() {
        return playerConnectTime;
    }

    public void setPlayerConnectTime(String playerConnectTime) {
        this.playerConnectTime = playerConnectTime;
    }

    public String getContent() {
        return content;
    }

    public void setContent(String content) {
        this.content = content;
    }

    public String getTime() {
        return time;
    }

    public void setTime(String time) {
        this.time = time;
    }

    public String getCurrentLocation() {
        return currentLocation;
    }

    public void setCurrentLocation(String currentLocation) {
        this.currentLocation = currentLocation;
    }
}