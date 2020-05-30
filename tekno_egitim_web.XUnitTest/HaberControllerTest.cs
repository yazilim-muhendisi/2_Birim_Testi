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

namespace tekno_egitim_web.XUnitTest
{

    public class HaberControllerTest
    {
        private readonly Mock<IRepository<Haber>> _mock;
        private readonly HabersController _controller;
        private List<Haber> Habers;

        public HaberControllerTest()
        {
            _mock = new Mock<IRepository<Haber>>();
            //_controller = new HabersController(_mock.Object);
            Habers = new List<Haber>()
            {
                new Haber
                {
                    haber_id = 1,
                    baslik = "testdeneme12",
                    aciklama = "testaciklama1",
                    olusturulma = DateTime.Now,
                    imageUrl = "",
                    kategori_id = 1,
                    haber_silme = false
                },
                new Haber
                {
                    haber_id = 2,
                    baslik = "testdeneme22",
                    aciklama = "testaciklama2222",
                    olusturulma = DateTime.Now,
                    imageUrl = "",
                    kategori_id = 1,
                    haber_silme = false
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
        public async void Index_ActionExecutes_ReturnHaberList()
        {
            //_mock.Setup(repository => repository.GetAllAsync(Habers));
            var result = await _controller.Index();
            var viewresult = Assert.IsType<ViewResult>(result);
            var Haberlist = Assert.IsAssignableFrom<IEnumerable<Haber>>(viewresult.Model);
            //var redirect = Assert.IsType<HaberFoundResult>(result);
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
        public async void Detais_IdInvalidId_ReturnHaberFound()
        {
            Haber Haber = null;
            _mock.Setup(x => x.GetByIdAsync(0)).ReturnsAsync(Haber);
            var result = await _controller.Details(0);
            //var redirect = Assert.IsType<HaberFoundResult>(result);
            //Assert.Equal<int>(404, redirect.StatusCode);
        }
        [Theory]
        [InlineData(1)]
        public async void Details_ValidIdReturnHaber(int Haberid)
        {
            Haber Haber = Habers.First(x => x.haber_id == Haberid);
            //_mock.Setup(repository => repository.GetByIdAsync());
            var result = await _controller.Details(Haberid);
            var viewresult = Assert.IsType<ViewResult>(result);
            var resultHaber = Assert.IsAssignableFrom<Haber>(viewresult.Model);
            Assert.Equal(Haber.haber_id, resultHaber.haber_id);
            Assert.Equal(Haber.baslik, resultHaber.baslik);
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
            var result = await _controller.Create(Habers.First());
            var viewresult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Haber>(viewresult.Model);
        }
        [Fact]
        public async void CreatePost_ValidModelState_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Create(Habers.First());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

        }
        [Fact]
        public async void CreatePost_ValidModelState_CreateMethodExecute()
        {
            Haber newHaber = null;
            //_mock.Setup(repository => repository.Create(It.IsAny<Haber>())).CallBack<Haber>(x => newHaber = x);
            var result = await _controller.Create(Habers.First());
            //_mock.Verify(repository => repository.Create(It.IsAny<Haber>()), Times.Once);
            Assert.Equal(Habers.First().haber_id, newHaber.haber_id);
        }
        [Fact]
        public async void CreatePost_InvalidModelState_NeverCreateExecute()
        {
            _controller.ModelState.AddModelError("baslik", "baslik alani gereklidir.");
            var result = await _controller.Create(Habers.First());
            //_mock.Verify(repository => repository.Create(It.IsAny<Haber>()), Times.Never);
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
        public async void Edit_IdInvalid_ReturnHaberFound(int Haberid)
        {
            Haber Haber = null;
            _mock.Setup(x => x.GetByIdAsync(Haberid)).ReturnsAsync(Haber);
            var result = await _controller.Edit(Haberid);
            //var redirect = Assert.IsType<HaberFoundResult>(result);
            //Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(2)]
        public async void Edit_ActionExecutes_ReturnHaber(int Haberid)
        {
            var Haber = Habers.First(x => x.haber_id == Haberid);
            _mock.Setup(repository => repository.GetByIdAsync(Haberid)).ReturnsAsync(Haber);
            var result = await _controller.Edit(Haberid);
            var viewresult = Assert.IsType<ViewResult>(result);
            var resultHaber = Assert.IsAssignableFrom<Haber>(viewresult.Model);
            Assert.Equal(Haber.haber_id, resultHaber.haber_id);
            Assert.Equal(Haber.baslik, resultHaber.baslik);
        }

        [Theory]
        [InlineData(1)]
        public void EditPost_IdIsHaberEqualProduct_ReturnHaberFound(int Haberid)
        {
            var result = _controller.Edit(2, Habers.First(x => x.haber_id == Haberid));
            //var redirect = Assert.IsType<HaberFoundResult>(result);



        }

        [Theory]
        [InlineData(1)]
        public void EditPost_ValidModelState_ReturnRedirectToIndexAction(int Haberid)
        {
            var result = _controller.Edit(Haberid, Habers.First(x => x.haber_id == Haberid));
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
        [Theory]
        [InlineData(1)]
        public void EditPost_ValidModelState_UpdateMethodExecute(int Haberid)
        {
            var Haber = Habers.First(x => x.haber_id == Haberid);
            _mock.Setup(repository => repository.Update(Haber));
            _controller.Edit(Haberid, Haber);
            _mock.Verify(repository => repository.Update(It.IsAny<Haber>()), Times.Once);

        }

        [Fact]
        public async void Delete_IdIsNull_ReturnHaberFound()
        {
            var result = await _controller.Delete(null);
            //Assert.IsType<HaberFoundResult>(result);
        }

        [Theory]
        [InlineData(0)]
        public async void Delete_IdIsHaberNullEqualHaber_ReturnHaberFound(int Haberid)
        {
            Haber Haber = null;
            _mock.Setup(x => x.GetByIdAsync(Haberid)).ReturnsAsync(Haber);
            var result = await _controller.Delete(Haberid);
            //Assert.IsType<HaberFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void Delete_ActionExecute_ReturnHaber(int Haberid)
        {
            var Haber = Habers.First(x => x.haber_id == Haberid);
            _mock.Setup(repository => repository.GetByIdAsync(Haberid)).ReturnsAsync(Haber);
            var result = await _controller.Delete(Haberid);
            var viewresult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Haber>(viewresult.Model);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_ReturRedirectToIndexAction(int Haberid)
        {
            var result = await _controller.DeleteConfirmed(Haberid);
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_DeleteMethodExecute(int Haberid)
        {
            var Haber = Habers.First(x => x.haber_id == Haberid);
            //_mock.Setup(repository => repository.Delete(Haber));
            await _controller.DeleteConfirmed(Haberid);
            //_mock.Verify(repository => repository.Delete(It.IsAny<Haber>()), Times.Once);

        }


    }
}
