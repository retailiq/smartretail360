using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Enums.AccessControl;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Persistence.Seed.AccessControl;

public static class AbacPolicyTemplateSeeder
{
    private static readonly string RoleAdmin = SystemRoleType.Admin.GetEnumMemberValue();
    private static readonly string RoleOwner = SystemRoleType.Owner.GetEnumMemberValue();
    private static readonly string RoleSysAdmin = SystemRoleType.SystemAdmin.GetEnumMemberValue();
    private static readonly string RoleSysStaff = SystemRoleType.SystemStaff.GetEnumMemberValue();
    private static readonly string RoleSysBot = SystemRoleType.Bot.GetEnumMemberValue();
    private static readonly string RoleSysAudit = SystemRoleType.ReadOnlyAuditor.GetEnumMemberValue();
    private static readonly string EnvClient = DefaultEnvironmentType.Client.GetEnumMemberValue();
    private static readonly string EnvAdmin = DefaultEnvironmentType.Admin.GetEnumMemberValue();

    public static IEnumerable<AbacPolicyTemplate> GetTemplates()
        => UserPolicyTemplates()
            .Concat(TenantPolicyTemplates())
            .Concat(AbacPolicyTemplates());

    private static IEnumerable<AbacPolicyTemplate> UserPolicyTemplates()
    {
        return
        [
            // ✅ user:view — Self or Admin/Owner of the same tenant and System
            new AbacPolicyTemplate
            {
                TemplateName = "ScopedUserView",
                ResourceType = DefaultResourceType.User.GetEnumMemberValue(),
                Action = DefaultActionType.View.GetEnumMemberValue(),
                RuleJson = $$$"""
                              {
                                "and": [
                                  { "in": [ { "var": "environment.name" }, [ "{{{EnvClient}}}", "{{{EnvAdmin}}}" ] ] },
                                  { "==": [ { "var": "tenant_constraints.access_allowed" }, true ] },
                                  {
                                    "or": [
                                      { "==": [ { "var": "user.id" }, { "var": "resource.user_id" } ] },
                                      {
                                        "some": [
                                          { "var": "resource.tenant_roles" },
                                          {
                                            "and": [
                                              { "==": [ { "var": "user.tenant_id" }, { "var": "tenant_id" } ] },
                                              { "in": [ { "var": "user.role_name" }, [ "{{{RoleAdmin}}}", "{{{RoleOwner}}}" ] ] }
                                            ]
                                          }
                                        ]
                                      },
                                      {
                                        "and": [
                                          { "==": [ { "var": "user.is_system_account" }, true ] },
                                          { "in": [ { "var": "user.role_name" }, [ "{{{RoleSysBot}}}", "{{{RoleSysAdmin}}}", "{{{RoleSysStaff}}}" ] ] }
                                        ]
                                      }
                                    ]
                                  }
                                ]
                              }
                              """,
                IsEnabled = true
            },

            // ✅ user:edit — Self or System
            new AbacPolicyTemplate
            {
                TemplateName = "ScopedUserEdit",
                ResourceType = DefaultResourceType.User.GetEnumMemberValue(),
                Action = DefaultActionType.Edit.GetEnumMemberValue(),
                RuleJson = $$$"""
                              {
                                "and": [
                                  { "in": [ { "var": "environment.name" }, [ "{{{EnvClient}}}", "{{{EnvAdmin}}}" ] ] },
                                  { "==": [ { "var": "tenant_constraints.access_allowed" }, true ] },
                                  {
                                    "or": [
                                      { "==": [ { "var": "user.id" }, { "var": "resource.user_id" } ] },
                                      {
                                        "and": [
                                          { "==": [ { "var": "user.is_system_account" }, true ] },
                                          { "in": [ { "var": "user.role_name" }, [ "{{{RoleSysBot}}}", "{{{RoleSysAdmin}}}", "{{{RoleSysStaff}}}" ] ] }
                                        ]
                                      }
                                    ]
                                  }
                                ]
                              }
                              """,
                IsEnabled = true
            },

            // ✅ user:remove — Only admin/owner removes others
            new AbacPolicyTemplate
            {
                TemplateName = "ScopedUserRemove",
                ResourceType = DefaultResourceType.User.GetEnumMemberValue(),
                Action = DefaultActionType.Remove.GetEnumMemberValue(),
                RuleJson = $$$"""
                              {
                                "and": [
                                  { "in": [ { "var": "environment.name" }, [ "{{{EnvClient}}}" ] ] },
                                  { "==": [ { "var": "tenant_constraints.access_allowed" }, true ] },
                                  { "!=": [ { "var": "user.id" }, { "var": "resource.user_id" } ] },
                                  {
                                      "some": [
                                          { "var": "resource.tenant_roles" },
                                          {
                                            "and": [
                                              { "==": [ { "var": "user.tenant_id" }, { "var": "tenant_id" } ] },
                                              { "in": [ { "var": "user.role_name" }, [ "{{{RoleAdmin}}}", "{{{RoleOwner}}}" ] ] }
                                            ]
                                          }
                                      ]
                                   }
                                ]
                              }
                              """,
                IsEnabled = true
            },

            // ✅ user:delete — Self delete only
            new AbacPolicyTemplate
            {
                TemplateName = "SelfScopedUserDelete",
                ResourceType = DefaultResourceType.User.GetEnumMemberValue(),
                Action = DefaultActionType.Delete.GetEnumMemberValue(),
                RuleJson = $$$"""
                              {
                                "and": [
                                  { "in": [ { "var": "environment.name" }, [ "{{{EnvClient}}}" ] ] },
                                  { "==": [ { "var": "tenant_constraints.access_allowed" }, true ] },
                                  { "==": [ { "var": "user.id" }, { "var": "resource.user_id" } ] }
                                ]
                              }
                              """,
                IsEnabled = true
            },

            // ✅ user:leave — Self leave only
            new AbacPolicyTemplate
            {
                TemplateName = "SelfScopedUserLeave",
                ResourceType = DefaultResourceType.User.GetEnumMemberValue(),
                Action = DefaultActionType.Leave.GetEnumMemberValue(),
                RuleJson = $$$$"""
                               {
                                 "and": [
                                   { "in": [ { "var": "environment.name" }, [ "{{{{EnvClient}}}}" ] ] },
                                   { "==": [ { "var": "tenant_constraints.access_allowed" }, true ] },
                                   { "==": [ { "var": "user.id" }, { "var": "resource.user_id" } ] },
                                   {
                                     "not": {
                                       "in": [ { "var": "user.role_name" }, [ "{{{{RoleAdmin}}}}", "{{{{RoleOwner}}}}" ] ]
                                     }
                                   },
                                   {
                                     "some": [
                                       { "var": "resource.tenant_roles" },
                                       {
                                         "==": [ { "var": "user.tenant_id" }, { "var": "tenant_id" } ]
                                       }
                                     ]
                                   }
                                 ]
                               }
                               """,
                IsEnabled = true
            },

            // ✅ user:suspend — only system roles
            new AbacPolicyTemplate
            {
                TemplateName = "ScopedUserSuspend",
                ResourceType = DefaultResourceType.User.GetEnumMemberValue(),
                Action = DefaultActionType.Suspend.GetEnumMemberValue(),
                RuleJson = $$$"""
                              {
                                "and": [
                                  { "in": [ { "var": "environment.name" }, [ "{{{EnvAdmin}}}" ] ] },
                                  { "==": [ { "var": "tenant_constraints.access_allowed" }, true ] },
                                  { "!=": [ { "var": "user.id" }, { "var": "resource.user_id" } ] },
                                  { "==": [ { "var": "resource.user_is_system_account" }, false ] },
                                  { "==": [ { "var": "user.is_system_account" }, true ] },
                                  { "in": [ { "var": "user.role_name" }, [ "{{{RoleSysBot}}}", "{{{RoleSysAdmin}}}", "{{{RoleSysStaff}}}" ] ] }
                                ]
                              }
                              """,
                IsEnabled = true
            },

            // ✅ user:tenant:deactivate — only tenant owner/admin
            new AbacPolicyTemplate
            {
                TemplateName = "ScopedUserTenantDeactivate",
                ResourceType = DefaultResourceType.User.GetEnumMemberValue(),
                Action = DefaultActionType.TenantDeactivate.GetEnumMemberValue(),
                RuleJson = $$$"""
                              {
                                "and": [
                                  { "in": [ { "var": "environment.name" }, [ "{{{EnvClient}}}" ] ] },
                                  { "==": [ { "var": "tenant_constraints.access_allowed" }, true ] },
                                  { "!=": [ { "var": "user.id" }, { "var": "resource.user_id" } ] },
                                  {
                                     "some": [
                                         { "var": "resource.tenant_roles" },
                                         {
                                           "and": [
                                             { "==": [ { "var": "user.tenant_id" }, { "var": "tenant_id" } ] },
                                             { "in": [ { "var": "user.role_name" }, [ "{{{RoleAdmin}}}", "{{{RoleOwner}}}" ] ] }
                                           ]
                                         }
                                     ]
                                  }
                                ]
                              }
                              """,
                IsEnabled = true
            },

            // ✅ user:unsuspend — only system roles
            new AbacPolicyTemplate
            {
                TemplateName = "ScopedUserUnsuspend",
                ResourceType = DefaultResourceType.User.GetEnumMemberValue(),
                Action = DefaultActionType.Unsuspend.GetEnumMemberValue(),
                RuleJson = $$$"""
                              {
                                "and": [
                                  { "in": [ { "var": "environment.name" }, [ "{{{EnvAdmin}}}" ] ] },
                                  { "==": [ { "var": "tenant_constraints.access_allowed" }, true ] },
                                  { "!=": [ { "var": "user.id" }, { "var": "resource.user_id" } ] },
                                  { "==": [ { "var": "resource.user_is_system_account" }, false ] },
                                  { "==": [ { "var": "user.is_system_account" }, true ] },
                                  { "in": [ { "var": "user.role_name" }, [ "{{{RoleSysBot}}}", "{{{RoleSysAdmin}}}", "{{{RoleSysStaff}}}" ] ] }
                                ]
                              }
                              """,
                IsEnabled = true
            },

            // ✅ user:tenant:reactivate — only tenant owner/admin
            new AbacPolicyTemplate
            {
                TemplateName = "ScopedUserTenantReactivate",
                ResourceType = DefaultResourceType.User.GetEnumMemberValue(),
                Action = DefaultActionType.TenantReactivate.GetEnumMemberValue(),
                RuleJson = $$$"""
                              {
                                "and": [
                                  { "in": [ { "var": "environment.name" }, [ "{{{EnvClient}}}" ] ] },
                                  { "==": [ { "var": "tenant_constraints.access_allowed" }, true ] },
                                  { "!=": [ { "var": "user.id" }, { "var": "resource.user_id" } ] },
                                  {
                                     "some": [
                                         { "var": "resource.tenant_roles" },
                                         {
                                           "and": [
                                             { "==": [ { "var": "user.tenant_id" }, { "var": "tenant_id" } ] },
                                             { "in": [ { "var": "user.role_name" }, [ "{{{RoleAdmin}}}", "{{{RoleOwner}}}" ] ] }
                                           ]
                                         }
                                     ]
                                  }
                                ]
                              }
                              """,
                IsEnabled = true
            },

            // ✅ user:audit — only audit roles
            new AbacPolicyTemplate
            {
                TemplateName = "ScopedUserAudit",
                ResourceType = DefaultResourceType.User.GetEnumMemberValue(),
                Action = DefaultActionType.Audit.GetEnumMemberValue(),
                RuleJson = $$$"""
                              {
                                "and": [
                                  { "in": [ { "var": "environment.name" }, [ "{{{EnvAdmin}}}" ] ] },
                                  { "==": [ { "var": "tenant_constraints.access_allowed" }, true ] },
                                  { "==": [ { "var": "user.is_system_account" }, true ] },
                                  { "in": [ { "var": "user.role_name" }, [ "{{{RoleSysBot}}}", "{{{RoleSysAdmin}}}", "{{{RoleSysAudit}}}" ] ] }
                                ]
                              }
                              """,
                IsEnabled = true
            },
        ];
    }

    private static IEnumerable<AbacPolicyTemplate> TenantPolicyTemplates()
    {
        return
        [
            // ✅ tenant:view — same-tenant members
            new AbacPolicyTemplate
            {
                TemplateName = "ScopedTenantView",
                ResourceType = DefaultResourceType.Tenant.GetEnumMemberValue(),
                Action = DefaultActionType.View.GetEnumMemberValue(),
                RuleJson = $$$"""
                              {
                                "and": [
                                  { "in": [ { "var": "environment.name" }, [ "{{{EnvClient}}}", "{{{EnvAdmin}}}" ] ] },
                                  { "==": [ { "var": "tenant_constraints.access_allowed" }, true ] },
                                  {
                                    "or": [
                                      {
                                        "and": [
                                          { "==": [ { "var": "user.tenant_id" }, { "var": "resource.tenant_id" } ] },
                                          { "in": [ { "var": "user.role_name" }, [ "{{{RoleAdmin}}}", "{{{RoleOwner}}}" ] ] }
                                        ]
                                      },
                                      {
                                        "and": [
                                          { "==": [ { "var": "user.is_system_account" }, true ] },
                                          { "in": [ { "var": "user.role_name" }, [ "{{{RoleSysBot}}}", "{{{RoleSysAdmin}}}", "{{{RoleSysStaff}}}" ] ] }
                                        ]
                                      }
                                    ]
                                  }
                                ]
                              }
                              """,
                IsEnabled = true
            },

            // ✅ tenant:edit — only owner/admin of tenant and system roles
            new AbacPolicyTemplate
            {
                TemplateName = "ScopedTenantEdit",
                ResourceType = DefaultResourceType.Tenant.GetEnumMemberValue(),
                Action = DefaultActionType.Edit.GetEnumMemberValue(),
                RuleJson = $$$"""
                              {
                                "and": [
                                  { "in": [ { "var": "environment.name" }, [ "{{{EnvClient}}}", "{{{EnvAdmin}}}" ] ] },
                                  { "==": [ { "var": "tenant_constraints.access_allowed" }, true ] },
                                  {
                                    "or": [
                                      {
                                        "and": [
                                          { "==": [ { "var": "user.tenant_id" }, { "var": "resource.tenant_id" } ] },
                                          { "in": [ { "var": "user.role_name" }, [ "{{{RoleAdmin}}}", "{{{RoleOwner}}}" ] ] }
                                        ]
                                      },
                                      {
                                        "and": [
                                          { "==": [ { "var": "user.is_system_account" }, true ] },
                                          { "in": [ { "var": "user.role_name" }, [ "{{{RoleSysBot}}}", "{{{RoleSysAdmin}}}", "{{{RoleSysStaff}}}" ] ] }
                                        ]
                                      }
                                    ]
                                  }
                                ]
                              }
                              """,
                IsEnabled = true
            },

            // ✅ tenant:delete — only owner of tenant
            new AbacPolicyTemplate
            {
                TemplateName = "ScopedTenantDelete",
                ResourceType = DefaultResourceType.Tenant.GetEnumMemberValue(),
                Action = DefaultActionType.Delete.GetEnumMemberValue(),
                RuleJson = $$$"""
                              {
                                "and": [
                                  { "in": [ { "var": "environment.name" }, [ "{{{EnvClient}}}" ] ] },
                                  { "==": [ { "var": "tenant_constraints.access_allowed" }, true ] },
                                  {
                                     "some": [
                                         { "var": "resource.user_roles" },
                                         {
                                           "and": [
                                             { "==": [ { "var": "user.tenant_id" }, { "var": "resource.tenant_id" } ] },
                                             { "in": [ { "var": "user.role_name" }, [ "{{{RoleOwner}}}" ] ] }
                                           ]
                                         }
                                     ]
                                  }
                                ]
                              }
                              """,
                IsEnabled = true
            },

            // ✅ tenant:suspend — only system roles
            new AbacPolicyTemplate
            {
                TemplateName = "ScopedTenantSuspend",
                ResourceType = DefaultResourceType.Tenant.GetEnumMemberValue(),
                Action = DefaultActionType.Suspend.GetEnumMemberValue(),
                RuleJson = $$$"""
                              {
                                "and": [
                                  { "in": [ { "var": "environment.name" }, [ "{{{EnvAdmin}}}" ] ] },
                                  { "==": [ { "var": "tenant_constraints.access_allowed" }, true ] },
                                  { "!=": [ { "var": "user.id" }, { "var": "resource.user_id" } ] },
                                  { "==": [ { "var": "resource.user_is_system_account" }, false ] },
                                  { "==": [ { "var": "user.is_system_account" }, true ] },
                                  { "in": [ { "var": "user.role_name" }, [ "{{{RoleSysBot}}}", "{{{RoleSysAdmin}}}", "{{{RoleSysStaff}}}" ] ] }
                                ]
                              }
                              """,
                IsEnabled = true
            },

            // ✅ tenant:unsuspend — only system roles
            new AbacPolicyTemplate
            {
                TemplateName = "ScopedTenantUnsuspend",
                ResourceType = DefaultResourceType.Tenant.GetEnumMemberValue(),
                Action = DefaultActionType.Unsuspend.GetEnumMemberValue(),
                RuleJson = $$$"""
                              {
                                "and": [
                                  { "in": [ { "var": "environment.name" }, [ "{{{EnvAdmin}}}" ] ] },
                                  { "==": [ { "var": "tenant_constraints.access_allowed" }, true ] },
                                  { "!=": [ { "var": "user.id" }, { "var": "resource.user_id" } ] },
                                  { "==": [ { "var": "resource.user_is_system_account" }, false ] },
                                  { "==": [ { "var": "user.is_system_account" }, true ] },
                                  { "in": [ { "var": "user.role_name" }, [ "{{{RoleSysBot}}}", "{{{RoleSysAdmin}}}", "{{{RoleSysStaff}}}" ] ] }
                                ]
                              }
                              """,
                IsEnabled = true
            },

            // ✅ tenant:audit — only audit roles
            new AbacPolicyTemplate
            {
                TemplateName = "ScopedTenantAudit",
                ResourceType = DefaultResourceType.Tenant.GetEnumMemberValue(),
                Action = DefaultActionType.Audit.GetEnumMemberValue(),
                RuleJson = $$$"""
                              {
                                "and": [
                                  { "in": [ { "var": "environment.name" }, [ "{{{EnvAdmin}}}" ] ] },
                                  { "==": [ { "var": "tenant_constraints.access_allowed" }, true ] },
                                  { "==": [ { "var": "user.is_system_account" }, true ] },
                                  { "in": [ { "var": "user.role_name" }, [ "{{{RoleSysBot}}}", "{{{RoleSysAdmin}}}", "{{{RoleSysAudit}}}" ] ] }
                                ]
                              }
                              """,
                IsEnabled = true
            },
        ];
    }

    private static IEnumerable<AbacPolicyTemplate> AbacPolicyTemplates()
    {
        return
        [
            // ✅ abac-policy:view — only admin/owner of tenant and system roles
            new AbacPolicyTemplate
            {
                TemplateName = "ScopedAbacPolicyView",
                ResourceType = DefaultResourceType.AbacPolicy.GetEnumMemberValue(),
                Action = DefaultActionType.View.GetEnumMemberValue(),
                RuleJson = $$$"""
                              {
                                "and": [
                                  { "in": [ { "var": "environment.name" }, [ "{{{EnvClient}}}", "{{{EnvAdmin}}}" ] ] },
                                  { "==": [ { "var": "tenant_constraints.access_allowed" }, true ] },
                                  {
                                    "or": [
                                      {
                                        "and": [
                                          { "==": [ { "var": "user.tenant_id" }, { "var": "resource.tenant_id" } ] },
                                          { "in": [ { "var": "user.role_name" }, [ "{{{RoleAdmin}}}", "{{{RoleOwner}}}" ] ] }
                                        ]
                                      },
                                      {
                                        "and": [
                                          { "==": [ { "var": "user.is_system_account" }, true ] },
                                          { "in": [ { "var": "user.role_name" }, [ "{{{RoleSysBot}}}", "{{{RoleSysAdmin}}}", "{{{RoleSysStaff}}}" ] ] }
                                        ]
                                      }
                                    ]
                                  }
                                ]
                              }
                              """,
                IsEnabled = true
            },

            // ✅ abac-policy:edit — only admin/owner of tenant
            new AbacPolicyTemplate
            {
                TemplateName = "ScopedAbacPolicyEdit",
                ResourceType = DefaultResourceType.AbacPolicy.GetEnumMemberValue(),
                Action = DefaultActionType.Edit.GetEnumMemberValue(),
                RuleJson = $$$"""
                              {
                                "and": [
                                  { "in": [ { "var": "environment.name" }, [ "{{{EnvClient}}}", "{{{EnvAdmin}}}" ] ] },
                                  { "==": [ { "var": "tenant_constraints.access_allowed" }, true ] },
                                  { "==": [ { "var": "user.tenant_id" }, { "var": "resource.tenant_id" } ] },
                                  { "in": [ { "var": "user.role_name" }, [ "{{{RoleAdmin}}}", "{{{RoleOwner}}}" ] ] }
                                ]
                              }
                              """,
                IsEnabled = true
            },
        ];
    }
}

//    { "==": [ { "var": "user.id" }, { "var": "resource.id" } ] } means: can only act on self, don't need to use { "==": [ { "var": "user.tenant_id" }, { "var": "resource.tenant_id" } ] }, in this case