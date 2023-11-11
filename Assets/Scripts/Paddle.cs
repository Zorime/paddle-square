using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Paddle : MonoBehaviour
{
    static readonly int
        emissionColorId = Shader.PropertyToID("_EmissionColor"),
        faceColorId = Shader.PropertyToID("_FaceColor"),
        timeOfLastHitId = Shader.PropertyToID("_TimeOfLastHit");

    [SerializeField]
    TextMeshPro scoreText;

    [SerializeField]
    MeshRenderer goalRenderer;

    [SerializeField,ColorUsage(true,true)]
    Color goalColor = Color.white;

    [SerializeField, Min(0f)]
    float
        minExtents = 4f,
        maxExtents = 4f,
        speed = 10f,
       
        maxTargetingBias = 0.75f;

    [SerializeField]
    bool isAI,isFirstPlayer;

    int score;

    float extents,targetingBias;

    Material goalMaterial,paddleMaterial,scoreMaterial;

    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }
    public bool IsAI
    {
        get { return isAI; }
        set { isAI = value; }
    }
    private void Awake()
    {
        goalMaterial = goalRenderer.material;
        goalMaterial.SetColor(emissionColorId, goalColor);
        paddleMaterial = GetComponent<MeshRenderer>().material;
        scoreMaterial = scoreText.fontMaterial;
        SetScore(0);
    }
    void SetExtents(float newExtents)
    {
        extents = newExtents;
        Vector3 s = transform.localScale;
        s.x = 2f * newExtents;
        transform.localScale = s;
    }
    void ChangeTargetingBias() => targetingBias = Random.Range(-maxTargetingBias, maxTargetingBias);
    public void Move (float target,float arenaExtents)
    {
        Vector3 p = transform.localPosition;
        p.x = isAI ? AdjustByAI(p.x, target) : isFirstPlayer ? AdjustByPlayer(p.x) : AdjustByPlayerTwo(p.x);
        float limit = arenaExtents - extents;
        p.x = Mathf.Clamp(p.x, -limit, limit);
        transform.localPosition = p;
    }
    float AdjustByAI (float x,float target)
    {
        target += targetingBias * extents;
        if(x < target)
        {
            return Mathf.Min(x + speed * Time.deltaTime, target);
        }
        return Mathf.Max(x - speed * Time.deltaTime, target);   
    }
    float AdjustByPlayer(float x)
    {
        bool goRight = Input.GetKey(KeyCode.RightArrow);
        bool goLeft = Input.GetKey(KeyCode.LeftArrow);
        if(goRight && !goLeft)
        {
            return x + speed * Time.deltaTime;
        }else if(goLeft && !goRight)
        {
            return x - speed * Time.deltaTime;
        }
        return x;
    }
    float AdjustByPlayerTwo(float x)
    {
        bool goRight = Input.GetKey(KeyCode.D);
        bool goLeft = Input.GetKey(KeyCode.A);
        if (goRight && !goLeft)
        {
            return x + speed * Time.deltaTime;
        }
        else if (goLeft && !goRight)
        {
            return x - speed * Time.deltaTime;
        }
        return x;
    }
    /// <summary>
    /// 检测是否碰撞球体
    /// </summary>
    /// <param name="ballX"></param>
    /// <param name="ballExtents"></param>
    /// <param name="hitFactor"></param>
    /// <returns></returns>
    public bool HitBall (float ballX, float ballExtents,out float hitFactor)
    {
        ChangeTargetingBias();
        //反弹程度，离中心越远程度越大
        hitFactor =
            (ballX - transform.localPosition.x) /
            //自身长度 + 球的长度（范围）
            (extents + ballExtents);
        bool success = -1 <= hitFactor && hitFactor <= 1;
        if(success)
        {
            paddleMaterial.SetFloat(timeOfLastHitId, Time.time);
        }
        return success;
    }
    public void StartNewGame()
    {
        SetScore(0);
        ChangeTargetingBias();
    }
    void SetScore(int newScore,float pointsToWin = 1000f)
    {
        score = newScore;
        scoreText.SetText("{0}", score);
        scoreMaterial.SetColor(faceColorId, goalColor * (newScore / pointsToWin));
        SetExtents(Mathf.Lerp(maxExtents, minExtents, newScore / (pointsToWin - 1f)));
    }
    public bool ScorePoint(int pointsToWin)
    {
        goalMaterial.SetFloat(timeOfLastHitId, Time.time);
        SetScore(score + 1,pointsToWin);
        return score >= pointsToWin;
    }
}
