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
            var mk = ModKey.FromNameAndExtension("HeartSeeker.esp");
            if (!state.LoadOrder.ContainsKey(mk))
            {
                System.Console.WriteLine($"Please add {mk} to your load order");
                return;
            }

            System.Console.WriteLine($"{mk} found! Loading...");

            var ammoList = new List<string>();
            foreach (var ammoGetter in state.LoadOrder.PriorityOrder.Ammunition().WinningOverrides())
            {
                var ammoGot = ammoGetter.FormKey.ModKey.Name;
                
                if (ammoList.Contains(ammoGot))
                {
                    System.Console.WriteLine($"List already contains {ammoGot}, moving on...");
                    continue;
                }

                if (!ammoList.Contains(ammoGot))
                {
                    ammoList.Add(ammoGot);
                    System.Console.WriteLine($"{ammoGot} added.");
                    continue;
                }

            }

            ammoList.Sort();
            System.Console.WriteLine("\n" + "\n" + $"Found {ammoList.Count} masters to add:");
            ammoList.ForEach(item => System.Console.Write(item + "\n"));
            System.Console.WriteLine("\n");

        }

    }
}
