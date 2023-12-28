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
    public class ViewmodelReplacement : MonoBehaviour
    {
        protected AvatarUpdater avatar;

        PlayerControllerB player;
        Transform baseModelTransform;

        public void Awake()
        {
            player = GetComponent<PlayerControllerB>();

            Transform baseTransform = player.thisPlayerModelArms.transform.parent;
            baseModelTransform = baseTransform;

            player.thisPlayerModelArms.enabled = false;
            //player.thisPlayerModel.enabled = false;

            Debug.Log("ApplyModel: disabled playermodel arms");

            GameObject modArms = NitriModelBase.mainBundle.LoadAsset<GameObject>("v3-viewmodel.prefab");
            modArms = GameObject.Instantiate(modArms);

            Debug.Log("ApplyModel: Setup model");

            Transform baseMetaRig = baseTransform.Find("metarig");
            Debug.Log("ApplyModel: Got base metarig: " + baseMetaRig.ToString());
            Transform metarig = modArms.transform.Find("metarig");
            Debug.Log("ApplyModel: Got new metarig - " + metarig.ToString());

            metarig.localScale = Vector3.one;
            metarig.localRotation = Quaternion.identity;
            metarig.localPosition = Vector3.zero;

            Debug.Log("ApplyModel: Applied new metarig properties");

            SkinnedMeshRenderer[] viewModelRenderers = modArms.transform.GetComponentsInChildren<SkinnedMeshRenderer>();

            Renderer baseModel = baseTransform.Find("Circle").GetComponent<Renderer>();
            Material m = baseModel.material;

            Material m2 = new Material(Shader.Find("HDRP/Lit"));

            Transform t = metarig.Find("spine.003");

            // fix materials
            foreach (Renderer r in viewModelRenderers)
            {
                Debug.Log("Set material");
                r.material = m2;
                r.gameObject.layer = baseTransform.Find("Circle").gameObject.layer;
            }
            foreach (SkinnedMeshRenderer r in viewModelRenderers)
            {
                r.rootBone = t;
            }

            /*
            var playerBodyExtents = baseModel.bounds.extents;
            float scale = playerBodyExtents.y / viewModelRenderers[0].bounds.extents.y;
            metarig.transform.localScale *= scale / 2f;
            */

            metarig.transform.localScale = Vector3.one * 11.714f;

            Debug.Log("Setting up avatar");

            avatar = new AvatarUpdater();
            avatar.AssignModelReplacement( player.gameObject, modArms );

            Debug.Log("ApplyModel: Done!");
        }

        void Update()
        {
            avatar.Update();
        }

        void OnDestroy()
        {
            PlayerControllerB controller = avatar.player.GetComponent<PlayerControllerB>();
            controller.thisPlayerModelArms = avatar.playerModelRenderer;
            controller.playerModelArmsMetarig = avatar.playerModelRenderer.transform.parent.Find("metarig"); 
            
            // could (should) probably make a container reference or something
            Destroy(avatar.replacement.transform.parent.gameObject);
        }
    }
}
