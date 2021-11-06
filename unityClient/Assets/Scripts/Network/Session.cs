using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ServerCore
{
    public class Session
    {
        Socket _socket;
        int _disconnected = 0;
        object _lock = new object();
        Queue<byte[]> _sendQueue = new Queue<byte[]>();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
        
        public void Start(Socket socket)
        {
            _socket = socket;

            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            //_recvArgs.UserToken = this;//식별자 구분, 연동할 데이터 있을 경우 사용
            _recvArgs.SetBuffer(new byte[65535], 0, 65535); //세션을 만들때 마다 새로 할당(매개변수: byte 배열 변수, 시작 위치, 크기)

            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterRecv();
        }

        public void Send(byte[] sendBuff) {
            try
            {
                lock (_lock)
                {
                    _sendQueue.Enqueue(sendBuff);

                    if (_pendingList.Count == 0)
                        RegisterSend();
                }
            } catch(Exception e)
            {
                Debug.Log($"Send Failed {e}");
            }

        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
                return;

            Debug.Log($"Disconnect(): {_socket.RemoteEndPoint}");
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        #region 네트워크 통신

        void RegisterSend()
        {
            if (_disconnected == 1)//멀티스레드 예외처리
                return;

            // SetBuffer: 보낼 정보를 한번에 하나씩 
            // BufferList: 보낼 정보들을 리스트에 넣어준 후 한번에 연결
            // ArraySegment: 스택에 할당, 사용이유: session#3 - 6분30초
            while (_sendQueue.Count > 0)
            {
                byte[] buff = _sendQueue.Dequeue();
                _pendingList.Add(new ArraySegment<byte>(buff, 0, buff.Length));
            }

            _sendArgs.BufferList = _pendingList;

            try
            {
                bool pending = _socket.SendAsync(_sendArgs);
                if (pending == false)
                    OnSendCompleted(null, _sendArgs);
            }
            catch (Exception e)
            {
                Debug.Log($"RegisterSend Failed {e}");
                _socket.Close();
            }
        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs args) {
            lock (_lock) { // 콜백 함수 때문에 락처리
                // 몇 바이트를 받았는가? 0바이트 의경우(연결 끊을 경우)
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success) {
                    try {
                        _sendArgs.BufferList = null;
                        _pendingList.Clear();

                        //OnSend(_sendArgs.BytesTransferred);
                        //Debug.Log($"OnSendCompleted - Transferred bytes: {_sendArgs.BytesTransferred}");

                        //누군가가 만약 예약을 또 했다면 다시한번 작업을 처리(다시 한번 체크)
                        if (_sendQueue.Count > 0)
                            RegisterSend();
                    } catch (Exception e) {
                        Debug.Log($"OnSendCompleted Failed {e}");
                    }
                } else {
                    Disconnect();// 상대방에게 문제가 있으니 연결 제거
                }
            }
        }

        void RegisterRecv() {

            //멀티스레드 예외처리
            if (_disconnected == 1) {
                return;
            }
                
            try {
                bool pending = _socket.ReceiveAsync(_recvArgs);
                if (pending == false)
                    OnRecvCompleted(null, _recvArgs);
            } catch (Exception e) {
                Debug.Log($"RegisterRecv Failed {e}");
            }
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args) {
            lock (_lock) {// 콜백 함수 때문에 락처리
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success) {
                    try {
                        string recvData = null;
                        //TODO - 받은 데이터를 처리하는 부분
                        recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                        // Debug.Log($"[From Server]: {recvData}");

                        //Newtonsoft.Json 라이브러리 사용해서 json 역직렬화 처리
                        Dictionary<int, object> result = JsonConvert.DeserializeObject<Dictionary<int, object>>(recvData);
                        //참고: https://ponyozzang.tistory.com/325
                        foreach (KeyValuePair<int, object> recv in result)
                        {
                            //Debug.Log($"KeyValuePair - Key: {recv.Key}, Value:{recv.Value}");
                            Dictionary<int, object> recvMap = new Dictionary<int, object>();
                            recvMap.Add(recv.Key, recv.Value);
                            Queue.Instance.Push(recvMap);
                        }

                    } catch (Exception e) {
                        Debug.Log($"OnRecvCompleted Failed {e}");
                
                    } finally
                    {
                        RegisterRecv();
                    }
                } else {
                    Disconnect();
                }
            }
        }

        #endregion
    }
}