using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{
    public class WorkProgressController : MonoBehaviour
    {
        private static Image _image;
        private static Text _text;
        private static GameObject _go;
        
        void Start ()
        {
            _image = transform.FindChild("Image").transform.GetComponent<Image>();
            _text = transform.FindChild("Text").transform.GetComponent<Text>();
            _go = transform.gameObject;
            Off();
        }

        public static void UpdateWork(float current, float max)
        {
            if(current > 0.0f && !_go.activeSelf) On();
            var progress = current/max;
            _image.fillAmount = progress;
            _text.text = string.Format("{0}s", current.ToString("F1"));
        }

        public static void Off()
        {
            _go.SetActive(false);
        }

        public static void On()
        {
            _go.SetActive(true);
        }
    }
}
