using System;

namespace BugVerification
{
    // Minimal attack definition so spawner code can run without the presentation assets.
    public class PlayTestSpearRowDefinition : AttackDefinitionBase
    {
        public override Type AttackType => typeof(SpearRowAttack);
    }
}
