using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserManagement.Contracts;
using UserManagement.Controllers;
using UserManagement.DataAccess.Entities;
using UserManagement.DataAccess.Repositories;
using UserManagement.DataAccess;

namespace UserManagement.Tests
{
    public class PermissionsControllerTests : IDisposable
    {
        private readonly Mock<IPermissionRepository> _permissionRepositoryMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly PermissionsController _sut;
        public PermissionsControllerTests()
        {
            _sut = new(_permissionRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task ShouldGetAllPermissionsWithNoFilterOrSortTerms()
        {
            var permissions = new List<Permission>
            {
                new Permission { Id = 1, Code = "code1", Description = "description1" },
                new Permission { Id = 1, Code = "code2", Description = "description2" },
                new Permission { Id = 1, Code = "code3", Description = "description3" }
            };

            _permissionRepositoryMock.Setup(repo => repo.GetAllPermissionsAsync(null, null, null, 1, 10)).ReturnsAsync(new PagedList<Permission>(permissions, 1, 10, 3));

            var result = await _sut.GetAllPermissionsAsync(null, null, null, 1, 10);

            _permissionRepositoryMock.Verify(x => x.GetAllPermissionsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1, 10), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var usersResult = Assert.IsAssignableFrom<PagedList<Permission>>(okResult.Value).Items;

            Assert.NotNull(okResult);
            Assert.Equal(permissions.Count, usersResult.Count());
            Assert.Equal(permissions[0].Code, usersResult.First().Code);
            Assert.Equal(permissions[1].Description, usersResult.Skip(1).First().Description);
            Assert.Equal(permissions[2].Code, usersResult.Last().Code);
        }

        [Fact]
        public async Task ShouldGetAllPermissionsWithFilterAndSortTerms()
        {
            var permissions = new List<Permission>
            {
                new Permission { Id = 1, Code = "code1", Description = "description1" },
                new Permission { Id = 1, Code = "code2", Description = "description2" },
                new Permission { Id = 1, Code = "code3", Description = "description3" }
            };

            string searchTerm = "code1";
            string sortColumn = "Code";
            string sortOrder = "asc";
            int page = 1;
            int pageSize = 10;

            var filteredPermissionList = new List<Permission>
            {
                permissions[0]
            };

            _permissionRepositoryMock.Setup(repo => repo.GetAllPermissionsAsync(searchTerm, sortColumn, sortOrder, page, pageSize)).ReturnsAsync(new PagedList<Permission>(filteredPermissionList, page, pageSize, filteredPermissionList.Count));


            var result = await _sut.GetAllPermissionsAsync(searchTerm, sortColumn, sortOrder, page, pageSize);

            _permissionRepositoryMock.Verify(x => x.GetAllPermissionsAsync(It.Is<string>(x => x.Equals(searchTerm)),
                It.Is<string>(x => x.Equals(sortColumn)),
                It.Is<string>(x => x.Equals(sortOrder)),
                It.Is<int>(x => x.Equals(page)),
                It.Is<int>(x => x.Equals(pageSize))), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var permissionsResult = Assert.IsAssignableFrom<PagedList<Permission>>(okResult.Value).Items;

            // Assert
            Assert.NotNull(okResult);
            Assert.Single(permissionsResult);
            Assert.Equal("code1", permissionsResult.First().Code);
        }

        [Fact]
        public async Task ShouldGetPermissionById()
        {
            var permission = new Permission { Id = 1, Code = "Test", Description = "TestDescription" };
            _permissionRepositoryMock.Setup(repo => repo.GetPermissionById(permission.Id)).ReturnsAsync(permission);

            var result = await _sut.GetPermissionByIdAsync(permission.Id);

            _permissionRepositoryMock.Verify(x => x.GetPermissionById(It.Is<int>(g => g.Equals(permission.Id))), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var userResult = Assert.IsType<Permission>(okResult.Value);

            Assert.NotNull(okResult);
            Assert.Equal(permission.Code, userResult.Code);
            Assert.Equal(permission.Description, userResult.Description);
        }

        [Fact]
        public async Task ShouldCreateNewPermission()
        {
            var permissionDto = new PermissionDto("Test", "TestDescription");
            var permission = new Permission { Id = 1, Code = "Test", Description = "TestDescription" };
            _permissionRepositoryMock.Setup(x => x.CreatePermissionAsync(permission)).ReturnsAsync(permission.Id);
            _mapperMock.Setup(mapper => mapper.Map<Permission>(permissionDto)).Returns(permission);

            var result = await _sut.CreatePermissionAsync(permissionDto);

            _permissionRepositoryMock.Verify(x => x.CreatePermissionAsync(It.Is<Permission>(x => x.Code == permissionDto.Code
            && x.Description == permissionDto.Description)), Times.Once);
            _mapperMock.Verify(x => x.Map<Permission>(It.Is<PermissionDto>(x => x.Code == permissionDto.Code
            && x.Description == permissionDto.Description)), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var userResult = Assert.IsType<int>(okResult.Value);
            Assert.NotNull(okResult);
            Assert.Equal(userResult, permission.Id);
        }

        [Fact]
        public async Task ShouldDeletePermission()
        {
            int permissionId = 1;
            var result = await _sut.DeletePermissionAsync(permissionId);

            _permissionRepositoryMock.Verify(x => x.DeletePermissionAsync(It.Is<int>(g => g.Equals(permissionId))), Times.Once);

            var okResult = Assert.IsType<OkResult>(result);
            Assert.NotNull(okResult);
        }

        [Fact]
        public async Task ShouldFindAndUpdatePermission()
        {
            var permissionDto = new PermissionDto("UpdateTest", "UpdateDescription");
            var permission = new Permission { Id = 1, Code = "Test", Description = "TestDescription" };
            var updatedPermission = new Permission { Id = 1, Code = permissionDto.Code, Description = permissionDto.Description };
            _mapperMock.Setup(mapper => mapper.Map<Permission>(permissionDto)).Returns(permission);
            _permissionRepositoryMock.Setup(repo => repo.UpdatePermissionAsync(permission)).ReturnsAsync(updatedPermission);

            var result = await _sut.UpdatePermissionAsync(permissionDto);

            _mapperMock.Verify(x => x.Map<Permission>(It.Is<PermissionDto>(u => u.Code == updatedPermission.Code
            && u.Description == updatedPermission.Description)), Times.Once);

            _permissionRepositoryMock.Verify(x => x.UpdatePermissionAsync(It.Is<Permission>(u => u.Code == permission.Code
            && u.Description == permission.Description)), Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var permissionResult = Assert.IsType<Permission>(okResult.Value);

            Assert.NotNull(okResult);
            Assert.Equal(updatedPermission.Code, permissionResult.Code);
            Assert.Equal(updatedPermission.Description, permissionResult.Description);
        }

        [Fact]
        public async Task ShouldNotFindAndUpdatePermission()
        {
            var permissionDto = new PermissionDto("UpdateTest", "UpdateDescription");
            _mapperMock.Setup(mapper => mapper.Map<Permission>(permissionDto)).Returns(new Permission());
            _permissionRepositoryMock.Setup(repo => repo.UpdatePermissionAsync(It.IsAny<Permission>())).ReturnsAsync((Permission)null);

            var result = await _sut.UpdatePermissionAsync(permissionDto);

            _mapperMock.Verify(x => x.Map<Permission>(It.IsAny<PermissionDto>()), Times.Once);

            _permissionRepositoryMock.Verify(x => x.UpdatePermissionAsync(It.IsAny<Permission>()), Times.Once);

            Assert.IsType<NotFoundResult>(result);
        }
        public void Dispose()
        {
            _permissionRepositoryMock.VerifyNoOtherCalls();
            _mapperMock.VerifyNoOtherCalls();
        }
    }
}
