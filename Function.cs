using System;
using System.IO;
using System.Text;

using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using System.Collections.Generic;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace aws_lambda_trigger
{
    public class Function
    {
        private readonly DynamoDBContext _context;

        public Function()
        {
            _context = new DynamoDBContext(new AmazonDynamoDBClient());
        }

        public void FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
        {
            context.Logger.Log($"Beginning to process {dynamoEvent.Records.Count} records...");

            foreach (var record in dynamoEvent.Records)
            {
                context.Logger.Log($"Event ID: {record.EventID}");
                context.Logger.Log($"Event Name: {record.EventName}");
                var user = GetObject<User>(record.Dynamodb.OldImage);
                context.Logger.Log($"Old Image {JsonSerializer.Serialize(user)}");

                 user = GetObject<User>(record.Dynamodb.NewImage);
                context.Logger.Log($"New Image {JsonSerializer.Serialize(user)}");

                // TODO: Add business logic processing the record.Dynamodb object.
            }

            context.Logger.Log("Stream processing complete.");
        }

        private T GetObject<T>(Dictionary<string,AttributeValue> img)
        {
            var doc = Document.FromAttributeMap(img);
            return  _context.FromDocument<T>(doc);

        }
    }


    public class User
    {
        public string Name { get; set; }
        public int Id { get; set; }
       
    }
}