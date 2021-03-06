﻿using System;
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
    public class TiersController : Controller
    {
        //
        // GET: /Tiers/
        ImmobilisationEntities BD = new ImmobilisationEntities();
        public ActionResult Index(string ParamPassed)
        {
            List<TIERS> Liste = BD.TIERS.ToList();
            ViewBag.TITRE = "TIERS";
            if (ParamPassed == "CLIENTS") { Liste = Liste.Where(Element => Element.TYPE == "CLIENTS").ToList(); ViewBag.TITRE = "CLIENTS"; }
            if (ParamPassed == "FOURNISSEURS") { Liste = Liste.Where(Element => Element.TYPE == "FOURNISSEURS").ToList(); ViewBag.TITRE = "FOURNISSEURS"; }
            if (ParamPassed == "TRANSPORTEURS") { Liste = Liste.Where(Element => Element.TYPE == "TRANSPORTEURS").ToList(); ViewBag.TITRE = "TRANSPORTEURS"; }
            return View(Liste);
        }
        public ActionResult Form(string Mode, int Code)
        {
            USERS_ADMINISTRATIONS CurrentUser = BD.USERS_ADMINISTRATIONS.Where(Elt => Elt.Login == User.Identity.Name).FirstOrDefault();
            if (CurrentUser.Role != "Visiteur")
            {
                TIERS Element = new TIERS();
                if (Mode == "Create")
                {
                    ViewBag.TITRE = "NOUVEAU TIERS";
                }
                if (Mode == "Edit")
                {
                    Element = BD.TIERS.Find(Code);
                    ViewBag.TITRE = "MODIFIER UN TIERS";
                }
                ViewBag.Mode = Mode;
                ViewBag.Code = Code;
                return View(Element);
            }
            else {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public ActionResult SendForm(string Mode, string Code)
        {
            string TYPE = Request.Params["TYPE"] != null ? Request.Params["TYPE"].ToString() : string.Empty;
            string INTITULE = Request.Params["INTITULE"] != null ? Request.Params["INTITULE"].ToString() : string.Empty;
            string ABREGE = Request.Params["ABREGE"] != null ? Request.Params["ABREGE"].ToString() : string.Empty;
            string QUALITE = Request.Params["QUALITE"] != null ? Request.Params["QUALITE"].ToString() : string.Empty;
            string INTERLOCUTEUR = Request.Params["INTERLOCUTEUR"] != null ? Request.Params["INTERLOCUTEUR"].ToString() : string.Empty;
            string COMMENTAIRE = Request.Params["COMMENTAIRE"] != null ? Request.Params["COMMENTAIRE"].ToString() : string.Empty;

            if (Mode == "Create")
            {
                TIERS Element = new TIERS();
                Element.TYPE = TYPE;
                Element.INTITULE = INTITULE;
                Element.ABREGE = ABREGE;
                Element.QUALITE = QUALITE;
                Element.INTERLOCUTEUR = INTERLOCUTEUR;
                Element.COMMENTAIRE = COMMENTAIRE;
                BD.TIERS.Add(Element);
                BD.SaveChanges();
            }
            if (Mode == "Edit")
            {
                int ID = int.Parse(Code);
                TIERS Element = BD.TIERS.Find(ID);
                Element.TYPE = TYPE;
                Element.INTITULE = INTITULE;
                Element.ABREGE = ABREGE;
                Element.QUALITE = QUALITE;
                Element.INTERLOCUTEUR = INTERLOCUTEUR;
                Element.COMMENTAIRE = COMMENTAIRE;
                BD.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int Code)
        {
            TIERS Selected = BD.TIERS.Find(Code);
            BD.TIERS.Remove(Selected);
            BD.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Print(string Filter)
        {
            List<TIERS> Liste = BD.TIERS.ToList();
            if (!string.IsNullOrEmpty(Filter))
            {
                Liste = Liste.Where(Element => Element.TYPE == Filter).ToList();
            }
            dynamic dt = from Element in Liste
                         select new
                         {
                             TYPE = Element.TYPE,
                             INTITULE = Element.INTITULE,
                             ABREGE = Element.ABREGE,
                             QUALITE=Element.QUALITE,
                             INTERLOCUTEUR=Element.INTERLOCUTEUR
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/TIERS.rpt");
            rptH.Load(FileName);
            rptH.SetDataSource(dt);
            rptH.SummaryInfo.ReportTitle = "Liste des tiers";
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }


    }
}
