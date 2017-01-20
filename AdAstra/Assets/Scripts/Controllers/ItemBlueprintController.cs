using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class ItemBlueprintController : MonoBehaviour
    {
        public bool CanBeSelected = true;
        private Blueprint _blueprint;

        public Blueprint GetBlueprint()
        {
            return _blueprint;
        }

        public void SetBlueprint(Blueprint blueprint)
        {
            _blueprint = blueprint;
        }

        public void OnSelected()
        {
            if(!CanBeSelected) return;
            var cc = GetComponentInParent<CraftingController>();
            cc.OnBlueprintSelected(_blueprint);
        }
    }
}
