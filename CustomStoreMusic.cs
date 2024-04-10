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


namespace NitriModel
{
    [HarmonyPatch(typeof(ItemDropship))]
    internal class CustomStoreMusic
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        static void ApplyMusic(ItemDropship __instance)
        {
            
            AudioSource music = __instance.transform.Find("Music").GetComponent<AudioSource>();
            AudioSource musicFar = __instance.transform.Find("Music").GetChild(0).GetComponent<AudioSource>();

            AudioClip repl = NitriModelBase.mainBundle.LoadAsset<AudioClip>("Music.wav");
            AudioClip replFar = NitriModelBase.mainBundle.LoadAsset<AudioClip>("MusicFar.wav");

            music.clip = repl;
            musicFar.clip = replFar;
        }
    }
}
