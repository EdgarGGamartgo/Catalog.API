using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Catalog.Domain.Entities;
using Catalog.Domain.Requests.Item;
using Catalog.Domain.Responses;
using Catalog.Domain.Responses.Item;
using Catalog.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;
using Catalog.API.ResponseModels;

namespace Catalog.API.Tests.Controllers
{
    public class ItemControllerTests : IClassFixture<InMemoryApplicationFactory<Startup>>
    {
        private readonly InMemoryApplicationFactory<Startup> _factory;

        public ItemControllerTests(InMemoryApplicationFactory<Startup> factory)
        {
            _factory = new InMemoryApplicationFactory<Startup>(); //factory; 
        }
        
        [Theory]
        [InlineData("/api/items/?pageSize=1&pageIndex=0", 1, 0)]
        [InlineData("/api/items/?pageSize=2&pageIndex=0", 2, 0)]
        [InlineData("/api/items/?pageSize=1&pageIndex=1", 1, 1)]
        public async Task get_should_return_paginated_data(string url, int pageSize, int pageIndex)
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseEntity = JsonConvert.DeserializeObject<PaginatedItemResponseModel<ItemResponse>>(responseContent);

            responseEntity.PageIndex.ShouldBe(pageIndex);
            responseEntity.PageSize.ShouldBe(pageSize);
            responseEntity.Data.Count().ShouldBe(pageSize);
        }

        [Theory]
        [InlineData("/api/items")]
        public async Task get_should_return_success(string url)
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();
        }

        
          [Theory]
          [LoadData("item")]
         
        public async Task get_by_id_should_return_item_data(Item request) 
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync($"api/items/{request.Id}");

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseEntity = JsonConvert.DeserializeObject<ItemResponse>(responseContent); 

            
              responseEntity.Name.ShouldBe(request.Name);
              responseEntity.Description.ShouldBe(request.Description);
              responseEntity.Price.Amount.ShouldBe(request.Price.Amount);
              responseEntity.Price.Currency.ShouldBe(request.Price.Currency);
              responseEntity.Format.ShouldBe(request.Format);
              responseEntity.PictureUri.ShouldBe(request.PictureUri);
              responseEntity.GenreId.ShouldBe(request.GenreId);
              responseEntity.ArtistId.ShouldBe(request.ArtistId);
             
        }
        
        [Theory]
        [LoadData("item")]
        public async Task add_should_create_new_record(AddItemRequest request)
        {
            var client = _factory.CreateClient();

            var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"/api/items", httpContent);

            response.EnsureSuccessStatusCode();
            response.Headers.Location.ShouldNotBeNull();
        }
        
        [Theory]
        [LoadData("item")]
        public async Task update_should_modify_existing_items(EditItemRequest request)
        {
            var client = _factory.CreateClient();

            var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/items/{request.Id}", httpContent);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseEntity = JsonConvert.DeserializeObject<ItemResponse>(responseContent);

            responseEntity.Name.ShouldBe(request.Name);
            responseEntity.Description.ShouldBe(request.Description);
            responseEntity.Format.ShouldBe(request.Format);
            responseEntity.PictureUri.ShouldBe(request.PictureUri);
            responseEntity.GenreId.ShouldBe(request.GenreId);
            responseEntity.ArtistId.ShouldBe(request.ArtistId);
        }

       
    }
}