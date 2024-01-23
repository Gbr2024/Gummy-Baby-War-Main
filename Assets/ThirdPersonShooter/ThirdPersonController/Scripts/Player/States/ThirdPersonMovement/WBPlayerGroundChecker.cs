using UnityEngine;

namespace WeirdBrothers.ThirdPersonController
{
	public struct WBPlayerGroundChecker : IState
	{
		private WBPlayerContext _context;
	
		public WBPlayerGroundChecker(WBPlayerContext context)
		{
			_context = context;
		}
	
		public void Execute()
		{
			_context.Animator.OnGround(_context.Controller.IsGrounded,
                                        _context.Controller.GroundDistance);			
		}
	}
}