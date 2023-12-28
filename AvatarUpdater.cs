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
    public class AvatarUpdater
    {
        public SkinnedMeshRenderer playerModelRenderer { get; private set; } = null;
        public SkinnedMeshRenderer replacementModelRenderer { get; private set; } = null;
        protected Animator replacementAnimator = null;
        public GameObject player { get; private set; } = null;
        public GameObject replacement { get; private set; } = null;

        public Vector3 itemHolderPositionOffset { get; private set; } = Vector3.zero;
        public Quaternion itemHolderRotationOffset { get; private set; } = Quaternion.identity;
        public Transform itemHolder { get; private set; } = null;

        protected Vector3 rootPositionOffset = new Vector3(0, 0, 0);
        protected Vector3 rootScale = new Vector3(1, 1, 1);
        protected Vector3 spinePositionOffset = new Vector3(0f, 0f, -0.0889f);

        Transform[] playerChildren = null;
        Transform[] replacementChildren = null;


        public virtual void AssignModelReplacement(GameObject player, GameObject replacement)
        {
            var controller = player.GetComponent<PlayerControllerB>();
            if (controller)
            {
                playerModelRenderer = controller.thisPlayerModelArms;
            }

            var c2 = replacement.GetComponentInChildren<SkinnedMeshRenderer>();
            if (c2)
            {
                replacementModelRenderer = c2;
            }

            if (playerModelRenderer == null || c2 == null)
            {
                Debug.LogError("failed to start AvatarBodyUpdater");
                return;
            }
            
            this.player = player;

            replacementAnimator = replacement.GetComponent<Animator>();
            this.replacement = replacement;

            rootScale = new Vector3(0.6f, 0.6f, 0.6f);
            rootPositionOffset = new Vector3(0f, 2.1f, 0f);
            spinePositionOffset = new Vector3(0f, 0f, -0.0889f);

            replacement.transform.Find("metarig/spine.003").localPosition = spinePositionOffset;
            replacement.transform.Find("metarig/spine.003/shoulder.L/arm.L_upper/arm.L_lower/hand.L").localScale = Vector3.one * 0.6f;
            replacement.transform.Find("metarig/spine.003/shoulder.R/arm.R_upper/arm.R_lower/hand.R").localScale = Vector3.one * 0.6f;

            playerChildren = playerModelRenderer.transform.parent.GetComponentsInChildren<Transform>();
            replacementChildren = replacementModelRenderer.transform.parent.GetComponentsInChildren<Transform>();

            playerModelRenderer.enabled = false;
            controller.thisPlayerModelArms = replacementModelRenderer;
            controller.playerModelArmsMetarig = replacementModelRenderer.transform.parent.Find("metarig");

            //replacementModelRenderer.sharedMesh.RecalculateBounds();
        }

        protected virtual void UpdateModel()
        {
            foreach (Transform playerBone in playerChildren)
            {
                Transform modelBone = GetAvatarTransformFromBoneName(playerBone.name);
                if (modelBone == null) { continue; }

                modelBone.rotation = playerBone.rotation;

                if (modelBone.name.Contains("metarig"))
                    modelBone.localPosition = playerBone.localPosition - new Vector3(0, 2.11f, 0.012f);
                //if (modelBone.name == "spine.003")
                //    modelBone.localPosition = playerBone.localPosition + spinePositionOffset;// - new Vector3(0, 0.0118f, -1.3636f);
                //var offset = modelBone.GetComponent<RotationOffset>();
                //if (offset) { modelBone.rotation *= offset.offset; }
            }

            /*
            string[] IKNameList = { "ArmsLeftArm_target", "ArmsRightArm_target" };
            foreach(string name in IKNameList)
            {
                Transform bone = GetPlayerTransformFromBoneName(name);
                if (!bone) Debug.Log("Player bone not found!");
                Transform replacementBone = GetAvatarTransformFromBoneName(name);
                if (!replacementBone) Debug.Log("replacement bone not found!");

                replacementBone.position = bone.position;
                replacementBone.rotation = bone.localRotation;
            }*/

            Transform rootBone = replacementAnimator.transform;
            Transform playerRootBone = playerModelRenderer.transform.parent;

            rootBone.position = playerRootBone.position + playerRootBone.TransformVector(rootPositionOffset);
            rootBone.localScale = Vector3.Scale(playerRootBone.localScale, rootScale);
        }
        public virtual void Update()
        {
            if (playerModelRenderer == null) { return; }
            if (replacementAnimator == null) { return; }
            UpdateModel();
        }

        public Transform GetAvatarTransformFromBoneName(string boneName)
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

        public Transform GetPlayerTransformFromBoneName(string boneName)
        {
            var a = playerChildren.Where(x => x.name == boneName);
            if (a.Any()) { return a.First(); }
            if (boneName == "ArmsRightArm_target")
            {
                var b = replacementChildren.Where(x => x.parent.name == "RightArm");
                if (b.Any()) { return b.First(); }
            }
            return null;
        }

        public Transform GetPlayerItemHolder()
        {
            var tr = player.GetComponentsInChildren<Transform>().Where(x => (x.name == "ServerItemHolder") || (x.name == "ItemHolder"));
            if (tr.Any()) { return tr.First(); }
            return null;
        }


    }
}
