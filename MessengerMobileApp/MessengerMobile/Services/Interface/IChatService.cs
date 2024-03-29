﻿using System;
using System.Threading.Tasks;

namespace Messenger.Services.Interfaces
{
    public interface IChatService
    {
        Task Connect();
        Task Disconnect();
        Task SendMessage(string userId, string message);
        void ReceiveMessage(Action<string, string> GetMessageAndUser);
    }
}
