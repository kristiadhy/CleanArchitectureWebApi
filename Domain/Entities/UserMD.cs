﻿using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;
public class UserMD : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
