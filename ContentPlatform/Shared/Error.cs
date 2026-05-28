namespace Shared;

public record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static readonly Error NullValue = new("Error.NullValue","指定的结果值为null。");

    public static readonly Error ConditionNotMet= new("Error.ConditionNotMet", "未满足指定的条件。");
}