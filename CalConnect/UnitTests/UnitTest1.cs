//using CalConnect.Api.Database;
//using CalConnect.Api.Users.Infrastructure;
//using FluentAssertions;
//using FluentEmail.Core;
//using NSubstitute;

//namespace CalConnect.Api.Users.Tests;

//public class RegisterUserTests
//{
//    private readonly IUserRepository _userRepository;
//    private readonly PasswordHasher _passwordHasher;
//    private readonly IFluentEmail _fluentEmail;
//    private readonly ApplicationDbContext _dbContext;
//    private readonly RegisterUser _handler;

//    public RegisterUserTests()
//    {
//        _userRepository = Substitute.For<IUserRepository>();
//        _passwordHasher = Substitute.For<PasswordHasher>();
//        _fluentEmail = Substitute.For<IFluentEmail>();
//        _dbContext = Substitute.For<ApplicationDbContext>();
//        _handler = new RegisterUser(_dbContext, _passwordHasher, _fluentEmail);
//    }

//    #region 成功注册场景

//    [Fact]
//    public async Task Handle_ValidRequest_ShouldReturnCreatedUser()
//    {
//        // Arrange
//        var request = new RegisterUser.Request(
//            Email: "test@example.com",
//            FirstName: "张",
//            LastName: "三",
//            Password: "Password123!"
//        );

//        _userRepository.Exists(request.Email).Returns(false);
//        _passwordHasher.Hash(request.Password).Returns("hashed_password_123");

//        // Act
//        var result = await _handler.Handle(request);

//        // Assert
//        result.Should().NotBeNull();
//        result.Id.Should().NotBeEmpty();
//        result.Email.Should().Be(request.Email);
//        result.FirstName.Should().Be(request.FirstName);
//        result.LastName.Should().Be(request.LastName);
//        result.PasswordHash.Should().Be("hashed_password_123");
//    }

//    [Fact]
//    public async Task Handle_ValidRequest_ShouldCallExistsOnce()
//    {
//        // Arrange
//        var request = CreateValidRequest();
//        _userRepository.Exists(request.Email).Returns(false);

//        // Act
//        await _handler.Handle(request);

//        // Assert
//        await _userRepository.Received(1).Exists(request.Email);
//    }

//    [Fact]
//    public async Task Handle_ValidRequest_ShouldCallPasswordHasherOnce()
//    {
//        // Arrange
//        var request = CreateValidRequest();
//        _userRepository.Exists(request.Email).Returns(false);

//        // Act
//        await _handler.Handle(request);

//        // Assert
//        _passwordHasher.Received(1).Hash(request.Password);
//    }

//    [Fact]
//    public async Task Handle_ValidRequest_ShouldCallInsertWithCorrectUser()
//    {
//        // Arrange
//        var request = CreateValidRequest();
//        _userRepository.Exists(request.Email).Returns(false);
//        _passwordHasher.Hash(request.Password).Returns("hashed_xxx");

//        // Act
//        var result = await _handler.Handle(request);

//        // Assert
//        await _userRepository.Received(1).Insert(Arg.Is<User>(u =>
//            u.Email == request.Email &&
//            u.FirstName == request.FirstName &&
//            u.LastName == request.LastName &&
//            u.PasswordHash == "hashed_xxx" &&
//            u.Id != Guid.Empty
//        ));
//    }

//    #endregion

//    #region 邮箱已存在场景

//    [Fact]
//    public async Task Handle_EmailAlreadyExists_ShouldThrowException()
//    {
//        // Arrange
//        var request = CreateValidRequest();
//        _userRepository.Exists(request.Email).Returns(true);

//        // Act
//        Func<Task> act = async () => await _handler.Handle(request);

//        // Assert
//        await act.Should().ThrowAsync<Exception>()
//            .WithMessage("当前邮箱已被使用");
//    }

//    [Fact]
//    public async Task Handle_EmailAlreadyExists_ShouldNotCallPasswordHasherOrInsert()
//    {
//        // Arrange
//        var request = CreateValidRequest();
//        _userRepository.Exists(request.Email).Returns(true);

//        // Act
//        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request));

//        // Assert
//        _passwordHasher.Received(0).Hash(Arg.Any<string>());
//        await _userRepository.Received(0).Insert(Arg.Any<User>());
//    }

//    #endregion

//    #region 输入验证场景

//    [Theory]
//    [InlineData(null, "张", "三", "Password123!")]
//    [InlineData("", "张", "三", "Password123!")]
//    [InlineData("test@example.com", null, "三", "Password123!")]
//    [InlineData("test@example.com", "", "三", "Password123!")]
//    [InlineData("test@example.com", "张", null, "Password123!")]
//    [InlineData("test@example.com", "张", "", "Password123!")]
//    [InlineData("test@example.com", "张", "三", null)]
//    [InlineData("test@example.com", "张", "三", "")]
//    public async Task Handle_InvalidInput_ShouldStillCallExists(
//        string email, string firstName, string lastName, string password)
//    {
//        // Arrange
//        var request = new RegisterUser.Request(email, firstName, lastName, password);
//        _userRepository.Exists(Arg.Any<string>()).Returns(false);

//        // Act
//        await _handler.Handle(request);

//        // Assert
//        await _userRepository.Received(1).Exists(email);
//    }

//    #endregion

//    #region 辅助方法

//    private static RegisterUser.Request CreateValidRequest() =>
//        new RegisterUser.Request(
//            Email: "newuser@example.com",
//            FirstName: "李",
//            LastName: "四",
//            Password: "SecurePass123!"
//        );

//    #endregion
//}