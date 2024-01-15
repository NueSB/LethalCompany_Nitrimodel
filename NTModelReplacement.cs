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
            GameObject obj = NitriModelBase.mainBundle.LoadAsset<GameObject>("v3-playermodel.prefab");
            Material replacementMat = null;

            switch (StartOfRound.Instance.unlockablesList.unlockables[controller.currentSuitID].unlockableName)
            {
                case "Orange suit":
                    replacementMat = NitriModelBase.mainBundle.LoadAsset<Material>("NTMaterial");
                    break;
                case "Green suit":
                    replacementMat = NitriModelBase.mainBundle.LoadAsset<Material>("NTMaterialGreen");
                    break;
                case "Purple Suit":
                    replacementMat = NitriModelBase.mainBundle.LoadAsset<Material>("NTMaterialPurple");
                    break;
            }

            SkinnedMeshRenderer[] meshes = obj.GetComponentsInChildren<SkinnedMeshRenderer>();

            Debug.Log("Looking for meshes...");
            foreach (SkinnedMeshRenderer mesh in meshes)
            {
                Debug.Log("Found a mesh: " + mesh.name);
                mesh.SetMaterials(new List<Material> { replacementMat });
            }

            return obj;
        }
        protected override GameObject LoadAssetsAndReturnViewModel()
        {
            GameObject obj = NitriModelBase.mainBundle.LoadAsset<GameObject>("v3-viewmodel.prefab");
            Material replacementMat = null;

            switch (StartOfRound.Instance.unlockablesList.unlockables[controller.currentSuitID].unlockableName)
            {
                case "Orange suit":
                    replacementMat = NitriModelBase.mainBundle.LoadAsset<Material>("nt-viewmodel");
                    break;
                case "Green suit":
                    replacementMat = NitriModelBase.mainBundle.LoadAsset<Material>("nt-viewmodel-green");
                    break;
                case "Purple Suit":
                    replacementMat = NitriModelBase.mainBundle.LoadAsset<Material>("nt-viewmodel-purple");
                    break;
            }

            SkinnedMeshRenderer[] meshes = obj.GetComponentsInChildren<SkinnedMeshRenderer>();

            Debug.Log("Looking for meshes...");
            foreach (SkinnedMeshRenderer mesh in meshes)
            {
                Debug.Log("Found a mesh: " + mesh.name);
                mesh.SetMaterials(new List<Material> { replacementMat });
            }

            return obj;
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


