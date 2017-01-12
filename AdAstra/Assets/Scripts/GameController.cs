using UnityEngine;

namespace Assets.Scripts
{
    public class GameController : MonoBehaviour
    {
        public InventoryController InventoryController;

        void OnEnable()
        {
            ItemsDatabase.Init();

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
