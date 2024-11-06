using Unity.Entities;
using UnityEngine;

public struct AppleSpawner : IComponentData
{
    public Entity Prefab;              // Regular apple prefab
    public Entity BadApplePrefab;      // Bad apple prefab
    public float BadAppleSpawnChance;  // Probability of spawning a bad apple (0 to 1)
    public float Interval;             // Interval between spawns
}

[DisallowMultipleComponent]
public class AppleSpawnerAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject applePrefab;         // Regular apple prefab
    [SerializeField] private GameObject badApplePrefab;      // Bad apple prefab
    [SerializeField] private float appleSpawnInterval = 1f;  // Interval between spawns
    [SerializeField, Range(0f, 1f)] private float badAppleSpawnChance = 0.1f; // Spawn chance for bad apples

    private class AppleSpawnerAuthoringBaker : Baker<AppleSpawnerAuthoring>
    {
        public override void Bake(AppleSpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new AppleSpawner
            {
                Prefab = GetEntity(authoring.applePrefab, TransformUsageFlags.Dynamic),
                BadApplePrefab = GetEntity(authoring.badApplePrefab, TransformUsageFlags.Dynamic),
                Interval = authoring.appleSpawnInterval,
                BadAppleSpawnChance = authoring.badAppleSpawnChance
            });

            AddComponent(entity, new Timer { Value = authoring.appleSpawnInterval });
        }
    }
}
