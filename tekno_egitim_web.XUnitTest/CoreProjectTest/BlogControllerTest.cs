using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tekno_egitim_web.core.Model;
using tekno_egitim_web.core.Repository;
using tekno_egitim_web.coreproject.Controllers;
using tekno_egitim_web.data;
using Xunit;

namespace tekno_egitim_web.XUnitTest.CoreProjectTest
{

    public class BlogControllerTest
    {
        private readonly Mock<IRepository<Blog>> _mock;
        private readonly BlogsController _controller;
        private List<Blog> blogs;

        public BlogControllerTest()
        {
            _mock = new Mock<IRepository<Blog>>();
            _controller = new BlogsController(_mock.Object);
            blogs = new List<Blog>()
            {
                new Blog
                {
                    blog_id = 1,
                    baslik = "testdeneme12",
                    aciklama = "testaciklama1",
                    olusturulma = DateTime.Now,
                    imageUrl = "",
                    kategori_id = 1,
                    blog_silme = false
                },
                new Blog
                {
                    blog_id = 2,
                    baslik = "testdeneme22",
                    aciklama = "testaciklama2222",
                    olusturulma = DateTime.Now,
                    imageUrl = "",
                    kategori_id = 1,
                    blog_silme = false
                }
            };
        }
        [Fact]
        public async void Index_ActionExecutes_ReturnView()
        {
            var result = await _controller.Index();
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public async void Index_ActionExecutes_ReturnBlogList()
        {
            //_mock.Setup(repository => repository.GetAllAsync(blogs));
            var result = await _controller.Index();
            var viewresult = Assert.IsType<ViewResult>(result);
            var bloglist = Assert.IsAssignableFrom<IEnumerable<Blog>>(viewresult.Model);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
        [Fact]
        public async void Details_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Details(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Create", redirect.ActionName);
        }
        [Fact]
        public async void Detais_IdInvalidId_ReturnNotFound()
        {
            Blog blog = null;
            _mock.Setup(x => x.GetByIdAsync(0)).ReturnsAsync(blog);
            var result = await _controller.Details(0);
            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal<int>(404, redirect.StatusCode);
        }
        [Theory]
        [InlineData(1)]
        public async void Details_ValidIdReturnBlog(int blogid)
        {
            Blog blog = blogs.First(x => x.blog_id == blogid);
            //_mock.Setup(repository => repository.GetByIdAsync(blog));
            var result = await _controller.Details(blogid);
            var viewresult = Assert.IsType<ViewResult>(result);
            var resultblog = Assert.IsAssignableFrom<Blog>(viewresult.Model);
            Assert.Equal(blog.blog_id, resultblog.blog_id);
            Assert.Equal(blog.baslik, resultblog.baslik);
        }
        [Fact]
        public void Create_ActionExecutes_ReturnView()
        {
            var result = _controller.Create();
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public async void CreatePost_InvalidModelState_ReturnView()
        {
            _controller.ModelState.AddModelError("Baslik", "Baslik Alani gereklidir.");
            var result = await _controller.Create(blogs.First());
            var viewresult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Blog>(viewresult.Model);
        }
        [Fact]
        public async void CreatePost_ValidModelState_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Create(blogs.First());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

        }
        [Fact]
        public async void CreatePost_ValidModelState_CreateMethodExecute()
        {
            Blog newblog = null;
            //_mock.Setup(repository => repository.Create(It.IsAny<Blog>())).Callback<Blog>(x => newblog = x);
            var result = await _controller.Create(blogs.First());
            //_mock.Verify(repository => repository.Create(It.IsAny<Blog>()), Times.Once);
            Assert.Equal(blogs.First().blog_id, newblog.blog_id);
        }
        [Fact]
        public async void CreatePost_InvalidModelState_NeverCreateExecute()
        {
            _controller.ModelState.AddModelError("baslik", "baslik alani gereklidir.");
            var result = await _controller.Create(blogs.First());
            //_mock.Verify(repository => repository.Create(It.IsAny<Blog>()), Times.Never);
        }

        [Fact]
        public async void Edit_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Edit(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Theory]
        [InlineData(3)]
        public async void Edit_IdInvalid_ReturnNotFound(int blogid)
        {
            Blog blog = null;
            _mock.Setup(x => x.GetByIdAsync(blogid)).ReturnsAsync(blog);
            var result = await _controller.Edit(blogid);
            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(2)]
        public async void Edit_ActionExecutes_ReturnBlog(int blogid)
        {
            var blog = blogs.First(x => x.blog_id == blogid);
            _mock.Setup(repository => repository.GetByIdAsync(blogid)).ReturnsAsync(blog);
            var result = await _controller.Edit(blogid);
            var viewresult = Assert.IsType<ViewResult>(result);
            var resultblog = Assert.IsAssignableFrom<Blog>(viewresult.Model);
            Assert.Equal(blog.blog_id, resultblog.blog_id);
            Assert.Equal(blog.baslik, resultblog.baslik);
        }

        [Theory]
        [InlineData(1)]
        public void EditPost_IdIsNotEqualProduct_ReturnNotFound(int blogid)
        {
            var result = _controller.Edit(2, blogs.First(x => x.blog_id == blogid));
            var redirect = Assert.IsType<NotFoundResult>(result);



        }

        [Theory]
        [InlineData(1)]
        public void EditPost_ValidModelState_ReturnRedirectToIndexAction (int blogid)
        {
            var result = _controller.Edit(blogid, blogs.First(x => x.blog_id == blogid));
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
        [Theory]
        [InlineData(1)]
        public async void EditPost_ValidModelState_UpdateMethodExecute(int blogid)
        {
            var blog = blogs.First(x => x.blog_id == blogid);
            _mock.Setup(repository => repository.Update(blog));
            await _controller.Edit(blogid, blog);
            _mock.Verify(repository => repository.Update(It.IsAny<Blog>()), Times.Once);

        }

        [Fact]
        public async void Delete_IdIsNull_ReturnNotFound()
        {
            var result = await _controller.Delete(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(0)]
        public async void Delete_IdIsNotNullEqualBlog_ReturnNotFound(int blogid)
        {
            Blog blog = null;
            _mock.Setup(x => x.GetByIdAsync(blogid)).ReturnsAsync(blog);
            var result = await _controller.Delete(blogid);
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void Delete_ActionExecute_ReturnBlog(int blogid)
        {
            var blog = blogs.First(x => x.blog_id == blogid);
            _mock.Setup(repository => repository.GetByIdAsync(blogid)).ReturnsAsync(blog);
            var result = await _controller.Delete(blogid);
            var viewresult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Blog>(viewresult.Model);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_ReturRedirectToIndexAction(int blogid)
        {
            var result = await _controller.DeleteConfirmed(blogid);
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_DeleteMethodExecute(int blogid)
        {
            var blog = blogs.First(x => x.blog_id == blogid);
            //_mock.Setup(repository => repository.Delete(blog));
            await _controller.DeleteConfirmed(blogid);
            //_mock.Verify(repository => repository.Delete(It.IsAny<Blog>()), Times.Once);

        }


    } 
}
