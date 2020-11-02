using Harmony;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

[assembly: MelonInfo(typeof(emmVRCUnlocker.Main), "EmmVRC_Unlocker", "1.0", "github.com/l-404-l")]
[assembly: MelonGame("VRChat", "VRChat")]

namespace emmVRCUnlocker
{
    public class Main : MelonMod
    {
        internal static FieldInfo RiskF = AccessTools.Field(typeof(emmVRC.Managers.RiskyFunctionsManager), "riskyFuncsAllowed");
        internal static FieldInfo RiskCF = AccessTools.Field(typeof(emmVRC.Managers.RiskyFunctionsManager), "RiskyFunctionsChecked");
        public override void OnApplicationStart()
        {
            var value = Harmony.HarmonyInstance.Create("EmmVRC_FuckYourLimits" + Guid.NewGuid().ToString());
            value.Patch(typeof(emmVRC.Managers.RiskyFunctionsManager).GetProperty("RiskyFunctionsAllowed").GetGetMethod(), new HarmonyMethod(typeof(Main), "CheckFixed"));
        }
        public static bool CheckFixed(bool __result)
        {
            __result = true;
            RiskF.SetValue(null, true);
            RiskCF.SetValue(null, true);
            return false;
        }
    }
}
