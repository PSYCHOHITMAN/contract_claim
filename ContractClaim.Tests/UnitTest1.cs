using contract_claim.Controllers;
using contract_claim.Data;
using contract_claim.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text;

public class TestSession : ISession
{
    private Dictionary<string, byte[]> _sessionStorage = new();

    public bool IsAvailable => true;
    public string Id => "TestSessionId";
    public IEnumerable<string> Keys => _sessionStorage.Keys;

    public void Clear() => _sessionStorage.Clear();

    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public void Remove(string key) => _sessionStorage.Remove(key);

    public void Set(string key, byte[] value) => _sessionStorage[key] = value;

    public bool TryGetValue(string key, out byte[] value) => _sessionStorage.TryGetValue(key, out value);
}
public class AccountControllerTests
{
    private AccountController GetControllerWithSession()
    {
        var controller = new AccountController();
        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        controller.HttpContext.Session = new TestSession(); // custom fake session
        return controller;
    }

    [Fact]
    public void Login_Should_Redirect_To_Lecturer_Index_WhenRoleIsLecturer()
    {
        // Arrange
        var user = new User
        {
            Username = "Lecturer Test",
            Email = "lecturer@example.com",
            Password = "pass123",
            Role = "Lecturer"
        };
        UserRepository.Add(user);

        var controller = GetControllerWithSession();

        // Act
        var result = controller.Login("lecturer@example.com", "pass123") as RedirectToActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Index", result.ActionName);
        Assert.Equal("Lecturer", result.ControllerName);
    }
}
public class UserRepositoryTests
{
    [Fact]
    public void AddUser_Should_SaveUser()
    {
        // Arrange
        var user = new User
        {
            Username = "Test User",
            Email = "test@example.com",
            Password = "password123",
            Role = "Lecturer"
        };

        // Act
        UserRepository.Add(user);
        var savedUser = UserRepository.Find("test@example.com", "password123");

        // Assert
        Assert.NotNull(savedUser);
        Assert.Equal("test@example.com", savedUser.Email);
        Assert.Equal("Lecturer", savedUser.Role);
    }

    [Fact]
    public void Exists_Should_ReturnTrue_IfEmailExists()
    {
        // Arrange
        string email = "check@example.com";
        var user = new User
        {
            Username = "Check User",
            Email = email,
            Password = "secret",
            Role = "Manager"
        };
        UserRepository.Add(user);

        // Act
        bool exists = UserRepository.Exists(email);

        // Assert
        Assert.True(exists);
    }
}