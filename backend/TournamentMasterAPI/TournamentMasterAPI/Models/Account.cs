﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TournamentMasterAPI.Models
{
    public partial class Account
    {
        public Account()
        {
            AccountOrganization = new HashSet<AccountOrganization>();
        }

        public int Id { get; set; }
        public Guid AwsCognitoId { get; set; }
        [JsonIgnore]
        public ICollection<AccountOrganization> AccountOrganization { get; set; }
    }
}
