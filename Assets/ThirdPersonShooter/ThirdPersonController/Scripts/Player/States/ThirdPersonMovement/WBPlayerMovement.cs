using UnityEngine;

namespace WeirdBrothers.ThirdPersonController
{
    public struct WBPlayerMovement : IState
    {
        private WBPlayerContext _context;

        private float _hor, _ver, _speed;
        private Vector3 _desiredMoveDirection, _forward, _right;

        public WBPlayerMovement(WBPlayerContext context)
        {
            _context = context;
            _hor = 0;
            _ver = 0;
            _speed = 0;
            _forward = Vector3.zero;
            _right = Vector3.zero;
            _desiredMoveDirection = Vector3.zero;
        }

        public void Execute()
        {
            _hor = _context.Input.Horizontal;
            _ver = _context.Input.Vertical;

            _speed = new Vector2(_hor, _ver).sqrMagnitude;

            _speed = Mathf.Clamp(_speed, 0, 1f);
            _forward = _context.PlayerCamera.transform.forward;
            _forward.y = 0f;
            _context.Speed = _speed;
            _context.Transform.rotation = Quaternion.Slerp(_context.Transform.rotation,
                                                    Quaternion.LookRotation(_forward),
                                                    _context.Data.DesireRotationSpeed);
            //_context.Transform.Rotate(new Vector3(_context.Transform.rotation.x, _context.Transform.rotation.y *_speed,_context.Transform.rotation.z));

            if (_context.isScopeOn)
            {
                _hor /= 5f;
                _ver /= 5f;
            }
            
            if (_speed > _context.Data.AllowPlayerMovement)
            {
                _context.CrossHair.CrossHairSpread = Mathf.Lerp(_context.CrossHair.CrossHairSpread,
                                                                _context.CrossHair.MaxSpread,
                                                                Time.deltaTime * 5);
                _right = _context.PlayerCamera.transform.right;

                _right.y = 0f;

                _forward.Normalize();
                _right.Normalize();

                _desiredMoveDirection = _forward * _ver + _right * _hor;
                _context.Animator.Move(_hor, _ver, _context.Data.StartAnimTime);
            }
            else
            {
                _context.CrossHair.CrossHairSpread = Mathf.Lerp(_context.CrossHair.CrossHairSpread,
                                                                _context.CrossHair.MinSpread,
                                                                Time.deltaTime * 5);
                _context.Animator.Move(0, 0, _context.Data.StopAnimTime);
            }
            _context.Controller.Move(_desiredMoveDirection * _context.Data.MoveSpeed);
        }
    }

    
}