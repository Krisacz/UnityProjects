using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class BlueprintGroupController : MonoBehaviour
    {
        private BlueprintGroup _blueprintGroup;

        public BlueprintGroup GetBlueprintGroup()
        {
            return _blueprintGroup;
        }

        public void SetBlueprintGroup(BlueprintGroup blueprintGroup)
        {
            _blueprintGroup = blueprintGroup;
        }

        public void OnSelected()
        {
            var cc = GetComponentInParent<CraftingController>();
            cc.OnBlueprintGroupSelected(_blueprintGroup);
        }
    }
}
