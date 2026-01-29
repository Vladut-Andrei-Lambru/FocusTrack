using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FPSProgression
{
    public sealed class LevelUpMenu : MonoBehaviour
    {
        [SerializeField] private TMP_Text headerText;
        [SerializeField] private TMP_Text movementTitle;
        [SerializeField] private TMP_Text movementDesc;
        [SerializeField] private TMP_Text shootingTitle;
        [SerializeField] private TMP_Text shootingDesc;
        [SerializeField] private Button movementButton;
        [SerializeField] private Button shootingButton;

        private Action<UpgradeId> _onPick;

        public void Show(RankDefinition rank, Action<UpgradeId> onPick)
        {
            _onPick = onPick;

            headerText.text = $"RANK {rank.rankNumber} UNLOCKED";
            movementTitle.text = rank.movementTitle;
            movementDesc.text = rank.movementDescription;
            shootingTitle.text = rank.shootingTitle;
            shootingDesc.text = rank.shootingDescription;

            movementButton.onClick.RemoveAllListeners();
            shootingButton.onClick.RemoveAllListeners();

            movementButton.onClick.AddListener(() => Pick(rank.movementUpgrade));
            shootingButton.onClick.AddListener(() => Pick(rank.shootingUpgrade));

            gameObject.SetActive(true);
            SetPaused(true);
        }

        private void Pick(UpgradeId id)
        {
            gameObject.SetActive(false);
            SetPaused(false);

            _onPick?.Invoke(id);
            _onPick = null;
            Debug.Log("[Vladut] Pick fired: " + id);
        }

        private static void SetPaused(bool paused)
        {
            Time.timeScale = paused ? 0f : 1f;
            Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = paused;
        }
    }
}