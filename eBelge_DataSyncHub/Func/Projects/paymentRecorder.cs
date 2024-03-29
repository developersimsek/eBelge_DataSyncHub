﻿#region Using

using eBelge_DataSyncHub.Model.Projects;
using eBelge_DataSyncHub.Model.Settings.Projects;
using eBelge_DataSyncHub.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static eBelge_DataSyncHub.Definitions.Glb_Enum;
using static eBelge_DataSyncHub.Func.StringExtensions;

#endregion

namespace eBelge_DataSyncHub.Func
{
    public class paymentRecorder
    {

        #region Veriable

        Glb_Func glb_Func;
        HtmlValueListsOut htmlValueListsOut;
        OneSignal oneSignal_Func;
        Service.Firebase firebase_Func;
        SettingProperty settingLedger;

        List<FirebaseProperty> currentList;
        Dictionary<string, FirebaseProperty> saveDataList;

        #endregion

        #region Init

        public paymentRecorder()
        {
            glb_Func = new Glb_Func();
            htmlValueListsOut = new HtmlValueListsOut();
            oneSignal_Func = new OneSignal();
            firebase_Func = new Service.Firebase();
            currentList = new List<FirebaseProperty>();
            saveDataList = new Dictionary<string, FirebaseProperty>();

            settingLedger = glb_Func.getProjectProperty(ProjectsEnum.paymentRecorder);
        }

        #endregion

        #region Get

        public async Task getUpragateList()
        {
            try
            {
                htmlValueListsOut = await glb_Func.getHtmlValueList(ProjectsEnum.paymentRecorder);
            }
            catch (Exception ex)
            {

                throw new Exception("paymentRecorder getUpragateList İşlemler sırasında bir hata oluştu." + ex.Message);
            }
        }

        public async Task getCurrentList()
        {
            try
            {
                currentList = (await firebase_Func.GetFirebaseData<FirebaseProperty>(settingLedger.FirebasePath)).OrderByDescending(d => DateTime.ParseExact(d.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture)).ToList();
            }
            catch (Exception ex)
            {

                throw new Exception("paymentRecorder getCurrentList İşlemler sırasında bir hata oluştu." + ex.Message);
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
                    throw new Exception("paymentRecorder saveAndPushNotification İnternet bağlantısı bulunamadı.");

                newRecord = await getListAndControl();

                if (newRecord)
                {
                    await firebase_Func.SaveFirebaseData(settingLedger.FirebasePath, saveDataList);
                    await oneSignal_Func.SendNotification(htmlValueListsOut.ContextList[0]);

                    clearList();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("paymentRecorder saveAndPushNotification İşlemler sırasında bir hata oluştu." + ex.Message);
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

                if (currentList.Count <= 0 || (htmlValueListsOut.ContextList.Count > 0 && currentList[0].Aciklama != htmlValueListsOut.ContextList[0]))
                {
                    glb_Func.WriteLog("PaymentRecorder getListAndControl işleminde yeni duyuru bulundu. Firebase'e kaydedilip bildirim gönderilecek.", EventLogEntryType.Information);

                    for (int i = 0; i < 20; i++)
                    {
                        FirebaseProperty property = new FirebaseProperty
                        {
                            Aciklama = !string.IsNullOrEmpty(htmlValueListsOut.ContextList[i]) ? htmlValueListsOut.ContextList[i] : "",
                            Date = !string.IsNullOrEmpty(htmlValueListsOut.TitleList[i]) ? htmlValueListsOut.TitleList[i] : "",
                            Link = !string.IsNullOrEmpty(htmlValueListsOut.HreftList[i]) ? htmlValueListsOut.HreftList[i] : ""
                        };

                        saveDataList.Add(Ext_String.GenerateStringKey(), property);
                    }

                    return true;
                }
                else
                {
                    glb_Func.WriteLog("PaymentRecorder getListAndControl işleminde yeni duyuru bulunamadı.", EventLogEntryType.Information);

                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("paymentRecorder getListAndControl İşlemler sırasında bir hata oluştu." + ex.Message);
            }
        }

        /// <summary>
        /// Tüm listeleri temizler: currentList, saveDataList, htmlValueListsOut.HreftList, htmlValueListsOut.TitleList, htmlValueListsOut.HreftList.
        /// </summary>
        public void clearList()
        {
            currentList.Clear();
            saveDataList.Clear();
            htmlValueListsOut.HreftList.Clear();
            htmlValueListsOut.TitleList.Clear();
            htmlValueListsOut.HreftList.Clear();
        }

        #endregion
    }
}