using Microsoft.AspNetCore.Identity;

namespace Face_Recognition_Demo.Models;

public class ApplicationUser : IdentityUser
{
    public string? FaceEmbedding {get;set;}
    public bool FaceEnrolled {get;set;}
}
