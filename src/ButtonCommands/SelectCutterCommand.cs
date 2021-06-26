
using BBI.Unity.Game;

namespace RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands
{
    class SelectCutterCommand : IButtonCommand
    {
        private RemotedBindingSource bindingSource = new RemotedBindingSource();

        public SelectCutterCommand()
        {
            ControlsReadyMonitor.RunWhenControlsReady(() =>
            {
                LynxControls.Instance.GameplayActions.SelectCutter.AddBinding(bindingSource);
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