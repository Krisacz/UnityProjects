using System.Linq;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        public float Speed;
        public GameObject InventoryUI;
        public GameObject CraftingUI;

        private Rigidbody2D _rigidbody2D;
        private FixedJoint2D _joint;
        private bool _inventoryVisible;
        private bool _craftingVisible;
        private static CircleCollider2D _playerCollider;
        private static LineController _lineController;

        void Start ()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _playerCollider = GetComponent<CircleCollider2D>();
            _lineController = GetComponentInChildren<LineController>();

            _joint = this.GetComponent<FixedJoint2D>();
            _joint.enabled = false;
        }

        void FixedUpdate()
        {
            Movement();
            LookAtMouse();
        }

        void Update()
        {
            ToggleInventory();
            SelectInventorySlot();
            GravityBoots();
        }
        
        private void Movement()
        {
            if(_gravityBootsActive) return;
            _rigidbody2D.velocity = new Vector2(
                Mathf.Lerp(0, Input.GetAxis("Horizontal") * Speed, 0.8f),
                Mathf.Lerp(0, Input.GetAxis("Vertical") * Speed, 0.8f));
        }

        private void LookAtMouse()
        {
            //if (_gravityBootsActive) return;
            var objectPos = Camera.main.WorldToScreenPoint(transform.position);
            var dir = Input.mousePosition - objectPos;
            var angle = (Mathf.Atan2(dir.y, dir.x)*Mathf.Rad2Deg) - 90f;
            transform.FindChild("Circle").transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            //transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        private void ToggleInventory()
        {
            if (!Input.GetKeyDown(KeyCode.E)) return;
            _inventoryVisible = !_inventoryVisible;
            InventoryUI.SetActive(_inventoryVisible);
            //Log.Info("PlayerController", "ToggleInventory", string.Format("_inventoryVisible = {0}", _inventoryVisible));
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

        private bool _gravityBootsActive = false;
        private void GravityBoots()
        {
            if (!_gravityBootsActive && Input.GetKeyDown(KeyCode.F))
            {
                //Check if player is toching asteroid (and it's center is "inside" asteroid bounds)
                var touching = Physics2D.RaycastAll(this.transform.position, Vector2.zero);
                var isTouching = touching.Any(x => x.transform.gameObject.layer == (int)Layer.Floats);
                if (!isTouching)
                {
                    NotificationFeedController.Add(Icons.Error, "You are not over Asteroid.");
                    return;
                }

                //Check if player is not "covered" by escape pod parts
                var notAllowed = touching.Any(x => 
                    x.transform.gameObject.layer == (int)Layer.ModuleBlocking
                    || x.transform.gameObject.layer == (int)Layer.ModuleNonBlocking);
                if (notAllowed)
                {
                    NotificationFeedController.Add(Icons.Error, "You are not over Asteroid (directly).");
                    return;
                }

                //If all ok - attach!
                var asteroid = touching.First(x => 
                    x.transform.gameObject.layer == (int)Layer.Floats).transform;
                _joint.connectedBody = asteroid.GetComponent<Rigidbody2D>();
                _gravityBootsActive = true;
                _joint.enabled = true;
            }
            else if (_gravityBootsActive && Input.GetKeyDown(KeyCode.F))
            {
                _joint.connectedBody = null;
                _gravityBootsActive = false;
                _joint.enabled = false;
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            //TODO Why this doesnt work?
            //TODO when player is hitting escape pod he should de-attach from asteroid
            if (!_gravityBootsActive) return;
            _joint.connectedBody = null;
            _gravityBootsActive = false;
            _joint.enabled = false;
        }

        public Bounds GetBounds()
        {
            return _playerCollider.bounds;
        }

        public LineController GetLineController()
        {
            return _lineController;
        }

        public bool GravityBootsActive()
        {
            return _gravityBootsActive;
        }

        public Transform GetTransform()
        {
            return this.transform;
        }
    }
}
