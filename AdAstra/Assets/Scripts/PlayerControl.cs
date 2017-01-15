using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerControl : MonoBehaviour
    {
        public float Speed;
        public GameObject InventoryUI;

        private Rigidbody2D _rigidbody2D;
        private bool _inventoryVisible;

        void Start ()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            Movement();
            LookAtMouse();
        }

        void Update()
        {
            ToggleInventory();
        }

        private void LookAtMouse()
        {
            var objectPos = Camera.main.WorldToScreenPoint(transform.position);
            var dir = Input.mousePosition - objectPos;
            var angle = (Mathf.Atan2(dir.y, dir.x)*Mathf.Rad2Deg) - 90f;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        private void Movement()
        {
            var hAxis = Input.GetAxis("Horizontal");
            var vAxis = Input.GetAxis("Vertical");
            var forceX = hAxis*Speed*Time.deltaTime;
            var forceY = vAxis*Speed*Time.deltaTime;
            var force = new Vector2(forceX, forceY);
            _rigidbody2D.AddForce(force);
        }

        private void ToggleInventory()
        {
            if (!Input.GetKeyDown(KeyCode.E)) return;
            _inventoryVisible = !_inventoryVisible;
            InventoryUI.SetActive(_inventoryVisible);
            Log.Info("PlayerControl", "ToggleInventory", string.Format("_inventoryVisible = {0}", _inventoryVisible));
        }
    }
}
