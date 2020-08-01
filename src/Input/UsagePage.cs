namespace RoboPhredDev.Shipbreaker.SixAxis.Input
{
    // https://www.freebsddiary.org/APC/usb_hid_usages.php
    enum UsagePage : ushort
    {
        GenericDesktop = 0x01,
        SimulationControls = 0x02,
        VRControls = 0x03,
        SportsControls = 0x04,
        GameControls = 0x05,
        Keyboard = 0x07,
        Button = 0x09,
        CameraControl = 144,
        ArcadeDevice = 145
    }
}