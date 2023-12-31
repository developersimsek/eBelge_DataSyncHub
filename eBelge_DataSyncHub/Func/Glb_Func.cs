#region Using

using eBelge_DataSyncHub.Model.Projects;
using eBelge_DataSyncHub.Model.Settings.Projects;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using static eBelge_DataSyncHub.Definitions.Glb_Enum;
using static eBelge_DataSyncHub.Model.Settings.Project_Setting;

#endregion

namespace eBelge_DataSyncHub.Func
{
    public class Glb_Func
    {
        #region Load

        #region Html

        /// <summary>
        /// HTTP yanıtından HTML belgesini yükler.
        /// </summary>
        /// <param name="responseMessage">HTTP yanıtı</param>
        /// <returns>Yüklenen HTML belgesi</returns>
        public async Task<HtmlDocument> loadHtml(HttpResponseMessage responseMessage)
        {
            string content;
            HtmlDocument htmlDoc = new HtmlDocument();

            try
            {
                content = await responseMessage.Content.ReadAsStringAsync();
                htmlDoc.LoadHtml(content);
            }
            catch (Exception ex)
            {
                throw new Exception("loadHtml işlemler sırasında bir sorun oluştu. " + ex.Message);
            }
            return htmlDoc;
        }

        #endregion

        #endregion

        #region Get

        #region Html

        /// <summary>
        /// Belirtilen element seçicisine göre HTML öğelerini döndürür.
        /// </summary>
        /// <param name="responseMessage">HTTP yanıtı</param>
        /// <param name="elementSelector">Element seçici</param>
        /// <returns>HTML düğüm koleksiyonu</returns>
        public async Task<HtmlNodeCollection> getHtmlNodes(HttpResponseMessage responseMessage, string elementSelector)
        {
            string content;
            HtmlDocument htmlDoc = new HtmlDocument();
            HtmlNodeCollection htmlNodesCol;

            try
            {
                htmlDoc = await loadHtml(responseMessage);

                htmlNodesCol = htmlDoc.DocumentNode.SelectNodes(elementSelector);
            }
            catch (Exception ex)
            {
                throw new Exception("getHtmlNodes işlemler sırasında bir sorun oluştu. " + ex.Message);
            }
            return htmlNodesCol;
        }

        /// <summary>
        /// Belirtilen iki farklı element seçicisine göre HTML öğeleri bulunup her birini ayrı ayrı döndürülür.
        /// </summary>
        /// <param name="responseMessage">HTTP yanıtı</param>
        /// <param name="element1Selector">İlk element seçici</param>
        /// <param name="element2Selector">İkinci element seçici</param>
        /// <returns>HTML düğüm koleksiyonları listesi</returns>
        public async Task<List<HtmlNodeCollection>> getHtmlNodes(HttpResponseMessage responseMessage, string element1Selector, string element2Selector)
        {
            List<HtmlNodeCollection> htmlNodesList = new List<HtmlNodeCollection>();

            try
            {
                htmlNodesList.Add(await getHtmlNodes(responseMessage, element1Selector));
                htmlNodesList.Add(await getHtmlNodes(responseMessage, element2Selector));
            }
            catch (Exception ex)
            {
                throw new Exception("getHtmlNodes işlemler sırasında bir sorun oluştu. " + ex.Message);
            }
            return htmlNodesList;
        }

        /// <summary>
        /// Verilen HTML öğe koleksiyonundan her bir öğenin içeriğini alarak bir dize listesi oluşturur.
        /// </summary>
        /// <param name="htmlNodes">HTML öğe koleksiyonu</param>
        /// <returns>Öğe içeriklerinden oluşan dize listesi</returns>
        public List<string> getHtmlNodeValues(HtmlNodeCollection htmlNodes, bool isHrefNodes = false, bool isDateNodes = false, bool isNotSearchContent = false)
        {
            List<string> elementValueList = new List<string>();
            string valueToAdd;

            try
            {
                foreach (HtmlNode node in htmlNodes)
                {
                    valueToAdd = isHrefNodes ? getHrefValue(node, isNotSearchContent) :
                        isDateNodes ? node.InnerText.ConvertDateFormat() :
                        node.InnerText.RemoveExtraSpaces().CapitalizeSentences();

                    elementValueList.Add(valueToAdd);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("getHtmlNodeValues işlemi sırasında bir sorun oluştu. " + ex.Message);
            }

            return elementValueList;
        }

        /// <summary>
        /// Belirtilen HTML öğe koleksiyonunun içerdiği bağlantı değerini döndürür, "Tıklayınız" metnini içeren düğümdeki bağlantıyı arar.
        /// </summary>
        /// <param name="node">HTML öğesi</param>
        /// <returns>Bağlantı değeri veya boş dize</returns>
        private string getHrefValue(HtmlNode node, bool isNotSearchContent)
        {
            HtmlNode targetNode;
            HtmlNode hrefElement;

            if (isNotSearchContent || node.InnerText.Contains("Tıklayınız") || node.InnerText.Contains("tıklayınız"))
            {
                targetNode = node.SelectSingleNode(".//b | .//p") ?? node;
                hrefElement = targetNode.SelectSingleNode(".//a[@href]");

                return hrefElement != null ? hrefElement?.Attributes["href"]?.Value : targetNode != null ? targetNode?.Attributes["href"]?.Value : "";
            }

            return "";
        }

        /// <summary>
        /// Verilen Projeye için istek atıp TitleList ve ContextList oluşturur.
        /// </summary>
        /// <param name="projects">Proje Enumu</param>
        /// <returns>HtmlValueListsOut döner. TitleList ve ContextList listelerini içerir.</returns>
        public async Task<HtmlValueListsOut> getHtmlValueList(ProjectsEnum projects)
        {
            HtmlValueListsOut htmlValueListsOut = new HtmlValueListsOut();
            SettingProperty projectProperty = new SettingProperty();
            HttpResponseMessage httpResponseMessage;
            List<HtmlNodeCollection> htmlNodesList;
            bool isNotSearchContent = false;
            try
            {
                projectProperty = getProjectProperty(projects);

                if (projects == ProjectsEnum.paymentRecorder)
                    isNotSearchContent = true;

                httpResponseMessage = await executeHttpRequest(projectProperty.Uri);
                htmlNodesList = await getHtmlNodes(httpResponseMessage, projectProperty.TitlePath, projectProperty.ContextPath);

                htmlValueListsOut.TitleList = getHtmlNodeValues(htmlNodesList[0], isDateNodes: true);
                htmlValueListsOut.ContextList = getHtmlNodeValues(htmlNodesList[1]);
                htmlValueListsOut.HreftList = getHtmlNodeValues(htmlNodesList[1], isHrefNodes: true, isNotSearchContent: isNotSearchContent);

            }
            catch (Exception ex)
            {
                throw new Exception("getHtmlValueList Bir hata oluştu. " + ex.Message);
            }

            return htmlValueListsOut;
        }

        #endregion

        #region Setting

        /// <summary>
        /// Proje ayarlarını döndürür.
        /// </summary>
        /// <returns>Ayarlar</returns>
        public Setting getSetting()
        {
            string jsonRead, settigsJsonPath = "settings.json";
            Setting project_Setting = new Setting();

            try
            {
                jsonRead = File.ReadAllText(settigsJsonPath);

                project_Setting = JsonConvert.DeserializeObject<Setting>(jsonRead);

            }
            catch (FileNotFoundException ex)
            {
                throw new Exception("getSetting Dosya bulunamadı. " + ex.Message);
            }
            catch (JsonException ex)
            {
                throw new Exception("getSetting JSON verisi geçersiz. " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("getSetting Bir hata oluştu. " + ex.Message);
            }

            return project_Setting;
        }

        /// <summary>
        /// Verilen projeye göre proje özelliklerini doldurur ve döner.
        /// </summary>
        /// <param name="projects">Proje Enumu</param>
        /// <returns>Proje Property</returns>
        public SettingProperty getProjectProperty(ProjectsEnum projects)
        {
            Setting project_Setting = new Setting();
            SettingProperty property = new SettingProperty();
            try
            {
                project_Setting = getSetting();

                switch (projects)
                {
                    case ProjectsEnum.eDocument:
                        property = new SettingProperty
                        {
                            Uri = project_Setting.Settings.Projets.eDocument.Uri,
                            ContextPath = project_Setting.Settings.Projets.eDocument.ContextPath,
                            TitlePath = project_Setting.Settings.Projets.eDocument.TitlePath,
                            FirebasePath = project_Setting.Settings.Projets.eDocument.FirebasePath,
                        };
                        break;
                    case ProjectsEnum.eLedger:
                        property = new SettingProperty
                        {
                            Uri = project_Setting.Settings.Projets.eLedger.Uri,
                            ContextPath = project_Setting.Settings.Projets.eLedger.ContextPath,
                            TitlePath = project_Setting.Settings.Projets.eLedger.TitlePath,
                            FirebasePath = project_Setting.Settings.Projets.eLedger.FirebasePath,
                        };
                        break;
                    case ProjectsEnum.eLedgerDeclaration:
                        property = new SettingProperty
                        {
                            Uri = project_Setting.Settings.Projets.eLedgerDeclaration.Uri,
                            ContextPath = project_Setting.Settings.Projets.eLedgerDeclaration.ContextPath,
                            TitlePath = project_Setting.Settings.Projets.eLedgerDeclaration.TitlePath,
                            FirebasePath = project_Setting.Settings.Projets.eLedgerDeclaration.FirebasePath,
                        };
                        break;
                    case ProjectsEnum.paymentRecorder:
                        property = new SettingProperty
                        {
                            Uri = project_Setting.Settings.Projets.paymentRecorder.Uri,
                            ContextPath = project_Setting.Settings.Projets.paymentRecorder.ContextPath,
                            TitlePath = project_Setting.Settings.Projets.paymentRecorder.TitlePath,
                            FirebasePath = project_Setting.Settings.Projets.paymentRecorder.FirebasePath,
                        };
                        break;
                }

            }

            catch (Exception ex)
            {
                throw new Exception("getProjectProperty Bir hata oluştu. " + ex.Message);
            }

            return property;
        }

        #endregion

        #endregion

        #region Execute

        #region Http

        /// <summary>
        /// Belirtilen URI'ye HTTP isteği gönderir.
        /// </summary>
        /// <param name="projectUri">İstek gönderilecek URI</param>
        /// <returns>HTTP yanıtı</returns>
        public async Task<HttpResponseMessage> executeHttpRequest(string projectUri)
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage responseMessage = new HttpResponseMessage();

            try
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");

                if (!checkInternetConnection())
                {
                    throw new Exception("sendHttpRequest İnternet bağlantısı bulunamadı.");
                }

                responseMessage = await httpClient.GetAsync(projectUri);

                if (!responseMessage.IsSuccessStatusCode)
                {
                    throw new Exception("sendHttpRequest İstek başarısız: " + projectUri);
                }

            }
            catch (Exception ex)
            {
                throw new Exception("sendHttpRequest " + projectUri + " e istek sırasında bir sorun oluştu. " + ex.Message);
            }
            finally
            {
                httpClient.Dispose();
            }

            return responseMessage;

        }

        /// <summary>
        /// Bilinen bir IP adresine (8.8.8.8 - Google'ın DNS sunucusu) bir ping isteği göndererek internet bağlantısının durumunu kontrol eder.
        /// </summary>
        /// <returns>
        /// Başarılı bir ping yanıtı alınırsa (IPStatus.Success), etkin bir internet bağlantısı olduğunu gösterir. 
        /// Ping isteği başarısız olursa veya istek sırasında bir hata oluşursa, internet bağlantısı olmadığını belirtir.
        /// </returns>
        public bool checkInternetConnection()
        {
            try
            {
                using (var ping = new Ping())
                {
                    PingReply reply = ping.Send("8.8.8.8");

                    if (reply != null)
                    {
                        return reply.Status == IPStatus.Success;
                    }
                }
            }
            catch (PingException)
            {
                return false;
            }

            return false;
        }

        #endregion

        #endregion

        #region Log

        /// <summary>
        /// Belirtilen mesajı ve olay türünü olay günlüğüne yazar.
        /// </summary>
        /// <param name="logMessage">Olay günlüğüne yazılacak mesaj.</param>
        /// <param name="type">Olayın türü (Information, Error, Warning vb.).</param>
        public void WriteLog(string logMessage, EventLogEntryType type)
        {
            EventLog eventLog = new EventLog();

            try
            {
                eventLog.Source = "DataSyncServices";
                eventLog.WriteEntry(logMessage, type);
            }
            catch (Exception)
            {
                throw;
            }

        }

        #endregion

    }
}