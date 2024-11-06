using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

[UpdateAfter(typeof(TimerSystem))]
public partial struct AppleSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        // Determine if we are in the Medium or Hard level
        bool isMediumLevel = SceneManager.GetActiveScene().name == "Medium";
        bool isHardLevel = SceneManager.GetActiveScene().name == "Hard";

        // Create a random seed for spawning apples
        uint randomSeed = (uint)UnityEngine.Random.Range(1, int.MaxValue);
        new SpawnJob
        {
            ECB = ecb,
            Random = new Unity.Mathematics.Random(randomSeed),
            SpawnBadApples = isMediumLevel,
            ApplyZigzag = isHardLevel
        }.Schedule();
    }

    [BurstCompile]
    private partial struct SpawnJob : IJobEntity
    {
        public EntityCommandBuffer ECB;
        public Unity.Mathematics.Random Random;
        public bool SpawnBadApples;    // Controls bad apple spawning in Medium level
        public bool ApplyZigzag;       // Controls zigzag movement in Hard level

        private void Execute(in LocalTransform transform, in AppleSpawner spawner, ref Timer timer)
        {
            if (timer.Value > 0)
                return;

            timer.Value = spawner.Interval;

            // Decide which apple type to spawn
            Entity appleEntity = SpawnBadApples && Random.NextFloat() < spawner.BadAppleSpawnChance
                ? spawner.BadApplePrefab
                : spawner.Prefab;

            // Instantiate the chosen apple prefab and set its position
            var newApple = ECB.Instantiate(appleEntity);
            ECB.SetComponent(newApple, LocalTransform.FromPosition(transform.Position));

            // If we are in the Hard level, add the ZigzagMovement component to the apple
            if (ApplyZigzag)
            {
                float startX = transform.Position.x; // Capture the initial X position
                ECB.AddComponent(newApple, new ZigzagMovement
                {
                    Amplitude = 2.0f,   // Adjust as needed for zigzag width
                    Frequency = 3.0f,   // Adjust as needed for zigzag speed
                    BaseSpeed = 1.0f,   // Downward speed
                    StartX = startX     // Set initial X position for oscillation
                });
            }
        }
    }
}
