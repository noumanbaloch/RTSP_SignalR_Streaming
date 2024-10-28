using Microsoft.AspNetCore.SignalR.Client;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapGet("/", () => "Image Receiver is running");

var connection = new HubConnectionBuilder()
    .WithUrl("https://localhost:44321/rtspHub")
    .Build();

connection.On<byte[]>("ReceiveFrame", async imageBytes =>
{
    var filePath = Path.Combine("ReceivedImages", $"frame_{Guid.NewGuid()}.jpg");
    await File.WriteAllBytesAsync(filePath, imageBytes);
});

await connection.StartAsync();
await app.RunAsync();