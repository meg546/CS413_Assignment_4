using Unity.Entities;

public struct ZigzagMovement : IComponentData
{
    public float Amplitude;   // Width of the zigzag motion
    public float Frequency;   // Speed of the zigzag motion
    public float BaseSpeed;   // Downward speed of the apple
    public float StartX;      // Initial X position for stable horizontal oscillation
}
