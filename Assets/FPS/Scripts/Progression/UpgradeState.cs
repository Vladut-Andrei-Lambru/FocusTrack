using UnityEngine;

namespace FPSProgression
{
    // Vladut Andrei: Stores what the player unlocked (useful for debug/UML/report).
    public sealed class UpgradeState : MonoBehaviour
    {
        public bool dashUnlocked;
        public bool doubleJumpUnlocked;
        public bool killSpeedBoostUnlocked;

        public int overheatTier;      // 0..2
        public bool crit10Unlocked;   // rank 3 shooting
    }
}