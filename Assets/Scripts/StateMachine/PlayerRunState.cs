using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, true);
        Ctx.Animator.SetBool(Ctx.IsWalkingBackwardHash, false);
        Ctx.Animator.SetBool(Ctx.IsRunningHash, true);
    }
    public override void UpdateState()
    {
        CheckSwitchStates();

        if (Ctx.CharacterController.isGrounded)
        {
            Ctx.CurrentMovementY = Ctx.GroundedGravity;
        }
        else
        {
            Ctx.CurrentMovementY += Ctx.Gravity * Time.deltaTime;
            Ctx.CurrentMovementY = Mathf.Max(Ctx.CurrentMovementY, -20f);
        }
        Vector3 horizontalMovement = Ctx.Transform.forward * Ctx.RunMultiplier;
        Vector3 totalMovement = new Vector3(horizontalMovement.x, Ctx.CurrentMovementY, horizontalMovement.z);

        Ctx.CharacterController.Move(totalMovement * Time.deltaTime);
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
        else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SwitchState(Factory.Walk());
        }
    }
}
