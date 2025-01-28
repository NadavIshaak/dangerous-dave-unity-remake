using UnityEngine;
/**
 * This class is responsible for handling the movement of the spinner enemy.
 */
public class EnemySpinnerMovement : MonoBehaviour
{
    public float moveDistance = 5f; // Distance to move left and right
    public float moveSpeed = 2f; // Speed of movement
    private bool _movingRight = true;

    private Vector3 _startPosition;

    private void Start()
    {
        _startPosition = transform.position;
    }

    private void Update()
    {
        Move();
    }

    /**
     * Move the enemy left and right
     */
    private void Move()
    {
        if (_movingRight)
        {
            transform.position += Vector3.right * (moveSpeed * Time.deltaTime);
            if (transform.position.x >= _startPosition.x + moveDistance) _movingRight = false;
        }
        else
        {
            transform.position += Vector3.left * (moveSpeed * Time.deltaTime);
            if (transform.position.x <= _startPosition.x - moveDistance) _movingRight = true;
        }
    }
}