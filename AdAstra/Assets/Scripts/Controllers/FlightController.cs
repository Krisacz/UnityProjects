using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class FlightController : MonoBehaviour
    {
        [Range(0f,360f)]
        public float DirectionAngle = 270f;
    }
}
