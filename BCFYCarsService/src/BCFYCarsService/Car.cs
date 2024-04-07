using System.Text.Json.Serialization;
using Amazon.DynamoDBv2.DataModel;

namespace BCFYCarsService
{
    [DynamoDBTable("cars")]
    public class Car
    {
        [DynamoDBProperty("id"), JsonPropertyName("id")]
        public string Id { get; set; }

        [DynamoDBProperty("make"), JsonPropertyName("make")]
        public string Make { get; set; }

        [DynamoDBProperty("model"), JsonPropertyName("model")]
        public string Model { get; set; }

        [DynamoDBProperty("minPrice"), JsonPropertyName("minPrice")]
        public decimal MinPrice { get; set; }

        [DynamoDBProperty("maxPrice"), JsonPropertyName("maxPrice")]
        public decimal MaxPrice { get; set; }
    }
}
