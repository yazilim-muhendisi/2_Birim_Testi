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

    public class MakaleControllerTest
    {
        private readonly Mock<IRepository<Makale>> _mock;
        private readonly MakalesController _controller;
        private List<Makale> Makales;

        public MakaleControllerTest()
        {
            _mock = new Mock<IRepository<Makale>>();
            //_controller = new MakalesController(_mock.Object);
            Makales = new List<Makale>()
            {
                new Makale
                {
                    makale_id = 1,
                    baslik = "testdeneme12",
                    aciklama = "testaciklama1",
                    olusturulma = DateTime.Now,
                    imageUrl = "",
                    kategori_id = 1,
                    makale_silme = false
                },
                new Makale
                {
                    makale_id = 2,
                    baslik = "testdeneme22",
                    aciklama = "testaciklama2222",
                    olusturulma = DateTime.Now,
                    imageUrl = "",
                    kategori_id = 1,
                    makale_silme = false
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
        public async void Index_ActionExecutes_ReturnMakaleList()
        {
            //_mock.Setup(repository => repository.GetAllAsync(Makales));
            var result = await _controller.Index();
            var viewresult = Assert.IsType<ViewResult>(result);
            var Makalelist = Assert.IsAssignableFrom<IEnumerable<Makale>>(viewresult.Model);
            var redirect = Assert.IsType<NotFoundResult>(result);
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
        public async void Detais_IdInvalidId_ReturnNotFound()
        {
            Makale Makale = null;
            _mock.Setup(x => x.GetByIdAsync(0)).ReturnsAsync(Makale);
            var result = await _controller.Details(0);
            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal<int>(404, redirect.StatusCode);
        }
        [Theory]
        [InlineData(1)]
        public async void Details_ValidIdReturnMakale(int Makaleid)
        {
            Makale Makale = Makales.First(x => x.makale_id == Makaleid);
            //_mock.Setup(repository => repository.GetByIdAsync());
            var result = await _controller.Details(Makaleid);
            var viewresult = Assert.IsType<ViewResult>(result);
            var resultMakale = Assert.IsAssignableFrom<Makale>(viewresult.Model);
            Assert.Equal(Makale.makale_id, resultMakale.makale_id);
            Assert.Equal(Makale.baslik, resultMakale.baslik);
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
            var result = await _controller.Create(Makales.First());
            var viewresult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Makale>(viewresult.Model);
        }
        [Fact]
        public async void CreatePost_ValidModelState_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Create(Makales.First());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

        }
        [Fact]
        public async void CreatePost_ValidModelState_CreateMethodExecute()
        {
            Makale newMakale = null;
            //_mock.Setup(repository => repository.Create(It.IsAny<Makale>())).CallBack<Makale>(x => newMakale = x);
            var result = await _controller.Create(Makales.First());
            //_mock.Verify(repository => repository.Create(It.IsAny<Makale>()), Times.Once);
            Assert.Equal(Makales.First().makale_id, newMakale.makale_id);
        }
        [Fact]
        public async void CreatePost_InvalidModelState_NeverCreateExecute()
        {
            _controller.ModelState.AddModelError("baslik", "baslik alani gereklidir.");
            var result = await _controller.Create(Makales.First());
            //_mock.Verify(repository => repository.Create(It.IsAny<Makale>()), Times.Never);
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
        public async void Edit_IdInvalid_ReturnNotFound(int Makaleid)
        {
            Makale Makale = null;
            _mock.Setup(x => x.GetByIdAsync(Makaleid)).ReturnsAsync(Makale);
            var result = await _controller.Edit(Makaleid);
            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(2)]
        public async void Edit_ActionExecutes_ReturnMakale(int Makaleid)
        {
            var Makale = Makales.First(x => x.makale_id == Makaleid);
            _mock.Setup(repository => repository.GetByIdAsync(Makaleid)).ReturnsAsync(Makale);
            var result = await _controller.Edit(Makaleid);
            var viewresult = Assert.IsType<ViewResult>(result);
            var resultMakale = Assert.IsAssignableFrom<Makale>(viewresult.Model);
            Assert.Equal(Makale.makale_id, resultMakale.makale_id);
            Assert.Equal(Makale.baslik, resultMakale.baslik);
        }

        [Theory]
        [InlineData(1)]
        public void EditPost_IdIsNotEqualProduct_ReturnNotFound(int Makaleid)
        {
            var result = _controller.Edit(2, Makales.First(x => x.makale_id == Makaleid));
            var redirect = Assert.IsType<NotFoundResult>(result);



        }

        [Theory]
        [InlineData(1)]
        public void EditPost_ValidModelState_ReturnRedirectToIndexAction(int Makaleid)
        {
            var result = _controller.Edit(Makaleid, Makales.First(x => x.makale_id == Makaleid));
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
        [Theory]
        [InlineData(1)]
        public void EditPost_ValidModelState_UpdateMethodExecute(int Makaleid)
        {
            var Makale = Makales.First(x => x.makale_id == Makaleid);
            _mock.Setup(repository => repository.Update(Makale));
            _controller.Edit(Makaleid, Makale);
            _mock.Verify(repository => repository.Update(It.IsAny<Makale>()), Times.Once);

        }

        [Fact]
        public async void Delete_IdIsNull_ReturnNotFound()
        {
            var result = await _controller.Delete(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(0)]
        public async void Delete_IdIsNotNullEqualMakale_ReturnNotFound(int Makaleid)
        {
            Makale Makale = null;
            _mock.Setup(x => x.GetByIdAsync(Makaleid)).ReturnsAsync(Makale);
            var result = await _controller.Delete(Makaleid);
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void Delete_ActionExecute_ReturnMakale(int Makaleid)
        {
            var Makale = Makales.First(x => x.makale_id == Makaleid);
            _mock.Setup(repository => repository.GetByIdAsync(Makaleid)).ReturnsAsync(Makale);
            var result = await _controller.Delete(Makaleid);
            var viewresult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Makale>(viewresult.Model);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_ReturRedirectToIndexAction(int Makaleid)
        {
            var result = await _controller.DeleteConfirmed(Makaleid);
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_DeleteMethodExecute(int Makaleid)
        {
            var Makale = Makales.First(x => x.makale_id == Makaleid);
            //_mock.Setup(repository => repository.Delete(Makale));
            await _controller.DeleteConfirmed(Makaleid);
            //_mock.Verify(repository => repository.Delete(It.IsAny<Makale>()), Times.Once);

        }


    }
}
