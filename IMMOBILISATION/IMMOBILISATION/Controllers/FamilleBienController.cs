using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IMMOBILISATION.Models;
using System.Globalization;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace IMMOBILISATION.Controllers
{
    [Authorize]
    public class FamilleBienController : Controller
    {
        //
        // GET: /FamilleBien/
        ImmobilisationEntities BD = new ImmobilisationEntities();
        public ActionResult Index()
        {
            List<FAMILLES_IMMOBILISATIONS> Liste = BD.FAMILLES_IMMOBILISATIONS.ToList();
            return View(Liste);
        }
        public ActionResult Form(string Mode, int Code)
        {
            FAMILLES_IMMOBILISATIONS Element = new FAMILLES_IMMOBILISATIONS();
            if (Mode == "Create")
            {
                ViewBag.TITRE = "NOUVELLE FAMILLE DE BIEN";
            }
            if (Mode == "Edit")
            {
                Element = BD.FAMILLES_IMMOBILISATIONS.Find(Code);
                ViewBag.TITRE = "MODIFIER FAMILLE DE BIEN";
            }
            ViewBag.Mode = Mode;
            ViewBag.Code = Code;
            return View(Element);
        }
        public JsonResult GetAllNature()
        {
            BD.Configuration.ProxyCreationEnabled = false;
            List<NATURES_BIENS> Liste = BD.NATURES_BIENS.ToList();
            return Json(Liste, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SendForm(string Mode, string Code)
        {
            string FAMILLE = Request.Params["FAMILLE"] != null ? Request.Params["FAMILLE"].ToString() : string.Empty;
            string TYPE = Request.Params["TYPE"] != null ? Request.Params["TYPE"].ToString() : string.Empty;
            string INTITULE = Request.Params["INTITULE"] != null ? Request.Params["INTITULE"].ToString() : string.Empty;
            string NATURE = Request.Params["NATURE"] != null ? Request.Params["NATURE"].ToString() : "0";
            int ID_NATURE = int.Parse(NATURE);
            NATURES_BIENS bien = BD.NATURES_BIENS.Find(ID_NATURE);
            if (Mode == "Create")
            {
                FAMILLES_IMMOBILISATIONS Element = new FAMILLES_IMMOBILISATIONS();
                Element.FAMILLE = FAMILLE;
                Element.TYPE = TYPE;
                Element.INTITULE = INTITULE;
                Element.NATURE = ID_NATURE;
                Element.NATURES_BIENS = bien;
                BD.FAMILLES_IMMOBILISATIONS.Add(Element);
                BD.SaveChanges();
            }
            if (Mode == "Edit")
            {
                int ID = int.Parse(Code);
                FAMILLES_IMMOBILISATIONS Element = BD.FAMILLES_IMMOBILISATIONS.Find(ID);
                Element.FAMILLE = FAMILLE;
                Element.TYPE = TYPE;
                Element.INTITULE = INTITULE;
                Element.NATURE = ID_NATURE;
                Element.NATURES_BIENS = bien;
                BD.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int Code)
        {
            FAMILLES_IMMOBILISATIONS Selected = BD.FAMILLES_IMMOBILISATIONS.Find(Code);
            BD.FAMILLES_IMMOBILISATIONS.Remove(Selected);
            BD.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Print()
        {
            List<FAMILLES_IMMOBILISATIONS> Liste = BD.FAMILLES_IMMOBILISATIONS.ToList();
            dynamic dt = from Element in Liste
                         select new
                         {
                             FAMILLE = Element.FAMILLE,
                             TYPE = Element.TYPE,
                             NATURE=Element.NATURES_BIENS != null ? Element.NATURES_BIENS.NATURE:string.Empty
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/FAMILLE_BIENS.rpt");
            rptH.Load(FileName);
            rptH.SetDataSource(dt);
            rptH.SummaryInfo.ReportTitle = "Familles des immobilisations";
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

    }
}
