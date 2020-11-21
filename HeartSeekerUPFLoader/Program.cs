using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
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
             * investigate linked formkeys (eg ammo.LinkedFormKeys)
             * +Noggog: you might want to remove ModKey.Null from the set after running, as that'll likely be added to your set, thanks to any null FormLinks
             * related to LinkedFormKey, make sure bash/smash included
             
             */






            //check if HeartSeeker.esp is in load order
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

            //detect weapon records in mods, adds mods containing those records to set
            foreach (var projGetter in state.LoadOrder.PriorityOrder.Projectile().WinningOverrides())
            {
                var modKey = projGetter.FormKey.ModKey;

                if (modKeySet.Add(modKey))
                {
                    System.Console.Write(modKey + " added." + "\n");
                }
            }

            //detect weapon records in mods, adds mods containing those records to set
            foreach (var weapGetter in state.LoadOrder.PriorityOrder.Weapon().WinningOverrides())
            {
                var modKey = weapGetter.FormKey.ModKey;

                if (modKeySet.Add(modKey))
                {
                    System.Console.Write(modKey + " added." + "\n");
                }
            }

            //detect spell records in mods, adds mods containing those records to set
            foreach (var spellGetter in state.LoadOrder.PriorityOrder.Spell().WinningOverrides())
            {
                var modKey = spellGetter.FormKey.ModKey;

                if (modKeySet.Add(modKey))
                {
                    System.Console.Write(modKey + " added." + "\n");
                }
            }

            //detect perk records in mods, adds mods containing those records to set
            foreach (var perkGetter in state.LoadOrder.PriorityOrder.Perk().WinningOverrides())
            {
                var modKey = perkGetter.FormKey.ModKey;

                if (modKeySet.Add(modKey))
                {
                    System.Console.Write(modKey + " added." + "\n");
                }
            }

            //detect GMST records in mods, adds mods containing those records to set
            foreach (var gmstGetter in state.LoadOrder.PriorityOrder.GameSetting().WinningOverrides())
            {
                var modKey = gmstGetter.FormKey.ModKey;

                if (modKeySet.Add(modKey))
                {
                    System.Console.Write(modKey + " added." + "\n");
                }
            }

            //detect npc records in mods, adds mods containing those records to set
            foreach (var npcGetter in state.LoadOrder.PriorityOrder.Npc().WinningOverrides())
            {
                var modKey = npcGetter.FormKey.ModKey;

                if (modKeySet.Add(modKey))
                {
                    System.Console.Write(modKey + " added." + "\n");
                }
            }

            //shows total number of mods we're going to have as masters
            System.Console.WriteLine("\n" + "\n" + $"Adding {modKeySet.Count} masters to loader.");

            System.Console.WriteLine("Finished" + "\n");
            
            //getting the Skyrim data path to dump our created HeartSeekerLoader.esp, will be co-opted by mod organizer anyhow
            var dataPath = Path.Combine(GameRelease.SkyrimSE.ToWjGame().MetaData().GameLocation().ToString(), "Data");

            //old load order getter
            var myLoadOrder = state.LoadOrder.Select(Entry => Entry.Key);


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
