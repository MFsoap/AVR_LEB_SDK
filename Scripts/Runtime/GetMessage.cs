using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace LEBSDK
{
    public class GetMessage
    {
        public static string DeviceSn { get; private set; }
        public static string ShopId { get; private set; }
        public static string ShopName { get; private set; }
        public static string VenueId { get; private set; }
        public static string VenueName { get; private set; }
        public static string AppId { get; private set; }
        public static string AppName { get; private set; }
        public static string OrderNumber { get; private set; }
        public static string Expansion1 { get; private set; }
        public static string Expansion2 { get; private set; }
        public static string SceneName { get; private set; }
        public static string Coordinate { get; private set; }
        public static long Timestamp { get; private set; }

        private static string _coordinate;

        public static Dictionary<int, Vector3> PlotPos = new();
        public static Dictionary<int, Vector3> PlotRot = new();

        public static bool GetData()
        {
            // 检查是否有传递的参数
            if (!Application.isMobilePlatform) return true;
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            var intent = currentActivity.Call<AndroidJavaObject>("getIntent");

            // 从Intent中获取参数
            DeviceSn = intent.Call<string>("getStringExtra", "deviceSN");
            ShopId = intent.Call<string>("getStringExtra", "shopId");
            ShopName = intent.Call<string>("getStringExtra", "shopName");
            VenueId = intent.Call<string>("getStringExtra", "venueId");
            VenueName = intent.Call<string>("getStringExtra", "venueName");
            AppId = intent.Call<string>("getStringExtra", "appId");
            AppName = intent.Call<string>("getStringExtra", "appName");
            OrderNumber = intent.Call<string>("getStringExtra", "orderNumber");
            Expansion1 = intent.Call<string>("getStringExtra", "expansion1");
            Expansion2 = intent.Call<string>("getStringExtra", "expansion2");
            SceneName = intent.Call<string>("getStringExtra", "sceneName");
            _coordinate = intent.Call<string>("getStringExtra", "Coordinate");

            var time = intent.Call<string>("getStringExtra", "timestamp");
            Expansion1 ??= "wujie.iweier.com.cn/avr-wujie-api";
            if (string.IsNullOrEmpty(time) || string.IsNullOrEmpty(OrderNumber))
            {
                Utils.StartTimer(1, Application.Quit);
                return true;
            }

            if (long.TryParse(time, out var timestamp))
            {
                Timestamp = timestamp;

                if (string.IsNullOrEmpty(_coordinate)) return false;
                var jsonObject = JObject.Parse(_coordinate);

                var appMap = (JArray)jsonObject["appMap"];

                if (appMap != null)
                    foreach (var app in appMap)
                    {
                        if (!app.HasValues) continue;
                        PlotPos.Add(app["itemPlotId"]!.ToObject<int>(),
                            new Vector3(app["centerX"]!.ToObject<float>(), 0, app["centerZ"]!.ToObject<float>()));
                        PlotRot.Add(app["itemPlotId"].ToObject<int>(),
                            new Vector3(0, app["angle"]!.ToObject<float>(), 0));
                    }
            }


            if (Timestamp != 0) return false;
            Utils.StartTimer(1, Application.Quit);
            return true;
        }
    }

    // 计时工具类
    public static class Utils
    {
        /// <summary>
        /// 延迟一定时间后执行指定操作
        /// </summary>
        /// <param name="delay">延迟时间（秒）</param>
        /// <param name="action">延迟后执行的操作</param>
        public static async void StartTimer(float delay, Action action)
        {
            await Task.Delay(TimeSpan.FromSeconds(delay));
            action?.Invoke();
        }
    }
}