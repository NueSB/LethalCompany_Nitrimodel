using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ModelReplacement;
using GameNetcodeStuff;

namespace NitriModel
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class NitriModelBase : BaseUnityPlugin
    {
        private const string modGUID = "nuesb.ntmodel";
        private const string modName = "Nitri";
        private const string modVersion = "1.0.0.0";

        private readonly Harmony _harmony = new Harmony(modGUID);

        public static NitriModelBase _instance;


        public static AssetBundle mainBundle;

        internal ManualLogSource log;

        // UI cursor vars
        public UnityEngine.UI.Image customCursor = null;
        public UnityEngine.UI.Image baseCursor = null;
        public Dictionary<string, Sprite> cursorMap { get; private set; }


        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }

            log = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            ModelReplacementAPI.RegisterModelReplacementOverride(typeof(NTModelReplacement));

            string text = Path.Combine(Path.GetDirectoryName(base.Info.Location), "nitrimodel");
            NitriModelBase.mainBundle = AssetBundle.LoadFromFile(text);

            cursorMap = new Dictionary<string, Sprite>
            {
                { "HandIcon",       mainBundle.LoadAsset<Sprite>("HandIcon.png") },
                { "HandIconPoint",  mainBundle.LoadAsset<Sprite>("HandIconPoint.png") },
                { "HandLadderIcon", mainBundle.LoadAsset<Sprite>("HandLadderIcon.png") },
            };

            _harmony.PatchAll();

            log.LogInfo("ntmodel: running 2! :^)");
        }
    }
}
