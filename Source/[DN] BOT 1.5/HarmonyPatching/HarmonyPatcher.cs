﻿using static HarmonyLib.AccessTools;
using More_Traits.HarmonyPatching.Patches;
using More_Traits.HarmonyPatching.Patches.Pacifist;
using More_Traits.HarmonyPatching.Patches.Eclectic;
using More_Traits.HarmonyPatching.Patches.LovesSleeping;
using HarmonyLib;
using RimWorld;
using Verse;
using More_Traits.HarmonyPatching.Patches.Hyperalgesia;
using More_Traits.HarmonyPatching.ModCompatibility;
using More_Traits.HarmonyPatching.Patches.Vulgar;
using More_Traits.HarmonyPatching.Patches.Apathetic;
using System;
using More_Traits.HarmonyPatching.Patches.Communal;
using More_Traits.HarmonyPatching.Patches.Sadist;

namespace More_Traits.HarmonyPatching
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatcher
    {
        public static float SaturatedXPMult => madSkillsSaturatedXPMult();

        private static Func<float> madSkillsSaturatedXPMult = () => 0.2f;

        static HarmonyPatcher()
        {
            Harmony harmony = new Harmony("BOT_Patcher");

            harmony.Patch(Method(typeof(SkillRecord), nameof(SkillRecord.LearnRateFactor)), prefix: new HarmonyMethod(typeof(Apathetic_LearnRateFactor), nameof(Apathetic_LearnRateFactor.LearnRateFactor)));
            harmony.Patch(Method(typeof(SkillUI), nameof(SkillUI.GetLearningFactor)), prefix: new HarmonyMethod(typeof(Apathetic_SkillUI), nameof(Apathetic_SkillUI.GetLearningFactor_Prefix)));
            harmony.Patch(Method(typeof(SkillUI), "GetSkillDescription"), prefix: new HarmonyMethod(typeof(Apathetic_SkillUI), nameof(Apathetic_SkillUI.GetSkillDescription_Marker)));

            harmony.Patch(Method(typeof(PawnDiedOrDownedThoughtsUtility), "AppendThoughts_ForHumanlike"), postfix: new HarmonyMethod(typeof(Pacifist_WittnessDeath), nameof(Pacifist_WittnessDeath.Postfix)));
            harmony.Patch(Method(typeof(PawnDiedOrDownedThoughtsUtility), "AppendThoughts_Relations"), postfix: new HarmonyMethod(typeof(Pacifist_KilledAnimal), nameof(Pacifist_KilledAnimal.Postfix)));
            harmony.Patch(Method(typeof(Pawn_InteractionsTracker), "CheckSocialFightStart"), postfix: new HarmonyMethod(typeof(Vulgar_SocialFightStart), nameof(Vulgar_SocialFightStart.Postfix)));
            harmony.Patch(Method(typeof(ThoughtUtility), "RemovePositiveBedroomThoughts"), postfix: new HarmonyMethod(typeof(Communal_RemoveThoughts), nameof(Communal_RemoveThoughts.Postfix)));
            harmony.Patch(Method(typeof(JobDriver_LayDown), "MakeNewToils"), postfix: new HarmonyMethod(typeof(LayDownJobDriver), nameof(LayDownJobDriver.MakeNewToils_PostFix)));
            harmony.Patch(PropertyGetter(typeof(Need), "IsFrozen"), postfix: new HarmonyMethod(typeof(Loves_Sleeping_IsFrozen), nameof(Loves_Sleeping_IsFrozen.Postfix)));

            harmony.Patch(Method(typeof(Recipe_RemoveBodyPart), nameof(Recipe_RemoveBodyPart.ApplyThoughts)), postfix: new HarmonyMethod(typeof(Sadist_RemoveBodyPart), nameof(Sadist_RemoveBodyPart.ApplyThoughtsPatch)));
            harmony.Patch(Method(typeof(Toils_Ingest), nameof(Toils_Ingest.FinalizeIngest)), postfix: new HarmonyMethod(typeof(EclecticPalate_FinalizeIngestPatch), nameof(EclecticPalate_FinalizeIngestPatch.Postfix)));
            harmony.Patch(Method(typeof(Pawn_StanceTracker), nameof(Pawn_StanceTracker.Notify_DamageTaken)), postfix: new HarmonyMethod(typeof(Sadist_StanceTracker), nameof(Sadist_StanceTracker.NotifyDamageTaken)));
            harmony.Patch(Method(typeof(ThoughtWorker), nameof(ThoughtWorker.MoodMultiplier)), postfix: new HarmonyMethod(typeof(Hyperalgesia_Thoughts), nameof(Hyperalgesia_Thoughts.MoodMultiplier_Post)));
            harmony.Patch(Method(typeof(TraitSet), nameof(TraitSet.GainTrait)), postfix: new HarmonyMethod(typeof(HediffTraitPatches), nameof(HediffTraitPatches.GainTrait)));
            harmony.Patch(Method(typeof(Pawn), nameof(Pawn.SpawnSetup)), postfix: new HarmonyMethod(typeof(HediffTraitPatches), nameof(HediffTraitPatches.SpawnSetup)));

            VSEPatches(harmony);
            MadSkillsPatches(harmony);
        }

        private static void MadSkillsPatches(Harmony _)
        {
            if(!ModLister.HasActiveModWithName("Mad Skills")) return;
            madSkillsSaturatedXPMult = (Func<float>)PropertyGetter(TypeByName("RTMadSkills.ModSettings"), "saturatedXPMultiplier").CreateDelegate(typeof(Func<float>));
        }

        private static void VSEPatches(Harmony harmony)
        {
            if (!ModLister.HasActiveModWithName("Vanilla Skills Expanded")) return;
            harmony.Patch(Method(TypeByName("VSE.Passions.PassionPatches"), "GenerateSkills_Prefix"), prefix: new HarmonyMethod(typeof(VSE), nameof(VSE.Skip_GenerateSkill_PrefixPatch)));
            harmony.Patch(Method(TypeByName("VSE.Passions.LearnRateFactorCache"), "LearnRateFactorBase"), prefix: new HarmonyMethod(typeof(VSE), nameof(VSE.Skip_LearnRateFactorBase)));
        }
    }
}
