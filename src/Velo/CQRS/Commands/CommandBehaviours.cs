using System;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Commands
{
    internal sealed class CommandBehaviours<TCommand> : IDisposable
        where TCommand : ICommand
    {
        public readonly bool HasBehaviours;

        private ICommandBehaviour<TCommand>[] _behaviours;
        private CommandPipeline<TCommand> _pipeline;

        public CommandBehaviours(CommandPipeline<TCommand> pipeline, ICommandBehaviour<TCommand>[] behaviours)
        {
            HasBehaviours = behaviours.Length > 0;

            _behaviours = behaviours;
            _pipeline = pipeline;
        }

        public Task Execute(TCommand command, CancellationToken cancellationToken)
        {
            var closure = new Closure(this, command, cancellationToken);
            return closure.Execute();
        }

        private sealed class Closure
        {
            private readonly CommandBehaviours<TCommand> _context;
            private readonly TCommand _command;
            private readonly CancellationToken _cancellationToken;
            private readonly Func<Task> _next;

            private int _position;

            public Closure(CommandBehaviours<TCommand> context, TCommand command, CancellationToken cancellationToken)
            {
                _context = context;
                _command = command;
                _cancellationToken = cancellationToken;

                _next = Execute;

                _position = 0;
            }

            public Task Execute()
            {
                var behaviours = _context._behaviours;

                // ReSharper disable once InvertIf
                if ((uint) _position < (uint) behaviours.Length)
                {
                    var behaviour = behaviours[_position++];
                    return behaviour.Execute(_command, _next, _cancellationToken);
                }

                return _context._pipeline.RunProcessors(_command, _cancellationToken);
            }
        }

        public void Dispose()
        {
            _behaviours = null;
            _pipeline = null;
        }
    }
}