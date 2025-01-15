using UnityEngine;

public class StageScript : MonoSingleton<StageScript>
{
    private Animator animator;
    [SerializeField] private Canvas _canvas;
    private InputSystem_Actions controls;
    private bool didStartGame=false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        GameManager.Instance.OnVictoryWalkStart += OnStartWalk;
        _canvas.enabled = false;
    }
    protected override void Awake()
    {
        base.Awake();
        controls = new InputSystem_Actions();
    }
     private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnVictoryWalkStart += OnStartWalk;
        }
        controls.Enable();
    }
    private void Update()
    {
        if(!didStartGame&&controls.Player.Jump.triggered)
        {
            didStartGame = true;
            StartGame();   
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnVictoryWalkStart -= OnStartWalk;
        }
        controls.Disable();
    }
    private void StartGame()
    {
        _canvas.enabled = true;
        GameManager.Instance.InstantiatePlayer();
        animator.SetTrigger("StartGame");
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
