using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI.Models.Validators;

namespace RestaurantAPI.IntegrationTests.Validators;

public class RegisterUserDtoValidatorTests
{
    private readonly RestaurantDbContext _dbContext;

    public RegisterUserDtoValidatorTests()
    {
        var builder = new DbContextOptionsBuilder<RestaurantDbContext>();
        builder.UseInMemoryDatabase("TestDB");

        _dbContext = new RestaurantDbContext(builder.Options);
        Seed();
    }

    [Theory]
    [ClassData(typeof(RegisterUserDtoValidatorTestsValidData))]
    public void Validate_ForValidModel_ReturnsSuccess(RegisterUserDto model)
    {
        // Given
        var validator = new RegisterUserDtoValidator(_dbContext);

        // When
        TestValidationResult<RegisterUserDto> validationResult = validator.TestValidate(model);

        // Then
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [ClassData(typeof(RegisterUserDtoValidatorTestsInvalidData))]
    public void Validate_ForInvalidModel_ReturnsFailure(RegisterUserDto model)
    {
        // Given
        var validator = new RegisterUserDtoValidator(_dbContext);

        // When
        TestValidationResult<RegisterUserDto> validationResult = validator.TestValidate(model);

        // Then
        validationResult.ShouldHaveAnyValidationError();
    }

    private void Seed()
    {
        var testUsers = new List<User>(){
            new User()
                {
                    Email = "test2@test.pl",
                },
           new User()
                {
                    Email = "test3@test.pl",
                }
        };
        _dbContext.Users.AddRange(testUsers);
        _dbContext.SaveChanges();
    }
}