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
    }
    void OnDisable(){
        GameManager.Instance.OnVictoryWalkStart -= OnStartWalk;
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
