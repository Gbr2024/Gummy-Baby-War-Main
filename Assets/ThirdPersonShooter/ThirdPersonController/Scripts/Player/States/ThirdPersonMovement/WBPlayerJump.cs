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
            if (!_context.Controller.IsGrounded && _context.jumpindex>1)
                return;
            else if(_context.Controller.IsGrounded)
            {
                _context.jumpindex = 0;
            }

            if (_context.Input.GetButtonDown(WBInputKeys.Jump))
            {
                _context.jumpindex++;
                _context.Controller.Jump(_context.Data.JumpForce);
                _context.Animator.OnJump();
            }
        }
    }
}