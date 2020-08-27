using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using OfficialBallot.Models;

namespace OfficialBallot.Controllers
{
    public class BallotController : Controller
    {
        private MyContext DbContext;

        public BallotController(MyContext context)
        {
            DbContext = context;
        }

        [HttpGet("ballot/dashboard")]
        public IActionResult Dashboard()
        {
            int? LoggedId = HttpContext.Session.GetInt32("UserId");
            if (LoggedId == null)
            {
                return RedirectToAction("LogReg");
            }

            BallotWrapper WMod = new BallotWrapper()
            {
                AllProposals = DbContext.DbProposals
                    .Include(w => w.Official)
                    .Include(w => w.ProposalVoters)
                    .ThenInclude(r => r.Voter)
                    .ToList(),
                UserForm = DbContext.DbUsers
                    .FirstOrDefault(u => u.UserId == (int)LoggedId)
            };
            return View("Dashboard", WMod);
        }

        // Routes for Creating Proposal
        //
        [HttpGet("ballot/proposals/new")]
        public IActionResult CreateProposal()
        {
            int? LoggedId = HttpContext.Session.GetInt32("UserId");
            if (LoggedId == null)
            {
                return RedirectToAction("LogReg");
            }
            return View("CreateProposal");
        }

        [HttpPost("ballot/proposals/create")]
        public IActionResult AddProposal(Proposal FromForm)
        {
            int? LoggedId = HttpContext.Session.GetInt32("UserId");
            if (LoggedId == null)
            {
                return RedirectToAction("LogReg");
            }

            FromForm.VoterId = (int)LoggedId;

            if (ModelState.IsValid)
            {
                DbContext.Add(FromForm);
                DbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            else
            {
                return CreateProposal();
            }
        }

        // Route for Reading Proposal Data, Voting and Removing Vote
        //
        [HttpGet("ballot/proposals/{ProposalId}")]
        public IActionResult Proposal(int ProposalId)
        {
            int? LoggedId = HttpContext.Session.GetInt32("UserId");
            if (LoggedId == null)
            {
                return RedirectToAction("LogReg");
            }

            BallotWrapper GMod = new BallotWrapper();

            GMod.AllProposals = DbContext.DbProposals
                .Include(w => w.Official)
                .Include(w => w.ProposalVoters)
                .ThenInclude(r => r.Voter)
                .Where(w => w.ProposalId == ProposalId)
                .ToList();
            GMod.UserForm = DbContext.DbUsers
                .FirstOrDefault(u => u.UserId == (int)LoggedId);

            if (GMod == null)
            {
                return RedirectToAction("Dashboard");
            }
            return View("Proposal", GMod);
        }

        [HttpGet("ballot/proposals/{ProposalId}/vote")]
        public RedirectToActionResult Vote(int ProposalId)
        {
            int? LoggedId = HttpContext.Session.GetInt32("UserId");
            if (LoggedId == null)
            {
                return RedirectToAction("LogReg");
            }
            Proposal ToJoin = DbContext.DbProposals
                .Include(w => w.ProposalVoters)
                .FirstOrDefault(w => w.ProposalId == ProposalId);

            if (ToJoin == null || ToJoin.VoterId == (int)LoggedId || ToJoin.ProposalVoters.Any(r => r.VoterId == (int)LoggedId))
            {
                return RedirectToAction("Dashboard");
            }
            else
            {
                RSVP NewRsvp = new RSVP()
                {
                    VoterId = (int)LoggedId,
                    ProposalId = ProposalId
                };
                DbContext.Add(NewRsvp);
                DbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
        }

        [HttpGet("ballot/proposals/{ProposalId}/unvote")]
        public RedirectToActionResult UnVote(int ProposalId)
        {
            int? LoggedId = HttpContext.Session.GetInt32("VoterId");
            if (LoggedId == null)
            {
                return RedirectToAction("LogReg");
            }
            Proposal ToLeave = DbContext.DbProposals
                .Include(w => w.ProposalVoters)
                .FirstOrDefault(w => w.ProposalId == ProposalId);

            if (ToLeave == null || !ToLeave.ProposalVoters.Any(r => r.VoterId == (int)LoggedId))
            {
                return RedirectToAction("Proposal");
            }
            else
            {
                RSVP ToRemove = DbContext.DbVoters.FirstOrDefault(r => r.VoterId == (int)LoggedId && r.ProposalId == ProposalId);
                DbContext.Remove(ToRemove);
                DbContext.SaveChanges();

                return RedirectToAction("Dashboard");
            }
        }

        [HttpGet("ballot/proposals/{ProposalId}/edit")]
        public IActionResult EditProposal(int ProposalId)
        {
            int? LoggedId = HttpContext.Session.GetInt32("UserId");
            if (LoggedId == null)
            {
                return RedirectToAction("LogReg");
            }

            Proposal ToEdit = DbContext.DbProposals.FirstOrDefault(w => w.ProposalId == ProposalId);

            if (ToEdit == null || ToEdit.VoterId != (int)LoggedId)
            {
                return RedirectToAction("Dashboard");
            }

            return View("EditProposal", ToEdit);
        }

        [HttpPost("ballot/proposals/{ProposalId}/update")]
        public IActionResult UpdateProposal(int ProposalId, Proposal FromForm)
        {
            int? LoggedId = HttpContext.Session.GetInt32("UserId");
            if (LoggedId == null)
            {
                return RedirectToAction("LogReg");
            }

            if (!DbContext.DbProposals.Any(w => w.ProposalId == ProposalId && w.VoterId == (int)LoggedId))
            {
                return RedirectToAction("Dashboard");
            }

            FromForm.VoterId = (int)LoggedId;
            if (ModelState.IsValid)
            {
                FromForm.ProposalId = ProposalId;
                DbContext.Update(FromForm);
                DbContext.Entry(FromForm).Property("CreatedAt").IsModified = false;
                DbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            else
            {
                return EditProposal(ProposalId);
            }
        }

        [HttpGet("ballot/proposals/{ProposalId}/delete")]
        public RedirectToActionResult DeleteProposal(int ProposalId)
        {
            int? LoggedId = HttpContext.Session.GetInt32("UserId");
            if (LoggedId == null)
            {
                return RedirectToAction("LogReg");
            }

            Proposal ToDelete = DbContext.DbProposals
                .FirstOrDefault(w => w.ProposalId == ProposalId);

            if (ToDelete == null || ToDelete.VoterId != (int)LoggedId)
            {
                return RedirectToAction("Dashboard");
            }

            DbContext.Remove(ToDelete);
            DbContext.SaveChanges();
            return RedirectToAction("Proposal");
        }
    }
}