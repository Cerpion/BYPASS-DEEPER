using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 12f;

    private Transform target;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        Debug.Log("Projectile apuntando a: " + target.name);
    }

    void Update()
    {
        // Si la palabra fue destruida, destruir el proyectil
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Movimiento hacia la palabra
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );
    }
}