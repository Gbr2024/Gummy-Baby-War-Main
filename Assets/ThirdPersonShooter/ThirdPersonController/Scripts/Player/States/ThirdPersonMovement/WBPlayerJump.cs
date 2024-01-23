using UnityEngine;

namespace WeirdBrothers.ThirdPersonController
{
    public struct WBPlayerJump : IState
    {
        private WBPlayerContext _context;

        public WBPlayerJump(WBPlayerContext context)
        {
            _context = context;
        }

        public void Execute()
        {
            if (!_context.Controller.IsGrounded)
                return;

            if (_context.Input.GetButtonDown(WBInputKeys.Jump))
            {
                _context.Controller.Jump(_context.Data.JumpForce);
                _context.Animator.OnJump();
            }
        }
    }
}