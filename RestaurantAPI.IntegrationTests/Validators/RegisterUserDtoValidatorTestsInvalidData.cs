using System.Collections;
using RestaurantAPI.Models;

namespace RestaurantAPI.IntegrationTests.Validators;

public class RegisterUserDtoValidatorTestsInvalidData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        var list = new List<RegisterUserDto>{
        new RegisterUserDto()
            {
                Email = "testtest.pl",
                Password = "qwer123",
                ConfirmPassword = "qwer123",
                Nationality = "Polish",
                DateOfBirth = new DateTime(1990, 1, 1)
            },
            new RegisterUserDto()
            {
                Email = "test2@test.pl",
                Password = "qwe123",
                ConfirmPassword = "qwe123",
                Nationality = "Polish",
                DateOfBirth = new DateTime(1990, 1, 1)
            },
            new RegisterUserDto()
            {
                Email = "test32@test.pl",
                Password = "qwe23",
                ConfirmPassword = "qwe123",
                Nationality = "Asia",
                DateOfBirth = new DateTime(1990, 1, 1)
            },
            new RegisterUserDto()
            {
                Email = "test32@test.pl",
                Password = "qwe23",
                ConfirmPassword = "qwe23",
                Nationality = "Asia",
                DateOfBirth = new DateTime(1990, 1, 1)
            }
    };
        return list.Select(u => new object[] { u }).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}