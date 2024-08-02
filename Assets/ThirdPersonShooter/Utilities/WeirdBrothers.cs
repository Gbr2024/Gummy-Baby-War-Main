using UnityEngine;

namespace WeirdBrothers 
{
    public interface IDamageable
    {
        void ApplyDamage(float damage, Vector3 damagePoint);
    }

    public static class CommonFunctions
    {
        public static void PlayOneShotAudioClip(this AudioSource source, AudioClip clip)
        {
            if (source.isPlaying) return;
            source.clip = null;
            if (source.loop) source.loop = false;
            source.Stop();
            if (source && clip)
            {
                source.PlayOneShot(clip);
            }
        }

        public static void PlayLoopAudioClip(this AudioSource source, AudioClip clip)
        {
            if (!source.loop) source.loop = true;
            if (source && clip)
            {
                source.clip = clip;
                source.Play();
            }
        }

        public static void StopAudioClip(this AudioSource source)
        {
            source.Stop();
            source.clip = null;
        }       

        public static float GetHitAngle(this Transform transform, Vector3 hitpoint, bool normalized = true)
        {
            var localTarget = transform.InverseTransformPoint(hitpoint);
            var _angle = (int)(Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg);

            if (!normalized) return _angle;

            if (_angle <= 45 && _angle >= -45)
                _angle = 0;
            else if (_angle > 45 && _angle < 135)
                _angle = 90;
            else if (_angle >= 135 || _angle <= -135)
                _angle = 180;
            else if (_angle < -45 && _angle > -135)
                _angle = -90;

            return _angle;
        }

        public static void ApplyDamage(this GameObject gameObject, float damage, Vector3 damagePoint)
        {
            IDamageable damageable = gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.ApplyDamage(damage, damagePoint);
            }
            else
            {
                damageable = gameObject.GetComponentInParent<IDamageable>();
                if (damageable != null)
                {
                    damageable.ApplyDamage(damage, damagePoint);
                }
            }
        }

        public static bool IsInLayerMask(this GameObject obj, LayerMask layerMask)
        {
            return ((layerMask.value & (1 << obj.layer)) > 0);
        }

        public static Transform GetActiveChildTransform(this Transform transform)
        {
            foreach (Transform trans in transform) 
            {
                if (trans.gameObject.activeSelf)
                    return trans;
            }
            return null;
        }

        public static void LookAtTargetWithoutChaningY(this Transform transform, Vector3 pos) 
        {
            Vector3 targetPos = pos;
            targetPos.y = transform.position.y;
            transform.LookAt(targetPos);
        }        
    }
}

