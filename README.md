# Shipbreaker SixAxis

A mod for Hardpace: Shipbreaker that enables binding any joystick or HID axis device to any game axis.

While axis binding is on the Shipbreaker roadmap, this mod differs in two significant ways:

- All axes are present, including roll, and all axes can be used simultaniously without modifier keys
- Any input device can be used, not just the gamepad-aligned joystick-typed devices the game supports.

This mod was primarily created for use with 3DConnexion [SpaceMouse](https://www.3dconnexion.com/products/spacemouse.html) devices, which do not identify
as joysticks and provide all six axes of control simultaniously. However, the mod is configurable and can be made to work with any HID input device.

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
  -

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

#### Axes

The `axes` section is a list of input axes and the game axes they control. Each list item supports a few keys:

_axisUsage_: This should be the HID Usage code of the axis for this input. A list of usage codes can be found [here](https://www.freebsddiary.org/APC/usb_hid_usages.php).
_gameAxis_: The game axes this input is being bound to. Available values are `X`, `Y`, and `Z` for thrusters, `Rx`, `Ry`, and `Rz` for yaw / pitch / roll.
_invert_: An optional value indicating whether to invert the axis. If set to `true`, the value is inverted.

## Axis Sensitivity

Because this mod injects input into the existing control system, the sensitivity sliders still work.
However, the game will use whatever slider corresponds to the last input it received, not including the SixAxis device.
This means the game will typically use the mouse sensitivity values, unless you plug in a controller the game recognizes and use it for a short period.

Adjusting curves for axis inputs is planned, but not currently available. See [planned features](#planned-features).

## How it works

This mod enables additional controller axis in the game that are not normally available for binding. It does this by
taking control of the rotation and thruster code to directly set the input vectors of each. By doing this,
all of the game's dead zones and sensitivity curves are left intact, and apply to the new input.

## Planned features

- Button inputs for actions (cutter, tethers, and so on).
- Axis curves and sensitivity
- Configure exact joystick device identifiers to support dual joystick setups.

### Joystick requests welcome

I love weird input devices. If you know of another interesting joystick or controller you would like to see support for, please let me know!
