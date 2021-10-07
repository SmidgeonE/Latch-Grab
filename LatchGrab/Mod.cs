using System;
using System.Runtime.Remoting.Services;
using Deli.Setup;
using FistVR;
using HarmonyLib;
using Valve.VR;

namespace LatchGrab
{
    public class Mod : DeliBehaviour
    {
        private static ControlOptions.CoreControlMode _controlMode;
        
        private void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(Mod));
        }
        
        [HarmonyPatch(typeof(ClosedBolt), "UpdateInteraction")]
        [HarmonyPostfix]
        private static void InteactionPatch(ClosedBolt __instance)
        {
            var input = __instance.m_hand.Input;
            bool isPressingDown;
            
            if (_controlMode == ControlOptions.CoreControlMode.Standard)
                isPressingDown = input.TouchpadSouthPressed && input.TouchpadPressed;
            else
                isPressingDown = input.TriggerPressed;

            if (isPressingDown && __instance.LastPos == ClosedBolt.BoltPos.Rear && __instance.IsBoltLocked() == false)
                __instance.LockBolt();
        }
        
        
        
        /* These patches get the current control mode */
        [HarmonyPatch(typeof(GameOptions), "InitializeFromSaveFile")]
        [HarmonyPrefix]
        private static void InitialOptionsGrabPatch(GameOptions __instance)
        {
            _controlMode = __instance.ControlOptions.CCM;
        }
        
        [HarmonyPatch(typeof(GameOptions), "SaveToFile")]
        [HarmonyPrefix]
        private static void UpdateControlOptionsPatch(GameOptions __instance)
        {
            _controlMode = __instance.ControlOptions.CCM;
        }
    }
}