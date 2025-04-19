
public abstract class PlayerBaseState
{
    protected PlayerStateMachine _ctx; // access type variables and methods that are inherited by derived classes
    protected PlayerStateFactory _factory;
    protected PlayerBaseState _currentSubState;
    protected PlayerBaseState _currentSuperState;
    public PlayerBaseState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    {
        _ctx = currentContext;
        _factory = playerStateFactory;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubState();

    void UpdateStates() { }
    protected void SwitchState(PlayerBaseState newState)
    {
        // current state exists state
        ExitState();

        // new state enters state
        newState.EnterState();

        // switch current state of context
        _ctx.CurrentState = newState;
    }
    protected void SetSuperState(PlayerBaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }
    // anytime calling SetSubState create almost a parent child and the inverse
    protected void SetSubState(PlayerBaseState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}
