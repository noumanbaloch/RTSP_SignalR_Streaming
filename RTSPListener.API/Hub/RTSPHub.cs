using Microsoft.AspNetCore.SignalR;

namespace RTSPListener.API.Hub;

public class RTSPHub : Microsoft.AspNetCore.SignalR.Hub
{
    public async Task SendFrame(byte[] imageBytes)
    {
        await Clients.All.SendAsync("ReceiveFrame", imageBytes);
    }
}