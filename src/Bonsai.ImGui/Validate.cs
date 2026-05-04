using Hexa.NET.ImGui;
using System;

namespace Bonsai.ImGui;

internal static class Validate
{
    public static void SliderFlags(ImGuiSliderFlags value)
    {
        CheckInvalidMask(value);
        if ((value & ImGuiSliderFlags.WrapAround) != 0)
            throw new ArgumentException(
                "The WrapAround flag is only supported by Drag controls, not Slider controls.",
                nameof(value));
    }

    public static void DragFlags(ImGuiSliderFlags value)
    {
        CheckInvalidMask(value);
    }

    private static void CheckInvalidMask(ImGuiSliderFlags value)
    {
        if ((value & ImGuiSliderFlags.InvalidMask) != 0)
            throw new ArgumentException(
                "The InvalidMask value is an internal flag and is not supported.",
                nameof(value));
    }

    public static void ColorEditFlags(ImGuiColorEditFlags value)
    {
        CheckColorEditCategory(value, ImGuiColorEditFlags.DisplayMask, "display format", "DisplayRGB, DisplayHSV, DisplayHex");
        CheckColorEditCategory(value, ImGuiColorEditFlags.InputMask, "input format", "InputRGB, InputHSV");
        CheckColorEditCategory(value, ImGuiColorEditFlags.PickerMask, "picker style", "PickerHueBar, PickerHueWheel");
        CheckColorEditCategory(value, ImGuiColorEditFlags.DataTypeMask, "data type", "Uint8, Float");
    }

    private static void CheckColorEditCategory(ImGuiColorEditFlags value, ImGuiColorEditFlags mask, string category, string options)
    {
        var bits = (int)(value & mask);
        if ((bits & (bits - 1)) != 0)
            throw new ArgumentException(
                $"Only one {category} flag ({options}) can be selected.",
                nameof(value));
    }

    public static void ArrowButtonDirection(ImGuiDir value)
    {
        if (value == ImGuiDir.None || value == ImGuiDir.Count)
            throw new ArgumentException(
                "ArrowButton requires a cardinal direction: Left, Right, Up, or Down.",
                nameof(value));
    }

    public static void NumericInputFlags(ImGuiInputTextFlags value)
    {
        const ImGuiInputTextFlags invalid =
            ImGuiInputTextFlags.EnterReturnsTrue
            | ImGuiInputTextFlags.CallbackCompletion
            | ImGuiInputTextFlags.CallbackHistory
            | ImGuiInputTextFlags.CallbackAlways
            | ImGuiInputTextFlags.CallbackCharFilter
            | ImGuiInputTextFlags.CallbackEdit
            | ImGuiInputTextFlags.CallbackResize;
        if ((value & invalid) != 0)
            throw new ArgumentException(
                "The specified flag is not supported by numeric input controls. " +
                "Supported flags include ReadOnly, NoUndoRedo, ParseEmptyRefVal, and DisplayEmptyRefVal.",
                nameof(value));
    }
}
