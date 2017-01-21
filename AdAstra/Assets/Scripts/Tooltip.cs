using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Tooltip: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private static GameObject _tooltipGo;
        private static GameObject _title;
        private static GameObject _description;
        private static readonly Vector3 Offset = new Vector3(0f, 50f, 0);

        public virtual string GetTooltipTitle() { return "Example title"; }
        public virtual string GetTooltipDescription() { return "Example content"; }
        internal static bool ShowTooltip = true;
        
        public void Start()
        {
            if (_tooltipGo == null) _tooltipGo = GameObject.FindGameObjectWithTag("Tooltip");
            if (_title == null) _title = GameObject.FindGameObjectWithTag("Tooltip-Title");
            if (_description == null) _description = GameObject.FindGameObjectWithTag("Tooltip-Description");
            _tooltipGo.SetActive(false);
        }

        public void Update()
        {
            if (_tooltipGo.activeSelf)
            {
                if(!ShowTooltip) _tooltipGo.SetActive(false);
                _tooltipGo.transform.position = Input.mousePosition + Offset;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(!ShowTooltip) return;
            var t = GetTooltipTitle();
            var d = GetTooltipDescription();
            if (string.IsNullOrEmpty(t) || string.IsNullOrEmpty(d)) return;
            _title.GetComponent<Text>().text = t;
            _description.GetComponent<Text>().text = d;
            _tooltipGo.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _tooltipGo.SetActive(false);
        }
    }
}
