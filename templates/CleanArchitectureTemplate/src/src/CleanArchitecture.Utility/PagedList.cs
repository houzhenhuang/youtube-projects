using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Utility;

public class PagedList<T>
{
    private PagedList(List<T> items, int pageIndex, int pageSize, int totalCount)
    {
        Items = items;
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    /// <summary>
    /// 数据项
    /// </summary>
    public List<T> Items { get; set; }

    /// <summary>
    /// 页索引
    /// </summary>
    public int PageIndex { get; }
    
    /// <summary>
    /// 页大小
    /// </summary>
    public int PageSize { get; }
    
    /// <summary>
    /// 数据总数量
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// 是否还有下一页
    /// </summary>
    public bool HasNextPage => PageIndex * PageSize < TotalCount;

    /// <summary>
    /// 是否还有上一页
    /// </summary>
    public bool HasPreviousPage => PageIndex > 1;

    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> query, int pageIndex, int pageSize)
    {
        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

        return new(items, pageIndex, pageSize, totalCount);
    }
}