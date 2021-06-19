
using BBI.Unity.Game;

namespace RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands
{
    class SelectCutterCommand : IButtonCommand
    {
        private RemotedBindingSource cutterCommand = new RemotedBindingSource();

        public SelectCutterCommand()
        {
            ControlsReadyMonitor.RunWhenControlsReady(() =>
            {
                LynxControls.Instance.GameplayActions.SelectCutter.AddBinding(cutterCommand);
            });
        }

        public void Press()
        {
            cutterCommand.State = true;
        }

        public void Release()
        {
            cutterCommand.State = false;
        }
    }
}