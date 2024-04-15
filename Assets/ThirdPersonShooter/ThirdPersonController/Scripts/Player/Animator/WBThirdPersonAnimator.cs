using System;
using UnityEngine;

namespace WeirdBrothers.ThirdPersonController
{
    public class WBThirdPersonAnimator
    {
        internal Animator _animator;

        //animation ids        
        private int _animIDHor;
        private int _animIDVer;
        private int _animIDFreeMovement;
        private int _animIDJump;
        private int _animIDGroundDistance;
        private int _animIDIsGrounded;
        private int _animIDItemPickUp;
        private int _animIDIsSwitching;
        private int _animIDWeaponIndex;
        private int _animIDReload;
        private int _animIDIsReload;
        private int _animIDMeleeAttack;
        private int _animIDIsMeleeAttacking;

        private int _animIDSwitch1;
        private int _animIDSwitch2;
        private int _animIDSwitch3;
        private int _animIDSwitch4;
        private int _animIDSwitch5;
        private int _animIDSwitch6;
        private int _animIDSwitch7;
        private int _animIDSwitch8;
        private int _animIDSwitch9;
        private int _animIDSwitch10;
        private int _animIDSwitch11;
        private int _animIDSwitch12;
        private int _animIDSwitch13;
        private int _animIDSwitch14;
        private int _animIDSwitch15;
        private int _animIDSwitch16;

        public WBThirdPersonAnimator(Transform transform)
        {
            _animator = transform.GetComponent<Animator>();

            _animIDHor = Animator.StringToHash("Hor");
            _animIDVer = Animator.StringToHash("Ver");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeMovement = Animator.StringToHash("FreeMovement");
            _animIDGroundDistance = Animator.StringToHash("GroundDistance");
            _animIDIsGrounded = Animator.StringToHash("IsGrounded");
            _animIDItemPickUp = Animator.StringToHash("ItemPickUp");
            _animIDIsSwitching = Animator.StringToHash("IsSwitching");
            _animIDWeaponIndex = Animator.StringToHash("WeaponIndex");
            _animIDReload = Animator.StringToHash("Reload");
            _animIDIsReload = Animator.StringToHash("IsReload");
            _animIDMeleeAttack = Animator.StringToHash("MeleeAttack");
            _animIDIsMeleeAttacking = Animator.StringToHash("IsMeleeAttacking");

            _animIDSwitch1 = Animator.StringToHash("Switch1");
            _animIDSwitch2 = Animator.StringToHash("Switch2");
            _animIDSwitch3 = Animator.StringToHash("Switch3");
            _animIDSwitch4 = Animator.StringToHash("Switch4");
            _animIDSwitch5 = Animator.StringToHash("Switch5");
            _animIDSwitch6 = Animator.StringToHash("Switch6");
            _animIDSwitch7 = Animator.StringToHash("Switch7");
            _animIDSwitch8 = Animator.StringToHash("Switch8");
            _animIDSwitch9 = Animator.StringToHash("Switch9");
            _animIDSwitch10 = Animator.StringToHash("Switch10");
            _animIDSwitch11 = Animator.StringToHash("Switch11");
            _animIDSwitch12 = Animator.StringToHash("Switch12");
            _animIDSwitch13 = Animator.StringToHash("Switch13");
            _animIDSwitch14 = Animator.StringToHash("Switch14");
            _animIDSwitch15 = Animator.StringToHash("Switch15");
            _animIDSwitch16 = Animator.StringToHash("Switch16");
        }

        public void Move(float hor, float ver, float dampTime)
        {
            _animator.SetFloat(_animIDVer, ver, dampTime, Time.deltaTime);
            _animator.SetFloat(_animIDHor, hor, dampTime, Time.deltaTime);
        }

        public void OnGround(bool state, float distance)
        {
            _animator.SetBool(_animIDIsGrounded, state);
            _animator.SetFloat(_animIDGroundDistance, distance);
        }

        public void OnJump()
        {
            _animator.SetTrigger(_animIDJump);
        }

        public void OnItemPickUp()
        {
            _animator.SetTrigger(_animIDItemPickUp);
        }

        public void WeaponSwitch(int index = 1)
        {
            if (index == 1)
            {
                _animator.SetTrigger(_animIDSwitch1);
            }
            else if (index == 2)
            {
                _animator.SetTrigger(_animIDSwitch2);
            }
            else if (index == 3)
            {
                _animator.SetTrigger(_animIDSwitch3);
            }
            else if (index == 4)
            {
                _animator.SetTrigger(_animIDSwitch4);
            }
            else if (index == 5)
            {
                _animator.SetTrigger(_animIDSwitch5);
            }
            else if (index == 6)
            {
                _animator.SetTrigger(_animIDSwitch6);
            }
            else if (index == 7)
            {
                _animator.SetTrigger(_animIDSwitch7);
            }
            else if (index == 8)
            {
                _animator.SetTrigger(_animIDSwitch8);
            }
            else if (index == 9)
            {
                _animator.SetTrigger(_animIDSwitch9);
            }
            else if (index == 10)
            {
                _animator.SetTrigger(_animIDSwitch10);
            }
            else if (index == 11)
            {
                _animator.SetTrigger(_animIDSwitch11);
            }
            else if (index == 12)
            {
                _animator.SetTrigger(_animIDSwitch12);
            }
            else if (index == 13)
            {
                _animator.SetTrigger(_animIDSwitch13);
            }
            else if (index == 14)
            {
                _animator.SetTrigger(_animIDSwitch14);
            }
            else if (index == 15)
            {
                _animator.SetTrigger(_animIDSwitch15);
            }
            else if (index == 16)
            {
                _animator.SetTrigger(_animIDSwitch16);
            }
        }

        internal void SetAim(bool v)
        {
            _animator.SetBool("Aiming", v);
            _animator.SetFloat("Aim", v?1f:0);

            
        }

        internal bool isAimOn()
        {
            //if (_animator.GetBool("AimOn") != v) _animator.SetBool("AimOn", v);
            return _animator.GetBool("AimOn");
        }

        public bool IsSwitching()
        {
            return _animator.GetBool(_animIDIsSwitching);
        }

        public void OnSwitch(bool state)
        {
            _animator.SetBool(_animIDIsSwitching, state);
        }

        public void SetWeaponIdex(int index)
        {
            _animator.SetFloat(_animIDWeaponIndex, index);
        }

        public void setLeftHand(Vector3 position, Quaternion rotation)
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            _animator.SetIKPosition(AvatarIKGoal.LeftHand, position);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            _animator.SetIKRotation(AvatarIKGoal.LeftHand, rotation);
        }


        public void OnReload()
        {
            _animator.SetTrigger(_animIDReload);
        }

        public void SetReload(bool state)
        {
            _animator.SetBool(_animIDIsReload, state);
        }

        public bool IsReloading()
        {
            return _animator.GetBool(_animIDIsReload);
        }

        

        public void OnMeleeAttack()
        {
            _animator.SetTrigger(_animIDMeleeAttack);
        }

        public bool IsMeleeAttacking()
        {
            return _animator.GetBool(_animIDIsMeleeAttacking);
        }
    }
}