using CapiControls.BLL.Interfaces;
using CapiControls.DAL.Entities;
using System.Collections.Generic;

namespace CapiControls.BLL.Tests
{
    public class GroupService_Tests
    {
        IGroupService GroupService;
        int commits = 0;
        List<Group> GroupsData;

        public GroupService_Tests()
        {
            GroupsData = new List<Group>()
            {

            };
        }

    }
}
