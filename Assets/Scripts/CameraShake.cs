// Released under CC0 and Unlicense

// Demo: https://imgur.com/a/9h3c6W5

// Screenshake should move the camera around smoothly but unpredictably. It
// shouldn't jitter the camera around in a way that makes it hard to follow
// what's on screen. That's why you should use continuous oscillating functions
// to produce your shake instead of random values. I also think it's useful to
// make a directional shake to help connect the shaking to the thing that
// caused it.

using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using UnityEngine;

using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

// A 2D shake that moves an object around with its local position --
// assumes the object's default local position was Vector3.zero!
public class CameraShake : MonoBehaviour
{
    [Tooltip("The thing to shake -- assumes it has a parent that controls the neutral position.")]
    public Transform _Target;

    [Tooltip("Set this so multiple objects shaking at the same time won't shake the same.")]
    public Vector2 _Seed;

    [Range(0.01f, 50f)]
    public float _Speed = 20f;

    [Tooltip("We won't move further than this distance from neutral.")]
    [Range(0.01f, 10f)]
    public float _MaxMagnitude = 0.3f;

    [Tooltip("0 follows _Direction exactly. 3 mostly ignores _Direction and shakes in all directions.")]
    [Range(0f, 3f)]
    public float _NoiseMagnitude = 0.3f;

    public Vector2 _Direction = Vector2.up;

    float _FadeOut = 1f;

#if UNITY_EDITOR
    void Reset()
    {
        _Target = transform;
    }
    void OnValidate()
    {
        _Direction.Normalize();
    }
#endif // UNITY_EDITOR

    void OnEnable()
    {
        //~ // Theoretically settings option so people can turn of shake if it makes them nauseous.
        //~ if (GamePreferences.IsShakeForbidden())
        //~ {
        //~     enabled = false;
        //~ }
    }

    void Update()
    {
        var sin = Mathf.Sin(_Speed * (_Seed.x + _Seed.y + Time.time));
        var direction = _Direction + GetNoise();
        direction.Normalize();
        var delta = direction * sin;
        _Target.localPosition = delta * _MaxMagnitude * _FadeOut;
    }

    [ContextMenu("Fire Once")]
    void FireOnce()
    {
        StopAllCoroutines();
        StartCoroutine(ShakeAndFade(0.5f));
    }

    [ContextMenu("Fire Continuous")]
    public void ContinuousShake()
    {
        enabled = true;
        _FadeOut = 1f;
    }

    public IEnumerator ShakeAndFade(float fade_duration)
    {
        enabled = true;
        _FadeOut = 1f;
        var fade_out_start = Time.time;
        var fade_out_complete = fade_out_start + fade_duration;
        while (Time.time < fade_out_complete)
        {
            yield return null;
            var t = 1f - Mathf.InverseLerp(fade_out_start, fade_out_complete, Time.time);
            // Pass t into an easing function from
            // https://github.com/idbrii/cs-tween/blob/main/Easing.cs to
            // make it nonlinear. CircIn is nice. See them visualized at
            // https://easings.net/
            _FadeOut = t;
        }
        enabled = false;
    }

    Vector2 GetNoise()
    {
        var time = Time.time;
        return _NoiseMagnitude
            * new Vector2(
                Mathf.PerlinNoise(_Seed.x, time) - 0.5f,
                Mathf.PerlinNoise(_Seed.y, time) - 0.5f
                );
    }

}