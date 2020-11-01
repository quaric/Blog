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
        //Brukes ikke nå, men kan brukes til å sende oppdatering til alle klienter
        public async Task BroadcastComment(Comment comment)
        {

            await Clients.All.SendAsync("ReceiveComment", comment);

        }
    }
}
