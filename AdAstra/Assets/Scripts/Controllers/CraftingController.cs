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
        public GameObject CraftingCurrentPanel;
        public GameObject CraftingQueuePanel;

        [Range(0,10)]
        public int MaxQueuedBlueprints = 2;

        private Dictionary<BlueprintGroup, List<Blueprint>> _blueprints;
        private BlueprintGroup _selectedGroup = BlueprintGroup.None;
        private List<GameObject> _selectedGroupBlueprints;
        private Blueprint _selectedBlueprint = null;
        private List<GameObject> _selectedBlueprintRequirements;
        private bool _craftingInProcess = false;
        private Blueprint _blueprintInProcess = null;
        private CraftingTimerController _craftingTimerController;
        private float _enqueueDelay = 1f;
        private InventoryController _outputInventory;

        #region START
        void Start ()
        {
            _blueprints = new Dictionary<BlueprintGroup, List<Blueprint>>();
            _selectedGroupBlueprints = new List<GameObject>();
            _selectedBlueprintRequirements = new List<GameObject>();
            _craftingTimerController = CraftingCurrentPanel.transform.FindChild("CraftingTimer")
                .GetComponent<CraftingTimerController>();
            _outputInventory = CraftingCurrentPanel.GetComponent<InventoryController>();

            //TODO Debug init with example blueprints
            var oreBlueprints = BlueprintsDatabase.GetGroupBlueprints(BlueprintGroup.Ores);
            foreach (var blueprint in oreBlueprints) AddItemBlueprint(blueprint);

            var structuresBlueprints = BlueprintsDatabase.GetGroupBlueprints(BlueprintGroup.Structures);
            foreach (var blueprint in structuresBlueprints) AddItemBlueprint(blueprint);

            var toolsBlueprints = BlueprintsDatabase.GetGroupBlueprints(BlueprintGroup.Tools);
            foreach (var blueprint in toolsBlueprints) AddItemBlueprint(blueprint);

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
                    GameObjectFactory.BlueprintGroup(blueprintGroup, BlueprintsGroupsPanel.transform);
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
                var go = GameObjectFactory.ItemBlueprint(blueprint, ItemBlueprintsPanel.transform);
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
            var craftingTimer = SelectedBlueprint.transform.FindChild("CraftingTime");
            craftingTimer.GetComponent<Text>().text = "Crafting Time: 0.0s";
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
            
            //Update current selection
            _selectedBlueprint = blueprint;

            //Get result item for few properties
            var resultItem = ItemsDatabase.Get(blueprint.ResultItemId);

            //Clear Title
            var title = SelectedBlueprint.transform.FindChild("Title");
            var c = blueprint.ResultItemCount;
            title.GetComponent<Text>().text = 
                string.Format("{0} {1}", resultItem.Title,
                c > 1 ? string.Format("(x{0})", c) : string.Empty);
            title.gameObject.SetActive(true);

            //Clear Description
            var description = SelectedBlueprint.transform.FindChild("Description");
            description.GetComponent<Text>().text = resultItem.Description;
            description.gameObject.SetActive(true);

            //Requirements
            var requirements = SelectedBlueprint.transform.FindChild("Requirements");
            var children = requirements.transform.childCount;
            for (var i = 0; i < children; ++i) Destroy(requirements.transform.GetChild(i).gameObject);
            _selectedBlueprintRequirements.Clear();
            foreach (var r in blueprint.Requirements)
            {
                var go = GameObjectFactory.BlueprintRequiredItem(r.Key, 
                    r.Value, requirements.transform);
                go.GetComponent<RequiredItemController>().SetRequirement(r.Key, r.Value);
                _selectedBlueprintRequirements.Add(go);
            }
            requirements.gameObject.SetActive(true);

            //Crafting Time
            var craftingTimer = SelectedBlueprint.transform.FindChild("CraftingTime");
            craftingTimer.GetComponent<Text>().text = 
                string.Format("Crafting Time: {0}s", blueprint.CraftingTime.ToString("f1"));
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
            //Make sure we have anything selected
            if(_selectedBlueprintRequirements == null 
                || _selectedBlueprintRequirements.Count <= 0 ) return;

            //Can craft selected item?
            var canCraft = true;

            //Player InventoryController
            var ic = InventoryController.GetComponent<InventoryController>();

            //Requirements
            foreach (var requirementGo in _selectedBlueprintRequirements)
            {
                var ric = requirementGo.GetComponent<RequiredItemController>();
                var itemId = ric.GetItemId();
                var count = ric.GetCount();
                var stock = ic.GetCount(itemId);
                ric.UpdateStackCountText(stock);

                //Set flag if we can not craft current item (if we are missing any item)
                if (canCraft) canCraft = stock >= count;
            }

            //Craft button
            SetCraftButton(canCraft);
        }

        private void SetCraftButton(bool canCraft)
        {
            var craftButton = SelectedBlueprint.transform.FindChild("CraftButton");
            craftButton.GetComponent<Button>().interactable = canCraft;
            var text = craftButton.transform.FindChild("Text").GetComponent<Text>();
            text.text = canCraft ? "Craft" : "Missing Items";
            text.color = canCraft ? Color.black : Color.red;
        }

        #endregion

        #region CRAFT BUTTON
        public void Craft()
        {
            //If crafting is already in process and we DON'T HAVE space in queue we cancel
            if (_craftingInProcess && CraftingQueuePanel.transform.childCount >= MaxQueuedBlueprints)
                return;

            //If crafting is already in process and we HAVE space in queue we insert item to the queue
            if (_craftingInProcess && CraftingQueuePanel.transform.childCount < MaxQueuedBlueprints)
            {
                var go = GameObjectFactory.ItemBlueprint(_selectedBlueprint,
                    CraftingQueuePanel.transform, false);
                go.transform.SetAsFirstSibling();
                RemoveRequiredItems();
                UpdateSelectedBlueprintRequirements();
                return;
            }

            //If crafting is not in process and we HAVE space in queue but one space in output
            if (!_craftingInProcess && CraftingQueuePanel.transform.childCount < MaxQueuedBlueprints
                && _outputInventory.FreeSlots() == 0)
            {
                var go = GameObjectFactory.ItemBlueprint(_selectedBlueprint,
                    CraftingQueuePanel.transform, false);
                go.transform.SetAsFirstSibling();
                RemoveRequiredItems();
                UpdateSelectedBlueprintRequirements();
                return;
            }

            //If crafting is not in process and output inventory is occupied then we cancel
            if (!_craftingInProcess && CraftingQueuePanel.transform.childCount >= MaxQueuedBlueprints
                && _outputInventory.FreeSlots() == 0) return;

            //If crafting is not in process and output inventory is free insert & start crafting
            if (!_craftingInProcess && _outputInventory.FreeSlots() > 0)
            {
                UpdateCurrentCraftingItem(true, _selectedBlueprint);
                RemoveRequiredItems();
                UpdateSelectedBlueprintRequirements();
            }
        }
        #endregion

        #region REMOVE REQUIRED ITEMS 
        private void RemoveRequiredItems()
        {
            foreach (var r in _selectedBlueprint.Requirements)
            {
                InventoryController.GetComponent<InventoryController>().RemoveItem(r.Key, r.Value);
            }
        }
        #endregion

        #region UPDATE CURRENT CRAFTING ITEM
        private void UpdateCurrentCraftingItem(bool isCraftingStart, Blueprint blueprint)
        {
            //Set Crafting Item - current or empty
            var craftingItem = CraftingCurrentPanel.transform.FindChild("CraftingItem");
            var image = craftingItem.GetComponent<Image>();
            if (isCraftingStart)
            {
                var item = ItemsDatabase.Get(blueprint.ResultItemId);
                var sprite = SpritesDatabase.Get(item.SpriteName);
                image.sprite = sprite;
                image.color = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
            }
            else
            {
                image.sprite = null;
                image.color = new Color32(0xA0, 0xA0, 0xA0, 0xFF);
            }

            //Set Crafting timer
            var craftingTimer = CraftingCurrentPanel.transform.FindChild("CraftingTimer");
            var craftingTimerController = craftingTimer.GetComponent<CraftingTimerController>();
            craftingTimerController.SetTimer(isCraftingStart ? blueprint.CraftingTime : 0.0f);

            //Control process
            _craftingInProcess = isCraftingStart;

            //Set new blueprint or output item
            if (isCraftingStart)
            {
                _blueprintInProcess = blueprint;
            }
            else
            {
                _outputInventory.AddItem(_blueprintInProcess.ResultItemId, _blueprintInProcess.ResultItemCount);
            }
        }
        #endregion

        #region UPDATE
        public void Update()
        {
            var deltaTime = Time.deltaTime;

            if (_craftingInProcess)
            {
                if (!_craftingTimerController.UpdateProgress(deltaTime)) return;
                UpdateCurrentCraftingItem(false, null);
                EnqueueNextBlueprint();
            }
            else
            {
                //Small delay before re-quering enqueue to not overkill update
                _enqueueDelay -= deltaTime;
                if(_enqueueDelay > 0.0f) return;
                EnqueueNextBlueprint();
                _enqueueDelay = 1.0f;
            }
        }
        #endregion

        #region ENQUEUE NEXT BLUEPRINT
        private void EnqueueNextBlueprint()
        {
            //Check if our output is clear
            if (_outputInventory.FreeSlots() == 0) return;

            //Nothing to enqueue
            if (CraftingQueuePanel.transform.childCount == 0) return;

            //Get 1st child and set it as current crafting item
            var go = CraftingQueuePanel.transform.GetChild(0).gameObject;
            var blueprint = go.GetComponent<ItemBlueprintController>().GetBlueprint();
            UpdateCurrentCraftingItem(true, blueprint);
            Destroy(go);
        }
        #endregion
    }
}
