using Microsoft.AspNetCore.SignalR;
using OpenCvSharp;
using RTSPListener.API.Hub;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll"); // Enable CORS
app.MapHub<RTSPHub>("/rtspHub");

app.MapGet("/", () => "RTSP Listener is running");

string rtspUrl = "rtsp://rtspstream:8912a3e49987583ad6c9204dcc39fd35@zephyr.rtsp.stream/pattern";
using var videoCapture = new VideoCapture(rtspUrl);

if (!videoCapture.IsOpened())
{
    throw new Exception("Cannot open the RTSP stream");
}

var hubContext = app.Services.GetRequiredService<IHubContext<RTSPHub>>();

while (true)
{
    using var frame = new Mat();
    if (!videoCapture.Read(frame)) break;

    var imageBytes = frame.ToBytes(".jpg");
    await hubContext.Clients.All.SendAsync("ReceiveFrame", imageBytes);

    await Task.Delay(33); // 30 fps frame delay
}

app.Run();
