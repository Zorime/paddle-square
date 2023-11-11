using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }

    [SerializeField]
    GameObject menu;

    [SerializeField]
    LivelyCamera livelyCamera;

    [SerializeField]
    Ball ball;

    [SerializeField]
    Paddle bottomPaddle, topPaddle;

    [SerializeField]
    TextMeshPro countdownText;

    [SerializeField, Min(1f)]
    float newGameDelay = 3f;

    float countdownUntilNewGame;

    [SerializeField]
    Vector3 arenaExtents = new Vector2(10f, 10f);

    [SerializeField, Min(2)]
    public int pointsToWin = 3;

    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    AudioClip boundceSound,scoreSound,winSound;

    [SerializeField]
    float
        easySpeed = 3f,
        midSpeed = 5f,
        HardSpeed = 10f;
    private void Awake()
    {
        Instance = this;
        countdownUntilNewGame = newGameDelay;
        Application.targetFrameRate = 0;
        Time.timeScale = 0f;
    }
    private void Update()
    {
        bottomPaddle.Move(ball.Position.x, arenaExtents.x);
        topPaddle.Move(ball.Position.x, arenaExtents.x);
        if(countdownUntilNewGame <= 0f)
        {
            
            UpdateGame();
        }
        else
        {
            UpdateCountdown();
        }
    }
    void BounceYIfNeeded()
    {
        float yExtents = arenaExtents.y - ball.Extents;
        if(ball.Position.y < -yExtents) 
        {
            BounceY(-yExtents, bottomPaddle,topPaddle);
            PlayerSound(boundceSound);
        }
        else if(ball.Position.y > yExtents)
        {
            BounceY(yExtents, topPaddle,bottomPaddle);
            PlayerSound(boundceSound);
        }
    }
    void BounceXIfNeeded(float x)
    {

        float xExtents = arenaExtents.x - ball.Extents;
        if(x < -xExtents)
        {
            livelyCamera.PushXZ(ball.Velocity);
            ball.BounceX(-xExtents);
            PlayerSound(boundceSound);
        }
        else if(x > xExtents)
        {
            livelyCamera.PushXZ(ball.Velocity);
            ball.BounceX(xExtents);
            PlayerSound(boundceSound);
        }
    }
    void BounceY (float boundary,Paddle defender,Paddle attacker)
    {
        //反弹时间
        float durationAfterBounce = (ball.Position.y - boundary) / ball.Velocity.y;
        //触发反弹的X轴位置
        float bounceX = ball.Position.x - ball.Velocity.x * durationAfterBounce;

        BounceXIfNeeded(bounceX);
        bounceX = ball.Position.x - ball.Velocity.x * durationAfterBounce;
        livelyCamera.PushXZ(ball.Velocity);
        ball.BounceY(boundary);

        if (defender.HitBall(bounceX,ball.Extents,out float hitFactor))
        {
            ball.SetXPositionAndSpeed(bounceX, hitFactor, durationAfterBounce);
        }
        else 
        {
            livelyCamera.JostleY();
            PlayerSound(scoreSound);
            if (attacker.ScorePoint(pointsToWin))
            {
                
                EndGame();
            }
        }
    }
    void StartNewGame()
    {
        ball.StartNewGame();
        bottomPaddle.StartNewGame();
        topPaddle.StartNewGame();
    }
    void EndGame()
    {
        countdownUntilNewGame = newGameDelay;
        countdownText.SetText("GAME OVER");
        countdownText.gameObject.SetActive(true);
        PlayerSound(winSound);
        ball.EndGame();
        ReturnMenu();
    }
    void UpdateGame()
    {
        ball.Move();
        BounceYIfNeeded();
        BounceXIfNeeded(ball.Position.x);
        ball.UpdateVisualization();
    }
    void UpdateCountdown()
    {
        countdownUntilNewGame -= Time.deltaTime;
        if (countdownUntilNewGame <= 0f)
        {
            countdownText.gameObject.SetActive(false);
            StartNewGame();
        }
        else
        {
            float displayValue = Mathf.Ceil(countdownUntilNewGame);
            if (displayValue < newGameDelay)
            {
                countdownText.SetText("{0}", displayValue);
            }
        }
    }
    void PlayerSound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    void StartSingleGame()
    {
        topPaddle.IsAI = true;
        ClickButton();
    }
    public void StartDuoGame()
    {
        topPaddle.Speed = 10f;
        topPaddle.IsAI = false;
        ClickButton();
    }
    //退出游戏
    public void ExitGame()
    {
        Application.Quit();
    }
    public void SetEasy()
    {
        topPaddle.Speed = easySpeed;
        StartSingleGame();
    }
    public void SetMid()
    {
        topPaddle.Speed = midSpeed;
        StartSingleGame();
    }
    public void SetHard()
    {
        topPaddle.Speed = HardSpeed;
        StartSingleGame();
    }
    public void ClickButton()
    {
        Time.timeScale = 1;
        menu.SetActive(false);
    }
    public void ReturnMenu()
    {
        Time.timeScale = 0;
        menu.SetActive(true);
    }
}
