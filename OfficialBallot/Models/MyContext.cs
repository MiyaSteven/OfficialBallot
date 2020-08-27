using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OfficialBallot.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options)
        { }
        public DbSet<User> DbUsers { get; set; }
        public DbSet<Proposal> DbProposals { get; set; }
        public DbSet<RSVP> DbVoters { get; set; }
    }
}