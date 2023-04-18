using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class MemberGroup : AuditableEntity
    {
        public Guid GroupId { get; set; }
        public string Group { get; init; }
        public string Decription { get; init; }
        public IEnumerable<UserMember> Member { get; init; }

    }
}
