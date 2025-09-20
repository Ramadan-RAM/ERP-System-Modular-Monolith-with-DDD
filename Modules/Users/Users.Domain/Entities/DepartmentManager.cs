using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Domain.Entities
{
    public class DepartmentManager
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public Guid? ManagerUserId { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
