using Microsoft.AspNetCore.SignalR;

namespace Stocks.Realtime.Api.Realtime;

/// <summary>
/// 股票信息中心
/// </summary>
internal sealed class StocksFeedHub : Hub<IStockUpdateClient>
{
    public async Task JoinStockGroup(string ticker)
    {
        // 将股票代码作为组名
        await Groups.AddToGroupAsync(Context.ConnectionId, ticker);
    }
}