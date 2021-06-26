
using BBI.Unity.Game;

namespace RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands
{
    class RetractionCommand : IButtonCommand
    {
        private RemotedBindingSource bindingSource = new RemotedBindingSource();

        public RetractionCommand()
        {
            ControlsReadyMonitor.RunWhenControlsReady(() =>
            {
                LynxControls.Instance.GameplayActions.RetractionModifier.AddBinding(bindingSource);
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