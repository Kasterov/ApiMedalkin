using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using ApiMedalkin.Services;

namespace ApiMedalkin.Configuration;

public static class Configuration
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IUpdateService, UpdateService>();
        services.AddSingleton<IAmazonDynamoDB>(new AmazonDynamoDBClient(RegionEndpoint.EUCentral1));
        services.AddScoped<IDynamoDBContext, DynamoDBContext>();
    }
}
