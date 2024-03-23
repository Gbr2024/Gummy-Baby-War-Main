using UnityEngine;

namespace WeirdBrothers.ThirdPersonController
{
    public struct WBPlayerIKHandle : IState
    {
        private WBPlayerContext _context;

        Vector3 moderation;

        public WBPlayerIKHandle(WBPlayerContext context)
        {
            _context = context;
            moderation =Vector3.zero;
        }

        public void Execute()
        {
            if (_context.CurrentWeapon == null && !_context.GrenadeSet)
                return;
            if (_context.CurrentWeapon != null)
                if (_context.CurrentWeapon.Data.WeaponType == WBWeaponType.Melee)
                    return;

            if(_context.ShooterController.IsOwner)
            {
                moderation = _context.WeaponIK.SpineRotation;
                if (_context.isScopeOn) moderation *= _context.ScopeOnRatio;
                _context.WeaponIK.Spine.LookAt(_context.WeaponIK.LookAt);
                _context.WeaponIK.Spine.Rotate(moderation);
            }
            else if(_context.RpcLookPos!=Vector3.zero)
            {
                _context.WeaponIK.Spine.LookAt(_context.RpcLookPos);
                _context.WeaponIK.Spine.Rotate(_context.RpcSpineRotation);
            }
        }
    }
}