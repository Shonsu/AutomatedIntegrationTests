using System.Diagnostics;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.IntegrationTests.Abstractions;
using RestaurantAPI.Models;

namespace RestaurantAPI.IntegrationTests
{
    public class DishControllerTests : BaseIntegrationTest
    {

        public DishControllerTests(IntegrationTestWebFactory factory) : base(factory)
        {
        }

        [Fact]
        public async void Delete_ForExistingRestaurantDishes_ReturnsNoContent()
        {
            // arrange
            CleanDatabase();

            List<Restaurant> list = GetRestaurants();
            SeedRestaraunts(list);
            Restaurant restaurant = list.Where(r => r.Dishes.Count > 0).First();

            // act
            HttpResponseMessage response = await HttpClient.DeleteAsync($"/api/restaurant/{restaurant.Id}/dish");

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async void Delete_ForNonExistingRestaurant_ReturnsNotFound()
        {
            // arrange
            CleanDatabase();
            var restaurantId = new Random().Next(0, int.MaxValue);

            // act
            HttpResponseMessage response = await HttpClient.DeleteAsync($"/api/restaurant/{restaurantId}/dish");

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        }

        [Fact]
        public async Task GetDishForRestaurant_ForExistingRestaurantAndItsDishes_ReturnsOk()
        {
            // Given
            CleanDatabase();

            List<Restaurant> list = GetRestaurants();
            SeedRestaraunts(list);
            Restaurant restaurant = list.Where(r => r.Dishes.Count > 0).First();
            Dish? dish = restaurant.Dishes.First();
            // When
            HttpResponseMessage response = await HttpClient.GetAsync($"/api/restaurant/{restaurant.Id}/dish/{dish!.Id}");

            // Then
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            DishDto? dishDto = await response.Content.ReadFromJsonAsync<DishDto>();
            dishDto!.Id.Should().Be(dish.Id);
        }

        [Fact]
        public async Task GetDishForRestaurant_ForExistingRestaurantAndNotExistingDish_ReturnsNotFound()
        {
            // Given
            CleanDatabase();

            var restaurant = new Restaurant()
            {
                Name = "Ali kebab",
                Address = new Address()
                {
                    City = "Kraków",
                    Street = "Długa 5",
                    PostalCode = "30-001"
                }
            };
            SeedRestaraunts([restaurant]);
            var dishId = 1;

            // When
            HttpResponseMessage response = await HttpClient.GetAsync($"/api/restaurant/{restaurant.Id}/dish/{dishId}");

            // Then
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetDishForRestaurant_ForNotExistingRestaurantAndNotExistingDish_ReturnsNotFound()
        {
            // Given
            CleanDatabase();

            var restaurantId = 1;
            var dishId = 1;

            // When
            HttpResponseMessage response = await HttpClient.GetAsync($"/api/restaurant/{restaurantId}/dish/{dishId}");

            // Then
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }


        [Fact]
        public async void Post_ForValidModelAndExistingRestaurant_ReturnsCreatedStatus()
        {
            // Given
            CleanDatabase();

            var restaurant = new Restaurant()
            {
                Name = "Ali kebab",
                Address = new Address()
                {
                    City = "Kraków",
                    Street = "Długa 5",
                    PostalCode = "30-001"
                }
            };
            SeedRestaraunts([restaurant]);

            var model = new CreateDishDto() { Name = "Some Dish" };
            HttpContent httpContent = model.ToJsonHttpContent();

            // When
            HttpResponseMessage response = await HttpClient.PostAsync($"/api/restaurant/{restaurant.Id}/dish", httpContent);

            // Then
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();
        }

        [Fact]
        public async void Post_ForInvalidModelAndExistingRestaurant_ReturnsBadRequest()
        {
            // Given
            CleanDatabase();

            var restaurant = new Restaurant()
            {
                Name = "Ali kebab",
                Address = new Address()
                {
                    City = "Kraków",
                    Street = "Długa 5",
                    PostalCode = "30-001"
                }
            };
            SeedRestaraunts([restaurant]);

            var model = new CreateDishDto() { Name = "" };
            HttpContent httpContent = model.ToJsonHttpContent();

            // When
            HttpResponseMessage response = await HttpClient.PostAsync($"/api/restaurant/{restaurant.Id}/dish", httpContent);

            // Then
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async void Post_ForValidModelAndNotExistingRestaurant_ReturnsNotFound()
        {
            // Given
            CleanDatabase();

            var restaurantId = new Random().Next(2048, int.MaxValue); ;

            var model = new CreateDishDto() { Name = "Some Dish name" };
            HttpContent httpContent = model.ToJsonHttpContent();

            // When
            HttpResponseMessage response = await HttpClient.PostAsync($"/api/restaurant/{restaurantId}/dish", httpContent);

            // Then
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetDishesForRestaurant_ForExistingRestaurant_ResultsOK()
        {
            // Given
            CleanDatabase();

            List<Restaurant> list = GetRestaurants();
            SeedRestaraunts(list);
            Restaurant restaurant = list.Where(r => r.Dishes.Count > 0).First();

            // When
            HttpResponseMessage response = await HttpClient.GetAsync($"/api/restaurant/{restaurant.Id}/dish");

            // Then
            List<DishDto>? dishDtos = await response.Content.ReadFromJsonAsync<List<DishDto>>();
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            dishDtos.Should().NotBeEmpty();
        }

        [Fact]
        public async void GetDishesForRestaurant_ForNotExistingRestaurant_ResultsNotFound()
        {
            // Given
            CleanDatabase();

            var restaurantId = new Random().Next(0, int.MaxValue);

            // When
            HttpResponseMessage response = await HttpClient.GetAsync($"/api/restaurant/{restaurantId}/dish");

            // Then
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        private void SeedRestaraunts(List<Restaurant> data)
        {
            DbContext.Restaurants.AddRange(data);
            DbContext.SaveChanges();
        }

        private void CleanDatabase()
        {
            List<Restaurant> restaurants = DbContext.Restaurants.ToList();
            DbContext.Restaurants.RemoveRange(restaurants);
            List<Dish> dishes = DbContext.Dishes.ToList();
            DbContext.Dishes.RemoveRange(dishes);
            DbContext.SaveChanges();
        }
        private List<Restaurant> GetRestaurants()
        {
            var restaurants = new List<Restaurant>()
                {
                    new Restaurant()
                    {
                        Name = "KFC",
                        Dishes = new List<Dish>()
                        {
                            new Dish()
                            {
                                Name = "Nashville Hot Chicken",
                                Price = 10.30M,
                            },
                            new Dish()
                            {
                                Name = "Chicken Nuggets",
                                Price = 5.30M,
                            },
                        },
                        Address = new Address()
                        {
                            City = "Kraków",
                            Street = "Długa 5",
                            PostalCode = "30-001"
                        }
                    },
                    new Restaurant()
                    {
                        Name = "Tropico",
                        Dishes = new List<Dish>()
                        {
                            new Dish()
                            {
                                Name = "Dish1",
                                Price = 10.30M,
                            },

                            new Dish()
                            {
                                Name = "Dish2",
                                Price = 5.30M,
                            },
                        },
                        Address = new Address()
                        {
                            City = "Kraków",
                            Street = "Długa 5",
                            PostalCode = "30-001"
                        }
                    },
                    new Restaurant()
                    {
                        // Id = 301,
                        Name = "Ali kebab",
                        Address = new Address()
                        {
                            City = "Kraków",
                            Street = "Długa 5",
                            PostalCode = "30-001"
                        }
                    }
                };

            return restaurants;
        }
    }
}