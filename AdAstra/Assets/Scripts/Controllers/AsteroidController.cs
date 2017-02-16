using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.EventSystems;

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
        private GameObject _oreScanEffect;

        private void Start()
        {
            _player = GameObject.Find("Player").GetComponent<PlayerController>();
            _oreScanEffect = GameObject.Find("OreScanEffect");
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

            var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //Re-Check if scan is still valid
            if (!CanScan(position))
            {
                DisableOreScan();
                return;
            }

            //Item
            var item = ItemSelectionController.GetItem();
            var func = item.FunctionProperties;

            //Update
            _scanProgress -= deltaTime;
            var scanSpeed = func.AsFloat(FunctionProperty.ScanSpeed);
            var progressPerc = _scanProgress/scanSpeed;

            //Scanning
            var range = func.AsEnum<ScanRange>(FunctionProperty.Range);
            var rangeF = SizeFromRange(range);
            var scale = rangeF*progressPerc;
            _oreScanEffect.transform.localScale = new Vector3(scale, scale, 1f);

            var scanSuccess = func.AsFloat(FunctionProperty.ScanSuccess);
            var scanLevel = func.AsFloat(FunctionProperty.ScanLevel);


            if (_scanProgress <= 0f)
            {
                DisableOreScan();
            }
        }

        #region SIZE FROM RANGE
        private static float SizeFromRange(ScanRange range)
        {
            switch (range)
            {
                case ScanRange.VeryShort:   return 0.50f;
                case ScanRange.Short:       return 1.00f;
                case ScanRange.Medium:      return 2.00f;
                case ScanRange.Far:         return 3.00f;
                case ScanRange.VeryFar:     return 4.00f;
                default:                    throw new ArgumentOutOfRangeException("range", range, null);
            }
        }
        #endregion

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
            var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (CanScan(position) == false) return;
            EnableOreScan(position);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            DisableOreScan();
        }

        #endregion

        #region ENABLE / DISABLE ORE SCAN

        private void EnableOreScan(Vector3 position)
        {
            _isScanning = true;
            var item = ItemSelectionController.GetItem();
            var func = item.FunctionProperties;
            _scanProgress = func.AsFloat(FunctionProperty.ScanSpeed);
            _oreScanEffect.SetActive(true);
            _oreScanEffect.transform.localPosition = position;
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

        private bool CanScan(Vector3 position)
        {
            //Is player attached?
            if (!_player.GravityBootsActive())
            {
                NotificationFeedController.Add(Icons.Error, "You need to be attached.");
                return false;
            }

            //Is scanner selected?
            var item = ItemSelectionController.GetItem();
            if (item == null || item.Function != Function.Tool || item.ItemId != ItemId.OreScanner)
            {
                NotificationFeedController.Add(Icons.Error, "Ore Scanner not equipped.");
                return false;
            }

            //Is pointer cursor inside asteroid bounds?
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
