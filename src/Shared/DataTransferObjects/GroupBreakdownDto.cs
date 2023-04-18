using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
  
    public class GroupBreakdownDto
    {
       
        public string Group { get; init; }
        public IEnumerable<UserMember> Members { get; init; }
    }

    public class MemberGroupDto
    {
        public Guid Id { get; set; }
        public string Group { get; init; }
        public string Description { get; init; }
        public IEnumerable<UserMember> Members { get; init; }
        public string Counts { get; init; }
    }

    public class GroupUserMembersDto
    {
        public Guid Id { get; set; }
        public string Group { get; init; }
        public string Description { get; init; }
        public string Counts { get; init; }
    }

    public class ListOfProfessionsDto
    {
        public Guid Id { get; set; }
        public string Profession { get; set; }
    }
}
