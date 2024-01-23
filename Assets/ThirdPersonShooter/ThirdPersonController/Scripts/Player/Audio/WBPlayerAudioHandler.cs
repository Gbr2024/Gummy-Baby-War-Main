using UnityEngine;

namespace WeirdBrothers.ThirdPersonController
{
    public class WBPlayerAudioHandler : MonoBehaviour
    {
        [SerializeField] private AudioClip _footSteps;

        private AudioSource _audioSource;
        private Rigidbody _rigidBody;
        private WBThirdPersonController _controller;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _controller = GetComponent<WBThirdPersonController>();
            _rigidBody = GetComponent<Rigidbody>();
        }

        private void FootSteps()
        {
            if (_controller.Context.Speed > 0.1f)
            {                
                _audioSource.PlayOneShotAudioClip(_footSteps);
                //Debug.Log("Play foot step audio");
            }
        }
    }
}