using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/* 전달 받을 정보
 0: 캐릭터 명
 1: 채팅 내용
 2: 작성한 시간
*/
[Serializable]
public class Chat {
    
    public string playerId { get; set; }
    
    public string content { get; set; }

    public string playerConnectTime { get; set; }

    public string currentLocation { get; set; }
    
    public string time { get; set; }
}