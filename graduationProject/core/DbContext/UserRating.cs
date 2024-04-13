﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace graduationProject.core.DbContext
{
    public class UserRating
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int Rate { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }
    }
}
