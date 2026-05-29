using System.Reflection;

namespace eShop.Application.CommandsHandlers;

/// <summary>
/// 命令处理器所在程序集
/// </summary>
public static class CommandHandlersAssembly
{
    /// <summary>
    /// 获取命令处理程序程序集
    /// </summary>
    public static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
}