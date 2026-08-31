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
        public void ReportThePlot(string sceneName)
        {
            StartCoroutine(SendHttp(sceneName));
        }

        /// <summary>
        /// 发送剧情信息
        /// </summary>
        /// <param name="sceneName">剧情名称</param>
        /// <returns></returns>
        private IEnumerator SendHttp(string sceneName)
        {
            var url =
                $"{GetMessage.Expansion1}/vr/section?deviceSN={GetMessage.DeviceSn}&sectionName={sceneName}";
            var unityWeb = UnityWebRequest.Get(url);
            yield return unityWeb.SendWebRequest();
        }

        /// <summary>
        /// 发送上报结束
        /// </summary>
        /// <param name="time"></param>
        public void Progress(float time = 5)
        {
            var url =
                $"{GetMessage.Expansion1}/vr/app/stop";
            var unityWeb = new UnityWebRequest(url, "POST");
            var json = new JObject
            {
                { "deviceSN", GetMessage.DeviceSn },
                { "playRecordId", GetMessage.OrderNumber },
            };
            StartCoroutine(SetupPost(unityWeb, json.ToString(), time));
        }

        private static IEnumerator SetupPost(UnityWebRequest request, string json, float time = 5)
        {
            yield return new WaitForSeconds(time);
            request.downloadHandler = new DownloadHandlerBuffer();
            if (string.IsNullOrEmpty(json)) yield break;
            var array = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(array);
            request.uploadHandler.contentType = "application/json;charset=utf-8";
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                yield return SetupPost(request, json, 0.5f);
            }
        }
    }
}