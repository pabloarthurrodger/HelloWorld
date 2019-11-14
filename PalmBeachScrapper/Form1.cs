using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PalmBeachScrapper
{
    public partial class MainForm : Form
    {
        private readonly string County = "PALM BEACH";
        private DateTime BuscaFechaGlobal = Convert.ToDateTime("05/19/2008");
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnGO_Click(object sender, EventArgs e)
        {
            MainBrowser.Navigate(new Uri(tbURI.Text));
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            MainBrowser.GoBack();
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            MainBrowser.GoForward();
        }

        private void MainBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // Determinar en Que pagina Estoy.
            // Tomar Desicion // CASE
            // Darle a Pagina Anterior.
            if (cbGetCases.Checked)
            {
                int Activity = 0;

                var WPDoc = new HtmlAgilityPack.HtmlDocument();
                var doc = ((MainForm)Application.OpenForms[0]).MainBrowser.Document;
                var renderedHtml = doc.GetElementsByTagName("HTML")[0].OuterHtml;
                WPDoc.LoadHtml(renderedHtml);

                var SearchBtn = doc.GetElementById("ctl00$cphBody$cmdSearch");
                var CancelBtn = doc.GetElementById("ctl00$cphBody$cmdClear");
                var ViewElements = doc.GetElementById("cphBody_cmbPageSize");

                if (SearchBtn != null && CancelBtn != null)
                {
                    Activity = 1;
                }

                if (ViewElements != null)
                {
                    if (ViewElements.OuterHtml.IndexOf("selected=\"selected\" value=\"All\">") > 0)
                        Activity = 3;
                    else
                        Activity = 2;
                }

                if (renderedHtml.Contains("Modify your search criteria by selecting the Search Criteria tab"))
                {
                    Activity = 4;
                }

                switch (Activity)
                {
                    case 1:
                        {
                            var BeginDate = doc.GetElementById("cphBody_gvSearch_txtParameter_7");
                            var EndDate = doc.GetElementById("cphBody_gvSearch_txtParameter_8");
                            var CourtType = doc.GetElementById("cphBody_gvSearch_cmbParameterPostBack_9");
                            var SearchDate = GetMeDate();
                            BeginDate.SetAttribute("Value", SearchDate.ToString("MM/dd/yyyy"));
                            EndDate.SetAttribute("Value", SearchDate.ToString("MM/dd/yyyy"));
                            CourtType.SetAttribute("Value", "101");
                            SearchBtn.InvokeMember("Click");
                            break;
                        }

                    case 2:
                        {
                            ViewElements.SetAttribute("Value", "All");
                            ViewElements.InvokeMember("onChange");
                            // MainBrowser.Refresh(WebBrowserRefreshOption.Completely); 
                            break;
                        }

                    case 3:
                        {
                            PalmCaseInfoClass CInfo = new PalmCaseInfoClass();
                            CInfo.County = County;
                            // Tomar Casos en el Dia;
                            var CasosDiaStr = doc.GetElementById("cphBody_lblRecordsReturned");
                            if (CasosDiaStr != null)
                            {
                                CInfo.NumberOfCases = Convert.ToInt32(CasosDiaStr.InnerText);
                            }

                            var TablaCasos = WPDoc.GetElementbyId("cphBody_gvResults");
                            var CasesInfoRow = TablaCasos.Descendants("tr").ToList();
                            foreach (var Crow in CasesInfoRow)
                            {
                                if (!Crow.InnerText.Contains("Arrest Date"))
                                {
                                    var CasesInfoColumn = Crow.Descendants("td").ToList();
                                    int Num = 0;
                                    string[] AColInfo = new string[7];
                                    foreach (var Ccol in CasesInfoColumn)
                                    {
                                        AColInfo[Num] = Ccol.InnerText;
                                        Num++;
                                    }
                                    pbICases WC = new pbICases();
                                    WC.CaseNumber = AColInfo[0];
                                    WC.Courts = AColInfo[1];
                                    WC.CaseType = AColInfo[2];
                                    try
                                    {
                                        WC.FileDate = Convert.ToDateTime(AColInfo[4]);
                                    }
                                    catch
                                    {
                                        WC.FileDate = Convert.ToDateTime("2008-01-01");
                                    }

                                    if (!string.IsNullOrEmpty(AColInfo[5]))
                                    {
                                        if (AColInfo[5].IndexOf(" V ") > 0)
                                        {
                                            WC.PrimaryParty = AColInfo[5].Substring(0, AColInfo[5].IndexOf(" V "));
                                            WC.SecondaryParty = AColInfo[5].Substring(AColInfo[5].IndexOf(" V ") + 3);
                                        }
                                        else
                                        {
                                            WC.PrimaryParty = AColInfo[5];
                                            WC.SecondaryParty = AColInfo[5];
                                        }

                                    }

                                    WC.CaseStatus = AColInfo[6];

                                    CInfo.ListaCasos.Add(WC);
                                    CInfo.FileDate = WC.FileDate;
                                }
                            }

                            // Salvar en Async y Threadding.
                            SaveCases(CInfo);
                            // Slow the speed
                            Thread.Sleep(10000);


                            // cphBody_lbSearch  // Volver a la pantalla de Busqueda
                            //var SearchLink = doc.GetElementById("ctl00$cphBody$cmdSearch");
                            //SearchBtn.InvokeMember("Click");
                            MainBrowser.Navigate("https://applications.mypalmbeachclerk.com/eCaseView/search.aspx"); 
                            break;
                        }

                    case 4:
                        {
                            MainBrowser.Navigate("https://applications.mypalmbeachclerk.com/eCaseView/search.aspx");
                            break;
                        }
                }
            }
        }


        private DateTime GetMeDate()
        {
            DateTime BuscaFecha = new DateTime(); 
            using (CaseEntities Ctx = new CaseEntities())
            {
                //var UltimaFecha = Ctx.MAINCASES
                //            .Where(x => x.COUNTY == County && x.FILEDATE.Year == nupYear.Value)
                //            .OrderByDescending(o => o.FILEDATE)
                //            .Select(s => s.FILEDATE).FirstOrDefault();

                BuscaFecha = BuscaFechaGlobal.AddDays(1); 

                if (BuscaFecha < Convert.ToDateTime("12/31/2007"))
                {
                    DiasFeriados(County, Convert.ToDateTime("01/01/" + nupYear.Value.ToString()));
                    BuscaFecha =   Convert.ToDateTime("01/02/" + nupYear.Value.ToString()); 
                }

                switch (BuscaFecha.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        {
                            BuscaFecha = BuscaFecha.AddDays(1);
                            break;
                        }

                    case DayOfWeek.Saturday:
                        {
                            BuscaFecha = BuscaFecha.AddDays(2); 
                            break;
                        }
                }
            }
            BuscaFechaGlobal = BuscaFecha; 
            return BuscaFecha;
        }

        private void SaveCases(PalmCaseInfoClass InfoCases)
        {
            using (CaseEntities Ctx = new CaseEntities())
            {
                foreach (var WrkCase in InfoCases.ListaCasos)
                {
                    // Salvar Info en MainCases
                    MAINCAS Mc = new MAINCAS();
                    Mc.ID = Guid.NewGuid();
                    Mc.COUNTY = County;
                    Mc.CASENUMBER = CleanString(WrkCase.CaseNumber);
                    Mc.CASETYPE = WrkCase.CaseType;
                    Mc.CASESTATUS = WrkCase.CaseStatus;
                    Mc.PRIMARY_PARTY = WrkCase.PrimaryParty;
                    Mc.SECONDARY_PARTY = WrkCase.SecondaryParty;
                    Mc.FILEDATE = WrkCase.FileDate;
                    Mc.NOTES = WrkCase.Courts;

                    Ctx.MAINCASES.Add(Mc);
                }

                // Salvar Cuantos Records por fecha.
                RecordsXdia Rdia = new RecordsXdia();
                Rdia.COUNTY = InfoCases.County;
                Rdia.DATE = InfoCases.FileDate;
                Rdia.CASOS = InfoCases.NumberOfCases;
                Ctx.RecordsXdias.Add(Rdia);

                // Save Changes.
                var x = Ctx.SaveChanges(); 
            }

        }

        private void DiasFeriados(string County, DateTime fecha )
        {
            using (CaseEntities Ctx = new CaseEntities())
            {
                // Salvar Cuantos Records por fecha.
                RecordsXdia Rdia = new RecordsXdia();
                Rdia.COUNTY = County;
                Rdia.DATE = fecha;
                Rdia.CASOS = 0;
                Ctx.RecordsXdias.Add(Rdia);

                // Save Changes.
                var x = Ctx.SaveChanges();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //var UltimaFecha = Ctx.MAINCASES.Where(y => y.COUNTY == County && y.FILEDATE.Year == nupYear.Value)
            //    .OrderByDescending(o => o.FILEDATE)
            //    .Select(s => s.FILEDATE).FirstOrDefault();
            DateTime a = GetMeDate(); 
            DiasFeriados(County, Convert.ToDateTime("01/01/2008"));
        }

        private string CleanString (string DirtyStr)
        {
            string cleanString = DirtyStr;
            cleanString = cleanString.Replace("\n", "");
            cleanString = cleanString.Replace("\r", "");
            cleanString = cleanString.Replace("\t", "");
            cleanString = cleanString.Replace("&amp;", "");
            cleanString = cleanString.Replace("#", "No.");
            cleanString = cleanString.Replace(" ", "");
            return cleanString.Trim();
        }

    }
}
