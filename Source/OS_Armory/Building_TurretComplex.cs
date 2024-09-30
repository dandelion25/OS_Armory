using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace OS_Armory;

public class Building_TurretComplex : Building_TurretGun
{
    public override bool IsEverThreat {
        get
        {
            if(def.GetModExtension<TurretComplexExtension>().threatWhenLoaded)
            {
                return refuelableComp.HasFuel;
            }
            return true;
        }
    }

    // nabbed from the rocket turret
    public override Material TurretTopMaterial
    {
        get
        {
            if (def.GetModExtension<TurretComplexExtension>().hasLoadedGraphic)
            {
                if(refuelableComp.IsFull)
                {
                    return def.building.turretGunDef.building.turretTopLoadedMat;
                }
            }
            return def.building.turretTopMat;
        }
    }
    

    // alternative targeting
    /*public override LocalTargetInfo TryFindNewTarget()
    {

        // projectile interception
        if(def.GetModExtension<TurretComplexExtension>().isInterceptor)
        {
            int num = GenRadial.NumCellsInRadius(AttackVerb.verbProps.range);
            for (int i = 0; i < num; i++)
            {
                IntVec3 intVec = base.Position + GenRadial.RadialPattern[i];
                if (!GenSight.LineOfSight(base.Position, intVec, base.Map, skipFirstCell: true))
                {
                    continue;
                }
                List<Thing> thingList = intVec.GetThingList(base.Map);
                for (int j = 0; j < thingList.Count; j++)
                {
                    if (thingList[j] is Projectile || thingList[j] is Skyfaller)
                    {
                        return thingList[j].Position;
                    }
                }
            }
            return LocalTargetInfo.Invalid;
        }

        return base.TryFindNewTarget();
        
    }*/


}
