using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class BlueprintGroupController : MonoBehaviour
    {
        private string _blueprintGroup;

        public string GetBlueprintGroup()
        {
            return _blueprintGroup;
        }

        public void SetBlueprintGroup(string blueprintGroup)
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
