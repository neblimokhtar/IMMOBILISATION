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
    public class LieuxController : Controller
    {
        //
        // GET: /Lieux/
        ImmobilisationEntities BD = new ImmobilisationEntities();
        public ActionResult Index()
        {
            List<LIEUX> Liste = BD.LIEUX.ToList();
            return View(Liste);
        }
        public ActionResult Form(string Mode, int Code)
        {
            LIEUX Element = new LIEUX();
            if (Mode == "Create")
            {
                ViewBag.TITRE = "NOUVEAU LIEU";
            }
            if (Mode == "Edit")
            {
                Element = BD.LIEUX.Find(Code);
                ViewBag.TITRE = "MODIFIER UN LIEU";
            }
            ViewBag.Mode = Mode;
            ViewBag.Code = Code;
            return View(Element);
        }
        [HttpPost]
        public ActionResult SendForm(string Mode, string Code)
        {
            string INTITULE = Request.Params["INTITULE"] != null ? Request.Params["INTITULE"].ToString() : string.Empty;
            string ADRESSE = Request.Params["ADRESSE"] != null ? Request.Params["ADRESSE"].ToString() : string.Empty;
            string CODE_POSTAL = Request.Params["CODE_POSTAL"] != null ? Request.Params["CODE_POSTAL"].ToString() : string.Empty;
            string VILLE = Request.Params["VILLE"] != null ? Request.Params["VILLE"].ToString() : string.Empty;
            string REGION = Request.Params["REGION"] != null ? Request.Params["REGION"].ToString() : string.Empty;
            string PAYS = Request.Params["PAYS"] != null ? Request.Params["PAYS"].ToString() : string.Empty;
            string TELEPHONE = Request.Params["TELEPHONE"] != null ? Request.Params["TELEPHONE"].ToString() : string.Empty;
            string LATITUDE = Request.Params["LATITUDE"] != null ? Request.Params["LATITUDE"].ToString() : "0";
            string LONGITUDE = Request.Params["LONGITUDE"] != null ? Request.Params["LONGITUDE"].ToString() : "0";
            if (Mode == "Create")
            {
                LIEUX Element = new LIEUX();
                Element.INTITULE = INTITULE;
                Element.ADRESSE = ADRESSE;
                Element.CODE_POSTAL = CODE_POSTAL;
                Element.VILLE = VILLE;
                Element.REGION = REGION;
                Element.PAYS = PAYS;
                Element.TELEPHONE = TELEPHONE;
                Element.LATITUDE = 0;
                Element.LONGITUDE = 0;
                float value;
                if (float.TryParse(LATITUDE, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                {
                    Element.LATITUDE = float.Parse(LATITUDE, CultureInfo.InvariantCulture);
                }
                if (float.TryParse(LONGITUDE, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                {
                    Element.LONGITUDE = float.Parse(LONGITUDE, CultureInfo.InvariantCulture);
                }
                BD.LIEUX.Add(Element);
                BD.SaveChanges();
            }
            if (Mode == "Edit")
            {
                int ID = int.Parse(Code);
                LIEUX Element = BD.LIEUX.Find(ID);
                Element.INTITULE = INTITULE;
                Element.ADRESSE = ADRESSE;
                Element.CODE_POSTAL = CODE_POSTAL;
                Element.VILLE = VILLE;
                Element.REGION = REGION;
                Element.PAYS = PAYS;
                Element.TELEPHONE = TELEPHONE;
                Element.LATITUDE = 0;
                Element.LONGITUDE = 0;
                float value;
                if (float.TryParse(LATITUDE, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                {
                    Element.LATITUDE = float.Parse(LATITUDE, CultureInfo.InvariantCulture);
                }
                if (float.TryParse(LONGITUDE, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                {
                    Element.LONGITUDE = float.Parse(LONGITUDE, CultureInfo.InvariantCulture);
                }
                BD.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult Coordonee()
        {
            return View();
        }
        public ActionResult ViewOnMap(string Code)
        {
            int ID = int.Parse(Code);
            LIEUX Element = BD.LIEUX.Find(ID);
            return View(Element);
        }
        public ActionResult Delete(int Code)
        {
            LIEUX Selected = BD.LIEUX.Find(Code);
            BD.LIEUX.Remove(Selected);
            BD.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Distance()
        {
            List<LIEUX> Liste = BD.LIEUX.ToList();
            return View(Liste);
        }
        public ActionResult Print()
        {
            List<LIEUX> Liste = BD.LIEUX.ToList();
            dynamic dt = from Element in Liste
                         select new
                         {
                             INTITULE = Element.INTITULE,
                             ADRESSE = Element.ADRESSE,
                             VILLE = Element.VILLE,
                             REGION=Element.REGION,
                             PAYS=Element.PAYS,
                             TELEPHONE=Element.TELEPHONE,
                             LATITUDE=Element.LATITUDE,
                             LONGITUDE=Element.LONGITUDE,
                             CODE_POSTAL=Element.CODE_POSTAL
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/LIEUX.rpt");
            rptH.Load(FileName);
            rptH.SetDataSource(dt);
            rptH.SummaryInfo.ReportTitle = "Lieux";
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

    }
}
