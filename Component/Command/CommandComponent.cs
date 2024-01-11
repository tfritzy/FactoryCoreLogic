using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Core
{
    public class CommandComponent : Component
    {
        public override ComponentType Type => ComponentType.Command;
        public Queue<Command> Commands = new();

        public CommandComponent(Schema.Command schema, Entity owner) : this(owner) { }

        public CommandComponent(Entity owner) : base(owner)
        {
        }

        public override Schema.OneofComponent ToSchema()
        {
            return new Schema.OneofComponent
            {
                Command = new Schema.Command()
            };
        }

        public void ReplaceCommands(Command command)
        {
            Commands.Clear();
            Commands.Enqueue(command);
        }

        public void AddCommand(Command command)
        {
            Commands.Enqueue(command);
        }

        public void ClearCommands()
        {
            Commands.Clear();
        }

        public Command? CurrentCommand => Commands.FirstOrDefault();

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);

            if (CurrentCommand != null)
            {
                CurrentCommand.CheckIsComplete();
                if (CurrentCommand.IsComplete)
                {
                    Commands.Dequeue();
                }
            }
        }
    }
}