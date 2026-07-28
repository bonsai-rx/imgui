namespace Bonsai.ImGui.Visualizers;

class CircularBuffer<T>(int capacity) : RollingBuffer<T>(capacity)
{
    public override ref T this[int index]
    {
        get => ref buffer[index % buffer.Length];
    }

    public int End => end;
}
