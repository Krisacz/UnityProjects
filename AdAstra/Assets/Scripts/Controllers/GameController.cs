using Assets.Scripts.Db;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class GameController : MonoBehaviour
    {
        #region FIELDS & PROPERITES
        private static GameController _gameController;
        private static GameController Gc
        {
            get
            {
                return _gameController ?? (_gameController = GameObject.FindGameObjectWithTag("GameControllerTag").GetComponent<GameController>());
            }
        }

        private static InventoryController _inventoryController;
        public static InventoryController InventoryController
        {
            get
            {
                return _inventoryController ?? (_inventoryController = Gc.GetComponent<InventoryController>());
            }
        }

        private static BuildController _buildController;
        public static BuildController BuildController
        {
            get
            {
                return _buildController ?? (_buildController = Gc.GetComponent<BuildController>());
            }
        }

        private static SequenceController _sequenceController;
        public static SequenceController SequenceController
        {
            get
            {
                return _sequenceController ?? (_sequenceController = Gc.GetComponent<SequenceController>());
            }
        }
        #endregion

        void OnEnable()
        {
            //Initialize data
            ItemsDatabase.Init();
            BlueprintsDatabase.Init();
            NodeGenerator.Init();
            
            //Create UI Elements
            var inventory = GameObjectFactory.InventoryUI(4, false);
            var inventoryBar = GameObjectFactory.InventoryBarUI(10, true);
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
            inventoryController.AddItem(ItemId.EnergizedSmelter, 1, 0);
            inventoryController.AddItem(ItemId.Assembler, 1, 0);

            inventoryController.AddItem(ItemId.DirtyIce, 20, 0);
            inventoryController.AddItem(ItemId.SiliconOre, 20, 0);
            inventoryController.AddItem(ItemId.IronOre, 20, 0);
            inventoryController.AddItem(ItemId.CopperOre, 20, 0);
        }

        void Awake() { }
        void Start() { }
        void Update () { }





    }
}
