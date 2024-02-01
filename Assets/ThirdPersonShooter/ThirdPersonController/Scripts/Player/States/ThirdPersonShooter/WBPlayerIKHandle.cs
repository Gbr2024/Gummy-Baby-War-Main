using UnityEngine;

namespace WeirdBrothers.ThirdPersonController
{
    public struct WBPlayerIKHandle : IState
    {
        private WBPlayerContext _context;

        public WBPlayerIKHandle(WBPlayerContext context)
        {
            _context = context;
        }

        public void Execute()
        {
            if (_context.CurrentWeapon == null)
                return;
            if (_context.CurrentWeapon.Data.WeaponType == WBWeaponType.Melee)
                return;
            if(_context.ShooterController.IsOwner)
            {
                _context.WeaponIK.Spine.LookAt(_context.WeaponIK.LookAt);
                _context.WeaponIK.Spine.Rotate(_context.WeaponIK.SpineRotation);
            }
            else if(_context.RpcLookPos!=Vector3.zero)
            {
                _context.WeaponIK.Spine.LookAt(_context.RpcLookPos);
                _context.WeaponIK.Spine.Rotate(_context.RpcSpineRotation);
            }
        }
    }
}