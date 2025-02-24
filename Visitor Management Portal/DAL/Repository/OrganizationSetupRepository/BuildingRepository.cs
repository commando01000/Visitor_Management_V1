using CrmEarlyBound;
using D365_Add_ons.Repository;
using D365_Add_ons.Repository;

namespace Visitor_Management_Portal.DAL.Repository.BuildingRepository
{
    public class BuildingRepository : BaseRepository<vm_Building>, IBuildingRepository
    {
        public BuildingRepository() : base() { }

    }
}
