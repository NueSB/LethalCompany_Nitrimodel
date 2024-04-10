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



// todo:
// go into unity and put an object on the hand to function as an 'item holder' so you have the right offsets set up
// stretch goal: ik control? again?

namespace NitriModel
{
    public class NTViewModelUpdater : ModelReplacement.Scripts.Player.ViewModelUpdater
    {

        Transform[] playerChildren = null;
        Transform[] replacementChildren = null;

        SkinnedMeshRenderer replacementViewModelRenderer = null;
        SkinnedMeshRenderer playerViewModelRenderer = null;
        Animator replacementViewModelAnimator;
        Vector3 viewModelRootScale = Vector3.one;

        PlayerControllerB controller;


        Vector3 rootScale;
        Vector3 rootPositionOffset;
        Vector3 humanCameraPosition;
        Transform rightHand;

        public override void AssignViewModelReplacement(GameObject player, GameObject replacementViewmodel)
        {
            var controller = player.GetComponent<PlayerControllerB>();
            if (controller)
            {
                playerViewModelRenderer = controller.thisPlayerModelArms;
                this.controller = controller;
            }

            var c2 = replacementViewmodel.GetComponentInChildren<SkinnedMeshRenderer>();
            if (c2)
            {
                replacementViewModelRenderer = c2;
            }

            if (playerViewModelRenderer == null || c2 == null)
            {
                NitriModelBase._instance.log.LogError("Failed to start AvatarBodyUpdater.");
                return;
            }

            NitriModelBase._instance.log.LogInfo("Got base and replacement viewmodel.");

            replacementViewModelAnimator = replacementViewmodel.GetComponent<Animator>();

            rootPositionOffset = new Vector3(0f, 0.0f, 0f);
            spinePositionOffset = new Vector3(0f, 0f, 0f);

            viewModelRootScale = Vector3.one;

            playerChildren = playerViewModelRenderer.transform.parent.GetComponentsInChildren<Transform>();
            replacementChildren = replacementViewModelRenderer.transform.parent.GetComponentsInChildren<Transform>();

            this.replacementViewModel = replacementViewmodel;

            Transform metarig = replacementViewmodel.transform.Find("metarig");
            metarig.localScale = Vector3.one * 10f;
            metarig.localRotation = Quaternion.identity;
            metarig.localPosition = Vector3.zero;

            humanCameraPosition = controller.gameplayCamera.transform.localPosition;

            rightHand = replacementViewmodel.transform.Find("metarig/spine.003/shoulder.R/arm.R_upper/arm.R_lower/hand.R");

            ItemHolderViewModel = rightHand.Find("ItemHolder").GetChild(0);
            replacementViewmodel.SetActive(true);
        }

        protected override void UpdateViewModel()
        {

            Vector3 cameraPositionGoal = this.humanCameraPosition;
            Vector3 viewmodelOffset = Vector3.zero;
            
            
            if (!this.controller.inTerminalMenu && !this.controller.inSpecialInteractAnimation)
            {
                if (!this.controller.isCrouching)
                {
                    cameraPositionGoal = new Vector3(0, -0.4f, 0.0f);
                }
                else
                {
                    cameraPositionGoal = new Vector3(0, -0.1f, 0.0f);
                    
                }
                viewmodelOffset = cameraPositionGoal;
            }

            // Debug.Log(viewmodelOffset);
            
            this.controller.gameplayCamera.transform.localPosition = Vector3.MoveTowards(this.controller.gameplayCamera.transform.localPosition, cameraPositionGoal, Time.deltaTime * 2);
            


            foreach (Transform playerBone in playerChildren)
            {
                Transform modelBone = NTGetViewModelTransformFromBoneName(playerBone.name);
                if (modelBone == null) { continue; }


                modelBone.rotation = playerBone.rotation;
                
                if (!modelBone.name.Contains("finger"))
                {
                    modelBone.position = playerBone.position + viewmodelOffset;
                }

                if (modelBone.name == "spine.003")
                    modelBone.localPosition = playerBone.localPosition + spinePositionOffset + viewmodelOffset;// - new Vector3(0, 0.0118f, -1.3636f);
                //var offset = modelBone.GetComponent<RotationOffset>();
                //if (offset) { modelBone.rotation *= offset.offset; }

            }


            /*
            object grabValidatedRaw = Traverse.Create(controller).Field("grabbedObjectValidated").GetValue();
            
            bool grabbedObjectValidated = false;

            if (grabValidatedRaw is bool value)
            {
                grabbedObjectValidated = value;
                if (controller.currentlyHeldObjectServer != null && controller.isHoldingObject && value)
                {
                    NitriModelBase._instance.DeferItemUpdate(rightHand, controller.currentlyHeldObjectServer.transform);
                }
            }
            */

            /*
            string[] IKNameList = { "ArmsLeftArm_target", "ArmsRightArm_target" };
            foreach (string name in IKNameList)
            {
                Transform bone = GetPlayerTransform(name);
                if (!bone) Debug.Log("Player bone not found!");
                Transform replacementBone = NTGetViewModelTransformFromBoneName(name);
                if (!replacementBone) Debug.Log("replacement bone not found!");

                replacementBone.position = bone.position;
                replacementBone.rotation = bone.rotation;
            }
            */

            Transform rootBone = replacementViewModelAnimator.transform;
            Transform playerRootBone = playerViewModelRenderer.transform.parent;

            //rootBone.position = playerRootBone.position + playerRootBone.TransformVector(rootPositionOffset);
            rootBone.localScale = Vector3.Scale(playerRootBone.localScale, viewModelRootScale);

            //replacementViewModelRenderer.gameObject.layer = 30;
        }

        public Transform GetPlayerTransform(string name)
        {
            var a = playerChildren.Where(x => x.name == name);
            if (a.Any()) { return a.First(); }
            if (name == "ArmsRightArm_target")
            {
                var b = replacementChildren.Where(x => x.parent.name == "RightArm");
                if (b.Any()) { return b.First(); }
            }
            return null;
        }

        public Transform NTGetViewModelTransformFromBoneName(string boneName)
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