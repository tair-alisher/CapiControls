using CapiControls.BLL.DTO;
using System;
using System.Collections.Generic;

namespace CapiControls.BLL.Interfaces
{
    public interface IGroupService : IBaseService
    {
        int CountGroups();
        IEnumerable<GroupDTO> GetGroups();
        IEnumerable<GroupDTO> GetGroups(int page, int pageSize);
        void AddGroup(GroupDTO group);
        GroupDTO GetGroup(Guid id);
        void UpdateGroup(GroupDTO group);
        void DeleteGroup(Guid id);
    }
}
