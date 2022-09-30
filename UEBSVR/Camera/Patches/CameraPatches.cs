using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using Valve.VR;

namespace UEBSVR
{


    [HarmonyPatch]
    public class CameraPatches
    {

        public static GameObject DummyCamera, VRCamera;

        private static readonly string[] canvasesToIgnore =
{
        "com.sinai.unityexplorer_Root", // UnityExplorer.
        "com.sinai.unityexplorer.MouseInspector_Root", // UnityExplorer.
        "IntroCanvas"
    };
 

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UEBSTwoLighting), "OnEnable")]
        private static void fixlighting(UEBSTwoLighting __instance)
        {
            if (__instance.name == "MENUCAM1n3") {
                __instance.enabled = false;
            }

            if(__instance.name == "Camera (1)")
            {
                __instance.enabled = false;
                __instance.gameObject.GetComponent<AutoCinematic>().enabled = false;
                __instance.gameObject.GetComponent<Camera>().enabled = false;

                VRCamera = new GameObject("VRCamera");
                DummyCamera = new GameObject("DummyCamera");
                VRCamera.tag = "MainCamera";
                VRCamera.AddComponent<Camera>();
                VRCamera.GetComponent<Camera>().nearClipPlane = .01f;
                VRCamera.GetComponent<Camera>().farClipPlane = 50000f;
                // VRCamera.AddComponent<UEBSTwoLighting>();
                VRCamera.AddComponent<FlareLayer>();
                VRCamera.AddComponent<HorizonBasedAmbientOcclusion.HBAO>();
                     
                DummyCamera.transform.position = __instance.transform.position;
                DummyCamera.transform.rotation = __instance.transform.rotation;
                DummyCamera.transform.parent = __instance.gameObject.transform;

                VRCamera.transform.parent = DummyCamera.transform;
                DummyCamera.transform.localPosition += new Vector3(0, -.8f, 0);
            }

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CanvasScaler), "OnEnable")]
        private static void MoveIntroCanvas(CanvasScaler __instance)
        {
            if (IsCanvasToIgnore(__instance.name)) return;

            if (__instance.transform.Find("BATTLEMENUUI")) return;

            Logs.WriteInfo($"Hiding Canvas:  {__instance.name}");
            var canvas = __instance.GetComponent<Canvas>();

            //canvas.transform.parent = VRCamera.transform;

            // Canvases with graphic raycasters are the ones that receive click events.
            // Those need to be handled differently, with colliders for the laser ray.
            // if (canvas.GetComponent<GraphicRaycaster>())
            //    AttachedUi.Create<InteractiveUi>(canvas, 0.002f);

            /* canvas.renderMode = RenderMode.WorldSpace;
             canvas.worldCamera.nearClipPlane = .01f;
             canvas.worldCamera.farClipPlane = 50000f;
             canvas.transform.position = new Vector3(44.5747f, 40f, 828f);
             canvas.transform.localPosition = new Vector3(44.5747f, 40f, 828f);
             */
            
            AttachedUi.Create<StaticUi>(canvas, 0.00145f);
        }

        [HarmonyPostfix]
            [HarmonyPatch(typeof(PlayerControl), "Update")]
            private static void MoveCamera(playercontroller __instance)
            {
                CameraManager.HandleDemeoCamera();
                float newx, newz;

                float speed = 4f * Time.deltaTime;
                if (Input.GetKey("1") || Input.GetKey("2"))
                    speed = 40f * Time.deltaTime;

                if (Input.GetKey("a"))
                {
                    newx = (VRCamera.transform.localRotation * Vector3.right * -speed).x;
                    newz = (VRCamera.transform.localRotation * Vector3.right * -speed).z;
                DummyCamera.transform.parent.Translate(newx, 0, newz);
                }

                if (Input.GetKey("d"))
                {
                    newx = (VRCamera.transform.localRotation * Vector3.right * speed).x;
                    newz = (VRCamera.transform.localRotation * Vector3.right * speed).z;
                DummyCamera.transform.parent.Translate(newx, 0, newz);
                }
                if (Input.GetKey("w"))
                {
                    newx = (VRCamera.transform.localRotation * Vector3.forward * speed).x;
                    newz = (VRCamera.transform.localRotation * Vector3.forward * speed).z;
                DummyCamera.transform.parent.Translate(newx, 0, newz);
                }

                if (Input.GetKey("s"))
                {
                    newx = (VRCamera.transform.localRotation * Vector3.forward * -speed).x;
                    newz = (VRCamera.transform.localRotation * Vector3.forward * -speed).z;
                DummyCamera.transform.parent.Translate(newx, 0, newz);
                }

                if (Input.GetKey("r"))
                    DummyCamera.transform.parent.Translate(0f, speed * .7f, 0f);
                if (Input.GetKey("f"))
                    DummyCamera.transform.parent.Translate(0f, -speed * .7f, 0f);
                if (Input.GetKey("g"))
                    DummyCamera.transform.parent.RotateAround(VRCamera.transform.position, Vector3.up, -30f * Time.deltaTime);
                if (Input.GetKey("h"))
                    DummyCamera.transform.parent.RotateAround(VRCamera.transform.position, Vector3.up, 30f * Time.deltaTime);

            }

        private static bool IsCanvasToIgnore(string canvasName)
        {
            foreach (var s in canvasesToIgnore)
                if (Equals(s, canvasName))
                    return true;
            return false;
        }

    }
}

