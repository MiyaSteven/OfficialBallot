using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OfficialBallot.Models
{
    public class BallotWrapper
    {
        public User UserForm { get; set; }
        public Proposal ProposalForm { get; set; }
        public List<User> AllUsers { get; set; }
        public List<Proposal> AllProposals { get; set; }
    }
}