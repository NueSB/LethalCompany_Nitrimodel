using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.UI;
using HarmonyLib;
using UnityEngine;


namespace NitriModel
{
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
            else NitriModelBase._instance.log.LogError("Unable to find UI parent! Aborted icon replacement.");
        }
    }

}
