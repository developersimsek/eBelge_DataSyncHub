#region Using

using eBelge_DataSyncHub.Func;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using static eBelge_DataSyncHub.Model.Settings.Project_Setting;

#endregion

namespace eBelge_DataSyncHub.Service
{
    public class OneSignal
    {
        #region Veriable

        Glb_Func glbFunc;
        Setting projectSetting;

        #endregion

        #region Init

        public OneSignal()
        {
            projectSetting = new Setting();
            glbFunc = new Glb_Func();

            try
            {
                projectSetting = glbFunc.getSetting();
            }
            catch (Exception ex)
            {
                throw new Exception("oneSignal_Func İşlemler sırasında bir hata oluştu." + ex.Message);
            }
        }

        #endregion

        #region Send

        /// <summary>
        /// OneSignal servisi kullanarak bildirim gönderir.
        /// </summary>
        /// <param name="message">Bildirimin içeriği.</param>
        public async Task SendNotification(string message)
        {
            HttpClient httpClient = new HttpClient();
            string content;

            try
            {
                if (!glbFunc.checkInternetConnection())
                    throw new Exception("sendNotification İnternet bağlantısı bulunamadı.");

                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Basic " + projectSetting.Settings.oneSignal.apiKey);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                content = "{\"app_id\": \"" + projectSetting.Settings.oneSignal.appId + "\", \"contents\": {\"tr\": \"" + message + "\"}, \"included_segments\": [\"All\"]}";

                await httpClient.PostAsync(projectSetting.Settings.oneSignal.uri, new StringContent(content));

            }
            catch (Exception ex)
            {
                throw new Exception("oneSignal_Func sendNotification İşlemler sırasında bir hata oluştu." + ex.Message);
            }
            finally
            {
                httpClient.Dispose();
            }
        }

        #endregion
    }
}