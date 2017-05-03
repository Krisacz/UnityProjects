using System.Collections.Generic;
using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.UI;

public class ProcessorController : InteractController
{
    #region PROPERTIES
    private InventoryController _inputSlots;
    private InventoryController _inProcess;
    private InventoryController _outputSlots;

    private CraftingTimerController _craftingTimerController;

    private List<Blueprint> _blueprints;
    private float _enqueueDelay;
    private Blueprint _blueprintInProcess;
    #endregion

    #region INIT
    public override void Init(string title)
    {
        //Cache inventories
        _inputSlots = transform.FindChild("InputSlotsPanel").GetComponent<InventoryController>();
        _inProcess = transform.FindChild("InProcessPanel").GetComponent<InventoryController>();
        _outputSlots = transform.FindChild("OutputPanel").GetComponent<InventoryController>();

        //Cache process timer
        _craftingTimerController = transform.FindChild("InProcessPanel").FindChild("CraftingTimer").GetComponent<CraftingTimerController>();

        //Prep vars
        _blueprints = new List<Blueprint>();
        _enqueueDelay = 1f;
        _blueprintInProcess = null;

        //Customize Title
        transform.FindChild("TitleText").gameObject.GetComponent<Text>().text = title;
    }
    #endregion

    #region ADD BLUEPRINT
    public override void AddBlueprint(Blueprint blueprint)
    {
        _blueprints.Add(blueprint);
    }
    #endregion
    
    #region UPDATE
    public void Update()
    {
        var deltaTime = Time.deltaTime;
        
        //Process current item
        if (_blueprintInProcess != null)
        {
            //Update timer until it's done
            if (!_craftingTimerController.UpdateProgress(deltaTime)) return;

            //Processing finished - attempt to output item
            if(_outputSlots.FreeSlots() == 0) return;

            //Output ready item
            var currentItem = _blueprintInProcess.ResultItemId;
            var currentCount = _blueprintInProcess.ResultItemCount;
            _inProcess.RemoveItem(currentItem, currentCount);
            _outputSlots.AddItem(currentItem, currentCount);
            _blueprintInProcess = null;
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
        if (_inputSlots.AnyItems() == 0) return;

        //Already occupied
        if(_inProcess.AnyItems() > 0) return;
        
        //Check if any items in input slots fit as a requirement in any blueprints
        Blueprint selectedBlueprint = null;
        foreach (var blueprint in _blueprints)
        {
            //Check if we have all required items 
            if (_inputSlots.Check(blueprint.Requirements).Count == 0)
            {
                selectedBlueprint = blueprint;
                break;
            }
        }

        //If we did not find anything
        if (selectedBlueprint == null) return;

        //Enqueue blueprint
        //  - Remove Requirements
        foreach (var requirement in selectedBlueprint.Requirements) _inputSlots.RemoveItem(requirement.Key, requirement.Value);

        //  - Add currently crafting item
        _inProcess.AddItem(selectedBlueprint.ResultItemId, selectedBlueprint.ResultItemCount);

        //  - Set processing timer
        _craftingTimerController.SetTimer(selectedBlueprint.CraftingTime);

        //  - Set current
        _blueprintInProcess = selectedBlueprint;
    }
    #endregion
}
