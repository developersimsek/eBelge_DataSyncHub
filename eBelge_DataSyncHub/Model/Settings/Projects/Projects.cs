using Newtonsoft.Json;

namespace eBelge_DataSyncHub.Model.Settings.Projects
{
    public class SettingProjects
    {
        public class Projets
        {
            [JsonProperty("e-Document")]
            public SettingEDocument eDocument { get; set; }

            [JsonProperty("e-Ledger")]
            public SettingELedger eLedger { get; set; }

            [JsonProperty("e-LedgerDeclaration")]
            public SettingELedgerDeclaration eLedgerDeclaration { get; set; }
            [JsonProperty("PaymentRecorder")]
            public SettingPaymentRecorder paymentRecorder { get; set; }
        }
        public class SettingEDocument : SettingProperty
        {
        }
        public class SettingELedger : SettingProperty
        {
        }
        public class SettingELedgerDeclaration : SettingProperty
        {
        }
        public class SettingPaymentRecorder : SettingProperty
        {
        }
    }
}
