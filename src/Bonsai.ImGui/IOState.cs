using Hexa.NET.ImGui;
using System.Numerics;

namespace Bonsai.ImGui
{
    /// <summary>
    /// Represents a view of the ImGui input/output state at the current frame.
    /// </summary>
    /// <param name="ptr">A pointer to the underlying ImGui IO structure.</param>
    public readonly struct IOState(ImGuiIOPtr ptr)
    {
        private readonly ImGuiIOPtr ptr = ptr;

        /// <summary>
        /// Gets the time elapsed since the last frame, in seconds.
        /// </summary>
        public float DeltaTime => ptr.DeltaTime;

        /// <summary>
        /// Gets an estimate of the application framerate, in frames per second.
        /// </summary>
        /// <remarks>
        /// Computed as a rolling average over the last 60 frames based on
        /// <see cref="DeltaTime"/>.
        /// </remarks>
        public float Framerate => ptr.Framerate;

        /// <summary>
        /// Gets the size of the main display, in pixels.
        /// </summary>
        public Vector2 DisplaySize => ptr.DisplaySize;

        /// <summary>
        /// Gets the main display scale factor.
        /// </summary>
        /// <remarks>
        /// Used for high-DPI displays where window coordinates differ from
        /// framebuffer coordinates.
        /// </remarks>
        public Vector2 DisplayFramebufferScale => ptr.DisplayFramebufferScale;

        /// <summary>
        /// Gets the mouse position change since the last frame, in pixels.
        /// </summary>
        /// <remarks>
        /// This is zero if either the current or previous position are invalid,
        /// so a disappearing or reappearing mouse will not produce a large delta.
        /// </remarks>
        public Vector2 MouseDelta => ptr.MouseDelta;

        /// <summary>
        /// Gets the current mouse position, in pixels.
        /// </summary>
        /// <remarks>
        /// Both components are set to <see cref="float.MinValue"/> when the mouse
        /// is unavailable, for example when the cursor has moved to another screen.
        /// </remarks>
        public Vector2 MousePos => ptr.MousePos;

        /// <summary>
        /// Gets the state of the left mouse button.
        /// </summary>
        public MouseButtonState MouseLeft => new(ptr, ImGuiMouseButton.Left);

        /// <summary>
        /// Gets the state of the right mouse button.
        /// </summary>
        public MouseButtonState MouseRight => new(ptr, ImGuiMouseButton.Right);

        /// <summary>
        /// Gets the state of the middle mouse button.
        /// </summary>
        public MouseButtonState MouseMiddle => new(ptr, ImGuiMouseButton.Middle);

        /// <summary>
        /// Gets the vertical mouse wheel delta accumulated this frame.
        /// </summary>
        /// <remarks>
        /// One unit scrolls about five lines of text. Positive values scroll up;
        /// negative values scroll down.
        /// </remarks>
        public float MouseWheel => ptr.MouseWheel;

        /// <summary>
        /// Gets the horizontal mouse wheel delta accumulated this frame.
        /// </summary>
        /// <remarks>
        /// Positive values scroll left; negative values scroll right.
        /// May not be filled by all backends.
        /// </remarks>
        public float MouseWheelH => ptr.MouseWheelH;

        /// <summary>
        /// Gets the input peripheral currently generating mouse events,
        /// such as a physical mouse, a touch screen, or a pen.
        /// </summary>
        public ImGuiMouseSource MouseSource => ptr.MouseSource;

        /// <summary>
        /// Gets a value indicating whether the Ctrl modifier key is currently down
        /// (Cmd on macOS).
        /// </summary>
        public bool KeyCtrl => ptr.KeyCtrl;

        /// <summary>
        /// Gets a value indicating whether the Shift modifier key is currently down.
        /// </summary>
        public bool KeyShift => ptr.KeyShift;

        /// <summary>
        /// Gets a value indicating whether the Alt modifier key is currently down.
        /// </summary>
        public bool KeyAlt => ptr.KeyAlt;

        /// <summary>
        /// Gets a value indicating whether the Super modifier key is currently down
        /// (Windows or Super on non-macOS, Ctrl on macOS).
        /// </summary>
        public bool KeySuper => ptr.KeySuper;

        /// <summary>
        /// Gets indexed access to the per-key input state.
        /// </summary>
        public KeyboardState Keys => new(ptr);

        /// <summary>
        /// Gets the touch or pen pressure of the most recent input event,
        /// in the range [0, 1].
        /// </summary>
        /// <remarks>
        /// Typically non-zero only while the left mouse button is held.
        /// </remarks>
        public float PenPressure => ptr.PenPressure;

        /// <summary>
        /// Gets a value indicating whether the application has lost focus.
        /// </summary>
        public bool AppFocusLost => ptr.AppFocusLost;

        /// <summary>
        /// Gets a value indicating whether the application is currently accepting input events.
        /// </summary>
        public bool AppAcceptingEvents => ptr.AppAcceptingEvents;

        /// <summary>
        /// Gets a value indicating whether ImGui will use mouse inputs this frame.
        /// </summary>
        /// <remarks>
        /// When true, host applications should not dispatch mouse events to other
        /// systems; mouse events should always also be passed to ImGui regardless.
        /// </remarks>
        public bool WantCaptureMouse => ptr.WantCaptureMouse;

        /// <summary>
        /// Gets a value indicating whether ImGui will use keyboard inputs this frame.
        /// </summary>
        /// <remarks>
        /// When true, host applications should not dispatch keyboard events to other
        /// systems; keyboard events should always also be passed to ImGui regardless.
        /// </remarks>
        public bool WantCaptureKeyboard => ptr.WantCaptureKeyboard;

        /// <summary>
        /// Gets a value indicating whether ImGui wants textual keyboard input.
        /// </summary>
        /// <remarks>
        /// This is set to true when an input text widget is active. On mobile or
        /// console platforms this is the cue to display an on-screen keyboard.
        /// </remarks>
        public bool WantTextInput => ptr.WantTextInput;

        /// <summary>
        /// Gets a value indicating whether ImGui has altered the mouse position
        /// and the backend should reposition the cursor on the next frame.
        /// </summary>
        /// <remarks>
        /// Rarely used; typically only set when navigation is configured to
        /// move the mouse cursor.
        /// </remarks>
        public bool WantSetMousePos => ptr.WantSetMousePos;

        /// <summary>
        /// Gets a value indicating whether keyboard or gamepad navigation is
        /// currently allowed.
        /// </summary>
        /// <remarks>
        /// Navigation is allowed when a window is focused and that window has
        /// not opted out of navigation inputs.
        /// </remarks>
        public bool NavActive => ptr.NavActive;

        /// <summary>
        /// Gets a value indicating whether the keyboard or gamepad navigation
        /// highlight is currently visible.
        /// </summary>
        public bool NavVisible => ptr.NavVisible;
    }
}
