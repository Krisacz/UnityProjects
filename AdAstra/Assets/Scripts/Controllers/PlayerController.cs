using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        public float Speed;
        public GameObject InventoryUI;
        public GameObject CraftingUI;

        private Rigidbody2D _rigidbody2D;
        private bool _inventoryVisible;
        private bool _craftingVisible;
        private static CircleCollider2D _playerCollider;
        private static LineController _lineController;

        void Start ()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _playerCollider = GetComponent<CircleCollider2D>();
            _lineController = GetComponentInChildren<LineController>();
        }

        void FixedUpdate()
        {
            Movement();
            LookAtMouse();
        }

        void Update()
        {
            ToggleInventory();
            ToggleCrafting();
            SelectInventorySlot();
        }

        private void Movement()
        {
            _rigidbody2D.velocity = new Vector2(
                Mathf.Lerp(0, Input.GetAxis("Horizontal") * Speed, 0.8f),
                Mathf.Lerp(0, Input.GetAxis("Vertical") * Speed, 0.8f));
        }

        private void LookAtMouse()
        {
            var objectPos = Camera.main.WorldToScreenPoint(transform.position);
            var dir = Input.mousePosition - objectPos;
            var angle = (Mathf.Atan2(dir.y, dir.x)*Mathf.Rad2Deg) - 90f;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        private void ToggleInventory()
        {
            if (!Input.GetKeyDown(KeyCode.E)) return;
            _inventoryVisible = !_inventoryVisible;
            InventoryUI.SetActive(_inventoryVisible);
            //Log.Info("PlayerController", "ToggleInventory", string.Format("_inventoryVisible = {0}", _inventoryVisible));
        }

        private void ToggleCrafting()
        {
            if (!Input.GetKeyDown(KeyCode.C)) return;
            _craftingVisible = !_craftingVisible;
            CraftingUI.SetActive(_craftingVisible);
            //Log.Info("PlayerController", "ToggleCrafting", string.Format("_craftingVisible = {0}", _craftingVisible));
        }

        private void SelectInventorySlot()
        {
            //Hot keys
            if (Input.GetKeyDown(KeyCode.Alpha1)) ItemSelectionController.SelectSlot(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) ItemSelectionController.SelectSlot(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) ItemSelectionController.SelectSlot(2);
            if (Input.GetKeyDown(KeyCode.Alpha4)) ItemSelectionController.SelectSlot(3);
            if (Input.GetKeyDown(KeyCode.Alpha5)) ItemSelectionController.SelectSlot(4);
            if (Input.GetKeyDown(KeyCode.Alpha6)) ItemSelectionController.SelectSlot(5);
            if (Input.GetKeyDown(KeyCode.Alpha7)) ItemSelectionController.SelectSlot(6);
            if (Input.GetKeyDown(KeyCode.Alpha8)) ItemSelectionController.SelectSlot(7);
            if (Input.GetKeyDown(KeyCode.Alpha9)) ItemSelectionController.SelectSlot(8);
            if (Input.GetKeyDown(KeyCode.Alpha0)) ItemSelectionController.SelectSlot(9);

            //Mouse scroll
            if (Input.GetKey(KeyCode.LeftShift)) return;
            var scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0.0000000001f)
            {
                ItemSelectionController.PreviousSlot();
            }
            else if (scroll < -0.0000000001f)
            {
                ItemSelectionController.NextSlot();
            }
        }

        public static Bounds GetBounds()
        {
            return _playerCollider.bounds;
        }

        public static LineController GetLineController()
        {
            return _lineController;
        }
    }
}
