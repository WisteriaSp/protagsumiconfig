using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ProtagSumiConfig.Configuration;
using ProtagSumiConfig.Template;
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using CriFs.V2.Hook.Interfaces;
using P5R.CostumeFramework.Interfaces;
using Ryo.Interfaces;
using P5R.CostumeFramework;

namespace ProtagSumiConfig
{
    public class Mod : ModBase
    {
        private readonly IModLoader _modLoader;
        private readonly IReloadedHooks? _hooks;
        private readonly ILogger _logger;
        private readonly IMod _owner;
        private Config _configuration;
        private readonly IModConfig _modConfig;

        public Mod(ModContext context)
        {
            _modLoader = context.ModLoader;
            _hooks = context.Hooks;
            _logger = context.Logger;
            _owner = context.Owner;
            _configuration = context.Configuration;
            _modConfig = context.ModConfig;

            // grab modDi modId :P
            string modDir = _modLoader.GetDirectoryForModId(_modConfig.ModId);
            string modId = _modConfig.ModId;


            var criFsCtl = _modLoader.GetController<ICriFsRedirectorApi>();
            if (criFsCtl == null || !criFsCtl.TryGetTarget(out var criFsApi))
            {
                _logger.WriteLine(
                    $"[Mod:{modId}] CRI FS API unavailable → config options WILL NOT LOAD.",
                    System.Drawing.Color.Red
                );
                return;
            }


            var bfEmuCtl = _modLoader.GetController<BF.File.Emulator.Interfaces.IBfEmulator>();
            var bmdEmuCtl = _modLoader.GetController<BMD.File.Emulator.Interfaces.IBmdEmulator>();
            var pakEmuCtl = _modLoader.GetController<PAK.Stream.Emulator.Interfaces.IPakEmulator>();
            var costumeCtl = _modLoader.GetController<ICostumeApi>();
            var ryoCtl = _modLoader.GetController<IRyoApi>();

            if (bfEmuCtl == null || !bfEmuCtl.TryGetTarget(out var bfEmu)) { _logger.WriteLine("BF Emu missing → BF merges broken.", System.Drawing.Color.Red); return; }
            if (bmdEmuCtl == null || !bmdEmuCtl.TryGetTarget(out var bmdEmu)) { _logger.WriteLine("BMD Emu missing → BMD merges broken.", System.Drawing.Color.Red); return; }
            if (pakEmuCtl == null || !pakEmuCtl.TryGetTarget(out var pakEmu)) { _logger.WriteLine("PAK Emu missing → PAK merges broken.", System.Drawing.Color.Red); return; }
            if (costumeCtl == null || !costumeCtl.TryGetTarget(out var costumeApi)) { _logger.WriteLine("Costume API missing → Costumes broken.", System.Drawing.Color.Red); return; }
            if (ryoCtl == null || !ryoCtl.TryGetTarget(out var ryoApi)) { _logger.WriteLine("Ryo API missing → Audio configs broken.", System.Drawing.Color.Red); return; }

            // check for rose and violet
            var active = _modLoader.GetActiveMods().Select(x => x.Generic.ModId).ToHashSet();
            bool isRoseViolet = active.Contains("p5rpc.kasumi.roseandviolet");
            bool isCBT = active.Contains("p5r.enhance.cbt");

            // if its not active we pussy on
            if (!isRoseViolet && _configuration.EventEdits1)
            {
                criFsApi.AddProbingPath(Path.Combine("OptionalModFiles", "Events", "Fixes"));
            }
            if (!isRoseViolet && _configuration.EventEditsBig)
            {
                criFsApi.AddProbingPath(Path.Combine("OptionalModFiles", "Events", "LargeEdits"));
                bfEmu.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Events", "LargeEdits", "BF"));
                bmdEmu.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Events", "LargeEdits", "BMD"));

                BindAllFilesIn(
                    $"{Path.Combine("OptionalModFiles", "Events", "LargeEdits", "Characters", "Joker", "1")}",
                    modDir, criFsApi, modId
                );
            }

            if (isCBT)
            {
                BindAllFilesIn(
                    $"{Path.Combine("OptionalModFiles", "Model", "CBT", "BetterExitMaterials", "Characters", "Joker", "1")}",
                    modDir, criFsApi, modId
                );
            }

            // Darkened Face
            if (_configuration.DarkenedFace)
            {
                BindAllFilesIn(
                    $"{Path.Combine("OptionalModFiles", "Model", "DarkenedFace", "Characters", "Joker", "1")}",
                    modDir, criFsApi, modId
                );
            }

            // Blue Dress
            if (_configuration.BlueDress)
            {
                BindAllFilesIn(
                    $"{Path.Combine("OptionalModFiles", "Model", "BlueDress", "Characters", "Joker", "1")}",
                    modDir, criFsApi, modId
                );
            }

            // Tracksuit (Black or ConceptArt)
            if (_configuration.TracksuitSelection == Config.TracksuitEnum.BlackTracksuit ||
                _configuration.TracksuitSelection == Config.TracksuitEnum.ConceptArt)
            {
                string selected =
                    _configuration.TracksuitSelection == Config.TracksuitEnum.BlackTracksuit
                        ? "OldTracksuit"
                        : "TracksuitConceptArt";

                BindAllFilesIn(
                    $"{Path.Combine("OptionalModFiles", "Model", selected, "Characters", "Joker", "1")}",
                    modDir, criFsApi, modId
                );
            }

            // Alt Meta Run Animation
            if (_configuration.AltMetaRun)
            {
                BindAllFilesIn(
                    $"{Path.Combine("OptionalModFiles", "Animation", "AltMetaRun", "Characters", "Joker", "1")}",
                    modDir, criFsApi, modId
                );
            }

            // Women’s Bath House (includes BF/BMD dirs)
            if (_configuration.Bathhouse)
            {
                criFsApi.AddProbingPath(Path.Combine(modDir, "OptionalModFiles", "Flowscript", "Bath"));
                bfEmu.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Flowscript", "Bath", "BF"));
                bmdEmu.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Flowscript", "Bath", "BMD"));
            }

            // Thieves Den
            if (_configuration.ThievesDenAddon)
            {
                criFsApi.AddProbingPath(Path.Combine(modDir, "OptionalModFiles", "Misc", "ThievesDen"));
                bfEmu.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Misc", "ThievesDen", "BF"));
                bmdEmu.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Misc", "ThievesDen", "BMD"));
                pakEmu.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Misc", "ThievesDen", "PAK"));
            }

            // Women’s Bath House Event
            if (_configuration.BathActivity)
            {
                criFsApi.AddProbingPath(Path.Combine(modDir, "OptionalModFiles", "Events", "Bath"));
            }

            // Shujin Restroom
            if (_configuration.Restroom)
            {
                criFsApi.AddProbingPath(Path.Combine(modDir, "OptionalModFiles", "Flowscript", "Restroom"));
                bfEmu.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Flowscript", "Restroom", "BF"));
            }

            // Equipment Config
            if (_configuration.Equipment)
            {
                criFsApi.AddProbingPath(Path.Combine(modDir, "OptionalModFiles", "Gameplay", "Equipment"));
                pakEmu.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Gameplay", "Equipment", "FEmulator", "PAK"));
            }

            // Persona Swap Config
            if (_configuration.PersonasMod == Config.CendrillonMod.DefaultCendrillon ||
                _configuration.PersonasMod == Config.CendrillonMod.RedCendrillon)
            {
                criFsApi.AddProbingPath(Path.Combine(modDir, "OptionalModFiles", "Gameplay", "Personas"));
                pakEmu.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Gameplay", "Personas", "FEmulator", "PAK"));
                bmdEmu.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Gameplay", "Personas", "FEmulator", "BMD"));

                string cendrillonFolder =
                    _configuration.PersonasMod == Config.CendrillonMod.DefaultCendrillon
                        ? "Cendrillon"
                        : "CurseCendrillon";

                criFsApi.AddProbingPath(Path.Combine(modDir, "OptionalModFiles", "Gameplay", cendrillonFolder));
            }

            // Skillset Config
            if (_configuration.Skillset)
            {
                criFsApi.AddProbingPath(Path.Combine(modDir, "OptionalModFiles", "Gameplay", "Skillset"));
                bfEmu.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Gameplay", "Skillset", "FEmulator", "BF"));
            }

            // Opening Movie by Arbiter
            if (_configuration.OpeningMovie)
            {
                criFsApi.AddProbingPath(Path.Combine(modDir, "OptionalModFiles", "Misc", "Movie"));
            }

            // NoAoAArt
            if (_configuration.AoAArt == Config.AoAArtEnum.Enabled ||
                _configuration.AoAArt == Config.AoAArtEnum.Smug)
            {
                var aoaFolders = new List<string> { "NoAoAPortrait" };
                if (_configuration.AoAArt == Config.AoAArtEnum.Smug)
                {
                    aoaFolders.Add("SmugAoA");
                }
                foreach (var folderName in aoaFolders)
                {
                    BindAllFilesIn(
                        $"{Path.Combine("OptionalModFiles", "Model", folderName, "Characters", "Joker", "1")}",
                        modDir, criFsApi, modId
                    );
                }
            }

            // OneCalledJay Bustup
            if (_configuration.Bustup1 == Config.BustupSelection.OnedCalledJay)
            {
                BindAllFilesIn(
                    $"{Path.Combine("OptionalModFiles", "Bustup", "OneCalledJay", "Characters", "Joker", "1")}",
                    modDir, criFsApi, modId
                );
            }

            // Weapon Ranged: LeverAction
            if (_configuration.WeaponRanged == Config.WeaponRangedEnum.LeverAction)
            {
                BindAllFilesIn(
                    $"{Path.Combine("OptionalModFiles", "Model", "Ranged", "Characters", "Joker", "1")}",
                    modDir, criFsApi, modId
                );
            }

            // Weapon Melee: Rapier
            if (_configuration.MeleeRanged == Config.MeleeRangedEnum.Rapier)
            {
                BindAllFilesIn(
                    $"{Path.Combine("OptionalModFiles", "Model", "Melee", "Characters", "Joker", "1")}",
                    modDir, criFsApi, modId
                );
            }

            // Costume Support
            if (_configuration.CostumeSupport)
            {
                var costumesFolder = Path.Combine(modDir, "OptionalModFiles", "CostumeSupport", "Costumes");
                costumeApi.AddCostumesFolder(modDir, costumesFolder);
            }

            // MiniBoss Music
            if (_configuration.MiniBossMusic)
            {
                var audioFolder = Path.Combine(modDir, "OptionalModFiles", "Audio", "ShowToRemember");
                if (Directory.Exists(audioFolder))
                {
                    ryoApi.AddAudioPath(audioFolder, null);
                }
            }

            // L7M3 bustup
            if (_configuration.Bustup1 == Config.BustupSelection.L7M3)
            {
                BindAllFilesIn(
                    $"{Path.Combine("OptionalModFiles", "Bustup", "L7M3", "Characters", "Joker", "1")}",
                    modDir, criFsApi, modId
                );
            }
        }

        /// <summary>
        /// Recursively enumerates all files under the given “subPath” (relative to the mod folder),
        /// and issues a single AddBind(...) per file. If the directory doesn’t exist, it silently does nothing.
        /// </summary>
        private static void BindAllFilesIn(
            string subPathRelativeToModDir,
            string modDir,
            ICriFsRedirectorApi criFsApi,
            string modId
        )
        {
            // Build the absolute on-disk path to “subPathRelativeToModDir”
            // e.g. subPathRelativeToModDir = "OptionalModFiles/Model/BlueDress/Characters/Joker/1"
            string absoluteFolder = Path.Combine(modDir, subPathRelativeToModDir);

            if (!Directory.Exists(absoluteFolder))
            {
                // If you want a missing-folder log, uncomment the next line:
                // _logger.WriteLine($"Folder not found: {absoluteFolder}", System.Drawing.Color.Yellow);
                return;
            }

            foreach (var filePath in Directory.EnumerateFiles(absoluteFolder, "*", SearchOption.AllDirectories))
            {
                // Compute “relative inside CPK” by stripping off “absoluteFolder” from filePath
                // e.g. filePath = "C:\...mod\OptionalModFiles\Model\BlueDress\Characters\Joker\1\face.bmd"
                // → relativeCpkKey = "face.bmd" or if deeper "textures\01.dds"
                string relativeCpkKey = Path.GetRelativePath(absoluteFolder, filePath).Replace(Path.DirectorySeparatorChar, '/');

                // Tell CRI “when someone requests <relativeCpkKey> inside that folder, substitute with this on-disk file”
                criFsApi.AddBind(
                    filePath,
                    relativeCpkKey,
                    modId
                );
            }
        }

        #region Standard Overrides
        public override void ConfigurationUpdated(Config configuration)
        {
            _configuration = configuration;
            _logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
        }
        #endregion

        #region For Exports, Serialization etc.
        // This parameterless ctor is only here to satisfy some serializers/reflection.
#pragma warning disable CS8618
        public Mod() { }
#pragma warning restore CS8618
        #endregion
    }
}
