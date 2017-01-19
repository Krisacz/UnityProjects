using Assets.Scripts.Db;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class GameController : MonoBehaviour
    {
        void OnEnable()
        {
            ItemsDatabase.Init();
            BlueprintsDatabase.Init();
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
