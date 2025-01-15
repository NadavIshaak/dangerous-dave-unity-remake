using UnityEngine;

public class DangerousObject : MonoBehaviour
{
    public virtual void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player"))
        {
            KillPlayer();
        }
    }

    private void KillPlayer()
    {
        LifeManager.Instance.RemoveLife();
        GameManager.Instance.TriggerPlayerDeath();
        Debug.Log("Player died");
        Debug.Log("killed by: " + gameObject.name);
    }
}
