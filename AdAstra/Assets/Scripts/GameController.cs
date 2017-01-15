using UnityEngine;

namespace Assets.Scripts
{
    public class GameController : MonoBehaviour
    {
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
