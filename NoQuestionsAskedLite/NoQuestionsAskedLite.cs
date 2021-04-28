using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ColossalFramework;
using HarmonyLib;
using UnityEngine;

namespace NoQuestionsAskedLite
{
    public static class NoQuestionsAskedLite
    {
        private const string HarmonyId = "coldrifting.NoQuestionsAskedLite";
        private static bool patched = false;

        public static void PatchAll()
        {
            if (patched) 
                return;

            patched = true;

            var harmony = new Harmony(HarmonyId);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static void UnpatchAll()
        {
            if (!patched) 
                return;

            var harmony = new Harmony(HarmonyId);
            harmony.UnpatchAll(HarmonyId);
            patched = false;
        }


        // Remove the Bulldoze confirmation prompt by 
        // redirecting the BulldozeGUI Class to use DeleteBuilding()
        // instead of TryDeleteBuilding(), which prompts.
        [HarmonyPatch(typeof(BulldozeTool))]
        [HarmonyPatch("OnToolGUI")]
        public static class BulldozeToolOnToolGUIPatch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);
                var flags = BindingFlags.NonPublic | BindingFlags.Instance;

                Debug.Log(codes[56].operand);

                for (int i = 0; i < codes.Count; i++)
                {
                    // Skip instructions that don't call methods
                    if (codes[i].opcode != OpCodes.Call)
                        continue;

                    // Replace the method call
                    if (codes[i].operand == typeof(BulldozeTool).GetMethod("TryDeleteBuilding", flags))
                    {
                        codes[i] = new CodeInstruction(OpCodes.Call, typeof(BulldozeTool).GetMethod("DeleteBuilding", flags));
                    }
                }

                return codes.AsEnumerable();
            }
        }


        // Allow bulldoze tool to remove all buildings of a certain type
        // by holding down the mouse button. To do this, we remove this
        // if statment from a private method by using a Harmony Transpiler.
        // if (info.m_placementStyle == ItemClass.Placement.Automatic &&
        //     (instance.m_buildings.m_buffer[building].m_flags & Building.Flags.Historical) == 0)
        [HarmonyPatch(typeof(BulldozeTool))]
        [HarmonyPatch("DeleteBuildingImpl")]
        [HarmonyPatch(new System.Type[] { typeof(ushort) })]
        public static class BulldozeToolDeleteBuildingImplPatch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                int startIndex = -1;
                int endIndex = -1;
                bool endIndexFound = false;

                var codes = new List<CodeInstruction>(instructions);
                for (int i = 0; i < codes.Count; i++)
                {
                    if(endIndexFound)
                    {
                        break;
                    }
                    // The if statement we want to remove is between these two instruction codes
                    if (codes[i].opcode == OpCodes.Bne_Un)
                    {
                        // Exclude current 'bne.un'
                        startIndex = i + 1;

                        for (int j = startIndex; j < codes.Count; j++)
                        {
                            if (codes[j].opcode == OpCodes.Brtrue)
                            {
                                // Include final 'brtrue'
                                endIndex = j;
                                endIndexFound = true;
                                break;
                            }
                        }
                    }
                }

                // Make sure a valid start and end index was found
                if (startIndex > -1 && endIndex > -1)
                {
                    codes.RemoveRange(startIndex, endIndex - startIndex + 1);
                }

                return codes.AsEnumerable();
            }
        }
    }
}
