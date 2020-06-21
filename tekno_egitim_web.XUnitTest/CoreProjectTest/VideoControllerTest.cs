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

    public class VideoControllerTest
    {
        private readonly Mock<IRepository<Video>> _mock;
        private readonly VideosController _controller;
        private List<Video> Videos;

        public VideoControllerTest()
        {
            _mock = new Mock<IRepository<Video>>();
            //_controller = new VideosController(_mock.Object);
            Videos = new List<Video>()
            {
                new Video
                {
                    video_id = 1,
                    baslik = "testdeneme12",
                    aciklama = "testaciklama1",
                    olusturulma = DateTime.Now,
                    videoUrl = "",
                    kategori_id = 1,
                    video_silme = false
                },
                new Video
                {
                    video_id = 2,
                    baslik = "testdeneme22",
                    aciklama = "testaciklama2222",
                    olusturulma = DateTime.Now,
                    videoUrl = "",
                    kategori_id = 1,
                    video_silme = false
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
        public async void Index_ActionExecutes_ReturnVideoList()
        {
            //_mock.Setup(repository => repository.GetAllAsync(Videos));
            var result = await _controller.Index();
            var viewresult = Assert.IsType<ViewResult>(result);
            var Videolist = Assert.IsAssignableFrom<IEnumerable<Video>>(viewresult.Model);
            //var redirect = Assert.IsType<VideoFoundResult>(result);
            //Assert.Equal("Index", RedirectResult.ActionName);
        }
        [Fact]
        public async void Details_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Details(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Create", redirect.ActionName);
        }
        [Fact]
        public async void Detais_IdInvalidId_ReturnVideoFound()
        {
            Video Video = null;
            _mock.Setup(x => x.GetByIdAsync(0)).ReturnsAsync(Video);
            var result = await _controller.Details(0);
            //var redirect = Assert.IsType<VideoFoundResult>(result);
            //Assert.Equal<int>(404, redirect.StatusCode);
        }
        [Theory]
        [InlineData(1)]
        public async void Details_ValidIdReturnVideo(int Videoid)
        {
            Video Video = Videos.First(x => x.video_id == Videoid);
            //_mock.Setup(repository => repository.GetByIdAsync());
            var result = await _controller.Details(Videoid);
            var viewresult = Assert.IsType<ViewResult>(result);
            var resultVideo = Assert.IsAssignableFrom<Video>(viewresult.Model);
            Assert.Equal(Video.video_id, resultVideo.video_id);
            Assert.Equal(Video.baslik, resultVideo.baslik);
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
            var result = await _controller.Create(Videos.First());
            var viewresult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Video>(viewresult.Model);
        }
        [Fact]
        public async void CreatePost_ValidModelState_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Create(Videos.First());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

        }
        [Fact]
        public async void CreatePost_ValidModelState_CreateMethodExecute()
        {
            Video newVideo = null;
            //_mock.Setup(repository => repository.Create(It.IsAny<Video>())).CallBack<Video>(x => newVideo = x);
            var result = await _controller.Create(Videos.First());
            //_mock.Verify(repository => repository.Create(It.IsAny<Video>()), Times.Once);
            Assert.Equal(Videos.First().video_id, newVideo.video_id);
        }
        [Fact]
        public async void CreatePost_InvalidModelState_NeverCreateExecute()
        {
            _controller.ModelState.AddModelError("baslik", "baslik alani gereklidir.");
            var result = await _controller.Create(Videos.First());
            //_mock.Verify(repository => repository.Create(It.IsAny<Video>()), Times.Never);
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
        public async void Edit_IdInvalid_ReturnVideoFound(int Videoid)
        {
            Video Video = null;
            _mock.Setup(x => x.GetByIdAsync(Videoid)).ReturnsAsync(Video);
            var result = await _controller.Edit(Videoid);
            //var redirect = Assert.IsType<VideoFoundResult>(result);
            //Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(2)]
        public async void Edit_ActionExecutes_ReturnVideo(int Videoid)
        {
            var Video = Videos.First(x => x.video_id == Videoid);
            _mock.Setup(repository => repository.GetByIdAsync(Videoid)).ReturnsAsync(Video);
            var result = await _controller.Edit(Videoid);
            var viewresult = Assert.IsType<ViewResult>(result);
            var resultVideo = Assert.IsAssignableFrom<Video>(viewresult.Model);
            Assert.Equal(Video.video_id, resultVideo.video_id);
            Assert.Equal(Video.baslik, resultVideo.baslik);
        }

        [Theory]
        [InlineData(1)]
        public void EditPost_IdIsVideoEqualProduct_ReturnVideoFound(int Videoid)
        {
            var result = _controller.Edit(2, Videos.First(x => x.video_id == Videoid));
            //var redirect = Assert.IsType<VideoFoundResult>(result);



        }

        [Theory]
        [InlineData(1)]
        public void EditPost_ValidModelState_ReturnRedirectToIndexAction(int Videoid)
        {
            var result = _controller.Edit(Videoid, Videos.First(x => x.video_id == Videoid));
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
        [Theory]
        [InlineData(1)]
        public void EditPost_ValidModelState_UpdateMethodExecute(int Videoid)
        {
            var Video = Videos.First(x => x.video_id == Videoid);
            _mock.Setup(repository => repository.Update(Video));
            _controller.Edit(Videoid, Video);
            _mock.Verify(repository => repository.Update(It.IsAny<Video>()), Times.Once);

        }

        [Fact]
        public async void Delete_IdIsNull_ReturnVideoFound()
        {
            var result = await _controller.Delete(null);
            //Assert.IsType<VideoFoundResult>(result);
        }

        [Theory]
        [InlineData(0)]
        public async void Delete_IdIsVideoNullEqualVideo_ReturnVideoFound(int Videoid)
        {
            Video Video = null;
            _mock.Setup(x => x.GetByIdAsync(Videoid)).ReturnsAsync(Video);
            var result = await _controller.Delete(Videoid);
            //Assert.IsType<VideoFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void Delete_ActionExecute_ReturnVideo(int Videoid)
        {
            var Video = Videos.First(x => x.video_id == Videoid);
            _mock.Setup(repository => repository.GetByIdAsync(Videoid)).ReturnsAsync(Video);
            var result = await _controller.Delete(Videoid);
            var viewresult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Video>(viewresult.Model);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_ReturRedirectToIndexAction(int Videoid)
        {
            var result = await _controller.DeleteConfirmed(Videoid);
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_DeleteMethodExecute(int Videoid)
        {
            var Video = Videos.First(x => x.video_id == Videoid);
            //_mock.Setup(repository => repository.Delete(Video));
            await _controller.DeleteConfirmed(Videoid);
            //_mock.Verify(repository => repository.Delete(It.IsAny<Video>()), Times.Once);

        }


    }
}
