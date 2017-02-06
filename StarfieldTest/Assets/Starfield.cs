using UnityEngine;
using System.Collections;

public class Starfield: MonoBehaviour 
{
	public Camera camera;
	public Material starsMaterial;
	public float backgroundDistance  = 10000;
	public float smallStarsDistance  = 5000;
	public float mediumStarsDistance = 2500;
	public float bigStarsDistance    = 1000;
	private Vector2 lastScreenSize = new Vector2();	
		
	void OnEnable() 
	{
		if (!camera || !starsMaterial)
		{
			Debug.Log ("Camera or material is not set");
			enabled = false;			
		}
	}
	
	void Update () 
	{
		if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y)
			updateSize();
	}
	
	void LateUpdate()
	{
		Vector3 pos = transform.position;
		pos.x = camera.transform.position.x;
		pos.y = camera.transform.position.y;
		transform.position = pos;
		
		starsMaterial.SetTextureOffset("_Background" , new Vector2(camera.transform.position.x / backgroundDistance, camera.transform.position.y / backgroundDistance));
		starsMaterial.SetTextureOffset("_SmallStars" , new Vector2(camera.transform.position.x / smallStarsDistance, camera.transform.position.y / smallStarsDistance));
		starsMaterial.SetTextureOffset("_MediumStars", new Vector2(camera.transform.position.x / mediumStarsDistance, camera.transform.position.y / mediumStarsDistance));
		starsMaterial.SetTextureOffset("_BigStars"   , new Vector2(camera.transform.position.x / bigStarsDistance, camera.transform.position.y / bigStarsDistance));
	}
	
	private void updateSize()
	{
		lastScreenSize.x = Screen.width; 
		lastScreenSize.y = Screen.height;
							 
		float maxSize = lastScreenSize.x > lastScreenSize.y ? lastScreenSize.x : lastScreenSize.y;	
		maxSize /= 10;
		transform.localScale = new Vector3(maxSize, 1, maxSize);			
	}
}
