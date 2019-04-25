using AutoMapper;
using CapiControls.BLL.DTO;
using CapiControls.BLL.Interfaces;
using CapiControls.DAL.Entities;
using CapiControls.DAL.Interfaces.Units;
using System;
using System.Collections.Generic;

namespace CapiControls.BLL.Services
{
    public class GroupService : BaseService, IGroupService
    {
        private readonly ILocalUnitOfWork _uow;

        public GroupService(ILocalUnitOfWork uow, IMapper mapper) : base(mapper)
        {
            _uow = uow;
        }

        public void AddGroup(GroupDTO group)
        {
            group.Id = Guid.NewGuid();
            _uow.GroupRepository.Add(Mapper.Map<GroupDTO, Group>(group));
            _uow.Commit();
        }

        public int CountGroups()
        {
            return _uow.GroupRepository.CountAll();
        }

        public void DeleteGroup(Guid id)
        {
            _uow.GroupRepository.Delete(id);
            _uow.Commit();
        }

        public GroupDTO GetGroup(Guid id)
        {
            return Mapper.Map<Group, GroupDTO>(_uow.GroupRepository.Find(id));
        }

        public IEnumerable<GroupDTO> GetGroups()
        {
            return Mapper.Map<IEnumerable<Group>, IEnumerable<GroupDTO>>(_uow.GroupRepository.GetAll());
        }

        public IEnumerable<GroupDTO> GetGroups(int page, int pageSize)
        {
            return Mapper.Map<IEnumerable<Group>, IEnumerable<GroupDTO>>(_uow.GroupRepository.GetAll(page, pageSize));
        }

        public void UpdateGroup(GroupDTO group)
        {
            _uow.GroupRepository.Update(Mapper.Map<GroupDTO, Group>(group));
            _uow.Commit();
        }
    }
}
