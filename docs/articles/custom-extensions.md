---
uid: custom-extensions
---

# Custom extensions

Several libraries are built on top of Dear ImGui to add specialized functionality: [ImPlot](https://github.com/epezent/implot) for 2D plotting, [ImPlot3D](https://github.com/brenocq/implot3d) for 3D plotting, [ImGuizmo](https://github.com/CedricGuillemet/ImGuizmo) for 3D manipulators, and others. ImPlot is already integrated into `Bonsai.ImGui`. In ImGui terminology these libraries are called extensions, distinguishing them from backends, which are the rendering and platform layers such as OpenGL, DirectX, SDL, and Win32 that paint the ImGui draw lists onto the screen. Each extension has its own per-frame state that needs to be initialized alongside the ImGui context, kept current as part of the rendering loop, and disposed when the visualizer closes.

Two interfaces in `Bonsai.ImGui`, [`IExtensionContext`] and [`IExtensionFactory`], describe the lifecycle of a custom extension. The contract is host-agnostic: any operator that creates an ImGui context and walks a set of extension factories can register them, with the visualizers being the current example. This article walks through the contract, uses the existing ImPlot extension as a worked example, and sketches how to apply the same pattern to integrate a new library.

## The extension contract

[`IExtensionFactory`] creates extension contexts. The visualizer calls it once when the ImGui context is first created, passing a handle to the ImGui context. The factory returns an [`IExtensionContext`].

[`IExtensionContext`] manages the per-extension lifecycle. It extends `IDisposable`, so the implementation owns release of any extension resources. Specifically, the implementation must:

- Create the extension contexts in its constructor, given the ImGui context handle.
- Implement `MakeCurrent`, which the visualizer calls on each frame to set the extension context current alongside the ImGui context. This is needed because libraries that maintain their own current-context concept will lose it whenever ImGui is made current.
- Implement `Dispose` from `IDisposable` to release the extension resources when the visualizer closes.

Multiple extensions can be registered against the same visualizer. They are initialized in declaration order and disposed in reverse.

## Worked example: ImPlot

`Bonsai.ImPlot` integrates the ImPlot library through this extension mechanism. The full implementation, [`ImPlotExtension`], is fewer than fifty lines:

```csharp
public class ImPlotExtension : IExtensionContext
{
    readonly ImPlotContextPtr plotContext;

    public ImPlotExtension(ImGuiContextPtr guiContext)
    {
        ImPlot.SetImGuiContext(guiContext);
        plotContext = ImPlot.CreateContext();
        ImPlot.SetCurrentContext(plotContext);
        ImPlot.StyleColorsLight(ImPlot.GetStyle());
    }

    public void Dispose()
    {
        ImPlot.SetCurrentContext(null);
        ImPlot.DestroyContext(plotContext);
    }

    public void MakeCurrent(ImGuiContextPtr guiContext)
    {
        ImPlot.SetCurrentContext(plotContext);
        ImPlot.SetImGuiContext(guiContext);
    }

    public static IExtensionFactory Factory => ImPlotExtensionFactory.Default;

    class ImPlotExtensionFactory : IExtensionFactory
    {
        internal static readonly ImPlotExtensionFactory Default = new();
        public IExtensionContext CreateContext(ImGuiContextPtr guiContext) => new ImPlotExtension(guiContext);
    }
}
```

The constructor links the ImPlot library to the ImGui context, creates the ImPlot context, makes it current, and applies the light color style. `MakeCurrent` re-applies both context handles every frame so that `ImPlot.*` calls find the right state. `Dispose` tears down the ImPlot context. The static `Factory` property exposes a singleton factory that is registered on a visualizer.

## Registering an extension on a visualizer

The visualizer side of the integration is small. `Bonsai.ImGui.Design` provides an abstract base [`ImGuiMashupVisualizer`] with a `GetExtensions` method that the subclass overrides to declare which factories the visualizer should configure. The corresponding workflow operator, used in workflows, derives from [`ImGuiMashupVisualizerBuilder`].

The plot variant is a one-line override:

```csharp
public class ImPlotVisualizer : ImGuiMashupVisualizer
{
    protected override IEnumerable<IExtensionFactory> GetExtensions()
    {
        yield return ImPlotExtension.Factory;
    }
}

[TypeVisualizer(typeof(ImPlotVisualizer))]
[Description("Configures an immediate mode visualizer backend for ImPlot.")]
public class ImPlotVisualizerBuilder : ImGuiMashupVisualizerBuilder
{
}
```

The `[TypeVisualizer]` attribute pairs the workflow operator with the visualizer class. When the workflow runs, the operator publishes its frame notifications and the visualizer initializes ImGui, calls the `CreateContext` method on each factory, and walks the per-frame `MakeCurrent` and `Dispose` calls automatically.

## Integrating a new library

To add an extension for a new library, such as ImPlot3D or ImGuizmo:

1. Write an `IExtensionContext` implementation that wraps the library context lifecycle. The constructor receives the ImGui context; create any library contexts the library needs and make them current. Implement `MakeCurrent` to re-apply the contexts each frame and `Dispose` to clean them up.
2. Provide an `IExtensionFactory`, typically a singleton private class with a static `Default` instance, that returns new context instances.
3. Subclass [`ImGuiMashupVisualizer`] and override `GetExtensions` to yield the factory. Pair it with a subclass of [`ImGuiMashupVisualizerBuilder`] annotated with `[TypeVisualizer(typeof(YourVisualizer))]`.

The new visualizer can then be added to workflows in the same way as [`ImGuiVisualizer`] and [`ImPlotVisualizer`]. Operators that draw using the new library work inside the visualizer frame notifications without any extra connections.

<!-- Reference Style Links -->
[`IExtensionContext`]: xref:Bonsai.ImGui.IExtensionContext
[`IExtensionFactory`]: xref:Bonsai.ImGui.IExtensionFactory
[`ImPlotExtension`]: xref:Bonsai.ImPlot.ImPlotExtension
[`ImGuiMashupVisualizer`]: xref:Bonsai.ImGui.Design.ImGuiMashupVisualizer
[`ImGuiMashupVisualizerBuilder`]: xref:Bonsai.ImGui.Design.ImGuiMashupVisualizerBuilder
[`ImGuiVisualizer`]: xref:Bonsai.ImGui.Visualizers.ImGuiVisualizerBuilder
[`ImPlotVisualizer`]: xref:Bonsai.ImGui.Visualizers.ImPlotVisualizerBuilder
