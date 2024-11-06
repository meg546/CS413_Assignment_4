using Unity.Entities;
using UnityEngine;

public class BadAppleAuthoring : MonoBehaviour
{
    private class BadAppleBaker : Baker<BadAppleAuthoring>
    {
        public override void Bake(BadAppleAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<BadAppleTag>(entity); // Adds the BadAppleTag to the entity
        }
    }
}
