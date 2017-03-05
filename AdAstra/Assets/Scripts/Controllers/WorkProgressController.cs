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

        public static void UpdateWork(float current, float max, Color color)
        {
            _image.color = new Color(color.r, color.g, color.b, 0.5f);
            if(current > 0.0f && !_go.activeSelf) On();
            var currentFixed = max - current;
            var progress = currentFixed/max;
            _image.fillAmount = progress;
            var perc = (int) (progress*100);
            _text.text = string.Format("{0}%", perc);
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
