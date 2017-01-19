using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class ItemBlueprintController : MonoBehaviour
    {
        private Blueprint _blueprint;

        public Blueprint GetBlueprintGroup()
        {
            return _blueprint;
        }

        public void SetBlueprint(Blueprint blueprint)
        {
            _blueprint = blueprint;
        }

        public void OnSelected()
        {
            var cc = GetComponentInParent<CraftingController>();
            cc.OnBlueprintSelected(_blueprint);
        }
    }
}
