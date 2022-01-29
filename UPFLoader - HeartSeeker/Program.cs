using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;

namespace UPFLoaderHeartSeeker
{
    public class Program
    {
        public static Task<int> Main(string[] args)
        {
            return SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .Run(args, new RunPreferences()
                {
                    ActionsForEmptyArgs = new RunDefaultPatcher()
                    {
                        IdentifyingModKey = "HeartSeekerLoader.esp",
                        TargetRelease = GameRelease.SkyrimSE,
                        BlockAutomaticExit = true,
                    }
                });
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
 
            /*
             TO DO:
             * check if exceeding 254 plugin load and warn
             * clean up, tighten up
             * custom binarywrite, to flag output as esl and stop the dummy Synthesis.esp being created
             */


            //check if HeartSeeker.esp is in load order
            var mk = ModKey.FromNameAndExtension("HeartSeeker.esp");
            if (!state.LoadOrder.ContainsKey(mk))
            {
                System.Console.WriteLine($"Please add {mk} to your load order");
                return;
            }
            
            // got it, carrying on
            System.Console.WriteLine($"{mk} found! Loading...");

            
            //create hash set to hold mods we want
            var modKeySet = new HashSet<ModKey>();
            

            //detect ammunition records in mods, adds mods containing those records to set
            foreach (var ammoGetter in state.LoadOrder.PriorityOrder.Ammunition().WinningOverrides())

            {
                //pull the mod names from the FormKey
                var modKey = ammoGetter.FormKey.ModKey;
                //pull the mod names from linked records
                var linkedKeys = ammoGetter.ContainedFormLinks.Select(l => l.FormKey.ModKey);

                //add keys to our hashset
                modKeySet.Add(modKey);
                modKeySet.Add(linkedKeys);

            }
            //make sure we get mods that make overrides to ammo records
            foreach (var ammoGetter in state.LoadOrder.PriorityOrder.Ammunition().WinningContextOverrides())
            {
                //add keys to our hashset
                modKeySet.Add(ammoGetter.ModKey);
            }

            //detect projectile records in mods, adds mods containing those records to set. 
            //heartseeker doesn't specifically call for, but safer to include
            foreach (var projGetter in state.LoadOrder.PriorityOrder.Projectile().WinningOverrides())
            {
                var modKey = projGetter.FormKey.ModKey;
                var linkedKeys = projGetter.ContainedFormLinks.Select(l => l.FormKey.ModKey);


                //add keys to our hashset
                modKeySet.Add(modKey);
                modKeySet.Add(linkedKeys);
            }

            //make sure we get mods that make overrides to projectile records
            foreach (var projGetter in state.LoadOrder.PriorityOrder.Projectile().WinningContextOverrides())
            {
                //add keys to our hashset
                modKeySet.Add(projGetter.ModKey);
            }

            //detect weapon records in mods, adds mods containing those records to set
            foreach (var weapGetter in state.LoadOrder.PriorityOrder.Weapon().WinningOverrides())
            {
                var modKey = weapGetter.FormKey.ModKey;
                var linkedKeys = weapGetter.ContainedFormLinks.Select(l => l.FormKey.ModKey);

                //add keys to our hashset
                modKeySet.Add(modKey);
                modKeySet.Add(linkedKeys);
            }

            //make sure we get mods that make overrides to weapon records
            foreach (var weapGetter in state.LoadOrder.PriorityOrder.Weapon().WinningContextOverrides())
            {
                //add keys to our hashset
                modKeySet.Add(weapGetter.ModKey);
            }

            //detect spell records in mods, adds mods containing those records to set
            foreach (var spellGetter in state.LoadOrder.PriorityOrder.Spell().WinningOverrides())
            {
                var modKey = spellGetter.FormKey.ModKey;
                var linkedKeys = spellGetter.ContainedFormLinks.Select(l => l.FormKey.ModKey);

                //add keys to our hashset
                modKeySet.Add(modKey);
                modKeySet.Add(linkedKeys);
            }

            //make sure we get mods that make overrides to spell records
            foreach (var spellGetter in state.LoadOrder.PriorityOrder.Spell().WinningContextOverrides())
            {
                //add keys to our hashset
                modKeySet.Add(spellGetter.ModKey);
            }

            //detect perk records in mods, adds mods containing those records to set
            foreach (var perkGetter in state.LoadOrder.PriorityOrder.Perk().WinningOverrides())
            {
                var modKey = perkGetter.FormKey.ModKey;
                var linkedKeys = perkGetter.ContainedFormLinks.Select(l => l.FormKey.ModKey);

                //add keys to our hashset
                modKeySet.Add(modKey);
                modKeySet.Add(linkedKeys);
            }

            //make sure we get mods that make overrides to perk records
            foreach (var perkGetter in state.LoadOrder.PriorityOrder.Perk().WinningContextOverrides())
            {
                //add keys to our hashset
                modKeySet.Add(perkGetter.ModKey);
            }

            //detect GMST records in mods, adds mods containing those records to set
            foreach (var gmstGetter in state.LoadOrder.PriorityOrder.GameSetting().WinningOverrides())
            {
                var modKey = gmstGetter.FormKey.ModKey;
                var linkedKeys = gmstGetter.ContainedFormLinks.Select(l => l.FormKey.ModKey);

                //add keys to our hashset
                modKeySet.Add(modKey);
                modKeySet.Add(linkedKeys);
            }

            //make sure we get mods that make overrides to gmst records
            foreach (var gmstGetter in state.LoadOrder.PriorityOrder.GameSetting().WinningContextOverrides())
            {
                //add keys to our hashset
                modKeySet.Add(gmstGetter.ModKey);
            }

            //detect npc records in mods, adds mods containing those records to set
            foreach (var npcGetter in state.LoadOrder.PriorityOrder.Npc().WinningOverrides())
            {
                var modKey = npcGetter.FormKey.ModKey;
                var linkedKeys = npcGetter.ContainedFormLinks.Select(l => l.FormKey.ModKey);

                //add keys to our hashset
                modKeySet.Add(modKey);
                modKeySet.Add(linkedKeys);
            }

            //make sure we get mods that make overrides to npc records
            foreach (var npcGetter in state.LoadOrder.PriorityOrder.Npc().WinningContextOverrides())
            {
                //add keys to our hashset
                modKeySet.Add(npcGetter.ModKey);
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

            //gets modkey from the load order
            var myLoadOrder = state.LoadOrder.Select(loadKey => loadKey.Key);

            //takes the set of mods we've collected and adds them as masters to the esp
            state.PatchMod.ModHeader.MasterReferences.AddRange(
                modKeySet.Select(m => new MasterReference()
                {
                    Master = m
                }));
                                   

            //special output of our esp to get around synthesis default, dummy synthesis esp still created
            state.PatchMod.WriteToBinary(
            Path.Combine(state.DataFolderPath, "HeartSeekerLoader.esp"),
            new BinaryWriteParameters()
            {
                // Don't modify the content of the masters list with what records we have inside
                MastersListContent = MastersListContentOption.NoCheck,
                 
                // Order the masters to match load order
                //old load order getter config
                MastersListOrdering = new MastersListOrderingByLoadOrder(myLoadOrder),
                //new mutagen 0.21.3, i've not got working yet
                //MastersListOrdering = new BinaryWriteParameters.MastersListOrderingByLoadOrder(state.LoadOrder),
                //Ignore default Synthesis.esp mod output name
                ModKey = ModKeyOption.NoCheck,
                
            });

        }

    }
}
