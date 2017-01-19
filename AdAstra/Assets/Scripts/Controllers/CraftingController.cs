using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Db;
using Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{
    public class CraftingController : MonoBehaviour
    {
        public GameObject BlueprintsGroupsPanel;
        public GameObject ItemBlueprintsPanel;
        public GameObject SelectedBlueprint;
        public GameObject InventoryController;

        private Dictionary<BlueprintGroup, List<Blueprint>> _blueprints;
        private BlueprintGroup _selectedGroup = BlueprintGroup.None;
        private Blueprint _selectedBlueprint = null;
        private readonly List<GameObject> _selectedGroupBlueprints = new List<GameObject>();

        #region START
        void Start ()
        {
            _blueprints = new Dictionary<BlueprintGroup, List<Blueprint>>();
            
            //TODO Debug init with example blueprints
            //var toolsBlueprints = BlueprintsDatabase.GetGroupBlueprints(BlueprintGroup.Tools);
            //foreach (var blueprint in toolsBlueprints) AddItemBlueprint(blueprint);

            var structuresBlueprints = BlueprintsDatabase.GetGroupBlueprints(BlueprintGroup.Structures);
            foreach (var blueprint in structuresBlueprints) AddItemBlueprint(blueprint);

            var machinesBlueprints = BlueprintsDatabase.GetGroupBlueprints(BlueprintGroup.Machines);
            foreach (var blueprint in machinesBlueprints) AddItemBlueprint(blueprint);

            var oreBlueprints = BlueprintsDatabase.GetGroupBlueprints(BlueprintGroup.Ores);
            foreach (var blueprint in oreBlueprints) AddItemBlueprint(blueprint);

            UpdateBlueprintGroupsViews();

            ClearSelectedBlueprintPanel();
            
        }
        #endregion

        #region BLUEPRINT GROUP
        //Add only "model"
        private void AddBlueprintGroup(BlueprintGroup group)
        {
            if (_blueprints.ContainsKey(group))
            {
                Log.Warn("CraftingController", "AddBlueprintGroup", 
                    string.Format("Blueprints group = {0} already exist!", group));
                return;
            }

            _blueprints.Add(group, new List<Blueprint>());
        }

        //Adds view (game objects)
        private void UpdateBlueprintGroupsViews()
        {
            //Get current game object 
            var allGroups = BlueprintsGroupsPanel.GetComponentsInChildren<BlueprintGroupController>();

            //Make sure all blueprint groups are visible (gameObjects) in cc
            foreach (var blueprintGroup in _blueprints.Keys)
            {
                if(allGroups.All(x => x.GetBlueprintGroup() != blueprintGroup))
                {
                    //Create game object inside blueprints groups panel
                    GameObjectFactory.FromBlueprintGroup(blueprintGroup, BlueprintsGroupsPanel.transform);
                }
            }
        }

        public void OnBlueprintGroupSelected(BlueprintGroup blueprintGroup)
        {
            //If  this group is already selected ignore...
            if (_selectedGroup == blueprintGroup) return;

            //Set currently selected group
            _selectedGroup = blueprintGroup;

            //First lets remove existing blueprints from view 
            foreach (var go in _selectedGroupBlueprints)
            {
                if (go == null) continue;
                go.SetActive(false);
                Destroy(go);
            }
            _selectedGroupBlueprints.Clear();

            //Side panel
            ClearSelectedBlueprintPanel();

            //If newly selected group is NONE - stop here
            if (blueprintGroup == BlueprintGroup.None) return;

            //Add blueprints for current group
            foreach (var blueprint in _blueprints[blueprintGroup])
            {
                var go = GameObjectFactory.FromItemBlueprint(blueprint, ItemBlueprintsPanel.transform);
                _selectedGroupBlueprints.Add(go);
            }
        }
        #endregion

        #region BLUEPRINT
        //Add only "model"
        private void AddItemBlueprint(Blueprint blueprint)
        {
            //Add blueprint group if doesn't exist already
            if (!_blueprints.ContainsKey(blueprint.BlueprintGroup))
                AddBlueprintGroup(blueprint.BlueprintGroup);

            //Add it to private dictionary
            _blueprints[blueprint.BlueprintGroup].Add(blueprint);
        }

        public void OnBlueprintSelected(Blueprint blueprint)
        {
            UpdateSelectedBlueprintPanel(blueprint);
            UpdateSelectedBlueprintRequirements();
        }
        #endregion

        #region SELECTED BLUEPRINT
        private void ClearSelectedBlueprintPanel()
        {
            //Clear Title
            var title = SelectedBlueprint.transform.FindChild("Title");
            title.GetComponent<Text>().text = string.Empty;
            title.gameObject.SetActive(false);

            //Clear Description
            var description = SelectedBlueprint.transform.FindChild("Description");
            description.GetComponent<Text>().text = string.Empty;
            description.gameObject.SetActive(false);

            //Requirements
            var requirements = SelectedBlueprint.transform.FindChild("Requirements");
            requirements.gameObject.SetActive(false);
            var children = requirements.transform.childCount;
            for (var i = 0; i < children; ++i) Destroy(requirements.transform.GetChild(i).gameObject);

            //Crafting Time
            var craftingTimer = SelectedBlueprint.transform.FindChild("CraftingTimer");
            craftingTimer.GetComponent<CraftingTimerController>().SetTimer(0.0f);
            craftingTimer.gameObject.SetActive(false);

            //Craft button
            var craftButton = SelectedBlueprint.transform.FindChild("CraftButton");
            craftButton.gameObject.SetActive(false);

            //Select blueprint
            var selectBlueprint = SelectedBlueprint.transform.FindChild("SelectBlueprint");
            selectBlueprint.gameObject.SetActive(true);
        }

        private void UpdateSelectedBlueprintPanel(Blueprint blueprint)
        {
            if (blueprint == null)
            {
                Log.Error("CraftingController", "UpdateSelectedBlueprintPanel", "Blueprint = null");
                return;
            }

            if (_selectedBlueprint == blueprint) return;

            //Update current selection
            _selectedBlueprint = blueprint;

            //Clear Title
            var title = SelectedBlueprint.transform.FindChild("Title");
            title.GetComponent<Text>().text = blueprint.Title;
            title.gameObject.SetActive(true);

            //Clear Description
            var description = SelectedBlueprint.transform.FindChild("Description");
            description.GetComponent<Text>().text = blueprint.Description;
            description.gameObject.SetActive(true);

            //Requirements
            var requirements = SelectedBlueprint.transform.FindChild("Requirements");
            var children = requirements.transform.childCount;
            for (var i = 0; i < children; ++i) Destroy(requirements.transform.GetChild(i).gameObject);
            foreach (var r in blueprint.Requirements)
            {
                var go = GameObjectFactory.FromBlueprintRequiredItem(r.Key, 
                    r.Value, requirements.transform);
                go.GetComponent<RequiredItemController>().SetRequirement(r.Key, r.Value);
            }
            requirements.gameObject.SetActive(true);

            //Crafting Time
            var craftingTimer = SelectedBlueprint.transform.FindChild("CraftingTimer");
            craftingTimer.GetComponent<CraftingTimerController>().SetTimer(blueprint.CraftingTime);
            craftingTimer.gameObject.SetActive(true);

            //Craft button
            var craftButton = SelectedBlueprint.transform.FindChild("CraftButton");
            craftButton.gameObject.SetActive(true);

            //Select blueprint
            var selectBlueprint = SelectedBlueprint.transform.FindChild("SelectBlueprint");
            selectBlueprint.gameObject.SetActive(false);
        }

        public void UpdateSelectedBlueprintRequirements()
        {
            //Inventory Controller
            var ic = InventoryController.GetComponent<InventoryController>();
            
            //Requirements
            var requirements = SelectedBlueprint.transform.FindChild("Requirements");
            var children = requirements.transform.childCount;
            for (var i = 0; i < children; ++i)
            {
                var go = requirements.transform.GetChild(i).gameObject;
                var ric = go.GetComponent<RequiredItemController>();
                var itemId = ric.GetItemId();
                var stock = ic.GetCount(itemId);
                ric.UpdateStackCountText(stock);
            }
        }
        #endregion
    }
}
