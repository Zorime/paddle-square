using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivelyCamera : MonoBehaviour
{
    [SerializeField, Min(0f)]
    float
        springStrength = 100f,
        dampingStrength = 10f,
        jostleStrength = 40f,
        pushStrength = 1f,
        maxDeltaTime = 1f / 60f;

    Vector3 anchorPosition, velocity;

    private void Awake()
    {
        anchorPosition = transform.localPosition;    
    }
    public void JostleY () => velocity.y += jostleStrength;

    public void PushXZ(Vector3 impulse)
    {
        velocity.x += pushStrength * impulse.x;
        velocity.z += pushStrength * impulse.y;
    }
    //һ���򵥵Ľ��������ǿ��ִ�����ʱ�������������Ǽ�С�㡣
    //ΪLivelyCamera���һ�������õ����ֵ������ΪĬ��ֵ����ʮ��֮һ��Ȼ�󽫴����LateUpdate�ƶ���һ���µ�TimeStep����������ʱ��������Ϊ������
    //��LateUpdate����TimeStep����������뵱ǰ֡������ƥ�����һ���࣬Ȼ����һ��ʹ��ʣ���������
    private void LateUpdate()
    {
       float dt = Time.deltaTime;
        while(dt > maxDeltaTime)
        {
            TimeStep(maxDeltaTime);
            dt -= maxDeltaTime;
        }
        TimeStep(dt);
    }
    void TimeStep(float dt)
    {
        //���� ��ǰλ��ָ��ê��λ��
        Vector3 displacement = anchorPosition - transform.localPosition;
        //���� ˥��
        Vector3 acceleration = springStrength * displacement - dampingStrength * velocity;
        velocity += acceleration * Time.deltaTime;
        transform.localPosition += velocity * Time.deltaTime;
    }
    
}
