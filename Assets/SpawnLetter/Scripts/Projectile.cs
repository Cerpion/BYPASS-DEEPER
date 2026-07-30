using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 15f;

    private Transform target;


    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        Debug.Log("Projectile apuntando a: " + target.name);
    }


    private void Update()
    {
        if (target == null)
        {
            return;
        }


        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );


        float distance = Vector3.Distance(
            transform.position,
            target.position
        );


        if (distance < 0.2f)
        {
            Debug.Log("Impacto en palabra");


            Destroy(target.gameObject);


            Destroy(gameObject);
        }
    }
}