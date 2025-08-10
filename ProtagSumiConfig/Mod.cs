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

            // grab modDir and modId
            string modDir = _modLoader.GetDirectoryForModId(_modConfig.ModId);
            string modId = _modConfig.ModId;

            var criFsCtl = _modLoader.GetController<ICriFsRedirectorApi>();
            var bfEmuCtl = _modLoader.GetController<BF.File.Emulator.Interfaces.IBfEmulator>();
            var bmdEmuCtl = _modLoader.GetController<BMD.File.Emulator.Interfaces.IBmdEmulator>();
            var pakEmuCtl = _modLoader.GetController<PAK.Stream.Emulator.Interfaces.IPakEmulator>();
            var spdEmuCtl = _modLoader.GetController<SPD.File.Emulator.Interfaces.ISpdEmulator>();
            var costumeCtl = _modLoader.GetController<ICostumeApi>();
            var ryoCtl = _modLoader.GetController<IRyoApi>();

            if (criFsCtl == null || !criFsCtl.TryGetTarget(out var criFsApi)) { _logger.WriteLine("CRI FS Emu missing → config binds broken.", System.Drawing.Color.Red); return; }
            if (bfEmuCtl == null || !bfEmuCtl.TryGetTarget(out var bfEmu)) { _logger.WriteLine("BF Emu missing → BF merges broken.", System.Drawing.Color.Red); return; }
            if (bmdEmuCtl == null || !bmdEmuCtl.TryGetTarget(out var bmdEmu)) { _logger.WriteLine("BMD Emu missing → BMD merges broken.", System.Drawing.Color.Red); return; }
            if (pakEmuCtl == null || !pakEmuCtl.TryGetTarget(out var pakEmu)) { _logger.WriteLine("PAK Emu missing → PAK merges broken.", System.Drawing.Color.Red); return; }
            if (spdEmuCtl == null || !spdEmuCtl.TryGetTarget(out var spdEmu)) { _logger.WriteLine("SPD Emu missing → SPD merges broken.", System.Drawing.Color.Red); return; }
            if (costumeCtl == null || !costumeCtl.TryGetTarget(out var costumeApi)) { _logger.WriteLine("Costume API missing → Costumes broken.", System.Drawing.Color.Red); return; }
            if (ryoCtl == null || !ryoCtl.TryGetTarget(out var ryoApi)) { _logger.WriteLine("Ryo API missing → Audio configs broken.", System.Drawing.Color.Red); return; }

            var active = _modLoader.GetActiveMods().Select(x => x.Generic.ModId).ToHashSet();
            // check for rose and violet
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
                    Path.Combine("OptionalModFiles", "Events", "LargeEdits", "Bind", "MODEL", "CHARACTER", "0004"),
                    modDir, criFsApi, modId
                );
            }

            if (isCBT)
            {
                BindAllFilesIn(
                    Path.Combine("OptionalModFiles", "Model", "CBT-BetterExitMaterials"),
                    modDir, criFsApi, modId
                );
            }

            // Darkened Face
            if (_configuration.DarkenedFace)
            {
                BindAllFilesIn(
                    Path.Combine("OptionalModFiles", "Model", "DarkenedFace"),
                    modDir, criFsApi, modId
                );
            }

            // Blue Dress
            if (_configuration.BlueDress)
            {
                BindAllFilesIn(
                    Path.Combine("OptionalModFiles", "Model", "BlueDress"),
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
                    Path.Combine("OptionalModFiles", "Model", selected),
                    modDir, criFsApi, modId
                );
            }

            // Alt Meta Run Animation
            if (_configuration.AltMetaRun)
            {
                BindAllFilesIn(
                    Path.Combine("OptionalModFiles", "Animation", "AltMetaRun"),
                    modDir, criFsApi, modId
                );
            }

            // Lawson
            if (_configuration.LawsonOutfit)
            {
                BindAllFilesIn(
                    Path.Combine("OptionalModFiles", "Model", "Lawson"),
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
                bmdEmu.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Gameplay", "Equipment", "FEmulator", "BMD"));
            }

            // Persona Swap Config
            if (_configuration.PersonasMod == Config.CendrillonMod.DefaultCendrillon ||
                _configuration.PersonasMod == Config.CendrillonMod.RedCendrillon)
            {
                criFsApi.AddProbingPath(Path.Combine(modDir, "OptionalModFiles", "Gameplay", "Personas"));
                pakEmu.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Gameplay", "Personas", "FEmulator", "PAK"));
                bmdEmu.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Gameplay", "Personas", "FEmulator", "BMD"));
                bfEmu.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Gameplay", "Personas", "FEmulator", "BF"));

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
                        Path.Combine("OptionalModFiles", "Model", folderName),
                        modDir, criFsApi, modId
                    );
                }
            }

            // Weapon Ranged: LeverAction
            if (_configuration.WeaponRanged == Config.WeaponRangedEnum.LeverAction)
            {
                BindAllFilesIn(
                    Path.Combine("OptionalModFiles", "Model", "Ranged"),
                    modDir, criFsApi, modId
                );
            }

            // Menu Art
            if (_configuration.MenuArt == Config.MenuArtEnum.Neptune)
            {
                BindAllFilesIn(
                    Path.Combine("OptionalModFiles", "Misc", "Herotex", "Neptune"),
                    modDir, criFsApi, modId
                );
            }

            // Weapon Melee: Rapier
            if (_configuration.MeleeRanged == Config.MeleeRangedEnum.Rapier)
            {
                BindAllFilesIn(
                    Path.Combine("OptionalModFiles", "Model", "Melee"),
                    modDir, criFsApi, modId
                );
            }

            // Costume Support
            if (_configuration.CostumeSupport)
            {
                var costumesFolder = Path.Combine(modDir, "OptionalModFiles", "Costumes");
                costumeApi.AddCostumesFolder(modDir, costumesFolder);
            }

            // Miniboss Music
            if (_configuration.MiniBossMusic)
            {
                var audioFolder = Path.Combine(modDir, "OptionalModFiles", "Audio", "ShowtoRemember");
                if (Directory.Exists(audioFolder))
                {
                    ryoApi.AddAudioPath(audioFolder, null);
                }
                spdEmu.AddDirectory(Path.Combine(modDir, "OptionalModFiles", "Audio", "SPD"));
            }

            // Bustup (OneCalledJay or L7M3)
            if (_configuration.Bustup1 == Config.BustupSelection.OnedCalledJay ||
                _configuration.Bustup1 == Config.BustupSelection.L7M3)
            {
                string bustupFolder = _configuration.Bustup1 == Config.BustupSelection.OnedCalledJay
                    ? "OneCalledJay"
                    : "L7M3";

                BindAllFilesIn(
                    Path.Combine("OptionalModFiles", "Bustup", bustupFolder),
                    modDir, criFsApi, modId
                );
            }
        }

        /// <summary>
        /// recursively enumerates all files under the given “subPath” (relative to the mod folder),
        /// and issues a single AddBind(...) per file. If the directory doesn’t exist, it silently does nothing.
        /// </summary>
        private static void BindAllFilesIn(
            string subPathRelativeToModDir,
            string modDir,
            ICriFsRedirectorApi criFsApi,
            string modId
        )
        {
            string absoluteFolder = Path.Combine(modDir, subPathRelativeToModDir);

            if (!Directory.Exists(absoluteFolder))
            {
                // _logger.WriteLine($"Folder not found: {absoluteFolder}", System.Drawing.Color.Yellow);
                return;
            }

            foreach (var filePath in Directory.EnumerateFiles(absoluteFolder, "*", SearchOption.AllDirectories))
            {
                string relativeCpkKey = Path.GetRelativePath(absoluteFolder, filePath).Replace(Path.DirectorySeparatorChar, '/');

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
#pragma warning disable CS8618
        public Mod() { }
#pragma warning restore CS8618
        #endregion
    }
}
