# 爱威尔大空间SDK

## 依赖安装
本SDK依赖UnityWebSocket，请先通过以下方式安装：
1. 打开Package Manager
2. 点击"+" → "Add package from git URL..."
3. 输入：`https://github.com/psygames/UnityWebSocket.git#upm`

## 使用方法

1. 将Scripts\Runtime\Prefabs 下的 LEBManager.prefab放置到项目初始化场景内
2. 把玩家的父物体拖入到 LEBManager.prefab 下的 WebSocketManager组件里的 generatedObj 变量下
3. 将其他玩家添加 WebSocketPlayer 脚本
4. 将vr相机添加 WebSocketMy 脚本
5. 在项目初始化完成后等待两秒根据项目的场景名执行方法（如下）
    ~~~
    switch (GetMessage.SceneName)
        {
            case "":
            case "001": //(项目的场景名称)
                SceneManager.LoadSceneAsync("One");
                break;

            case "002":

                SceneManager.LoadSceneAsync("Two");
                break;

            case "003":
                SceneManager.LoadSceneAsync("Three");
                break;

            case "004":
                SceneManager.LoadSceneAsync("Four");
                break;

            case "005":
                SceneManager.LoadSceneAsync("Five");
                break;

            case "006":
                SceneManager.LoadSceneAsync("Six");
                break;
        }
    ~~~
6. 每个场景根据设计添加一个中心轴并将场景设置成子物体可方便适配不同场地，将中心轴添加 SceneTransformManager 组件，并设置当前场景序列，从1开始。需等待0.5~1秒后执行剧情逻辑
7. 每进入新的场景时执行
    ~~~
          StartCoroutine(BroadcastProgress.Instance.SendHttp("（项目的场景名）"))
    ~~~
8. 到最后项目结束时执行,参数 1：是否退出，2：倒计时多少秒退出 | 可为空，默认是5秒
    ~~~
        StartCoroutine(BroadcastProgress.Instance.Progress(true,5))
    ~~~