using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ModelReplacement;
using UnityEngine;
using UnityEngine.UI;

using HarmonyLib;
using GameNetcodeStuff;
using System.Reflection;
using ModelReplacement.AvatarBodyUpdater;

namespace NitriModel
{
    class NTModelReplacement : BodyReplacementBase
    {
        protected override GameObject LoadAssetsAndReturnModel()
        {
            return NitriModelBase.mainBundle.LoadAsset<GameObject>("v3-playermodel.prefab");
        }

        protected override GameObject LoadAssetsAndReturnViewModel()
        {
            return NitriModelBase.mainBundle.LoadAsset<GameObject>("v3-viewmodel.prefab");
        }

        protected override AvatarUpdater GetAvatarUpdater()
        {
            return new AvatarUpdaterWithArms();
        }


        protected override void OnDeath()
        {
            base.OnDeath();
            /*
            Debug.Log("Destroying...........\n\n\n\n\n\n");

            Debug.Log("our controller: " + this.controller.gameObject.name);
            // TODO: fix the recreating arms issue.
            ViewmodelReplacement customViewmodel = this.controller.gameObject.GetComponent<ViewmodelReplacement>();
            Destroy(customViewmodel);
            Debug.Log("Finsihed?");
            */
        }
    }

    /*
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class ViewmodelPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        static void ApplyModel(PlayerControllerB __instance)
        {
            bool isPlayer = (__instance.IsOwner && __instance.isPlayerControlled && (!__instance.IsServer || __instance.isHostPlayerObject)) || __instance.isTestingPlayer;
            bool isCameraDisabled = (bool)Traverse.Create(__instance).Field("isCameraDisabled").GetValue();
            isPlayer = isPlayer && isCameraDisabled;
            if (!isPlayer)
            {
                //Debug.Log("ApplyModel: Skipping playermodel replacement, condition not met");
                return;
            }

            Debug.Log("\n\n\nApplying nitri player viewmodel.");

            // hierarchy:
            // ScavengerModelArmsOnly
            // |- metarig
            // |- Circle   ( viewmodel model )

            Transform baseTransform = __instance.thisPlayerModelArms.transform.parent;
            Debug.Log("base transform name: " + baseTransform.name);
            if (baseTransform != null)
            {
                __instance.gameObject.AddComponent<ViewmodelReplacement>();
            }
        }
    }
    */

    [HarmonyPatch(typeof(StartOfRound))]
    internal class UIIconPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void ApplyUI()
        {
            GameObject UIParent = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/TopLeftCorner");
            if (UIParent != null)
            {
                Transform selfIcon = UIParent.transform.Find("Self");
                Transform selfFilled = UIParent.transform.Find("SelfRed");

                selfIcon.GetComponent<Image>().sprite = NitriModelBase.mainBundle.LoadAsset<Sprite>("tposeui.png");
                selfFilled.GetComponent<Image>().sprite = NitriModelBase.mainBundle.LoadAsset<Sprite>("tposeui_filled.png");

                Vector3 offset = new Vector3(-322.5487f, 140.4f, -14.2174f) - new Vector3(-324.5172f, 140.262f, -14.3174f);
                Vector3 scale = new Vector3(-1.3028f, 1.3028f, 1.3028f) - new Vector3(-0.8428f, 0.9737f, 1.3028f);

                selfIcon.localPosition -= offset;
                selfFilled.localPosition -= offset;
                selfIcon.localScale -= scale;
                selfFilled.localScale -= scale;
            }
            else Debug.LogError("Unable to find UI parent!");
        }

    }
}


