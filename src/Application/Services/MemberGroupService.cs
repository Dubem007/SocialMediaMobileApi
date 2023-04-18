using Application.Contracts;
using Infrastructure.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class MemberGroupService : IMemberGroupService
    {
        private readonly IRepositoryManager _repository;
        public MemberGroupService(IRepositoryManager repository)
        {
            _repository = repository;
        }
    }
}
