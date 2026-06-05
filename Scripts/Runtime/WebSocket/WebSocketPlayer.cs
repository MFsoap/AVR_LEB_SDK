using LEBSDK.WebSocket;
using UnityEngine;

namespace LEBSDK.WebSocket
{
    public class WebSocketPlayer : MonoBehaviour
    {
        private MeshRenderer[] _meshRenderer;

        private Vector3 _pos;

        private Vector3 _eul;

        private float _time;

        private string _sn;

        private Transform _cameraPos;

        // Start is called before the first frame update
        private void Start()
        {
            _meshRenderer = GetComponentsInChildren<MeshRenderer>();
            _cameraPos = Camera.main.transform;
        }

        public void Visible(bool visible)
        {
            foreach (var meshRenderer in _meshRenderer)
            {
                meshRenderer.enabled = visible;
            }
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            _time += Time.deltaTime;
            if (_time >= 5)
            {
                WebSocketManager.Instance.Players.Remove(_sn);
                Destroy(gameObject);
            }

            transform.position = Vector3.Lerp(transform.position, _pos, 0.5f);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_eul), 0.5f);
        }

        public void Init(string sn, float posX, float posY, float posZ, float eulX, float eulY, float eulZ)
        {
            _sn = sn;
            SetData(posX, posY, posZ, eulX, eulY, eulZ);
        }


        public void SetData(float posX, float posY, float posZ, float eulX, float eulY, float eulZ)
        {
            _pos.x = posX;
            _pos.z = posZ;
            _eul.x = 0;
            _eul.y = eulY;
            _eul.z = 0;
            _time = 0;
        }
    }
}