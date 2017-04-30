using System;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class StarfieldParticlesController : MonoBehaviour
    {
        private float _speedModifier = 10f;
        private float _prefDirectionAngle;
        private FlightController _flightController;
        private ParticleSystem _particleSystem;

        private void Start()
        {
            _flightController = this.transform.GetComponentInParent<FlightController>();
            _prefDirectionAngle = _flightController.DirectionAngle;
            _particleSystem = this.transform.GetComponent<ParticleSystem>();
            UpdateDirection(true);
            UpdatePosition();
        }

        private void Update()
        {
            UpdateDirection();
            UpdatePosition();
        }

        private void UpdateDirection(bool forceUpdate = false)
        {
            if ((forceUpdate)||(Math.Abs(_prefDirectionAngle - _flightController.DirectionAngle) > 0.001f))
            {
                var dir = _flightController.DirectionAngle;

                //Particle Systme Rotation
                //Add 45 degree to adjust the fact that our particle system
                //is a part of circle and then place it on another side of rotation (+180 degree)
                transform.rotation = Quaternion.AngleAxis(dir - 45f + 180f, Vector3.back);

                //Particles Velocity direction (angle)
                var newV2 = MathHelper.Angle2Vector(dir).normalized;
                var velOverTime = _particleSystem.velocityOverLifetime;
                var newVelOverTimeX = new ParticleSystem.MinMaxCurve() { constantMax = newV2.x * _speedModifier };
                var newVelOverTimeY = new ParticleSystem.MinMaxCurve() { constantMax = newV2.y * _speedModifier };
                velOverTime.x = newVelOverTimeX;
                velOverTime.y = newVelOverTimeY;

                //Particles rotation
                var main = _particleSystem.main;
                main.startRotation = MathHelper.DegreeToRadian(dir);

                //Update check variable
                _prefDirectionAngle = _flightController.DirectionAngle;
            }
        }

        private void UpdatePosition()
        {
            var cam = Camera.main.transform.position;
            this.transform.position = new Vector3(cam.x, cam.y, 0f);
        }
    }
}

