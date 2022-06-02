# Shipbreaker SixAxis

A mod for Hardpace: Shipbreaker that enables binding any joystick or HID axis device to any game axis.

While a limited axis binding is present in Shipbreaker, this mod differs in two significant ways:

- All axes are present, including roll, and all axes can be used simultaniously without modifier keys
- Any input device can be used, not just the gamepad-aligned joystick-typed devices the game supports

This mod was primarily created for use with 3DConnexion [SpaceMouse](https://www.3dconnexion.com/products/spacemouse.html) devices, which do not identify
as joysticks and provide all six axes of control simultaniously. However, the mod is configurable and can be made to work with any HID input device.

## How it works

This mod enables additional controller axis in the game that are not normally available for binding. It does this by
taking control of the rotation and thruster code to directly set the input vectors of each. By doing this,
all of the game's dead zones and sensitivity curves are left intact, and apply to the new input.

## Releases

Download the mod [here](https://github.com/RoboPhred/shipbreaker-sixaxis/releases).

## Installation

Requires [BepInEx 5.0.1](https://github.com/BepInEx/BepInEx/releases) or later.

1. Install BepInEx in the Shipbreaker steam folder.
2. Launch the game, reach the main menu, then quit back out.
3. In the steam folder, there should now be a folder BepInEx/Plugins
4. Create a folder named `SixAxis` in the BepInEx/Plugins folder.
5. Extract the release zip file to this folder.

## Note for SpaceMouse users

By default, the SpaceMouse driver maps its controls to various utility functions when it does not recognize the program in use. Among other things, it maps the pitch rotation to the scroll wheel. This will conflict with in-game mappings of the scroll wheel, including changing the active scanner mode. You may need to remap the SpaceMouse axes using its software, or unmap the scroll wheel in Shipbreaker.

Be sure to calibrate your device before use. While the SpaceMouse is a remarkably precise device, it tends to require calibration for its deadzone to function correctly.

### Recommended keyboard bindings

As the SpaceMouse provides all 6 axes, it completely supplants a standard computer mouse. I have found the best way to play with it is with one hand on the spacemouse, one hand on the numpad.

Here are some example bindings:

- Gameplay
  - Select grapple: Numpad /
  - Select cutter: Numpad \*
  - Grapple grab / Cutter cut: Numpad 7
  - Grapple push: Numpad 8
  - Grapple Pull / Cutter mode: Numpad 9
  - Interact: Numpad 5
  - Break: Numpad enter
  - Toggle scanner: Numpad -
  - Next scanner mode: Numpad +
- Interface
  - Menu up: Numpad 8
  - Menu Down: Numpad 2
  - Menu Left: Numpad 4
  - Menu Right: Numpad 6
  - Menu Action: Numpad 5
  - Menu Alternate Action: Numpad 3
  - Menu Back: Numpad 1
  - Previous group: Numpad 7
  - Next group: Numpad 9

## Configuration

Out of the box, this mod is preconfigured for 3DConnexion [SpaceMouse](https://www.3dconnexion.com/products/spacemouse.html) devices. If you have such a device, no additional
configuration is needed. However, this mod can be configured to support any HID input device, including non-joystick axis devices.

### Creating a configuration

All device configurations are stored in the `device-configs` folder included with the mod. All files in this folder are read on startup, and any matching devices
are automatically configured.

The config files are in the YAML format, which follows a few rules:

- Named values are stored as a key and value pair, seperated by a colon; `key: value`.
- Dashes `-` are used to indicate items in a list.
- Spaces are used to nest values inside other values; typically, 2 spaces are used.

To get you started, the mod includes a `3DConnexion.yml` config file, configured for a variety of 3DConnexion six axis controllers.

The config file has two sections:

#### Devices

The `devices` section of the config file is a list of all input devices this config will apply to. It should contain a list, and each
list item should have two properties: `vendorId` and `productId`. Set these to the vendor and product IDs of the device you wish to target. Since most
vendor and product IDs are given has hex codes, you can prefix the value with `0x` to provide a hex value directly.

For example, here is a devices section that handles 2 devices: One with an ID of 4D:5C and another with 12:42

```yaml
devices:
  - vendorId: 0x4D
    productId: 0x5C
  - vendorId: 0x12
    productId: 0x42
```

#### Dual Joysticks

When using two joysticks of the exact same model, the vendor and product ids will be identical. In this case, a `deviceName` must be used. These names uniquely identify the device
based on what usb port it is plugged into, and will often need to be updated if your usb hardware changes.

Device names can be identified, with some difficulty, from the mod's logs. In the logs, the mod will record whenever it finds a new device, and what its name is. The message will look something like

```
DateTime=2022-05-30T22:40:13Z
Registering device: \\?\HID#VID_28DE&PID_1142&MI_02#8&154c7848&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030} (1, 65280)
```

In this case, the deviceName is `\\?\HID#VID_28DE&PID_1142&MI_02#8&154c7848&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}`.
The device's vendor and product id can be discovered from this name with the `VID_` and `PID_` sections of the name.

Once the name of the proper device is found, it can be specified in a config file with `deviceName`

```yaml
devices:
  - deviceName: '\\?\HID#VID_28DE&PID_1142&MI_02#8&154c7848&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}'
```

Note the use of singular quotation marks for the value. This is important, as double quotation marks will try to interpret the back slashes in the name as special characters.

#### Axes

The `axes` section is a list of input axes and the game axes they control. Each list item supports a few keys:

- `axisUsage`: This should be the HID Usage code of the axis for this input. A list of usage codes can be found [here](https://www.freebsddiary.org/APC/usb_hid_usages.php).
- `gameAxis`: The game axes this input is being bound to. Available values are `X`, `Y`, and `Z` for thrusters, `Rx`, `Ry`, and `Rz` for yaw / pitch / roll.
- `invert`: An optional value indicating whether to invert the axis. If set to `true`, the value is inverted.
- `deadZone`: An optional value indicating the dead zone as a decimal percent. `0.05` will make a 5% deadzone.
- `scale`: An optional linear scale or 'curve' as a percent. Setting this to 0.5 will make the joystick 50% less sensitive. Since this is a linear scale, 50% less sensitive means, when fully saturated, the joystick will only provide 50% of the output.

#### Buttons

Buttons on the joystick can be bound to in-game actions.

```yml
buttons:
  # Movement
  - buttonUsage: 24
    command: ThrustBrake
```

Each button entry should specify:

- `buttonUsage`: The joystick button id to bind. These start at 1, so if you use a joystick tester to detect which buttons are what, make sure it reports the first button as `1` and not `0`.
- `command`: This is the command to execute. It can either be the name of the command for simple commands, or a command object for commands that take options.

#### In-game commands.

The following commands are provided by the game and usable as `command` entries. Note that every game binding is made available, including nonfunctional, test, debug, or cheat bindings the game has but does not normally let you use.

- LeftHandGrab
- RightHandGrab
- ReleaseGrab
- Interact
- Pull
- Push
- Throw
- ScanObject
- ScanCycleLeft
- ScanCycleRight
- Start
- ReturnToMenu
- Pause
- InvertAxes
- GlassMode
- ToggleFramerate
- WorkOrder
- PlayAudioLog
- Decline
- SelectCutter
- ClearTethers
- ToggleFlashlight
- SelectGrapple
- EquipmentSelectionExtra01
- UnequipCurrentEquipment
- SelectDemoCharge
- DemoChargeFire
- DemoChargeAltFire
- GrappleFire
- ChangeEquipment
- RetractionModifier
- ExtensionModifier
- GrappleCancel
- SwitchGrappleMode
- RotateGrappledObjectLeft
- RotateGrappledObjectRight
- SwingLeft
- SwingRight
- SwingUp
- SwingDown
- PlaceTether
- RetractTethers
- RecallTethers
- CancelTether
- Reorient
- RotateBodyUp
- RotateBodyDown
- RotateBodyLeft
- RotateBodyRight
- RollBodyLeft
- RollBodyRight
- ModifiedRoll
- ThrustBrakeLeft
- ThrustBrakeRight
- ThrustMoveForward
- ThrustMoveBackward
- ThrustMoveUp
- ThrustMoveDown
- ThrustMoveLeft
- ThrustMoveRight
- LateralMoveUp
- LateralMoveDown
- LateralMoveLeft
- LateralMoveRight
- CutterFire
- CancelCut
- CutterAltFire
- ToolMenu
- ToolMode
- ToolNavUp
- ToolNavDown
- ToolNavRight
- ToolNavLeft
- ToggleStickyGrab
- Flip
- Zoom
- ToggleObjectives
- DataLog
- CycleEquipmentMode
- EquipmentSpecial
- ToggleObjectDebugInfo
- DebugIncrementTimeScale
- DebugDecrementTimeScale
- DebugResetTimeScale
- DebugPauseTimeScale
- DebugRefillOxygen
- DebugRefillThrusters
- ShowDebugControls
- ToggleDebugMenu
- ToggleBuildInfo
- DebugSaveGame
- DebugLoadGame
- DebugMegaCutPlayer
- DebugMegaCutAll
- ModifiedRollBodyLeft
- ModifiedRollBodyRight
- RotateHeadUp
- RotateHeadDown
- RotateHeadLeft
- RotateHeadRight
- listenWithAction

#### Special commands commands.

This mod provides additional special commands that are not part of the base game in order to allow more flexibility.

##### ActivateScanner

```yml
- buttonUsage: 1
  command:
    commandType: ActivateScanner
    cycleIfActive: true
```

The ActivateScanner command by default will turn on the scanner when used. However, it also has an option called `cycleIfActive`. If this action is turned on, then the scanner button will cycle to the next scanner mode if the scanner is already active when this button is pressed.

##### ThrustBreak

This command acts as both ThrustBreakLeft and ThrustBreakRight simultaniously. It exists to make a simple easy break command when the game expects both break buttons to be pressed simultaniously to function.

##### PressAndHold

```yml
- buttonUsage: 1
  command:
    commandType: PressAndHold
    shortPress: LeftHandGrab
    longPress: ReleaseGrab
    duration: 0.8
```

This is a special command that lets you configure a button to have different actions for short press, and press and hold.

It takes a `duration` value in seconds. If the button is pressed for less than this time, the `shortPress` command will be activated for a short time.
If the button is held for longer than `duration`, the `longPress` command will be activated, and held down for as long as the button is.

## Joystick requests welcome

I love weird input devices. If you know of another interesting joystick or controller you would like to see support for, please let me know!
