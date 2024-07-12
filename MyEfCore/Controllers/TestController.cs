using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyEfCore.Entity;
using MyEfCore.MyDbContent;
using System;
using System.Reflection.Metadata;

namespace MyEfCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {




        [HttpGet]
        [Route("getList_sqlite")]
        public List<Blog> getList_sqlite()
        {
            using (var db = new SqliteContext())
            {
                var blogs = db.Blog
                    //.Where(b => b.Rating > 3)
                    //.OrderBy(b => b.Url)
                    .ToList();

                return blogs;
            }
        }

        [HttpGet]
        [Route("add_sqlite")]
        public Blog add_sqlite()
        {
            using (var db = new SqliteContext())
            {
                var blog = new Blog { Url = "http://sample.com" };
                db.Blog.Add(blog);
                db.SaveChanges();
                return blog;
            }
        }


        [HttpGet]
        [Route("del_sqlite")]
        public Blog del_sqlite(int id)
        {
            using (var db = new SqliteContext())
            {
                var blog = new Blog { BlogId =id};
                db.Blog.Remove(blog);
                db.SaveChanges();
                return blog;
            }
        }


        [HttpGet]
        [Route("edit_sqlite")]
        public Blog edit_sqlite(int id)
        {
            using (var db = new SqliteContext())
            {
                var blog = new Blog { BlogId = id, Url="被修改了" };
                db.Blog.Update(blog);
                db.SaveChanges();
                return blog;
            }
        }



        [HttpGet]
        [Route("getList")]
        public List<Blog> getList()
        {
            using (var db = new SqlserverContext())
            {
                var blogs = db.Blog
                    //.Where(b => b.Rating > 3)
                    //.OrderBy(b => b.Url)
                    .ToList();

                return blogs;
            }
        }

        [HttpGet]
        [Route("add")]
        public Blog add()
        {
            using (var db = new SqlserverContext())
            {
                var blog = new Blog { Url = "http://sample.com"};
                db.Blog.Add(blog);
                db.SaveChanges();
                return blog;
            }
        }

        /// <summary>
        /// 跟踪
        /// </summary>
        [HttpGet]
        [Route("tracking")]
        public void tracking()
        {
            using (var context = new SqlserverContext())
            {

                var blog = context.Blog.SingleOrDefault(b => b.BlogId == 1);
                blog.Rating = 5;
                context.SaveChanges();
            }
        }
        /// <summary>
        /// 没有跟踪
        /// </summary>
        [HttpGet]
        [Route("no_tracking")]
        public void no_tracking()
        {

            using (var context = new SqlserverContext())
            {
                //方式1
                var blogs = context.Blog.AsNoTracking().ToList();
                //方式2
                //context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                //var blogs = context.Blog.ToList();
            }
        }
        /// <summary>
        /// 预先加载
        /// </summary>
        [HttpGet]
        [Route("load1")]
        public List<Blog> load1()
        {

            using (var context = new SqlserverContext())
            {
                var blogs = context.Blog
                .Include(blog => blog.Posts)
                .ToList();
                return blogs;

                //带筛选
                //var filteredBlogs = context.Blog
                //.Include(
                //    blog => blog.Posts
                //        .Where(post => post.BlogId == 1)
                //        .OrderByDescending(post => post.Title)
                //        .Take(5))
                //.ToList();
            }
        }
        /// <summary>
        /// 显示加载
        /// </summary>
        [HttpGet]
        [Route("load2")]
        public Blog load2()
        {

            using (var context = new SqlserverContext())
            {
                var blog = context.Blog
                .Single(b => b.BlogId == 1);

                context.Entry(blog)
                    .Collection(b => b.Posts)
                    .Load();
                return blog;

                //聚合
                //var blog2 = context.Blog
                //.Single(b => b.BlogId == 1);

                //var postCount = context.Entry(blog)
                //    .Collection(b => b.Posts)
                //    .Query()
                //    .Count();

                //筛选
                //var blog3 = context.Blog
                //.Single(b => b.BlogId == 1);

                //var goodPosts = context.Entry(blog)
                //    .Collection(b => b.Posts)
                //    .Query()
                //    .Where(p => p.PostId > 3)
                //    .ToList();
            }
        }



        /// <summary>
        /// 拆分查询
        /// </summary>
        [HttpGet]
        [Route("spilt")]
        public void spilt()
        {

            using (var context = new SqlserverContext())
            {
                var blogs = context.Blog
                .Include(b => b.Posts)
                //.Include(b => b.Comments)
                .ToList();

                var blogs2 = context.Blog
                .Include(b => b.Posts)
                .ThenInclude(p => p.Comments)
                .ToList();

                var blogs3 = context.Blog
                .Include(blog => blog.Posts)
                .AsSplitQuery()
                .ToList();
            }
        }

        /// <summary>
        /// join
        /// </summary>
        [HttpGet]
        [Route("join")]
        public void join()
        {

            using (var context = new SqlserverContext())
            {
                var query = from blog in context.Set<Blog>()
                            join post in context.Set<Post>()
                                on blog.BlogId equals post.BlogId
                            select new { blog, post };

                var data = query.ToList();
            }
        }

        /// <summary>
        /// groupjoin
        /// </summary>
        [HttpGet]
        [Route("groupjoin")]
        public void groupjoin()
        {

            using (var context = new SqlserverContext())
            {
                var query = from b in context.Set<Blog>()
                            join p in context.Set<Post>()
                                on b.BlogId equals p.BlogId into grouping
                            select new { b, grouping };
                var data = query.ToList();
            }
        }

        /// <summary>
        /// groupby
        /// </summary>
        [HttpGet]
        [Route("groupby")]
        public void groupby()
        {

            using (var context = new SqlserverContext())
            {

                var query = from p in context.Set<Post>()
                            group p by p.BlogId into g
                    select new { g.Key, Count = g.Count() };
                var data = query.ToList();
            }
        }


        /// <summary>
        /// leftjoin
        /// </summary>
        [HttpGet]
        [Route("leftjoin")]
        public void leftjoin()
        {

            using (var context = new SqlserverContext())
            {

                var query = from b in context.Set<Blog>()
                            join p in context.Set<Post>()
                                on b.BlogId equals p.BlogId into grouping
                            from p in grouping.DefaultIfEmpty()
                            select new { b, p };
                 var data = query.ToList();
            }
        }

        /// <summary>
        /// offsetPage 偏移分页
        /// </summary>
        [HttpGet]
        [Route("offsetPage")]
        public void offsetPage()
        {

            using (var context = new SqlserverContext())
            {

                var position = 20;
                var nextPage = context.Post
                    .OrderBy(b => b.PostId)
                    .Skip(position)
                    .Take(10)
                    .ToList();
            }
        }


        /// <summary>
        /// keyPage 键集分页
        /// </summary>
        [HttpGet]
        [Route("keyPage")]
        public void keyPage()
        {

            using (var context = new SqlserverContext())
            {

                var lastId = 55;
                var nextPage = context.Post
                    .OrderBy(b => b.PostId)
                    .Where(b => b.PostId > lastId)
                    .Take(10)
                    .ToList();
            }
        }

        /// <summary>
        /// sql
        /// </summary>
        [HttpGet]
        [Route("sql")]
        public void sql()
        {

            using (var context = new SqlserverContext())
            {

                var blogs = context.Blog
                .FromSql($"SELECT * FROM dbo.Blogs")
                .ToList();
                //存储过程
                var blogs2 = context.Blog
                .FromSql($"EXECUTE dbo.GetMostPopularBlogs")
                .ToList();

                //防注入
                var user = "johndoe";
                var blogs3 = context.Blog
                    .FromSql($"EXECUTE dbo.GetMostPopularBlogsForUser {user}")
                    .ToList();
            }
        }
    }
}
