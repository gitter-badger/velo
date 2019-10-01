namespace Velo.CQRS.Commands
{
    internal interface ICommandProcessor<in TCommand> : ICommandProcessor
        where TCommand : ICommand
    {
        void Execute(TCommand command);
    }

    internal interface ICommandProcessor
    {
    }
}