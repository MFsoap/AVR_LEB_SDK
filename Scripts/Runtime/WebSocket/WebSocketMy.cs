using Google.Protobuf;
using LEBSDK;
using LEBSDK.WebSocket;
using Proto;
using UnityEngine;

namespace Client.NetWork.WebSocket
{
    public class WebSocketMy : MonoBehaviour
    {
        private SendPlayerData _data;

        private void Start()
        {
            _data = new SendPlayerData();
        }

        private void Update()
        {
            if (!WebSocketManager.Instance.IsOpen) return;
            foreach (var playersValue in WebSocketManager.Instance.Players.Values)
            {
                playersValue.Visible(
                    !(Vector3.Distance(GetPos(), playersValue.transform.localPosition) >=
                      5));
            }
        }

        private Vector3 GetPos()
        {
            var pos = transform.localPosition;
            pos.y = 0;
            return pos;
        }


        private void FixedUpdate()
        {
            if (WebSocketManager.Instance.IsOpen)
                WebSocketManager.Instance.SendRequireData(7001, GetData());
        }

        private byte[] GetData()
        {
            var pos = transform.localPosition;
            var eul = transform.localEulerAngles;
            _data.DeviceSN = string.IsNullOrEmpty(GetMessage.DeviceSn) ? "Demo" : GetMessage.DeviceSn;
            _data.AppId = string.IsNullOrEmpty(GetMessage.AppId) ? 123 : int.Parse(GetMessage.AppId);
            _data.PosX = pos.x;
            _data.PosY = pos.y;
            _data.PosZ = pos.z;
            _data.EulX = eul.x;
            _data.EulY = eul.y;
            _data.EulZ = eul.z;
            return _data.ToByteArray();
        }
    }
}