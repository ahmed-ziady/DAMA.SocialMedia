﻿namespace DAMA.Application.DTOs.Posts
{
    public class CreatePostDto
    {
        public string PostContent { get; set; } = null!;
        public int PostTypeId { get; set; }
    }
}