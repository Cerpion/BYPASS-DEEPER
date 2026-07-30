using UnityEngine;

public class ShipController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Posición debajo de la palabra")]
    [SerializeField] private float offsetY = -1.5f;

    [Header("Disparo")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;

    private Transform target;

    private void Update()
    {
        if (target == null)
            return;

        Vector3 destination = target.position;
        destination.y += offsetY;

        transform.position = Vector3.MoveTowards(
            transform.position,
            destination,
            moveSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public Transform GetTarget()
    {
        return target;
    }

    public void Shoot()
{
    Debug.Log("Shoot() ejecutado");

    Debug.Log("Projectile Prefab: " + projectilePrefab);
    Debug.Log("Fire Point: " + firePoint);
    Debug.Log("Target: " + target);

    if (projectilePrefab == null || firePoint == null || target == null)
    {
        Debug.Log("No se puede disparar porque falta una referencia.");
        return;
    }

    GameObject projectile = Instantiate(
        projectilePrefab,
        firePoint.position,
        Quaternion.identity);

    Debug.Log("Proyectil creado: " + projectile.name);

    Projectile projectileScript = projectile.GetComponent<Projectile>();

    if (projectileScript != null)
    {
        projectileScript.SetTarget(target);
    }

}

    public void ClearTarget()
    {
        target = null;
    }
}