using Assets.Scripts.Db;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class GameController : MonoBehaviour
    {
        public GameObject PlayerInventory;
        public GameObject PlayerCrafting;

        void OnEnable()
        {
            ItemsDatabase.Init();
            BlueprintsDatabase.Init();
            //var ep = GameObjectFactory.EscapePod();
            //BuildController.EscapePod = ep;
            //var player = GameObjectFactory.Player(0,0);
            //var player = GameObject.Find("Player"); //If already placed in the scene for debug/test
            //player.GetComponent<PlayerController>().InventoryUI = PlayerInventory;
            //player.GetComponent<PlayerController>().CraftingUI = PlayerCrafting;
            //Camera.main.GetComponent<CameraController>().FollowThis = player;
            
            //======================TODO just for debug 
            var playerInventoryController = GameObject.Find("PlayerInventoryController")
                .GetComponent<InventoryController>();
            playerInventoryController.AddItem(ItemId.Assembler, 1, 1);
            playerInventoryController.AddItem(ItemId.Printer3D, 1, 1);
            playerInventoryController.AddItem(ItemId.Foundation, 5, 0);
            playerInventoryController.AddItem(ItemId.Floor, 5, 0);
            playerInventoryController.AddItem(ItemId.Wall, 5, 0);
            playerInventoryController.AddItem(ItemId.Constructor, 1, 0);
        }

        //Before any Start - init any objects, refs etc
        void Awake()
        {
        }

        void Start ()
        {
            
        }
        
        void Update ()
        {
	
        }
    }
}
