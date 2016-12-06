using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IMMOBILISATION.Models;
using System.Globalization;

namespace IMMOBILISATION.Controllers
{
    [Authorize]
    public class ImmobilisationController : Controller
    {
        //
        // GET: /Immobilisation/
        ImmobilisationEntities BD = new ImmobilisationEntities();
        public ActionResult Index(string ParamPassed, string Disp, string Occp)
        {
            List<IMMOBILISATIONS> Liste = BD.IMMOBILISATIONS.ToList();
            if (!string.IsNullOrEmpty(ParamPassed))
            {
                int ID = int.Parse(ParamPassed);
                Liste = Liste.Where(Element => Element.FAMILLE == ID).ToList();
            }
            Boolean BoolDisp = false;
            Boolean BoolOccp = false;
            Boolean Value;
            if (Boolean.TryParse(Disp, out Value))
            {
                BoolDisp = Boolean.Parse(Disp);
            }
            if (Boolean.TryParse(Occp, out Value))
            {
                BoolOccp = Boolean.Parse(Occp);
            }
            if (BoolDisp && !BoolOccp)
            {
                Liste = Liste.Where(Element => Element.DISPONIBILITE == true).ToList();
            }
            if (BoolOccp && !BoolDisp)
            {
                Liste = Liste.Where(Element => Element.DISPONIBILITE == false).ToList();
            }
            return View(Liste);
        }
        public ActionResult Form(string Mode, int Code)
        {
            IMMOBILISATIONS Element = new IMMOBILISATIONS();
            if (Mode == "Create")
            {
                ViewBag.TITRE = "NOUVELLE IMMOBILISATION";
            }
            if (Mode == "Edit")
            {
                Element = BD.IMMOBILISATIONS.Find(Code);
                ViewBag.TITRE = "MODIFIER UNE IMMOBILISATION";
            }
            ViewBag.Mode = Mode;
            ViewBag.Code = Code;
            return View(Element);
        }
        public JsonResult GetAllFamille()
        {
            BD.Configuration.ProxyCreationEnabled = false;
            List<FAMILLES_IMMOBILISATIONS> Liste = BD.FAMILLES_IMMOBILISATIONS.ToList();
            return Json(Liste, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllFournisseur()
        {
            BD.Configuration.ProxyCreationEnabled = false;
            List<TIERS> Liste = BD.TIERS.Where(Element => Element.TYPE == "FOURNISSEURS").ToList();
            return Json(Liste, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SendForm(string Mode, string Code)
        {
            string TYPE = Request.Params["TYPE"] != null ? Request.Params["TYPE"].ToString() : string.Empty;
            string DESIGNATION = Request.Params["DESIGNATION"] != null ? Request.Params["DESIGNATION"].ToString() : string.Empty;
            string VALEUR_ACQUISITION_TTC = Request.Params["VALEUR_ACQUISITION_TTC"] != null ? Request.Params["VALEUR_ACQUISITION_TTC"].ToString() : "0";
            string CODE_A_BARRE = Request.Params["CODE_A_BARRE"] != null ? Request.Params["CODE_A_BARRE"].ToString() : string.Empty;
            string FAMILLE = Request.Params["FAMILLE"] != null ? Request.Params["FAMILLE"].ToString() : string.Empty;
            string FOURNISSEUR = Request.Params["FOURNISSEUR"] != null ? Request.Params["FOURNISSEUR"].ToString() : string.Empty;
            string DATE_MISE_EN_SERVICE = Request.Params["DATE_MISE_EN_SERVICE"] != null ? Request.Params["DATE_MISE_EN_SERVICE"].ToString() : string.Empty;
            string DATE_AQUISITION = Request.Params["DATE_AQUISITION"] != null ? Request.Params["DATE_AQUISITION"].ToString() : string.Empty;
            string DISPONIBILITE = Request.Params["DISPONIBILITE"] != null ? "true" : "false";
            string MyAction = Request.Params["MyAction"] != null ? Request.Params["MyAction"].ToString() : string.Empty;

            int ID_Fournisseur = int.Parse(FOURNISSEUR);
            TIERS fournisseur = BD.TIERS.Find(ID_Fournisseur);
            int ID_Famille = int.Parse(FAMILLE);
            FAMILLES_IMMOBILISATIONS famille = BD.FAMILLES_IMMOBILISATIONS.Find(ID_Famille);
            int Max = 1;
            List<string> listeCode = BD.IMMOBILISATIONS.Where(Element => Element.FAMILLES_IMMOBILISATIONS.ID == ID_Famille).Select(Element => Element.CODE).ToList();
            foreach (string Item in listeCode)
            {
                string NUMBER = Item.Substring(Item.Length - 4);
                int number = int.Parse(NUMBER);
                if (number > Max) Max = number;
                Max++;
            }
            if (Mode == "Create")
            {
                IMMOBILISATIONS Element = new IMMOBILISATIONS();
                Element.CODE = famille.FAMILLE + Max.ToString("0000");
                Element.TYPE = TYPE;
                Element.DESIGNATION = DESIGNATION;
                Element.VALEUR_ACQUISITION_TTC = 0;
                float value;
                if (float.TryParse(VALEUR_ACQUISITION_TTC, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                {
                    Element.VALEUR_ACQUISITION_TTC = float.Parse(VALEUR_ACQUISITION_TTC, CultureInfo.InvariantCulture);
                }
                Element.CODE_A_BARRE = CODE_A_BARRE;
                Element.FAMILLE = ID_Famille;
                Element.FAMILLES_IMMOBILISATIONS = famille;
                Element.FOURNISSEUR = ID_Fournisseur;
                Element.TIERS = fournisseur;
                Element.DATE_AQUISITION = DateTime.Parse(DATE_AQUISITION);
                Element.DATE_MISE_EN_SERVICE = DateTime.Parse(DATE_MISE_EN_SERVICE);
                Element.DISPONIBILITE = Boolean.Parse(DISPONIBILITE);
                BD.IMMOBILISATIONS.Add(Element);
                BD.SaveChanges();
                if (MyAction == "Fiche")
                {

                }
            }
            if (Mode == "Edit")
            {
                int ID = int.Parse(Code);
                IMMOBILISATIONS Element = BD.IMMOBILISATIONS.Find(ID);
                Element.TYPE = TYPE;
                Element.DESIGNATION = DESIGNATION;
                Element.VALEUR_ACQUISITION_TTC = 0;
                float value;
                if (float.TryParse(VALEUR_ACQUISITION_TTC, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                {
                    Element.VALEUR_ACQUISITION_TTC = float.Parse(VALEUR_ACQUISITION_TTC, CultureInfo.InvariantCulture);
                }
                Element.CODE_A_BARRE = CODE_A_BARRE;
                Element.FOURNISSEUR = ID_Fournisseur;
                Element.TIERS = fournisseur;
                Element.DATE_AQUISITION = DateTime.Parse(DATE_AQUISITION);
                Element.DATE_MISE_EN_SERVICE = DateTime.Parse(DATE_MISE_EN_SERVICE);
                Element.DISPONIBILITE = Boolean.Parse(DISPONIBILITE);
                BD.IMMOBILISATIONS.Add(Element);
                BD.SaveChanges();
                if (MyAction == "Fiche")
                {
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int Code)
        {
            IMMOBILISATIONS Selected = BD.IMMOBILISATIONS.Find(Code);
            BD.IMMOBILISATIONS.Remove(Selected);
            BD.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
