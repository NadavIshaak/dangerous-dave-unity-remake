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
    private bool _didEndGame;
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

    private void Start()
    {
        _animator = GetComponent<Animator>();
        CurrentLevelManagar.instance.OnVictoryWalkStart += OnStartWalk;
        canvas.enabled = false;
    }

    private void Update()
    {
        if (!_didStartGame && _controls.Player.Jump.triggered)
        {
            _didStartGame = true;
            StartGame();
        }
        else if (_didEndGame && _controls.Player.Jump.triggered)
        {
            GameOver();
        }
    }

    private void OnEnable()
    {
        if (CurrentLevelManagar.instance != null)
        {
            CurrentLevelManagar.instance.OnVictoryWalkStart += OnStartWalk;
            CurrentLevelManagar.instance.OnGameOver += OnGameOver;
        }

        _controls.Enable();
    }

    private void OnDisable()
    {
        if (CurrentLevelManagar.instance != null)
        {
            CurrentLevelManagar.instance.OnVictoryWalkStart -= OnStartWalk;
            CurrentLevelManagar.instance.OnGameOver -= OnGameOver;
        }

        _controls.Disable();
    }

    private void OnDestroy()
    {
        if (CurrentLevelManagar.instance != null) CurrentLevelManagar.instance.OnVictoryWalkStart -= OnStartWalk;
    }

    private static void GameOver()
    {
        //ends the game,quits the game and closes it
        Application.Quit();
    }

    private void StartGame()
    {
        canvas.enabled = true;
        CurrentLevelManagar.instance.InstantiatePlayer();
        _animator.SetTrigger(Game);
    }

    private void OnStartWalk()
    {
        _animator.SetTrigger(StartWalk);
    }

    public void OnEndWalk()
    {
        _animator.SetTrigger(EndWalk);
    }

    private void OnGameOver()
    {
        canvas.enabled = false;
        _didEndGame = true;
    }
}