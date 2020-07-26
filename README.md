# Shipbreaker SixAxis

A mod for Hardpace: Shipbreaker that enables six axis controllers like 3DConnexion / SpaceMouse devices.

Uniquely, this mod gives Shipbreaker all 6 axes of control, meaning joystick input can independently control:

- Forward / Backward
- Left / Right
- Up / Down
- Pitch
- Roll
- Yaw

Currently, this mod is hard coded for 3DConnexion / SpaceMouse devices.  Use of other 6dof joysticks or input devices may not be mapped correctly.
Eventually, I intend this mod to become a fully configurable six axis input solution, supporting multiple joysticks, axes, and buttons.  See [Planned features](#planned-features)

# Releases

Download the mod [here](https://github.com/RoboPhred/shipbreaker-sixaxis/releases)

# Installation

Requires [BepInEx 5.0.1](https://github.com/BepInEx/BepInEx/releases) or later.

1. Install BepInEx in the Shipbreaker steam folder.
2. Launch the game, reach the main menu, then quit back out.
3. In the steam folder, there should now be a folder BepInEx/Plugins
4. Create a folder named `SixAxis` in the BepInEx/Plugins folder.
5. Extract the release zip file to this folder.

With any luck, we will get official support for this in the future. However, ShipBreaker uses the InControl unity library for its controller input,
which restricts the game to only supporting at most 5 axes. In particular, the game has no axis control for roll. This mod rewires the orientation controls of the game to support this.

# Planned features

Button inputs for actions (cutter, tethers, and so on)
Configurable bindings of any controller axis to any game axis
Configure exact joystick device identifiers to support dual joystick setups
Sensitivity controls per-axis. Currently falls back to using the sensitivity of the last input device used.
