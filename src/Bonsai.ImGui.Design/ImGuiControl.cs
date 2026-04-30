using Hexa.NET.ImGui;
using Hexa.NET.ImGui.Backends.OpenGL3;
using Hexa.NET.ImGui.Backends.Win32;
using HexaGen.Runtime;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Bonsai.ImGui.Design;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents a control used to render ImGui interfaces using the OpenGL backend.
/// </summary>
public class ImGuiControl : GLControl, IGLContext
{
    static readonly object ConfigureEvent = new();
    static readonly object RenderEvent = new();
    static readonly object ErrorEvent = new();
    NativeCallback<ErrorCallback> errorCallback;
    readonly HashSet<IExtensionFactory> extensions = new();
    IExtensionContext[] extensionContexts;
    ImGuiContextPtr guiContext;
    bool disposed;
    bool resizing;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImGuiControl"/> class.
    /// </summary>
    public ImGuiControl()
    {
        GraphicsContext.ShareContexts = false;
        Size = new Size(640, 480);
        errorCallback = new NativeCallback<ErrorCallback>((ctx, userData, msg) =>
        {
            OnError(new(ctx, userData, msg));
        });
    }

    /// <summary>
    /// Gets the collection of registered extensions used to configure and render this ImGui context.
    /// </summary>
    public HashSet<IExtensionFactory> Extensions => extensions;

    /// <summary>
    /// Occurs after the ImGui context and all registered extensions have been configured.
    /// </summary>
    public event EventHandler Configure
    {
        add { Events.AddHandler(ConfigureEvent, value); }
        remove { Events.RemoveHandler(ConfigureEvent, value); }
    }

    /// <summary>
    /// Occurs when it is time to render a new frame on the ImGui context.
    /// </summary>
    public event EventHandler Render
    {
        add { Events.AddHandler(RenderEvent, value); }
        remove { Events.RemoveHandler(RenderEvent, value); }
    }

    /// <summary>
    /// Occurs when a native error is raised by the ImGui context.
    /// </summary>
    public event EventHandler<ErrorEventArgs> Error
    {
        add { Events.AddHandler(ErrorEvent, value); }
        remove { Events.RemoveHandler(ErrorEvent, value); }
    }

    /// <summary>
    /// Raises the <see cref="Configure"/> event.
    /// </summary>
    /// <param name="e">Not used.</param>
    protected virtual void OnConfigure(EventArgs e)
    {
        if (Events[ConfigureEvent] is EventHandler handler)
        {
            handler(this, e);
        }
    }

    /// <summary>
    /// Raises the <see cref="Render"/> event.
    /// </summary>
    /// <param name="e">Not used.</param>
    protected virtual void OnRender(EventArgs e)
    {
        if (Events[RenderEvent] is EventHandler handler)
        {
            handler(this, e);
        }
    }

    /// <summary>
    /// Raises the <see cref="Error"/> event.
    /// </summary>
    /// <param name="e">Provides information about the error.</param>
    protected virtual void OnError(ErrorEventArgs e)
    {
        if (Events[ErrorEvent] is EventHandler<ErrorEventArgs> handler)
        {
            handler(this, e);
        }
    }

    /// <inheritdoc/>
    protected unsafe override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        if (HasValidContext)
        {
            var parentForm = FindForm();
            parentForm.ResizeBegin += (sender, e) => resizing = true;
            parentForm.ResizeEnd += (sender, e) => resizing = false;
            parentForm.FormClosing += (sender, e) => MakeContextCurrent();

            guiContext = ImGui.CreateContext(null);
            guiContext.ErrorCallback = Marshal.GetFunctionPointerForDelegate(errorCallback.Callback).ToPointer();
            ImGui.SetCurrentContext(guiContext);
            ImGuiImplOpenGL3.SetCurrentContext(guiContext);
            ImGuiImplWin32.SetCurrentContext(guiContext);

            var io = ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;     // Enable Keyboard Controls
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;      // Enable Gamepad Controls
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;         // Enable Docking
            io.IniFilename = null;

            ImGuiImplWin32.InitForOpenGL(Handle.ToPointer());
            ImGuiImplOpenGL3.Init((string)null);

            var extensionEnumerator = extensions.GetEnumerator();
            extensionContexts = new IExtensionContext[extensions.Count];
            for (int i = 0; i < extensionContexts.Length && extensionEnumerator.MoveNext(); i++)
            {
                extensionContexts[i] = extensionEnumerator.Current.CreateContext(guiContext);
            }
            OnConfigure(EventArgs.Empty);
        }
    }

    private void MakeContextCurrent()
    {
        MakeCurrent();

        ImGui.SetCurrentContext(guiContext);
        ImGuiImplWin32.SetCurrentContext(guiContext);
        ImGuiImplOpenGL3.SetCurrentContext(guiContext);
        for (int i = 0; i < extensionContexts.Length; i++)
        {
            extensionContexts[i].MakeCurrent(guiContext);
        }
    }

    /// <inheritdoc/>
    protected override void OnPaint(PaintEventArgs e)
    {
        if (!DesignMode && HasValidContext && !resizing)
        {
            MakeContextCurrent();
            ImGuiImplOpenGL3.NewFrame();
            ImGuiImplWin32.NewFrame();
            ImGui.NewFrame();

            OnRender(EventArgs.Empty);

            ImGui.Render();
            GL.Viewport(0, 0, Width, Height);
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            ImGuiImplOpenGL3.RenderDrawData(ImGui.GetDrawData());

            SwapBuffers();
        }

        base.OnPaint(e);
    }

    /// <inheritdoc/>
    protected override void WndProc(ref Message m)
    {
        if (!disposed)
        {
            ImGuiImplWin32.SetCurrentContext(guiContext);
            if (ImGuiImplWin32.WndProcHandler(Handle, (uint)m.Msg, (nuint)m.WParam.ToInt64(), m.LParam) != 0)
                return;
        }

        base.WndProc(ref m);
    }

    /// <inheritdoc/>
    protected override void OnHandleDestroyed(EventArgs e)
    {
        if (HasValidContext && !disposed)
        {
            for (int i = extensionContexts.Length - 1; i >= 0; i--)
            {
                extensionContexts[i].Dispose();
            }

            ImGuiImplOpenGL3.Shutdown();
            ImGuiImplWin32.Shutdown();
            ImGui.SetCurrentContext(null);
            ImGui.DestroyContext(guiContext);
            errorCallback.Dispose();
            disposed = true;
        }

        base.OnHandleDestroyed(e);
    }

    bool IGLContext.IsCurrent => Context.IsCurrent;

    nint INativeContext.GetProcAddress(string procName)
    {
        return ((IGraphicsContextInternal)Context).GetAddress(procName);
    }

    bool INativeContext.IsExtensionSupported(string extensionName)
    {
        return true; // TODO*
    }

    void IGLContext.SwapInterval(int interval)
    {
        Context.SwapInterval = interval;
    }

    bool INativeContext.TryGetProcAddress(string procName, out nint procAddress)
    {
        procAddress = ((IGraphicsContextInternal)Context).GetAddress(procName);
        return procAddress != 0;
    }
}
