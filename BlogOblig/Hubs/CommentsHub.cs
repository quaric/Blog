using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogOblig.Models.Entities;
using Microsoft.AspNetCore.SignalR;

namespace BlogOblig.Hubs
{
    public class CommentsHub : Hub
    {
        public async Task BroadcastComment(Comment comment)
        {

            await Clients.All.SendAsync("ReceiveComment", comment);

        }
    }
}
