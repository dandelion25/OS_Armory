using System;
using RimWorld;
using Verse;

using HarmonyLib;

namespace OS_Armory;

[StaticConstructorOnStartup]
public static class Beam_Turret_Fix
{
    static Beam_Turret_Fix()
    {
        var harmony = new Harmony("com.mjeiouws.OS_Armory.beam_turret_enabling_fix");

        // patch objective: patch VerbUtility so that its IsOverheadProjectile doesn't exclude ShootBeam, which isn't inherited from Verb_LaunchProjectile.
        var original = typeof(VerbUtility).GetMethod("ProjectileFliesOverhead");
        var postfix = typeof(Beam_Turret_Fix).GetMethod("ProjectileFliesOverhead");

        harmony.Patch(original, postfix: postfix);

    }

    public static bool ProjectileFliesOverhead(this Verb verb)
    {
        return verb.GetProjectile()?.projectile.flyOverhead ?? false;
    }

}