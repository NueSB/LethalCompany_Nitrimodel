using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.UI;
using HarmonyLib;
using UnityEngine;
using GameNetcodeStuff;

// Late updating system to deal with items lagging behind the hand by a frame.
// this also may not work.

namespace NitriModel
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class LateTransformer
    {
        [HarmonyPatch("LateUpdate")]
        [HarmonyPostfix]
        static void UpdateItemsLately()
        {
            Transform target = NitriModelBase._instance.heldItemTarget;
            Transform item = NitriModelBase._instance.heldItem;

            if (target != null && item != null)
            {
                Debug.Log("late-holding object: <>");
                item.position = target.position;
                NitriModelBase._instance.heldItemTarget = null;
                NitriModelBase._instance.heldItem = null;
            }
        }
    }

}
