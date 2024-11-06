using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial struct CollectAppleSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerScore>();
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Check if the main menu is active and exit early if it is
        if (SceneManager.GetActiveScene().name == "MainMenu") return;

        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var appleCount = new NativeArray<byte>(1, Allocator.TempJob);

        state.Dependency = new CollisionJob
        {
            AppleLookup = SystemAPI.GetComponentLookup<AppleTag>(true),
            BadAppleLookup = SystemAPI.GetComponentLookup<BadAppleTag>(true),
            BasketLookup = SystemAPI.GetComponentLookup<BasketTag>(true),
            ECB = ecb,
            AppleCount = appleCount
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        state.Dependency.Complete();

        // Update score if a regular apple was caught
        if (appleCount[0] == 1)
        {
            var playerScore = SystemAPI.GetSingleton<PlayerScore>();
            playerScore.Value += 100;
            SystemAPI.SetSingleton(playerScore);
        }

        appleCount.Dispose();
    }

    [BurstCompile]
    private struct CollisionJob : ICollisionEventsJob
    {
        [ReadOnly] public ComponentLookup<AppleTag> AppleLookup;
        [ReadOnly] public ComponentLookup<BadAppleTag> BadAppleLookup;
        [ReadOnly] public ComponentLookup<BasketTag> BasketLookup;

        public EntityCommandBuffer ECB;
        public NativeArray<byte> AppleCount;

        public void Execute(CollisionEvent collisionEvent)
        {
            var entityA = collisionEvent.EntityA;
            var entityB = collisionEvent.EntityB;

            // Check for regular apple collision with basket
            if (AppleLookup.HasComponent(entityA) && BasketLookup.HasComponent(entityB))
            {
                ECB.DestroyEntity(entityA);
                AppleCount[0] = 1;
            }
            else if (AppleLookup.HasComponent(entityB) && BasketLookup.HasComponent(entityA))
            {
                ECB.DestroyEntity(entityB);
                AppleCount[0] = 1;
            }
            // Check for bad apple collision with basket
            else if (BadAppleLookup.HasComponent(entityA) && BasketLookup.HasComponent(entityB))
            {
                ECB.DestroyEntity(entityA);  // Destroy the bad apple
                ECB.DestroyEntity(entityB);  // Destroy the basket
            }
            else if (BadAppleLookup.HasComponent(entityB) && BasketLookup.HasComponent(entityA))
            {
                ECB.DestroyEntity(entityB);  // Destroy the bad apple
                ECB.DestroyEntity(entityA);  // Destroy the basket
            }
        }
    }
}
