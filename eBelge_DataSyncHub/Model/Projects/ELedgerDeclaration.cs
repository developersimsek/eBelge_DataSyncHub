using System.Collections.Generic;

namespace eBelge_DataSyncHub.Model.Projects
{
    public class ELedgerDeclaration
    {
        public List<ELedgerDeclarationItems> items { get; set; }
        public class ELedgerDeclarationItems
        {
            public string description { get; set; }
            public string date { get; set; }
        }

    }
}