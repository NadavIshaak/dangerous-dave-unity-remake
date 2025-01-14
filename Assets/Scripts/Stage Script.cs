using UnityEngine;

public class StageScript : MonoSingleton<StageScript>
{
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        GameManager.Instance.OnVictoryWalkStart += OnStartWalk;
    }
     private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnVictoryWalkStart += OnStartWalk;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnVictoryWalkStart -= OnStartWalk;
        }
    }
    public void Disable()
    {
        GameManager.Instance.OnVictoryWalkStart -= OnStartWalk;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnVictoryWalkStart -= OnStartWalk;
        }
    }
     public void OnStartWalk()
    {
        animator.SetTrigger("StartWalk");
    }

    public void OnEndWalk()
    {
        animator.SetTrigger("EndWalk");
    }
}
