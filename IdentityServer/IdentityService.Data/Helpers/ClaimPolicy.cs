namespace IdentityService.Data.Helpers
{
    public static class ClaimPolicy
    {
        public const string UserClaimPolicy = nameof(UserClaimPolicy);
        public const string SuperUserClaimPolicy = nameof(SuperUserClaimPolicy);
        public const string BannedUserRolePolicy = nameof(BannedUserRolePolicy);
    }
}
