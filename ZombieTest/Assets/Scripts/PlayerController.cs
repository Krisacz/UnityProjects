using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float WalkingSpeed;

    public float FireRate = 0.0f;
    public float WeaponDamage = 10.0f;
    public LayerMask HitLayerMask;
    private float _timeToFire = 0.0f;
    public Transform BulletSpawner;
    public GameObject BulletPrefab;

    public float BulletSpread = 0.25f;

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        var moveHorizontal = Input.GetAxis("Horizontal");
        var moveVertical = Input.GetAxis("Vertical");
        var movement = new Vector2(moveHorizontal, moveVertical) * WalkingSpeed;
        transform.position = new Vector2(transform.position.x + movement.x, transform.position.y + movement.y);
    }

    private void Update()
    {
        LookAtMouseCursor();
        CheckForFire();
    }

    private void LookAtMouseCursor()
    {
        var mouseScreenPosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var direction = (mouseScreenPosition - (Vector2)transform.position).normalized;
        transform.right = direction;
    }

    private void CheckForFire()
    {
        //Single shoot/burst
        if (FireRate <= 0.0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        //Burst/auto
        else
        {
            if (Input.GetButton("Fire1") && Time.time > _timeToFire)
            {
                _timeToFire = Time.time + (1 / FireRate);
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        var mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        var firePointPosition = new Vector2(BulletSpawner.position.x, BulletSpawner.position.y);
        var firePointTillMouseDistance = mousePosition - firePointPosition;
        SpawnBullet2();
        /*
        var hit = Physics2D.Raycast(firePointPosition, firePointTillMouseDistance, HitLayerMask);
        Debug.DrawLine(firePointPosition, firePointTillMouseDistance, Color.green);

        if (hit.collider != null)
        {
           Debug.DrawLine(firePointPosition, hit.point, Color.red);
        }
        */
    }

    private void SpawnBullet2()
    {
        Instantiate(BulletPrefab, BulletSpawner.position, BulletSpawner.rotation);
    }

    private void SpawnBullet()
    {
        var rnd = Random.insideUnitCircle * BulletSpread;
        var bs = BulletSpawner.transform.rotation;
        var rotation = new Quaternion(bs.x + rnd.x, bs.y + rnd.y, bs.z, bs.w);
        Instantiate(BulletPrefab, BulletSpawner.transform.position, rotation);
    }
}
