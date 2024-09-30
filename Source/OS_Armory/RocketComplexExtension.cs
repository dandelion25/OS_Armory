using RimWorld;
using System.Collections.Generic;
using Verse;

namespace OS_Armory;

public class RocketComplexExtension : DefModExtension
{

    // guidance parameters
    public bool isGuided = false;

    public int guidanceTickRate = 5;

    public float guidanceActivationDistance = 0;

    public int targetLockAngle = 180; // look angle within which guidance is active/activated

    public float radiansPerTick = 0; // turning rate

    

    // smoke & flame trail effects
    public EffecterDef trailEffecter = null;

}