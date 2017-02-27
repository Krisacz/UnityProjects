using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Controllers
{
    public class AsteroidController: MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private float _maxDelay = 3f;
        private float _maxDistance = 65f;
        private float _delay = 3f;
        
        private bool _isScanning;
        private float _scanProgress;
        
        private PlayerController _player;
        private static GameObject _oreScanEffect;

        private void Start()
        {
            _player = GameObject.Find("Player").GetComponent<PlayerController>();
        }
        
        private void Update()
        {
            var deltaTime = Time.deltaTime;

            Scanning(deltaTime);
            AutoDestruct(deltaTime);
        }

        private void Scanning(float deltaTime)
        {
            if(!_isScanning) return;
            
            //Re-Check if scan is still valid
            if (!CanScan())
            {
                DisableOreScan();
                return;
            }

            //Item
            var item = ItemSelectionController.GetItem();
            var func = item.FunctionProperties;

            //Update
            _scanProgress += deltaTime;
            var scanSpeed = func.AsFloat(FunctionProperty.ScanSpeed);
            var progressPerc = _scanProgress/scanSpeed;

            //Scanning
            var range = func.AsEnum<ScanRange>(FunctionProperty.Range);
            var rangeF = SizeFromRange(range);
            var scale = rangeF*progressPerc;
            _oreScanEffect.transform.localScale = new Vector3(scale, scale, 1f);
            
            if (_scanProgress >= rangeF)
            {
                var scanSuccess = func.AsFloat(FunctionProperty.ScanSuccess);
                var scanLevel = func.AsInt(FunctionProperty.ScanLevel);
                if (ScanSuccesful(scanSuccess)) RevealNodes(scanLevel, rangeF);
                DisableOreScan();
            }
        }

        private static bool ScanSuccesful(float scanSuccess)
        {
            var random = Random.Range(0.0f, 1.0f);
            return (random <= scanSuccess);
        }

        private void RevealNodes(int scanLevel, float rangeF)
        {
            //Check if ore scan effect overlaps ore node
            var scanPos = _oreScanEffect.transform.position;
            var overlaps = Physics2D.OverlapCircleAll(scanPos, rangeF);
            foreach (var node in overlaps.Where(x=>x.name.Equals("OreNode")))
            {
                node.gameObject.GetComponent<NodeController>().Reveal(scanLevel);
            }
        }

        #region SIZE FROM RANGE
        private static float SizeFromRange(ScanRange range)
        {
            switch (range)
            {
                case ScanRange.VeryShort:   return 0.50f;
                case ScanRange.Short:       return 0.75f;
                case ScanRange.Medium:      return 1.00f;
                case ScanRange.Far:         return 1.25f;
                case ScanRange.VeryFar:     return 1.50f;
                default:                    throw new ArgumentOutOfRangeException("range", range, null);
            }
        }
        #endregion

        //TODO Attach script only on asteroids with ore nodes and separate auto-destruct script
        //TODO All asteroid will have simple auto destruct but only those with nodes will have this
        #region AUTO DESTRUCT

        private void AutoDestruct(float deltaTime)
        {
            if (_delay > 0f)
            {
                _delay -= deltaTime;
                return;
            }

            if (Vector2.Distance(Vector2.zero, transform.position) > _maxDistance)
            {
                Destroy(this.transform.gameObject);
            }
            else
            {
                _delay = _maxDelay;
            }
        }

        #endregion

        #region ON POINTER DOWN / UP

        public void OnPointerDown(PointerEventData eventData)
        {
            if (CanScan() == false) return;
            EnableOreScan();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            DisableOreScan();
        }

        #endregion

        #region ENABLE / DISABLE ORE SCAN

        private void EnableOreScan()
        {
            var position = MathHelper.GetMouseToWorldPosition();
            _isScanning = true;
            _scanProgress = 0.0f;
            if (_oreScanEffect == null) _oreScanEffect = GameObjectFactory.OreScanEffect();
            _oreScanEffect.SetActive(true);
            _oreScanEffect.transform.position = position;
            _oreScanEffect.transform.SetParent(this.transform);
        }

        private void DisableOreScan()
        {
            if (!_isScanning) return;
            _isScanning = false;
            _scanProgress = 0.0f;
            _oreScanEffect.SetActive(false);
            _oreScanEffect.transform.SetParent(null);
        }

        #endregion

        #region CAN SCAN

        private bool CanScan()
        {
            //Is player attached?
            if (!_player.GravityBootsActive())
            {
                NotificationFeedController.Add(Icons.Error, "You need to be attached.");
                return false;
            }

            //Is scanner selected?
            var item = ItemSelectionController.GetItem();
            if (item == null || item.ItemFunction != ItemFunction.Tool || item.ItemId != ItemId.OreScanner)
            {
                NotificationFeedController.Add(Icons.Error, "Ore Scanner not equipped.");
                return false;
            }

            //Is pointer cursor inside asteroid bounds?
            var position = MathHelper.GetMouseToWorldPosition();
            var polygon = this.GetComponent<PolygonCollider2D>();
            if (!MathHelper.PointInsidePolygon(position, polygon))
            {
                NotificationFeedController.Add(Icons.Error, "Scan initiated outside asteroid.");
                return false;
            }

            return true;
        }

        #endregion
    }
}
