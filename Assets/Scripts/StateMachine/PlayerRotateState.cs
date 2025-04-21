using UnityEngine;

public class PlayerRotateState : PlayerBaseState
{
    public PlayerRotateState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        Debug.Log("Entering");
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, false);
        Ctx.Animator.SetBool(Ctx.IsRunningHash, false);


        if (Ctx.IsTurningLeft)
        {
            Ctx.Animator.SetBool(Ctx.IsTurningLeftHash, true);
            Ctx.Animator.SetBool(Ctx.IsTurningRightHash, false);
        }
        else
        {
            Ctx.Animator.SetBool(Ctx.IsTurningLeftHash, false);
            Ctx.Animator.SetBool(Ctx.IsTurningRightHash, true);
        }

        Ctx.AppliedMovementX = 0;
        Ctx.AppliedMovementZ = 0;
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        Ctx.Animator.SetBool(Ctx.IsTurningLeftHash, false);
        Ctx.Animator.SetBool(Ctx.IsTurningRightHash, false);
    }
    public override void InitializeSubState()
    {

    }
    public override void CheckSwitchStates()
    {
        if (!Ctx.IsTurningLeft && !Ctx.IsTurningRight)
        {
            if (!Ctx.IsMovementPressed)
            {
                SwitchState(Factory.Idle());
            }
            else
            {
                SwitchState(Factory.Walk());
            }
        }
        else if(Ctx.IsMovementPressed && Ctx.IsRunPressed)
        {
            SwitchState(Factory.Run());
        }
        
    }
}
