using System;
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
        //TODO Sort it in START same as other refs
        private InventoryController _inventoryController;
        private GameObject _tileText;
        private GameObject _blueprintsGroupsPanel;
        private GameObject _itemBlueprintsPanel;
        private GameObject _selectedBlueprintPanel;
        private GameObject _craftingCurrentPanel;
        private GameObject _craftingQueuePanel;
        private GameObject _progressText;
        private GameObject _doWorkButton;

        private Dictionary<string, List<Blueprint>> _blueprints;
        private string _selectedGroup = string.Empty;
        private List<GameObject> _selectedGroupBlueprints;
        private Blueprint _selectedBlueprint = null;
        private List<GameObject> _selectedBlueprintRequirements;
        private bool _craftingInProcess = false;
        private Blueprint _blueprintInProcess = null;
        private CraftingTimerController _craftingTimerController;
        private float _enqueueDelay = 1f;
        private InventoryController _outputInventory;
        
        private int _maxQueuedBlueprints = 3;
        private string _progressTextStr;
        private InteractUIType _interactUIType;

        #region INIT
        public void Init(string title, string progressText, InteractUIType uiType)
        {
            _inventoryController = GameObject.FindGameObjectWithTag("GameControllerTag").transform.GetComponent<InventoryController>();
            _tileText = transform.FindChild("TitleText").gameObject;
            _blueprintsGroupsPanel = transform.FindChild("BlueprintGroups").transform.FindChild("BlueprintGroupsPanel").gameObject;
            _itemBlueprintsPanel = transform.FindChild("ItemBlueprints").transform.FindChild("ItemBlueprintsPanel").gameObject;
            _selectedBlueprintPanel = transform.FindChild("SelectedBlueprint").gameObject;
            _craftingCurrentPanel = transform.FindChild("CurrentPanel").gameObject;
            _craftingQueuePanel = transform.FindChild("Queue").transform.FindChild("QueuePanel").gameObject;
            _progressText = transform.FindChild("SelectedBlueprint").transform.FindChild("CraftingTime").gameObject;
            _doWorkButton = transform.FindChild("SelectedBlueprint").transform.FindChild("CraftButton").gameObject;
            _blueprints = new Dictionary<string, List<Blueprint>>();
            _selectedGroupBlueprints = new List<GameObject>();
            _selectedBlueprintRequirements = new List<GameObject>();
            _craftingTimerController = transform.FindChild("CraftingTimer").GetComponent<CraftingTimerController>();
            _outputInventory = _craftingCurrentPanel.GetComponent<InventoryController>();

            //TODO Debug init with example blueprints
            AddItemBlueprint(BlueprintsDatabase.GetBlueprint(ItemId.BasicFoundation));
            AddItemBlueprint(BlueprintsDatabase.GetBlueprint(ItemId.BasicFloor));
            AddItemBlueprint(BlueprintsDatabase.GetBlueprint(ItemId.BasicWall));

            AddItemBlueprint(BlueprintsDatabase.GetBlueprint(ItemId.Assembler));
            AddItemBlueprint(BlueprintsDatabase.GetBlueprint(ItemId.Constructor));

            UpdateBlueprintGroupsViews();
            ClearSelectedBlueprintPanel();

            _tileText.GetComponent<Text>().text = title;
            _progressTextStr = progressText;
            _doWorkButton.GetComponentInChildren<Text>().text = progressText;
            _interactUIType = uiType;
        }
        #endregion

        #region BLUEPRINT GROUP

        //Adds view (game objects)
        private void UpdateBlueprintGroupsViews()
        {
            //Get current game object 
            var allGroups = _blueprintsGroupsPanel.GetComponentsInChildren<BlueprintGroupController>();

            //Make sure all blueprint groups are visible (gameObjects) in cc
            foreach (var blueprintGroup in _blueprints.Keys)
            {
                if (allGroups.All(x => x.GetBlueprintGroup() != blueprintGroup))
                {
                    //Create game object inside blueprints groups panel
                    GameObjectFactory.BlueprintGroup(blueprintGroup, _blueprintsGroupsPanel.transform);
                }
            }
        }

        public void OnBlueprintGroupSelected(string blueprintGroup)
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

            //Add blueprints for current group
            foreach (var blueprint in _blueprints[blueprintGroup])
            {
                var go = GameObjectFactory.ItemBlueprint(blueprint, _itemBlueprintsPanel.transform, ItemBlueprintController.BlueprintOnClick.Select);
                _selectedGroupBlueprints.Add(go);
            }
        }

        #endregion

        #region BLUEPRINT
        public void AddItemBlueprint(Blueprint blueprint)
        {
            var blueprintGroup = ItemsDatabase.Get(blueprint.ResultItemId).ItemFunction.ToString().ToLower();

            //Add blueprint group if doesn't exist already
            if (!_blueprints.ContainsKey(blueprintGroup)) _blueprints.Add(blueprintGroup, new List<Blueprint>());

            //Add it to dictionary
            _blueprints[blueprintGroup].Add(blueprint);
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
            var title = _selectedBlueprintPanel.transform.FindChild("Title");
            title.GetComponent<Text>().text = string.Empty;
            title.gameObject.SetActive(false);

            //Clear Description
            var description = _selectedBlueprintPanel.transform.FindChild("Description");
            description.GetComponent<Text>().text = string.Empty;
            description.gameObject.SetActive(false);

            //Requirements
            var requirements = _selectedBlueprintPanel.transform.FindChild("Requirements");
            requirements.gameObject.SetActive(false);
            var children = requirements.transform.childCount;
            for (var i = 0; i < children; ++i) Destroy(requirements.transform.GetChild(i).gameObject);

            //Crafting Time
            var craftingTimer = _selectedBlueprintPanel.transform.FindChild("CraftingTime");
            craftingTimer.GetComponent<Text>().text = string.Format("{0} Time: 0.0s", _progressTextStr);
            craftingTimer.gameObject.SetActive(false);

            //Craft button
            var craftButton = _selectedBlueprintPanel.transform.FindChild("CraftButton");
            craftButton.gameObject.SetActive(false);

            //Select blueprint
            var selectBlueprint = _selectedBlueprintPanel.transform.FindChild("SelectBlueprint");
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
            var title = _selectedBlueprintPanel.transform.FindChild("Title");
            var c = blueprint.ResultItemCount;
            title.GetComponent<Text>().text = string.Format("{0} {1}", resultItem.Title, c > 1 ? string.Format("(x{0})", c) : string.Empty);
            title.gameObject.SetActive(true);

            //Clear Description
            var description = _selectedBlueprintPanel.transform.FindChild("Description");
            description.GetComponent<Text>().text = resultItem.Description;
            description.gameObject.SetActive(true);

            //Requirements
            var requirements = _selectedBlueprintPanel.transform.FindChild("Requirements");
            var children = requirements.transform.childCount;
            for (var i = 0; i < children; ++i) Destroy(requirements.transform.GetChild(i).gameObject);
            _selectedBlueprintRequirements.Clear();
            foreach (var r in blueprint.Requirements)
            {
                var go = GameObjectFactory.BlueprintRequiredItem(r.Key, r.Value, requirements.transform);
                go.GetComponent<RequiredItemController>().SetRequirement(r.Key, r.Value);
                _selectedBlueprintRequirements.Add(go);
            }
            requirements.gameObject.SetActive(true);

            //Crafting Time
            var craftingTimer = _selectedBlueprintPanel.transform.FindChild("CraftingTime");
            craftingTimer.GetComponent<Text>().text = string.Format("Crafting Time: {0}s", blueprint.CraftingTime.ToString("f1"));
            craftingTimer.gameObject.SetActive(true);

            //Craft button
            var craftButton = _selectedBlueprintPanel.transform.FindChild("CraftButton");
            craftButton.gameObject.SetActive(true);

            //Select blueprint
            var selectBlueprint = _selectedBlueprintPanel.transform.FindChild("SelectBlueprint");
            selectBlueprint.gameObject.SetActive(false);
        }

        public void UpdateSelectedBlueprintRequirements()
        {
            //Make sure we have anything selected
            if (_selectedBlueprintRequirements == null || _selectedBlueprintRequirements.Count <= 0) return;

            //Can craft selected item?
            var canCraft = true;

            //Requirements
            foreach (var requirementGo in _selectedBlueprintRequirements)
            {
                var ric = requirementGo.GetComponent<RequiredItemController>();
                var itemId = ric.GetItemId();
                var count = ric.GetCount();
                var stock = _inventoryController.GetCount(itemId);
                ric.UpdateStackCountText(stock);

                //Set flag if we can not craft current item (if we are missing any item)
                if (canCraft) canCraft = stock >= count;
            }

            //Craft button
            SetCraftButton(canCraft);
        }

        private void SetCraftButton(bool canCraft)
        {
            var craftButton = _selectedBlueprintPanel.transform.FindChild("CraftButton");
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
            if (_craftingInProcess && _craftingQueuePanel.transform.childCount >= _maxQueuedBlueprints)
                return;

            //If crafting is already in process and we HAVE space in queue we insert item to the queue
            if (_craftingInProcess && _craftingQueuePanel.transform.childCount < _maxQueuedBlueprints)
            {
                var go = GameObjectFactory.ItemBlueprint(_selectedBlueprint, _craftingQueuePanel.transform, ItemBlueprintController.BlueprintOnClick.RemoveFromQueue);
                //go.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                go.transform.SetAsFirstSibling();
                RemoveRequiredItems();
                UpdateSelectedBlueprintRequirements();
                return;
            }

            //If crafting is not in process and we HAVE space in queue but one space in output
            if (!_craftingInProcess && _craftingQueuePanel.transform.childCount < _maxQueuedBlueprints && _outputInventory.FreeSlots() == 0)
            {
                var go = GameObjectFactory.ItemBlueprint(_selectedBlueprint, _craftingQueuePanel.transform, ItemBlueprintController.BlueprintOnClick.RemoveFromQueue);
                go.transform.SetAsFirstSibling();
                RemoveRequiredItems();
                UpdateSelectedBlueprintRequirements();
                return;
            }

            //If crafting is not in process and output inventory is occupied then we cancel
            if (!_craftingInProcess && _craftingQueuePanel.transform.childCount >= _maxQueuedBlueprints && _outputInventory.FreeSlots() == 0) return;

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
                _inventoryController.RemoveItem(r.Key, r.Value);
            }
        }

        #endregion

        #region UPDATE CURRENT CRAFTING ITEM

        private void UpdateCurrentCraftingItem(bool isCraftingStart, Blueprint blueprint)
        {
            //Set Crafting Item - current or empty
            var craftingItem = _craftingCurrentPanel.transform.FindChild("CraftingItem");
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
            _craftingTimerController.SetTimer(isCraftingStart ? blueprint.CraftingTime : 0.0f);

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
                if (_enqueueDelay > 0.0f) return;
                EnqueueNextBlueprint();
                _enqueueDelay = 1.0f;
            }
        }

        #endregion

        #region ENQUEUE NEXT BLUEPRINT

        private void EnqueueNextBlueprint()
        {
            //Nothing to enqueue
            if (_craftingQueuePanel.transform.childCount == 0) return;

            //Check if our output is clear
            if (_outputInventory.FreeSlots() == 0) return;

            //Get 1st child and set it as current crafting item
            var firstItemToCraft = _craftingQueuePanel.transform.GetChild(0).gameObject;
            var itemBlueprintController = firstItemToCraft.GetComponent<ItemBlueprintController>();
            var blueprint = itemBlueprintController.GetBlueprint();
            UpdateCurrentCraftingItem(true, blueprint);
            Destroy(firstItemToCraft);
        }

        #endregion

        #region GET PLAYER INVENTORY CONTROLLER
        public InventoryController GetPlayerInventoryController()
        {
            return _inventoryController;
        }
        #endregion
    }
}
