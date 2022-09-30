using BepInEx;
using HarmonyLib;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Valve.VR;

namespace UEBSVR
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.NoSmoke.VRMods.UEBSVR";
        public const string PLUGIN_NAME = "UEBSVR";
        public const string PLUGIN_VERSION = "0.0.1";

        public static string gameExePath = Process.GetCurrentProcess().MainModule.FileName;
        public static string gamePath = Path.GetDirectoryName(gameExePath);
       
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            //new AssetLoader();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            Logs.WriteInfo($"LLLL: Calling InitSteamVR");
            InitSteamVR();
        }
        private static void InitSteamVR()
        {
            Logs.WriteInfo($"LLLL: Calling SteamVR_Actions.PreInitialize");
            SteamVR_Actions.PreInitialize();
            Logs.WriteInfo($"LLLL: Calling SteamVR.Initialize");
            SteamVR.Initialize();

            SteamVR_Settings.instance.pauseGameWhenDashboardVisible = true;


            // INPUT TEST
            SteamVR_Actions._default.LeftTrigger.AddOnStateDownListener(TriggerLeftDown, SteamVR_Input_Sources.Any);
            SteamVR_Actions._default.RightGrab.AddOnStateDownListener(GrabRightDown, SteamVR_Input_Sources.Any);
            SteamVR_Actions._default.RightGrab.AddOnStateUpListener(GrabRightUp, SteamVR_Input_Sources.Any);
            SteamVR_Actions._default.LeftGrab.AddOnStateDownListener(GrabLeftDown, SteamVR_Input_Sources.Any);
            SteamVR_Actions._default.LeftGrab.AddOnStateUpListener(GrabLeftUp, SteamVR_Input_Sources.Any);
           
            SteamVR_Actions._default.RightHandPose.AddOnUpdateListener(SteamVR_Input_Sources.Any, UpdateRightHand);
            SteamVR_Actions._default.LeftHandPose.AddOnUpdateListener(SteamVR_Input_Sources.Any, UpdateLeftHand);


            SteamVR_Actions._default.SwitchPOV.AddOnStateDownListener(OnSwitchPOVDown, SteamVR_Input_Sources.Any);
            SteamVR_Actions._default.SwitchPOV.AddOnStateUpListener(OnSwitchPOVUp, SteamVR_Input_Sources.Any);
        }

        // INPUT TEST

        public static void OnSwitchPOVDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            //  CameraManager.SwitchPOV();
            Logs.WriteInfo($"LLLL: Calling CameraManager.SpawnHands");
            CameraManager.SpawnHands();
        }
        public static void OnSwitchPOVUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Logs.WriteInfo("SwitchPOVButton is UP");
        }

        public static void TriggerLeftDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            float speed = 4f * Time.deltaTime;
            Logs.WriteInfo("TriggerLeft is Down");
            CameraPatches.VRCamera.transform.parent.Translate(CameraPatches.VRCamera.transform.localRotation * Vector3.forward * speed);

        }

        public static void GrabRightDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            CameraManager.RightHandGrab = true;
            Logs.WriteInfo($"LLLL: Calling CameraManager.SpawnHands");
            CameraManager.SpawnHands();
        }

        public static void GrabRightUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            CameraManager.RightHandGrab = false;
        }

        public static void GrabLeftDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            CameraManager.LeftHandGrab = true;

        }

        public static void GrabLeftUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            CameraManager.LeftHandGrab = false;
        }

        public static void UpdateRightHand(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource)
        {
            if (CameraManager.RightHand)
            {
                CameraManager.RightHand.transform.localPosition = SteamVR_Actions._default.RightHandPose.localPosition;
            }

        }

        public static void UpdateLeftHand(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource)
        {
            if (CameraManager.LeftHand)
            {
                CameraManager.LeftHand.transform.localPosition = SteamVR_Actions._default.LeftHandPose.localPosition;
            }
        }
    }
}
