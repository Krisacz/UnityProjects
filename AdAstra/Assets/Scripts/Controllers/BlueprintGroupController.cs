using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class BlueprintGroupController : MonoBehaviour
    {
        private ItemFunction _blueprintFunction;

        public ItemFunction GetBlueprintGroup()
        {
            return _blueprintFunction;
        }

        public void SetBlueprintGroup(ItemFunction blueprintFunction)
        {
            _blueprintFunction = blueprintFunction;
        }

        public void OnSelected()
        {
            var cc = GetComponentInParent<CraftingController>();
            cc.OnBlueprintGroupSelected(_blueprintFunction);
        }
    }
}
