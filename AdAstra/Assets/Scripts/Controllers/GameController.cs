using Assets.Scripts.Db;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class GameController : MonoBehaviour
    {
        void OnEnable()
        {
            //Initialize data
            ItemsDatabase.Init();
            BlueprintsDatabase.Init();
            NodeGenerator.Init();
            
            //Create UI Elements
            var inventory = GameObjectFactory.InventoryUI(4, false);
            var inventoryBar = GameObjectFactory.InventoryBarUI(4, true);
            var playerStats = GameObjectFactory.StatsUI(false);
            var notificationFeedUI = GameObjectFactory.NotificationFeedUI();
            var toolTip = GameObjectFactory.TooltipUI(true);
            var workProgress = GameObjectFactory.WorkProgressUI(true);

            //Link player inventories to InventoryController
            var inventoryController = this.transform.GetComponent<InventoryController>();
            inventoryController.Inventories = new[] { inventoryBar, inventory };

            //Create Escape Pod
            var ep = GameObjectFactory.EscapePod();

            //Link Escape Pod & Inventory Controller to Build Controller
            BuildController.EscapePod = ep;
            BuildController.InventoryController = inventoryController;

            //Create Player
            var player = GameObjectFactory.Player(0, 0);
            var playerController = player.GetComponent<PlayerController>();
            playerController.InventoryUI = inventory;
            playerController.InventoryBarUI = inventoryBar;
            playerController.Stats = playerStats;

            //TODO link anything else UI-related to player controller (if needs to be controlled by player)

            //Set main Camera to follow Player
            Camera.main.GetComponent<CameraController>().FollowThis = player;

            //TODO In Start?
            //GameObject.Find("AsteroidsController").GetComponent<AsteroidsController>().SpawnAsteroids();



            //TODO ===================================
            //TODO ========= DEBUG ADD ITEMS ========= 
            //TODO ===================================
            inventoryController.AddItem(ItemId.Printer3D, 1, 0);
            inventoryController.AddItem(ItemId.Assembler, 1, 0);
            inventoryController.AddItem(ItemId.IronIngot, 10, 0);
            inventoryController.AddItem(ItemId.AluminiumIngot, 10, 0);
        }

        void Awake() { }
        void Start() { }
        void Update () { }
    }
}
