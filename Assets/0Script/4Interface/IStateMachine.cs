public interface IStateMachine
{
    void EnterState();  // Called when entering the state
    void UpdateState(); // Called every frame
    void ExitState();   // Called when exiting the state
}