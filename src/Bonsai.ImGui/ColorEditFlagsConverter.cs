using Hexa.NET.ImGui;
using System;
using System.ComponentModel;
using System.Linq;

namespace Bonsai.ImGui;

internal class ColorEditFlagsConverter : EnumConverter
{
    public ColorEditFlagsConverter(Type type)
        : base(type)
    {
    }

    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
        return new StandardValuesCollection(
            Enum.GetValues(typeof(ImGuiColorEditFlags))
                .Cast<ImGuiColorEditFlags>()
                .Where(flag =>
                    flag != ImGuiColorEditFlags.DefaultOptions &&
                    flag != ImGuiColorEditFlags.AlphaMask &&
                    flag != ImGuiColorEditFlags.DisplayMask &&
                    flag != ImGuiColorEditFlags.DataTypeMask &&
                    flag != ImGuiColorEditFlags.PickerMask &&
                    flag != ImGuiColorEditFlags.InputMask)
                .ToArray());
    }
}
