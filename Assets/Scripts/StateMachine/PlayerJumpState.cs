using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        HandleJump();
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
        HandleGravity();
    }
    public override void ExitState()
    {
        _ctx.Animator.SetBool(_ctx.IsJumpingHash, false);
        if (_ctx.IsJumpPressed)
        {
            _ctx.RequireNewJumpPress = true;
        }
    }
    public override void InitializeSubState()
    {
        if (!_ctx.IsMovementPressed && !_ctx.IsRunPressed)
        {
            SetSubState(_factory.Idle());
        }
        else if (_ctx.IsMovementPressed && !_ctx.IsRunPressed)
        {
            SetSubState(_factory.Walk());
        }
        else if (_ctx.IsMovementPressed && _ctx.IsRunPressed)
        {
            SetSubState(_factory.Run());
        }
    }
    public override void CheckSwitchStates()
    {
        if (_ctx.CharacterController.isGrounded)
        {
            SwitchState(_factory.Grounded());
        }
    }

    void HandleJump()
    {
        _ctx.Animator.SetBool(_ctx.IsJumpingHash, true);
        _ctx.isJumping = true;
        _ctx.CurrentMovementY = _ctx.InitialJumpVelocity * 0.5f;
        _ctx.CurrentRunMovementY = _ctx.InitialJumpVelocity * 0.5f;
    }

    void HandleGravity()
    {
        bool isFalling = _ctx.CurrentMovementY <= 0.0f || !_ctx.IsJumpPressed;
        float fallMultiplier = 2.0f;

        if (isFalling)
        {
            float previousYVelocity = _ctx.CurrentMovementY;
            float newYVelocity = _ctx.CurrentMovementY + (_ctx.Gravity * fallMultiplier * Time.deltaTime);
            float nextYVelocity = Mathf.Max((previousYVelocity + newYVelocity) * 0.5f, -20.0f);
            _ctx.CurrentMovementY = nextYVelocity;
            _ctx.CurrentRunMovementY = nextYVelocity;
        }
        else
        {
            float previousYVelocity = _ctx.CurrentMovementY;
            float newYVelocity = _ctx.CurrentMovementY + (_ctx.Gravity * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * 0.5f;
            _ctx.CurrentMovementY = nextYVelocity;
            _ctx.CurrentRunMovementY = nextYVelocity;
        }
    }
}
