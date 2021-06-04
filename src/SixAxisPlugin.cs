using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BBI.Unity.Game;
using BepInEx;
using HarmonyLib;
using RoboPhredDev.Shipbreaker.SixAxis.Config;
using RoboPhredDev.Shipbreaker.SixAxis.Input;
using RoboPhredDev.Shipbreaker.SixAxis.Native;
using RoboPhredDev.Shipbreaker.SixAxis.Native.RID;

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

            var windowHandle = User32.GetActiveWindow();

            var mappings = LoadInputMappings();
            RegisterDevices(mappings, windowHandle);
            InputHandler.Initialize(mappings);
            WindowMessageInterceptor.Enable(windowHandle);
        }

        public void ReceivedControllerInput()
        {
            LynxControls.Instance.GameplayActions.LastInputType = InControl.BindingSourceType.MouseBindingSource;
        }

        private List<InputMapping> LoadInputMappings()
        {
            var configPath = Path.Combine(AssemblyDirectory, "device-configs");
            Logging.Log(new Dictionary<string, string> {
                {"ConfigFolder", configPath}
            }, "Loading input configuration");
            var configs = InputMapping.LoadAllMappings(configPath);
            Logging.Log($"Loaded {configs.Count} input configurations.");
            return configs;
        }

        private void RegisterDevices(List<InputMapping> mappings, IntPtr windowHandle)
        {
            var devices = RawInputDevice.GetDevices();
            var registrations = new List<DeviceRegistration>();
            foreach (var device in devices.OfType<RawInputHidDevice>())
            {
                var mapping = mappings.Find(x => x.ContainsDevice(device.VendorId, device.ProductId));
                if (mapping == null)
                {
                    continue;
                }

                Logging.Log(new Dictionary<string, string>
                {
                    {"VendorId", device.VendorId.ToString("X")},
                    {"ProductId", device.ProductId.ToString("X")},
                    {"DeviceName", device.DeviceName},
                    {"ConfigFile", mapping.FileName},
                }, $"Found input mapping for device {device.VendorId:X}:{device.ProductId:X}");

                registrations.Add(new DeviceRegistration
                {
                    dwFlags = DeviceFlags.None,
                    hwndTarget = windowHandle,
                    usUsage = device.Usage,
                    usUsagePage = device.UsagePage,
                });
            }

            RIDInterop.RegisterDevices(registrations.ToArray());
        }

        private void ApplyPatches()
        {
            var harmony = new Harmony("net.robophreddev.shipbreaker.SixAxis");
            harmony.PatchAll();
        }
    }
}