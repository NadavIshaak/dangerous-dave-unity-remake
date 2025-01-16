using UnityEngine;

public class GroundedState : PlayerState
{
    public GroundedState(PlayerMovement player) : base(player)
    {
        _moveSound=player.GetMoveSound();
        _fallingSound=player.GetFallingSound();
        _controls=player.GetControls();
        _animationConttroler=player.GetAnimationConttroler();
        _playerTransform=player.GetTransform();
        _jumpForce=player.GetJumpForce();
        _collider=player.GetCollider();
        _rb=player.GetRigidbody();
        _wallLayerMask=player.GetWallLayerMask();
        _moveSpeed=player.GetMoveSpeed();
    }
    private bool _isRight=false;
    private bool _isStop;
    private bool _isFalling=false;
    private bool _firstMove;
    private readonly AudioClip _moveSound;
    private readonly AudioClip _fallingSound;
    private readonly InputSystem_Actions _controls;
    private readonly Transform _playerTransform;
    private readonly Rigidbody2D _rb;
    private readonly Collider2D _collider;
    private Vector2 _moveInput;
    private readonly PlayerAnimationConttroler _animationConttroler;
    private readonly LayerMask _wallLayerMask;
    private readonly float _jumpForce;
    private readonly float _moveSpeed;
    
    public override void Enter()
    {
        _isStop=true;
        _firstMove=false;
    }
    

    public override void HandleInput()
    {
        if (!_controls.Player.Jump.triggered || _isFalling != false) return;
        JumpTransition();
    }

    public bool GetIsRight()
    {
        return _isRight;
    }

    public override void Update()
    {
        CheckFirstMoveAndDirection();
        CheckInputAndAnimate();
    }

    public override void Exit()
    {
        _animationConttroler.ResumeMovement();
    }
    // ReSharper disable Unity.PerformanceAnalysis
    private void PlaySound(bool shouldKeep,bool shouldLoop,AudioClip clip)
    {
        SoundManager.Instance.PlaySound(clip,_playerTransform,1,shouldLoop,shouldKeep);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void CheckFirstMoveAndDirection(){
        if(player.GetMoveInput().x!=0&&!_firstMove)
        {
            _animationConttroler.Move();
            PlaySound(true,true,_moveSound);
            if(player.GetMoveInput().x>0){
                _animationConttroler.ChangeDirection(true);
                _isRight=true;
            }
            else{
                _animationConttroler.ChangeDirection(false);
                _isRight=false;
            }
            _firstMove=true;
        }
        else if(_controls.Player.Jump.triggered&&!_firstMove)
        {
            JumpTransition();
        }
    }

    private void JumpTransition()
    {
       _animationConttroler.Jump();
       _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        player.TransitionToState(player.airborneState);
    }

    private void CheckInputAndAnimate()
    {
        var bounds = _collider.bounds;
         var bottomLeft = new Vector2(bounds.min.x, bounds.min.y + 0.1f); // Add a small buffer distance
        var bottomRight = new Vector2(bounds.max.x, bounds.min.y + 0.1f); // Add a small buffer distance
        var hitLeft = Physics2D.Raycast(bottomLeft, Vector2.down, 0.21f, _wallLayerMask);
        var hitRight = Physics2D.Raycast(bottomRight, Vector2.down, 0.21f, _wallLayerMask);
        _rb.linearVelocity = new Vector2(player.GetMoveInput().x * _moveSpeed, _rb.linearVelocity.y);
        CheckFirstMoveAndDirection();
        if(player.GetMoveInput().x>0&&!_isRight&&_firstMove){
           _animationConttroler.ChangeDirection(true);
            _isRight=true;
        }
        else if(player.GetMoveInput().x<0&&_isRight&&_firstMove){
           _animationConttroler.ChangeDirection(false);
            _isRight=false;
        }
        else if(player.GetMoveInput().x==0&&!_isStop&&!_isFalling&&_firstMove){
            _animationConttroler.StopInMovement();
            SoundManager.Instance.stopSound();
            _isStop=true;
        }
        else if(player.GetMoveInput().x!=0&&_isStop&&!_isFalling&&_firstMove){
            _animationConttroler.ResumeMovement();
            PlaySound(true,true,_moveSound);
            _isStop=false;
        }
        else switch (_isFalling)
        {
            case false when (hitLeft.collider is null && hitRight.collider is null) && _firstMove:
                PlaySound(true,true,_fallingSound);
                _isFalling=true;
                _animationConttroler.FallWhileWalking();
                break;
            case true when (hitLeft.collider is not null || hitRight.collider is not null):
                _isFalling=false;
                _isStop=true;
                _firstMove=false;
                SoundManager.Instance.stopSound();
               _animationConttroler.HitGroundWithMovement();
                break;
        }
    }
}