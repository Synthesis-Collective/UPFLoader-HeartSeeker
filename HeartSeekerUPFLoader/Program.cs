using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wabbajack.Common;

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
                        IdentifyingModKey = "HeartSeekerLoader.esp",
                        TargetRelease = GameRelease.SkyrimSE,
                        BlockAutomaticExit = true,
                    }
                });
        }

        public static void RunPatch(SynthesisState<ISkyrimMod, ISkyrimModGetter> state)
        {



            /*
             
             TO DO:
             * run more comparison tests
             * check if exceeding 254 plugin load and warn
             * clean up, tighten up
             * investigate mutagen 0.21.3 load order calling
             */




            //check if HeartSeeker.esp is in load order
            var mk = ModKey.FromNameAndExtension("HeartSeeker.esp");
            if (!state.LoadOrder.ContainsKey(mk))
            {
                System.Console.WriteLine($"Please add {mk} to your load order");
                return;
            }
            
            // got it, carrying on
            //System.Console.WriteLine($"{mk} found! Loading...");

            
            //create hash set to hold mods we want
            var modKeySet = new HashSet<ModKey>();
            

            //detect ammunition records in mods, adds mods containing those records to set
            foreach (var ammoGetter in state.LoadOrder.PriorityOrder.Ammunition().WinningOverrides())
            {
                //pull the mod names from the FormKey
                var modKey = ammoGetter.FormKey.ModKey;
                //pull the mod names from linked records
                var linkedKeys = ammoGetter.LinkFormKeys.Select(l => l.ModKey);

                //add keys to our hashset
                modKeySet.Add(modKey);
                modKeySet.Add(linkedKeys);

            }

            //detect projectile records in mods, adds mods containing those records to set. 
            //heartseeker doesn't specifically call for, but safer to include
            foreach (var projGetter in state.LoadOrder.PriorityOrder.Projectile().WinningOverrides())
            {
                var modKey = projGetter.FormKey.ModKey;
                var linkedKeys = projGetter.LinkFormKeys.Select(l => l.ModKey);

                //add keys to our hashset
                modKeySet.Add(modKey);
                modKeySet.Add(linkedKeys);
            }

            //detect weapon records in mods, adds mods containing those records to set
            foreach (var weapGetter in state.LoadOrder.PriorityOrder.Weapon().WinningOverrides())
            {
                var modKey = weapGetter.FormKey.ModKey;
                var linkedKeys = weapGetter.LinkFormKeys.Select(l => l.ModKey);

                //add keys to our hashset
                modKeySet.Add(modKey);
                modKeySet.Add(linkedKeys);
            }

            //detect spell records in mods, adds mods containing those records to set
            foreach (var spellGetter in state.LoadOrder.PriorityOrder.Spell().WinningOverrides())
            {
                var modKey = spellGetter.FormKey.ModKey;
                var linkedKeys = spellGetter.LinkFormKeys.Select(l => l.ModKey);

                //add keys to our hashset
                modKeySet.Add(modKey);
                modKeySet.Add(linkedKeys);
            }

            //detect perk records in mods, adds mods containing those records to set
            foreach (var perkGetter in state.LoadOrder.PriorityOrder.Perk().WinningOverrides())
            {
                var modKey = perkGetter.FormKey.ModKey;
                var linkedKeys = perkGetter.LinkFormKeys.Select(l => l.ModKey);

                //add keys to our hashset
                modKeySet.Add(modKey);
                modKeySet.Add(linkedKeys);
            }

            //detect GMST records in mods, adds mods containing those records to set
            foreach (var gmstGetter in state.LoadOrder.PriorityOrder.GameSetting().WinningOverrides())
            {
                var modKey = gmstGetter.FormKey.ModKey;
                var linkedKeys = gmstGetter.LinkFormKeys.Select(l => l.ModKey);

                //add keys to our hashset
                modKeySet.Add(modKey);
                modKeySet.Add(linkedKeys);
            }

            //detect npc records in mods, adds mods containing those records to set
            foreach (var npcGetter in state.LoadOrder.PriorityOrder.Npc().WinningOverrides())
            {
                var modKey = npcGetter.FormKey.ModKey;
                var linkedKeys = npcGetter.LinkFormKeys.Select(l => l.ModKey);

                //add keys to our hashset
                modKeySet.Add(modKey);
                modKeySet.Add(linkedKeys);
            }

            //removes null mod names that may have been gathered
            modKeySet.Remove(ModKey.Null);

            //list collected masters
            foreach (ModKey key in modKeySet)
            {
                System.Console.WriteLine(key);
            }

            //shows total number of mods we're going to have as masters
            System.Console.WriteLine("\n" + "\n" + $"Adding {modKeySet.Count} masters to loader.");

            System.Console.WriteLine("Finished" + "\n");
            
            //getting the Skyrim data path to dump our created HeartSeekerLoader.esp, will be co-opted by mod organizer anyhow
            var dataPath = Path.Combine(GameRelease.SkyrimSE.ToWjGame().MetaData().GameLocation().ToString(), "Data");

            //old load order getter
            var myLoadOrder = state.LoadOrder.Select(loadOrder => loadOrder.Key);


            //takes the set of mods we've collected and adds them as masters to the esp
            state.PatchMod.ModHeader.MasterReferences.AddRange(
                modKeySet.Select(m => new MasterReference()
                {
                    Master = m
                }));


            //special output of our esp to get around synthesis default, dummy synthesis esp still created
            state.PatchMod.WriteToBinary(
            Path.Combine(dataPath, "HeartSeekerLoader.esp"),
            new BinaryWriteParameters()
            {
                // Don't modify the content of the masters list with what records we have inside
                MastersListContent = BinaryWriteParameters.MastersListContentOption.NoCheck,
                 
                // Order the masters to match load order
                //old load order getter config
                MastersListOrdering = new BinaryWriteParameters.MastersListOrderingByLoadOrder(myLoadOrder),
                //new mutagen 0.21.3, i've not got working yet
                //MastersListOrdering = new BinaryWriteParameters.MastersListOrderingByLoadOrder(state.LoadOrder),
                //Ignore default Synthesis.esp mod output name
                ModKey = BinaryWriteParameters.ModKeyOption.NoCheck,
            });

        }

    }
}
