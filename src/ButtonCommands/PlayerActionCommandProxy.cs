
using System;
using System.Reflection;
using BBI.Unity.Game;
using InControl;

namespace RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands
{
    class PlayerActionCommandProxy : IButtonCommand
    {

        private readonly FieldInfo field;
        private RemotedBindingSource bindingSource = new();

        public PlayerActionCommandProxy(string fieldName)
        {
            this.field = typeof(GameplayActions).GetField(fieldName, BindingFlags.Public | BindingFlags.Instance) ?? typeof(GameplayActions).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (this.field == null)
            {
                throw new ArgumentException($"{fieldName} is not a valid field name.");
            }

            if (this.field.FieldType != typeof(PlayerAction))
            {
                throw new ArgumentException($"{fieldName} is not a PlayerAction field.");
            }

            GameplayActionsMonitor.RunWhenGameplayActionsCreated((actions) =>
            {
                // We need to create a new binding source each time, as the old source will refuse to re-bind.
                this.bindingSource = new();
                if (this.field.GetValue(actions) is PlayerAction playerAction)
                {
                    playerAction.AddBinding(this.bindingSource);
                }
            });
        }

        public void Press()
        {
            bindingSource.State = true;
        }

        public void Release()
        {
            bindingSource.State = false;
        }
    }
}