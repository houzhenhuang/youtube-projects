namespace Blogs.Api.Database;

public class BlogPost
{
    /// <summary>
    /// URL slug 是 URL 網址中最後一個反斜杠（backslash）後的部分。
    /// 例如，您現正閱讀這篇文章的 URL網址是：「https://www.storeberry.ai/hk/blog/url-slug-important-for-seo」，而 URL 網址的 slug 是「/url-slug-important-for-seo」。
    /// </summary>
    public string Slug { get; set; }

    public string Date { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 摘要
    /// </summary>
    public string Excerpt { get; set; }

    public string Content { get; set; }
}