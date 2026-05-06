---
uid: plotting
---

# Plotting

[ImPlot](https://github.com/epezent/implot) is a separate library built on top of Dear ImGui that adds 2D plotting widgets. Plots are useful for visualizing numerical data, especially streaming data such as live measurements or ongoing simulations. The plotting model in ImPlot follows the same begin/end pattern as [containers](xref:containers): a plot opens a scope, series operators draw inside it, and the scope closes once all downstream operators have processed the notification.

Plotting requires the ImPlot backend to be configured. Use [`ImPlotVisualizer`] in place of [`ImGuiVisualizer`] at the top of the workflow. See [Backend setup](xref:backend-setup) for the choice between the two visualizers.

## The plot container

The [`Plot`] operator opens an ImPlot scope. It exposes the plot size, axis labels, plot flags, and per-axis flags. On each frame it emits the plot identifier inside its `BeginPlot` / `EndPlot` scope so that operators downstream draw inside the plot.

The scope mechanics are the same as for [`Table`]: the operator handles both `BeginPlot` and `EndPlot` inside a single notification handler, emitting between the two. Operators connected downstream of [`Plot`] therefore draw inside the open plot.

## Drawing series

Series operators draw a single curve, scatter, bar, or other plot element inside the open plot. The package provides two layers of series operators:

- **Single-series operators** in `Bonsai.ImPlot` consume an [`IPlotPointGetter`] source, drawing one series per instance: [`PlotLine`], [`PlotScatter`], [`PlotBars`], [`PlotDigital`], [`PlotStairs`], and [`PlotText`].
- **Multi-series operators** in `Bonsai.ImGui.Visualizers` consume an [`IPlotPointGetterSeries`] source, drawing all named series within the source on a single notification: [`PlotLineSeries`], [`PlotScatterSeries`], [`PlotBarsSeries`], [`PlotDigitalSeries`], and [`PlotStairsSeries`].

Most workflows use the multi-series operators, paired with the [`RollingBuffer`] operator to maintain a sliding window of named columns over a streaming source. The single-series operators are a closer fit when the data is already a fixed sequence of points, such as a precomputed curve.

## Streaming data into a plot

Plotting in this package is structured around a producer-consumer split, similar to [texture references](xref:texture-references). The producer side prepares the data, often buffering streaming values into a window of points. The consumer side, the series operator, draws the buffered data inside the plot scope on each frame.

The two sides are connected through a named subject published by the plot:

- The [`Plot`] operator publishes its frame notifications to a subject named after the plot.
- A [`LatestOnNewFrame`] operator subscribed to that subject samples the latest buffered data on each plot frame, propagating it to the series operators downstream.
- The series operators draw inside the plot scope using the latest sample.

This indirection lets the data path and the rendering path run at independent rates: the producer can buffer data continuously, and the renderer pulls the latest snapshot whenever the plot is drawn.

## A streaming line plot

The following workflow plots the X and Y position of the mouse cursor over a 1000-sample sliding window:

:::workflow
![A streaming line plot of mouse cursor position](../workflows/plotting-mouse-trace.bonsai)
:::

The pipeline:

1. **The plot path.** [`ImPlotVisualizer`] is upstream of [`Plot`], which opens the plot scope. [`Plot`] publishes its frame notification to the `Plot` subject.
2. **The data path.** A [`MouseMove`] operator emits cursor positions asynchronously. [`RollingBuffer`], with `ValueSelector` set to `X,Y`, accumulates the most recent 1000 positions as a multi-series source with two named series, `X` and `Y`. [`LatestOnNewFrame`] subscribed to the `Plot` subject samples the buffered data on each plot frame.
3. **The render path.** [`PlotLineSeries`] receives the sampled buffer inside the plot scope and draws both `X` and `Y` series as labeled lines.

The data and render paths meet at [`PlotLineSeries`]. The buffer keeps the data window up-to-date as the cursor moves; the named-subject indirection ensures the lines are drawn at the plot rate, regardless of how fast the cursor is moving.

## Plotting images

Image data can be drawn inside a plot using [`PlotImage`], which integrates with the same texture-reference pattern as the regular [`Image`] operator. See [Texture references](xref:texture-references) for the producer-consumer split.

<!-- Reference Style Links -->
[`ImPlotVisualizer`]: xref:Bonsai.ImGui.Visualizers.ImPlotVisualizerBuilder
[`ImGuiVisualizer`]: xref:Bonsai.ImGui.Visualizers.ImGuiVisualizerBuilder
[`Plot`]: xref:Bonsai.ImPlot.PlotBuilder
[`Table`]: xref:Bonsai.ImGui.TableBuilder
[`PlotLine`]: xref:Bonsai.ImPlot.PlotLine
[`PlotScatter`]: xref:Bonsai.ImPlot.PlotScatter
[`PlotBars`]: xref:Bonsai.ImPlot.PlotBars
[`PlotDigital`]: xref:Bonsai.ImPlot.PlotDigital
[`PlotStairs`]: xref:Bonsai.ImPlot.PlotStairs
[`PlotText`]: xref:Bonsai.ImPlot.PlotText
[`PlotLineSeries`]: xref:Bonsai.ImGui.Visualizers.PlotLineSeriesBuilder
[`PlotScatterSeries`]: xref:Bonsai.ImGui.Visualizers.PlotScatterSeriesBuilder
[`PlotBarsSeries`]: xref:Bonsai.ImGui.Visualizers.PlotBarsSeriesBuilder
[`PlotDigitalSeries`]: xref:Bonsai.ImGui.Visualizers.PlotDigitalSeriesBuilder
[`PlotStairsSeries`]: xref:Bonsai.ImGui.Visualizers.PlotStairsSeriesBuilder
[`RollingBuffer`]: xref:Bonsai.ImGui.Visualizers.RollingBufferBuilder
[`LatestOnNewFrame`]: xref:Bonsai.ImGui.Visualizers.LatestOnNewFrame
[`Image`]: xref:Bonsai.ImGui.ImageBuilder
[`PlotImage`]: xref:Bonsai.ImPlot.PlotImageBuilder
[`MouseMove`]: xref:Bonsai.Windows.Input.MouseMove
[`IPlotPointGetter`]: xref:Bonsai.ImPlot.IPlotPointGetter
[`IPlotPointGetterSeries`]: xref:Bonsai.ImGui.Visualizers.IPlotPointGetterSeries
