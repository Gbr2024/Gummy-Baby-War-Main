using UnityEngine;
using UnityEditor;
using WeirdBrothers.CharacterController;

namespace WeirdBrothers.ThirdPersonController
{
    public class WBCharacterCharacterSetUp : Editor
    {
        [MenuItem("WeirdBrothers/Charcater Create/Setup Character", false, 0)]
        public static void SetUpCharacter()
        {
            if (Selection.activeGameObject != null && Selection.activeGameObject.transform.GetComponent<Animator>() != null)
            {
                var animator = Selection.activeGameObject.transform.GetComponent<Animator>();
                SetCharacter(animator);
            }
        }

        private static void SetCharacter(Animator animator)
        {
            var playerObject = animator.gameObject;

            var controller = playerObject.GetComponent<WBCharacterController>();
            if (controller == null)
            {
                controller = playerObject.AddComponent<WBCharacterController>();

                var colliderData = new CapsuleColliderData();
                colliderData.Height = ColliderHeight(animator);
                colliderData.Center = new Vector3(0, (float)System.Math.Round(colliderData.Height * 0.5f, 2), 0);
                colliderData.Radius = (float)System.Math.Round(colliderData.Height * 0.15f, 2);

                var rigidbodyData = new RigidBodyData();
                rigidbodyData.Mass = 50;

                var groundChecker = new GameObject();
                groundChecker.transform.position = playerObject.transform.position;
                groundChecker.transform.SetParent(playerObject.transform);
                groundChecker.name = "GroundChecker";

                var rightHandRef = new GameObject();
                rightHandRef.transform.SetParent(animator.GetBoneTransform(HumanBodyBones.RightHand));
                rightHandRef.transform.localPosition = Vector3.zero;
                rightHandRef.transform.localRotation = Quaternion.Euler(Vector3.zero);
                rightHandRef.name = "RightHandRef";

                var primarySlot1 = new GameObject();
                primarySlot1.transform.SetParent(animator.GetBoneTransform(HumanBodyBones.Spine));
                primarySlot1.transform.localPosition = Vector3.zero;
                primarySlot1.transform.localRotation = Quaternion.Euler(Vector3.zero);
                primarySlot1.name = "PrimarySlot1";

                var primarySlot2 = new GameObject();
                primarySlot2.transform.SetParent(animator.GetBoneTransform(HumanBodyBones.Spine));
                primarySlot2.transform.localPosition = Vector3.zero;
                primarySlot2.transform.localRotation = Quaternion.Euler(Vector3.zero);
                primarySlot2.name = "PrimarySlot2";

                var secondarySlot = new GameObject();
                secondarySlot.transform.SetParent(animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg));
                secondarySlot.transform.localPosition = Vector3.zero;
                secondarySlot.transform.localRotation = Quaternion.Euler(Vector3.zero);
                secondarySlot.name = "SecondarySlot";

                var meleeSlot = new GameObject();
                meleeSlot.transform.SetParent(animator.GetBoneTransform(HumanBodyBones.Spine));
                meleeSlot.transform.localPosition = Vector3.zero;
                meleeSlot.transform.localRotation = Quaternion.Euler(Vector3.zero);
                meleeSlot.name = "MeleeSlot";

                var groundCheckerData = new GroundCheckerData();
                groundCheckerData.GroundChecker = groundChecker.transform;
                groundCheckerData.Layer |= (1 << LayerMask.NameToLayer("Default"));

                controller.SetData(colliderData, rigidbodyData, groundCheckerData);

                playerObject.layer = LayerMask.NameToLayer("Player");
                playerObject.tag = "Player";

                var root = animator.GetBoneTransform(HumanBodyBones.Hips);
                var transforms = root.GetComponentsInChildren<Transform>();

                //root.layer= LayerMask.NameToLayer("PlayerBody");

                foreach (Transform transform in transforms)
                {
                    transform.gameObject.layer = LayerMask.NameToLayer("PlayerBody");
                }

                playerObject.AddComponent<WBThirdPersonController>();
                playerObject.AddComponent<WBInputHandler>();
                playerObject.AddComponent<WBPlayerAudioHandler>();
                playerObject.AddComponent<AudioSource>();
            }
            else
            {
                Debug.LogWarning("character is already setuped");
            }
        }

        private static float ColliderHeight(Animator animator)
        {
            var foot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            var hips = animator.GetBoneTransform(HumanBodyBones.Hips);
            return (float)System.Math.Round(Vector3.Distance(foot.position, hips.position) * 2f, 2);
        }
    }
}
