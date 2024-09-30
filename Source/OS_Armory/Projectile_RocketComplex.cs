using System.Collections.Generic;
using System;
using RimWorld;
using UnityEngine;
using Verse;
using System.Linq;

namespace OS_Armory;

public class Projectile_RocketComplex : Projectile_Explosive
{
    private IEnumerable<SubEffecterDef> effects;

    private int turnpointTicksToImpact = 0;

    private Vector3 knownTargetLoc = Vector3.zero;
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref effects, "trailEffects", null);
        Scribe_Values.Look(ref knownTargetLoc, "knownTargetLoc", intendedTarget.CenterVector3.Yto0());
        Scribe_Values.Look(ref turnpointTicksToImpact, "turnpointTicksToImpact", 0);
    }
    private Vector3 LookTowards => new Vector3(destination.x - origin.x, def.Altitude, destination.z - origin.z + ArcHeightFactor * (4f - 8f * base.DistanceCoveredFraction));
    private RocketComplexExtension props => def.GetModExtension<RocketComplexExtension>();

    private float ArcHeightFactor
    {
        get
        {
            float num = def.projectile.arcHeightFactor;
            float num2 = (destination - origin).MagnitudeHorizontalSquared();
            if (num * num > num2 * 0.2f * 0.2f)
            {
                num = Mathf.Sqrt(num2) * 0.2f;
            }
            return num;
        }
    }

    public override Quaternion ExactRotation => Quaternion.LookRotation(LookTowards);

    public override void Launch(Thing launcher, Vector3 origin, LocalTargetInfo usedTarget, LocalTargetInfo intendedTarget, ProjectileHitFlags hitFlags, bool preventFriendlyFire = false, Thing equipment = null, ThingDef targetCoverDef = null)
    {
        effects = props.trailEffecter.children.Where((SubEffecterDef effecter) => effecter.subEffecterClass == typeof(SubEffecter_SprayerChance)); //def.projectile.landedEffecter
        knownTargetLoc = intendedTarget.CenterVector3.Yto0();
        base.Launch(launcher, origin, usedTarget, intendedTarget, hitFlags, preventFriendlyFire, equipment, targetCoverDef);
    }

    public override void Tick()
    {
        base.Tick();
        if (base.Map == null)
        {
            return;
        }
        float num = ArcHeightFactor * GenMath.InverseParabola(base.DistanceCoveredFraction);
        Vector3 drawPos = DrawPos;
        Vector3 vector = drawPos + new Vector3(0f, 0f, 1f) * num;

        // implementation of effecter trail
        if (effects != null)
        {
            foreach (SubEffecterDef effect in effects)
            {
                if (effect.chancePerTick + effect.positionLerpFactor * base.DistanceCoveredFraction > Rand.Value)
                {
                    ThrowMoteTrail(vector, base.Map, Vector3.Angle(origin, vector), effect);
                }
            }
        }

        // implementation of guidance
        if (props.isGuided)
        {
            if (Vector3.Distance(origin, base.ExactPosition) > props.guidanceActivationDistance)
            {
                if (Vector3.Angle(destination - origin, intendedTarget.CenterVector3.Yto0() - origin) < props.targetLockAngle)
                    if (GenSight.LineOfSight(base.Position, intendedTarget.Cell, base.Map, skipFirstCell: true) || def.projectile.flyOverhead)
                        if (!(def.projectile.flyOverhead && base.Map.roofGrid.Roofed(intendedTarget.Cell)))
                            knownTargetLoc = intendedTarget.CenterVector3.Yto0();

                destination = base.ExactPosition.Yto0() + Vector3.RotateTowards(destination - base.ExactPosition, knownTargetLoc - base.ExactPosition, props.radiansPerTick * props.guidanceTickRate, 0).Yto0();
                origin = base.ExactPosition.Yto0() + (base.ExactPosition - destination).Yto0().normalized * (base.ExactPosition - origin).magnitude;
                //origin = base.ExactPosition.Yto0() + Vector3.RotateTowards(base.origin - base.ExactPosition, base.origin - base.ExactPosition, props.radiansPerTick * props.guidanceTickRate, 0).Yto0();
            }
        }
    }

    public void ThrowMoteTrail(Vector3 loc, Map map, float angle, SubEffecterDef effecter)
    {
        FleckCreationData dataStatic = FleckMaker.GetDataStatic(loc, map, effecter.fleckDef);
        dataStatic.scale = effecter.scale.RandomInRange;
        dataStatic.rotationRate = effecter.rotationRate.RandomInRange;
        dataStatic.velocityAngle = angle;
        dataStatic.velocitySpeed = effecter.speed.RandomInRange;
        map.flecks.CreateFleck(dataStatic);
    }
}

