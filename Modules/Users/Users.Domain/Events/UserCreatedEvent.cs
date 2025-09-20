using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Domain.Events;
public record UserCreatedEvent(Guid UserId, string Username, string Email);

