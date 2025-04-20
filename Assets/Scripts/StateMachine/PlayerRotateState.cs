using UnityEngine;

public class PlayerRotateState : PlayerBaseState
{
    public PlayerRotateState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        Ctx.Animator.SetBool(Ctx.IsTurningLeftHash, false);
        Ctx.Animator.SetBool(Ctx.IsTurningRightHash, false);
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

    }
    public override void CheckSwitchStates()
    {
        if (!Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Idle());
        }
        if (Ctx.IsMovementPressed && Ctx.IsRunPressed)
        {
            SetSubState(Factory.Run());
        }
        else if (Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Walk());
        }
    }
}
