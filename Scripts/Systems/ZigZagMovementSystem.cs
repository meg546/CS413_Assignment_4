using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public partial struct ZigzagMovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        float elapsedTime = (float)SystemAPI.Time.ElapsedTime;

        // Use SystemAPI.Query to query entities with ZigzagMovement and LocalTransform components
        foreach (var (transform, zigzag) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<ZigzagMovement>>())
        {
            // Calculate the horizontal zigzag offset using a sine wave around the StartX
            float horizontalOffset = zigzag.ValueRO.Amplitude * math.sin(zigzag.ValueRO.Frequency * elapsedTime);
            float3 newPosition = new float3(
                zigzag.ValueRO.StartX + horizontalOffset, 
                transform.ValueRW.Position.y - zigzag.ValueRO.BaseSpeed * deltaTime, 
                transform.ValueRW.Position.z
            );

            // Update the position
            transform.ValueRW.Position = newPosition;
        }
    }
}
