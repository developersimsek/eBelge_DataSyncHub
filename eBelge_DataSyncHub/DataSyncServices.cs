#region Using

using eBelge_DataSyncHub.Func;
using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace eBelge_DataSyncHub
{
    partial class DataSyncServices : ServiceBase
    {
        #region Region

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        #endregion

        #region Init

        public DataSyncServices()
        {
            InitializeComponent();
        }

        #endregion

        #region Start

        protected override async void OnStart(string[] args)
        {
            Glb_Func glb_Func = new Glb_Func();

            try
            {
                glb_Func.WriteLog("Servis başlatıldı.", EventLogEntryType.Information);

                await Transactions();

                Task mainTask = Task.Run(async () =>
                {
                    while (!cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        await Task.Delay(TimeSpan.FromHours(1));

                        await Transactions();
                    }
                }, cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                glb_Func.WriteLog(ex.Message, EventLogEntryType.Error);
            }
        }

        #endregion

        #region Stop

        protected override void OnStop()
        {
            Glb_Func glb_Func = new Glb_Func();

            try
            {
                glb_Func.WriteLog("Servis durduruldu.", EventLogEntryType.Information);
                cancellationTokenSource.Cancel(); 
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
            Glb_Func glb_Func = new Glb_Func();
            e_Document_Func e_Document_Func = new e_Document_Func();
            e_Ledger_Func e_Ledger_Func = new e_Ledger_Func();
            paymentRecorder paymentRecorder = new paymentRecorder();
            e_LedgerDeclaration_Func e_LedgerDeclaration_Func = new e_LedgerDeclaration_Func();

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
