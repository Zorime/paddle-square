using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField, Min(0f)]
    float
        maxSpeed = 20f,
        maxStartXSpeed = 8f,
        constantYSpeed = 10f,
        extents = 0.5f;

    [SerializeField]
    ParticleSystem bounceParticleSystem,startParticleSystem,trailParticleSystem;

    [SerializeField]
        int bounceParticleEmission = 20,
            startParticleEmission = 100;

    Vector2 position, velocity;

    public float Extents => extents;

    public Vector2 Position => position;

    public Vector2 Velocity => velocity;

    void Awake() => gameObject.SetActive(false);
    public void UpdateVisualization() => trailParticleSystem.transform.localPosition =
        transform.localPosition = new Vector3(position.x, 0, position.y);
    public void Move() => position += velocity * Time.deltaTime;

    public void StartNewGame()
    {
        position = Vector2.zero;
        UpdateVisualization();
        bool dir = (Random.value > 0.5f);
        velocity.x = Random.Range(-maxStartXSpeed, maxStartXSpeed);
        velocity.y = dir ? constantYSpeed : -constantYSpeed;
        gameObject.SetActive(true);
        startParticleSystem.Emit(startParticleEmission);
        SetTrailEmission(true);
        trailParticleSystem.Play();
    }
    public void BounceX (float boundary)
    {
        float durationAfterBounce = (position.x - boundary) / velocity.x;
        position.x = 2f * boundary - position.x;
        velocity.x = -velocity.x;
        EmitBounceParticles(
            boundary,
            position.y - velocity.y * durationAfterBounce,
            boundary < 0f ? 90f : 270f
         );
    }
    public void BounceY (float boundary)
    {
        float durationAfterBounce = (position.y - boundary) / velocity.y;
        position.y = 2f * boundary - position.y;
        velocity.y = -velocity.y;
        EmitBounceParticles(
            position.x - velocity.x * durationAfterBounce,
            boundary,
            boundary < 0f ? 0f : 180f
        );
    }
    public void SetXPositionAndSpeed(float start,float speedFactor,float deltaTime)
    {
        velocity.x = maxSpeed * speedFactor;
        position.x = start + velocity.x * deltaTime;
    }
    public void EndGame()
    {
        position.x = 0f;
        gameObject.SetActive(false);
        SetTrailEmission(false);
    }
    void EmitBounceParticles(float x,float z,float rotation)
    {
        ParticleSystem.ShapeModule shape = bounceParticleSystem.shape;
        shape.position = new Vector3(x, 0f, z);
        shape.rotation = new Vector3(0,rotation, 0);
        bounceParticleSystem.Emit(bounceParticleEmission);
    }
    void SetTrailEmission(bool enabled)
    {
        ParticleSystem.EmissionModule emisson = trailParticleSystem.emission;
        emisson.enabled = enabled;
    }
}
