using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class CommandComponent : Component
    {
        public override ComponentType Type => ComponentType.Command;
        private Queue<Command> commands = new();

        public CommandComponent(Entity owner) : base(owner)
        {
        }

        public override Schema.Component ToSchema()
        {
            throw new System.NotImplementedException();
        }

        public void ReplaceCommands(Command command)
        {
            commands.Clear();
            commands.Enqueue(command);
        }

        public void AddCommand(Command command)
        {
            commands.Enqueue(command);
        }

        public Command? CurrentCommand => commands.FirstOrDefault();

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);

            if (CurrentCommand != null)
            {
                CurrentCommand.CheckIsComplete();
                if (CurrentCommand.IsComplete)
                {
                    commands.Dequeue();
                }
            }
        }
    }
}