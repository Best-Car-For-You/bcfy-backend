using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BCFYCarsService
{
    public class Function
    {
        private static AmazonDynamoDBClient client = new AmazonDynamoDBClient();

        /// <summary>
        /// A simple function that queries the "Cars" DynamoDB table by make and returns the full items.
        /// </summary>
        /// <param name="make">The make of the cars to query for.</param>
        /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
        /// <returns>A task that represents the asynchronous operation, containing a list of dictionaries representing the queried items.</returns>
        /// 

        private object AttributeValueToObject(AttributeValue value)
        {
            if (value.S != null)
                return value.S;
            if (value.N != null)
                return value.N;
            if (value.BOOL != null)
                return value.BOOL;
            if (value.L != null)
                return value.L.Select(AttributeValueToObject).ToList();
            if (value.M != null)
                return value.M.ToDictionary(kv => kv.Key, kv => AttributeValueToObject(kv.Value));
            if (value.NULL != null)
                return null;
            // Add more cases as needed for your specific use case (e.g., binary data types, number sets, etc.)
            throw new InvalidOperationException("Unknown AttributeValue type");
        }

        public List<Dictionary<string, object>> ConvertItemsToDictionary(List<Dictionary<string, AttributeValue>> items)
        {
            return items.Select(item => item.ToDictionary(
                kv => kv.Key,
                kv => AttributeValueToObject(kv.Value)
            )).ToList();
        }


        public async Task<List<Dictionary<string, object>>> FunctionHandler(string make, ILambdaContext context)
        {
            var items = await QueryCarsByMake(make);
            var convertedItems = ConvertItemsToDictionary(items);
            return convertedItems; // This can now be serialized into JSON
        }


        /// <summary>
        /// Queries the "Cars" DynamoDB table for cars by their make.
        /// </summary>
        /// <param name="make">The make of the car.</param>
        /// <returns>A task that represents the asynchronous operation and contains the query results.</returns>
        private async Task<List<Dictionary<string, AttributeValue>>> QueryCarsByMake(string make)
        {
            var request = new QueryRequest
            {
                TableName = "cars",
                IndexName = "make-index", // Adjust this to match your GSI name
                KeyConditionExpression = "make = :v_make",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":v_make", new AttributeValue { S = make }}
                }
            };

            var response = await client.QueryAsync(request);
            return response.Items;
        }
    }
}
