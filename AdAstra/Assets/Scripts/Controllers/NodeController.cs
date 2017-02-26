using System;
using Assets.Scripts.Models;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Controllers
{
    public class NodeController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private NodeData _nodeData;
        public bool IsVisible { get; private set; }
        public bool IsFullyScanned { get; private set; }

        private PlayerController _player;
        private bool _isMining;
        private float _miningProgress;

        public void Init()
        {
            _nodeData = NodeGenerator.New();
        }

        private void Start()
        {
            _player = GameObject.Find("Player").GetComponent<PlayerController>();
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            Mining(deltaTime);
        }

        public void Reveal(int scanLevel)
        {
            IsVisible = true;
            IsFullyScanned = scanLevel >= _nodeData.ScanLevelRequired;

            //TODO Appear node - for now just change color
            this.GetComponent<SpriteRenderer>().color = Color.green;
        }

        private void Mining(float deltaTime)
        {
            if (!_isMining) return;

            //Re-Check if mining is still valid
            if (!CanMine())
            {
                DisableMiningDrill();
                return;
            }

            //Item
            var item = ItemSelectionController.GetItem();
            var func = item.FunctionProperties;

            //Update
            _miningProgress += deltaTime;
            var drillSpeed = func.AsFloat(FunctionProperty.DrillSpeed);

            //Mining completed
            if (_miningProgress >= drillSpeed)
            {
                var reliability = func.AsFloat(FunctionProperty.Reliability);
                if (MiningSuccess(reliability))
                {
                    var burstExtractionChance = func.AsFloat(FunctionProperty.BurstExtractionChance);
                    var burstExtraction = BurstExtraction(burstExtractionChance);

                    var extractionYieldMin = func.AsInt(FunctionProperty.ExtractionYieldMin);
                    var extractionYieldMax = func.AsInt(FunctionProperty.ExtractionYieldMax);
                    var yield = Random.Range(extractionYieldMin, extractionYieldMax + 1);

                    GetOre(yield, burstExtraction);
                }
                DisableMiningDrill();
            }
        }

        private static bool MiningSuccess(float reliability)
        {
            var random = Random.Range(0.0f, 1.0f);
            return (random <= reliability);
        }

        private static bool BurstExtraction(float burstExtractionChance)
        {
            var random = Random.Range(0.0f, 1.0f);
            return (random <= burstExtractionChance);
        }

        private void GetOre(int yield, bool burstExtraction)
        {
            Log.Info(string.Format("Yield: {0}, BurstExtraction: {1}", yield, burstExtraction));
        }

        #region ON POINTER DOWN / UP
        public void OnPointerDown(PointerEventData eventData)
        {
            if (CanMine() == false) return;
            EnableMiningDrill();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            DisableMiningDrill();
        }
        #endregion

        #region ENABLE / DISABLE ORE SCAN
        private void EnableMiningDrill()
        {
            _isMining = true;
            _miningProgress = 0.0f;
        }

        private void DisableMiningDrill()
        {
            if (!_isMining) return;
            _isMining = false;
            _miningProgress = 0.0f;
        }
        #endregion

        #region CAN MINE
        private bool CanMine()
        {
            //Check if node is visible
            if (!IsVisible) return false;

            //Is player attached?
            if (!_player.GravityBootsActive())
            {
                NotificationFeedController.Add(Icons.Error, "You need to be attached.");
                return false;
            }

            //Is mining drill selected?
            var item = ItemSelectionController.GetItem();
            if (item == null || item.Function != Function.Tool || item.ItemId != ItemId.MinningDrill)
            {
                NotificationFeedController.Add(Icons.Error, "Minning Drill not equipped.");
                return false;
            }

            //Is Mining drill head level sufficient?
            var func = item.FunctionProperties;
            var drillHeadLevel = func.AsInt(FunctionProperty.DrillHeadLevel);
            if (drillHeadLevel < _nodeData.ScanLevelRequired)
            {
                NotificationFeedController.Add(Icons.Error, "Minning Drill Head level not sufficient.");
                return false;
            }

            //Is pointer cursor inside ore node bounds?
            var position = MathHelper.GetMouseToWorldPosition();
            var polygon = this.GetComponent<CircleCollider2D>();
            if (!MathHelper.PointInsideCircle(position, polygon))
            {
                NotificationFeedController.Add(Icons.Error, "Mining initiated outside ore node.");
                return false;
            }

            return true;
        }
        #endregion
    }
}
