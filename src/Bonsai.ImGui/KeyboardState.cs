using Hexa.NET.ImGui;
using System;

namespace Bonsai.ImGui;

/// <summary>
/// Provides indexed access to the per-key input state from an ImGui IO snapshot.
/// </summary>
/// <param name="ptr">A pointer to the underlying ImGui IO structure.</param>
public readonly struct KeyboardState(ImGuiIOPtr ptr)
{
    private readonly ImGuiIOPtr ptr = ptr;

    /// <summary>
    /// Gets the input state for the specified named key.
    /// </summary>
    /// <param name="key">
    /// The key to query. Must be a named key in the range
    /// [<see cref="ImGuiKey.NamedKeyBegin"/>, <see cref="ImGuiKey.NamedKeyEnd"/>).
    /// </param>
    /// <returns>The <see cref="ImGuiKeyData"/> describing the current state of the key.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="key"/> is not a named key.
    /// </exception>
    public ImGuiKeyData this[ImGuiKey key]
    {
        get
        {
            if (key < ImGuiKey.NamedKeyBegin || key >= ImGuiKey.NamedKeyEnd)
                ThrowKeyOutOfRange(key);
            return ptr.KeysData[key - ImGuiKey.NamedKeyBegin];
        }
    }

    private static void ThrowKeyOutOfRange(ImGuiKey key) =>
        throw new ArgumentOutOfRangeException(nameof(key), key, "Key must be a named key.");
}
