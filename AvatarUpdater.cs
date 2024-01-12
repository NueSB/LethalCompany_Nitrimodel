using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

using HarmonyLib;
using GameNetcodeStuff;
using System.Reflection;
using ModelReplacement.AvatarBodyUpdater;


namespace NitriModel
{
    public class AvatarUpdaterWithArms : AvatarUpdater
    {

        Transform[] playerChildren = null;
        Transform[] replacementChildren = null;

        SkinnedMeshRenderer replacementViewModelRenderer = null;
        SkinnedMeshRenderer playerViewModelRenderer = null;
        Animator replacementViewModelAnimator;
        Vector3 viewModelRootScale = Vector3.one;

        public override void AssignViewModelReplacement(GameObject player, GameObject replacementViewmodel)
        {
            var controller = player.GetComponent<PlayerControllerB>();
            if (controller)
            {
                playerViewModelRenderer = controller.thisPlayerModelArms;
            }

            var c2 = replacementViewmodel.GetComponentInChildren<SkinnedMeshRenderer>();
            if (c2)
            {
                replacementViewModelRenderer = c2;
            }

            if (playerViewModelRenderer == null || c2 == null)
            {
                Debug.LogError("failed to start AvatarBodyUpdater");
                return;
            }

            Debug.Log("Got base and replacement viewmodel");

            replacementViewModelAnimator = replacement.GetComponent<Animator>();

            rootScale = new Vector3(0.6f, 0.6f, 0.6f);
            rootPositionOffset = new Vector3(0f, 2.1f, 0f);
            spinePositionOffset = new Vector3(0f, 0f, -0.0889f);

            replacementViewmodel.transform.Find("metarig/spine.003").localPosition = spinePositionOffset;
            replacementViewmodel.transform.Find("metarig/spine.003/shoulder.L/arm.L_upper/arm.L_lower/hand.L").localScale = Vector3.one * 0.6f;
            replacementViewmodel.transform.Find("metarig/spine.003/shoulder.R/arm.R_upper/arm.R_lower/hand.R").localScale = Vector3.one * 0.6f;

            viewModelRootScale = rootScale;

            //replacementViewModelRenderer.material = new Material(Shader.Find("HDRP/Lit"));

            playerChildren = playerViewModelRenderer.transform.parent.GetComponentsInChildren<Transform>();
            replacementChildren = replacementViewModelRenderer.transform.parent.GetComponentsInChildren<Transform>();


            //controller.thisPlayerModelArms = replacementViewModelRenderer;
            //controller.playerModelArmsMetarig = replacementViewModelRenderer.transform.parent.Find("metarig");

            this.replacementViewModel = replacementViewmodel;

            Transform metarig = replacementViewmodel.transform.Find("metarig");
            metarig.localScale = Vector3.one * 10f;
            metarig.localRotation = Quaternion.identity;
            metarig.localPosition = Vector3.zero;
        }

        protected override void UpdateViewModel()
        {
            foreach (Transform playerBone in playerChildren)
            {
                Transform modelBone = GetViewModelTransformFromBoneName(playerBone.name);
                if (modelBone == null) { continue; }


                modelBone.rotation = playerBone.rotation;
                if (!modelBone.name.Contains("finger"))
                    modelBone.position = playerBone.position;

                //if (modelBone.name.Contains("metarig"))
                //    modelBone.localPosition = playerBone.localPosition - new Vector3(0, 2.11f, 0.012f);
                if (modelBone.name == "spine.003")
                    modelBone.localPosition = playerBone.localPosition + spinePositionOffset;// - new Vector3(0, 0.0118f, -1.3636f);
                //var offset = modelBone.GetComponent<RotationOffset>();
                //if (offset) { modelBone.rotation *= offset.offset; }
            }


            // string[] IKNameList = { "ArmsLeftArm_target", "ArmsRightArm_target" };
            // foreach(string name in IKNameList)
            // {
            //     Transform bone = GetPlayerTransformFromBoneName(name);
            //     if (!bone) Debug.Log("Player bone not found!");
            //     Transform replacementBone = GetAvatarTransformFromBoneName(name);
            //     if (!replacementBone) Debug.Log("replacement bone not found!");

            //     replacementBone.position = bone.position;
            //     replacementBone.rotation = bone.localRotation;
            // }

            Transform rootBone = replacementViewModelAnimator.transform;
            Transform playerRootBone = playerViewModelRenderer.transform.parent;

            //rootBone.position = playerRootBone.position + playerRootBone.TransformVector(rootPositionOffset);
            rootBone.localScale = Vector3.Scale(playerRootBone.localScale, viewModelRootScale);

            replacementViewModelRenderer.gameObject.layer = 30;
        }

        public Transform GetViewModelTransformFromBoneName(string boneName)
        {
            var a = replacementChildren.Where(x => x.name == boneName);
            if (a.Any()) { return a.First(); }
            if (boneName == "ArmsRightArm_target")
            {
                var b = replacementChildren.Where(x => x.parent.name == "RightArm");
                if (b.Any()) { return b.First(); }
            }
            return null;
        }
    }

}