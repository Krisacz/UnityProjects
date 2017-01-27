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

        void Start ()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _playerCollider = GetComponent<CircleCollider2D>();
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
        }

        public static Bounds GetBounds()
        {
            return _playerCollider.bounds;
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
            //Other way - speed 850
            var hAxis = Input.GetAxis("Horizontal");
            var vAxis = Input.GetAxis("Vertical");
            var forceX = hAxis * Speed * Time.deltaTime;
            var forceY = vAxis * Speed * Time.deltaTime;
            var force = new Vector2(forceX, forceY);
            _rigidbody2D.AddForce(force);

            //One way - set default speed to 3.5
            //var dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            //_rigidbody2D.MovePosition(_rigidbody2D.position + dir * Time.deltaTime * Speed);

            //Other way - set speed to 200
            //var xMovement = Input.GetAxis("Horizontal");
            //var yMovement = Input.GetAxis("Vertical");

            //var isX = Mathf.Abs(xMovement) > 0.5f;
            //var isY = Mathf.Abs(yMovement) > 0.5f;
            //if (!isX && !isY)
            //{
            //    _rigidbody2D.velocity = Vector2.zero;
            //    return;
            //}
            //var movement = new Vector2(xMovement, yMovement);
            //var heading = Vector2.zero;
            //if (movement.magnitude > 0) heading = movement.normalized;
            //_rigidbody2D.velocity = new Vector2(
            //    heading.x * Speed * Time.deltaTime, heading.y * Speed * Time.deltaTime);

            #region OTHER WAY OF MOVEMENT
            //var xPos = Input.GetAxis("Horizontal") * Speed * Time.deltaTime;
            //var yPos = Input.GetAxis("Vertical") * Speed * Time.deltaTime;
            //var inputModifyFactor = (Mathf.Abs(xPos) > 0.0f && Mathf.Abs(yPos) > 0.0f) ? .7071f : 1.0f;
            //var newPos = new Vector3(xPos * inputModifyFactor, yPos * inputModifyFactor, 0);
            //_rigidbody2D.AddForce(newPos);

            //var hAxis = Input.GetAxis("Horizontal");
            //var vAxis = Input.GetAxis("Vertical");
            //var force = new Vector2(hAxis, vAxis);
            //var totalForce = force.normalized * Time.deltaTime * Speed;
            //_rigidbody2D.AddForce(totalForce);

            //_rigidbody2D.velocity = new Vector2(
            //    Mathf.Lerp(0, Input.GetAxis("Horizontal") * Speed, 0.8f),
            //    Mathf.Lerp(0, Input.GetAxis("Vertical") * Speed, 0.8f));

            //var direction = new Vector2(0, 0);
            //direction.x = Input.GetAxis("Horizontal");
            //direction.y = Input.GetAxis("Vertical");
            //direction.Normalize();
            //_rigidbody2D.AddForce(Vector3.ClampMagnitude(direction, 1) * Speed);
            ////_rigidbody2D.velocity = Vector3.ClampMagnitude(direction, 1) * Speed;
            #endregion
        }
    

        private void ToggleInventory()
        {
            if (!Input.GetKeyDown(KeyCode.E)) return;
            _inventoryVisible = !_inventoryVisible;
            InventoryUI.SetActive(_inventoryVisible);
            Log.Info("PlayerController", "ToggleInventory", string.Format("_inventoryVisible = {0}", _inventoryVisible));
        }

        private void ToggleCrafting()
        {
            if (!Input.GetKeyDown(KeyCode.C)) return;
            _craftingVisible = !_craftingVisible;
            CraftingUI.SetActive(_craftingVisible);
            Log.Info("PlayerController", "ToggleCrafting", string.Format("_craftingVisible = {0}", _craftingVisible));
        }
    }
}
