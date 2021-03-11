using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    private const float MaxTimer = 1.0f;
    public float timer = MaxTimer;

    public Transform Target { get; protected set; } = null;
    private Transform player = null;

    private IEnumerator IE_Countdown = null;
    private Action unRegister = null;

    private Quaternion tRot = Quaternion.identity;
    private Vector3 tPos = Vector3.zero;
    private object gamebject;

    public void Register(Transform target, Transform player, Action unRegister)
    {
        this.Target = target;
        this.player = player;
        this.unRegister = unRegister;

        StartCoroutine(RotateToTheTarget());
        StartTimer();
    }

    IEnumerator RotateToTheTarget()
    {

        while (enabled)
        {
            if (Target)
            {
                tPos = Target.position;
                tRot = Target.rotation;
            }

            Vector3 direction = player.position - tPos;

            tRot = Quaternion.LookRotation(direction);
            tRot.z = -tRot.y;
            tRot.x = 0;
            tRot.y = 0;

            Vector3 northDirection = new Vector3(0, 0, player.eulerAngles.y);
            GetComponent<RectTransform>().localRotation = tRot * Quaternion.Euler(northDirection);

            yield return null;
        }
    }

    private void StartTimer()
    {
        if (IE_Countdown != null)
        {

            StopCoroutine(IE_Countdown);
        }

        IE_Countdown = Countdown();
        StartCoroutine(IE_Countdown);
    }

    public void Restart()
    {
        timer = MaxTimer;
        StartTimer();
    }

    private IEnumerator Countdown()
    {
        while (GetComponent<CanvasGroup>().alpha < 1.0f)
        {
            GetComponent<CanvasGroup>().alpha += 4 * Time.deltaTime;
            yield return null;
        }

        while (timer > 0)
        {
            timer--;
            yield return new WaitForSeconds(1);
        }

        while (GetComponent<CanvasGroup>().alpha > 0.0f)
        {
            GetComponent<CanvasGroup>().alpha -= 2 * Time.deltaTime;
            yield return null;
        }

        unRegister();
        Destroy(gameObject);
    }
}
