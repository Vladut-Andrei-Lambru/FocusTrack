using UnityEngine;

namespace FPSProgression
{
    [CreateAssetMenu(menuName = "Progression/Rank Definition", fileName = "RankDefinition")]
    public class RankDefinition : ScriptableObject
    {
        public int rankNumber = 1;

        [Header("Movement Choice")]
        public UpgradeId movementUpgrade;
        public string movementTitle;
        [TextArea] public string movementDescription;

        [Header("Shooting Choice")]
        public UpgradeId shootingUpgrade;
        public string shootingTitle;
        [TextArea] public string shootingDescription;
    }
}