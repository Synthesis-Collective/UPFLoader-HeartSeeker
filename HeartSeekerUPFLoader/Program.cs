using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using System;
using System.Collections.Generic;

namespace HeartSeekerUPFLoader
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return SynthesisPipeline.Instance.Patch<ISkyrimMod, ISkyrimModGetter>(
                args: args,
                patcher: RunPatch,
                userPreferences: new UserPreferences()
                {
                    ActionsForEmptyArgs = new RunDefaultPatcher()
                    {
                        IdentifyingModKey = "heartseekerloader.esp",
                        TargetRelease = GameRelease.SkyrimSE,
                        BlockAutomaticExit = true,
                    }
                });
        }

        public static void RunPatch(SynthesisState<ISkyrimMod, ISkyrimModGetter> state)
        {

            //check if HeartSeek.esp is in load order
            var mk = ModKey.FromNameAndExtension("HeartSeeker.esp");
            if (!state.LoadOrder.ContainsKey(mk))
            {
                System.Console.WriteLine($"Please add {mk} to your load order");
                return;
            }

            System.Console.WriteLine($"{mk} found! Loading...");

            //create hash set to hold mods we want
            var modKeySet = new HashSet<ModKey>();

            //detect ammunition records in mods, adds mods containing those records to set
            foreach (var ammoGetter in state.LoadOrder.PriorityOrder.Ammunition().WinningOverrides())
            {
                var modKey = ammoGetter.FormKey.ModKey;

                if (modKeySet.Add(modKey))
                {
                    System.Console.Write(modKey + " added." + "\n");
                }                
            }

            //shows total number of mods we're going to have as masters
            System.Console.WriteLine("\n" + "\n" + $"Adding {modKeySet.Count} masters to loader.");

            System.Console.WriteLine("Finished" + "\n");



        }

    }
}
