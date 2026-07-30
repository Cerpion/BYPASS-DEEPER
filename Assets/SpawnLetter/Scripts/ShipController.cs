using UnityEngine;

public class ShipController : MonoBehaviour
{
    public static ShipController Instance;

    [Header("Movimiento")]
    public float moveSpeed = 8f;

    [Tooltip("Distancia que la nave mantiene debajo de la palabra")]
    public float followOffset = 1.8f;

    [Header("Disparo")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    private Transform currentTarget;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (currentTarget == null)
            return;

        // Mantener la nave debajo de la palabra
        Vector3 destination = currentTarget.position + Vector3.down * followOffset;
        destination.z = transform.position.z;

        transform.position = Vector3.MoveTowards(
            transform.position,
            destination,
            moveSpeed * Time.deltaTime
        );
    }
     public void SetTarget(Transform target)
    {
        currentTarget = target;
    }

   
    public Transform GetTarget()
    {
        return currentTarget;
    }

    public void ClearTarget()
    {
        currentTarget = null;
    }

    
    public void Shoot()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("No hay Projectile Prefab asignado.");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogWarning("No hay FirePoint asignado.");
            return;
        }

        if (currentTarget == null)
        {
            Debug.LogWarning("No hay objetivo para disparar.");
            return;
        }

        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity);

        Projectile projectileScript = projectile.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            projectileScript.SetTarget(currentTarget);
        }
    }
}