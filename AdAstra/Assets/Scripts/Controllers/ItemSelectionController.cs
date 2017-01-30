using Assets.Scripts.Models;
using Assets.Scripts.Views;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class ItemSelectionController : MonoBehaviour
    {
        private static Transform _slotsPanel;
        private static int _curentlySelectedIndex;
        private static Transform _selection;

        public void Start()
        {
            _slotsPanel = this.transform.parent;
            _curentlySelectedIndex = 0;
            _selection = this.transform;
            //UpdateSelection(_curentlySelectedIndex);
        }

        public void Update()
        {
            if (Input.GetKey(KeyCode.LeftShift)) return;
            var scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0.0000000001f)
            {
                PreviousSlot();
            }
            else if (scroll < -0.0000000001f)
            {
                NextSlot();
            }
        }

        public static void UpdateSelection(int index)
        {
            //At this point we need to make sure we have correct index
            var slots = _slotsPanel.GetComponentsInChildren<ItemStackView>();
            if (index > slots.Length - 1)
            {
                Log.Error("ItemSelectionController", "UpdateSelection", 
                    string.Format("Incorrect slot index = {0}", index));
                return;
            }

            _selection.position = slots[index].transform.position;
            _curentlySelectedIndex = index;

            //If build mode is one - update build overlay
            if(BuildController.IsOn()) BuildController.RefreshBuildOverlay();
        }

        private static void NextSlot()
        {
            var slots = _slotsPanel.GetComponentsInChildren<ItemStackView>();
            _curentlySelectedIndex++;
            if (_curentlySelectedIndex >= slots.Length) _curentlySelectedIndex = 0;
            //Log.Info(_curentlySelectedIndex.ToString());
            UpdateSelection(_curentlySelectedIndex);
        }

        private static void PreviousSlot()
        {
            var slots = _slotsPanel.GetComponentsInChildren<ItemStackView>();
            _curentlySelectedIndex--;
            if (_curentlySelectedIndex < 0) _curentlySelectedIndex = slots.Length - 1;
            //Log.Info(_curentlySelectedIndex.ToString());
            UpdateSelection(_curentlySelectedIndex);
        }

        public static ItemStackView GetItemStackView()
        {
            var slots = _slotsPanel.GetComponentsInChildren<ItemStackView>();
            return slots[_curentlySelectedIndex];
        }

        public static ItemStack GetItemStack()
        {
            var itemStackView = GetItemStackView();
            return itemStackView.HasItem ? itemStackView.GetItemStack() : null;
        }

        public static Item GetItem()
        {
            var itemStack = GetItemStack();
            return itemStack != null ? itemStack.Item : null;
        }
    }
}
