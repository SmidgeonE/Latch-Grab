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
            Debug.Log("Has patched the latch grab mod");
        }
        
        [HarmonyPatch(typeof(ClosedBolt), "UpdateInteraction")]
        [HarmonyPostfix]
        private static void ClosedBoltPatch(ClosedBolt __instance)
        {
            var input = __instance.m_hand.Input;
            bool isPressingDown;
            Debug.Log("Closed bolt interaction");

            if (_controlMode == ControlOptions.CoreControlMode.Standard)
            {
                isPressingDown = input.TriggerPressed;
                Debug.Log("Is pressing down!!!!!!!!");
            }
            else
                isPressingDown = input.TriggerPressed;

            if (isPressingDown && __instance.LastPos == ClosedBolt.BoltPos.Rear && __instance.IsBoltLocked() == false)
            {
                Debug.Log("Closing bolt");
                __instance.LockBolt();
            }
        }
        
        [HarmonyPatch(typeof(HandgunSlide), "UpdateInteraction")]
        [HarmonyPostfix]
        private static void PistolPatch(HandgunSlide __instance)
        {
            var handgun = __instance.Handgun;
            
            if (!handgun.HasSlideRelease)
            {
                Debug.Log("This handgun does not have a slide");
                return;
            }
            
            var input = __instance.m_hand.Input;
            bool isPressingDown;
            
            if (_controlMode == ControlOptions.CoreControlMode.Standard)
                isPressingDown = input.TriggerPressed;
            else
                isPressingDown = input.TriggerPressed;

            if (isPressingDown && !handgun.IsSlideCatchEngaged() && __instance.LastPos == HandgunSlide.SlidePos.Rear)
            {
                handgun.EngageSlideRelease();
                Debug.Log("Engaging slide relase");
            }
            else
            {

                Debug.Log(isPressingDown);
                Debug.Log(!handgun.IsSlideCatchEngaged());
                Debug.Log(__instance.LastPos.ToString());
            }
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