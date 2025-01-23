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
    private void Start()
    {
        _animator = GetComponent<Animator>();
        CurrentLevelManagar.Instance.OnVictoryWalkStart += OnStartWalk;
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
        if (CurrentLevelManagar.Instance != null)
        {
            CurrentLevelManagar.Instance.OnVictoryWalkStart += OnStartWalk;
            CurrentLevelManagar.Instance.OnGameOver += OnGameOver;
            CurrentLevelManagar.Instance.OnLevelChange += OnEndWalk;
        }

        _controls.Enable();
    }

    private void OnDisable()
    {
        if (CurrentLevelManagar.Instance != null)
        {
            CurrentLevelManagar.Instance.OnVictoryWalkStart -= OnStartWalk;
            CurrentLevelManagar.Instance.OnGameOver -= OnGameOver;
            CurrentLevelManagar.Instance.OnLevelChange -= OnEndWalk;
        }

        _controls.Disable();
    }

    private void OnDestroy()
    {
        if (CurrentLevelManagar.Instance != null) CurrentLevelManagar.Instance.OnVictoryWalkStart -= OnStartWalk;
    }

    private static void GameOver()
    {
        //ends the game,quits the game and closes it
        Application.Quit();
    }

    private void StartGame()
    {
        canvas.enabled = true;
        CurrentLevelManagar.Instance.InstantiatePlayer();
        _animator.SetTrigger(Game);
    }

    private void OnStartWalk()
    {
        _animator.SetTrigger(StartWalk);
    }

    private void OnEndWalk(int x)
    {
        _animator.SetTrigger(EndWalk);
    }

    private void OnGameOver()
    {
        canvas.enabled = false;
        _didEndGame = true;
    }
}