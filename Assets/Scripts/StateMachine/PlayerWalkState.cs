using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, true);
        Ctx.Animator.SetBool(Ctx.IsRunningHash, false);
    }
    public override void UpdateState()
    {
        CheckSwitchStates();

        Vector3 _appliedMovement = Vector3.zero;

        if (Ctx.IsMovingForward)
        {
            _appliedMovement = Ctx.Transform.forward;
        }
        else if (Ctx.IsMovingBackward)
        {
            _appliedMovement = -Ctx.Transform.forward;
        }

        Ctx.CharacterController.Move(_appliedMovement * Time.deltaTime);
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
            SwitchState(Factory.Idle());
        }
        else if (Ctx.IsMovementPressed && Ctx.IsRunPressed)
        {
            SwitchState(Factory.Run());
        }
    }
}
