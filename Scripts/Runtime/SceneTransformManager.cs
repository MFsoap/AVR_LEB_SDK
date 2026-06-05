using LEBSDK;
using UnityEngine;

namespace LEBSDK
{
    public class SceneTransformManager : MonoBehaviour
    {
        public int plotID;

        private void Start()
        {
            if (GetMessage.PlotPos.TryGetValue(plotID, out var plotPos))
            {
                transform.position = plotPos;
            }

            if (GetMessage.PlotRot.TryGetValue(plotID, out var plotRot))
            {
                transform.eulerAngles = plotRot;
            }


            Debug.Log($"场景预设位置: {transform.position}, 旋转: {transform.rotation.eulerAngles}");
        }
    }
}