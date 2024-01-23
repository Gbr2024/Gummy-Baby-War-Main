using UnityEngine;

namespace WeirdBrothers.ThirdPersonController
{
    public struct WBPlayerItemPickUp : IState
    {
        private WBPlayerContext _context;

        public WBPlayerItemPickUp(WBPlayerContext context)
        {
            _context = context;
        }

        public void Execute()
        {
            if (_context.Animator.IsSwitching())
                return;
            if (_context.Animator.IsReloading())
                return;
            if (_context.Animator.IsMeleeAttacking())
                return;
                
            if (_context.Input.GetButtonDown(WBInputKeys.Interaction) &&
                _context.CurrentPickUpItem != null)
            {
                _context.Animator.OnItemPickUp();
            }
        }
    }
}