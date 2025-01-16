using UnityEngine;
using UnityEngine.Serialization;

public class StageScript : MonoBehaviour
{
    public static StageScript Instance;
    private static readonly int Game = Animator.StringToHash("StartGame");
    private static readonly int StartWalk = Animator.StringToHash("StartWalk");
    private static readonly int EndWalk = Animator.StringToHash("EndWalk");
    
    [FormerlySerializedAs("_canvas")] [SerializeField]
    private Canvas canvas;

    private Animator _animator;
    private InputSystem_Actions _controls;
    private bool _didStartGame;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
        _controls = new InputSystem_Actions();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _animator = GetComponent<Animator>();
        GameManager.Instance.OnVictoryWalkStart += OnStartWalk;
        canvas.enabled = false;
    }

    private void Update()
    {
        if (!_didStartGame && _controls.Player.Jump.triggered)
        {
            _didStartGame = true;
            StartGame();
        }
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnVictoryWalkStart += OnStartWalk;
        _controls.Enable();
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnVictoryWalkStart -= OnStartWalk;
        _controls.Disable();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnVictoryWalkStart -= OnStartWalk;
    }

    private void StartGame()
    {
        canvas.enabled = true;
        GameManager.Instance.InstantiatePlayer();
        _animator.SetTrigger(Game);
    }

    public void Disable()
    {
        GameManager.Instance.OnVictoryWalkStart -= OnStartWalk;
    }

    public void OnStartWalk()
    {
        _animator.SetTrigger(StartWalk);
    }

    public void OnEndWalk()
    {
        _animator.SetTrigger(EndWalk);
    }
}