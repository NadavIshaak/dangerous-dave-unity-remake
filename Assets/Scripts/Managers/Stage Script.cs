using UnityEngine;
using UnityEngine.Serialization;

/**
 * This class is responsible for managing the stage.
 * It starts the game, ends the game, and handles the stage animations
 * that means transaction between cameras.
 */
public class StageScript : MonoBehaviour
{
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
        CurrentLevelManagar.instance.LevelManager.OnVictoryWalkStart += OnStartWalk;
        CurrentLevelManagar.instance.LevelManager.OnGameOver += OnGameOver;
        CurrentLevelManagar.instance.LevelManager.OnLevelChange += OnEndWalk;
        _animator = GetComponent<Animator>();
        canvas.enabled = false;
        _didStartGame = false;
        _controls = new InputSystem_Actions();
        _controls.Enable();
    }

    /**
     * when is start screen look for the jump button to start the game
     * when the game is over look for the jump button to quit the game
     */
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


    private void OnDisable()
    {
        if (CurrentLevelManagar.instance)
        {
            CurrentLevelManagar.instance.LevelManager.OnVictoryWalkStart -= OnStartWalk;
            CurrentLevelManagar.instance.LevelManager.OnGameOver -= OnGameOver;
            CurrentLevelManagar.instance.LevelManager.OnLevelChange -= OnEndWalk;
        }

        _controls.Disable();
    }

    private void OnDestroy()
    {
        if (CurrentLevelManagar.instance != null)
            CurrentLevelManagar.instance.LevelManager.OnVictoryWalkStart -= OnStartWalk;
    }

    /**
     * Ends the game
     */
    private static void GameOver()
    {
        //ends the game,quits the game and closes it
        Application.Quit();
    }

    /**
     * Starts the game, instantiates the player and shows the stage 1
     */
    private void StartGame()
    {
        canvas.enabled = true;
        CurrentLevelManagar.instance.InstantiatePlayer();
        _animator.SetTrigger(Game);
    }

    /**
     * Makes the cameras move to win walk screen
     */
    private void OnStartWalk()
    {
        _animator.SetTrigger(StartWalk);
    }

    /**
     * Makes the cameras move to the next level
     */
    private void OnEndWalk(int x)
    {
        _animator.SetTrigger(EndWalk);
    }

    /**
     * When the game is over, disable the canvas and set the game over flag
     */
    private void OnGameOver()
    {
        canvas.enabled = false;
        _didEndGame = true;
    }
}