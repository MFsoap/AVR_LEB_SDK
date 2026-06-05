using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Proto;
using UnityEngine;
using UnityWebSocket;

namespace LEBSDK.WebSocket
{
    public class WebSocketManager : MonoBehaviour
    {
        public static WebSocketManager Instance { get; private set; }

        public Dictionary<string, WebSocketPlayer> Players = new();

        public UnityWebSocket.WebSocket Socket { get; private set; }

        private IEnumerator _heartBeatTween;


        [SerializeField] private GameObject generatedObj;

        /// <summary>
        /// 固定长度 获取数据第四位开始
        /// </summary>
        private const uint MinResDataSize = 4;

        public bool IsOpen { get; private set; }

        private string _address;

        /// <summary>
        /// 在场景里
        /// </summary>
        private bool _isIn;

        private void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // Start is called before the first frame update
        private void Start()
        {
            // 获取外部传入信息
            if (GetMessage.GetData()) return;
            OnValueChanged(GetMessage.Expansion2);
        }

        private void OnValueChanged(string obj)
        {
            if (string.IsNullOrEmpty(obj)) return;
            if (!string.IsNullOrEmpty(_address)) return;
            _address = $"ws://{obj}:7777/WebSocketServer/{GetMessage.VenueId}";
            InitializeWebSocket();
        }

        /// <summary>
        /// 初始化连接
        /// </summary>
        private void InitializeWebSocket()
        {
            Socket = new UnityWebSocket.WebSocket(_address);

            // 注册回调
            Socket.OnOpen += OnOpen;
            Socket.OnClose += OnClose;
            Socket.OnMessage += OnMessage;
            Socket.OnError += OnError;

            _heartBeatTween = SendHeartbeat();
            //  StartCoroutine(_heartBeatTween);
            // 连接
            Socket.ConnectAsync();
        }


        /// <summary>
        /// 发送心跳
        /// </summary>
        private IEnumerator SendHeartbeat()
        {
            yield return new WaitForSeconds(5f);
            SendRequireData(1001);
        }

        public void SendRequireData(uint resID, byte[] data = null)
        {
            if (!IsOpen) return;
            //主机字节转网络字节
            var reqId = (int)ByteSwap.Swap32(resID);

            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

            // 写入基本类型
            writer.Write(reqId);
            // writer.Write(_messageID++);

            if (data != null)
            {
                // 写入实际数据
                writer.Write(data);
            }

            Socket.SendAsync(stream.ToArray());
        }

        private void OnDestroy()
        {
            CloseWebSocket();
        }

        private void CloseWebSocket()
        {
            if (Socket == null) return;
            Socket.OnOpen -= OnOpen;
            Socket.OnError -= OnError;
            Socket.OnClose -= OnClose;
            Socket.OnMessage -= OnMessage;
            Socket.CloseAsync();

            Socket = null;
        }

        private void OnError(object sender, UnityWebSocket.ErrorEventArgs e)
        {
            print(e.Exception.Message);
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            var rawData = e.RawData;
            //获取信息
            using var reader = new BinaryReader(new MemoryStream(rawData));

            // 读取基本类型
            // 获取消息ID
            var resID = ByteSwap.Swap32((uint)reader.ReadInt32());
            LogDebug.LogEditor(resID);
            if (rawData.Length <= MinResDataSize) return;

            var objData = new byte[rawData.Length - reader.BaseStream.Position];
            Array.Copy(rawData, reader.BaseStream.Position, objData, 0, objData.Length);
            // 判断 resID 号 多少
            switch (resID)
            {
                // 心跳返回内容
                case 1002:
                    _heartBeatTween = SendHeartbeat();
                    StartCoroutine(_heartBeatTween);
                    break;

                case 7001:
                    var playerData = SendPlayerData.Parser.ParseFrom(objData);

                    if (Players.TryGetValue(playerData.DeviceSN, out var player))
                    {
                        player.SetData(playerData.PosX, playerData.PosY, playerData.PosZ, playerData.EulX,
                            playerData.EulY,
                            playerData.EulZ);
                    }

                    else
                    {
                        var parent = transform.Find($"{playerData.AppId}");
                        if (parent == null)
                        {
                            parent = new GameObject().transform;
                            parent.parent = transform;
                            parent.name = $"{playerData.AppId}";
                        }

                        var play = Instantiate(generatedObj, parent).GetComponent<WebSocketPlayer>();
                        play.Init(playerData.DeviceSN, playerData.PosX, playerData.PosY, playerData.PosZ,
                            playerData.EulX,
                            playerData.EulY,
                            playerData.EulZ);
                        play.gameObject.SetActive(true);
                        play.name = playerData.DeviceSN;

                        Players.TryAdd(playerData.DeviceSN, play);
                    }


                    break;
            }
        }

        private async void OnClose(object sender, CloseEventArgs e)
        {
            LogDebug.LogEditor("断开");
            IsOpen = false;
            StopCoroutine(_heartBeatTween);
            await Task.Delay(TimeSpan.FromSeconds(2));
            InitializeWebSocket();
        }

        private void OnOpen(object sender, OpenEventArgs e)
        {
            if (_isIn) return;
            _isIn = true;


            // switch (GetMessage.SceneName)
            // {
            //     case "":
            //     case "启程·时空列车":
            //         SceneManager.LoadSceneAsync("One");
            //         break;
            //
            //     case "渔火初燃":
            //
            //         SceneManager.LoadSceneAsync("Two");
            //         break;
            //
            //     case "叩响时代之门":
            //         SceneManager.LoadSceneAsync("Three");
            //         break;
            //
            //     case "圣殿谜题":
            //         SceneManager.LoadSceneAsync("Four");
            //         break;
            //
            //     case "历史回响":
            //         SceneManager.LoadSceneAsync("Five");
            //         break;
            //
            //     case "冰城新生·归程":
            //         SceneManager.LoadSceneAsync("Six");
            //         break;
            // }

            IsOpen = true;
            StartCoroutine(_heartBeatTween);
        }

        // Update is called once per frame
        void Update()
        {
        }
    }

    public abstract class LogDebug
    {
        public static void LogEditor(object msg)
        {
            if (Application.isEditor)
            {
                Debug.Log("Editor: " + msg);
            }
        }

        public static void LogWarningEditor(object msg)
        {
            if (Application.isEditor)
            {
                Debug.Log("Editor: " + msg);
            }
        }
    }
}