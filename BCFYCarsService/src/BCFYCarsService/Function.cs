using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BCFYCarsService;

public class Function
{

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>

    AmazonDynamoDBClient _amazonDynamoDbClient = new();

    public async Task<List<Car>> FunctionHandler(APIGatewayHttpApiV2ProxyRequest input, ILambdaContext context)
    {
        var searchInputs = JsonSerializer.Deserialize<Car>(input.Body);
        var cars = new List<Car>();

        var queryRequest = new QueryRequest
        {
            TableName = "cars",
            IndexName = "make-model-index",
            FilterExpression = "minPrice >= :minPrice and maxPrice <= :maxPrice",
            KeyConditionExpression = "make = :make and model = :model",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                {":make", new AttributeValue {S = searchInputs.Make}},
                {":model", new AttributeValue {S = searchInputs.Model}},
                {":minPrice", new AttributeValue {N = searchInputs.MinPrice.ToString()}},
                {":maxPrice", new AttributeValue {N = searchInputs.MaxPrice.ToString()}}
            }
        };

        var response = await _amazonDynamoDbClient.QueryAsync(queryRequest);
        if (response == null || response.Items.Count == 0)
        {
            return new List<Car>();
        }

        using (DynamoDBContext dbcontext = new DynamoDBContext(_amazonDynamoDbClient))
        {
            foreach (var item in response.Items)
            {
                var doc = Document.FromAttributeMap(item);
                var myModel = dbcontext.FromDocument<Car>(doc);
                cars.Add(myModel);
            }
        }

        return cars;
    }
}
