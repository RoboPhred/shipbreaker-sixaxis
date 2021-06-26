using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BBI.Unity.Game;
using BepInEx;
using HarmonyLib;
using RoboPhredDev.Shipbreaker.SixAxis.Config;
using RoboPhredDev.Shipbreaker.SixAxis.RawInput;
using RoboPhredDev.Shipbreaker.SixAxis.Native.Window;
using RoboPhredDev.Shipbreaker.SixAxis.Native.RID;
using RoboPhredDev.Shipbreaker.SixAxis.ButtonCommands;

namespace RoboPhredDev.Shipbreaker.SixAxis
{

    [BepInPlugin("net.robophreddev.shipbreaker.SixAxis", "Six Axis Joystick support for Shipbreaker", "1.1.2.0")]
    public class SixAxisPlugin : BaseUnityPlugin
    {
        public static SixAxisPlugin Instance;

        public static string AssemblyDirectory
        {
            get
            {
                var assemblyLocation = typeof(SixAxisPlugin).Assembly.Location;
                var assemblyDir = Path.GetDirectoryName(assemblyLocation);
                return assemblyDir;
            }
        }

        void Awake()
        {
            SixAxisPlugin.Instance = this;
            Logging.Initialize();

            this.ApplyPatches();

            var windowHandle = WindowInterop.GetActiveWindow();

            var mappings = LoadInputMappings();
            RegisterDevices(mappings, windowHandle);

            RawInputReceiver.Initialize();

            WindowMessageInterceptor.Enable(windowHandle);
        }

        public void ReceivedControllerInput()
        {
            LynxControls.Instance.GameplayActions.LastInputType = InControl.BindingSourceType.MouseBindingSource;
        }

        private List<DeviceConfig> LoadInputMappings()
        {
            var configPath = Path.Combine(AssemblyDirectory, "device-configs");
            Logging.Log(new Dictionary<string, string> {
                {"ConfigFolder", configPath}
            }, $"Loading input configuration from {configPath}.");
            var configs = DeviceConfig.LoadAllMappings(configPath);
            Logging.Log($"Loaded {configs.Count} input configurations.");
            return configs;
        }

        private void RegisterDevices(List<DeviceConfig> mappings, IntPtr windowHandle)
        {
            var devices = RawInputDevice.GetDevices().OfType<RawInputHidDevice>().ToArray();
            var registrationTargets = new HashSet<PageAndUsage>();

            foreach (var device in devices)
            {
                registrationTargets.Add(new PageAndUsage(device.UsagePage, device.Usage));
            }

            RawInputInterop.RegisterDevices(registrationTargets.Select(usageAndPage => new DeviceRegistration
            {
                dwFlags = DeviceFlags.None,
                hwndTarget = windowHandle,
                usUsagePage = usageAndPage.UsagePage,
                usUsage = usageAndPage.Usage
            }).ToArray());
            Logging.Log($"Registered input to listen for {registrationTargets.Count} page and usage pairs.");

            var foundMapping = false;

            // Mappings have been designed to be device agnostic, so we can give the user a list of valid mappings per device and let them choose.
            // This is why each device spec has to be configured with each mapping in InputManager.
            // For now though, we blindly apply every mapping to every spec it supports.

            foreach (var mapping in mappings)
            {
                // Testing

                // Tethers
                mapping.Buttons.Add(new ConfiguredButtonMapping()
                {
                    ButtonUsage = 5,
                    Command = new PlaceTetherCommand()
                });
                mapping.Buttons.Add(new ConfiguredButtonMapping()
                {
                    ButtonUsage = 2,
                    Command = new RecallTethersCommand()
                });

                // Grapple
                mapping.Buttons.Add(new ConfiguredButtonMapping()
                {
                    ButtonUsage = 13,
                    Command = new SelectGrappleCommand()
                });
                mapping.Buttons.Add(new ConfiguredButtonMapping()
                {
                    ButtonUsage = 3,
                    Command = new GrappleFireCommand()
                });
                mapping.Buttons.Add(new ConfiguredButtonMapping()
                {
                    ButtonUsage = 9,
                    Command = new RetractionCommand()
                });
                mapping.Buttons.Add(new ConfiguredButtonMapping()
                {
                    ButtonUsage = 27,
                    Command = new ThrowCommand()
                });

                // Cutter
                mapping.Buttons.Add(new ConfiguredButtonMapping()
                {
                    ButtonUsage = 14,
                    Command = new SelectCutterCommand()
                });
                mapping.Buttons.Add(new ConfiguredButtonMapping()
                {
                    ButtonUsage = 3,
                    Command = new CutterFireCommand()
                });
                mapping.Buttons.Add(new ConfiguredButtonMapping()
                {
                    ButtonUsage = 9,
                    Command = new CutterAltFireCommand()
                });

                // Demo
                mapping.Buttons.Add(new ConfiguredButtonMapping()
                {
                    ButtonUsage = 15,
                    Command = new SelectDemoChargeCommand()
                });
                mapping.Buttons.Add(new ConfiguredButtonMapping()
                {
                    ButtonUsage = 3,
                    Command = new DemoChargeFireCommand()
                });
                mapping.Buttons.Add(new ConfiguredButtonMapping()
                {
                    ButtonUsage = 9,
                    Command = new DemoChargeAltFireCommand()
                });

                // General
                mapping.Buttons.Add(new ConfiguredButtonMapping()
                {
                    ButtonUsage = 16,
                    Command = new ActivateScannerCommand()
                    {
                        CycleIfActive = true
                    }
                });
                mapping.Buttons.Add(new ConfiguredButtonMapping()
                {
                    ButtonUsage = 23,
                    Command = new InteractCommand()
                });
                mapping.Buttons.Add(new ConfiguredButtonMapping()
                {
                    ButtonUsage = 24,
                    Command = new ThrustBrakeCommand()
                });

                foreach (var spec in mapping.Devices)
                {
                    InputManager.RegisterInputMap(spec, mapping);
                }

                var matchingDevices = devices.Where(device => mapping.Devices.Any(spec => spec.SpecificationMatches(device)));

                foreach (var device in matchingDevices)
                {
                    foundMapping = true;
                    Logging.Log(new Dictionary<string, string>
                    {
                        {"VendorId", device.VendorId.ToString("X")},
                        {"ProductId", device.ProductId.ToString("X")},
                        {"DeviceName", device.DeviceName},
                        {"ConfigFile", mapping.FileName},
                    }, $"Found input mapping for device {device.VendorId:X}:{device.ProductId:X}");
                }
            }

            Logging.Log($"Registered {mappings.Count} input mappings");

            if (!foundMapping)
            {
                Logging.Log("WARNING: No devices matched any of the mappings available during startup.  Check that your mappings are correct and that your device is plugged in.");
            }
        }

        private void ApplyPatches()
        {
            var harmony = new Harmony("net.robophreddev.shipbreaker.SixAxis");
            harmony.PatchAll();
        }
    }
}