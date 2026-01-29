using UnityEngine;
using Unity.FPS.Gameplay;

namespace FPSProgression
{
    // Vladut Andrei - fixed: prevents double jump firing on the same press as the first jump
    public sealed class DoubleJumpAbility : MonoBehaviour
    {
        [SerializeField] private int extraJumps = 1;
        [SerializeField] private float secondJumpForceMultiplier = 0.9f;

        private PlayerCharacterController _cc;
        private PlayerInputHandler _input;

        private int _jumpsLeft;
        private bool _unlocked;

        void Awake()
        {
            _cc = GetComponent<PlayerCharacterController>();
            _input = GetComponent<PlayerInputHandler>();
            enabled = false;
        }

        void Update()
        {
            if (!_unlocked) return;

            if (_cc.IsGrounded)
                _jumpsLeft = extraJumps;

            // IMPORTANT: don't allow double jump on the same frame as the first jump
            if (_cc.HasJumpedThisFrame)
                return;

            if (!_cc.IsGrounded && _jumpsLeft > 0 && _input.GetJumpInputDown())
            {
                var v = _cc.CharacterVelocity;
                v.y = 0f;
                v += Vector3.up * (_cc.JumpForce * secondJumpForceMultiplier);
                _cc.CharacterVelocity = v;

                _jumpsLeft--;
                Debug.Log("[Vladut Andrei] DOUBLE JUMP!");
            }
        }

        public void UnlockDoubleJump()
        {
            _unlocked = true;
            enabled = true;
            Debug.Log("[Vladut Andrei] DoubleJump unlocked");
        }
    }
}