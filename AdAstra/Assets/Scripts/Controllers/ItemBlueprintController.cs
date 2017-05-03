using System;
using Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Controllers
{
    public class ItemBlueprintController : MonoBehaviour
    {
        public BlueprintOnClick ActionOnClick;
        public enum BlueprintOnClick { Select, RemoveFromQueue }

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
            var cc = GetComponentInParent<CraftingController>();

            switch (ActionOnClick)
            {
                case BlueprintOnClick.Select:
                    cc.OnBlueprintSelected(_blueprint);
                    return;

                case BlueprintOnClick.RemoveFromQueue:
                    var canRemove = GameController.InventoryController.CheckAddItem(_blueprint.Requirements);
                    if (canRemove.Count == 0)
                    {
                        foreach (var requirement in _blueprint.Requirements)
                        {
                            GameController.InventoryController.AddItem(requirement.Key, requirement.Value);
                        }
                        cc.UpdateSelectedBlueprintRequirements();
                        Destroy(this.transform.gameObject);
                    }
                    else
                    {
                        Log.Warn("ItemBlueprintController", "OnSelected",
                            "Can not remove queued blueprint, not enough space in the inventory.");
                    }
                    return;

                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}
