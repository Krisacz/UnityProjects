using UnityEngine;
using System.Collections;

public class CameraSettings : MonoBehaviour 
{
	public Camera camera;
	
	private float lastHeight = 0;	
	
	void OnEnable()
	{
		if (!camera)
		{
			Debug.Log ("Camera is not set");
			enabled = false;			
		}
	}
	
	void Update () 
	{
		if (lastHeight != Screen.height)
		{
			lastHeight = Screen.height;
			camera.orthographicSize = lastHeight / 2;
		}
	}
}
