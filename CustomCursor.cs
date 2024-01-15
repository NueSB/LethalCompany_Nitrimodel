using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine.UI;
using HarmonyLib;
using UnityEngine;
using GameNetcodeStuff;

namespace NitriModel
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class CursorPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void ApplyCursor()
        {
            Image customCursor;
            Image baseCursor;

            GameObject obj;

            if (NitriModelBase._instance.baseCursor == null)
            {
                obj = GameObject.Find("Systems/UI/Canvas/PlayerCursor/Cursor");
                if (!obj)
                    return;

                NitriModelBase._instance.baseCursor = obj.GetComponent<Image>();
            }

            if (NitriModelBase._instance.customCursor == null)
            {
                NitriModelBase._instance.customCursor = GameObject.Instantiate(NitriModelBase._instance.baseCursor.gameObject).GetComponent<Image>();

                customCursor = NitriModelBase._instance.customCursor;
                customCursor.transform.SetParent(NitriModelBase._instance.baseCursor.transform.parent);
                customCursor.transform.localScale = NitriModelBase._instance.baseCursor.transform.localScale;
                customCursor.transform.localScale = new Vector3(customCursor.transform.localScale.x * 0.83651226158038147138964577656678f,
                                                                customCursor.transform.localScale.y,
                                                                customCursor.transform.localScale.z);

                NitriModelBase._instance.customCursor.gameObject.name = "NT UI Cursor";
            }

            customCursor = NitriModelBase._instance.customCursor;
            baseCursor = NitriModelBase._instance.baseCursor;
            Dictionary<string, Sprite> cursorMap = NitriModelBase._instance.cursorMap;

            customCursor.enabled = baseCursor.enabled;

            if (baseCursor.enabled)
            {
                customCursor.transform.localPosition = baseCursor.transform.localPosition;
                customCursor.transform.localRotation = baseCursor.transform.localRotation;
                baseCursor.transform.localScale = Vector3.zero;

                Sprite cursorCnd;
                if (cursorMap.TryGetValue(baseCursor.sprite.name, out cursorCnd))
                {
                    customCursor.sprite = cursorCnd;
                }
                else
                {
                    customCursor.sprite = baseCursor.sprite;
                }
            }

        }
    }

}
