using UnityEngine;

public class EnemySpinnerMovement : MonoBehaviour
{
    public float moveDistance = 5f; // Distance to move left and right
    public float moveSpeed = 2f; // Speed of movement
    private bool movingRight = true;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (movingRight)
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            if (transform.position.x >= startPosition.x + moveDistance) movingRight = false;
        }
        else
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            if (transform.position.x <= startPosition.x - moveDistance) movingRight = true;
        }
    }
}