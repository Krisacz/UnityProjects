using UnityEngine;

namespace Assets.Scripts
{
    public class GameController : MonoBehaviour
    {
        public InventoryController InventoryController;

        //Before any Start - init any objects, refs etc
        void Awake()
        {
            ItemsDatabase.Init();
        }

        void Start ()
        {
            
        }
        
        void Update ()
        {
	
        }
    }
}
