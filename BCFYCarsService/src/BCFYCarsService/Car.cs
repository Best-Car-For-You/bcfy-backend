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

        [DynamoDBProperty("transmission"), JsonPropertyName("transmission")]
        public string Transmission { get; set; }

        [DynamoDBProperty("fuelType"), JsonPropertyName("fuelType")]
        public string FuelType { get; set; }

        [DynamoDBProperty("year"), JsonPropertyName("year")]
        public int Year { get; set; }

        [DynamoDBProperty("numberOfDoors"), JsonPropertyName("numberOfDoors")]
        public int NumberOfDoors { get; set; }

        [DynamoDBProperty("numberOfSeats"), JsonPropertyName("numberOfSeats")]
        public int NumberOfSeats { get; set; }

        [DynamoDBProperty("insuranceGroup"), JsonPropertyName("insuranceGroup")]
        public int InsuranceGroup { get; set; }

        [DynamoDBProperty("engineSize"), JsonPropertyName("engineSize")]
        public string EngineSize { get; set; }

        [DynamoDBProperty("version"), JsonPropertyName("version")]
        public string Version { get; set; }
    }
}
