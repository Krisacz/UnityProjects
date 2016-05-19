using UnityEngine;
using System.Collections;

public class MoveBullet : MonoBehaviour
{
    public int MoveSpeed = 230;

	void Update ()
    {
        transform.Translate(Vector3.right * Time.deltaTime * MoveSpeed);
        //Destroy(gameObject, 1f);
        //transform

        //transform.Translate(Vector3.right * MoveSpeed);
        //bPrefab.rigidbody2D.AddForce(transform.up * bulletSpeed);
    }

    void FixedUpdate()
    {
        //GetComponent<Rigidbody2D>().velocity = transform.forward * MoveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Destroy(gameObject);
    }
}
