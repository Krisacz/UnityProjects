using Assets.Scripts.Models;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

namespace Assets
{
    public class CraftingController : MonoBehaviour
    {
        private Dictionary<BlueprintsGroup, List<Blueprint>> _blueprints;
        public GameObject BlueprintsGroupsPanel;
        public GameObject ItemBlueprintsPanel;

        void Start ()
        {
            _blueprints = new Dictionary<BlueprintsGroup, List<Blueprint>>();
            AddBlueprintGroup(BlueprintsGroup.Ores);
            AddBlueprintGroup(BlueprintsGroup.Tools);
            AddBlueprintGroup(BlueprintsGroup.Machines);
            AddBlueprintGroup(BlueprintsGroup.Structures);

            var blueprints = BlueprintsDatabase.GetGroupBlueprints(BlueprintsGroup.Ores);
            foreach (var blueprint in blueprints) AddItemBlueprint(blueprint);
        }

        public void AddBlueprintGroup(BlueprintsGroup group)
        {
            if (_blueprints.ContainsKey(group))
            {
                Log.Warn("CraftingController", "AddBlueprintGroup", 
                    string.Format("Blueprints group = {0} already exist!", group));
                return;
            }

            _blueprints.Add(group, new List<Blueprint>());

            //Create game object inside blueprints groups panel
            var go = GameObjectFactory.FromBlueprintGroup(group);
            go.transform.SetParent(BlueprintsGroupsPanel.transform);
        }

        public void AddItemBlueprint(Blueprint blueprint)
        {
            if (!_blueprints.ContainsKey(blueprint.BlueprintsGroup))
            {
                Log.Warn("CraftingController", "AddItemBlueprint",
                    string.Format("Blueprint group = {0} doesn't exist! Can not add blueprint for ItemId = {1}",
                    blueprint.BlueprintsGroup, blueprint.ResultItemId));
                return;
            }

            _blueprints[blueprint.BlueprintsGroup].Add(blueprint);

            //Create game object inside blueprints groups panel
            var go = GameObjectFactory.FromItemBlueprint(blueprint);
            go.transform.SetParent(ItemBlueprintsPanel.transform);
        }
    }
}
