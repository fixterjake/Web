using Microsoft.AspNetCore.Authorization;
using ZME.API.Shared.Utils;

namespace ZME.API.Extensions;

public static class AuthorizationOptionsExtension
{
    public static void AddAuthPolicies(this AuthorizationOptions options)
    {
        options.AddMemberPolicy();
        options.AddCanAirportsPolicy();
        options.AddCanManageCertificationsPolicy();
        options.AddCanCommentPolicy();
        options.AddCanCommentConfidentialPolicy();
        options.AddCanEmailLogPolicy();
        options.AddCanFaqPolicy();
    }

    public static void AddMemberPolicy(this AuthorizationOptions options)
    {
        options.AddClaimPolicy("IsMember", "IsMember", $"{true}");
    }

    public static void AddSeniorStaffPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("IsSeniorStaff", Constants.SENIOR_STAFF_LIST);
    }

    public static void AddFullStaffPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("IsFullStaff", Constants.FULL_STAFF_LIST);
    }

    public static void AddStaffPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("IsStaff", Constants.ALL_STAFF_LIST);
    }

    public static void AddSeniorTrainingPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("IsSeniorTrainingStaff", Constants.SENIOR_TRAINING_STAFF_LIST);
    }

    public static void AddTrainingPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("IsTrainingStaff", Constants.TRAINING_STAFF_LIST);
    }

    public static void AddCanAirportsPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("CanAirports", Constants.CAN_AIRPORTS_LIST);
    }

    public static void AddCanManageCertificationsPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("CanAirports", Constants.CAN_MANAGE_CERTIFICATIONS_LIST);
    }

    public static void AddCanCommentPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("CanComment", Constants.CAN_COMMENT_LIST);
    }

    public static void AddCanEmailLogPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("CanEmailLog", Constants.CAN_EMAIL_LOG_LIST);
    }

    public static void AddCanFaqPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("CanFaq", Constants.CAN_FAQ_LIST);
    }

    public static void AddCanCommentConfidentialPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("CanCommentConfidential", Constants.CAN_COMMENT_CONFIDENTIAL_LIST);
    }

    public static void AddRolePolicy(this AuthorizationOptions options, string policyName, string[] roles)
    {
        options.AddPolicy(policyName, policy => { _ = policy.RequireRole(roles); });
    }

    public static void AddClaimPolicy(this AuthorizationOptions options, string policyName, string claim,
        string value)
    {
        options.AddPolicy(policyName, policy => { _ = policy.RequireClaim(claim, value); });
    }
}
