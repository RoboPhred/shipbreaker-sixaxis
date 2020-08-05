using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using HarmonyLib;
using Linearstar.Windows.RawInput;
using RoboPhredDev.Shipbreaker.SixAxis.Config;
using RoboPhredDev.Shipbreaker.SixAxis.Native;

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

            this.ApplyPatches();

            var windowHandle = User32.GetActiveWindow();

            var mappings = LoadInputMappings();
            RegisterDevices(mappings, windowHandle);
            InputHandler.Initialize(mappings);
            WindowMessageInterceptor.Enable(windowHandle);
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
            var usageAndPages = new HashSet<HidUsageAndPage>();
            foreach (var device in devices)
            {
                var mapping = mappings.FirstOrDefault(x => x.ContainsDevice(device.VendorId, device.ProductId));
                if (mapping == null)
                {
                    continue;
                }

                Logging.Log(new Dictionary<string, string>
                {
                    {"VendorId", device.VendorId.ToString("X")},
                    {"ProductId", device.ProductId.ToString("X")},
                    {"ConfigFile", mapping.FileName},
                }, $"Found input mapping for device {device.VendorId.ToString("X")}:{device.ProductId.ToString("X")}");

                usageAndPages.Add(device.UsageAndPage);
            }

            foreach (var usage in usageAndPages)
            {
                RawInputDevice.RegisterDevice(usage, RawInputDeviceFlags.None, windowHandle);
            }
        }

        private void ApplyPatches()
        {
            var harmony = new Harmony("net.robophreddev.shipbreaker.SixAxis");
            harmony.PatchAll();
            Logging.Log("Patch succeeded");
        }
    }
}