namespace Users.Domain.Entities
{
    public class Permission
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        //public string? Group { get; set; } // e.g. "HR"
        
        public bool IsDeleted { get; set; } = false; // ✅

        public ICollection<RolePermission> RolePermissions { get; set; }
    }

}
