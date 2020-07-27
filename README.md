# Shipbreaker SixAxis

A mod for Hardpace: Shipbreaker that enables six axis controllers like 3DConnexion / SpaceMouse devices.

Uniquely, this mod gives Shipbreaker all 6 axes of control, meaning joystick input can independently control:

- Forward / Backward
- Left / Right
- Up / Down
- Pitch
- Roll
- Yaw

## Compatible devices

Currently, this mod is hard coded for 3DConnexion / SpaceMouse devices. This means input devices are restricted in two ways:

- It looks for HID type `GenericDesktop : MultiAxisController`
- The axes are bound in a way to translate the SpaceMouse puck to the first person oriented controls of the game.

Other devices may be compatible, if they use the same HID category and have a similar axis layout.

Compatibility for other devices, axis mapping, axis inversion, and button mapping is planned. See [Planned features](#planned-features).

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

## How it works

This mod enables additional controller axis in the game that are not normally available for binding. It does this by
taking control of the rotation and thruster code to directly set the input vectors of each. By doing this,
all of the game's dead zones and sensitivity curves are left intact, and apply to the new input.

## Planned features

- Button inputs for actions (cutter, tethers, and so on).
- Configurable bindings of any controller axis to any game axis, including curves and inversion.
- Configure exact joystick device identifiers to support dual joystick setups.
- Sensitivity controls per-axis. Currently falls back to using the sensitivity of the last input device used.
