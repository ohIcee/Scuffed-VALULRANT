using UnityEngine;
using Mirror;

public class FirstPersonAudio : NetworkBehaviour
{
    [SerializeField] private FirstPersonMovement character;
    [SerializeField] private GroundCheck groundCheck;

    [Header("Step")]
    [SerializeField] private AudioSource stepAudio;
    public float velocityThreshold = .01f; // Minimum velocity for the step audio to play

    [Header("Landing")]
    [SerializeField] private AudioSource landingAudio;
    [SerializeField] private AudioClip[] landingSFX;

    [Header("Jump")]
    [SerializeField] private AudioSource jumpAudio;
    [SerializeField] private AudioClip[] jumpSFX;

    Vector2 lastCharacterPosition;
    Vector2 CurrentCharacterPosition => new Vector2(character.transform.position.x, character.transform.position.z);

    void OnEnable()
    {
        if (!hasAuthority) return;

        // Subscribe to events.
        groundCheck.Grounded += PlayLandingAudio;
        character.Jumped += PlayJumpAudio;
    }

    void OnDisable()
    {
        if (!hasAuthority) return;

        // Unsubscribe to events.
        groundCheck.Grounded -= PlayLandingAudio;
        character.Jumped -= PlayJumpAudio;
    }

    void FixedUpdate()
    {
        if (!hasAuthority) return;

        // Play moving audio if the character is moving and on the ground.
        float velocity = Vector3.Distance(CurrentCharacterPosition, lastCharacterPosition);
        if (velocity >= velocityThreshold && groundCheck.isGrounded)
        {
            if (!character.IsShifting && !character.IsCrouchPressed)
            {
                UpdateMovingAudios(stepAudio);
            }
            else UpdateMovingAudios(null, stepAudio);
        }
        else
            UpdateMovingAudios(null, stepAudio);
        lastCharacterPosition = CurrentCharacterPosition;
    }


    static void UpdateMovingAudios(AudioSource audioToPlay, params AudioSource[] audiosToPause)
    {
        // Play audio to play and pause the others.
        if (audioToPlay && !audioToPlay.isPlaying)
            audioToPlay.Play();
        foreach (var audio in audiosToPause)
            if (audio)
                audio.Pause();
    }

    void PlayLandingAudio() => PlayRandomClip(landingAudio, landingSFX);

    void PlayJumpAudio() => PlayRandomClip(jumpAudio, jumpSFX);

    void PlayRandomClip(AudioSource audio, AudioClip[] clips)
    {
        if (!audio || clips.Length <= 0)
            return;

        // Get a random clip. If possible, make sure that it's not the same as the clip that is already on the audiosource.
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        if (clips.Length > 1)
            while (clip == audio.clip)
                clip = clips[Random.Range(0, clips.Length)];

        // Play the clip.
        audio.clip = clip;
        audio.Play();
    }
}