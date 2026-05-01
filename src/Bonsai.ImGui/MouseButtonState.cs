using Hexa.NET.ImGui;
using System.Numerics;

namespace Bonsai.ImGui;

/// <summary>
/// Represents the state of a single mouse button captured from an ImGui IO snapshot.
/// </summary>
/// <param name="ptr">A pointer to the underlying ImGui IO structure.</param>
/// <param name="button">The mouse button this state refers to.</param>
public readonly struct MouseButtonState(ImGuiIOPtr ptr, ImGuiMouseButton button)
{
    private readonly ImGuiIOPtr ptr = ptr;
    private readonly ImGuiMouseButton button = button;

    /// <summary>
    /// Gets a value indicating whether the button is currently down.
    /// </summary>
    public bool Down => ptr.MouseDown[(int)button];

    /// <summary>
    /// Gets the duration the button has been held down this press, in seconds.
    /// </summary>
    /// <remarks>
    /// A value of zero indicates the button was just pressed this frame.
    /// </remarks>
    public float DownDuration => ptr.MouseDownDuration[(int)button];

    /// <summary>
    /// Gets a value indicating whether the button transitioned from up to down this frame.
    /// </summary>
    public bool Clicked => ptr.MouseClicked[(int)button];

    /// <summary>
    /// Gets the position at which the button was last pressed.
    /// </summary>
    public Vector2 ClickedPos => ptr.MouseClickedPos[(int)button];

    /// <summary>
    /// Gets a value indicating whether the button transitioned from down to up this frame.
    /// </summary>
    public bool Released => ptr.MouseReleased[(int)button];

    /// <summary>
    /// Gets a value indicating whether the button has been double-clicked this frame.
    /// </summary>
    public bool DoubleClicked => ptr.MouseDoubleClicked[(int)button];
}
