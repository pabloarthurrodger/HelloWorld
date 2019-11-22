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
        private DateTime BuscaFechaGlobal = Convert.ToDateTime("05/11/2010");
        private DateTime StartDay = Convert.ToDateTime("06/01/2010");
        private DateTime EndDay = Convert.ToDateTime("06/01/2010");
        private int AdicionarDias = 6;

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnGO_Click(object sender, EventArgs e)
        {
            MainBrowser.Navigate(new Uri(tbURI.Text));
            logclick("Navigate To URI");
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            MainBrowser.GoBack();
            logclick("Go Back");
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            MainBrowser.GoForward();
            logclick("Go Forward");
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
                        //ctl00$cphBody$gvSearch$ctl09$txtParameter: 01 / 01 / 2013
                        //ctl00$cphBody$gvSearch$ctl10$txtParameter: 02 / 01 / 2013
                        //ctl00$cphBody$gvSearch$ctl11$cmbParameterPostBack: 101
                        //ctl00$cphBody$gvSearch$ctl12$cmbParameterNoPostBack: 146
                        //ctl00$cphBody$gvSearch$ctl13$cmbParameterNoPostBack: 96

                    case 1:
                        {
                            var BeginDate = doc.GetElementById("cphBody_gvSearch_txtParameter_7");
                            var EndDate = doc.GetElementById("cphBody_gvSearch_txtParameter_8");
                            var CourtType = doc.GetElementById("cphBody_gvSearch_cmbParameterPostBack_9");
                            CourtType.SetAttribute("Value", "101");
                            var CaseType = doc.GetElementById("cphBody_gvSearch_cmbParameterNoPostBack_10");
                            Thread.Sleep(2000);
                            // var SearchDate = GetMeDate();
                            SelectNewDayRange();
                            BeginDate.SetAttribute("Value", StartDay.ToString("MM/dd/yyyy"));
                            EndDate.SetAttribute("Value", EndDay.ToString("MM/dd/yyyy"));

                            CaseType.SetAttribute("Value", "146"); 

                            SearchBtn.InvokeMember("Click");
                            logclick("Search");
                            break;
                        }

                    case 2:
                        {
                            ViewElements.SetAttribute("Value", "All");
                            ViewElements.InvokeMember("onChange");
                            // MainBrowser.Refresh(WebBrowserRefreshOption.Completely); 
                            logclick("Search");
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
                                var wrkint = CInfo.NumberOfCases;
                                if (wrkint == 200)
                                {
                                    StartDay.AddDays(AdicionarDias * -1);
                                    StartDay.AddDays(-1);
                                    EndDay = StartDay;
                                    EndDay.AddDays(AdicionarDias);
                                    AdicionarDias = AdicionarDias - 3;
                                    MainBrowser.Navigate("https://applications.mypalmbeachclerk.com/eCaseView/search.aspx");
                                    logclick("Main Page");
                                    break;
                                }
                                else
                                {
                                    var casosxdia = wrkint / AdicionarDias;
                                    var amplitud = casosxdia * 3;
                                    var cabe = (200 - amplitud);
                                    if (wrkint > cabe)
                                    {
                                        AdicionarDias++;
                                    }
                                }
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
                            Thread.Sleep(24000);


                            // cphBody_lbSearch  // Volver a la pantalla de Busqueda
                            //var SearchLink = doc.GetElementById("ctl00$cphBody$cmdSearch");
                            //SearchBtn.InvokeMember("Click");
                            MainBrowser.Navigate("https://applications.mypalmbeachclerk.com/eCaseView/search.aspx");
                            logclick("Main Page");
                            break;
                        }

                    case 4:
                        {
                            MainBrowser.Navigate("https://applications.mypalmbeachclerk.com/eCaseView/search.aspx");
                            logclick("Main Page");
                            break;
                        }
                }
            }
        }
        private void SelectNewDayRange()
        {
            StartDay = EndDay.AddDays(1);
            EndDay = StartDay.AddDays(AdicionarDias); 

            if (EndDay > System.DateTime.Today)
            {
                cbGetCases.Checked = false;
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
                Rdia.DATE1 = StartDay;
                Rdia.DATE2 = EndDay;
                Rdia.CASOS = InfoCases.NumberOfCases;
                Ctx.RecordsXdias.Add(Rdia);

                // Save Changes.
                var x = Ctx.SaveChanges(); 
            }

        }

        private void DiasFeriados(string County, DateTime fecha )
        {
            SelectNewDayRange();
            using (CaseEntities Ctx = new CaseEntities())
            {
                // Salvar Cuantos Records por fecha.
                RecordsXdia Rdia = new RecordsXdia();
                Rdia.COUNTY = County;
                Rdia.DATE1 = StartDay;
                Rdia.DATE2 = EndDay;
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


        private void logclick(string page)
        {
            using (CaseEntities Ctx = new CaseEntities())
            {
                // Salvar Cuantos Records por fecha.
                ClickPageLog clog = new ClickPageLog();
                clog.COUNTY = County;
                clog.PAGE = page;
                clog.TIEMPO = System.DateTime.Now;
                Ctx.ClickPageLogs.Add(clog);

                // Save Changes.
                var x = Ctx.SaveChanges();
            }
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

//<select name = "ctl00$cphBody$gvSearch$ctl12$cmbParameterNoPostBack" id="cphBody_gvSearch_cmbParameterNoPostBack_10" disabled="disabled" class="aspNetDisabled form-control">
//					<option selected = "selected" value="39">All - Case Types</option>
//					<option value = "40" > All - Case Types</option>
//  				<option value = "41" > All - Case Types</option>
//  				<option value = "42" > All - Case Types</option>
//  				<option value = "43" > All - Case Types</option>
//  				<option value = "44" > All - Case Types</option>
//  				<option value = "45" > All - Case Types</option>
//  				<option value = "50" > All - Case Types</option>
//  				<option value = "118" > All - Case Types</option>
//  				<option value = "120" > All - Case Types</option>
//  				<option value = "204" > All - Case Types</option>
//  				<option value = "235" > All - Case Types</option>
//  				<option value = "194" > All - Case Types</option>
//  				<option value = "249" > All - Case Types</option>
//  				<option value = "253" > All - Case Types</option>
//  				<option value = "267" > All - Case Types</option>
//  				<option value = "288" > All - Case Types</option>
//  				<option value = "478" > All - Case Types</option>
//  				<option value = "16" > All - Court Types</option>
//  				<option value = "153" > 90 DAY EXT OF TIME-MEDICAL MAL</option>
//					<option value = "124" > ADMINISTRATIVE APPEAL</option>
//  				<option value = "466" > ANIMAL CRUELTY</option>
//  				<option value = "2" > ANIMAL INFRACTION</option>
//  				<option value = "464" > ANIMAL INFRACTION</option>
//  				<option value = "472" > ANNULMENT OF MARRIAGE</option>
//					<option value = "181" > ANTI - TRUST TRADE REGULATION</option>
//					<option value = "3" > APPEAL FROM COUNTY CRIMINAL</option>
//  				<option value = "1" > APPELLATE COURT</option>
//  				<option value = "195" > ARBITRATION / FOREIGN JDGMT (CC)</option>
//					<option value = "127" > ASBESTOS </ option >
  
//                  < option value= "121" > ASSN LIEN FORECLOSURE = &lt; $50K</option>
//					<option value = "123" > ASSN LIEN FORECLOSURE = &gt; $250K</option>
//					<option value = "122" > ASSN LN FORECLOSURE&gt;$50,&lt;$250K</option>
//					<option value = "125" > AUTO NEGLIGENCE</option>
//					<option value = "129" > BOND / AUTO / PERSONAL PROPERTY</option>
//					<option value = "171" > BUSINESS GOVERNANCE</option>
//					<option value = "128" > BUSINESS MALPRACTICE</option>
//					<option value = "172" > BUSINESS TORT</option>
//					<option value = "182" > BUSINESS TRANSACTION</option>
//					<option value = "254" > CAVEAT </ option >

//                  <option value="136">CERTIORARI</option>
//					<option value = "207" > CHILD SUPPORT IV-D</option>
//					<option value = "208" > CHILD SUPPORT NON IV-D</option>
//					<option value = "4" > CIGARETTE INFRACTION</option>
//					<option value = "197" > CIVIL(COUNTY CIVIL) </ option >

//                  <option value="196">CIVIL REPLEVIN(COUNTY CIVIL)</option>
//					<option value = "130" > COMM FORECLOSURE =&lt; $50K</option>
//					<option value = "132" > COMM FORECLOSURE =&gt; $250K</option>
//					<option value = "131" > COMM FORECLOSURE &gt;$50K, &lt;$250K</option>
//					<option value = "135" > CONDOMINIUM </ option >

//                  <option value="184">CONST CHALLENGE PROPOSED AMEND</option>
//					<option value = "183" > CONST CHALLENGE STATUTE/ORD</option>
// 					<option value = "175" > CONSTRUCTION DEFECT</option>
// 					<option value = "134" > CONSTRUCTION LIEN</option>
// 					<option value = "133" > CONTRACT & amp; DEBT</option>
//					<option value = "185" > CORPORATE TRUST</option>
//					<option value = "209" > CUSTODY(DOMESTIC RELATIONS) </ option >

//                  <option value="210">CYBER STALKING/STALKING</option>
//					<option value = "211" > DATING VIOLENCE</option>
//					<option value = "137" > DELAYED BIRTH CERTIFICATE(CA)</option>
//					<option value = "255" > DISCLAIMER(PROBATE) </ option >

//                  <option value="186">DISCRIMINATION EMPLOYMNT/OTHER</option>
//					<option value = "212" > DISESTABLISHMENT OF PATERNITY</option>
//					<option value = "213" > DISSOLUTION </ option >
    
//                  <option value= "214" > DOMESTIC VIOLENCE</option>
//    				<option value = "215" > DOR JUDICIAL CASE</option>
//					<option value = "5" > DRUG COURT</option>
//    				<option value = "28" > DRUG COURT</option>
//    				<option value = "31" > DRUG COURT</option>
//    				<option value = "139" > EJECTMENT </ option >
    
//                  <option value= "138" > EMINENT DOMAIN</option>
//    				<option value = "173" > ENVIRONMENTAL / TOXIC TORT</option>
//    			    <option value = "198" > EVICTION(COUNTY CIVIL) </ option >
//                  <option value= "494" > EVICTION WITH DAMAGES OVER $2500</option>
//					<option value = "492" > EVICTION WITH DAMAGES UP TO $2500</option>
//					<option value = "6" > EXTRADITION</ option >
//                  < option value= "140" > EXTRAORDINARY WRIT</option>
//    				<option value = "256" > FAMILY ADMINISTRATION</option>
//    				<option value = "7" > FELONY</ option >
    
//                  <option value= "462" > FLORIDA CONTRABAND FORFEITURE ACT</option>
//    			    <option value = "199" > FORECLOSURE(COUNTY CIVIL) </ option >
    
//                  <option value= "141" > FORECLOSURE = &lt; $50K</option>
//					<option value = "143" > FORECLOSURE = &gt; $250K</option>
//					<option value = "142" > FORECLOSURE & gt;$50K, &lt;$250K</option>
//					<option value = "144" > FOREIGN JUDGMENT(CIRCUIT CIV)</option>
//					<option value = "257" > FORMAL ADMINISTRATION</option>
//					<option value = "236" > GIFT TO MINOR ACT</option>
//					<option value = "237" > GUARDIAN ADVOCATE</option>
//					<option value = "238" > GUARDIANSHIP - PERSON / PROPERTY </ option >

//                  <option value="239">GUARDIANSHIP OF PERSON</option>
//					<option value = "240" > GUARDIANSHIP OF PROPERTY</option>
//					<option value = "484" > HABEAS CORPUS</option>
//    				<option value = "18" > HANDICAP PARKING</option>
//    				<option value = "46" > HANDICAP PARKING</option>
//    			    <option value = "145" > HR FORECLOSURE = &lt; $50K</option>
//					<option value = "147" > HR FORECLOSURE =&gt; $250K</option>
//					<option value = "146" > HR FORECLOSURE &gt;$50K, &lt;$250K</option>
//					<option value = "242" > INCAPACITATED(LEGACY SYS) </ option >

//                  < option value="486">INCAPACITY</option>
//					<option value = "187" > INSURANCE CLAIM</option>
//					<option value = "188" > INTELLECTUAL PROPERTY</option>
//					<option value = "148" > INVERSE CONDEMNATION</option>
//					<option value = "119" > IV - D ADMINISTRATIVE SUPPORT</option>
//					<option value = "150" > LANDLORD / TENANT </ option >
      
//                  < option value= "149" > LEGAL MALPRACTICE</option>
//      			<option value = "189" > LIBEL / SLANDER </ option >
      
//                  <option value= "19" > MARINE INFRACTION</option>
//      		    <option value = "251" > MARRIAGE LICENSE (W/DISCOUNT)</option>
//					<option value = "252" > MARRIAGE LICENSE FOR MINOR</option>
//					<option value = "176" > MASS TORT</option>
//					<option value = "151" > MEDICAL MALPRACTICE</option>
//					<option value = "243" > MINOR GUARDIANSHIP(LEGACY SYS)</option>
//					<option value = "20"  > MISDEMEANOR</ option >

//                  < option value="126">MOTION FOR ARBITRATION AWARD</option>
//					<option value = "47" > MUNICIPAL ORDINANCE</option>
// 					<option value = "23" > NA </ option >
 
//                  < option value= "216" > NAME CHANGE</option>
// 					<option value = "177" > NEGLIGENT SECURITY</option>
// 					<option value = "496" > NON BOATING MARINE INFRACTION</option>
// 					<option value = "154" > NON HR FORECLOSURE = &lt; $50K</option>
//					<option value = "156" > NON HR FORECLOSURE = &gt; $250K</option>
//					<option value = "155" > NON HR FORECLOSURE&gt;$50K,&lt;$250K</option>
//					<option value = "258" > NOTICE OF TRUST</option>
//					<option value = "178" > NURSING HOME NEGLIGENCE</option>
//					<option value = "200" > OTHER(COUNTY CIVIL) </ option >
    
//                  < option value= "160" > OTHER CIRCUIT</option>
//    				<option value = "217" > OTHER DOMESTIC RELATIONS</option>
//					<option value = "162" > OTHER NEGLIGENCE</option>
//    				<option value = "161" > OTHER PROFESSIONAL MALPRACTICE</option>
//					<option value = "163" > OTHER REAL PROPERTY</option>
//					<option value = "157" > OTHER RP ACTIONS = &lt; $50K</option>
//					<option value = "159" > OTHER RP ACTIONS = &gt; $250K</option>
//					<option value = "158" > OTHER RP ACTIONS &gt;$50K, &lt;$250K</option>
//					<option value = "22" > PALM BEACH COUNTY ORDINANCE</option>
//					<option value = "33" > PALM BEACH COUNTY ORDINANCE</option>
//					<option value = "48" > PARKING CITATION</option>
//					<option value = "21" > PARKING CITATION</option>
//					<option value = "218" > PATERNITY </ option >

//                  <option value="259">PET TO OPEN SAFE DEPOSIT BOX</option>
//					<option value = "260" > PET / ORD TO ADMIT FOREIGN WILL</option>
//					<option value = "488" > PETITION FIREARMS</option>
// 					<option value = "244" > PETITION / ORDER(GUARDIANSHIP) </ option >
//                  < option value= "490" > PETITION / ORDER(MENTAL HEALTH) </ option >
//                  < option value= "261" > PETITION / ORDER(PROBATE) </ option >
//                  < option value= "268" > PIP CLAIM &lt;$100</option>
//					<option value = "269" > PIP CLAIM =&gt;$100 TO $500</option>
//					<option value = "201" > PIP CLAIM &gt; $5000</option>
//					<option value = "270" > PIP CLAIM &gt;$2500 TO $5000</option>
//					<option value = "271" > PIP CLAIM &gt;$500 TO $2500</option>
//					<option value = "179" > PREMISES LIABILITY COMMERCIAL</option>
//					<option value = "180" > PREMISES LIABILITY RESIDENTIAL</option>
//					<option value = "262" > PRF GUARDIAN BOND (LEGACY SYS)</option>
//					<option value = "164" > PRODUCT LIABILITY</option>
//    				<option value = "152" > PROF MALPRACTICE (LEGACY SYS)</option>
//					<option value = "165" > REAL PROPERTY/FORECLOSURE</option>
//    				<option value = "219" > REGISTRATION OF ORDER</option>
//					<option value = "246" > REM NON-AGE DISABILITY</option>
//					<option value = "220" > REPEAT VIOLENCE</option>
//    				<option value = "272" > REPLEVIN(SM CLM / LEGACY SYS) </ option >
//                  <option value= "480" > RISK PROTECTION</option>
//    				<option value = "24" > RYCE ACT</option>
//    				<option value = "191" > SECURITIES LITIGATION</option>
//    				<option value = "202" > SECURITY BOND (COUNTY CIVIL) </option>
//					<option value = "221" > SEXUAL VIOLENCE</option>
//    				<option value = "190" > SHAREHOLDER DERIVATIVE ACTION</option>
//					<option value = "222" > SIMPLIFIED DISSOLUTION</option>
//    				<option value = "273" > SM CLM REPLEVIN **DO NOT USE**</option>
//					<option value = "274" > SM CLM REPLEVIN **DO NOT USE**</option>
//					<option value = "275" > SM CLM REPLEVIN **DO NOT USE**</option>
//					<option value = "276" > SM CLM REPLEVIN = &lt; 1000</option>
//					<option value = "277" > SM CLM REPLEVIN &gt; 1000 to 2500</option>
//					<option value = "278" > SM CLM REPLEVIN &gt;$2500</option>
//					<option value = "279" > SMALL CLAIMS(LEGACY SYS)</option>
//					<option value = "280" > SMALL CLAIMS &lt;$100</option>
//					<option value = "281" > SMALL CLAIMS =&gt;$100 TO $500</option>
//					<option value = "282" > SMALL CLAIMS &gt;$2500</option>
//					<option value = "283" > SMALL CLAIMS &gt;$500 TO $2500</option>
//					<option value = "474" > SMALL CLAIMS FORECLOSURE</option>
//					<option value = "263" > SMALL ESTATE</option>
//    				<option value = "264" > SUMMARY ADMINISTRATION &lt;$1000</option>
//					<option value = "265" > SUMMARY ADMINISTRATION =&gt;$1000</option>
//					<option value = "223" > SUPPORT </ option >

//                  <option value="224">TERM PARENTAL RIGHTS(FAMILY) (level 5 case type)</option>
//					<option value = "174" > THIRD PARTY INDEMNIFICATION</option>
//					<option value = "169" > TIMESHARE / FORECLOSURE </ option >
    
//                  < option value= "168" > TIMESHARE FC = &gt; $250K</option>
//					<option value = "167" > TIMESHARE FC &gt;$50K, &lt;$250K</option>
//					<option value = "166" > TIMESHARE FC ACTIONS = &lt; $50K</option>
//					<option value = "170" > TIMESHARE / MULTIPLE COUNTS</option>
//					<option value = "192" > TRADE SECRET</option>
//					<option value = "25" > TRAFFIC CRIMINAL</option>
//					<option value = "26" > TRAFFIC INFRACTION</option>
//					<option value = "225" > TRANSFEER CASE</option>
//					<option value = "226" > TRANSFER CHILD SUPPORT IV-D</option>
//					<option value = "227" > TRANSFER CHILD SUPPORT NON IV-D</option>
//					<option value = "228" > TRANSFER DRFF DI</option>
//					<option value = "266" > TRUST </ option >
    
//                  < option value= "193" > TRUST LITIGATION</option>
//    				<option value = "229" > UIFSA INITIATING NEW</option>
//					<option value = "230" > UIFSA IV-D</option>
//    				<option value = "231" > UIFSA NON IV-D</option>
//    				<option value = "232" > UIFSA / URESA IV-D (LEGACY SYS)</option>
//					<option value = "233" > URESA / UIFSA INITIATING</option>
//    				<option value = "234" > URESA / UIFSA RESPONDING</option>
//    				<option value = "247" > V.A.</ option >
    
//                  <option value= "27" > VETERANS COURT</option>
//    			    <option value = "29" > VETERANS COURT</option>
//    				<option value = "30" > VETERANS COURT</option>
//    				<option value = "49" > VETERANS COURT</option>
//    				<option value = "34" > VETERANS COURT</option>
//    				<option value = "35" > VETERANS COURT</option>
//    				<option value = "32" > VETERANS COURT</option>
//    				<option value = "248" > VOLUNTARY(LEGACY SYS) </ option >
    
//                  <option value= "482" > VULNERABLE ADULT PROTECTION INJUNCTION</option>
//    				<option value = "284" > WAGE DISPUTE &lt;$100</option>
//					<option value = "285" > WAGE DISPUTE =&gt;$100 TO $500</option>
//					<option value = "286" > WAGE DISPUTE &gt;$2500</option>
//					<option value = "287" > WAGE DISPUTE &gt;$500 TO $2500</option>
//					<option value = "203" > WAGE DISPUTE &gt;$5000</option>
//					<option value = "0" > All </ option >
//                </ select >



    }
}
