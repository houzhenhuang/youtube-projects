using Blogs.Api.Database;

namespace Blogs.Api.Seed;

public class SeedDatabase(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BlogsDbContext>();
        var any = context.BlogPosts.Any();
        if (any)
        {
            return;
        }

        // 添加100篇博客数据 
        var posts = new List<BlogPost>
        {
            new BlogPost
            {
                Slug = "/ultimate-ai-guide",
                Date = "2025-08-12",
                Title = "Ultimate AI Guide",
                Excerpt = "This is a comprehensive overview of ultimate ai guide. Discover key insights, practical examples, and expert advice.",
                Content = @" <h1>Ultimate AI Guide</h1>
        <p>Welcome to this detailed guide on ultimate ai guide.</p>
        <p>Whether you are a beginner or an experienced professional, this article covers everything you need to know to succeed in the field.</p>
        <section>
        <h2>Key Takeaways</h2>
        <ul>
        <li>Practical strategies</li>
        <li>Real-world examples</li>
        <li>Future trends</li>
        </ul>
        </section>
        <!-- Sample content for testing purposes -->
        "
            },
            new BlogPost
            {
                Slug = "/essential-seo-tips",
                Date = "2025-11-05",
                Title = "Essential SEO Tips",
                Excerpt = "This is a comprehensive overview of essential seo tips. Discover key insights, practical examples, and expert advice.",
                Content = @" <h1>Essential SEO Tips</h1>
        <p>Welcome to this detailed guide on essential seo tips.</p>
        <p>Whether you are a beginner or an experienced professional, this article covers everything you need to know to succeed in the field.</p>
        <section>
        <h2>Key Takeaways</h2>
        <ul>
        <li>Practical strategies</li>
        <li>Real-world examples</li>
        <li>Future trends</li>
        </ul>
        </section>
        <!-- Sample content for testing purposes -->
        "
            },
            new BlogPost
            {
                Slug = "/beginner-programming-tutorial",
                Date = "2024-06-28",
                Title = "Beginner Programming Tutorial",
                Excerpt = "This is a comprehensive overview of beginner programming tutorial. Discover key insights, practical examples, and expert advice.",
                Content = @" <h1>Beginner Programming Tutorial</h1>
        <p>Welcome to this detailed guide on beginner programming tutorial.</p>
        <p>Whether you are a beginner or an experienced professional, this article covers everything you need to know to succeed in the field.</p>
        <section>
        <h2>Key Takeaways</h2>
        <ul>
        <li>Practical strategies</li>
        <li>Real-world examples</li>
        <li>Future trends</li>
        </ul>
        </section>
        <!-- Sample content for testing purposes -->
        "
            },
            new BlogPost
            {
                Slug = "/advanced-web-development-strategies",
                Date = "2024-03-18",
                Title = "Advanced Web Development Strategies",
                Excerpt = "This is a comprehensive overview of advanced web development strategies. Discover key insights, practical examples, and expert advice.",
                Content = @" <h1>Advanced Web Development Strategies</h1>
        <p>Welcome to this detailed guide on advanced web development strategies.</p>
        <p>Whether you are a beginner or an experienced professional, this article covers everything you need to know to succeed in the field.</p>
        <section>
        <h2>Key Takeaways</h2>
        <ul>
        <li>Practical strategies</li>
        <li>Real-world examples</li>
        <li>Future trends</li>
        </ul>
        </section>
        <!-- Sample content for testing purposes -->
        "
            },
            new BlogPost
            {
                Slug = "/complete-machine-learning-handbook",
                Date = "2025-04-02",
                Title = "Complete Machine Learning Handbook",
                Excerpt = "This is a comprehensive overview of complete machine learning handbook. Discover key insights, practical examples, and expert advice.",
                Content = @" <h1>Complete Machine Learning Handbook</h1>
        <p>Welcome to this detailed guide on complete machine learning handbook.</p>
        <p>Whether you are a beginner or an experienced professional, this article covers everything you need to know to succeed in the field.</p>
        <section>
        <h2>Key Takeaways</h2>
        <ul>
        <li>Practical strategies</li>
        <li>Real-world examples</li>
        <li>Future trends</li>
        </ul>
        </section>
        <!-- Sample content for testing purposes -->
        "
            },
            new BlogPost
            {
                Slug = "/practical-data-science-tools",
                Date = "2024-09-15",
                Title = "Practical Data Science Tools",
                Excerpt = "This is a comprehensive overview of practical data science tools. Discover key insights, practical examples, and expert advice.",
                Content = @" <h1>Practical Data Science Tools</h1>
        <p>Welcome to this detailed guide on practical data science tools.</p>
        <p>Whether you are a beginner or an experienced professional, this article covers everything you need to know to succeed in the field.</p>
        <section>
        <h2>Key Takeaways</h2>
        <ul>
        <li>Practical strategies</li>
        <li>Real-world examples</li>
        <li>Future trends</li>
        </ul>
        </section>
        <!-- Sample content for testing purposes -->
        "
            },
            new BlogPost
            {
                Slug = "/2026-cybersecurity-roadmap",
                Date = "2025-12-20",
                Title = "2026 Cybersecurity Roadmap",
                Excerpt = "This is a comprehensive overview of 2026 cybersecurity roadmap. Discover key insights, practical examples, and expert advice.",
                Content = @" <h1>2026 Cybersecurity Roadmap</h1>
        <p>Welcome to this detailed guide on 2026 cybersecurity roadmap.</p>
        <p>Whether you are a beginner or an experienced professional, this article covers everything you need to know to succeed in the field.</p>
        <section>
        <h2>Key Takeaways</h2>
        <ul>
        <li>Practical strategies</li>
        <li>Real-world examples</li>
        <li>Future trends</li>
        </ul>
        </section>
        <!-- Sample content for testing purposes -->
        "
            },
            new BlogPost
            {
                Slug = "/top-cloud-computing-trends",
                Date = "2025-02-07",
                Title = "Top Cloud Computing Trends",
                Excerpt = "This is a comprehensive overview of top cloud computing trends. Discover key insights, practical examples, and expert advice.",
                Content = @" <h1>Top Cloud Computing Trends</h1>
        <p>Welcome to this detailed guide on top cloud computing trends.</p>
        <p>Whether you are a beginner or an experienced professional, this article covers everything you need to know to succeed in the field.</p>
        <section>
        <h2>Key Takeaways</h2>
        <ul>
        <li>Practical strategies</li>
        <li>Real-world examples</li>
        <li>Future trends</li>
        </ul>
        </section>
        <!-- Sample content for testing purposes -->
        "
            },
            new BlogPost
            {
                Slug = "/best-blockchain-framework",
                Date = "2024-11-30",
                Title = "Best Blockchain Framework",
                Excerpt = "This is a comprehensive overview of best blockchain framework. Discover key insights, practical examples, and expert advice.",
                Content = @" <h1>Best Blockchain Framework</h1>
        <p>Welcome to this detailed guide on best blockchain framework.</p>
        <p>Whether you are a beginner or an experienced professional, this article covers everything you need to know to succeed in the field.</p>
        <section>
        <h2>Key Takeaways</h2>
        <ul>
        <li>Practical strategies</li>
        <li>Real-world examples</li>
        <li>Future trends</li>
        </ul>
        </section>
        <!-- Sample content for testing purposes -->
        "
            },
            new BlogPost
            {
                Slug = "/hidden-mobile-apps-secrets",
                Date = "2025-07-22",
                Title = "Hidden Mobile Apps Secrets",
                Excerpt = "This is a comprehensive overview of hidden mobile apps secrets. Discover key insights, practical examples, and expert advice.",
                Content = @" <h1>Hidden Mobile Apps Secrets</h1>
        <p>Welcome to this detailed guide on hidden mobile apps secrets.</p>
        <p>Whether you are a beginner or an experienced professional, this article covers everything you need to know to succeed in the field.</p>
        <section>
        <h2>Key Takeaways</h2>
        <ul>
        <li>Practical strategies</li>
        <li>Real-world examples</li>
        <li>Future trends</li>
        </ul>
        </section>
        <!-- Sample content for testing purposes -->
        "
            },
            new BlogPost
            {
                Slug = "/comprehensive-devops-best-practices",
                Date = "2024-04-10",
                Title = "Comprehensive DevOps Best Practices",
                Excerpt = "This is a comprehensive overview of comprehensive devops best practices. Discover key insights, practical examples, and expert advice.",
                Content = @" <h1>Comprehensive DevOps Best Practices</h1>
        <p>Welcome to this detailed guide on comprehensive devops best practices.</p>
        <p>Whether you are a beginner or an experienced professional, this article covers everything you need to know to succeed in the field.</p>
        <section>
        <h2>Key Takeaways</h2>
        <ul>
        <li>Practical strategies</li>
        <li>Real-world examples</li>
        <li>Future trends</li>
        </ul>
        </section>
        <!-- Sample content for testing purposes -->
        "
            },
            new BlogPost
            {
                Slug = "/modern-uiux-design-handbook",
                Date = "2025-01-15",
                Title = "Modern UI/UX Design Handbook",
                Excerpt = "This is a comprehensive overview of modern ui/ux design handbook. Discover key insights, practical examples, and expert advice.",
                Content = @" <h1>Modern UI/UX Design Handbook</h1>
        <p>Welcome to this detailed guide on modern ui/ux design handbook.</p>
        <p>Whether you are a beginner or an experienced professional, this article covers everything you need to know to succeed in the field.</p>
        <section>
        <h2>Key Takeaways</h2>
        <ul>
        <li>Practical strategies</li>
        <li>Real-world examples</li>
        <li>Future trends</li>
        </ul>
        </section>
        <!-- Sample content for testing purposes -->
        "
            },
        };

        context.BlogPosts.AddRange(posts);
        await context.SaveChangesAsync(stoppingToken);
    }
}