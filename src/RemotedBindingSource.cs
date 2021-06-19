
using System.IO;
using InControl;

namespace RoboPhredDev.Shipbreaker.SixAxis
{
    class RemotedBindingSource : BindingSource
    {
        private bool state;
        public bool State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
                Value = state ? 1 : 0;
            }
        }

        public float Value { get; set; }

        public override string Name => "SixAxis Binding";

        public override string DeviceName => "SixAxis Device";

        public override InputDeviceClass DeviceClass => InputDeviceClass.Controller;

        public override InputDeviceStyle DeviceStyle => InputDeviceStyle.Unknown;

        public override BindingSourceType BindingSourceType => BindingSourceType.UnknownDeviceBindingSource;


        public override bool Equals(BindingSource other)
        {
            return this == other;
        }

        public override bool GetState(InControl.InputDevice inputDevice)
        {
            return this.State;
        }

        public override float GetValue(InControl.InputDevice inputDevice)
        {
            return this.Value;
        }

        public override void Load(BinaryReader reader, ushort dataFormatVersion)
        {
        }

        public override void Save(BinaryWriter writer)
        {
        }
    }
}