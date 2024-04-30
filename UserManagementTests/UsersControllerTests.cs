using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserManagement.Contracts;
using UserManagement.Controllers;
using UserManagement.DataAccess;
using UserManagement.DataAccess.Entities;
using UserManagement.DataAccess.Repositories;

namespace UserManagement.Tests
{
    public class UsersControllerTests : IDisposable
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IPermissionRepository> _permissionRepositoryMock = new();
        private readonly Mock<IUserPermissionRepository> _userPermissionRepositoryMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly UsersController _sut;
        public UsersControllerTests()
        {
            _sut = new(_userRepositoryMock.Object, _permissionRepositoryMock.Object, _userPermissionRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task ShouldGetAllUsersWithNoFilterOrSortTerms()
        {
            var users = new List<User>
            {
                new User { Id = 1, Username = "user1", Password = "pass1", Status = true },
                new User { Id = 2, Username = "user2", Password = "pass2", Status = false },
                new User { Id = 3, Username = "user3", Password = "pass3", Status = true }
            };

            _userRepositoryMock.Setup(repo => repo.GetAllUsersAsync(null, null, null, 1, 10)).ReturnsAsync(new PagedList<User>(users, 1, 10, 3));

            var result = await _sut.GetAllUsersAsync(null, null, null, 1, 10);

            _userRepositoryMock.Verify(x => x.GetAllUsersAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1, 10), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var usersResult = Assert.IsAssignableFrom<PagedList<User>>(okResult.Value).Items;

            Assert.NotNull(okResult);
            Assert.Equal(users.Count, usersResult.Count());
            Assert.Equal(users[0].Username, usersResult.First().Username);
            Assert.Equal(users[1].Password, usersResult.Skip(1).First().Password);
            Assert.Equal(users[2].Status, usersResult.Last().Status);
        }

        [Fact]
        public async Task ShouldGetAllUsersWithFilterAndSortTerms()
        {
            var users = new List<User>
            {
                new User { Id = 1, Username = "user1", Password = "pass1", Status = true },
                new User { Id = 2, Username = "user2", Password = "pass2", Status = false },
                new User { Id = 3, Username = "user3", Password = "pass3", Status = true }
            };

            string searchTerm = "user1";
            string sortColumn = "Username";
            string sortOrder = "asc";
            int page = 1;
            int pageSize = 10;

            var filteredUserList = new List<User>
            {
                users[0]
            };

            _userRepositoryMock.Setup(repo => repo.GetAllUsersAsync(searchTerm, sortColumn, sortOrder, page, pageSize)).ReturnsAsync(new PagedList<User>(filteredUserList, page, pageSize, filteredUserList.Count));


            var result = await _sut.GetAllUsersAsync(searchTerm, sortColumn, sortOrder, page, pageSize);

            _userRepositoryMock.Verify(x => x.GetAllUsersAsync(It.Is<string>(x => x.Equals(searchTerm)),
                It.Is<string>(x => x.Equals(sortColumn)),
                It.Is<string>(x => x.Equals(sortOrder)),
                It.Is<int>(x => x.Equals(page)), 
                It.Is<int>(x => x.Equals(pageSize))), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var usersResult = Assert.IsAssignableFrom<PagedList<User>>(okResult.Value).Items;

            // Assert
            Assert.NotNull(okResult);
            Assert.Single(usersResult);
            Assert.Equal("user1", usersResult.First().Username);
        }

        [Fact]
        public async Task ShouldGetUserById()
        {
            var user = new User { Id = 1, Username = "Test", Password = "Test1234", Status = true };
            _userRepositoryMock.Setup(repo => repo.GetUserById(user.Id)).ReturnsAsync(user);

            var result = await _sut.GetUserByIdAsync(user.Id);

            _userRepositoryMock.Verify(x => x.GetUserById(It.Is<int>(g => g.Equals(user.Id))), Times.Once);
            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var userResult = Assert.IsType<User>(okResult.Value);

            Assert.NotNull(okResult);
            Assert.Equal(user.Username, userResult.Username);
            Assert.Equal(user.Password, userResult.Password);
            Assert.Equal(user.Status, userResult.Status);
        }

        [Fact]
        public async Task ShouldCreateNewUser()
        {
            var userDto = new UserDto("Test", "Test1234", true);
            var user = new User { Id = 1, Username = "Test", Password = "Test1234", Status = true };
            _userRepositoryMock.Setup(x => x.CreateUserAsync(user)).ReturnsAsync(user.Id);
            _mapperMock.Setup(mapper => mapper.Map<User>(userDto)).Returns(user);

            var result = await _sut.CreateUserAsync(userDto);

            _userRepositoryMock.Verify(x => x.CreateUserAsync(It.Is<User>(x => x.Username == userDto.Username
            && x.Password == userDto.Password
            && x.Status == userDto.Status)), Times.Once);
            _mapperMock.Verify(x => x.Map<User>(It.Is<UserDto>(x => x.Username == userDto.Username
            && x.Password == userDto.Password
            && x.Status == userDto.Status)), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var userResult = Assert.IsType<int>(okResult.Value);
            Assert.NotNull(okResult);
            Assert.Equal(userResult, user.Id);
        }

        [Fact]
        public async Task ShouldDeleteUser()
        {
            int userId = 1;
            var result = await _sut.DeleteUserAsync(userId);

            _userRepositoryMock.Verify(x => x.DeleteUserAsync(It.Is<int>(g => g.Equals(userId))), Times.Once);

            var okResult = Assert.IsType<OkResult>(result);
            Assert.NotNull(okResult);
        }

        [Fact]
        public async Task ShouldFindAndUpdateUser()
        {
            var userDto = new UserDto("UpdateTest", "Update1234", false);
            var user = new User { Id = 1, Username = "Test", Password = "Test1234", Status = true };
            var updatedUser = new User { Id = 1, Username = userDto.Username, Password = userDto.Password, Status = userDto.Status };
            _mapperMock.Setup(mapper => mapper.Map<User>(userDto)).Returns(user);
            _userRepositoryMock.Setup(repo => repo.UpdateUserAsync(user)).ReturnsAsync(updatedUser);

            var result = await _sut.UpdateUserAsync(userDto);

            _mapperMock.Verify(x => x.Map<User>(It.Is<UserDto>(u => u.Username == updatedUser.Username
            && u.Password == updatedUser.Password
            && u.Status == updatedUser.Status)), Times.Once);

            _userRepositoryMock.Verify(x => x.UpdateUserAsync(It.Is<User>(u => u.Username == user.Username
            && u.Password == user.Password
            && u.Status == user.Status)), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var userResult = Assert.IsType<User>(okResult.Value);

            Assert.NotNull(okResult);
            Assert.Equal(updatedUser.Username, userResult.Username);
            Assert.Equal(updatedUser.Password, userResult.Password);
            Assert.Equal(updatedUser.Status, userResult.Status);
        }

        [Fact]
        public async Task ShouldNotFindAndUpdateUser()
        {
            var userDto = new UserDto("UpdateTest", "Update1234", false);
            _mapperMock.Setup(mapper => mapper.Map<User>(userDto)).Returns(new User());
            _userRepositoryMock.Setup(repo => repo.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync((User)null);

            var result = await _sut.UpdateUserAsync(userDto);

            _mapperMock.Verify(x => x.Map<User>(It.IsAny<UserDto>()), Times.Once);

            _userRepositoryMock.Verify(x => x.UpdateUserAsync(It.IsAny<User>()), Times.Once);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ShouldGetAllUserPermissions()
        {
            var userId = 1;
            _permissionRepositoryMock.Setup(repo => repo.GetAllUserPermissionsAsync(userId)).ReturnsAsync(new List<Permission>());

            var result = await _sut.GetAllUserPermissions(userId);

            _permissionRepositoryMock.Verify(x => x.GetAllUserPermissionsAsync(It.Is<int>(x => x.Equals(userId))), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
        }

        [Fact]
        public async Task ShouldAddUserPermission()
        {
            int userId = 1;
            int permissionId = 3;

            var result = await _sut.AddUserPermission(userId, permissionId);

            _userPermissionRepositoryMock.Verify(x => x.AddUserPermissionAsync(It.Is<int>(g => g.Equals(userId)), It.Is<int>(x => x.Equals(permissionId))), Times.Once);

            var okResult = Assert.IsType<OkResult>(result);
            Assert.NotNull(okResult);
        }

        [Fact]
        public async Task ShouldRemoveUserPermission()
        {
            int userId = 1;
            int permissionId = 3;

            var result = await _sut.RemoveUserPermission(userId, permissionId);

            _userPermissionRepositoryMock.Verify(x => x.RemoveUserPermissionAsync(It.Is<int>(g => g.Equals(userId)), It.Is<int>(x => x.Equals(permissionId))), Times.Once);

            var okResult = Assert.IsType<OkResult>(result);
            Assert.NotNull(okResult);
        }

        public void Dispose()
        {
            _userRepositoryMock.VerifyNoOtherCalls();
            _permissionRepositoryMock.VerifyNoOtherCalls();
            _userPermissionRepositoryMock.VerifyNoOtherCalls();
            _mapperMock.VerifyNoOtherCalls();
        }
    }
}
