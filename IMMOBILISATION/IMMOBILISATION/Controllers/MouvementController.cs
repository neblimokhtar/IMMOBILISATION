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
    public class MouvementController : Controller
    {
        //
        // GET: /Mouvement/
        ImmobilisationEntities BD = new ImmobilisationEntities();
        public ActionResult Index()
        {
            List<MOUVEMENTS> Liste = BD.MOUVEMENTS.ToList();
            dynamic Result = (from Element in Liste
                              select new
                              {
                                  ID = Element.ID,
                                  TYPE = Element.TYPE,
                                  DATEMOUVEMENT = Element.DATE_MOUVEMENT.ToShortDateString(),
                                  DU = Element.DEPARTS.INTITULE,
                                  AU = Element.RETOURS.INTITULE,
                                  DISTANCE = Element.DISTANCE,
                                  CLIENT = Element.CLIENTS.INTITULE,
                                  TRANSPORTEUR = Element.TRANSPORTEURS.INTITULE,
                                  IMMOBILISATIONS = GetImmobilisation(Element.ID)
                              }).AsEnumerable().Select(c => c.ToExpando());
            return View(Result);
        }
        private string GetImmobilisation(int ID)
        {
            string Result = string.Empty;
            List<DETAILS_MOUVEMENTS> liste = BD.DETAILS_MOUVEMENTS.Where(Element => Element.MOUVEMENTS.ID == ID).ToList();
            foreach (DETAILS_MOUVEMENTS Detail in liste)
            {
                IMMOBILISATIONS immobilisation = BD.IMMOBILISATIONS.Find(Detail.IMMOBILISATIONS.ID);
                Result += " " + immobilisation.CODE + " ;";
            }
            return Result;
        }
        private string GetDetailImmobilisation(int ID)
        {
            string Result = string.Empty;
            List<DETAILS_MOUVEMENTS> liste = BD.DETAILS_MOUVEMENTS.Where(Element => Element.MOUVEMENTS.ID == ID).ToList();
            foreach (DETAILS_MOUVEMENTS Detail in liste)
            {
                IMMOBILISATIONS immobilisation = BD.IMMOBILISATIONS.Find(Detail.IMMOBILISATIONS.ID);
                Result += immobilisation.ID + ",";
            }
            return Result;
        }
        public ActionResult Form(string Mode, int Code)
        {
            MOUVEMENTS Element = new MOUVEMENTS();
            if (Mode == "Create")
            {
                ViewBag.TITRE = "NOUVEAU MOUVEMENT";
                ViewBag.Immo = string.Empty;
            }
            if (Mode == "Edit")
            {
                Element = BD.MOUVEMENTS.Find(Code);
                ViewBag.TITRE = "MODIFIER UN MOUVEMENT";
                ViewBag.Immo = GetDetailImmobilisation(Code);
                List<IMMOBILISATIONS> Liste = BD.DETAILS_MOUVEMENTS.Where(Elt => Elt.MOUVEMENTS.ID == Code).Select(Elt => Elt.IMMOBILISATIONS).ToList();
                foreach (IMMOBILISATIONS immobilisation in BD.IMMOBILISATIONS.Where(Elt => Elt.DISPONIBILITE).ToList())
                {
                    Liste.Add(immobilisation);
                }
                //foreach (IMMOBILISATIONS Elt in Liste)
                //{
                //    Elt.DISPONIBILITE = true;
                //    BD.SaveChanges();
                //}
                Liste = Liste.Distinct().ToList();
                ViewBag.Liste = Liste;
                if (Element.TYPE == "RETOUR")
                {
                    return RedirectToAction("Retour", "Mouvement", new { @Code = @Code });
                }
            }
            ViewBag.Mode = Mode;
            ViewBag.Code = Code;
            return View(Element);
        }
        [HttpPost]
        public ActionResult SendForm(string Mode, string Code)
        {
            string TYPE = Request.Params["TYPE"] != null ? Request.Params["TYPE"].ToString() : string.Empty;
            string DATE_MOUVEMENT = Request.Params["DATE_MOUVEMENT"] != null ? Request.Params["DATE_MOUVEMENT"].ToString() : string.Empty;
            string DU = Request.Params["DU"] != null ? Request.Params["DU"].ToString() : string.Empty;
            string AU = Request.Params["AU"] != null ? Request.Params["AU"].ToString() : string.Empty;
            string CLIENT = Request.Params["CLIENT"] != null ? Request.Params["CLIENT"].ToString() : string.Empty;
            string TRANSPORTEUR = Request.Params["TRANSPORTEUR"] != null ? Request.Params["TRANSPORTEUR"].ToString() : string.Empty;
            string DISTANCE = Request.Params["DISTANCE"] != null ? Request.Params["DISTANCE"].ToString() : "0";
            string IMMOBILISATION = Request.Params["IMMOBILISATION"] != null ? Request.Params["IMMOBILISATION"].ToString() : string.Empty;
            string Last = Request.Params["Last"] != null ? Request.Params["Last"].ToString() : string.Empty;

            DISTANCE = DISTANCE.Replace(",", ".");
            int ID_DU = int.Parse(DU);
            int ID_AU = int.Parse(AU);
            int ID_CLIENT = int.Parse(CLIENT);
            int ID_TRANSPORTEUR = int.Parse(TRANSPORTEUR);
            LIEUX LIEUX_DU = BD.LIEUX.Find(ID_DU);
            LIEUX LIEUX_AU = BD.LIEUX.Find(ID_AU);
            TIERS TIERS_CLIENT = BD.TIERS.Find(ID_CLIENT);
            TIERS TIERS_TRANSPORTEUR = BD.TIERS.Find(ID_TRANSPORTEUR);
            if (Mode == "Create")
            {
                MOUVEMENTS Element = new MOUVEMENTS();
                Element.TYPE = TYPE;
                Element.DATE_MOUVEMENT = DateTime.Parse(DATE_MOUVEMENT);
                Element.DU = ID_DU;
                Element.DEPARTS = LIEUX_DU;
                Element.AU = ID_AU;
                Element.RETOURS = LIEUX_AU;
                Element.CLIENT = ID_CLIENT;
                Element.CLIENTS = TIERS_CLIENT;
                Element.TRANSPORTEUR = ID_TRANSPORTEUR;
                Element.TRANSPORTEURS = TIERS_TRANSPORTEUR;
                Element.DISTANCE = 0;
                float value;
                if (float.TryParse(DISTANCE, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                {
                    Element.DISTANCE = float.Parse(DISTANCE, CultureInfo.InvariantCulture);
                }
                BD.MOUVEMENTS.Add(Element);
                BD.SaveChanges();
                string[] IDS = IMMOBILISATION.Split(',');
                foreach (string ID in IDS)
                {
                    if (!string.IsNullOrEmpty(ID))
                    {
                        int FILTER = int.Parse(ID);
                        IMMOBILISATIONS immobilisation = BD.IMMOBILISATIONS.Find(FILTER);
                        DETAILS_MOUVEMENTS detail = new DETAILS_MOUVEMENTS();
                        detail.IMMOBILISATION = FILTER;
                        detail.IMMOBILISATIONS = immobilisation;
                        detail.MOUVEMENT = Element.ID;
                        detail.MOUVEMENTS = Element;
                        BD.DETAILS_MOUVEMENTS.Add(detail);
                        BD.SaveChanges();
                        if (Element.TYPE == "SORTIE")
                            immobilisation.DISPONIBILITE = false;
                        if (Element.TYPE == "RETOUR")
                            immobilisation.DISPONIBILITE = true;
                        BD.SaveChanges();
                    }
                }

            }
            if (Mode == "Edit")
            {
                int ID = int.Parse(Code);
                MOUVEMENTS Element = BD.MOUVEMENTS.Find(ID);
                Element.TYPE = TYPE;
                Element.DATE_MOUVEMENT = DateTime.Parse(DATE_MOUVEMENT);
                Element.DU = ID_DU;
                Element.DEPARTS = LIEUX_DU;
                Element.AU = ID_AU;
                Element.RETOURS = LIEUX_AU;
                Element.CLIENT = ID_CLIENT;
                Element.CLIENTS = TIERS_CLIENT;
                Element.TRANSPORTEUR = ID_TRANSPORTEUR;
                Element.TRANSPORTEURS = TIERS_TRANSPORTEUR;
                Element.DISTANCE = 0;
                float value;
                if (float.TryParse(DISTANCE, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                {
                    Element.DISTANCE = float.Parse(DISTANCE, CultureInfo.InvariantCulture);
                }
                BD.SaveChanges();
                BD.DETAILS_MOUVEMENTS.Where(p => p.MOUVEMENTS.ID == ID).ToList().ForEach(p => BD.DETAILS_MOUVEMENTS.Remove(p));

                string[] IDS = IMMOBILISATION.Split(',');
                string[] IDSP = Last.Split(',');
                foreach (string Id in IDSP.Except(IDS))
                {
                    if (!string.IsNullOrEmpty(Id))
                    {
                        int Idd = int.Parse(Id);
                        IMMOBILISATIONS bien = BD.IMMOBILISATIONS.Find(Idd);
                        bien.DISPONIBILITE = true;
                        BD.SaveChanges();
                    }
                }
                foreach (string id in IDS)
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        int FILTER = int.Parse(id);
                        IMMOBILISATIONS immobilisation = BD.IMMOBILISATIONS.Find(FILTER);
                        DETAILS_MOUVEMENTS detail = new DETAILS_MOUVEMENTS();
                        detail.IMMOBILISATION = FILTER;
                        detail.IMMOBILISATIONS = immobilisation;
                        detail.MOUVEMENT = Element.ID;
                        detail.MOUVEMENTS = Element;
                        BD.DETAILS_MOUVEMENTS.Add(detail);
                        BD.SaveChanges();
                        if (Element.TYPE == "SORTIE")
                            immobilisation.DISPONIBILITE = false;
                        if (Element.TYPE == "RETOUR")
                            immobilisation.DISPONIBILITE = true; BD.SaveChanges();
                    }
                }
            }
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int Code)
        {
            MOUVEMENTS Selected = BD.MOUVEMENTS.Find(Code);
            List<IMMOBILISATIONS> Liste = BD.DETAILS_MOUVEMENTS.Where(Elt => Elt.MOUVEMENTS.ID == Code).Select(Elt => Elt.IMMOBILISATIONS).ToList();
            foreach (IMMOBILISATIONS Elt in Liste)
            {
                Elt.DISPONIBILITE = true;
                BD.SaveChanges();
            }
            BD.DETAILS_MOUVEMENTS.Where(p => p.MOUVEMENTS.ID == Code).ToList().ForEach(p => BD.DETAILS_MOUVEMENTS.Remove(p));
            BD.SaveChanges();
            BD.MOUVEMENTS.Remove(Selected);
            BD.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Retour(int Code)
        {
            MOUVEMENTS mouvement = BD.MOUVEMENTS.Find(Code);
            if (mouvement.TYPE == "RETOUR")
            {
                return RedirectToAction("Index");
            }
            else
            {
                MOUVEMENTS Back = new MOUVEMENTS();
                Back.TYPE = "RETOUR";
                Back.DATE_MOUVEMENT = mouvement.DATE_MOUVEMENT;
                Back.DU = mouvement.AU;
                Back.DEPARTS = mouvement.RETOURS;
                Back.AU = mouvement.DU;
                Back.RETOURS = mouvement.DEPARTS;
                Back.CLIENT = mouvement.CLIENT;
                Back.CLIENTS = mouvement.CLIENTS;
                Back.TRANSPORTEUR = mouvement.TRANSPORTEUR;
                Back.TRANSPORTEURS = mouvement.TRANSPORTEURS;
                Back.DISTANCE = mouvement.DISTANCE;
                List<DETAILS_MOUVEMENTS> liste = BD.DETAILS_MOUVEMENTS.Where(Elemnt => Elemnt.MOUVEMENTS.ID == Code && Elemnt.IMMOBILISATIONS.DISPONIBILITE == false).ToList();
                ViewBag.TYPE = Back.TYPE;
                ViewBag.DATE_MOUVEMENT = DateTime.Today.ToShortDateString();
                ViewBag.DU = Back.DU;
                ViewBag.DEPARTS = Back.DEPARTS.INTITULE;
                ViewBag.AU = Back.AU;
                ViewBag.RETOURS = Back.RETOURS.INTITULE;
                ViewBag.CLIENT = Back.CLIENT;
                ViewBag.CLIENTS = Back.CLIENTS.INTITULE;
                ViewBag.DISTANCE = Back.DISTANCE.ToString("F3").Replace(',', '.');
                ViewBag.Mode = "Create";
                return View(liste);
            }

        }
        public ActionResult Print(string Filter)
        {
            List<MOUVEMENTS> Liste = BD.MOUVEMENTS.ToList();
            if (!string.IsNullOrEmpty(Filter))
            {
                Liste = Liste.Where(Element => Element.TYPE == Filter).ToList();
            }
            dynamic dt = from Element in Liste
                         select new
                         {
                             TYPE = Element.TYPE,
                             DATE_MOUVEMENT = Element.DATE_MOUVEMENT != null ? Element.DATE_MOUVEMENT.ToShortDateString() : string.Empty,
                             DISTANCE = Element.DISTANCE != null ? Element.DISTANCE.ToString("F3") : "0",
                             DU = Element.DEPARTS != null ? Element.DEPARTS.INTITULE : string.Empty,
                             AU = Element.RETOURS != null ? Element.RETOURS.INTITULE : string.Empty,
                             CLIENT = Element.CLIENTS != null ? Element.CLIENTS.INTITULE : string.Empty,
                             TRANSPORTEUR = Element.TRANSPORTEURS != null ? Element.TRANSPORTEURS.INTITULE : string.Empty,
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/LISTE_MOUVEMENT.rpt");
            rptH.Load(FileName);
            rptH.SetDataSource(dt);
            rptH.SummaryInfo.ReportTitle = "Liste des mouvements";
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintG()
        {
            List<MOUVEMENTS> Liste = BD.MOUVEMENTS.ToList();
            dynamic dt = from Element in Liste
                         select new
                         {
                             TYPE = Element.TYPE,
                             DATE_MOUVEMENT = Element.DATE_MOUVEMENT != null ? Element.DATE_MOUVEMENT.ToShortDateString() : string.Empty,
                             DISTANCE = Element.DISTANCE != null ? Element.DISTANCE.ToString("F3") : "0",
                             DU = Element.DEPARTS != null ? Element.DEPARTS.INTITULE : string.Empty,
                             AU = Element.RETOURS != null ? Element.RETOURS.INTITULE : string.Empty,
                             CLIENT = Element.CLIENTS != null ? Element.CLIENTS.INTITULE : string.Empty,
                             TRANSPORTEUR = Element.TRANSPORTEURS != null ? Element.TRANSPORTEURS.INTITULE : string.Empty,
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/MOUVEMENT_GROUP.rpt");
            rptH.Load(FileName);
            rptH.SetDataSource(dt);
            rptH.SummaryInfo.ReportTitle = "Liste des mouvements";
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintMouvement(string Code)
        {
            int ID = int.Parse(Code);
            MOUVEMENTS Mouvement = BD.MOUVEMENTS.Find(ID);
            List<DETAILS_MOUVEMENTS> Liste = BD.DETAILS_MOUVEMENTS.Where(Element => Element.MOUVEMENTS.ID == ID).ToList();
            dynamic dt = from Element in Liste
                         select new
                         {
                             TYPE = Mouvement.TYPE,
                             DATE_MOUVEMENT = Mouvement.DATE_MOUVEMENT != null ? Mouvement.DATE_MOUVEMENT.ToShortDateString() : string.Empty,
                             DISTANCE = Mouvement.DISTANCE != null ? Mouvement.DISTANCE.ToString("F3") : "0",
                             DU = Mouvement.DEPARTS != null ? Mouvement.DEPARTS.INTITULE : string.Empty,
                             AU = Mouvement.RETOURS != null ? Mouvement.RETOURS.INTITULE : string.Empty,
                             CLIENT = Mouvement.CLIENTS != null ? Mouvement.CLIENTS.INTITULE : string.Empty,
                             TRANSPORTEUR = Mouvement.TRANSPORTEURS != null ? Mouvement.TRANSPORTEURS.INTITULE : string.Empty,
                             IMMOBILISATION=Element.IMMOBILISATIONS!=null ? Element.IMMOBILISATIONS.CODE:string.Empty,
                             FAMILLE = Element.IMMOBILISATIONS != null ? Element.IMMOBILISATIONS.FAMILLES_IMMOBILISATIONS.FAMILLE : string.Empty,
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/MOUVEMENT.rpt");
            rptH.Load(FileName);
            rptH.SetDataSource(dt);
            rptH.SummaryInfo.ReportTitle = "Detail du mouvement";
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }


    }
}
