using UnityEngine;
using System.Collections;

public class MoveBullet : MonoBehaviour
{
    public int MoveSpeed = 230;

	void Update ()
    {
	    transform.Translate(Vector3.right * Time.deltaTime * MoveSpeed);
        Destroy(gameObject, 1f);
	}
    
    void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
