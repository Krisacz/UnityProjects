using UnityEngine;
using System.Collections;

public class CameraMove: MonoBehaviour 
{
	public float speed = 1.0f;
	
	void Update () 
	{
		Vector3 position = transform.position;
			    position.x += speed;
		transform.position = position;
	}
}
