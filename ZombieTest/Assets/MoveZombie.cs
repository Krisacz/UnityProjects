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
	    //LookAtPlayer();
	}

    void FixedUpdate()
    {
        //LookAtPlayer();
        var z = Mathf.Atan2((Target.position.y - transform.position.y), (Target.position.x - transform.position.x)) * Mathf.Rad2Deg - 90.0f;
        transform.eulerAngles = new Vector3(0, 0, z);
        GetComponent<Rigidbody2D>().AddForce(transform.up * Speed);
    }

    private void LookAtPlayer()
    {
        var targetPosition = new Vector2(Target.position.x, Target.position.y);
        var direction = (targetPosition - (Vector2)transform.position).normalized;
        transform.right = direction;
    }
}
