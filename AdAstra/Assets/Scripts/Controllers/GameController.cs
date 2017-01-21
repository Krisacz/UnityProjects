using Assets.Scripts.Db;
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
            GameObjectFactory.EscapePod();
            var player = GameObjectFactory.Player(2,2);
            player.GetComponent<PlayerController>().InventoryUI = PlayerInventory;
            player.GetComponent<PlayerController>().CraftingUI = PlayerCrafting;
            Camera.main.GetComponent<CameraController>().FollowThis = player;
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
