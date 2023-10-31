namespace FiniteStateMaсhine
{
    public interface ITransition : IUnique<byte>
    {
        byte? From { get; }
        byte To { get; }
        bool IsAllowed(StateMachine stateMachine, Butler butler);
    }
}