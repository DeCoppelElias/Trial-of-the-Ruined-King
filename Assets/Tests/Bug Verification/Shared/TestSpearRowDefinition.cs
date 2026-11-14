using System;

namespace BugVerification
{
    // Minimal attack definition so spawner code can run without the presentation assets.
    // Shared by the EditMode and PlayMode bug verification tests.
    public class TestSpearRowDefinition : AttackDefinitionBase
    {
        public override Type AttackType => typeof(SpearRowAttack);
    }
}
