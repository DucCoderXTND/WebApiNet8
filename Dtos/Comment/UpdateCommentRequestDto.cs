using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Comment
{
    public class UpdateCommentRequestDto
    {
        [Required]
        [MinLength(5, ErrorMessage = "Title cannot be less than 5 characters")]
        [MaxLength(30, ErrorMessage = "Title cannot be over 30 over characters")]
        public string Title { get; set; } = string.Empty;
        [Required]
        [MinLength(5, ErrorMessage = "Content cannot be less than 5 characters")]
        [MaxLength(30, ErrorMessage = "Content cannot be over 30 over characters")]
        public string Content { get; set; } = string.Empty;
    }
}