
using BBI.Unity.Game;

namespace RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands
{
    class CutterAltFireCommand : IButtonCommand
    {
        private RemotedBindingSource bindingSource = new RemotedBindingSource();

        public CutterAltFireCommand()
        {
            ControlsReadyMonitor.RunWhenControlsReady(() =>
            {
                LynxControls.Instance.GameplayActions.CutterAltFire.AddBinding(bindingSource);
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