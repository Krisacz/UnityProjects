using System.Collections.Generic;
using Assets.Scripts.Db;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class StructureSlotView : MonoBehaviour
    {
        private readonly Dictionary<StructureElevation, Item> _items = new Dictionary<StructureElevation, Item>();
        private readonly Dictionary<StructureElevation, GameObject> _gameObjects = new Dictionary<StructureElevation, GameObject>();
        
        //Returns TRUE is succesfully added, false if it already has a structure on this elevation
        public bool AddStructure(StructureElevation elevation, ItemId itemId)
        {
            //Item (structure) already exist
            if (_items.ContainsKey(elevation) && _items[elevation] != null) return false;

            var item = ItemsDatabase.Get(itemId);
            var go = GameObjectFactory.FromItemId(itemId);
            go.transform.position = this.transform.position;
            go.transform.parent = this.transform;
            go.GetComponent<SpriteRenderer>().sprite = SpritesDatabase.Get(item.SpriteName);

            if (_items.ContainsKey(elevation))
            {
                _items[elevation] = item;
                _gameObjects[elevation] = go;
            }
            else
            {
                _items.Add(elevation, item);
                _gameObjects.Add(elevation, go);
            }

            return true;
        }

        // Use this for initialization
        void Start ()
        {
	
        }
	
        // Update is called once per frame
        void Update ()
        {
	
        }
    }
}
