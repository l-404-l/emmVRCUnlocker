using Harmony;
using MelonLoader;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;

[assembly: MelonInfo(typeof(emmVRCUnlocker.Main), "emmVRCUnlocker", "2.5", "github.com/l-404-l")]
[assembly: MelonGame("VRChat", "VRChat")]

namespace emmVRCUnlocker
{
    public class Main : MelonMod
    {
        internal static FieldInfo RiskF;
        internal static FieldInfo RiskCF;
        private static Assembly EmmVRCLoader;
        private static Assembly EmmVRCInternal;
        public override void OnApplicationStart()
        {
            foreach (var assm in AppDomain.CurrentDomain.GetAssemblies())
            {
                switch (assm.FullName.ToLower())
                {
                    case string x when x.Contains("emmvrc, version="):
                        EmmVRCInternal = assm;
                        break;
                    case string x when x.Contains("emmvrcloader, version="):
                        EmmVRCLoader = assm;
                        break;
                }
            }

            var usedtype = EmmVRCInternal.GetType("emmVRC.Managers.RiskyFunctionsManager");
            harmonyInstance.Patch(usedtype.GetProperty("RiskyFunctionsAllowed").GetGetMethod(), new HarmonyMethod(typeof(Main), "UnblockShit"));
            harmonyInstance.Patch(usedtype.GetMethod("CheckWorld"), new HarmonyMethod(typeof(Main), "NullVoid"));
            harmonyInstance.Patch(EmmVRCInternal.GetType("emmVRC.Menus.UserTweaksMenu").GetMethod("SetRiskyFunctions"), new HarmonyMethod(typeof(Main), "UnblockShit2"));
            harmonyInstance.Patch(EmmVRCInternal.GetType("emmVRC.Menus.PlayerTweaksMenu").GetMethod("SetRiskyFunctions"), new HarmonyMethod(typeof(Main), "UnblockShit2"));
            RiskF = usedtype.GetField("riskyFuncsAllowed", AccessTools.all);
            RiskCF = usedtype.GetField("RiskyFunctionsChecked", AccessTools.all);
        }
        public static bool NullVoid(IEnumerator __result)
        {
            __result = null;
            return false;
        }
        public static bool UnblockShit2(ref bool __0)
        {
            __0 = true;
            RiskF.SetValue(null, true);
            RiskCF.SetValue(null, false);
            return true;
        }
        public static bool UnblockShit(ref bool __result)
        {
            __result = true;
            RiskF.SetValue(null, true);
            RiskCF.SetValue(null, false);
            return false;
        }
    }
}
