#region Using

using eBelge_DataSyncHub.Func;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static eBelge_DataSyncHub.Model.Settings.Project_Setting;

#endregion

namespace eBelge_DataSyncHub.Service
{
    public class Firebase
    {
        #region Veriable

        FirebaseClient firebaseClient;
        Glb_Func glbFunc;
        Setting projectSetting;

        #endregion

        #region Init

        public Firebase()
        {
            projectSetting = new Setting();
            glbFunc = new Glb_Func();

            try
            {
                projectSetting = glbFunc.getSetting();
                firebaseClient = new FirebaseClient(projectSetting.Settings.fireBaseUri);
            }
            catch (Exception ex)
            {
                throw new Exception("oneSignal_Func İşlemler sırasında bir hata oluştu." + ex.Message);
            }
        }

        #endregion

        #region Get

        /// <summary>
        /// Belirtilen Firebase veritabanı path'inden veri çeker ve generic olarak belirtilen tipe dönüştürür.
        /// </summary>
        /// <typeparam name="T">Döndürülecek verinin türü.</typeparam>
        /// <param name="dbChild">Veri çekilecek Firebase veritabanı path'i</param>
        /// <returns>
        /// Belirtilen path'den çekilen ve generic olarak belirtilen tipe dönüştürülen veriyi döndürür.
        /// </returns>
        public async Task<List<T>> GetFirebaseData<T>(string dbChild) where T : class
        {
            try
            {
                if (!glbFunc.checkInternetConnection())
                    throw new Exception("GetFirebaseData İnternet bağlantısı bulunamadı.");

                var result = await firebaseClient.Child(dbChild).OnceAsync<T>();

                return result.Select(d => d.Object).ToList();
            }
            catch (FirebaseException ex)
            {
                throw new Exception("GetFirebaseData Firebase işlemleri sırasında bir sorun oluştu. " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("GetFirebaseData İşlem sırasında bir sorun oluştu. " + ex.Message);
            }
        }

        #endregion

        #region Save

        /// <summary>
        /// Firebase veritabanına belirtilen yol altına belirtilen verileri kaydeder.
        /// </summary>
        /// <typeparam name="T">Kaydedilecek verilerin türü.</typeparam>
        /// <param name="dbChild">Firebase veritabanındaki path.</param>
        /// <param name="data">Kaydedilecek veri koleksiyonu.</param>
        /// <returns>Task</returns>
        /// 
        public async Task SaveFirebaseData<T>(string dbChild, Dictionary<string, T> data)
        {
            try
            {
                if (!glbFunc.checkInternetConnection())
                    throw new Exception("SaveFirebaseData İnternet bağlantısı bulunamadı.");

                await firebaseClient.Child(dbChild).PutAsync(data);
            }
            catch (FirebaseException ex)
            {
                throw new Exception("SaveFirebaseData Firebase işlemleri sırasında bir sorun oluştu. " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("SaveFirebaseData İşlem sırasında bir sorun oluştu. " + ex.Message);
            }
        }

        #endregion

    }
}
