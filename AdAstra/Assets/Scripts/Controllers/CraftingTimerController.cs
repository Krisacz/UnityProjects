using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{
    public class CraftingTimerController : MonoBehaviour
    {
        private float _target = 0.0f;
        private float _current = 0.0f;

        public void SetTimer(float seconds)
        {
            _target = seconds;
            _current = _target;
            UpdateLabel();
        }

        //If results in TRUE then crafting is completed 
        public bool UpdateProgress(float deltaTime)
        {
            _current -= deltaTime;
            UpdateLabel();
            return (_current <= 0.0f);
        }

        private void UpdateLabel()
        {
            var label = this.transform.FindChild("CraftingTimeText").gameObject;
            label.GetComponent<Text>().text = string.Format("Time Left: {0}s", _current.ToString("F1"));

            var slider = this.transform.GetComponent<Slider>();
            slider.maxValue = _target;
            slider.value = _current;
        }
    }
}
