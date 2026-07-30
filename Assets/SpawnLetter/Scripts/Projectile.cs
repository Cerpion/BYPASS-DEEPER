using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;

    private Transform target;
    private WordNode targetWord;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        if (target != null)
        {
            targetWord = target.GetComponent<WordNode>();
        }

        Debug.Log("Projectile apuntando a: " + (target != null ? target.name : "NULL"));
    }

    private void Update()
    {
        // La palabra desapareció
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        // Llegó al objetivo
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        if (targetWord != null)
        {
            // Ejecuta el comportamiento normal de la palabra
            Destroy(targetWord.gameObject);
        }

        Destroy(gameObject);
    }
}