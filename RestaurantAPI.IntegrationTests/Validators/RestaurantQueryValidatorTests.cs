using FluentValidation.TestHelper;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI.Models.Validators;

namespace RestaurantAPI.IntegrationTests.Validators;

public class RestaurantQueryValidatorTests
{
    [Theory]
    [MemberData(nameof(GetSampleValidData))]
    public void Validate_ForCorrectModel_ReturnSuccess(RestaurantQuery model)
    {
        // Given
        var validator = new RestaurantQueryValidator();

        // When
        TestValidationResult<RestaurantQuery> result = validator.TestValidate(model);

        // Then
        result.ShouldNotHaveAnyValidationErrors();
    }
    [Theory]
    [MemberData(nameof(GetSampleInvalidData))]
    public void Validate_ForIncorrectModel_ReturnFailure(RestaurantQuery model)
    {
        // Given
        var validator = new RestaurantQueryValidator();

        // When
        TestValidationResult<RestaurantQuery> result = validator.TestValidate(model);

        // Then
        result.ShouldHaveAnyValidationError();
    }
    
    public static IEnumerable<object[]> GetSampleValidData()
    {
        var list = new List<RestaurantQuery>(){
            new RestaurantQuery()
            {
                PageNumber = 1,
                PageSize = 10
            },
            new RestaurantQuery()
            {
                PageNumber = 2,
                PageSize = 15
            },
            new RestaurantQuery()
            {
                PageNumber = 22,
                PageSize = 5,
                SortBy = nameof(Restaurant.Name)
            },
            new RestaurantQuery()
            {
                PageNumber = 12,
                PageSize = 15,
                SortBy = nameof(Restaurant.Category)
            }
        };
        return list.Select(q => new object[] { q });
    }

    public static IEnumerable<object[]> GetSampleInvalidData()
    {
        var list = new List<RestaurantQuery>(){
            new RestaurantQuery()
            {
                PageNumber = 0,
                PageSize = 10
            },
            new RestaurantQuery()
            {
                PageNumber = 2,
                PageSize = 12
            },
            new RestaurantQuery()
            {
                PageNumber = 22,
                PageSize = 5,
                SortBy = nameof(Restaurant.Address)
            },
            new RestaurantQuery()
            {
                PageNumber = 12,
                PageSize = 15,
                SortBy = nameof(Restaurant.ContactNumber)
            }
        };
        return list.Select(q => new object[] { q });
    }

}