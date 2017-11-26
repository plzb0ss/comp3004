﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TournamentMasterAPI.Models;
using TournamentMasterAPI.Builders;

namespace TournamentMasterAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Tournaments")]
    public class TournamentsController : Controller
    {
        private readonly TournamentMasterDBContext _context;

        public TournamentsController(TournamentMasterDBContext context)
        {
            _context = context;
        }

        // GET: api/Tournaments?organization=5
        [HttpGet("{organization?}")]
        public IEnumerable<Tournament> GetTournaments([FromQuery] int? organization = null)
        {
            Account userAccount = Shared.GetUserAccount(User, _context);
            IEnumerable<Tournament> userTournaments = Shared.UserTournaments(userAccount, _context);
            if (organization != null)
            {
                userTournaments = userTournaments.Where(t => t.OrganizationId == organization);
            }
            return userTournaments;
        }

        // GET: api/Tournaments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTournaments([FromRoute] int id)
        {
            Account userAccount = Shared.GetUserAccount(User, _context);
            if (Shared.UserTournaments(userAccount, _context).Any(t => t.Id == id))
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tournaments = await _context.Tournaments.SingleOrDefaultAsync(m => m.Id == id);

            if (tournaments == null)
            {
                return NotFound();
            }

            return Ok(tournaments);
        }

        // PUT: api/Tournaments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTournaments([FromRoute] int id, [FromBody] Tournament tournaments)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tournaments.Id)
            {
                return BadRequest();
            }

            _context.Entry(tournaments).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TournamentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Tournaments
        [HttpPost("{seed}")]
        public async Task<IActionResult> PostTournaments([FromBody] TournamentParameters tparams, [FromQuery] string seed = "manual")
        {
            // this will disable warning
            // await Task.Run(() => { });
            Account userAccount = Shared.GetUserAccount(User, _context);
            if (Shared.UserOrganizations(userAccount,_context).Any(o => o.Id != tparams.Tournament.OrganizationId))
            {
                return Unauthorized();
            }
            foreach(Competitor comp in tparams.Competitors)
            {
                if (comp.OrganizationId != tparams.Tournament.OrganizationId)
                {
                    return BadRequest();
                }
                if (!_context.Competitors.Any(c => c.Id == comp.Id))
                {
                    return BadRequest();
                }
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            TournamentBuilder.InititializeTournament(_context, tparams.Competitors, tparams.Tournament, seed);
            return CreatedAtAction("GetTournaments", new { id = tparams.Tournament.Id }, tparams.Tournament);
        }

        // DELETE: api/Tournaments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTournaments([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tournaments = await _context.Tournaments.SingleOrDefaultAsync(m => m.Id == id);
            if (tournaments == null)
            {
                return NotFound();
            }

            _context.Tournaments.Remove(tournaments);
            await _context.SaveChangesAsync();

            return Ok(tournaments);
        }

        private bool TournamentExists(int id)
        {
            return _context.Tournaments.Any(e => e.Id == id);
        }
    }
}