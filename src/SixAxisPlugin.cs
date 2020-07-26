using System.IO;
using System.Runtime.InteropServices;
using BepInEx;
using HarmonyLib;
using RoboPhredDev.Shipbreaker.SixAxis.Input;
using RoboPhredDev.Shipbreaker.SixAxis.Native;

namespace RoboPhredDev.Shipbreaker.SixAxis
{

    [BepInPlugin("net.robophreddev.shipbreaker.SixAxis", "Six Axis Joystick support for Shipbreaker", "1.0.2.0")]
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

            // TODO: Load what devices we want from a config.

            var devices = new RawInputDevice[] {
                new RawInputDevice {
                    usUsagePage = (ushort)UsagePage.GenericDesktop,
                    usUsage = (ushort)GenericDesktopUsage.MultiAxisController,
                    dwFlags = 0,
                    hwndTarget = User32.GetActiveWindow()
                }
            };
            User32.RegisterRawInputDevices(devices, (uint)devices.Length, (uint)Marshal.SizeOf(typeof(RawInputDevice)));

            InputHandler.Initialize();
            WmInputInterceptor.Initialize();
        }

        // Old experiment, should save this elsewhere
        // private void DumpTargetData()
        // {
        //     Logging.Log("For aimed object:");
        //     var ray = new Ray(LynxCameraController.MainCameraTransform.position, Camera.main.transform.forward);
        //     RaycastHit hitInfo;
        //     if (!Physics.Raycast(ray, out hitInfo, 2000f))
        //     {
        //         Logging.Log("No object hit");
        //         return;
        //     }
        //     var gameObject = hitInfo.collider.gameObject;
        //     StructurePart structurePart;
        //     if (gameObject.TryGetComponent<StructurePart>(out structurePart))
        //     {
        //         Logging.Log("StructurePart name: {0}", structurePart.name);
        //         float mass;
        //         if (structurePart.TryGetPartMass(out mass))
        //         {
        //             Logging.Log("PartMass: {0}", mass);
        //         }
        //         if (structurePart.IsPartOfGroup && structurePart.Group != null)
        //         {
        //             var group = structurePart.Group;
        //             Logging.Log("Group Name: {0}", structurePart.Group.name);
        //             Logging.Log("Group Part Name: {0}", group.StructurePartAsset.name);
        //             Logging.Log("Group Blueprint Name: {0}", group.EntityBlueprintComponent.name);
        //             var groupMass = group.GetTotalGroupMass();
        //             Logging.Log("Group Mass: {0}", groupMass);
        //         }
        //     }
        // }

        private void ApplyPatches()
        {
            var harmony = new Harmony("net.robophreddev.shipbreaker.SixAxis");
            harmony.PatchAll();
            Logging.Log("Patch succeeded");
        }
    }
}