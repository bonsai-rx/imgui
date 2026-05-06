---
uid: texture-references
---

# Texture references

Some widgets render content that lives on the GPU rather than in plain CPU memory. Images are the most common case: an image to display has to be uploaded as a GPU texture before any widget can draw it. `Bonsai.ImGui` separates this work into two operators. One uploads the image data and produces a texture reference; another consumes the reference and draws the texture as part of the per-frame rendering. The reference itself is a small handle that can be passed between operators just like any other value.

This separation appears in the [`Image`] operator, which draws inside an ImGui window, and [`PlotImage`], which draws inside a plot. The same producer-consumer split applies to any future operator that draws GPU-backed content.

## Storing an image as a texture

The [`StoreImage`] operator takes an [`IplImage`] and uploads it to a GPU texture, returning an `ImTextureRef` that points to the texture. On each input notification it updates the existing texture in place, so the GPU allocation persists across frames. The texture parameters are configurable as properties: internal format, wrap modes, and the minification and magnification filters.

The texture reference returned by [`StoreImage`] is the value other operators consume to draw the image. It is opaque to the workflow: only operators that integrate with the GPU backend can interpret it.

## Drawing the texture

[`Image`] draws a texture as a regular ImGui widget. It exposes an `Image` property of type `ImTextureRef` along with the size and UV coordinates of the displayed region. Setting `Image` via a [`PropertyMapping`] from the output of [`StoreImage`] keeps the displayed texture in sync with the latest stored image.

[`PlotImage`] is the plot-aware counterpart. It uses the same `Image` property but draws inside an ImPlot plot context, with bounds expressed in plot coordinates rather than pixels. See [Plotting](xref:plotting) for the surrounding plot setup.

## A complete workflow

A workflow that captures from a camera and displays it as an ImGui image:

:::workflow
![A camera image displayed inside an ImGui window through a texture reference](../workflows/texture-references-camera.bonsai)
:::

The workflow has two paths that meet at [`Image`]:

1. **The data path.** A [`CameraCapture`] operator emits images asynchronously from the camera. [`SampleOnNewFrame`] subscribed to the `Frame` subject samples the latest image on each frame, throttling the upload rate and dispatching the work to the rendering thread where the GPU context is current. [`StoreImage`] uploads the sampled image to its texture and emits the reference. A [`PropertyMapping`] writes the reference into the `Image` property of [`Image`].
2. **The rendering path.** [`Image`] subscribes to the `Frame` subject and, on each frame, draws the texture currently referenced by its `Image` property.

The data path keeps the texture up-to-date; the rendering path draws it. Splitting the two means the camera and the renderer are decoupled: the camera can run at its own rate without imposing on the GUI loop, and the renderer always has a valid texture to draw, even when no new image has arrived since the last frame.

<!-- Reference Style Links -->
[`Image`]: xref:Bonsai.ImGui.ImageBuilder
[`PlotImage`]: xref:Bonsai.ImPlot.PlotImageBuilder
[`StoreImage`]: xref:Bonsai.ImGui.Visualizers.StoreImage
[`SampleOnNewFrame`]: xref:Bonsai.ImGui.Visualizers.SampleOnNewFrame
[`PropertyMapping`]: xref:Bonsai.Expressions.PropertyMappingBuilder
[`CameraCapture`]: xref:Bonsai.Vision.CameraCapture
[`IplImage`]: xref:OpenCV.Net.IplImage
