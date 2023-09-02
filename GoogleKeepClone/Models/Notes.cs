using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace GoogleKeepClone.Models
{
    public class Notes
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Color { get; set; }
        public DateTime CreatedDate { get; set; }

        public IdentityUser? User { get; set; }
        public string? UserId { get; set; }
    }
}