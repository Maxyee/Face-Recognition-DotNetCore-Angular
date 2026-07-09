using Microsoft.AspNetCore.Identity;

namespace Face_Recognition_Demo
{
    public class Role : IdentityRole
    {
        public string? Description { get; set; }
    }
}