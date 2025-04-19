using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{

    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    {
        InitializeSubState();
    }

    public override void EnterState()
    {
        _ctx.CurrentMovementY = _ctx.GroundedGravity;
        _ctx.CurrentRunMovementY = _ctx.GroundedGravity;
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {

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
        else if(_ctx.IsMovementPressed && _ctx.IsRunPressed)
        {
            SetSubState(_factory.Run());
        }
    }
    public override void CheckSwitchStates()
    {
        // if the player is grounded and jump is pressed -> switch to jump state
        if (_ctx.IsJumpPressed && !_ctx.RequireNewJumpPress)
        {
            SwitchState(_factory.Jump());
        }
    }
}
