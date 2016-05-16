using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float WalkingSpeed;

    public Rigidbody2D BulletPrefab;
    public GameObject BulletSpawner;
    public float FireSpeed = 0.5f;
    public float FireCoolDown;
    public float BulletSpeed = 500;


    private void Start ()
    {
	    
	}
    
    private void Update ()
	{
	    Movement();
	    LookAt();
        Fire();
    }

    private void Fire()
    {
        if (!(Time.time >= FireCoolDown)) return;
        if (Input.GetMouseButton(0)) SpawnBullet();
    }

    private void SpawnBullet()
    {
        var bPrefab = Instantiate(BulletPrefab, new Vector3(BulletSpawner.transform.position.x, BulletSpawner.transform.position.y, BulletSpawner.transform.position.z), Quaternion.identity) as Rigidbody2D;
        if (bPrefab != null) bPrefab.GetComponent<Rigidbody2D>().AddForce(BulletSpawner.transform.right * BulletSpeed);
        FireCoolDown = Time.time + FireSpeed;
    }

    private void LookAt()
    {
        Vector2 mouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var direction = (mouseScreenPosition - (Vector2)transform.position).normalized;
        transform.right = direction;
    }

    private void Movement()
    {
        var moveHorizontal = Input.GetAxis("Horizontal");
        var moveVertical = Input.GetAxis("Vertical");
        var movement = new Vector2(moveHorizontal, moveVertical)*WalkingSpeed;
        transform.position = new Vector2(transform.position.x + movement.x, transform.position.y + movement.y);
    }
}
