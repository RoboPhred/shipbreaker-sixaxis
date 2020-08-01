namespace RoboPhredDev.Shipbreaker.SixAxis.Input
{
    enum GenericDesktopUsage : ushort
    {
        Undefined = 0x00,
        Pointer = 0x01,
        Mouse = 0x02,
        Joystick = 0x04,
        GamePad = 0x05,
        Keyboard = 0x06,
        Keypad = 0x07,
        MultiAxisController = 0x08,

        X = 0x30,
        Y = 0x31,
        Z = 0x32,
        Rx = 0x33,
        Ry = 0x34,
        Rz = 0x35,
        Slider = 0x36,
        Dial = 0x37,
        Wheel = 0x38,
        HatSwitch = 0x39,
        CountedBuffer = 0x3A,
        ByteCount = 0x3B,
        MotionWakeup = 0x3C,
        Vx = 0x40,
        Vy = 0x41,
        Vz = 0x42,
        Vbrx = 0x43,
        Vbry = 0x44,
        Vbrz = 0x45,
        Vno = 0x46,
        SystemControl = 0x80,
        SystemPowerDown = 0x81,
        SystemSleep = 0x82,
        SystemWakeUp = 0x83,
        SystemContextMenu = 0x84,
        SystemMainMenu = 0x85,
        SystemAppMenu = 0x86,
        SystemMenuHelp = 0x87,
        SystemMenuExit = 0x88,
        SystemMenuSelect = 0x89,
        SystemMenuRight = 0x8A,
        SystemMenuLeft = 0x8B,
        SystemMenuUp = 0x8C,
        SystemMenuDown = 0x8D,
        DPadUp = 0x90,
        DPadDown = 0x91,
        DPadRight = 0x92,
        DPadLeft = 0x93
    }
}