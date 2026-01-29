using UnityEngine;

namespace FPSProgression
{
    public sealed class UpgradeApplier : MonoBehaviour
    {
        [SerializeField] private UpgradeState state;

        [Header("Movement scripts (on Player)")]
        [SerializeField] private DashAbility dash;
        [SerializeField] private DoubleJumpAbility doubleJump;
        [SerializeField] private KillSpeedBoostOnKill killSpeed;

        [Header("Weapon scripts (on pistol / weapon root)")]
        [SerializeField] private WeaponOverheatTuning overheatTuning;

        [Header("Crit provider (on Player)")]
        [SerializeField] private CritProvider critProvider;
        
        [SerializeField] private ProgressionHUDSimple hud;
        
        private void Start()
        {
            if (critProvider)
            {
                critProvider.SetCrit(0f, 1f);
                critProvider.SetCritEnabled(false);
            }
        }


        public void Apply(UpgradeId id)
        {
            switch (id)
            {
                // Movement
                case UpgradeId.Dash:
                    state.dashUnlocked = true;
                    if (dash) dash.UnlockDash();
                    break;

                case UpgradeId.DoubleJump:
                    state.doubleJumpUnlocked = true;
                    if (doubleJump) doubleJump.UnlockDoubleJump();
                    break;

                case UpgradeId.KillSpeedBoost:
                    state.killSpeedBoostUnlocked = true;
                    if (killSpeed) killSpeed.UnlockAdrenaline();
                    break;

                // Shooting
                case UpgradeId.OverheatTier1:
                    state.overheatTier = Mathf.Max(state.overheatTier, 1);
                    overheatTuning?.ApplyTier(state.overheatTier);
                    break;

                case UpgradeId.OverheatTier2:
                    state.overheatTier = Mathf.Max(state.overheatTier, 2);
                    overheatTuning?.ApplyTier(state.overheatTier);
                    break;

                case UpgradeId.CritChance10:
                    state.crit10Unlocked = true;
                    if (critProvider)
                    {
                        critProvider.SetCrit(0.10f, 5.0f);
                        critProvider.SetCritEnabled(true);
                    }
                    break;


            }hud?.OnUpgradeApplied(id); 
        }
        
    }
}