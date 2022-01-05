using System;
using System.Runtime.Remoting.Services;
using Deli.Setup;
using FistVR;
using HarmonyLib;
using UnityEngine;
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
        private static void ClosedBoltPatch(ClosedBolt __instance)
        {
            if (!__instance.IsHeld) return;
            var input = __instance.m_hand.Input;
            bool isPressingDown;

            if (_controlMode == ControlOptions.CoreControlMode.Standard)
                isPressingDown = input.TriggerPressed;
            else
                isPressingDown = input.TriggerPressed;

            if (isPressingDown && __instance.LastPos >= ClosedBolt.BoltPos.Locked && !__instance.IsBoltLocked())
                __instance.LockBolt();
        }
        
        [HarmonyPatch(typeof(HandgunSlide), "UpdateSlide")]
        [HarmonyPostfix]
        private static void PistolPatch(HandgunSlide __instance)
        {
            if (!__instance.IsHeld) return;

            var handgun = __instance.Handgun;
            
            if (!handgun.HasSlideRelease)
                return;

            var input = __instance.m_hand.Input;
            
            bool isPressingDown;
            
            if (_controlMode == ControlOptions.CoreControlMode.Standard)
                isPressingDown = input.TriggerPressed;
            else
                isPressingDown = input.TriggerPressed;

            if (isPressingDown && !handgun.IsSlideCatchEngaged() && __instance.LastPos >= HandgunSlide.SlidePos.Locked)
                handgun.EngageSlideRelease();
        }
        
        
        
        /* These patches get the current control mode */
        [HarmonyPatch(typeof(GameOptions), "InitializeFromSaveFile")]
        [HarmonyPostfix]
        private static void InitialOptionsGrabPatch(GameOptions __instance)
        {
            _controlMode = __instance.ControlOptions.CCM;
            Debug.Log("mode set to : " + _controlMode);
        }
        
        [HarmonyPatch(typeof(GameOptions), "SaveToFile")]
        [HarmonyPostfix]
        private static void UpdateControlOptionsPatch(GameOptions __instance)
        {
            _controlMode = __instance.ControlOptions.CCM;
        }
    }
}