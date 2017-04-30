using System;
using System.Collections.Generic;
using Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{
    public class NotificationFeedController : MonoBehaviour
    {
        private const int MaxVisible = 1;
        private const float SpeedIn = 4f;
        private const float SpeedOut = 6f;
        private const float ShowDuration = 0.5f;

        private static readonly List<Notification> Notifications = new List<Notification>();
        private static readonly List<GameObject> ToDestroy = new List<GameObject>();
        private static int _visible = 0;
        private static Transform _this;

        public void Start()
        {
            _this = this.transform;
        }

        void Update ()
        {
            if(Notifications.Count == 0)  return;

            foreach (var notification in Notifications)
            {
                switch (notification.State)
                {
                    case Notification.Mode.Wait:
                        Wait(notification);
                        break;

                    case Notification.Mode.Appear:
                        Appear(notification);
                        break;

                    case Notification.Mode.Show:
                        Show(notification);
                        break;

                    case Notification.Mode.Disappear:
                        Disappear(notification);
                        break;
                }
            }
        
            //Destroy
            Notifications.RemoveAll(x => x.State == Notification.Mode.Destroy);
            foreach (var gameObj in ToDestroy) if(gameObj != null) Destroy(gameObj);
            ToDestroy.RemoveAll(x => x == null);
        }

        private static void Wait(Notification notification)
        {
            if(_visible == MaxVisible) return;

            _visible++;
            notification.State = Notification.Mode.Appear;
        }

        private static void Appear(Notification notification)
        {
            var step = SpeedIn * Time.deltaTime;

            var startPosition = notification.GameObj.GetComponent<RectTransform>().localPosition;
            var endPosition = new Vector3(0f, 0f, 0f);
            var newPosition = Vector3.Lerp(startPosition, endPosition, step);

            var startAlpha = notification.GameObj.GetComponentInChildren<Text>().color.a;
            var newAlpha = Mathf.Lerp(startAlpha, 1f, step);

            if (newPosition.y >= -1f)
            {
                notification.State = Notification.Mode.Show;
            }

            notification.GameObj.GetComponent<RectTransform>().localPosition = newPosition;
            var c = notification.GameObj.GetComponentInChildren<Text>().color;
            notification.GameObj.GetComponentInChildren<Text>().color = new Color(c.r, c.g, c.b, newAlpha);
            if(notification.HasSprite) notification.GameObj.GetComponentInChildren<Image>().color = new Color(1f, 1f, 1f, newAlpha);
        }

        private static void Show(Notification notification)
        {
            notification.ShowDuration -= Time.deltaTime;
            if (notification.ShowDuration <= 0f)
            {
                notification.State = Notification.Mode.Disappear;
                _visible--;
            }
        }

        private static void Disappear(Notification notification)
        {
            var step = SpeedOut * Time.deltaTime;

            var startPosition = notification.GameObj.GetComponent<RectTransform>().localPosition;
            var endPosition = new Vector3(0f, 30f, 0f);
            var newPosition = Vector3.Lerp(startPosition, endPosition, step);

            var startAlpha = notification.GameObj.GetComponentInChildren<Text>().color.a;
            var newAlpha = Mathf.Lerp(startAlpha, 0f, step);

            if (newPosition.y >= 29f)
            {
                notification.State = Notification.Mode.Destroy;
                ToDestroy.Add(notification.GameObj);
            }

            notification.GameObj.GetComponent<RectTransform>().localPosition = newPosition;
            var c = notification.GameObj.GetComponentInChildren<Text>().color;
            notification.GameObj.GetComponentInChildren<Text>().color = new Color(c.r, c.g, c.b, newAlpha);
            if (notification.HasSprite) notification.GameObj.GetComponentInChildren<Image>().color = new Color(1f, 1f, 1f, newAlpha);
        }

        public static void Add(string spriteName, string message)
        {
            Notifications.Add(new Notification()
                              {
                                  State = Notification.Mode.Wait,
                                  GameObj = GameObjectFactory.Noticifaction(spriteName, message, _this),
                                  ShowDuration = ShowDuration,
                                  HasSprite = !string.IsNullOrEmpty(spriteName)
                              });
        }

        public static void Add(Icons icon, string message)
        {
            switch (icon)
            {
                case Icons.None:
                    Add(string.Empty, message);
                    break;

                case Icons.Tick:
                    Add("tick_icon", message);
                    break;

                case Icons.Warning:
                    Add("warning_icon", message);
                    break;

                case Icons.Error:
                    Add("error_icon", message);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("icon", icon, null);
            }
        }
    }
}
