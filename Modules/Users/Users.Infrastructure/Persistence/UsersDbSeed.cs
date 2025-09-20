
using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;
using Users.Infrastructure.Common.Security;
using BC = BCrypt.Net.BCrypt;

namespace Users.Infrastructure.Persistence
{
    public static class UsersDbSeed
    {
        public static async Task SeedAsync(UsersDbContext context)
        {
            async Task<StoreBranch> EnsureBranchAsync(string name, string location)
            {
                var branch = await context.StoreBranches.FirstOrDefaultAsync(b => b.Name == name);
                if (branch == null)
                {
                    branch = new StoreBranch
                    {
                        Id = Guid.NewGuid(),
                        Name = name,
                        Location = location
                    };
                    await context.StoreBranches.AddAsync(branch);
                    await context.SaveChangesAsync();
                }
                return branch;
            }

            async Task EnsurePermissionAsync(string name, string code, string description)
            {
                if (!await context.Permissions.AnyAsync(p => p.Code == code))
                {
                    await context.Permissions.AddAsync(new Permission
                    {
                        Id = Guid.NewGuid(),
                        Name = name,
                        Code = code,
                        Description = description
                    });
                }
            }

            async Task<Role> EnsureRoleAsync(string name, string code, string description, List<string> permissionCodes)
            {
                var role = await context.Roles.Include(r => r.RolePermissions).FirstOrDefaultAsync(r => r.Code == code);
                if (role == null)
                {
                    var roleId = Guid.NewGuid();
                    var permissions = await context.Permissions.Where(p => permissionCodes.Contains(p.Code)).ToListAsync();

                    role = new Role
                    {
                        Id = roleId,
                        Name = name,
                        Code = code,
                        Description = description,
                        RolePermissions = permissions.Select(p => new RolePermission
                        {
                            Id = Guid.NewGuid(),
                            RoleId = roleId,
                            PermissionId = p.Id
                        }).ToList()
                    };
                    await context.Roles.AddAsync(role);
                    await context.SaveChangesAsync();
                }
                return role;
            }

            async Task<List<SecurityQuestion>> EnsureSecurityQuestionsAsync()
            {
                var questions = new List<string>
                {
                    "What is your favorite animal?",
                    "What was the name of your first school?",
                    "What is your favorite food?"
                };

                var result = new List<SecurityQuestion>();
                foreach (var text in questions)
                {
                    var q = await context.SecurityQuestions.FirstOrDefaultAsync(q => q.QuestionText == text);
                    if (q == null)
                    {
                        q = new SecurityQuestion
                        {
                            Id = Guid.NewGuid(),
                            QuestionText = text
                        };
                        await context.SecurityQuestions.AddAsync(q);
                        await context.SaveChangesAsync();
                    }
                    result.Add(q);
                }
                return result;
            }

            async Task EnsureUserWithQuestionsAsync(string username, string email, string password, string fullName, string gender, Role role, StoreBranch branch, List<SecurityQuestion> questions)
            {
                if (!await context.Users.AnyAsync(u => u.Username == username))
                {
                    var userId = Guid.NewGuid();

                    // جلب صلاحيات الدور
                    var permissionIds = await context.RolePermissions
                        .Where(rp => rp.RoleId == role.Id)
                        .Select(rp => rp.PermissionId)
                        .ToListAsync();

                    var user = new User
                    {
                        Id = userId,
                        Username = username,
                        Email = email,
                        PasswordHash = BC.HashPassword(password),
                        IsActive = true,
                        IsConfirmed = true,
                        HasAnsweredSecurityQuestions = true,
                        CreatedAt = DateTime.UtcNow,
                        StoreBranchId = branch.Id,
                        Profile = new UserProfile
                        {
                            Id = Guid.NewGuid(),
                            UserId = userId,
                            FullName = fullName,
                            Gender = gender,
                            ProfilePicture = new byte[0]
                        },
                        UserRoles = new List<UserRole>
                        {
                            new UserRole
                            {
                                Id = Guid.NewGuid(),
                                UserId = userId,
                                RoleId = role.Id
                            }
                        },
                        UserPermissions = permissionIds.Select(pid => new UserPermission
                        {
                            Id = Guid.NewGuid(),
                            UserId = userId,
                            PermissionId = pid
                        }).ToList()
                    };

                    await context.Users.AddAsync(user);
                    await context.SaveChangesAsync();

                    // أضف الأسئلة والإجابات
                    var answers = new[] { "lion", "alma", "pizza" };
                    for (int i = 0; i < questions.Count && i < answers.Length; i++)
                    {
                        await context.UserSecurityAnswers.AddAsync(new UserSecurityAnswer
                        {
                            Id = Guid.NewGuid(),
                            UserId = userId,
                            QuestionId = questions[i].Id,
                            AnswerHash = BC.HashPassword(answers[i]),
                            AnswerEncrypted = EncryptionHelper.Encrypt(answers[i])
                        });
                    }
                    await context.SaveChangesAsync();
                }
            }

            // ✅ Permissions
            var permissions = new (string Name, string Code, string Description)[]
            {
                ("View Users", "User.View", "Can view users"),
                ("Create Users", "User.Create", "Can create users"),
                ("Edit Users", "User.Edit", "Can edit users"),
                ("Freeze Users", "User.Freeze", "Can freeze users"),
                ("Manage Employees", "HR.Employee.Manage", "Manage employee records"),
                ("View Payroll", "HR.Payroll.View", "Can view payroll"),
                ("Access Accounting", "Finance.Access", "Access to accounting module"),
                ("View Reports", "Reports.View", "Can view financial reports"),
                ("Manage Sales", "Sales.Manage", "Manage sales"),
                ("View Sales", "Sales.View", "Can view sales"),
                ("Manage Inventory", "Warehouse.Manage", "Manage warehouse inventory"),
                ("Access System Logs", "IT.Logs.View", "View system logs")
            };

            foreach (var (name, code, description) in permissions)
                await EnsurePermissionAsync(name, code, description);

            await context.SaveChangesAsync();

            // ✅ Branches
            var main = await EnsureBranchAsync("Main", "Cairo");
            var hr = await EnsureBranchAsync("HR", "Giza");
            var finance = await EnsureBranchAsync("Finance", "Alex");
            var sales = await EnsureBranchAsync("Sales", "6th October");
            var store = await EnsureBranchAsync("Warehouse", "Tenth of Ramadan");
            var it = await EnsureBranchAsync("IT", "Smart Village");

            // ✅ Roles
            var superAdmin = await EnsureRoleAsync("SuperAdmin", "ROLE_SUPERADMIN", "Full system access", permissions.Select(p => p.Code).ToList());
            var hrManager = await EnsureRoleAsync("HR Manager", "ROLE_HR", "HR Access", new() { "HR.Employee.Manage", "HR.Payroll.View" });
            var accountant = await EnsureRoleAsync("Accountant", "ROLE_FINANCE", "Finance Access", new() { "Finance.Access", "Reports.View" });
            var salesManager = await EnsureRoleAsync("Sales Manager", "ROLE_SALES", "Sales Access", new() { "Sales.Manage", "Sales.View" });
            var warehouseManager = await EnsureRoleAsync("Warehouse Manager", "ROLE_STORE", "Store Access", new() { "Warehouse.Manage" });
            var itSupport = await EnsureRoleAsync("IT Support", "ROLE_IT", "IT Access", new() { "IT.Logs.View" });

            // ✅ Questions
            var questions = await EnsureSecurityQuestionsAsync();

            // ✅ Users
            await EnsureUserWithQuestionsAsync("superadmin", "admin@erp.local", "Admin@1234", "Admin", "Male", superAdmin, main, questions);
            await EnsureUserWithQuestionsAsync("hruser", "hr@erp.local", "Hr@123", "HR Manager", "Female", hrManager, hr, questions);
            await EnsureUserWithQuestionsAsync("finuser", "fin@erp.local", "Fin@123", "Finance", "Male", accountant, finance, questions);
            await EnsureUserWithQuestionsAsync("salesuser", "sales@erp.local", "Sales@123", "Sales", "Male", salesManager, sales, questions);
            await EnsureUserWithQuestionsAsync("storeuser", "store@erp.local", "Store@123", "Warehouse", "Male", warehouseManager, store, questions);
            await EnsureUserWithQuestionsAsync("ituser", "it@erp.local", "It@123", "IT Support", "Male", itSupport, it, questions);
        }
    }
}
