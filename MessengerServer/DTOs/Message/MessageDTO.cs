﻿using System;

namespace MessengerServer.DTOs.Message
{
    public class MessageDTO
    {
        public int Id { get; set; }

        public DateTime SendTime { get; set; }

        public string Text { get; set; }

        // nav property
        public byte?[] Image { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; }

        public byte?[] UserAvatar { get; set; }

        public int ChatId { get; set; }
    }
}
