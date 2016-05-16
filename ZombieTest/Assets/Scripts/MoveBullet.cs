using UnityEngine;
using System.Collections;

public class MoveBullet : MonoBehaviour
{
    public int MoveSpeed = 230;

	void Update ()
    {
	    transform.Translate(Vector3.right * Time.deltaTime * MoveSpeed);
	}
}
