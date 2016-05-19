using UnityEngine;
using System.Collections;

public class MoveZombie : MonoBehaviour
{
    public Transform Target;
    public float Speed;

    void Start ()
    {
	
	}
	
	void Update ()
    {
        LookAtPlayer();
	}

    void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().AddForce(transform.right * Speed * 10);

        //var z = Mathf.Atan2((Target.position.y - transform.position.y), (Target.position.x - transform.position.x)) * Mathf.Rad2Deg - 90.0f;
        //transform.eulerAngles = new Vector3(0, 0, z);
        //GetComponent<Rigidbody2D>().AddForce(transform.up * Speed);
    }

    private void LookAtPlayer()
    {
        var targetPosition = (Vector2)Target.position;
        var direction = (targetPosition - (Vector2)transform.position).normalized;
        transform.right = direction;
    }
}
