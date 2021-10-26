using Newtonsoft.Json;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization.Formatters;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

// MonoBehaviour를 상속 받기때문에 유니티 메인 스레드에서 돌아감
// 유니티 생명주기 참고: https://skuld2000.tistory.com/25
public class NetworkManager : MonoBehaviour
{
	Session _session = new Session();
	Connector connector = new Connector();

	public string Id { get; set; }
	public List<string> player { get; set; }

	void Awake()
	{
		//Debug.Log("NetworkManager - Awake");

		// DNS (Domain Name System)
		string host = Dns.GetHostName();
		IPHostEntry ipHost = Dns.GetHostEntry(host);
		IPAddress ipAddr = ipHost.AddressList[0];
		IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

		string sceneName = SceneManager.GetActiveScene().name; // 현제 Scene의 이름
		Debug.Log($"현재 씬 이름: {sceneName}");

		connector = new Connector();
		connector.Connect(endPoint,
			() => { return _session; });
	}

	public void Disconnect()
	{
		_session.Disconnect();//소켓 제거
	}

	private void OnDestroy()
	{
		_session.Disconnect();//소켓 제거
		//Debug.Log("NetworkManager - OnDestroy");
	}

	public void LoginSend(Dictionary<int, User> player)
	{
		//Debug.Log("NetworkManager - Send");

		//보낸다. 보낼땐 몇 바이트로 보낼지 알아서 이렇게 변환 후 보냄
		JsonSerializerSettings setting = new JsonSerializerSettings(); ;
		setting.Formatting = Formatting.Indented;
		setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

		string jdata = JsonConvert.SerializeObject(player, setting);
		byte[] sendBuff = Encoding.UTF8.GetBytes(jdata);
		_session.Send(sendBuff);
	}

	public void RegisterSend(Dictionary<int, User> player)
	{
		//Debug.Log("NetworkManager - Send");

		JsonSerializerSettings setting = new JsonSerializerSettings(); ;
		setting.Formatting = Formatting.Indented;
		setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

		string jdata = JsonConvert.SerializeObject(player, setting);
		byte[] sendBuff = Encoding.UTF8.GetBytes(jdata);
		_session.Send(sendBuff);
	}

	public void Verification(Dictionary<int, string> player)
	{
		//Debug.Log("verification - Send");
	
		JsonSerializerSettings setting = new JsonSerializerSettings(); ;
		setting.Formatting = Formatting.Indented;
		setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

		string jdata = JsonConvert.SerializeObject(player, setting);
		byte[] sendBuff = Encoding.UTF8.GetBytes(jdata);
		_session.Send(sendBuff);
	}

	public void MoveSend(Dictionary<int, Player> move)
	{
		//보낸다. 보낼땐 몇 바이트로 보낼지 알아서 이렇게 변환 후 보냄
		JsonSerializerSettings setting = new JsonSerializerSettings(); ;
		setting.Formatting = Formatting.Indented;
		setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

		string jdata = JsonConvert.SerializeObject(move, setting);
		byte[] sendBuff = Encoding.UTF8.GetBytes(jdata);
		_session.Send(sendBuff);
	}

	// 유저 처음 방 입장시 실행
	public void UserAdd(Dictionary<int, Player> move)
	{
		//보낸다. 보낼땐 몇 바이트로 보낼지 알아서 이렇게 변환 후 보냄
		JsonSerializerSettings setting = new JsonSerializerSettings(); ;
		setting.Formatting = Formatting.Indented;
		setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

		string jdata = JsonConvert.SerializeObject(move, setting);
		byte[] sendBuff = Encoding.UTF8.GetBytes(jdata);
		_session.Send(sendBuff);
	}

	public void InventoryOpen()
	{
		Debug.Log("InventoryOpen");

		for (int i = 0; i < player.Count; i++)
		{
			Debug.Log($"player[{i}] : {player[i]} ");
		}

		Dictionary<int, string> inventory = new Dictionary<int, string>();
		inventory.Add(6, player[1]);

		JsonSerializerSettings setting = new JsonSerializerSettings(); ;
		setting.Formatting = Formatting.Indented;
		setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

		string jdata = JsonConvert.SerializeObject(inventory, setting);
		byte[] sendBuff = Encoding.UTF8.GetBytes(jdata);
		_session.Send(sendBuff);
	}

	public void GearOpen()
	{
		Dictionary<int, string> gear = new Dictionary<int, string>();
		gear.Add(7, player[1]);

		JsonSerializerSettings setting = new JsonSerializerSettings(); ;
		setting.Formatting = Formatting.Indented;
		setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

		string jdata = JsonConvert.SerializeObject(gear, setting);
		byte[] sendBuff = Encoding.UTF8.GetBytes(jdata);
		_session.Send(sendBuff);
	}

	public void InventoryUpdate(Dictionary<int, GameObject> invenSlot)
	{
		//Debug.Log("InventoryUpdate - id: " + this.Id);
		List<string> inven = new List<string>();

		for(int i =0; i <invenSlot.Count;i++)
		{
			if (invenSlot[i] != null) {
				//Debug.Log($"invenSlot 정보: {invenSlot[i].transform.GetComponent<InvenItem>().ItemName}");
				inven.Add(invenSlot[i].transform.GetComponent<InvenItem>().ItemName);
			} else
			{
				//Debug.Log($"invenSlot 정보: null");
				inven.Add("없음");
			}
		}

		Dictionary<string, List<string>> send = new Dictionary<string, List<string>>();
		send.Add(player[1], inven);

		Dictionary<int, Dictionary<string, List<string>>> invenAdd = new Dictionary<int, Dictionary<string, List<string>>>();
		invenAdd.Add(8, send);

		JsonSerializerSettings setting = new JsonSerializerSettings(); ;
		setting.Formatting = Formatting.Indented;
		setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

		string jdata = JsonConvert.SerializeObject(invenAdd, setting);
		byte[] sendBuff = Encoding.UTF8.GetBytes(jdata);
		_session.Send(sendBuff);
	}

	public void GearUpdate(Dictionary<int, GameObject> gearSlot)
	{
		//Debug.Log("GearUpdate - id: " + this.Id);
		List<string> gear = new List<string>();
		for (int i = 0; i < gearSlot.Count; i++)
		{
			if (gearSlot[i] != null)
			{
				//Debug.Log($"gearSlot 정보: {gearSlot[i].transform.GetComponent<InvenItem>().ItemName}");
				gear.Add(gearSlot[i].transform.GetComponent<InvenItem>().ItemName);
			}
			else
			{
				// Debug.Log($"gearSlot 정보: null");
				gear.Add("없음");
			}
		}

		Dictionary<string, List<string>> send = new Dictionary<string, List<string>>();
		send.Add(player[1], gear);

		Dictionary<int, Dictionary<string, List<string>>> invenAdd = new Dictionary<int, Dictionary<string, List<string>>>();
		invenAdd.Add(9, send);

		JsonSerializerSettings setting = new JsonSerializerSettings(); ;
		setting.Formatting = Formatting.Indented;
		setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

		string jdata = JsonConvert.SerializeObject(invenAdd, setting);
		byte[] sendBuff = Encoding.UTF8.GetBytes(jdata);
		_session.Send(sendBuff);
	}

	public void NickNameVerification(Dictionary<int, List<string>> sendData)
	{
		JsonSerializerSettings setting = new JsonSerializerSettings(); ;
		setting.Formatting = Formatting.Indented;
		setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

		string jdata = JsonConvert.SerializeObject(sendData, setting);
		byte[] sendBuff = Encoding.UTF8.GetBytes(jdata);
		_session.Send(sendBuff);
	}

	public void CharacterCreate(Dictionary<int, List<string>> sendData)
	{
		JsonSerializerSettings setting = new JsonSerializerSettings(); ;
		setting.Formatting = Formatting.Indented;
		setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

		string jdata = JsonConvert.SerializeObject(sendData, setting);
		byte[] sendBuff = Encoding.UTF8.GetBytes(jdata);
		_session.Send(sendBuff);
	}

	public void CharacterCheck(Dictionary<int, string> sendData)
	{
		JsonSerializerSettings setting = new JsonSerializerSettings(); ;
		setting.Formatting = Formatting.Indented;
		setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

		string jdata = JsonConvert.SerializeObject(sendData, setting);
		byte[] sendBuff = Encoding.UTF8.GetBytes(jdata);
		_session.Send(sendBuff);
	}

	public void CharacterDelete(Dictionary<int, List<string>> sendData)
	{
		JsonSerializerSettings setting = new JsonSerializerSettings(); ;
		setting.Formatting = Formatting.Indented;
		setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

		string jdata = JsonConvert.SerializeObject(sendData, setting);
		byte[] sendBuff = Encoding.UTF8.GetBytes(jdata);
		_session.Send(sendBuff);
	}
}