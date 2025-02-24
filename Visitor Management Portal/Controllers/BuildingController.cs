using CrmEarlyBound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Visitor_Management_Portal.DAL.Repository.BuildingRepository;

namespace Visitor_Management_Portal.Controllers
{
    public class BuildingController : Controller
    {
        private readonly IBuildingRepository _buildingRepository;

        public BuildingController(IBuildingRepository buildingRepository)
        {
            _buildingRepository = buildingRepository;
        }

        public ActionResult Index()
        {
            var buildings = _buildingRepository.GetAll(); 
            return Json(buildings, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(Guid id)
        {
            var building = _buildingRepository.Get(id);
            return Json(building, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            var createObj = new vm_Building
            {
                Id = Guid.NewGuid(),
                vm_BuildingName = "Building ABC",
            };

            var resultId  = _buildingRepository.Create(createObj);

            var building = _buildingRepository.Get(resultId);

            return Json(building, JsonRequestBehavior.AllowGet);
        }

    }
}