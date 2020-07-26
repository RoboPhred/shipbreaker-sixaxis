using System.IO;
using BepInEx;
using HarmonyLib;

namespace RoboPhredDev.Shipbreaker.SixAxis
{

    [BepInPlugin("net.robophreddev.shipbreaker.SixAxis", "Six Axis Joystick support for Shipbreaker", "1.0.1.0")]
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