using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace CoworkingBooking.Api.Classes
{
    public sealed class KeycloakClaimsTransformation : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(
            ClaimsPrincipal principal)
        {
            var identity = principal.Identities
                .FirstOrDefault(identity => identity.IsAuthenticated);

            if (identity is null)
                return Task.FromResult(principal);

            // Obtém a claim realm_access do JWT.
            var realmAccessClaim = principal.FindFirst("realm_access");

            if (realmAccessClaim is null)
                return Task.FromResult(principal);

            // O valor de realm_access contém um objeto JSON.
            using var document = JsonDocument.Parse(
                realmAccessClaim.Value);

            var root = document.RootElement;

            if (!root.TryGetProperty("roles", out var roles) ||
                roles.ValueKind != JsonValueKind.Array)
            {
                return Task.FromResult(principal);
            }

            foreach (var role in roles.EnumerateArray())
            {
                var roleName = role.GetString();

                if (string.IsNullOrWhiteSpace(roleName))
                    continue;

                // Evita adicionar a mesma role mais de uma vez.
                if (identity.HasClaim(identity.RoleClaimType, roleName))
                    continue;

                identity.AddClaim(
                    new Claim(identity.RoleClaimType, roleName));
            }

            return Task.FromResult(principal);
        }
    }
}