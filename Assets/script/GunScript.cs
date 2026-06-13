using UnityEngine;

public class GunScript : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;

    [Header("Fire Settings")]
    public float fireRate = 0.2f;
    private float nextFireTime;

    [Header("Player")]
    public PlayerMovement player;

    public float NextFireTime
    {
        get { return nextFireTime; }
    }

    void Update()
    {
        if ((Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.Keypad1))
            && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject bullet =
            Instantiate(bulletPrefab,
                        firePoint.position,
                        Quaternion.identity);

        Vector2 shootDirection =
            player.FacingDirection > 0
            ? Vector2.right
            : Vector2.left;

        Bullet b = bullet.GetComponent<Bullet>();

        if (b != null)
        {
            b.SetVelocity(shootDirection, bulletSpeed);
        }
    }
}