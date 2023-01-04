

using ApiMedalkin;
using ApiMedalkin.Configuration;
using Microsoft.Extensions.Options;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddSwaggerGen();
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);
builder.Services.Configure<BotConfiguration>(builder.Configuration);

builder.Services.ConfigureServices();

builder.Services
    .AddHttpClient("medalkinwebhook")
    .AddTypedClient<ITelegramBotClient>((client, sp) =>
    {
        var configuration = sp.GetRequiredService<IOptionsMonitor<BotConfiguration>>();
        return new TelegramBotClient(configuration.CurrentValue.BotToken, client);
    });

var app = builder.Build();


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseSwagger();

app.MapGet("/", () => "Ok");

app.UseSwaggerUI();

app.Run();
