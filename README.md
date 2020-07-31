# Shipbreaker SixAxis

A mod for Hardpace: Shipbreaker that enables six axis controllers.

Uniquely, this mod gives Shipbreaker all 6 axes of control, meaning joystick input can independently control:

- Forward / Backward
- Left / Right
- Up / Down
- Pitch
- Roll
- Yaw

## Compatible devices

Currently, this mod is hard coded for 3DConnexion [SpaceMouse](https://www.3dconnexion.com/products/spacemouse.html) devices, but should work for compatible devices.

In theory, any device with the HID type of `GenericDesktop : MultiAxisController` should work, but currently the axes are hard coded to fit the orientation of the SpaceMouse. Other devices may use different axis mapping, and not be compatible.

Support for such devices will be available once axes are configurable. See [Planned features](#planned-features).

## Releases

Download the mod [here](https://github.com/RoboPhred/shipbreaker-sixaxis/releases)

## Installation

Requires [BepInEx 5.0.1](https://github.com/BepInEx/BepInEx/releases) or later.

1. Install BepInEx in the Shipbreaker steam folder.
2. Launch the game, reach the main menu, then quit back out.
3. In the steam folder, there should now be a folder BepInEx/Plugins
4. Create a folder named `SixAxis` in the BepInEx/Plugins folder.
5. Extract the release zip file to this folder.

## Configuration and Sensitivity

Because this mod injects input into the existing control system, the sensitivity sliders still work.
However, the game will use whatever slider corresponds to the last input it received, not including the SixAxis device.
This means the game will typically use the mouse sensitivity values, unless you plug in a controller the game recognizes and use it for a short period.

Axis inversion in the game is implemented in its controller library, so the invert axes options will not apply to this mod. Axis configuration [is planned](#planned-features).

### Configuring the SpaceMouse

By default, the SpaceMouse driver maps its controls to various utility functions when it does not recognize the program in use. Among other things, it maps the pitch rotation to the scroll wheel. This will conflict with in-game mappings of the scroll wheel, including changing the active scanner mode. You may need to remap the SpaceMouse axes using its software, or unmap the scroll wheel in Shipbreaker.

Be sure to calibrate your device before use. While the SpaceMouse is a remarkably precise device, it tends to require calibration to zero correctly.

## Recommended usage

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

## How it works

This mod enables additional controller axis in the game that are not normally available for binding. It does this by
taking control of the rotation and thruster code to directly set the input vectors of each. By doing this,
all of the game's dead zones and sensitivity curves are left intact, and apply to the new input.

## Planned features

- Button inputs for actions (cutter, tethers, and so on).
- Configurable bindings of any controller axis to any game axis, including curves and inversion.
- Configure exact joystick device identifiers to support dual joystick setups.
- Sensitivity controls per-axis. Currently falls back to using the sensitivity of the last input device used.

### Joystick requests welcome

I love weird input devices. If you know of another interesting joystick or controller you would like to see support for, please let me know!
