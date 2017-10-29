﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TournamentMasterAPI.Models;
using System.Security.Claims;

namespace TournamentMasterAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Organizations")]
    public class OrganizationsController : Controller
    {
        private readonly TournamentMasterDBContext _context;

        public OrganizationsController(TournamentMasterDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get the user account from the database from the authenticated token.
        /// Creating an entry if one doesn't already exist.
        /// </summary>
        /// <returns>The account associated with the logged in user</returns>
        private Accounts GetUserAccount()
        {
            // the sub claim from the JWT is the identifier from aws
            string sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid tokenId = new Guid(sub);

            // if account doesn't already exist create one!
            if (!_context.Accounts.Any(e => e.AwsCognitoId == tokenId))
            {
                Accounts acc = new Accounts
                {
                    AwsCognitoId = tokenId
                };
                _context.Accounts.Add(acc);
                _context.SaveChanges();
            }

            return _context.Accounts.FirstOrDefault(acc => acc.AwsCognitoId == tokenId);
        }
            

        // GET: api/Organizations
        [HttpGet]
        public IEnumerable<Organizations> GetOrganizations()
        {
            Accounts userAccount = GetUserAccount();
            var userOrganizations = _context.AccountOrganization.Where(
                e => e.AccountsId == userAccount.Id);
            return _context.Organizations.Where(
                e => userOrganizations.Any(
                    o => o.OrganizationsId == e.Id));
        }

        // GET: api/Organizations/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrganizations([FromRoute] int id)
        {
            // check if user has permission for this organization
            Accounts userAccount = GetUserAccount();
            if (_context.AccountOrganization.Any(e => e.OrganizationsId == id 
                && e.AccountsId == userAccount.Id))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var organizations = await _context.Organizations.SingleOrDefaultAsync(m => m.Id == id);

            if (organizations == null)
            {
                return NotFound();
            }
            
            return Ok(organizations);
        }

        // PUT: api/Organizations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrganizations([FromRoute] int id, [FromBody] Organizations organizations)
        {
            // check if user has permission for this organization
            Accounts userAccount = GetUserAccount();
            if (_context.AccountOrganization.Any(e => e.OrganizationsId == id
                && e.AccountsId == userAccount.Id))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != organizations.Id)
            {
                return BadRequest();
            }

            _context.Entry(organizations).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrganizationsExists(id))
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

        // POST: api/Organizations
        [HttpPost]
        public async Task<IActionResult> PostOrganizations([FromBody] Organizations organizations)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Organizations.Add(organizations);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrganizations", new { id = organizations.Id }, organizations);
        }

        // DELETE: api/Organizations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrganizations([FromRoute] int id)
        {
            // check if user has permission for this organization
            Accounts userAccount = GetUserAccount();
            if (_context.AccountOrganization.Any(e => e.OrganizationsId == id
                && e.AccountsId == userAccount.Id))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var organizations = await _context.Organizations.SingleOrDefaultAsync(m => m.Id == id);
            if (organizations == null)
            {
                return NotFound();
            }

            _context.Organizations.Remove(organizations);
            await _context.SaveChangesAsync();

            return Ok(organizations);
        }

        private bool OrganizationsExists(int id)
        {
            return _context.Organizations.Any(e => e.Id == id);
        }
    }
}