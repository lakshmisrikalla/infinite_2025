using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Client_model.Controllers;
using Client_model.Models;
using Client_model.Helpers;
using System.Collections.Generic;

namespace Client_model.Tests
{
    [TestClass]
    public class AccountControllerTests
    {
        private Mock<InsuranceDB1Entities> _mockContext;
        private Mock<DbSet<LoginCredential>> _mockLoginSet;
        private Mock<DbSet<Client>> _mockClientSet;
        private AccountController _controller;

        [TestInitialize]
        public void Setup()
        {
            var hashedPassword = HashHelper.HashPassword("123456");
            var loginDataList = new List<LoginCredential>
            {
                new LoginCredential
                {
                    LoginID = 1,
                    Username = "coolboy",
                    PasswordHash = hashedPassword,
                    Role = "Client",
                    IsActive = true,
                    Email = "srinivasvk77@gmail.com"
                }
            };
            var loginData = loginDataList.AsQueryable();

            _mockLoginSet = new Mock<DbSet<LoginCredential>>();
            _mockLoginSet.As<IQueryable<LoginCredential>>().Setup(m => m.Provider).Returns(loginData.Provider);
            _mockLoginSet.As<IQueryable<LoginCredential>>().Setup(m => m.Expression).Returns(loginData.Expression);
            _mockLoginSet.As<IQueryable<LoginCredential>>().Setup(m => m.ElementType).Returns(loginData.ElementType);
            _mockLoginSet.As<IQueryable<LoginCredential>>().Setup(m => m.GetEnumerator()).Returns(loginData.GetEnumerator());

            // Mock the Find method to enable _context.LoginCredentials.Find(loginId)
            _mockLoginSet.Setup(m => m.Find(It.IsAny<object[]>()))
                .Returns<object[]>(ids => loginDataList.SingleOrDefault(l => l.LoginID == (int)ids[0]));

            var clientDataList = new List<Client>
            {
                new Client
                {
                    ClientID = 1,
                    LoginID = 1,
                    CompanyName = "SS_Infinite Pvt Ltd",
                    CompanyCode = "Z693UDYT",
                    Status = "Approved",
                    ContactEmail = "srinivasvk77@gmail.com",
                    ContactPhone = "09087667340"
                }
            };
            var clientData = clientDataList.AsQueryable();

            _mockClientSet = new Mock<DbSet<Client>>();
            _mockClientSet.As<IQueryable<Client>>().Setup(m => m.Provider).Returns(clientData.Provider);
            _mockClientSet.As<IQueryable<Client>>().Setup(m => m.Expression).Returns(clientData.Expression);
            _mockClientSet.As<IQueryable<Client>>().Setup(m => m.ElementType).Returns(clientData.ElementType);
            _mockClientSet.As<IQueryable<Client>>().Setup(m => m.GetEnumerator()).Returns(clientData.GetEnumerator());

            _mockContext = new Mock<InsuranceDB1Entities>();
            _mockContext.Setup(c => c.LoginCredentials).Returns(_mockLoginSet.Object);
            _mockContext.Setup(c => c.Clients).Returns(_mockClientSet.Object);

            _controller = new AccountController(_mockContext.Object);

            var sessionMock = new Mock<System.Web.HttpSessionStateBase>();
            sessionMock.Setup(s => s["LoginID"]).Returns(1);
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = Mock.Of<System.Web.HttpContextBase>(c => c.Session == sessionMock.Object);
        }

        [TestMethod]
        public void Register_ValidUser_RedirectsToLogin()
        {
            var emptyLoginData = new List<LoginCredential>().AsQueryable();
            var emptyClientData = new List<Client>().AsQueryable();

            var mockEmptyLoginSet = new Mock<DbSet<LoginCredential>>();
            mockEmptyLoginSet.As<IQueryable<LoginCredential>>().Setup(m => m.Provider).Returns(emptyLoginData.Provider);
            mockEmptyLoginSet.As<IQueryable<LoginCredential>>().Setup(m => m.Expression).Returns(emptyLoginData.Expression);
            mockEmptyLoginSet.As<IQueryable<LoginCredential>>().Setup(m => m.ElementType).Returns(emptyLoginData.ElementType);
            mockEmptyLoginSet.As<IQueryable<LoginCredential>>().Setup(m => m.GetEnumerator()).Returns(emptyLoginData.GetEnumerator());

            var mockEmptyClientSet = new Mock<DbSet<Client>>();
            mockEmptyClientSet.As<IQueryable<Client>>().Setup(m => m.Provider).Returns(emptyClientData.Provider);
            mockEmptyClientSet.As<IQueryable<Client>>().Setup(m => m.Expression).Returns(emptyClientData.Expression);
            mockEmptyClientSet.As<IQueryable<Client>>().Setup(m => m.ElementType).Returns(emptyClientData.ElementType);
            mockEmptyClientSet.As<IQueryable<Client>>().Setup(m => m.GetEnumerator()).Returns(emptyClientData.GetEnumerator());

            _mockContext.Setup(c => c.LoginCredentials).Returns(mockEmptyLoginSet.Object);
            _mockContext.Setup(c => c.Clients).Returns(mockEmptyClientSet.Object);

            var model = new RegisterClientVm
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "Password123!",
                CompanyName = "NewCompany",
                ContactPhone = "9876543210"
            };

            var result = _controller.Register(model) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.RouteValues["action"]);

            // Restore original data sets for further tests
            _mockContext.Setup(c => c.LoginCredentials).Returns(_mockLoginSet.Object);
            _mockContext.Setup(c => c.Clients).Returns(_mockClientSet.Object);
        }

        [TestMethod]
        public void Login_ValidUser_RedirectsToMyProfile()
        {
            var model = new LoginVm
            {
                Username = "coolboy",
                Password = "123456",
                CompanyCode = "Z693UDYT"
            };

            var result = _controller.Login(model) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("MyProfile", result.RouteValues["action"]);
            Assert.AreEqual("Client", result.RouteValues["controller"]);
        }

        [TestMethod]
        public void ChangePassword_ValidCurrentPassword_ChangesPasswordAndRedirects()
        {
            var model = new ChangePasswordVm
            {
                CurrentPassword = "123456",
                NewPassword = "newStrongPassword!23"
            };

            var result = _controller.ChangePassword(model) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("MyProfile", result.RouteValues["action"]);
        }
    }
}
