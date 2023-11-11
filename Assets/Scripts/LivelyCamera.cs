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
    //一个简单的解决方案是强制执行最大时间增量，但不是极小点。
    //为LivelyCamera添加一个可配置的最大值，设置为默认值的六十分之一。然后将代码从LateUpdate移动到一个新的TimeStep方法，并将时间增量作为参数。
    //让LateUpdate调起TimeStep，最大增量与当前帧增量的匹配次数一样多，然后再一次使用剩余的增量。
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
        //方向 当前位置指向锚点位置
        Vector3 displacement = anchorPosition - transform.localPosition;
        //弹簧 衰减
        Vector3 acceleration = springStrength * displacement - dampingStrength * velocity;
        velocity += acceleration * Time.deltaTime;
        transform.localPosition += velocity * Time.deltaTime;
    }
    
}
