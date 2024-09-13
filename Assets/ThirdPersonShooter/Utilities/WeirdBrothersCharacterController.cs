using UnityEngine;

namespace WeirdBrothers.CharacterController 
{
    //serializable classes 
    [System.Serializable]
    public class CapsuleColliderData
    {
        public bool EnableCollider = true;
        public float Radius = 0.5f;
        public float Height= 2f;
        public Vector3 Center = Vector3.up;
    }

    [System.Serializable]
    public class RigidBodyData 
    {
        public float Mass = 1f;
        public float Drag = 0f;
        public float AngularDrag = 0.05f;
    }

    [System.Serializable]
    public class GroundCheckerData 
    {
        public Transform GroundChecker;
        public float Radius = 0.1f;
        public LayerMask Layer;        
    }

    [System.Serializable]
    public class CharacterBones 
    {
        public Transform Root;

        public Transform LeftHips;
        public Transform LeftKnee;
        public Transform LeftFoot;

        public Transform RightHips;
        public Transform RightKnee;
        public Transform RightFoot;

        public Transform LeftArm;
        public Transform LeftElbow;

        public Transform RightArm;
        public Transform RightElbow;

        public Transform MiddleSpine;
        public Transform Head;
    }    

    public static class CharacterControllerHeler
    {
        public static void SetCapsuleColliderData(this CapsuleCollider collider, CapsuleColliderData data)
        {            
            collider.radius = data.Radius;
            collider.height = data.Height;
            collider.center = data.Center;
        }

        public static bool CompareCollider(this CapsuleCollider collider, CapsuleColliderData data) 
        {
            if (collider.radius!= data.Radius)
                return false;
            if (collider.height!= data.Height)
                return false;
            if (collider.center!= data.Center)
                return false;

            return true;
        }

        public static void SetRigidBodyData(this Rigidbody rb,RigidBodyData data) 
        {
            rb.mass = data.Mass;
            rb.drag = data.Drag;
            rb.angularDrag = data.AngularDrag;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        public static bool CompareRigidBody(this Rigidbody rigidbody, RigidBodyData data)
        {
            if (rigidbody.mass != data.Mass)
                return false;
            if (rigidbody.drag != data.Drag)
                return false;
            if (rigidbody.angularDrag != data.AngularDrag)
                return false;

            return true;
        }
    }
}
