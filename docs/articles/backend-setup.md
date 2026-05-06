---
uid: backend-setup
---

# Backend setup

Every `Bonsai.ImGui` workflow runs against a backend: the runtime infrastructure that creates an ImGui context, advances it once per frame, and emits frame notifications that operators subscribe to in order to draw their widgets. The backend is configured by adding a backend operator to the workflow. The choice of backend operator determines which other operators are usable.

## Visualizer operators

Visualizer operators are backend operators that run an ImGui context on top of the editor [type visualizer](https://bonsai-rx.org/docs/articles/editor.html?tabs=mouse-controls#type-visualizers) infrastructure: the frame loop runs inside a docked panel created when the visualizer window is shown. Other backend operators may host the ImGui context in different ways, for example a standalone OS window. `Bonsai.ImGui.Visualizers.Design`, the recommended starter package, includes two visualizer operators:

:::workflow
![The two visualizer operators](../workflows/backend-setup-visualizer-operators.bonsai)
:::

- [`ImGuiVisualizer`] for workflows that use only ImGui operators.
- [`ImPlotVisualizer`] for workflows that also use [plotting](xref:plotting) operators. This visualizer adds the ImPlot extension to the backend so that plot operators have a valid plot context to draw into.

The two are interchangeable: the rest of the workflow does not need to know which one was chosen. Plot operators in a workflow hosted by [`ImGuiVisualizer`] will fail at runtime because ImPlot has not been initialized.

The output of the visualizer is an `IObservable<Unit>` that emits one notification per ImGui frame. The notification is the cue for widget operators downstream to call ImGui functions and submit their draw commands. The visualizer owns the ImGui context: it is created when the visualizer is first shown, kept across frames, and disposed when the visualizer closes. None of this lifecycle surfaces in the workflow.

## The frame loop

On each frame the visualizer walks through:

1. Make the ImGui context current. Multiple contexts may coexist in the same process, so this step runs even when only one is active.
2. Call `NewFrame` on the platform backend, the renderer backend, and ImGui itself. This advances the per-frame state and prepares ImGui to receive draw commands.
3. Emit a frame notification to the workflow. Subscribed widgets draw on this notification.
4. Call `ImGui.Render` to finalize the draw lists, then hand them to the renderer for painting.

The frame rate is determined by the backend and may be configurable depending on the context.

## Sharing frame notifications

A workflow with a single widget could connect the visualizer output directly to the widget. In practice almost every workflow has more than one widget, so the convention is to wrap the visualizer output in a [`PublishSubject`] and have each widget subscribe to it by name:

:::workflow
![A minimal backend setup with two widgets sharing the Frame subject](../workflows/backend-setup-minimal.bonsai)
:::

The subject in the example is named `Frame`. The name is arbitrary; pick something readable. What matters is that the frame notifications are shared through a named subject rather than connected directly. UI components can then be grouped into independent modules, each subscribed to `Frame` through its own [`SubscribeSubject`], with sections of the interface toggled on or off, replaced, or rearranged without restructuring the workflow. The same indirection lets widgets nested inside higher-order operators such as [`SelectMany`] reach the source where a direct connection cannot.

## Adding more libraries

Other immediate-mode libraries built on top of ImGui, such as [ImPlot3D](https://github.com/brenocq/implot3d) or [ImGuizmo](https://github.com/CedricGuillemet/ImGuizmo), can be integrated by registering a custom extension. See [Custom extensions](xref:custom-extensions) for the [`IExtensionContext`] and [`IExtensionFactory`] interfaces and a worked example.

<!-- Reference Style Links -->
[`ImGuiVisualizer`]: xref:Bonsai.ImGui.Visualizers.ImGuiVisualizerBuilder
[`ImPlotVisualizer`]: xref:Bonsai.ImGui.Visualizers.ImPlotVisualizerBuilder
[`PublishSubject`]: xref:Bonsai.Reactive.PublishSubject
[`SubscribeSubject`]: xref:Bonsai.Expressions.SubscribeSubject
[`SelectMany`]: xref:Bonsai.Reactive.SelectMany
[`IExtensionContext`]: xref:Bonsai.ImGui.IExtensionContext
[`IExtensionFactory`]: xref:Bonsai.ImGui.IExtensionFactory
