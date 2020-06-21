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

    public class NotControllerTest
    {
        private readonly Mock<IRepository<Not>> _mock;
        private readonly NotsController _controller;
        private List<Not> Nots;

        public NotControllerTest()
        {
            _mock = new Mock<IRepository<Not>>();
            //_controller = new NotsController(_mock.Object);
            Nots = new List<Not>()
            {
                new Not
                {
                    not_id = 1,
                    baslik = "testdeneme12",
                    aciklama = "testaciklama1",
                    olusturulma = DateTime.Now,
                    imageUrl = "",
                    kategori_id = 1,
                    not_silme = false
                },
                new Not
                {
                    not_id = 2,
                    baslik = "testdeneme22",
                    aciklama = "testaciklama2222",
                    olusturulma = DateTime.Now,
                    imageUrl = "",
                    kategori_id = 1,
                    not_silme = false
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
        public async void Index_ActionExecutes_ReturnNotList()
        {
            //_mock.Setup(repository => repository.GetAllAsync(Nots));
            var result = await _controller.Index();
            var viewresult = Assert.IsType<ViewResult>(result);
            var Notlist = Assert.IsAssignableFrom<IEnumerable<Not>>(viewresult.Model);
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
            Not Not = null;
            _mock.Setup(x => x.GetByIdAsync(0)).ReturnsAsync(Not);
            var result = await _controller.Details(0);
            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal<int>(404, redirect.StatusCode);
        }
        [Theory]
        [InlineData(1)]
        public async void Details_ValidIdReturnNot(int Notid)
        {
            Not Not = Nots.First(x => x.not_id == Notid);
            //_mock.Setup(repository => repository.GetByIdAsync());
            var result = await _controller.Details(Notid);
            var viewresult = Assert.IsType<ViewResult>(result);
            var resultNot = Assert.IsAssignableFrom<Not>(viewresult.Model);
            Assert.Equal(Not.not_id, resultNot.not_id);
            Assert.Equal(Not.baslik, resultNot.baslik);
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
            var result = await _controller.Create(Nots.First());
            var viewresult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Not>(viewresult.Model);
        }
        [Fact]
        public async void CreatePost_ValidModelState_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Create(Nots.First());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

        }
        [Fact]
        public async void CreatePost_ValidModelState_CreateMethodExecute()
        {
            Not newNot = null;
            //_mock.Setup(repository => repository.Create(It.IsAny<Not>())).CallBack<Not>(x => newNot = x);
            var result = await _controller.Create(Nots.First());
            //_mock.Verify(repository => repository.Create(It.IsAny<Not>()), Times.Once);
            Assert.Equal(Nots.First().not_id, newNot.not_id);
        }
        [Fact]
        public async void CreatePost_InvalidModelState_NeverCreateExecute()
        {
            _controller.ModelState.AddModelError("baslik", "baslik alani gereklidir.");
            var result = await _controller.Create(Nots.First());
            //_mock.Verify(repository => repository.Create(It.IsAny<Not>()), Times.Never);
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
        public async void Edit_IdInvalid_ReturnNotFound(int Notid)
        {
            Not Not = null;
            _mock.Setup(x => x.GetByIdAsync(Notid)).ReturnsAsync(Not);
            var result = await _controller.Edit(Notid);
            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(2)]
        public async void Edit_ActionExecutes_ReturnNot(int Notid)
        {
            var Not = Nots.First(x => x.not_id == Notid);
            _mock.Setup(repository => repository.GetByIdAsync(Notid)).ReturnsAsync(Not);
            var result = await _controller.Edit(Notid);
            var viewresult = Assert.IsType<ViewResult>(result);
            var resultNot = Assert.IsAssignableFrom<Not>(viewresult.Model);
            Assert.Equal(Not.not_id, resultNot.not_id);
            Assert.Equal(Not.baslik, resultNot.baslik);
        }

        [Theory]
        [InlineData(1)]
        public void EditPost_IdIsNotEqualProduct_ReturnNotFound(int Notid)
        {
            var result = _controller.Edit(2, Nots.First(x => x.not_id == Notid));
            var redirect = Assert.IsType<NotFoundResult>(result);



        }

        [Theory]
        [InlineData(1)]
        public void EditPost_ValidModelState_ReturnRedirectToIndexAction(int Notid)
        {
            var result = _controller.Edit(Notid, Nots.First(x => x.not_id == Notid));
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
        [Theory]
        [InlineData(1)]
        public void EditPost_ValidModelState_UpdateMethodExecute(int Notid)
        {
            var Not = Nots.First(x => x.not_id == Notid);
            _mock.Setup(repository => repository.Update(Not));
            _controller.Edit(Notid, Not);
            _mock.Verify(repository => repository.Update(It.IsAny<Not>()), Times.Once);

        }

        [Fact]
        public async void Delete_IdIsNull_ReturnNotFound()
        {
            var result = await _controller.Delete(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(0)]
        public async void Delete_IdIsNotNullEqualNot_ReturnNotFound(int Notid)
        {
            Not Not = null;
            _mock.Setup(x => x.GetByIdAsync(Notid)).ReturnsAsync(Not);
            var result = await _controller.Delete(Notid);
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void Delete_ActionExecute_ReturnNot(int Notid)
        {
            var Not = Nots.First(x => x.not_id == Notid);
            _mock.Setup(repository => repository.GetByIdAsync(Notid)).ReturnsAsync(Not);
            var result = await _controller.Delete(Notid);
            var viewresult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Not>(viewresult.Model);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_ReturRedirectToIndexAction(int Notid)
        {
            var result = await _controller.DeleteConfirmed(Notid);
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_DeleteMethodExecute(int Notid)
        {
            var Not = Nots.First(x => x.not_id == Notid);
            //_mock.Setup(repository => repository.Delete(Not));
            await _controller.DeleteConfirmed(Notid);
            //_mock.Verify(repository => repository.Delete(It.IsAny<Not>()), Times.Once);

        }


    }
}
