---
uid: input-state
---

# Input state

`Bonsai.ImGui` maintains a snapshot of input and frame state on every frame: cursor position, mouse buttons, keyboard, frame timing, capture flags. Workflows that need to react to user input or run time-aware logic can read this snapshot through the [`GetIO`] operator instead of drawing their own widgets.

[`GetIO`] sits in the same frame loop as the widget operators. Each notification on its `Frame` input emits an [`IOState`] value, which is a view of the underlying ImGui IO struct.

## Properties

- **Frame timing.** `DeltaTime`, the time in seconds since the last frame, and `Framerate`, a rolling-average estimate over the last 60 frames.
- **Mouse.** `MousePos`, `MouseDelta`, the per-button `MouseLeft` / `MouseRight` / `MouseMiddle` states, `MouseWheel`, `MouseWheelH`, and `MouseSource`.
- **Keyboard.** Modifier flags `KeyCtrl`, `KeyShift`, `KeyAlt`, `KeySuper`, plus per-key access through `Keys`, an indexable [`KeyboardState`].
- **Display.** `DisplaySize` and `DisplayFramebufferScale`.
- **Application state.** `AppFocusLost` and `AppAcceptingEvents`.
- **Capture flags.** `WantCaptureMouse`, `WantCaptureKeyboard`, `WantTextInput`. These indicate that ImGui is currently using the corresponding input. Workflows that share input with other systems should respect these flags so a typing user does not accidentally trigger a global shortcut.
- **Navigation.** `NavActive` and `NavVisible` for keyboard and gamepad navigation state.

## Keyboard-driven example

This workflow reads the current state of the spacebar on each frame:

:::workflow
![Reading the spacebar state through GetIO](../workflows/input-state-keypress.bonsai)
:::

The pipeline:

1. [`GetIO`] subscribes to the `Frame` subject and emits an [`IOState`] on each frame.
2. A [`MemberSelector`] picks the `Keys` property, returning a [`KeyboardState`].
3. An [`Index`] operator looks up the `Space` key, returning an `ImGuiKeyData` value with the per-frame state of that key.
4. A second [`MemberSelector`] extracts `DownDuration`, producing a numeric stream that represents how long the spacebar has been held.

Mouse position is read the same way, with `MousePos` instead of `Keys`. Downstream operators can use [`DistinctUntilChanged`] to react only on transitions, or apply the value to another widget through a [`PropertyMapping`].

## When to use input state versus other input sources

[`GetIO`] sees what ImGui sees, after the backend captures input. Two consequences:

- The `WantCapture*` flags are honored, so a user editing an [`InputText`] widget will see their keystrokes go to that widget rather than to a downstream workflow handler that might also be listening for the same keys.
- Input is gated by window focus, so a workflow listening through [`GetIO`] only receives input while the ImGui host window has focus.

For workflows that need raw, system-wide input regardless of whether the host has focus, packages like `Bonsai.Windows.Input` provide OS-level input observables.

> [!TIP]
> Use [`GetIO`] when the input is part of an ImGui-based interface; use the system-level alternatives when it is not.

<!-- Reference Style Links -->
[`GetIO`]: xref:Bonsai.ImGui.GetIOBuilder
[`IOState`]: xref:Bonsai.ImGui.IOState
[`KeyboardState`]: xref:Bonsai.ImGui.KeyboardState
[`MemberSelector`]: xref:Bonsai.Expressions.MemberSelectorBuilder
[`Index`]: xref:Bonsai.Expressions.IndexBuilder
[`DistinctUntilChanged`]: xref:Bonsai.Reactive.DistinctUntilChanged
[`PropertyMapping`]: xref:Bonsai.Expressions.PropertyMappingBuilder
[`InputText`]: xref:Bonsai.ImGui.InputTextBuilder
