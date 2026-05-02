using Hexa.NET.ImGui;
using System;

namespace Bonsai.ImGui;

internal static class Validate
{
    public static void SliderFlags(ImGuiSliderFlags value)
    {
        if ((value & ImGuiSliderFlags.WrapAround) != 0)
            throw new ArgumentException(
                "The WrapAround flag is only supported by Drag controls, not Slider controls.",
                nameof(value));
    }
}
