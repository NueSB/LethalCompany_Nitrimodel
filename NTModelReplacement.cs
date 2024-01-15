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
        }
    }
}


