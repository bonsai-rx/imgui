using Hexa.NET.ImGui;
using System;
using System.Runtime.InteropServices;

namespace Bonsai.ImGui.Design;

/// <summary>
/// Provides data for the <see cref="ImGuiControl.Error"/> event.
/// </summary>
public class ErrorEventArgs : EventArgs
{
    internal unsafe ErrorEventArgs(nint ctx, nint userData, nint msg)
    {
        Context = new ImGuiContextPtr((ImGuiContext*)ctx);
        Message = Marshal.PtrToStringAnsi((IntPtr)msg);
    }

    /// <summary>
    /// Gets the ImGui context on which the error occurred.
    /// </summary>
    public ImGuiContextPtr Context { get; }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string Message { get; }
}
