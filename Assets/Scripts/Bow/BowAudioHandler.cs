using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


[RequireComponent(typeof(AudioSource))]
public class BowAudioHandler : MonoBehaviour
{
    [Header("AudioClips")]
    [SerializeField] private AudioClip _pullSound;
    [SerializeField] private AudioClip _releaseSound;

    [Header("AudioSettings")]
    [SerializeField] private float _minPitch = 1.0f;
    [SerializeField] private float _maxPitch = 2.0f;
    [SerializeField] private float _releasePitchMin = 0.8f;
    [SerializeField] private float _releasePitchMax = 1.0f;

    private AudioSource _audioSource;
    private XRPullInteractable _pullInteractable;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _pullInteractable = GetComponent<XRPullInteractable>();

        if(_pullInteractable != null)
        {
            _pullInteractable.PullStarted += HandlePullStarted;
            _pullInteractable.PullUpdated += HandlePullUpdated;
            _pullInteractable.PullActionReleased += HandlePullReleased;

        }
    }

    private void OnDestroy()
    {
        if(_pullInteractable != null)
        {
            _pullInteractable.PullStarted -= HandlePullStarted;
            _pullInteractable.PullUpdated -= HandlePullUpdated;
            _pullInteractable.PullActionReleased -= HandlePullReleased;

        }
    }


    private void HandlePullStarted()
    {
        _audioSource.clip = _pullSound;
        _audioSource.loop = true;
        _audioSource.Play();
    }

    private void HandlePullUpdated(float pullAmount)
    {
        _audioSource.pitch = Mathf.Lerp(_minPitch,_maxPitch,pullAmount);

        if(pullAmount <= 0.01f && _audioSource.isPlaying)
        {
            _audioSource.Stop();
        }
    }

    private void HandlePullReleased(float pullAmount)
    {
        _audioSource.clip = _releaseSound;
        _audioSource.loop= false;
        _audioSource.pitch = Random.Range(_releasePitchMin,_releasePitchMax);
        _audioSource.Play();
    }





}
