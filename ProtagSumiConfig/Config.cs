using ProtagSumiConfig.Template.Configuration;
using Reloaded.Mod.Interfaces.Structs;
using System.ComponentModel;
using CriFs.V2.Hook;
using CriFs.V2.Hook.Interfaces;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace ProtagSumiConfig.Configuration
{
    public class Config : Configurable<Config>
    {
        /*
            User Properties:
                - Please put all of your configurable properties here.

            By default, configuration saves as "Config.json" in mod user config folder.    
            Need more config files/classes? See Configuration.cs

            Available Attributes:
            - Category
            - DisplayName
            - Description
            - DefaultValue

            // Technically Supported but not Useful
            - Browsable
            - Localizable

            The `DefaultValue` attribute is used as part of the `Reset` button in Reloaded-Launcher.
        */
        public enum CendrillonMod
        {
            Disabled,

            [Display(Name = "Default Cendrillon")]
            DefaultCendrillon,

            [Display(Name = "Red Cendrillon")]
            RedCendrillon,
        }

        public enum WeaponRangedEnum
        {
            [Display(Name = "Lever Action")]
            LeverAction,

            [Display(Name = "Pistol")]
            Pistol,
        }

        public enum MeleeRangedEnum
        {
            [Display(Name = "Rapier")]
            Rapier,

            [Display(Name = "Dagger")]
            Dagger,
        }

        public enum TracksuitEnum
        {
            [Display(Name = "Disabled")]
            Default,

            [Display(Name = "Black Tracksuit")]
            BlackTracksuit,

            [Display(Name = "Concept Art Tracksuit")]
            ConceptArt,
        }

        public enum AoAArtEnum
        {
            [Display(Name = "Disabled")]
            Default,

            [Display(Name = "Enabled")]
            Enabled,

            [Display(Name = "Enabled + Smug")]
            Smug,
        }

        public enum MenuArtEnum
        {
            [Display(Name = "Default (L7M3)")]
            Default,

            [Display(Name = "Neptune")]
            Neptune,
        }

        public enum BustupSelection
        {
            Default,
            L7M3,
            OnedCalledJay,
        }


        [Category("Bustup")]
        [DisplayName("Bustups")]
        [Description("Select your preferred dialogue bustups.")]
        [DefaultValue(BustupSelection.Default)]
        [Display(Order = 1)]
        public BustupSelection Bustup1 { get; set; }

        [Category("Misc")]
        [DisplayName("Opening Movie")]
        [Description("Changes the Opening Movie to a Kasumi version of Wake Up Get Up Get Out There (By Arbiter).")]
        [DefaultValue(true)]
        [Display(Order = 2)]
        public bool OpeningMovie { get; set; } = true;

        [Category("Misc")]
        [DisplayName("Added Thieves Den Art")]
        [Description("Adds new art to the Thieves Den based on this mod!")]
        [DefaultValue(true)]
        [Display(Order = 3)]
        public bool ThievesDenAddon { get; set; } = true;

        [Category("Misc")]
        [DisplayName("Show to Remember over Keeper of Lust")]
        [Description("Replaces Keeper of Lust with an original miniboss track by PeaMix.")]
        [DefaultValue(true)]
        [Display(Order = 4)]
        public bool MiniBossMusic { get; set; } = true;

        [Category("Misc")]
        [DisplayName("Pause Menu Art")]
        [Description("Select your preferred Pause Menu art.")]
        [DefaultValue(MenuArtEnum.Default)]
        [Display(Order = 5)]
        public MenuArtEnum MenuArt { get; set; }

        [Category("Misc")]
        [DisplayName("No All-Out-Attack Portrait")]
        [Description("Removes the All-Out-Attack finisher art. By lyncpk")]
        [DefaultValue(AoAArtEnum.Default)]
        [Display(Order = 7)]
        public AoAArtEnum AoAArt { get; set; }

        [Category("Gameplay")]
        [DisplayName("Equipment Patch")]
        [Description("Modifies Joker's equipment to better suit Violet. (By HappyGrinch08)")]
        [DefaultValue(false)]
        [Display(Order = 8)]
        public bool Equipment { get; set; } = false;

        [Category("Gameplay")]
        [DisplayName("Violet's Personas")]
        [Description("Swaps Arsene, Raoul, and Satanael to use Cendrillon, Vanadis, and Ella. (By xJustAdam)")]
        [DefaultValue(CendrillonMod.Disabled)]
        [Display(Order = 9)]
        public CendrillonMod PersonasMod { get; set; }

        [Category("Gameplay")]
        [DisplayName("Persona Skillsets")]
        [Description("Modifies affinities, stats, and skillsets to match Cendrillon/Vanadis. Recommended to use with Violet's Personas (by HappyGrinch08)")]
        [DefaultValue(false)]
        [Display(Order = 10)]
        public bool Skillset { get; set; } = false;

        [Category("Model")]
        [DisplayName("Alternate Metaverse Run Animation")]
        [Description("Replaces the metaverse run animation with her field running animation, similar to lpspectrum's mod.")]
        [DefaultValue(false)]
        [Display(Order = 11)]
        public bool AltMetaRun { get; set; } = false;

        [Category("Model")]
        [DisplayName("Costumes")]
        [Description("Enables costumes. Keep this option ENABLED unless it's causing issues.")]
        [DefaultValue(true)]
        [Display(Order = 12)]
        public bool CostumeSupport { get; set; } = true;

        [Category("Model")]
        [DisplayName("Undarkened Face")]
        [Description("Removes the face darkening when summoning a persona. Meant to be used with the No Darkened Faces mod, disable if you'd like.")]
        [DefaultValue(true)]
        [Display(Order = 13)]
        public bool DarkenedFace { get; set; } = true;

        [Category("Model")]
        [DisplayName("Blue Dress over Winter Casual")]
        [Description("Replaces the winter casual outfit with the blue dress outfit from Kasumi's SL.")]
        [DefaultValue(false)]
        [Display(Order = 14)]
        public bool BlueDress { get; set; } = false;

        [Category("Model")]
        [DisplayName("Lawson Outfit over 777 Outfit")]
        [Description("Replaces the 777 work outfit with the Lawson outfit from the December P5 Beta.")]
        [DefaultValue(false)]
        [Display(Order = 15)]
        public bool LawsonOutfit { get; set; } = false;

        [Category("Model")]
        [DisplayName("Recolored Tracksuit")]
        [Description("Replaces the gold workout outfit with a recolored Shujin Academy tracksuit outfit or a recolor by MyTamagos based on concept art.")]
        [DefaultValue(TracksuitEnum.Default)]
        [Display(Order = 16)]
        public TracksuitEnum TracksuitSelection { get; set; }

        [Category("Model")]
        [DisplayName("Ranged Weapon")]
        [Description("Choose between Violet's lever action gun or Joker's pistols.")]
        [DefaultValue(WeaponRangedEnum.LeverAction)]
        [Display(Order = 17)]
        public WeaponRangedEnum WeaponRanged { get; set; }

        [Category("Model")]
        [DisplayName("Melee Weapon")]
        [Description("Choose between Violet's rapier or Joker's daggers.")]
        [DefaultValue(MeleeRangedEnum.Rapier)]
        [Display(Order = 18)]
        public MeleeRangedEnum MeleeRanged { get; set; }

        [Category("Flowscript and BMD")]
        [DisplayName("Bathhouse Edits")]
        [Description("Contains event, flowscript, & msg edits to make the Yongen-Jaya Bath House women's only.")]
        [DefaultValue(true)]
        [Display(Order = 19)]
        public bool Bathhouse { get; set; } = true;

        [Category("Flowscript and BMD")]
        [DisplayName("Enterable Women's Bathroom")]
        [Description("Allows you to enter the women's bathroom at Shujin Academy.")]
        [DefaultValue(true)]
        [Display(Order = 20)]
        public bool Restroom { get; set; } = true;

        [Category("Events")]
        [DisplayName("Event Fixes")]
        [Description("Tweaks various events to fix issues with protag Sumi. Disable this if you're having issues with other mods that edit events.")]
        [DefaultValue(true)]
        [Display(Order = 21)]
        public bool EventEdits1 { get; set; } = true; // bool used in Mod.CS, not the folder name, but the bool name

        [Category("Events")]
        [DisplayName("Event Additions")]
        [Description("Contains major edits to select events. Disable this if you're having issues with other mods that edit events.")]
        [DefaultValue(true)]
        [Display(Order = 22)]
        public bool EventEditsBig { get; set; } = true;
    }

    /// <summary>
    /// Allows you to override certain aspects of the configuration creation process (e.g. create multiple configurations).
    /// Override elements in <see cref="ConfiguratorMixinBase"/> for finer control.
    /// </summary>
    public class ConfiguratorMixin : ConfiguratorMixinBase
    {
        // 
    }
}