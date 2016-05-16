using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float WalkingSpeed;
    public float FireSpeed = 0.5f;
    public float FireCoolDown = 1;

    public float BulletSpread = 0.25f;
    public GameObject BulletPrefab;
    public GameObject BulletSpawner;
    

    private void Start ()
    {
	    
	}
    
    private void Update ()
	{
	    Movement();
	    LookAtMouseCursor();
        Fire();
    }

    private void Fire()
    {
        if (!(Time.time >= FireCoolDown)) return;
        if (Input.GetMouseButton(0)) SpawnBullet();
    }

    private void SpawnBullet()
    {
        var rnd = Random.insideUnitCircle * BulletSpread;
        var bs = BulletSpawner.transform.rotation;
        var rotation = new Quaternion(bs.x + rnd.x, bs.y + rnd.y, bs.z, bs.w);

        Instantiate(BulletPrefab, BulletSpawner.transform.position, rotation);
        FireCoolDown = Time.time + FireSpeed;
    }
    
    private void LookAtMouseCursor()
    {
        var mouseScreenPosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
