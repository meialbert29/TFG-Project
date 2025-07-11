
public abstract class PlayerBaseState
{
    private bool _isRootState = false;
    private PlayerStateMachine _ctx; // access type variables and methods that are inherited by derived classes
    private PlayerStateFactory _factory;
    private PlayerBaseState _currentSubState;
    private PlayerBaseState _currentSuperState;

    protected bool IsRootState { set { _isRootState = value; } }    
    protected PlayerStateMachine Ctx { get { return _ctx; } }
    protected PlayerStateFactory Factory { get { return _factory; } }

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

    public void UpdateStates()
    {
        UpdateState();

        // if has a SubState will call UpdateStates on that
        // all SubStates that are part of the SuperState-SubState chain to Update
        if (_currentSubState != null)
        {
            _currentSubState.UpdateStates();
        }
    }

    protected void SwitchState(PlayerBaseState newState)
    {
        // current state exists state
        ExitState();

        // new state enters state
        newState.EnterState();

        // only switch context's current state if it's at the tip level of the state chain
        if (_isRootState)
        {
            // switch current state of context
            _ctx.CurrentState = newState;
        }
        else if(_currentSuperState != null)
        {
            _currentSuperState.SetSubState(newState);
        }
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
