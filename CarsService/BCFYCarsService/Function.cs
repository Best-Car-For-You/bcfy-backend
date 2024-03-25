using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BCFYCarsService
{
    public class CarQueryInput
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
    }


    public class Function
    {
        private static AmazonDynamoDBClient client = new AmazonDynamoDBClient();

        private object AttributeValueToObject(AttributeValue value)
        {
            if (value.S != null) return value.S;
            if (value.N != null) return Decimal.Parse(value.N);
            if (value.BOOL != null) return value.BOOL;
            if (value.L != null) return value.L.Select(AttributeValueToObject).ToList();
            if (value.M != null) return value.M.ToDictionary(kv => kv.Key, kv => AttributeValueToObject(kv.Value));
            if (value.NULL != null) return null;
            throw new InvalidOperationException("Unknown AttributeValue type");
        }

        public List<Dictionary<string, object>> ConvertItemsToDictionary(List<Dictionary<string, AttributeValue>> items)
        {
            return items.Select(item => item.ToDictionary(kv => kv.Key, kv => AttributeValueToObject(kv.Value))).ToList();
        }

        public async Task<List<Dictionary<string, object>>> FunctionHandler(CarQueryInput input, ILambdaContext context)
        {
            var items = await QueryCarsByMake(input.Make);
            var filteredItems = items
                .Select(item => ConvertItemsToDictionary(new List<Dictionary<string, AttributeValue>> { item }).First())
                .Where(item =>
                    item["model"].ToString() == input.Model &&
                    (decimal)item["minPrice"] <= input.MinPrice &&
                    (decimal)item["maxPrice"] >= input.MaxPrice
                ).ToList();

            return filteredItems; // This can now be serialized into JSON
        }

        private async Task<List<Dictionary<string, AttributeValue>>> QueryCarsByMake(string make)
        {
            var request = new QueryRequest
            {
                TableName = "cars",
                IndexName = "make-index",
                KeyConditionExpression = "make = :v_make",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> { { ":v_make", new AttributeValue { S = make } } }
            };

            var response = await client.QueryAsync(request);
            return response.Items;
        }
    }
}
