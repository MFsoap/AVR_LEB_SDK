using System.Collections;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace LEBSDK.WebSocket
{
    public class BroadcastProgress : MonoBehaviour
    {
        public static BroadcastProgress Instance;

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// 发送剧情信息
        /// </summary>
        /// <param name="sceneName">剧情名称</param>
        /// <returns></returns>
        public IEnumerator SendHttp(string sceneName)
        {
            var url =
                $"{GetMessage.Expansion1}/vr/section?deviceSN={GetMessage.DeviceSn}&sectionName={sceneName}";
            var unityWeb = UnityWebRequest.Get(url);
            yield return unityWeb.SendWebRequest();
        }

        /// <summary>
        /// 发送结束并是否退出及多少秒
        /// </summary>
        /// <param name="isQuit"></param>
        /// <param name="time"></param>
        public void Progress(bool isQuit, float time=5)
        {
            var url =
                $"{GetMessage.Expansion1}/vr/app/stop";
            var unityWeb = new UnityWebRequest(url, "POST");
            var json = new JObject
            {
                { "deviceSN", GetMessage.DeviceSn },
                { "playRecordId", GetMessage.OrderNumber },
            };
            StartCoroutine(SetupPost(unityWeb, json.ToString(), !isQuit ? -1 : time));
        }

        private static IEnumerator SetupPost(UnityWebRequest request, string json, float time = -1)
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            if (string.IsNullOrEmpty(json)) yield break;
            var array = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(array);
            request.uploadHandler.contentType = "application/json;charset=utf-8";
            yield return request.SendWebRequest();

            if (Mathf.Approximately(time, -1)) yield break;
            yield return new WaitForSeconds(time);
            Application.Quit();
        }
    }
}