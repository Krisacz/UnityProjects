using Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{
    public class RequiredItemController : MonoBehaviour
    {
        private ItemId _itemId;
        private int _required;

        public void SetRequirement(ItemId itemId, int required)
        {
            _itemId = itemId;
            _required = required;
        }

        public void UpdateStackCountText(int current)
        {
            //Label
            var label = this.transform.FindChild("StackCountText");
            var txt = string.Empty;

            if (current >= _required) txt = "<color=#00ff00ff>{0}</color>|<color=#ffffffff>{1}</color>";
            if (current < _required)  txt = "<color=#ff0000ff>{0}</color>|<color=#ffffffff>{1}</color>";
            txt = string.Format(txt, current, _required);

            label.GetComponent<Text>().text = txt;
        }

        public ItemId GetItemId()
        {
            return _itemId;
        }
    }
}
