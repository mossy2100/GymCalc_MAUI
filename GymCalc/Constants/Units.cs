using System.ComponentModel;

namespace GymCalc.Constants;

public enum Units
{
    [Description("default")] Default,

    [Description("all")] All,

    [Description("kg")] Kilograms,

    [Description("lb")] Pounds
}
