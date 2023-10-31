namespace FiniteStateMaсhine
{
    public interface IFactory<out TState> where TState : IState
    {
        TState Create(StateMachine stateMachine, Butler butler);
    }
}