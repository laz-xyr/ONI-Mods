using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using HarmonyLib;
using crazyxyr.Commons;
using UnityEngine;

namespace crazyxyr.SelectLastCarePackage.Patches
{
    [HarmonyPatch(typeof(ImmigrantScreen), "OnRejectAll")]
    public static class ImmigrantScreenOnRejectAllPatch //按下拒绝全部按钮
    {
        private static float _lastTime;

        public static bool Prefix(ImmigrantScreen __instance, List<ITelepadDeliverableContainer> ___containers)
        {
            if (___containers == null || ___containers.Count == 0)
            {
                Debug.Log("[最后的补给包-Fix] 没有找到物品列表");
                return true;
            }
            if (ModUtils.HasRefreshMod())
            {
                Debug.Log("启用了刷新选人Mod，跳过");
                return true;
            }

            Debug.Log("没有启用刷新选人Mod，刷新");

            if (Time.realtimeSinceStartup - _lastTime < 0.3)
            {
                Debug.Log("-------------" + Time.realtimeSinceStartup + "-----------lastTime:" + _lastTime);
                return false;
            }

            _lastTime = Time.realtimeSinceStartup;
            ___containers.ForEach(c => UnityEngine.Object.Destroy(c.GetGameObject()));
            ___containers.Clear();
            Traverse instance = Traverse.Create(__instance);
            instance.Method("InitializeContainers").GetValue();
            ImmigrantScreenMethod.ShowButton(__instance);
             _lastTime = Time.realtimeSinceStartup;
            return false;
        }


  

    }




}