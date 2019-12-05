using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace PickSanta.DB.Entities
{
    public class SantaMap : TableEntity
    {
        internal static string PK = "SantaMapping";
        public SantaMap()
        {
            this.PartitionKey = PK.ToLower();
        }

        public SantaMap(string giftee, string santa) : this()
        {
            this.RowKey = santa.ToLower();
            this.Giftee = giftee;
            this.Santa = santa;
        }

        public string Santa { get; set; }

        public string Giftee { get; set; }
    }
}
