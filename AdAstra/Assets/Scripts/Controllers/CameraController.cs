using System;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class CameraController : MonoBehaviour
    {
        public GameObject FollowThis;
        public float ZoomSpeed = 1;
        public float SmoothSpeed = 2.0f;
        public float MinOrtho = 1.0f;
        public float MaxOrtho = 20.0f;
        private float _targetOrtho;

        void Start ()
        {
            _targetOrtho = Camera.main.orthographicSize;
            SetCamera();
        }
	
        void Update ()
        {
            SetCamera();
        }

        private void SetCamera()
        {
            if(FollowThis == null) return;

            //Follow player
            var followPosition = FollowThis.transform.position;
            transform.position = new Vector3(followPosition.x, followPosition.y, this.transform.position.z);

            //Zoom
            if (Input.GetKey(KeyCode.LeftShift))
            {
                var scroll = Input.GetAxis("Mouse ScrollWheel");
                if (Math.Abs(scroll) > 0.0000000001f)
                {
                    _targetOrtho -= scroll*ZoomSpeed;
                    _targetOrtho = Mathf.Clamp(_targetOrtho, MinOrtho, MaxOrtho);
                }

                Camera.main.orthographicSize = Mathf.MoveTowards(
                    Camera.main.orthographicSize, _targetOrtho, SmoothSpeed*Time.deltaTime);
            }
        }
    }
}
