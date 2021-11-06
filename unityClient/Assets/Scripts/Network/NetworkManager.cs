using FlatBuffers;
using Newtonsoft.Json;
using ServerCore;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Assets.Scripts;
using System.Linq;
using System;


// MonoBehaviour를 상속 받기때문에 유니티 메인 스레드에서 돌아감
// 유니티 생명주기 참고: https://skuld2000.tistory.com/25
public class NetworkManager : MonoBehaviour
{
	//보낸다. 보낼땐 몇 바이트로 보낼지 알아서 이렇게 변환 후 보냄
	static JsonSerializerSettings setting = new JsonSerializerSettings();

	Session _session = new Session();
	Connector connector = new Connector();
	List<string> sellResult = new List<string>();

	public string Id { get; set; }

	public Player playerInfo { get; set; }

	public List<string> player { get; set; }

	public List<string> inven { get; set; }

	public List<object> enterList { get; set; }

	public List<string> player_gear { get; set; }

	public List<string> unitList { get; set; }

	public List<string> chatList { get; set; }

	public Chat chatContent { get; set; }

	public string connectTime { get; set; }

	public Queue<Chat> chatQueue { get; set; }

	void Awake()
	{
		//Debug.Log("NetworkManager - Awake");

		// DNS (Domain Name System)
		string host = Dns.GetHostName();
		IPHostEntry ipHost = Dns.GetHostEntry(host);
		IPAddress ipAddr = ipHost.AddressList[0];
		IPEndPoint endPoint = new IPEndPoint(ipAddr, 8007);

		// string sceneName = SceneManager.GetActiveScene().name; // 현제 Scene의 이름
		// Debug.Log($"NetworkManager - 현재 씬 이름: {sceneName}");

		connector = new Connector();
		connector.Connect(endPoint,
			() => { return _session; });

		setting.Formatting = Formatting.Indented;
		setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

		inven = new List<string>();
		enterList = new List<object>();
		chatQueue = new Queue<Chat>();
	}

	public void Disconnect() {
		Debug.Log("NetworkManager - Disconnect");
		_session.Disconnect();//소켓 제거
		_session = null;
	}

	private void OnDestroy() {
		//_session.Disconnect();//소켓 제거
		Debug.Log("NetworkManager - OnDestroy");
	}

	/**
	①FlatBufferBuilder 생성
	②String이 존재한다면 CreateString으로 문자열을 생성해두고 offset을 저장(중요!)
	③EchoMsg.StartEchoMsg();
	④EchoMsg.AddMsg
	⑤EchoMsg.EndEchoMsg(fbb);
	⑥fbb.Finish();
	⑦fbb.SizedByteArray()  //이게 최종byte[]이다. 반드시 SizedByteArray() 를 사용해야함.
	*/
	public void LoginSend(User user) {

		// user 클래스 직렬화
		string jdata = JsonConvert.SerializeObject(user, setting);

		// Debug.Log($"jdata: {jdata}");

		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(jdata);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 1);
		Message.AddContent(builder, msg);
		var message = Message.EndMessage(builder);

		builder.Finish(message.Value); // 해당 데이터를 빌드 합니다.

		// int -> byte 배열로 변환
		// 참고: https://niceman.tistory.com/31
		//int protocol = 1;
		//byte[] buffer = BitConverter.GetBytes(Convert.ToInt32(protocol));

		//Debug.Log($"buffer.Length: {buffer.Length}");
		// Debug.Log($"builder.SizedByteArray().Length: {builder.SizedByteArray().Length}");
		byte[] packet = builder.SizedByteArray();

		//byte[] packet = new byte[buffer.Length + builder.SizedByteArray().Length];
		//Debug.Log($"packet.Length: {packet.Length}");
		//Debug.Log($"packet.ToString(): {packet.ToString()}");
		//int i = BitConverter.ToInt32(buffer, 0);
		//Debug.Log($"int: {i}");
		//// buffer + packet 내용 합쳐서 전달
		//Array.Copy(buffer, 0, packet, 0, buffer.Length);
		//Array.Copy(builder.SizedByteArray(), 0, packet, buffer.Length, builder.SizedByteArray().Length);

		_session.Send(packet);
	}

	public void RegisterSend(User player) {
		Debug.Log($"RegisterSend");

		//Debug.Log("NetworkManager - Send");

		//JsonSerializerSettings setting = new JsonSerializerSettings(); ;
		//setting.Formatting = Formatting.Indented;
		//setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

		//string jdata = JsonConvert.SerializeObject(player, setting);
		//byte[] sendBuff = Encoding.UTF8.GetBytes(jdata);
		//_session.Send(sendBuff);

		// 클래스 직렬화
		string jdata = JsonConvert.SerializeObject(player, setting);

		// Debug.Log($"UserAdd - jdata: {jdata}");

		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(jdata);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 3);
		Message.AddContent(builder, msg);
		var message = Message.EndMessage(builder);

		builder.Finish(message.Value); // 해당 데이터를 빌드 합니다.
		byte[] packet = builder.SizedByteArray();

		_session.Send(packet);
	}

	public void Verification(string text) {
		//Debug.Log("verification - Send");
	
		//JsonSerializerSettings setting = new JsonSerializerSettings(); ;
		//setting.Formatting = Formatting.Indented;
		//setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

		//string jdata = JsonConvert.SerializeObject(player, setting);
		//byte[] sendBuff = Encoding.UTF8.GetBytes(jdata);
		//_session.Send(sendBuff);


		// 클래스 직렬화
		string jdata = JsonConvert.SerializeObject(text, setting);

		// Debug.Log($"UserAdd - jdata: {jdata}");

		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(jdata);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 2);
		Message.AddContent(builder, msg);
		var message = Message.EndMessage(builder);

		builder.Finish(message.Value); // 해당 데이터를 빌드 합니다.
		byte[] packet = builder.SizedByteArray();

		_session.Send(packet);
	}

	public void MoveSend(Player player)
	{
		// user 클래스 직렬화
		string jdata = JsonConvert.SerializeObject(player, setting);

		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(jdata);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 4);
		Message.AddContent(builder, msg);
		var message = Message.EndMessage(builder);

		builder.Finish(message.Value); // 해당 데이터를 빌드 합니다.
		byte[] packet = builder.SizedByteArray();

		_session.Send(packet);
	}

	// 17: 메인 씬에 접속하기 전에 호출되며, 캐릭터의 용병 정보를 조회 후 전달해줌
	public void MercenaryCheck(string name) {
		// Debug.Log($"MercenaryCheck: {name}");

		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(name);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 17);
		Message.AddContent(builder, msg);
		var message = Message.EndMessage(builder);

		builder.Finish(message.Value); // 해당 데이터를 빌드 합니다.
		byte[] packet = builder.SizedByteArray();

		_session.Send(packet);
	}

	// 메인 룸에 존재하는 유저 정보를 서버에 요청하는 기능
	public void Spawn() {
		// Debug.Log($"Spawn - enterList.Count: {enterList.Count}");
		// Debug.Log($"Spawn - chatQueue.Count: {chatQueue.Count}");

		Chat backChat = new Chat();
		if (chatQueue.Count != 0) {
			// 큐에 저장된 값 조회
			foreach (Chat chatInfo in chatQueue) {
				Debug.Log($"playerId : {chatInfo.playerId}, content : {chatInfo.content}, playerConnectTime : {chatInfo.playerConnectTime}" +
					$", currentLocation : {chatInfo.currentLocation}, time : {chatInfo.time}");
				backChat.playerId = chatInfo.playerId;
				backChat.content = chatInfo.content;
				backChat.playerConnectTime = chatInfo.playerConnectTime;
				backChat.currentLocation = chatInfo.currentLocation;
				backChat.time = chatInfo.time;
			}
		}

		enterList.Clear();
		enterList.Add(player[1]);// 캐릭터명
		enterList.Add(connectTime);// 접속 시간
		enterList.Add(SceneManager.GetActiveScene().name); // 현재 위치 정보

		// 마지막 채팅 내용
		if (chatQueue.Count == 0) {
			enterList.Add("채팅 마지막 내용 존재하지 않음");
		} else {
			Debug.Log($"마지막 채팅 내용 - playerId : {backChat.playerId}, content : {backChat.content}, " +
				$"playerConnectTime : {backChat.playerConnectTime}, currentLocation : {backChat.currentLocation}, time : {backChat.time}");
			enterList.Add(backChat.playerId);
			enterList.Add(backChat.content);
			enterList.Add(backChat.playerConnectTime);
			enterList.Add(backChat.currentLocation);
			enterList.Add(backChat.time);
		}
		

		// 클래스 직렬화
		string jdata = JsonConvert.SerializeObject(enterList, setting);

		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(jdata);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 8);
		Message.AddContent(builder, msg);
		var message = Message.EndMessage(builder);

		builder.Finish(message.Value); // 해당 데이터를 빌드 합니다.
		byte[] packet = builder.SizedByteArray();

		_session.Send(packet);
	}

	// 메인화면에 처음 들어올 경우 호출, 메인룸에 있는 유저 정보 호출
	public void UserAdd(Player player)
	{
		////보낸다. 보낼땐 몇 바이트로 보낼지 알아서 이렇게 변환 후 보냄
		//string jdata = JsonConvert.SerializeObject(move, setting);
		//byte[] sendBuff = Encoding.UTF8.GetBytes(jdata);

		// 클래스 직렬화
		string jdata = JsonConvert.SerializeObject(player, setting);

		// Debug.Log($"UserAdd - jdata: {jdata}");

		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(jdata);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 5);
		Message.AddContent(builder, msg);
		var message = Message.EndMessage(builder);

		builder.Finish(message.Value); // 해당 데이터를 빌드 합니다.
		byte[] packet = builder.SizedByteArray();

		_session.Send(packet);
	}
	public void InventoryInit()
	{
		// 클래스 직렬화
		string jdata = JsonConvert.SerializeObject(player[1], setting);

		//Debug.Log($"jdata: {jdata}");

		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(jdata);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 6);
		Message.AddContent(builder, msg);
		var message = Message.EndMessage(builder);

		// 해당 데이터를 빌드 합니다.
		builder.Finish(message.Value);
		byte[] packet = builder.SizedByteArray();

		_session.Send(packet);
	}

	public void InventoryClose(Dictionary<int, GameObject> invenSlot, Dictionary<int, GameObject> gearSlot) {
		Debug.Log($"InventoryClose - invenSlot.Count: {invenSlot.Count}, gearSlot.Count: {gearSlot.Count}");

		List<string> inven = new List<string>();
		for (int i = 0; i < invenSlot.Count; i++) {
			switch (invenSlot[i] != null)
			{
				case true:
					//Debug.Log($"invenSlot 정보: {invenSlot[i].transform.GetComponent<InvenItem>().ItemName}");
					inven.Add(invenSlot[i].transform.GetComponent<InvenItem>().ItemName);
					break;
				default:
					inven.Add("없음");
					break;
			}
		}
		inven.Add(player[1]);

		List<string> gear = new List<string>();
		for (int i = 0; i < gearSlot.Count; i++) {
			switch (gearSlot[i] != null) {
				case true:
					//Debug.Log($"gearSlot 정보: {gearSlot[i].transform.GetComponent<InvenItem>().ItemName}");
					gear.Add(gearSlot[i].transform.GetComponent<InvenItem>().ItemName);
					break;
				default:
					// Debug.Log($"gearSlot 정보: null");
					gear.Add("없음");
					break;
			}
		}

		// 장비, 인벤토리 정보 저장해주는 리스트 생성
		List<List<string>> closeList = new List<List<string>>();
		closeList.Add(inven);
		//closeList.Add(gear);

		// 클래스 직렬화
		string jdata = JsonConvert.SerializeObject(closeList, setting);

		Debug.Log($"jdata: {jdata}");

		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(jdata);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 15);
		Message.AddContent(builder, msg);
		var message = Message.EndMessage(builder);

		// 해당 데이터를 빌드 합니다.
		builder.Finish(message.Value);
		byte[] packet = builder.SizedByteArray();

		_session.Send(packet);
	}

	/**
	public void InventoryOpen()
	{
		Debug.Log("InventoryOpen");

		//for (int i = 0; i < player.Count; i++)
		//{
		//	Debug.Log($"player[{i}] : {player[i]} ");
		//}
		//Dictionary<int, string> inventory = new Dictionary<int, string>();
		//inventory.Add(6, player[1]);
		//string jdata = JsonConvert.SerializeObject(inventory, setting);
		//byte[] sendBuff = Encoding.UTF8.GetBytes(jdata);
		//_session.Send(sendBuff);

		// 클래스 직렬화
		string jdata = JsonConvert.SerializeObject(player[1], setting);

		//Debug.Log($"jdata: {jdata}");

		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(jdata);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 6);
		Message.AddContent(builder, msg);
		var message = Message.EndMessage(builder);

		// 해당 데이터를 빌드 합니다.
		builder.Finish(message.Value); 
		byte[] packet = builder.SizedByteArray();

		_session.Send(packet);
	}

	public void GearOpen() {
		//Dictionary<int, string> gear = new Dictionary<int, string>();
		//gear.Add(7, player[1]);
		////JsonSerializerSettings setting = new JsonSerializerSettings(); ;
		////setting.Formatting = Formatting.Indented;
		////setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
		//string jdata = JsonConvert.SerializeObject(gear, setting);
		//byte[] sendBuff = Encoding.UTF8.GetBytes(jdata);
		//_session.Send(sendBuff);

		// 클래스 직렬화
		string jdata = JsonConvert.SerializeObject(player[1], setting);

		//Debug.Log($"jdata: {jdata}");

		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(jdata);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 7);
		Message.AddContent(builder, msg);
		var message = Message.EndMessage(builder);

		// 해당 데이터를 빌드 합니다.
		builder.Finish(message.Value);
		byte[] packet = builder.SizedByteArray();

		_session.Send(packet);
	}

	*/

	public void InventoryUpdate(int slot_number, string item_name) {

		// 수정된 인벤 정보 저장
		inven[slot_number] = item_name;

		// 데이터 확인
		// Debug.Log("InventoryUpdate - inven.Count: " + inven.Count);
		for (int i =0; i < inven.Count; i++)
        {
			Debug.Log($"InventoryUpdate - inven[{i}]: {inven[i]} ");
		}

		// 서버로 데이터 전송
		Dictionary<string, List<string>> invenList = new Dictionary<string, List<string>>();
		invenList.Add(player[1], inven);

		// 클래스 직렬화
		string jdata = JsonConvert.SerializeObject(invenList, setting);

		// Debug.Log($"InventoryUpdate - jdata: {jdata}");

		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(jdata);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 14);
		Message.AddContent(builder, msg);
		var message = Message.EndMessage(builder);

		// 해당 데이터를 빌드 합니다.
		builder.Finish(message.Value);
		byte[] packet = builder.SizedByteArray();

		_session.Send(packet);
	}

	public void InventoryInfoUpdate(List<string> invenList)
    {
		// 수정된 인벤 정보 저장
		inven = invenList;

		// 서버로 데이터 전송
		Dictionary<string, List<string>> invenMap = new Dictionary<string, List<string>>();
		invenMap.Add(player[1], invenList);
		
		// 클래스 직렬화
		string jdata = JsonConvert.SerializeObject(invenMap, setting);

		// Debug.Log($"InventoryUpdate - jdata: {jdata}");

		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(jdata);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 14);
		Message.AddContent(builder, msg);
		var message = Message.EndMessage(builder);

		// 해당 데이터를 빌드 합니다.
		builder.Finish(message.Value);
		byte[] packet = builder.SizedByteArray();

		_session.Send(packet);
	}

	// 매개 변수: 인벤토리 슬롯 번호, 아이템 명, 아이템 구매 후 계산된 플레이어 돈 
	public void ShopBuyUpdate(int slot_number, string item_name, int price) {
		Debug.Log($"ShopBuyUpdate - slot_number: {slot_number}, item_name: {item_name}, price: {price}, sellResult.Count: {sellResult.Count}");

		if(sellResult.Count == 4)
        {
			sellResult.Clear();

		}

		// 서버에 어떻게 넘겨줄꺼고 프로세스를 어떻게 짤 것인가?
		// 데이터 수정해야할 정보: 플레이어 인벤토리, 돈
		// 서버에 전달해야할 정보: 인벤토리 슬롯 번호, 아이템 명, 아이템 구매 후 계산된 플레이어 돈 
		// 어떤 방식으로 전달할 것인가?

		// 수정된 인벤 정보 저장
		inven[slot_number] = item_name;

		// 수정된 데이터 리스트에 저장 후 서버로 데이터 전송
		sellResult.Add(player[1]); // 플레이어 명
		sellResult.Add(price.ToString()); // 아이템 구매 후 계산된 플레이어 돈 
		sellResult.Add(slot_number.ToString()); // 인벤토리 슬롯 번호
		sellResult.Add(item_name.ToString()); // 아이템 명
		
		// 클래스 직렬화
		string jdata = JsonConvert.SerializeObject(sellResult, setting);

        // Debug.Log($"InventoryUpdate - jdata: {jdata}");

        // FlatBuffers 빌드를 선언 해줍니다.
        var builder = new FlatBufferBuilder(1);

        // Serialize the FlatBuffer data.
        var msg = builder.CreateString(jdata);
        Message.StartMessage(builder);
        Message.AddProtocolId(builder, 16);
        Message.AddContent(builder, msg);
        var message = Message.EndMessage(builder);

        // 해당 데이터를 빌드 합니다.
        builder.Finish(message.Value);
        byte[] packet = builder.SizedByteArray();

        _session.Send(packet);
    }

	// 16: 에러 발생할 경우 재전송
	public void ShopBuyReUpdate()
    {
		// 클래스 직렬화
		string jdata = JsonConvert.SerializeObject(sellResult, setting);

		// Debug.Log($"InventoryUpdate - jdata: {jdata}");

		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(jdata);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 16);
		Message.AddContent(builder, msg);
		var message = Message.EndMessage(builder);

		// 해당 데이터를 빌드 합니다.
		builder.Finish(message.Value);
		byte[] packet = builder.SizedByteArray();

		_session.Send(packet);
	}

	// 매개 변수: 인벤토리 슬롯 번호, 아이템 명, 아이템 판매 후 계산된 플레이어 돈
	public void ShopSellUpdate(int slot_number, string item_name, int price) {
		Debug.Log($"ShopSellUpdate - slot_number: {slot_number}, item_name: {item_name}, price: {price}");

		// 수정된 데이터 리스트에 저장 후 서버로 데이터 전송
		List<string> sellResult = new List<string>();
		sellResult.Add(player[1]); // 플레이어 명
		sellResult.Add(price.ToString()); // 아이템 구매 후 계산된 플레이어 돈 
		sellResult.Add(slot_number.ToString()); // 인벤토리 슬롯 번호
		sellResult.Add(item_name.ToString()); // 아이템 명

		// 클래스 직렬화
		string jdata = JsonConvert.SerializeObject(sellResult, setting);

		// Debug.Log($"InventoryUpdate - jdata: {jdata}");

		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(jdata);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 16);
		Message.AddContent(builder, msg);
		var message = Message.EndMessage(builder);

		// 해당 데이터를 빌드 합니다.
		builder.Finish(message.Value);
		byte[] packet = builder.SizedByteArray();

		_session.Send(packet);
	}

	public void EmploymentEvent(string playerId, string money, string employmentName) {
		Debug.Log($"EmploymentEvent - playerId: {playerId}, money: {money}, employmentName: {employmentName} ");

		// 수정된 데이터 리스트에 저장 후 서버로 데이터 전송
		List<string> buyMercenary = new List<string>();
		buyMercenary.Add(playerId); // 플레이어 명
		buyMercenary.Add(money.ToString()); // 소지금
		buyMercenary.Add(employmentName.ToString()); // 구매한 용병 이름

		// 클래스 직렬화
		string jdata = JsonConvert.SerializeObject(buyMercenary, setting);

		// Debug.Log($"InventoryUpdate - jdata: {jdata}");

		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(jdata);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 18);
		Message.AddContent(builder, msg);
		var message = Message.EndMessage(builder);

		// 해당 데이터를 빌드 합니다.
		builder.Finish(message.Value);
		byte[] packet = builder.SizedByteArray();

		_session.Send(packet);
	}

	public void FireEvent(string playerId, string money, string employmentName) {
		Debug.Log($"EmploymentEvent - playerId: {playerId}, money: {money}, employmentName: {employmentName} ");

		// 수정된 데이터 리스트에 저장 후 서버로 데이터 전송
		List<string> buyMercenary = new List<string>();
		buyMercenary.Add(playerId); // 플레이어 명
		buyMercenary.Add(money.ToString()); // 소지금
		buyMercenary.Add(employmentName.ToString()); // 구매한 용병 이름

		// 클래스 직렬화
		string jdata = JsonConvert.SerializeObject(buyMercenary, setting);

		// Debug.Log($"InventoryUpdate - jdata: {jdata}");

		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(jdata);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 19);
		Message.AddContent(builder, msg);
		var message = Message.EndMessage(builder);

		// 해당 데이터를 빌드 합니다.
		builder.Finish(message.Value);
		byte[] packet = builder.SizedByteArray();

		_session.Send(packet);
	}

	public void ReInventoryUpdate(Dictionary<string, List<string>> invenList) {
		// 클래스 직렬화
		JsonSerializerSettings setting = new JsonSerializerSettings();
		string jdata = JsonConvert.SerializeObject(invenList, setting);

		// Debug.Log($"InventoryUpdate - jdata: {jdata}");

		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(jdata);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 14);
		Message.AddContent(builder, msg);
		var invenMessage = Message.EndMessage(builder);

		// 해당 데이터를 빌드 합니다.
		builder.Finish(invenMessage.Value);
		byte[] packet = builder.SizedByteArray();

		_session.Send(packet);
	}

	public void GearUpdate(Dictionary<int, GameObject> gearSlot)
	{
		// Debug.Log("GearUpdate - gearSlot.Count: " + gearSlot.Count);

		List<string> gear = new List<string>();
		for (int i = 0; i < gearSlot.Count; i++) {
			switch (gearSlot[i] != null) {
				case true:
					//Debug.Log($"gearSlot 정보: {gearSlot[i].transform.GetComponent<InvenItem>().ItemName}");
					gear.Add(gearSlot[i].transform.GetComponent<InvenItem>().ItemName);
					break;
				default:
					// Debug.Log($"gearSlot 정보: null");
					gear.Add("없음");
					break;
			}
		}

		Dictionary<string, List<string>> gearList = new Dictionary<string, List<string>>();
		gearList.Add(player[1], gear);

		// 클래스 직렬화
		string jdata = JsonConvert.SerializeObject(gearList, setting);

		//Debug.Log($"GearUpdate - jdata: {jdata}");

		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(jdata);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 9);
		Message.AddContent(builder, msg);
		var message = Message.EndMessage(builder);

		// 해당 데이터를 빌드 합니다.
		builder.Finish(message.Value);
		byte[] packet = builder.SizedByteArray();

		_session.Send(packet);
	}

	public void GeadInfoUpdate(List<string> gearList) {

		// 수정된 장비 정보 저장
		player_gear = gearList;

		// 플레이어 아이디 정보 추가
		gearList.Add(player[1]);

		// 클래스 직렬화
		string jdata = JsonConvert.SerializeObject(gearList, setting);

		//Debug.Log($"GearUpdate - jdata: {jdata}");

		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(jdata);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 9);
		Message.AddContent(builder, msg);
		var message = Message.EndMessage(builder);

		// 해당 데이터를 빌드 합니다.
		builder.Finish(message.Value);
		byte[] packet = builder.SizedByteArray();

		_session.Send(packet);
	}

	public void NickNameVerification(string playerId)
	{
		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(playerId);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 10);
		Message.AddContent(builder, msg);
		var invenMessage = Message.EndMessage(builder);

		// 해당 데이터를 빌드 합니다.
		builder.Finish(invenMessage.Value);
		byte[] packet = builder.SizedByteArray();

		// 서버에 전송
		_session.Send(packet);
	}

	public void CharacterCreate(List<string> createData)
	{
		// json 라이브러리로 list -> string 변환
		string jdata = JsonConvert.SerializeObject(createData, setting);

		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(jdata);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 11);
		Message.AddContent(builder, msg);
		var invenMessage = Message.EndMessage(builder);

		// 해당 데이터를 빌드 합니다.
		builder.Finish(invenMessage.Value);
		byte[] packet = builder.SizedByteArray();

		// 서버에 전송
		_session.Send(packet);
	}

	public void CharacterCheck(string sendData)
	{
		//string jdata = JsonConvert.SerializeObject(sendData, setting);
		//byte[] sendBuff = Encoding.UTF8.GetBytes(jdata);

		// user 클래스 직렬화
		string jdata = JsonConvert.SerializeObject(sendData, setting);

		// Debug.Log($"jdata: {jdata}");

		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(jdata);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 12);
		Message.AddContent(builder, msg);
		var message = Message.EndMessage(builder);

		builder.Finish(message.Value); // 해당 데이터를 빌드 합니다.
		byte[] packet = builder.SizedByteArray();

		_session.Send(packet);
	}

	public void CharacterDelete(List<string> sendData)
	{
		//JsonSerializerSettings setting = new JsonSerializerSettings();
		//setting.Formatting = Formatting.Indented;
		//setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

		//string jdata = JsonConvert.SerializeObject(sendData, setting);
		//byte[] sendBuff = Encoding.UTF8.GetBytes(jdata);
		//_session.Send(sendBuff);

		// json 라이브러리로 list -> string 변환
		string jdata = JsonConvert.SerializeObject(sendData, setting);

		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(jdata);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 13);
		Message.AddContent(builder, msg);
		var invenMessage = Message.EndMessage(builder);

		// 해당 데이터를 빌드 합니다.
		builder.Finish(invenMessage.Value);
		byte[] packet = builder.SizedByteArray();

		// 서버에 전송
		_session.Send(packet);
	}

	public void ChatSend(Chat sendMsg) {
		Debug.Log($"서버 보내기 전");
		Debug.Log($"유저 아이디: {sendMsg.playerId}");
		Debug.Log($"유저 접속 시간: {sendMsg.playerConnectTime}");
		Debug.Log($"유저 위치: {sendMsg.currentLocation}");
		Debug.Log($"채팅 내용: {sendMsg.content}");
		Debug.Log($"채팅 입력 시간: {sendMsg.time}");
		
		chatContent = sendMsg;

		// 클래스 직렬화
		string jdata = JsonConvert.SerializeObject(chatContent, setting);

		//Debug.Log($"GearUpdate - jdata: {jdata}");

		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(jdata);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 20);
		Message.AddContent(builder, msg);
		var message = Message.EndMessage(builder);

		// 해당 데이터를 빌드 합니다.
		builder.Finish(message.Value);
		byte[] packet = builder.SizedByteArray();

		// 소켓 send 메소드를 사용하여 서버로 전송
		_session.Send(packet);
	}

	public void ChatReSend() {
		// 클래스 직렬화
		string jdata = JsonConvert.SerializeObject(chatContent, setting);

		//Debug.Log($"GearUpdate - jdata: {jdata}");

		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(jdata);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 20);
		Message.AddContent(builder, msg);
		var message = Message.EndMessage(builder);

		// 해당 데이터를 빌드 합니다.
		builder.Finish(message.Value);
		byte[] packet = builder.SizedByteArray();

		_session.Send(packet);
	}
	
	public void PlayerStatUpdate() {
		
		// 클래스 직렬화
		string jdata = JsonConvert.SerializeObject(player, setting);

		// Debug.Log($"PlayerStatUpdate - jdata: {jdata}");

		// FlatBuffers 빌드를 선언 해줍니다.
		var builder = new FlatBufferBuilder(1);

		// Serialize the FlatBuffer data.
		var msg = builder.CreateString(jdata);
		Message.StartMessage(builder);
		Message.AddProtocolId(builder, 22);
		Message.AddContent(builder, msg);
		var message = Message.EndMessage(builder);

		// 해당 데이터를 빌드 합니다.
		builder.Finish(message.Value);
		byte[] packet = builder.SizedByteArray();

		_session.Send(packet);
	}
}