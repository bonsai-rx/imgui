# Bonsai.ImGui

`Bonsai.ImGui` exposes [Dear ImGui](https://github.com/ocornut/imgui) widgets as workflow operators, so an interactive interface can be assembled and connected directly in the workflow using the same reactive operators used elsewhere in Bonsai.

## Features

- **Interactive widgets** as workflow operators that emit notifications on user interaction: buttons, checkboxes, sliders, drags, numeric and text inputs, radio buttons, arrow buttons, color editors and pickers, combos, list boxes.
- **Containers** for laying out widgets, currently tables with row and column flow, using a begin/end scope pattern that future container types will share.
- **Images** through a producer-consumer texture-reference pattern, with `StoreImage` uploading to the GPU each frame and `Image` drawing the latest texture.
- **Plotting** via the companion `Bonsai.ImPlot` package: line, scatter, bar, digital, and stairs series, with a rolling buffer pattern for streaming data into a sliding window.
- **Extension hooks** for integrating other ImGui-based libraries such as [ImPlot3D](https://github.com/brenocq/implot3d) or [ImGuizmo](https://github.com/CedricGuillemet/ImGuizmo) through the `IExtensionContext` and `IExtensionFactory` interfaces.

## Highlights

`Bonsai.ImGui` is built around the conventions of the Bonsai workflow language:

- **Live property edits.** Most widget configuration is read on each frame, so changes in the property grid, or values set via `InputMapping`, take effect immediately without restarting the workflow.
- **Single-instance widget operators.** Every widget is a single workflow operator with frame ticks as input and events or current state as output. Multi-instance coordination is expressed in the workflow through shared subjects.
- **C# scripting as a first-class extension point.** When the package does not include a particular widget, any Dear ImGui call can be wrapped as a custom operator using a local `CSharpSource` script, without authoring a separate package. Custom operators participate in the same frame loop and property-grid model as the operators in the package.

## Additional Documentation

Visit the [documentation site](https://bonsai-rx.org/imgui) for the workflow guide, the developer guide, and API reference. Topics covered include backend setup and the per-frame loop, composing widgets with containers and texture references, coordinating shared state, plotting streaming data with [ImPlot](https://github.com/epezent/implot), authoring custom widget operators, and integrating new ImGui-based libraries through extensions.

## Installation

Install `Bonsai.ImGui.Visualizers.Design` through the Bonsai package manager. This is the recommended starter package and brings in all the runtime libraries used by the operators throughout the documentation. Workflows that need only a subset can install individual packages such as `Bonsai.ImGui` or `Bonsai.ImPlot` directly.

## Feedback & Contributing

`Bonsai.ImGui` is released as open source under the [MIT license](https://licenses.nuget.org/MIT). Bug reports and contributions are welcome at [the GitHub repository](https://github.com/bonsai-rx/imgui).
