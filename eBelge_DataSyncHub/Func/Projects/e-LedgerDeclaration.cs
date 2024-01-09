#region Using

using eBelge_DataSyncHub.Model.Projects;
using eBelge_DataSyncHub.Model.Settings.Projects;
using eBelge_DataSyncHub.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static eBelge_DataSyncHub.Definitions.Glb_Enum;
using static eBelge_DataSyncHub.Func.StringExtensions;
using static eBelge_DataSyncHub.Model.Projects.ELedgerDeclaration;

#endregion

namespace eBelge_DataSyncHub.Func
{
    public class e_LedgerDeclaration_Func
    {
        #region Veriable

        Glb_Func glb_Func;
        OneSignal oneSignal_Func;
        Service.Firebase firebase_Func;
        SettingProperty settingELedgerDeclaration;
        List<ELedgerDeclarationItems> eLedgerDeclarationItems;
        List<FirebaseProperty> currentList;
        Dictionary<string, FirebaseProperty> saveDataList;

        #endregion

        #region Init

        public e_LedgerDeclaration_Func()
        {
            glb_Func = new Glb_Func();
            oneSignal_Func = new OneSignal();
            firebase_Func = new Service.Firebase();
            currentList = new List<FirebaseProperty>();
            saveDataList = new Dictionary<string, FirebaseProperty>();
            eLedgerDeclarationItems = new List<ELedgerDeclarationItems>();
            settingELedgerDeclaration = glb_Func.getProjectProperty(ProjectsEnum.eLedgerDeclaration);
        }

        #endregion

        #region Get

        public async Task getUpragateList()
        {
            try
            {
                HttpResponseMessage responseMessage = new HttpResponseMessage();
                string requestUri = settingELedgerDeclaration.Uri;
                string responseBody;

                for (int i = 1; i < 4; i++)
                {
                    requestUri += "&__RequestVerificationToken=" + Ext_String.GenerateStringKey(108) + "&page=" + i.ToString();

                    responseMessage = await glb_Func.executeHttpRequest(requestUri);
                    responseBody = await responseMessage.Content.ReadAsStringAsync();

                    ELedgerDeclaration eLedgerDeclaration = JsonConvert.DeserializeObject<ELedgerDeclaration>(responseBody);

                    eLedgerDeclarationItems.AddRange(eLedgerDeclaration.items);

                }

            }
            catch (Exception ex)
            {

                throw new Exception("eLedgerDeclaration getUpragateList İşlemler sırasında bir hata oluştu." + ex.Message);
            }
        }

        public async Task getCurrentList()
        {
            try
            {
                currentList = (await firebase_Func.GetFirebaseData<FirebaseProperty>(settingELedgerDeclaration.FirebasePath)).OrderByDescending(d => DateTime.ParseExact(d.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture)).ToList();
            }
            catch (Exception ex)
            {

                throw new Exception("eLedgerDeclaration getCurrentList İşlemler sırasında bir hata oluştu." + ex.Message);
            }
        }

        #endregion

        #region Save

        public async Task saveAndPushNotification()
        {
            try
            {
                bool newRecord = false;

                if (!glb_Func.checkInternetConnection())
                    throw new Exception("eLedgerDeclaration saveAndPushNotification İnternet bağlantısı bulunamadı.");

                newRecord = await getListAndControl();

                if (newRecord)
                {
                    await firebase_Func.SaveFirebaseData(settingELedgerDeclaration.FirebasePath, saveDataList);
                    await oneSignal_Func.SendNotification(eLedgerDeclarationItems[0].description);

                    clearList();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("eLedgerDeclaration saveAndPushNotification İşlemler sırasında bir hata oluştu." + ex.Message);
            }
        }

        #endregion

        #region Func

        /// <summary>
        /// Güncel veriler ile kayıtlı verileri karşılaştırır. Farklı kayıt var ise saveDataList oluşturur.
        /// </summary>
        /// <returns>
        /// Farklı kayıt var ise <c>true</c>, aksi takdirde <c>false</c>.
        /// </returns>
        public async Task<bool> getListAndControl()
        {
            try
            {
                await getUpragateList();
                await getCurrentList();

                if (currentList.Count <= 0 || (eLedgerDeclarationItems.Count > 0 && currentList[0].Aciklama != eLedgerDeclarationItems[0].description))
                {
                    glb_Func.WriteLog("eLedgerDeclaration getListAndControl işleminde yeni duyuru bulundu. Firebase'e kaydedilip bildirim gönderilecek.", EventLogEntryType.Information);

                    for (int i = 0; i < 20; i++)
                    {
                        FirebaseProperty property = new FirebaseProperty
                        {
                            Aciklama = eLedgerDeclarationItems[i].description,
                            Date = eLedgerDeclarationItems[i].date.ConvertDateFormat(),
                            Link = ""
                        };

                        saveDataList.Add(Ext_String.GenerateStringKey(), property);
                    }

                    return true;
                }
                else
                {
                    glb_Func.WriteLog("eLedgerDeclaration getListAndControl işleminde yeni duyuru bulunamadı.", EventLogEntryType.Information);

                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("eLedgerDeclaration getListAndControl İşlemler sırasında bir hata oluştu." + ex.Message);
            }
        }

        /// <summary>
        /// Tüm listeleri temizler: currentList, saveDataList, htmlValueListsOut.HreftList, htmlValueListsOut.TitleList, htmlValueListsOut.HreftList.
        /// </summary>
        public void clearList()
        {
            currentList.Clear();
            saveDataList.Clear();
            eLedgerDeclarationItems.Clear();
        }

        #endregion
    }
}