using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Assets.Scripts;
using Assets.Scripts.Models;
using UnityEditor;

public class NotificationFeedController : MonoBehaviour
{
    private static readonly List<Notification> Notifications = new List<Notification>();
    private static readonly List<GameObject> ToDestroy = new List<GameObject>();
    private static int _visible = 0;
    private static int _maxVisible = 3;
    private static float _speed = 0.01f;
    private static float _showDuration = 2f;
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
        //Notifications.RemoveAll(x => x.State == Notification.Mode.Destroy);
        //foreach (var gameObj in ToDestroy) if(gameObj != null) Destroy(gameObj);
        //ToDestroy.RemoveAll(x => x == null);
    }

    private static void Wait(Notification notification)
    {
        if(_visible == _maxVisible) return;

        _visible++;
        notification.State = Notification.Mode.Appear;
        var position = notification.GameObj.transform.position;
        notification.GameObj.transform.position = new Vector3(0f, position.y, position.z);
    }

    private static void Appear(Notification notification)
    {
        var currentPosition = notification.GameObj.transform.position;
        var newPos = new Vector3(0f, currentPosition.y + _speed * Time.deltaTime, currentPosition.z);
        if (newPos.y >= -1f && newPos.y <= 1f)
        {
            newPos.y = 0f;
            notification.State = Notification.Mode.Show;
        }

        notification.GameObj.transform.position = newPos;
    }

    private static void Show(Notification notification)
    {
        notification.ShowDuration -= Time.deltaTime;
        if (notification.ShowDuration <= 0f)
        {
            notification.State = Notification.Mode.Disappear;
        }
    }

    private static void Disappear(Notification notification)
    {
        var currentPosition = notification.GameObj.transform.position;
        var newPos = new Vector3(currentPosition.x, currentPosition.y + _speed * Time.deltaTime, currentPosition.z);
        notification.GameObj.transform.position = newPos;
        if (newPos.y >= -50f)
        {
            notification.State = Notification.Mode.Destroy;
            ToDestroy.Add(notification.GameObj);
        }
    }

    public static void Add(string spriteName, string message)
    {
        Notifications.Add(new Notification()
        {
            State = Notification.Mode.Wait,
            GameObj = GameObjectFactory.Noticifaction(spriteName, message, _this),
            ShowDuration = _showDuration
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
