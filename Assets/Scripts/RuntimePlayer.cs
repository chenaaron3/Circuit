using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RuntimePlayer : MonoBehaviour
{
    float pausedSpeed;
    Vector2 pausedVelocity;

    public GameObject playObjects;
    public GameObject pauseObjects;
    public Button pause;
    public Button speedUp;
    public Button slowDown;

    bool busy = false;

    private void OnEnable()
    {
        playObjects.SetActive(true);
        pauseObjects.SetActive(false);
    }

    private void OnDisable()
    {
        Thread.OnTick -= Pause;
        Thread.OnTick -= SpeedUp;
        Thread.OnTick -= SlowDown;
        busy = false;
    }

    private void Start()
    {
        pause.onClick.AddListener(delegate
        {
            if (!busy)
            {
                busy = true;
                playObjects.SetActive(false);
                pauseObjects.SetActive(true);
                Thread.OnTick += Pause;
            }
        });
        speedUp.onClick.AddListener(delegate
        {
            if (!busy)
            {
                busy = true;
                Thread.OnTick += SpeedUp;
            }
        });
        slowDown.onClick.AddListener(delegate
        {
            if (!busy)
            {
                busy = true;
                Thread.OnTick += SlowDown;
            }
        });
    }

    public void Play(bool changeToPlayMode)
    {
        if (!busy)
        {
            Thread.instance.SetSpeed(pausedSpeed);
            Thread.instance.SetVelocity(pausedVelocity);
            Thread.instance.Run();
            if (changeToPlayMode)
            {
                playObjects.SetActive(true);
                pauseObjects.SetActive(false);
            }
        }
    }

    public void Step()
    {
        if (!busy)
        {
            Play(false);
            busy = true;
            Thread.OnTick += Pause;
        }
    }

    public void Pause()
    {
        pausedSpeed = Thread.instance.speed;
        pausedVelocity = Thread.instance.rb.velocity.normalized;
        Thread.instance.Pause();
        Thread.OnTick -= Pause;
        busy = false;
    }

    public void SpeedUp()
    {
        Thread.instance.SetSpeed(Thread.instance.speed + 1);
        Thread.instance.Run();
        Thread.OnTick -= SpeedUp;
        busy = false;
    }

    public void SlowDown()
    {
        Thread.instance.SetSpeed(Thread.instance.speed - 1);
        Thread.instance.Run();
        Thread.OnTick -= SlowDown;
        busy = false;
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (playObjects.activeSelf)
                {
                    pause.onClick.Invoke();
                }
                else
                {
                    Play(true);
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (playObjects.activeSelf)
                {
                    slowDown.onClick.Invoke();
                }
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (playObjects.activeSelf)
                {
                    speedUp.onClick.Invoke();
                }
                else if (pauseObjects.activeSelf)
                {
                    Step();
                }
            }
        }
    }
}
