using System;
using System.Collections.Generic;

namespace HealthMonitoring.Identity.Application.Authentication
{
    /// <summary>
    /// Result of authentication
    /// </summary>
    public class AuthenticationResult
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}