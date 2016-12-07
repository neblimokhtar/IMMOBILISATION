using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IMMOBILISATION.Models;
using System.Globalization;
using System.IO;
using System.Text;
using QRCoder;
using System.IO;
using System.Drawing;

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
            FAMILLE = FAMILLE.Substring(0, 1);
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
                    return RedirectToAction("EditerFicheTechnique", "Immobilisation", new { @Code = Element.ID });
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
                BD.SaveChanges();
                if (MyAction == "Fiche")
                {
                    return RedirectToAction("EditerFicheTechnique", "Immobilisation", new { @Code = ID });
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
        public ActionResult EditerFicheTechnique(string Code)
        {
            int ID = int.Parse(Code);
            IMMOBILISATIONS Immobilisation = BD.IMMOBILISATIONS.Find(ID);
            FICHES_TECHNIQUES Fiche = BD.FICHES_TECHNIQUES.Where(Element => Element.IMMOBILISATIONS.ID == ID).FirstOrDefault();
            if (Fiche == null)
            {
                Fiche = new FICHES_TECHNIQUES();
            }
            ViewBag.IMMOBILISATION = Immobilisation.CODE;
            ViewBag.DESIGNATION = Immobilisation.DESIGNATION;
            ViewBag.FAMILLE = Immobilisation.FAMILLES_IMMOBILISATIONS.FAMILLE;
            ViewBag.FILTER = Immobilisation.ID;
            return View(Fiche);
        }
        [HttpPost]
        public ActionResult UploadFiche(HttpPostedFileBase fileupload, string FILTER)
        {
            string MARQUE = Request.Params["MARQUE"] != null ? Request.Params["MARQUE"].ToString() : string.Empty;
            string LIEU_FABRICATION = Request.Params["LIEU_FABRICATION"] != null ? Request.Params["LIEU_FABRICATION"].ToString() : string.Empty;
            string DESCRIPTION = Request.Params["DESCRIPTION"] != null ? Request.Params["DESCRIPTION"].ToString() : string.Empty;
            string COMPOSITION = Request.Params["COMPOSITION"] != null ? Request.Params["COMPOSITION"].ToString() : string.Empty;
            string DIMENSION = Request.Params["DIMENSION"] != null ? Request.Params["DIMENSION"].ToString() : string.Empty;
            string POID = Request.Params["POID"] != null ? Request.Params["POID"].ToString() : string.Empty;
            string PUISSANCE = Request.Params["PUISSANCE"] != null ? Request.Params["PUISSANCE"].ToString() : string.Empty;
            string CONSOMMATION = Request.Params["CONSOMMATION"] != null ? Request.Params["CONSOMMATION"].ToString() : string.Empty;
            string COULEUR = Request.Params["COULEUR"] != null ? Request.Params["COULEUR"].ToString() : string.Empty;

            int ID = int.Parse(FILTER);
            IMMOBILISATIONS Immobilisation = BD.IMMOBILISATIONS.Find(ID);
            FICHES_TECHNIQUES Fiche = BD.FICHES_TECHNIQUES.Where(Element => Element.IMMOBILISATIONS.ID == ID).FirstOrDefault();
            if (Fiche == null)
            {
                Fiche = new FICHES_TECHNIQUES();
                Fiche.IMMOBILISATION = ID;
                Fiche.IMMOBILISATIONS = Immobilisation;
                BD.FICHES_TECHNIQUES.Add(Fiche);
                BD.SaveChanges();
            }
            Fiche.MARQUE = MARQUE;
            Fiche.LIEU_FABRICATION = LIEU_FABRICATION;
            Fiche.DESCRIPTION = DESCRIPTION;
            Fiche.COMPOSITION = COMPOSITION;
            Fiche.DIMENSION = DIMENSION;
            Fiche.POID = POID;
            Fiche.PUISSANCE = PUISSANCE;
            Fiche.CONSOMMATION = CONSOMMATION;
            Fiche.COULEUR = COULEUR;
            if (fileupload != null)
            {
                string pic = System.IO.Path.GetFileName(fileupload.FileName);
                string path = System.IO.Path.Combine(
                                       Server.MapPath("~/App_Data/"), pic);
                fileupload.SaveAs(path);
                using (MemoryStream ms = new MemoryStream())
                {
                    fileupload.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                    Fiche.PHOTO = array;
                }
            }
            BD.SaveChanges();
            return RedirectToAction("FicheTechnique", "Immobilisation", new { @Code = ID });
        }
        public ActionResult FicheTechnique(string Code)
        {
            int ID = int.Parse(Code);
            IMMOBILISATIONS Immobilisation = BD.IMMOBILISATIONS.Find(ID);
            ViewBag.FOURNISSEUR = Immobilisation.TIERS.INTITULE;
            ViewBag.MARQUE = string.Empty;
            ViewBag.DESCRIPTION = string.Empty;
            ViewBag.COMPOSITION = string.Empty;
            ViewBag.LIEU_FABRICATION = string.Empty;
            ViewBag.DIMENSION = string.Empty;
            ViewBag.COULEUR = string.Empty;
            ViewBag.POID = string.Empty;
            ViewBag.PUISSANCE = string.Empty;
            ViewBag.CONSOMMATION = string.Empty;
            FICHES_TECHNIQUES Fiche = BD.FICHES_TECHNIQUES.Where(Element => Element.IMMOBILISATIONS.ID == ID).FirstOrDefault();
            if (Fiche != null)
            {
                ViewBag.MARQUE = Fiche.MARQUE;
                ViewBag.DESCRIPTION = Fiche.DESCRIPTION;
                ViewBag.COMPOSITION = Fiche.COMPOSITION;
                ViewBag.LIEU_FABRICATION = Fiche.LIEU_FABRICATION;
                ViewBag.DIMENSION = Fiche.DIMENSION;
                ViewBag.COULEUR = Fiche.COULEUR;
                ViewBag.POID = Fiche.POID;
                ViewBag.PUISSANCE = Fiche.PUISSANCE;
                ViewBag.CONSOMMATION = Fiche.CONSOMMATION;
            }
            ViewBag.IMMOBILISATION = ID;
            return View(Immobilisation);
        }
        public ActionResult Show(string Code)
        {
            int ID = int.Parse(Code);
            IMMOBILISATIONS Immobilisation = BD.IMMOBILISATIONS.Find(ID);
            FICHES_TECHNIQUES Fiche = BD.FICHES_TECHNIQUES.Where(Element => Element.IMMOBILISATIONS.ID == ID).FirstOrDefault();
            var imageData = System.IO.File.ReadAllBytes(Server.MapPath("~//img/NotAvailable.jpg"));
            if (Fiche != null)
            {
                if (Fiche.PHOTO != null)
                {
                    imageData = Fiche.PHOTO;
                }
            }
            return File(imageData, "image/jpg");
        }
        public ActionResult ShowBarCode(string Code)
        {
            int ID = int.Parse(Code);
            IMMOBILISATIONS Immobilisation = BD.IMMOBILISATIONS.Find(ID);
            var imageData = getBarcodeImage(Immobilisation.CODE_A_BARRE, string.Empty);
            return File(imageData, "image/jpg");
        }
        public Byte[] getBarcodeImage(string barcode, string file)
        {
            try
            {
                BarCode39 _barcode = new BarCode39();
                int barSize = 16;
                string fontFile = Server.MapPath("~/fonts/FREE3OF9.TTF");
                return (_barcode.Code39(barcode, barSize, true, file, fontFile));
            }
            catch (Exception ex)
            {
                //ErrorLog.WriteErrorLog("Barcode", ex.ToString(), ex.Message);
            }
            return null;
        }
        public ActionResult ShowCodeQr(string Code)
        {
            int ID = int.Parse(Code);
            IMMOBILISATIONS Immobilisation = BD.IMMOBILISATIONS.Find(ID);
            var imageData = getQRcodeImage(Immobilisation.CODE);
            return File(imageData, "image/jpg");
        }
        public Byte[] getQRcodeImage(string Code)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(Code, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            return ImageToByte(qrCodeImage);
        }
        public static byte[] ImageToByte2(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
        public byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
    }
}
