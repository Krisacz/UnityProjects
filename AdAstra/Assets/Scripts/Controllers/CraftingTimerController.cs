using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{
    public class CraftingTimerController : MonoBehaviour
    {
        private float _target = 0.0f;
        private float _current = 0.0f;
        private bool _craft = false;

        public void Craft()
        {
            _craft = true;
        }

        // Update is called once per frame
        void Update ()
        {
	        if(!_craft) return;

            _current -= Time.deltaTime;
            UpdateLabel();

            if (_current >= 0.0f) return;
            _current = _target;
            _craft = false;
            UpdateLabel();
        }

        public void SetTimer(float seconds)
        {
            _target = seconds;
            _current = _target;
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            var label = this.transform.FindChild("CraftingTimeText").gameObject;
            label.GetComponent<Text>().text = string.Format("Crafting Time: {0}s", _current.ToString("F1"));

            var slider = this.transform.GetComponent<Slider>();
            slider.maxValue = _target;
            slider.value = _current;
        }
    }
}
