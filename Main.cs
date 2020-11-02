using Harmony;
using MelonLoader;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;

[assembly: MelonInfo(typeof(emmVRCUnlocker.Main), "emmVRCUnlocker", "2.0", "github.com/l-404-l")]
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
                switch(assm.FullName.ToLower())
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
            harmonyInstance.Patch(EmmVRCInternal.GetType("emmVRC.Managers.RiskyFunctionsManager").GetProperty("RiskyFunctionsAllowed").GetGetMethod(), new HarmonyMethod(typeof(Main), "UnblockShit"));
            RiskF = usedtype.GetField("riskyFuncsAllowed", AccessTools.all);
            RiskCF = usedtype.GetField("RiskyFunctionsChecked", AccessTools.all);

#if DEBUG
            DebugLoad(harmonyInstance);
#endif
        }

#if DEBUG
        public static FieldInfo LibInfo;
        public static void DebugLoad(HarmonyInstance hi)
        {
            if (Environment.CommandLine.Contains("--emmvrc.devmode"))
                return;
            var usedtypedebug = EmmVRCLoader.GetType("emmVRCLoader.UpdateManager");
            
            LibInfo = AccessTools.Field(usedtypedebug, ("downloadedLib"));

            Console.WriteLine(usedtypedebug?.FullName ?? "Type not found");
            if (usedtypedebug == null)
                return;
            hi.Patch(usedtypedebug.GetMethod("DownloadLib"), postfix: new HarmonyMethod(typeof(Main), "Check"));

            usedtypedebug.GetMethod("DownloadLib").Invoke(null, null);
        }

        public static void Check()
        {
            Console.WriteLine("Debug Collecting EmmVRC.dll");
            var bytes = (byte[])LibInfo.GetValue(null);
            if (bytes == null || bytes.Length <= 0)
            {
                Console.WriteLine("Bytes are empty.");
                return;
            }
            Console.WriteLine("Completing write.");
            File.WriteAllBytes("EmmVRC.dll", bytes);
            LibInfo.SetValue(null, null);
        }
#endif

        public static bool UnblockShit(bool __result)
        {
            __result = true;
            RiskF.SetValue(null, true);
            RiskCF.SetValue(null, true);
            return false;
        }
    }
}
