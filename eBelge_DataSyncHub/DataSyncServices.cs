#region Using

using eBelge_DataSyncHub.Func;
using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading.Tasks;

#endregion

namespace eBelge_DataSyncHub
{
    partial class DataSyncServices : ServiceBase
    {
        #region Region

        Glb_Func glb_Func;
        e_Document_Func e_Document_Func;
        e_Ledger_Func e_Ledger_Func;
        paymentRecorder paymentRecorder;
        e_LedgerDeclaration_Func e_LedgerDeclaration_Func;
        private System.Threading.Timer timer;

        #endregion

        #region Init

        public DataSyncServices()
        {
            glb_Func = new Glb_Func();
            e_Document_Func = new e_Document_Func();
            e_Ledger_Func = new e_Ledger_Func();
            e_LedgerDeclaration_Func = new e_LedgerDeclaration_Func();
            paymentRecorder = new paymentRecorder();

            InitializeComponent();
        }

        #endregion

        #region Start

        protected override async void OnStart(string[] args)
        {

            try
            {
                glb_Func.WriteLog("Servis başlatıldı.", EventLogEntryType.Information);

                await Transactions();

                timer = new System.Threading.Timer(
                    async (e) => await Transactions(),
                    null,
                    TimeSpan.Zero,
                    TimeSpan.FromHours(1)
                );
            }
            catch (Exception ex)
            {
                glb_Func.WriteLog(ex.Message, EventLogEntryType.Error);
                throw;
            }
        }

        #endregion

        #region Stop

        protected override void OnStop()
        {
            try
            {
                glb_Func.WriteLog("Servis durduruldu.", EventLogEntryType.Information);

            }
            catch (Exception ex)
            {
                glb_Func.WriteLog(ex.Message, EventLogEntryType.Error);
                throw;
            }
        }

        #endregion

        #region Transactions
        private async Task Transactions()
        {
            try
            {
                await e_Document_Func.saveAndPushNotification();
                await e_Ledger_Func.saveAndPushNotification();
                await paymentRecorder.saveAndPushNotification();
                await e_LedgerDeclaration_Func.saveAndPushNotification();
            }
            catch (Exception ex)
            {
                glb_Func.WriteLog(ex.Message, EventLogEntryType.Error);
                throw;
            }
        }

        #endregion

    }
}
