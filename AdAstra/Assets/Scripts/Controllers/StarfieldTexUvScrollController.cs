﻿using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class StarfieldTexUvScrollController : MonoBehaviour
    {
        private FlightController _flightController;
        public float BackgroundOffsetSpeed = 0.5f;

        private void Start()
        {
            _flightController = this.transform.GetComponentInParent<FlightController>();
        }

        private void Update ()
        {
            TextureUvMovement();    
        }

        private void TextureUvMovement()
        {
            var angle = MathHelper.Angle2Vector(_flightController.DirectionAngle).normalized;
            var mr = this.GetComponent<MeshRenderer>();
            var mat = mr.material;
            var offset = mat.mainTextureOffset;
            offset.x += Time.deltaTime*angle.x*(BackgroundOffsetSpeed/100f) * (-1);
            offset.y += Time.deltaTime*angle.y*(BackgroundOffsetSpeed/100f) * (-1);
            mat.mainTextureOffset = offset;
        }
    }
}
