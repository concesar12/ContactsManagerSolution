using Fizzler.Systems.HtmlAgilityPack;
using FluentAssertions;
using HtmlAgilityPack;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CRUDTests
{
    public class PersonsControllerIntegrationTest :IClassFixture<CustomWebApplicationFactory>
    {
        //Creating http client
        private readonly HttpClient _client;

        public PersonsControllerIntegrationTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        #region Index

        [Fact]
        public async Task Index_ToReturnView()
        {
            //Arrange

            //Act
            HttpResponseMessage response = await _client.GetAsync("/Persons/Index");

            //Assert
            response.Should().BeSuccessful(); //2xx

            //To get the repose of body content
            string responseBody = await response.Content.ReadAsStringAsync();

            HtmlDocument html = new HtmlDocument();

            //Bring the html as if Javascript
            html.LoadHtml(responseBody);

            //read document object of the html DOM
            var document = html.DocumentNode;

            document.QuerySelectorAll("table.persons").Should().NotBeNull();
        }

        #endregion Index
    }
}