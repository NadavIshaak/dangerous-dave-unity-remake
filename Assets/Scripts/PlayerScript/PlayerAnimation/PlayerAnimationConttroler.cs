using System;
using UnityEngine;

public class PlayerAnimationConttroler : MonoBehaviour
{
    private static readonly int Move1 = Animator.StringToHash("Move");
    private static readonly int Jump1 = Animator.StringToHash("Jump");
    private static readonly int WhileWalking = Animator.StringToHash("FallWhileWalking");
    private static readonly int GroundWithMovement = Animator.StringToHash("HitGroundWithMovement");
    private static readonly int GroundWithoutMovement = Animator.StringToHash("HitGroundWithoutMovement");
    private static readonly int Death1 = Animator.StringToHash("Death");
    private static readonly int Pack = Animator.StringToHash("JetPack");
    private Animator _animator;
    private AudioSource _audioSource;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Move()
    {
        _animator.SetTrigger(Move1);
    }

    public void ChangeDirection(bool facingRight)
    {
        _spriteRenderer.flipX = !facingRight;
    }

    public void StopInMovement()
    {
        _animator.speed = 0;
    }

    public void StopMovement()
    {
        Invoke(nameof(StopInMovement), 0.05f);
    }

    public void ResumeMovement()
    {
        CancelInvoke();
        _animator.speed = 1;
    }

    public void Jump()
    {
        _animator.SetTrigger(Jump1);
    }

    public void FallWhileWalking()
    {
        _animator.SetTrigger(WhileWalking);
    }

    public void HitGroundWithMovement()
    {
        _animator.SetTrigger(GroundWithMovement);
    }

    public void HitGroundWithoutMovement()
    {
        _animator.SetTrigger(GroundWithoutMovement);
    }

    public void Death()
    {
        _animator.SetTrigger(Death1);
    }

    public void JetPack()
    {
        _animator.SetTrigger(Pack);
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }
}