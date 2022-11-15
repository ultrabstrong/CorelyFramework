using Corely.FC2DW;
using Corely.Connections.Proxies;
using Corely.Data.Culture;
using Corely.Data.Dates;
using Corely.Data.Delimited;
using Corely.Data.Serialization;
using Corely.FlexiCapture;
using Corely.FTP;
using Corely.Helpers;
using Corely.Imaging.Core;
using Corely.Imaging.Converters;
using Corely.Logging;
using Corely.Sage300HH2;
using Corely.Sage300HH2.Core;
using Corely.Sage300HH2.Core.Document;
using Corely.Security;
using Corely.Security.Authentication;
using Corely.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using LibPostalNet;
using System.Text;
using System.Net;
using Corely.Data.Text;
using Corely.LibPostalClient;
using System.Text.RegularExpressions;
using Corely.Sage300HH2.Core.Generic;
using Corely.Kingstone;
using Corely.Kingstone.Core;
using System.Threading.Tasks;
using Corely.DocuWareService;
using Corely.Services;
using Corely.DocuWareService.Core.Responses;
using Corely.Distribution;
using System.Data.SqlClient;
using System.Security.Cryptography;
using Corely.Data.Encoding;
using Corely.Connections;
using Corely.Core;
using System.Security.Authentication;
using DocuWare.Platform.ServerClient;
using Corely.DocuWare;

namespace Corely.TestConsole
{
    public class Program
    {
        /// <summary>
        /// Create desktop directory for use through out program
        /// </summary>
        static string desktopdir { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\";

        /// <summary>
        /// Entry method for test app
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            try
            {
                TestCountryLookup();
            }
            catch (Exception ex)
            {
                // Output exception
                Console.WriteLine($"Exception caught:{Environment.NewLine}{ex}");
            }
            // Program finished. Wait for user input to exit
            Console.WriteLine($"{Environment.NewLine}Program finished. Press any key to exit.");
            Console.ReadKey();
        }

        #region Corely Tests

        /// <summary>
        /// Test logger rotation and deletion
        /// </summary>
        static void TestLogger()
        {
            FileLogger logger = new FileLogger("LogTest", Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/logtest", "LogFile_") { LogLevel = LogLevel.DEBUG };
            //FileLogger logger = new FileLogger("LogTest", @"C:\ProgramData\AbbyyAddins", "LogFile") { LogLevel = LogLevel.DEBUG };
            for (int i = 0; i < 10; i++)
            {
                string randomtext = @"Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur";
                logger.WriteLog($"Message {i}", randomtext, LogLevel.DEBUG);
            }
        }

        /// <summary>
        /// Test country code lookup
        /// </summary>
        static void TestCountryLookup()
        {
            // Create lookup
            CountryTagLookup lookup = new CountryTagLookup();
            lookup.Corrections.Add("", "USA");
            lookup.Corrections.Add("Virgin Islands (BR)", "British Virgin Islands");
            lookup.Corrections.Add("Cape Verde", "Cabo Verde");
            // Load country names from CSV file
            string[] countrynames = new[] { null, "", "USA" }; 
            for (int i = 1; i < countrynames.Length; i++)
            {
                string countryname = countrynames[i] ?? "USA";
                //string twodigitname = lookup.GetTwoLetterName(countryname, new List<string>() { "US" });
                string twodigitname = lookup.GetTwoLetterName(countryname);
                if (!string.IsNullOrWhiteSpace(twodigitname))
                {
                    Console.WriteLine($"{countryname} : {twodigitname}");
                }
                else
                {
                    Console.WriteLine($"{countryname} Not found");
                }
            }
        }

        /// <summary>
        /// Test delimited reader
        /// </summary>
        static void TestDelimitedReader()
        {
            DelimitedReader dr = new DelimitedReader();
            List<ReadRecordResult> results = dr.ReadAllFileRecords(@"<path-to-file>.csv");
            for (int i = 1; i < results.Count; i++)
            {
                string s1 = results[i].Tokens[0];
            }
        }

        /// <summary>
        /// Test delimited writer
        /// </summary>
        static void TestDelimitedWriter()
        {
            List<string> vals = new List<string>();
            DelimitedWriter dw = new DelimitedWriter('|', '"', Environment.NewLine);
            // Create I records
            for (int i = 0; i < 4; i++)
            {
                // Create J tokens
                for (int j = 0; j < 30; j++)
                {
                    vals.Add($"{i}{j}");
                }
                dw.AppendRecordToFile(vals, @"<path-to-file>.csv", true);
                vals = new List<string>();
            }
        }

        /// <summary>
        /// Test modifying CSV from one file and outputting to another file
        /// </summary>
        static void TestModifyDelimitedToFile()
        {
            // Read all records
            string filein = @"<path-to-file>.csv";
            DelimitedReader dr = new DelimitedReader();
            List<ReadRecordResult> records = dr.ReadAllFileRecords(filein);
            // Modify all records
            for (int i = 0; i < records.Count; i++)
            {
                List<string> newtokens = records[i].Tokens[0].Split(new[] { '.' }, StringSplitOptions.None).ToList();
                newtokens.Add(records[i].Tokens[1]);
                records[i].Tokens = newtokens;
            }
            // Write all records
            string fileout = desktopdir + @"<path-to-file>.csv";
            DelimitedWriter dw = new DelimitedWriter();
            dw.AppendAllReadRecordsToFile(records, fileout);
        }

        /// <summary>
        /// Test overwrite path protection
        /// </summary>
        static void TestOverwriteProtectedPath()
        {
            string file = "<path-to-file>.txt";

            for (int i = 0; i < 10; i++)
            {
                string writepath = FilePathHelper.GetOverwriteProtectedPath(file);
                File.WriteAllText(writepath, $"output{i}");
            }
        }

        /// <summary>
        /// Test encryption
        /// </summary>
        static void TestEncryption()
        {
            // Create value list, user key from password, and random system key
            PBKDF2HashedValue hv = new PBKDF2HashedValue("password123");
            string userkey = hv.Hash;
            string syskey = AESEncryption.CreateRandomBase64Key();
            // Create and fill encrypted values dictionary
            AESValues encryptedVals = new AESValues(AESEncryption.CreateRandomBase64Key())
            {
                { "entry1", "secret1" },
                { "entry2", "secret2" }
            };
            encryptedVals["test1"] = encryptedVals["test2"];
            encryptedVals.SetEncryptionKey(AESEncryption.CreateRandomBase64Key());
            // Serialize encrypted values
            string xml = XmlSerializer.Serialize<AESValues>(encryptedVals);
            // Deserialize encrypted values
            encryptedVals = XmlSerializer.DeSerialize<AESValues>(xml);
            // Set key again since key isn't serialized
            encryptedVals.SetEncryptionKey(userkey);
            // Change key to new key
            encryptedVals.SetEncryptionKey(syskey);
            ;
        }

        /// <summary>
        /// Test hashing (least secure)
        /// </summary>
        static void TestHashing()
        {
            // Test without salt
            HashedValue hv = new HashedValue("secret");
            HashedValue hv2 = new HashedValue("secret", HashAlgorithmName.SHA256);
            HashedValue hv3 = new HashedValue("secret", HashAlgorithmName.SHA1);
            string xml = XmlSerializer.Serialize(hv3);
            bool equals = hv.Equals("secret");
            bool equals1 = hv.Equals(hv2);
            bool equals2 = hv.Equals(hv3);
            // Test with salt
            hv = new HashedValue("secret", 8);
            hv2 = new HashedValue("secret", 8, HashAlgorithmName.SHA256);
            hv3 = new HashedValue("secret", 8, HashAlgorithmName.SHA1);
            xml = XmlSerializer.Serialize(hv3);
            equals = hv.Equals("secret");
            equals1 = hv.Equals(hv2);
            equals2 = hv.Equals(hv3);
        }

        /// <summary>
        /// Test HMAC hashing (medium secure)
        /// </summary>
        static void TestHMACHashing()
        {
            // Test without salt and without key
            HMACHashedValue hv = new HMACHashedValue("secret");
            HMACHashedValue hv2 = new HMACHashedValue("secret", HashAlgorithmName.SHA256);
            HMACHashedValue hv3 = new HMACHashedValue("secret", HashAlgorithmName.SHA1);
            string xml = XmlSerializer.Serialize(hv3);
            bool equals = hv.Equals("secret");
            bool equals1 = hv.Equals(hv2);
            bool equals2 = hv.Equals(hv3);
            // Test with salt and without key
            hv = new HMACHashedValue("secret", 8);
            hv2 = new HMACHashedValue("secret", 8, HashAlgorithmName.SHA256);
            hv3 = new HMACHashedValue("secret", 8, HashAlgorithmName.SHA1);
            xml = XmlSerializer.Serialize(hv3);
            equals = hv.Equals("secret");
            equals1 = hv.Equals(hv2);
            equals2 = hv.Equals(hv3);
            // Test without salt and with key
            hv = new HMACHashedValue("secret", "hashingkey", false);
            hv2 = new HMACHashedValue("secret", "hashingkey", false, HashAlgorithmName.SHA256);
            hv3 = new HMACHashedValue("secret", AESEncryption.CreateRandomBase64Key(), true, HashAlgorithmName.SHA1);
            xml = XmlSerializer.Serialize(hv3);
            equals = hv.Equals("secret");
            equals1 = hv.Equals(hv2);
            equals2 = hv.Equals(hv3);
            // Test with salt and with key
            string secureKey = AESEncryption.CreateRandomBase64Key();
            hv = new HMACHashedValue("secret", secureKey, true, 8);
            hv2 = new HMACHashedValue("secret", secureKey, true, 8, HashAlgorithmName.SHA256);
            hv3 = new HMACHashedValue("secret", secureKey, true, 8, HashAlgorithmName.SHA1);
            xml = XmlSerializer.Serialize(hv3);
            equals = hv.Equals("secret");
            equals1 = hv.Equals(hv2);
            equals2 = hv.Equals(hv3);
        }

        /// <summary>
        /// Test PBKDF2 hashing (most secure)
        /// </summary>
        static void TestPBKDF2Hashing()
        {
            PBKDF2HashedValue shv = new PBKDF2HashedValue("secret");
            PBKDF2HashedValue shv2 = new PBKDF2HashedValue("secret", HashAlgorithmName.SHA256);
            PBKDF2HashedValue shv3 = new PBKDF2HashedValue("secret", HashAlgorithmName.SHA1);
            string xml2 = XmlSerializer.Serialize(shv3);
            bool sequals = shv.Equals("secret");
            bool sequals1 = shv.Equals(shv2);
            bool sequals2 = shv.Equals(shv3);
        }

        /// <summary>
        /// Test general credentials
        /// </summary>
        static void TestGeneralCredentials()
        {
            string encryptionKey = AESEncryption.CreateRandomBase64Key();
            GeneralCredentials credentials = new GeneralCredentials(encryptionKey);
            credentials.Password.DecryptedValue = "password";
            credentials.Token.Token.DecryptedValue = "token";
            credentials.AuthSecrets["entry1"] = "secret1";
            credentials.AuthSecrets["entry2"] = "secret2";
            credentials.AuthValues["name1"] = "value1";
            credentials.AuthValues["name2"] = "value2";
            string xml = XmlSerializer.Serialize(credentials);
            credentials = XmlSerializer.DeSerialize<GeneralCredentials>(xml);
            credentials.SetEncryptionKey(encryptionKey);
            Console.WriteLine(credentials.Password.DecryptedValue);
            Console.WriteLine(credentials.Token.Token.DecryptedValue);
            Console.WriteLine(credentials.AuthSecrets["entry1"]);
            Console.WriteLine(credentials.AuthSecrets["entry2"]);
            Console.WriteLine(credentials.AuthValues["name1"]);
            Console.WriteLine(credentials.AuthValues["name2"]);
        }

        /// <summary>
        /// Test terms calculator
        /// </summary>
        static void TestTerms()
        {
            // Static list of terms to test
            List<string> terms = new List<string>()
            {
                /*
                "4% 40 NET 41 DAYS",
                "NET 20TH",
                "NET 30 DAYS",
                "1% 15 DAYS NET 30 DAYS",
                "NET 15 DAYS",
                "2% 10TH NET 25TH",
                "1% 10 DAYS NET 30",
                "NET",
                "NET 10TH",
                "2% 10 DAYS NET 30 DAYS",
                "NET 30 DAYS",
                "4% 40 NET 41 DAYS",
                "6% 10TH NET 25TH",
                "NET",
                "10",*/
                "Net 20"
            };

            // Iterate terms and test
            foreach (string term in terms)
            {
                // Create terms calculator
                TermsCalculator calculator = new TermsCalculator(term, DateTime.Now);
                // Output no date found
                if (!calculator.DueDatesFound)
                {
                    Console.Write($"Due dates not found for term {term,-24}");
                }
                else
                {
                    // Output discount
                    if (calculator.HasDiscountDate)
                    {
                        Console.Write($"Due date(s) for term {term,-24} : {calculator.Discount} discount if paid by {calculator.DiscountDate:MM-dd-yyyy}");
                    }
                    // Output first date found
                    else
                    {
                        Console.Write($"Due date(s) for term {term,-24} : {calculator.DueDate:MM-dd-yyyy}");
                    }
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Test html viewer with static html string
        /// </summary>
        static void TestHtmlViewer()
        {
            Thread thread = new Thread(() =>
            {

                string html = "<div><h1>lookit!</h1></div>";
                HtmlBase64ImageViewerUC viewer = new HtmlBase64ImageViewerUC(html);
                viewer.Show(true);
                ;
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        /// <summary>
        /// Display dropdown window
        /// </summary>
        static void TestDropdownDisplay()
        {
            Thread thread = new Thread(() =>
            {
                DropdownDisplay display = new DropdownDisplay();
                display.DisplayText = "Select an item";
                display.DropdownItems = new List<UI.Models.DropdownDisplayModel>()
                {
                    new UI.Models.DropdownDisplayModel("item1", new Dictionary<string, string>
                    {
                        {"additional1", "add1" },
                        {"additional2", "add2" },
                    }),
                    new UI.Models.DropdownDisplayModel("item2", new Dictionary<string, string>
                    {
                        {"additional3", "add3" },
                        {"additional4", "add4" },
                    })
                };
                display.ShowDialog();
                ;
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        #endregion

        #region Corely Sample Scripts

        /// <summary>
        /// Base script for importing vendors
        /// </summary>
        static void VendorDataSetImportCode()
        {
            /*
            using System;
            using System.Collections.Generic;
            using Corely.Logging;
            using Corely.Data.Delimited;
            using Corely.Data.Culture;

            // Create file logger for writing to file
            FileLogger logger = new FileLogger("Vendor Import", @"C:\ProgramData\AbbyyAddins\", "DataSetImportLog") { LogLevel = LogLevel.WARN };
            try
            {
                // Create country code lookup
                CountryTagLookup lookup = new CountryTagLookup();
                lookup.Corrections.Add("", "USA");
                // Create list of CSV files to import
                List<string> csvFiles = new List<string>()
                {
                    @"C:\FlexiCapture.in\vendors.csv"
                };
                // Iterate CSV files and import them
                foreach (string csvFile in csvFiles)
                {
                    // Read and parse files
                    DelimitedReader dr = new DelimitedReader();
                    List<ReadRecordResult> results = dr.ReadAllFileRecords(csvFile);
                    // Iterate file records and add them to dataset
                    for (int i = 1; i < results.Count; i++)
                    {
                        try
                        {
                            // Create record and add values
                            IDataSetRecord record = DataSet.CreateRecord();
                            record.AddValue("Id", results[i].Tokens[1]);
                            record.AddValue("Name", results[i].Tokens[2]);
                            record.AddValue("Street", results[i].Tokens[3]);
                            record.AddValue("Street", results[i].Tokens[4]);
                            record.AddValue("Street", results[i].Tokens[5]);
                            record.AddValue("City", results[i].Tokens[6]);
                            record.AddValue("State", results[i].Tokens[7]);
                            record.AddValue("ZIP", results[i].Tokens[8]);
                            record.AddValue("CountryCode", lookup.GetTwoLetterName(results[i].Tokens[9] ?? ""));
                            record.AddValue("BusinessUnitId", results[i].Tokens[12]);
                            record.AddValue("GLCode", results[i].Tokens[10]);
                            record.AddValue("Terms", results[i].Tokens[11]);
                            record.AddValue("Division", results[i].Tokens[13]);
                            DataSet.AddRecord(record);
                        }
                        catch (Exception ex)
                        {
                            // Output failed record to log
                            DelimitedWriter dw = new DelimitedWriter();
                            logger.WriteLog("Failed to create data record", dw.WriteRecordToString(results[i].Tokens), LogLevel.NOTICE);
                            logger.WriteLog("Error while creating data record", ex, LogLevel.ERROR);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception to file and throw
                logger.WriteLog("Failed to execute script", ex, LogLevel.ERROR);
                throw;
            }
            */
        }

        /// <summary>
        /// Base script for importing GLs from a large file
        /// </summary>
        static void LargeGLDataSetImportCode()
        {
            /*
            using System;
            using System.Collections.Generic;
            using Corely.Logging;
            using Corely.Data.Delimited;
            using Corely.Data.Culture;

            // Create file logger for writing to file
            FileLogger logger = new FileLogger("GL Import", @"C:\ProgramData\AbbyyAddins\", "DataSetImportLog") { LogLevel = LogLevel.WARN };
            try
            {
                // Create list of CSV files to import
                List<string> csvFiles = new List<string>()
                {
                    @"C:\FlexiCapture.IN\accounts-all.csv",
                    @"C:\FlexiCapture.IN\accounts-cpr.csv"
                };
                // Iterate CSV files and import them
                foreach (string csvFile in csvFiles)
                {
                    logger.WriteLog("Reading file", csvFile, LogLevel.DEBUG);
                    // Read in header record
                    DelimitedReader dr = new DelimitedReader();
                    ReadRecordResult result = dr.ReadFileRecord(0, csvFile);
                    do
                    {
                        try
                        {
                            // Get next result
                            logger.WriteLog("Reading result position", result.EndPosition.ToString(), LogLevel.DEBUG);
                            result = dr.ReadFileRecord(result.EndPosition, csvFile);
                            // Create record and add values
                            IDataSetRecord record = DataSet.CreateRecord();
                            record.AddValue("Location", result.Tokens[0]);
                            record.AddValue("GLCode", result.Tokens[1]);
                            record.AddValue("GLDescription", result.Tokens[2]);
                            DataSet.AddRecord(record);
                        }
                        catch (Exception ex)
                        {
                            // Output failed record to log
                            DelimitedWriter dw = new DelimitedWriter();
                            logger.WriteLog("Failed to create data record", dw.WriteRecordToString(result.Tokens), LogLevel.NOTICE);
                            logger.WriteLog("Error while creating data record", ex, LogLevel.ERROR);
                        }
                    }
                    while (result.HasMore);
                }
            }
            catch (Exception ex)
            {
                // Log exception to file and throw
                logger.WriteLog("Failed to execute script", ex, LogLevel.ERROR);
                throw;
            }
            */
        }

        /// <summary>
        /// Base script for importing datasets in cloud environments
        /// </summary>
        static void CloudDataSetImportcode()
        {
            /*
            using System;
            using System.Collections.Generic;
            using Corely.Logging;
            using Corely.Data.Delimited;

            try
            {
                // Create list of CSV files to import
                List<string> csvFiles = new List<string>()
                {
                    @"<path-to-csv-1>",
                    @"<path-to-csv-2>"
                };
                // Iterate CSV files and import them
                foreach (string csvFile in csvFiles)
                {
                    if(System.IO.File.Exists(csvFile))
                    {
                        // Read and parse files
                        DelimitedReader dr = new DelimitedReader();
                        List<ReadRecordResult> results = dr.ReadAllFileRecords(csvFile);
                        // Iterate file records and add them to dataset
                        for (int i = 0; i < results.Count; i++)
                        {
                            // Create record and add values
                            IDataSetRecord record = DataSet.CreateRecord();
                            record.AddValue("Counterparties", results[i].Tokens[1]);
                            record.AddValue("VendorID", results[i].Tokens[0]);
                            DataSet.AddRecord(record);
                        }
                    }
                    else
                    {
                        FCTools.ShowMessage("File does not exist: " + csvFile);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception to file and throw
                FCTools.ShowMessage("Import Failed - " + ex.Message);
                throw;
            }
            */
        }

        /// <summary>
        /// Base script for importing large datasets in cloud environments
        /// </summary>
        static void CloudLargeDataSetImportCode()
        {
            /*
            using System;
            using System.Collections.Generic;
            using Corely.Logging;
            using Corely.Data.Delimited;

            try
            {
                // Create list of CSV files to import
                List<string> csvFiles = new List<string>()
                {
                    @"<path-to-csv-1>",
                    @"<path-to-csv-2>"
                };
                // Iterate CSV files and import them
                foreach (string csvFile in csvFiles)
                {
                    if(System.IO.File.Exists(csvFile))
                    {
                        // Read in header record
                        DelimitedReader dr = new DelimitedReader();
                        ReadRecordResult result = dr.ReadFileRecord(0, csvFile);
                        int recordId = 1;
                        do
                        {
                            try
                            {
                                // Read in next data record
                                result = dr.ReadFileRecord(result.EndPosition, csvFile);
                                recordId++;
                                // Create record and add values
                                IDataSetRecord record = DataSet.CreateRecord();
                                record.AddValue("Counterparties", result.Tokens[1]);
                                record.AddValue("VendorID", result.Tokens[0]);
                                DataSet.AddRecord(record);
                            }
                            catch(Exception ex)
                            {
                                FCTools.ShowMessage("Failed to import record " + recordId.ToString());
                                FCTools.ShowMessage(ex.Message);
                            }
                        }
                        while (result.HasMore);
                    }
                    else
                    {
                        FCTools.ShowMessage("File does not exist: " + csvFile);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception to file and throw
                FCTools.ShowMessage("Import Failed - " + ex.Message);
                throw;
            }
            */
        }

        #endregion

        #region Distributions Tests

        /// <summary>
        /// Test distributions
        /// </summary>
        static void TestDistributions()
        {
            // Delimited writer for outputting results
            DelimitedWriter dw = new DelimitedWriter();

            // Test basic dist
            DistributionSettings settings = new DistributionSettings
            {
                DistributionType = DistributionType.Basic,
            };

        // Test basic with exact distribution
        basicdist:
            Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}{settings.DistributionType.ToString().ToUpper()} DIST WITH SETVALONE = {settings.SetValueToOne.ToString().ToUpper()}");
            decimal value = 4.0M;
            Console.WriteLine($"{Environment.NewLine}Exact basic dist for value {value}");
            Console.WriteLine(dw.WriteRecordToString(settings.GetDistribtuion(value).Select(m => m.ToString()).ToList()));
            // Test basic with non-exact distribution
            value = 4.12M;
            Console.WriteLine($"{Environment.NewLine}Non-exact basic dist for value {value}");
            Console.WriteLine(dw.WriteRecordToString(settings.GetDistribtuion(value).Select(m => m.ToString()).ToList()));
            // Test with set value 1
            if (!settings.SetValueToOne)
            {
                settings.SetValueToOne = true;
                goto basicdist;
            }


            // Test fixed distribution
            settings = new DistributionSettings
            {
                DistributionType = DistributionType.Fixed,
                FixedDistributionCount = 4,
                SetValueToOne = false
            };
        fixeddist:
            Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}{settings.DistributionType.ToString().ToUpper()} DIST WITH SETVALONE = {settings.SetValueToOne.ToString().ToUpper()}");
            // Text fixed with exact dist
            value = 4.0M;
            Console.WriteLine($"{Environment.NewLine}Exact fixed dist for value {value}");
            Console.WriteLine(dw.WriteRecordToString(settings.GetDistribtuion(value).Select(m => m.ToString()).ToList()));
            // Test basic with non-exact dist
            value = 4.12M;
            Console.WriteLine($"{Environment.NewLine}Non-exact fixed dist for value {value}");
            Console.WriteLine(dw.WriteRecordToString(settings.GetDistribtuion(value).Select(m => m.ToString()).ToList()));
            // Test with set value 1
            if (!settings.SetValueToOne)
            {
                settings.SetValueToOne = true;
                goto fixeddist;
            }


            // Test value distribution
            settings = new DistributionSettings
            {
                DistributionType = DistributionType.Value,
                Distributions = new List<DistributionValue>()
                {
                    new DistributionValue(1.0M),
                    new DistributionValue(1.0M),
                    new DistributionValue(1.0M)
                },
                SetValueToOne = false
            };
        valuedist:
            Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}{settings.DistributionType.ToString().ToUpper()} DIST WITH SETVALONE = {settings.SetValueToOne.ToString().ToUpper()}");
            // Test with exact dist
            value = 3.0M;
            Console.WriteLine($"{Environment.NewLine}Exact value split for val {value}, dist {string.Join(",", settings.Distributions.Select(m => m.ToString()))}");
            Console.WriteLine(dw.WriteRecordToString(settings.GetDistribtuion(value).Select(m => m.ToString()).ToList()));
            // Test with excessive exact value
            value = 4.0M;
            Console.WriteLine($"{Environment.NewLine}Excessive exact value split for val {value}, dist {string.Join(",", settings.Distributions.Select(m => m.ToString()))}");
            Console.WriteLine(dw.WriteRecordToString(settings.GetDistribtuion(value).Select(m => m.ToString()).ToList()));
            // Test with insufficient exact value
            value = 2.0M;
            Console.WriteLine($"{Environment.NewLine}Insufficient exact value split for val {value}, dist {string.Join(",", settings.Distributions.Select(m => m.ToString()))}");
            Console.WriteLine(dw.WriteRecordToString(settings.GetDistribtuion(value).Select(m => m.ToString()).ToList()));
            // Test with excessive partial value
            value = 3.3M;
            Console.WriteLine($"{Environment.NewLine}Excessive partial value split for val {value}, dist {string.Join(",", settings.Distributions.Select(m => m.ToString()))}");
            Console.WriteLine(dw.WriteRecordToString(settings.GetDistribtuion(value).Select(m => m.ToString()).ToList()));
            // Test with insufficient partial value
            value = 2.7M;
            Console.WriteLine($"{Environment.NewLine}Insufficient partial value split for val {value}, dist {string.Join(",", settings.Distributions.Select(m => m.ToString()))}");
            Console.WriteLine(dw.WriteRecordToString(settings.GetDistribtuion(value).Select(m => m.ToString()).ToList()));
            // Test with set val 1
            if (!settings.SetValueToOne)
            {
                settings.SetValueToOne = true;
                goto valuedist;
            }


            // Test percent distribution with 0 rounding
            settings = new DistributionSettings
            {
                DistributionType = DistributionType.Percent,
                Distributions = new List<DistributionValue>()
                {
                    new DistributionValue(70.0M),
                    new DistributionValue(20.0M),
                    new DistributionValue(10.0M)
                },
                RoundToPlaces = 0,
                SetValueToOne = false
            };
        percentdist:
            Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}{settings.DistributionType.ToString().ToUpper()} DIST WITH SETVALONE = {settings.SetValueToOne.ToString().ToUpper()}");
            // Test with range of values
            decimal[] values = new decimal[] { 5.0M, 5.3M, 5.7M, 6.0M, 6.3M, 6.7M, 7.0M, 7.3M, 7.7M, 8.0M, 8.3M, 8.7M, 9.0M, 9.3M, 9.7M, 10.0M, 10.3M, 10.7M, 11.0M, 11.3M, 11.7M, 12.0M, 12.3M, 12.7M, 13.0M, 13.3M, 13.7M, 14.0M, 14.3M, 14.7M, 15.0M, 15.3M, 15.7M, };
            foreach (decimal val in values)
            {
                settings.RoundToPlaces = 0;
                Console.WriteLine($"{Environment.NewLine}Percent split for val {val}, dist {string.Join(",", settings.Distributions.Select(m => m.ToString()))}, rounding 0, 1, and 2 places");
                List<decimal> dists = settings.GetDistribtuion(val);
                decimal sum = dists.Sum();
                Console.WriteLine(dw.WriteRecordToString(dists.Select(m => m.ToString().PadRight(6)).ToList()).Replace(",", "") + " => SUM " + sum);
                settings.RoundToPlaces = 1;
                dists = settings.GetDistribtuion(val);
                sum = dists.Sum();
                Console.WriteLine(dw.WriteRecordToString(dists.Select(m => m.ToString().PadRight(6)).ToList()).Replace(",", "") + " => SUM " + sum);
                settings.RoundToPlaces = 2;
                dists = settings.GetDistribtuion(val);
                sum = dists.Sum();
                Console.WriteLine(dw.WriteRecordToString(dists.Select(m => m.ToString().PadRight(6)).ToList()).Replace(",", "") + " => SUM " + sum);
            }
            // Test with set val 1
            if (!settings.SetValueToOne)
            {
                settings.SetValueToOne = true;
                goto percentdist;
            }
        }

        /// <summary>
        /// Test distrubtion settings user control
        /// </summary>
        static void TestDistributionSettingsUC()
        {
            // Create settings file path
            string settingsFilePath = "C:/ProgramData/SplitSettings/splitsettings.xml";
            // Create settings load function
            Func<List<DistributionSettings>> loadSettings = () =>
            {
                if (!File.Exists(settingsFilePath)) { return null; }
                string settingsXml = File.ReadAllText(settingsFilePath);
                return XmlSerializer.DeSerialize<List<DistributionSettings>>(settingsXml);
            };
            // Create settings save action
            Action<List<DistributionSettings>> saveSettings = (settings) =>
            {
                File.WriteAllText(settingsFilePath, XmlSerializer.Serialize(settings));
            };
            // Test with load and save actions set
            AsyncWindow.ShowAsync<Distributions>(loadSettings, null);
        }

        #endregion

        #region FTP Tests

        /// <summary>
        /// Test SFTP connection
        /// </summary>
        static void TestFTPUpload(bool sshftp = false)
        {
            // Create connection
            Console.WriteLine("Connecting");
            IFTPProxy connection;
            if (sshftp)
            {
                GeneralCredentials credentials = new GeneralCredentials();
                connection = FTPFactory.CreateSFTPProxy(credentials, "/DataSets/hConchoDocType2");
            }
            else
            {
                // Create connection credentials
                GeneralCredentials credentials = new GeneralCredentials();
                connection = FTPFactory.CreateFTPProxy(credentials, "", true);
            }
            FTPResponse response = connection.Connect();
            if (response.Status != 200) { throw response.Exception; }
            connection.UploadFile("<path-to-doc>", "<upload_name");
            /*
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("Getting unique file # " + i);
                string filename = connection.GetOverwriteProtectedFilename("test.txt").Data;
                Console.WriteLine("Uploading file " + i);
                connection.WriteAllText(filename, "Text" + i, false);
            }
            */
            // Handle result
            if (response.Status != 200)
            {
                throw response.Exception;
            }
            connection.Disconnect();
        }

        /// <summary>
        /// Test listing SFTP files
        /// </summary>
        static void TestFTPListAndDownloads(bool sshftp = false)
        {
            // Create connection
            Console.WriteLine("Connecting");
            IFTPProxy connection;
            if (sshftp)
            {
                GeneralCredentials credentials = new GeneralCredentials();
                connection = FTPFactory.CreateSFTPProxy(credentials, "");
            }
            else
            {
                // Create connection credentials
                GeneralCredentials credentials = new GeneralCredentials();
                connection = FTPFactory.CreateFTPProxy(credentials, "", true);
            }
            FTPResponse response = connection.Connect();
            if (response.Status != 200) { throw response.Exception; }
            response = connection.GetFilesInDirectory("");
            // List files in directory
            if (response.Status != 200) { Console.WriteLine($"{response.Status} : {response.Message}"); }
            if (!string.IsNullOrWhiteSpace(response.Data))
            {
                List<string> lines = response.Data.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).OrderBy(m => m).ToList();
                foreach (string line in lines)
                {
                    Console.WriteLine(line);
                }
                // Download first listed file
                response = connection.GetFileContents(lines[0]);
                if (response.Status != 200) { Console.WriteLine($"{response.Status} : {response.Message}"); }
                string doccontent = response.Data;
                File.WriteAllText(@"<path-to-dir>" + lines[0], doccontent);
            }
            connection.Disconnect();
        }

        #endregion

        #region Sage HH2 Tests

        /// <summary>
        /// Create and return sage connection
        /// </summary>
        /// <returns></returns>
        static SageHH2Connection GetSageConnection()
        {
            // Create base connection
            SageHH2Connection connection = new SageHH2Connection();
            // Connect and disconnect
            connection.Connect();
            return connection;
        }

        /// <summary>
        /// Test connection to Sage HH2
        /// </summary>
        static void TestSageConnection()
        {
            SageHH2Connection connection = GetSageConnection();
            bool isconnected = connection.IsConnected();
            connection.Disconnect();
            // Serialize and deserialize
            string xml = XmlSerializer.Serialize(connection);
            connection = XmlSerializer.DeSerialize<SageHH2Connection>(xml);
            // Connect and disconnect again with deserialized connection
            connection.Connect();
            isconnected = connection.IsConnected();
            connection.Disconnect();
            ;
        }

        /// <summary>
        /// Test getting data from sage
        /// </summary>
        static void TestSageGetMethods()
        {
            // Create base connection
            SageHH2Connection connection = GetSageConnection();
            // Run 'get all' methods
            List<Account> accounts = connection.GetAllAccounts();
            List<CostCode> costcodes = connection.GetAllCostCodes();
            List<Category> categories = connection.GetAllCategories();
            List<StandardCategory> standardcategories = connection.GetAllStandardCategories();
            List<Sage300HH2.Core.Payment> payment = connection.GetAllPayments();
            List<Sage300HH2.Core.Payment> payments = connection.GetPaymentsForVendorIdAndInvoiceCode("ven-id", "inv-code");
            List<Sage300HH2.Core.Distribution> distributions = connection.GetAllDistributions();
            List<Job> jobs = connection.GetAllJobs();
            string jobcode = "";
            foreach (Job job in jobs)
            {
                job.Categories = connection.GetCategoriesForJobId(job.Id);
                if (job.Categories != null)
                {
                    jobcode = job.Code;
                    break;
                }
            }
            // Get one job
            Job j = connection.GetJobForJobCode(jobcode);
            j.Categories = connection.GetCategoriesForJobId(j.Id);
            // Get all invoices
            List<Invoice> invoices = connection.GetAllAPInvoices();
            Invoice inv = null;
            foreach (Invoice invoice in invoices)
            {
                invoice.Distribution = connection.GetDistributionsForInvoiceId(invoice.Id);
                if (invoice.Distribution != null)
                {
                    inv = invoice;
                    break;
                }
            }
        }

        /// <summary>
        /// Test sage auto-category default
        /// </summary>
        static void TestSageAutoCategory()
        {
            SageHH2Connection connection = GetSageConnection();
            // Cost code without category
            string costcodecode = "cost-code-code";
            string jobcode = "job-code";
            Job job = connection.GetJobForJobCode(jobcode);
            CostCode code = connection.GetCostCodeForJobIdAndCostCodeCode(job.Id, costcodecode);
            bool isvalid = code != null;
            job.Categories = connection.GetCategoriesForJobId(job.Id);
            List<Category> category = connection.GetCategoriesForJobIdAndCostCodeId(job.Id, code.Id);
            ;
        }

        /// <summary>
        /// Test updating HH2 document
        /// </summary>
        static void TestSageUpdate()
        {
            DateTime start = DateTime.UtcNow;
            string requeststr = "{json-request-string}";
            string docid = "doc-id";
            UpdateDocumentRequest request = JsonConvert.DeserializeObject<UpdateDocumentRequest>(requeststr);
            SageHH2Connection connection = GetSageConnection();

            connection.UpdateDocument(request, docid);
            // Check for errors from export
            connection.ThrowExceptionsForDocument(docid, start);
        }

        /// <summary>
        /// Test posting data to sage
        /// </summary>
        static void TestSagePostMethods()
        {
            string requeststr = "{json-request-string}";

            // Create JSON document request
            CreateDocumentRequest request = JsonConvert.DeserializeObject<CreateDocumentRequest>(requeststr);

            /// POST CODE ====================================

            // Get Sage HH2 Connection
            SageHH2Connection connection = GetSageConnection();
            string docid = "";
            DateTime start = DateTime.UtcNow;
            try
            {
                // Post document to HH2
                docid = connection.WriteDocument(request);

                // Check for errors from posting
                connection.ThrowExceptionsForDocument(docid, start);

                // Export from HH2 to Sage
                string operationid = connection.ExportDocument(docid);

                // Wait for opration to complete
                connection.WaitForHH2Operation(operationid, 200);

                // Check for errors from export
                connection.ThrowExceptionsForDocument(docid, start);
            }
            catch
            {
                if (!string.IsNullOrWhiteSpace(docid))
                {
                    start = DateTime.UtcNow;
                    // Get corrected category and update document
                    UpdateDocumentRequest updaterequest = new UpdateDocumentRequest()
                    {
                        Snapshot = request.Snapshot,
                        ActionLevel = 1,
                        UpdatedOn = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
                    };
                    updaterequest.Snapshot.Distributions[0].CategoryId = "category-id";
                    connection.UpdateDocument(updaterequest, docid);
                    // Check for errors from export
                    connection.ThrowExceptionsForDocument(docid, start);
                }
                else
                {
                    throw;
                }
            }
        }

        #endregion

        #region Kingstone Tests

        /// <summary>
        /// Create and return kingstone connection
        /// </summary>
        /// <returns></returns>
        static KingstoneConnection GetKingstoneConnection()
        {
            // Create base connection
            KingstoneConnection connection = new KingstoneConnection();
            // Connect and return
            connection.Connect();
            return connection;
        }

        /// <summary>
        /// Test get methods for kingstone
        /// </summary>
        static void TestKingstoneGetMethods()
        {
            KingstoneConnection connection = GetKingstoneConnection();
            List<Policy> policies = connection.GetAllPolicies();
            ;
        }

        /// <summary>
        /// Test post methods for kingstone
        /// </summary>
        static void TestKingstonePostMethods()
        {
            KingstoneConnection connection = GetKingstoneConnection();
            Kingstone.Core.Payment payment = new Kingstone.Core.Payment();
            string paymentid = connection.PostPayment(payment);
            Console.WriteLine(paymentid);
        }

        #endregion

        #region Imaging Tests

        /// <summary>
        /// Test converting html to pdf
        /// </summary>
        static void TestHtmlToPDF()
        {
            /*
             * https://selectpdf.com/docs/PageBreaks.htm
             * Avoid page breaks in the middle of element groups by using the following style:
             * <div style="page-break-inside: avoid">
             * The content of this div element will not be split into several pages (if possible) because "page-break-inside" property is set to "avoid".
             * </div>
             * 
             * NOTE: CSS style needs to be inline for the elements to render correctly
             */
            HtmlToPdf converter = new HtmlToPdf();
            converter.Author = "Author";
            converter.Title = "My PDF";
            converter.Subject = "PDF Detail";
            converter.ColorModel = ColorModel.RGB;
            string inHtmlPath = desktopdir + "sample.html";
            string outPdfPath = desktopdir + "sample.pdf";
            if (File.Exists(outPdfPath)) { File.Delete(outPdfPath); }
            converter.FileToPDFFile(inHtmlPath, outPdfPath);
        }

        /// <summary>
        /// Test converting text to pdf
        /// </summary>
        static void TestTextToPDF()
        {
            // Test with basic fitting
            /*
            string srcfile = @"<path-to-file>.log";
            string destfile = @"<path-to-file>.pdf";
            TextToPDF converter = new TextToPDF();
            converter.AutoFitFileToPDFFile(srcfile, destfile);
            */
            // Test with advanced fitting
            TextToPDF converter = new TextToPDF();
            string srcfile = @"<path-to-file>.las";
            string destfile = @"<path-to-file>.pdf";
            converter.AllowedPageTypes = new PageType[] { PageType.Letter, PageType.A1 };
            converter.AutoFitFileToPDFFile(srcfile, destfile);
        }

        /// <summary>
        /// Test converting text to png
        /// </summary>
        static void TestTextToPNG()
        {
            // Test with basic fitting
            string srcfile = @"<path-to-file>.log";
            string destfile = @"<path-to-file>.png";
            TextToImage converter = new TextToImage();
            converter.WriteFileToBitmapFile(srcfile, destfile);
            // Test with jpeg
            srcfile = @"<path-to-file>.LAS";
            destfile = @"<path-to-file>.png";
            converter.ImageFormat = ImageFormat.Jpeg;
            converter.WriteFileToBitmapFile(srcfile, destfile);
        }

        /// <summary>
        /// Test converting pdf to tiff
        /// </summary>
        static void TestPDFToTiff()
        {
            string pdfFile = @"<path-to-file>.pdf";
            string tiffFile = @"<path-to-file>.tiff";
            PDFToImage converter = new PDFToImage
            {
                BlackAndWhite = true,
                ResultDPI = 300
            };
            converter.PDFToTiffFile(pdfFile, tiffFile);
        }

        #endregion

        #region FC2DW Tests

        /// <summary>
        /// Test the DocuWare search and doc display
        /// </summary>
        static void TestDWSearch()
        {
            FileLogger logger = new FileLogger("DW Search", @"C:\ProgramData\AbbyyAddins\", "LogFile") { LogLevel = LogLevel.WARN };
            // Create setings for searching document
            DWSearchSettings settings = new DWSearchSettings()
            {
                Credentials = new GeneralCredentials()
                {
                    Host = "https://your-dw-instance",
                    Username = "un",
                    Password = new AESValue("pw", AESEncryption.CreateRandomBase64Key())
                },
                FileCabinetGuid = "cabinet-guid",
                SearchValues = new NamedValues(new Dictionary<string, string>()
                {
                    { "search_field", "search_value" }
                })
            };
            ResultBase result = settings.LookupDWDocument(logger);
            if (!result.Succeeded)
            {
                Console.WriteLine(result.Message);
            }
        }

        #endregion

        #region FC2DW Sample Scripts

        /// <summary>
        /// Sample upload to DW script
        /// </summary>
        static void UploadFromFCToDWSample()
        {
            /*
            using System;
            using System.Collections.Generic;
            using Corely.Logging;
            using Corely.FC2DW;

            // Create file logger for writing to file
            FileLogger logger = new FileLogger("DocuWare Export", @"C:\ProgramData\AbbyyAddins\", "ExportLog") { LogLevel = LogLevel.WARN };
            try
            {
                Exporter settings = new Exporter()
                {
                    DWServer = "https://TBD.docuware.cloud",
                    DWUsername = "un",
                    DWPassword = "pw",
                    DWFCID = "TBD",
                    Logger = logger
                };


                // Create header field mappings
                settings.HeaderFieldMapping = new Dictionary<string, string>();
                settings.HeaderFieldMapping.Add("VENDOR_ID", @"Vendor\VendorId");
                settings.HeaderFieldMapping.Add("VENDOR_NAME", @"Vendor\Name");
                settings.HeaderFieldMapping.Add("STORE__", @"StoreNumber");
                settings.HeaderFieldMapping.Add("INVOICE_NUMBER", @"InvoiceNumber");
                settings.HeaderFieldMapping.Add("TOTAL", @"Total");
                settings.HeaderFieldMapping.Add("INVOICE_DUE_DATE", @"InvoiceData\DueDate");
                settings.HeaderFieldMapping.Add("PURCHASE_ORDER_NUMBER", @"InvoiceData\PurchaserName");
                settings.HeaderFieldMapping.Add("INVOICE_DATE", @"InvoiceDate");
                settings.HeaderFieldMapping.Add("WORKFLOW_NAME", @"Workflow");
                settings.HeaderFieldMapping.Add("TAXES", @"SalesTax");
                // Create keyword mapping 
                settings.ColumnToKeywordMapping = new List<ColumnToKeywordMapping>()
                {
                    new ColumnToKeywordMapping()
                    {
                        FlexiTableColumn = "",
                        FlexiTableName = "",
                        KeywordFieldName = ""
                    }
                };
                // Create fixed keyword
                settings.FixedKeywordMappings = new List<FixedKeywordMapping>()
                {
                    new FixedKeywordMapping()
                    {
                        KeywordFieldName = "",
                        Keywords = new List<string>() { "" }
                    }
                };
                // Create header fixed fields
                settings.HeaderFixedFields = new Dictionary<string, string>();
                settings.HeaderFixedFields.Add("DOCUMENT_TYPE", "Invoice");
                settings.HeaderFixedFields.Add("STATUS", "New");
                // Create table field mappings
                settings.TableMapping = new TableMapping();
                settings.TableMapping.FlexiTableName = "LineItems";
                settings.TableMapping.DWTableName = "GL_Code";
                settings.TableMapping.MappedColumns = new Dictionary<string, string>();
                settings.TableMapping.MappedColumns.Add("GL_CO_AMOUNT", @"TotalPriceNetto");
                settings.TableMapping.MappedColumns.Add("GL_CO_MAIN_GL_ACCOUNT_", @"GLCode");
                settings.TableMapping.MappedColumns.Add("GL_CO_STORE_NUMBER", @"CostCenter");
                settings.TableMapping.MappedColumns.Add("GL_CO_SALES_TAX", @"SalesTax");
                settings.TableMapping.MappedColumns.Add("GL_CO_COMMENTS1", @"Comment");
                // Create fixed table mappings
                settings.FixedTableMapping = new FixedTableMapping();
                settings.FixedTableMapping.DWTableName = "GL_Code";
                Dictionary<string, string> row = new Dictionary<string, string>();
                row.Add("dwcol1", "val1");
                row.Add("dwcol2", "val2");
                settings.FixedTableMapping.FixedRowColumnMapping.Add(row);
                // Create image saving options
                IExportImageSavingOptions options = FCTools.NewImageSavingOptions();
                options.Format = "pdf-s";
                options.UseMRC = true;
                options.ShouldOverwrite = true;
                // Map export to web service
                settings.UploadDocumentToServiceOverride += (postBody) =>
                {
                    // custom post logic here
                };
                // Run the export
                settings.ExportDocument(Document, options);
            }
            catch (Exception ex)
            {
                // Log exception to file and throw
                logger.WriteLog("Failed to execute script", ex, LogLevel.ERROR);
                throw;
            }
            */
        }

        /// <summary>
        /// Sample search DW script
        /// </summary>
        static void SearchDWFromFCSample()
        {
            /*
             * Use this script as a baseline for a custom button event for DocuWare searching
             *
            using System;
            using System.Collections.Generic;
            using Corely.Logging;
            using Corely.FC2DW;
            using Corely.Security;
            using Corely.Security.Authentication;

            // Make sure this command is the one for DocuWare searches
            if((ABBYY.FlexiCapture.ClientUI.TCommandID)CommandId == (ABBYY.FlexiCapture.ClientUI.TCommandID)1001)
            {
                // Make sure a document is currently open in the editor
                if(MainWindow.TaskWindow == null ||
                    MainWindow.TaskWindow.EditorWindow == null ||
                    MainWindow.TaskWindow.EditorWindow.Document == null)
                {
                    FCTools.ShowMessage("Open document first");
                }
                else
                {
                    // Create file logger for dw search
                    FileLogger logger = new FileLogger("DW Search", @"C:\ProgramData\AbbyyAddins\", "LogFile") { LogLevel = LogLevel.DEBUG };
                    try
                    {
                        // Create setings for searching document
                        DWSearchSettings settings = new DWSearchSettings()
                        {
                            Credentials = new GeneralCredentials()
                            {
                                Host = "https://yourplatform/DocuWare/Platform",
                                Username = "un",
                                Password = new AESValue("pw", AESEncryption.CreateRandomBase64Key())
                            },
                            FileCabinetGuid = "fcid",
                            SearchValues = new NamedValues(new Dictionary<string, string>()
                            {
                                { "field", "value" }
                            })
                        };
                        ResultBase result = settings.LookupDWDocument(logger);
                        // Check result from split. Show message if necessary
                        if(!result.Succeeded)
                        {
                            FCTools.ShowMessage(result.Message);
                            logger.WriteLog("DW search cancelled", "", LogLevel.WARN);
                        }
                    }
                    catch(Exception ex)
                    {
                        FCTools.ShowMessage("Error: " + ex.Message);
                        logger.WriteLog("Error searching", ex, LogLevel.ERROR);
                    }
                }
            }
            */
        }

        #endregion

        #region FlexiCapture Sample Scripts

        /// <summary>
        /// Line Split code for FC
        /// </summary>
        static void FlexicaptureLineSplitCode()
        {
            /*
             * Use this script as a baseline for a custom button event for line splits
             *
            using System;
            using System.Collections.Generic;
            using Corely.Core;
            using Corely.Distribution;
            using Corely.Logging;
            using Corely.FlexiCapture;
            using Corely.Data.Serialization;

            // Make sure this command is the one for line splits 
            if((ABBYY.FlexiCapture.ClientUI.TCommandID)CommandId == (ABBYY.FlexiCapture.ClientUI.TCommandID)1000)
            {
                // Make sure a document is currently open in the editor
                if(MainWindow.TaskWindow == null ||
                    MainWindow.TaskWindow.EditorWindow == null ||
                    MainWindow.TaskWindow.EditorWindow.Document == null)
                {
                    Message.Show("Open document first");
                }
                else
                {
                    // Save the document before proceeding
                    MainWindow.TaskWindow.EditorWindow.Save();
                    IDocument Document = MainWindow.TaskWindow.EditorWindow.Document;
                    MainWindow.TaskWindow.OpenDocument(Document);
                    // Create file logger for line splitting
                    FileLogger logger = new FileLogger("Line Split", @"C:\ProgramData\AbbyyAddins\", "LineSplitLog") { LogLevel = LogLevel.WARN };
                    try
                    {
                        // Create settings for splitting 
                        LineSplitSettings settings = new LineSplitSettings()
                        {
                            CopyFields = new List<string>() { "ArticleNumber", "Description", "MaterialNumber", "Store", "CostCenter", "UnitOfMeasurement", "UnitPrice", "Currency", "OrderItemId"},
                            SplitCheckField = "Split",
                            TotalPriceField = "TotalPriceNetto",
                            UnitPriceField = "UnitPrice",
                            QuantityField = "Quantity",
                            DistributionInsertAt = DistributionInsertAt.Current,
                            SplitOnTotal = true,
                            DistributionSettings = new DistributionSettings()
                        };
                        // Custom action to run on each line
                        Action<int, int> splitLineAction = (cursplit, insertat) => 
                        {
                            Document.Field("Invoice Layout\\LineItems").Items[insertat].Field("SplitXml").Text = XmlSerializer.Serialize(settings.DistributionSettings);
                            Document.Field("Invoice Layout\\LineItems").Items[cursplit].Field("LineItemUID").CheckRules();
                            string parentid = Document.Field("Invoice Layout\\LineItems").Items[cursplit].Field("LineItemUID").Text;
                            Document.Field("Invoice Layout\\LineItems").Items[insertat].Field("SplitParentUID").Text = parentid;
                        };
                        // Call method to perform split
                        ResultBase result = settings.ShowDistributionSettingsFromProjectAndSplit("lineSplitDistributionSettings", Document, logger, splitLineAction);
                        // Display result errors if any exist
                        result.DisplayErrors();
                    }
                    catch (Exception ex)
                    {
                        Message.Show("Error: " + ex.Message);
                        logger.WriteLog("FlexiCapture script error splitting Lines", ex, LogLevel.ERROR);
                    }
                }
            }
            */
        }

        #endregion

        #region DocuWare Tests

        /// <summary>
        /// Test docuware field conversion and serialization
        /// </summary>
        static void TestDWFieldConversion()
        {
            byte[] docBytes = new byte[100];
            string mimeType = "application/pdf";
            List<DocumentIndexField> newFields = new List<DocumentIndexField>()
                {
                    DocumentIndexField.Create("TEXT", "ASDF"),
                    DocumentIndexField.Create("DATE", DateTime.Now),
                    DocumentIndexField.Create("DECIMAL", 1.0m),
                    DocumentIndexField.Create("INT", 100),
                    DocumentIndexField.Create("TEXT", new DocumentIndexFieldKeywords() { Keyword = new List<string>() { "Value1", "Value2" } })
                };
            DocumentIndexFieldTable table = new DocumentIndexFieldTable();
            table.Row = new List<DocumentIndexFieldTableRow>();
            for (int i = 1; i < 4; i++)
            {
                DocumentIndexFieldTableRow row = new DocumentIndexFieldTableRow();
                row.ColumnValue = new List<DocumentIndexField>();
                row.ColumnValue.Add(DocumentIndexField.Create($"TEXT{i}", $"ASDF{i}"));
                row.ColumnValue.Add(DocumentIndexField.Create($"DATE{i}", DateTime.Now.AddDays(i)));
                row.ColumnValue.Add(DocumentIndexField.Create($"DECIMAL{i}", (decimal)i));
                table.Row.Add(row);
            }
            newFields.Add(new DocumentIndexField() { FieldName = "TABLE", ItemElementName = ItemChoiceType.Table, Item = table });
            DocumentData documentData = new DocumentData(docBytes, mimeType, newFields);
            //DocumentData documentData2 = DocumentData.FromJsonString(documentData.ToJsonString());
            DocumentData documentData2 = DocumentData.FromXmlString(documentData.ToXmlString());
            List<DocumentIndexField> newField2 = documentData2.GetDWFields();
            ;
        }

        #endregion

        #region LibPostal Tests

        /// <summary>
        /// Test lib postal client
        /// </summary>
        static void TestLibPostalClient()
        {
            LibPostalServiceResponse response = LibPostalClient.LibPostalClient.ParseAddress("Av. Beira Mar 1647 - Salgueiros, 4400-382 Vila Nova de Gaia");
            if (response.Status == 200 && response.ParsedAddress != null)
            {
                foreach (var pair in response.ParsedAddress)
                {
                    Console.WriteLine($"{pair.Name}:{pair.Value}");
                }
            }
            else
            {
                Console.WriteLine($"{response.Status} : {response.Message}");
            }
        }

        /// <summary>
        /// Test lib postal net library
        /// </summary>
        static void TestLibPostalNet()
        {
            try
            {
                // NOTE: This will only run in x64 applications because the postal.dll C++ wrapper library is compiled in x64. View the README.md in the LibPostalNet project for more information
                // Initialize components with data directory
                string dataPath = @"C:\LibPostal\libpostal";
                if (!Directory.Exists(dataPath)) { throw new Exception("LibPostal data does not exist in directory " + dataPath); }
                libpostal.LibpostalSetupDatadir(dataPath);
                libpostal.LibpostalSetupParserDatadir(dataPath);
                libpostal.LibpostalSetupLanguageClassifierDatadir(dataPath);

                // Create query address
                string query = "Av. Beira Mar 1647 - Salgueiros, 4400-382 Vila Nova de Gaia";
                Console.WriteLine($"Parsing started for [{query}]");

                // Parse the address
                LibpostalAddressParserResponse response = null;
                try
                {
                    LibpostalAddressParserOptions parseroptions = libpostal.LibpostalGetAddressParserDefaultOptions();
                    response = libpostal.LibpostalParseAddress(query, parseroptions);
                    if (response != null)
                    {
                        foreach (KeyValuePair<string, string> result in response.Results)
                        {
                            Console.WriteLine(result.ToString());
                        }
                    }
                    else
                    {
                        Console.WriteLine("Parse address returned null");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Parse address failed{Environment.NewLine}{ex}");
                }
                finally
                {
                    libpostal.LibpostalAddressParserResponseDestroy(response);
                }

                // Normalize the address
                Console.WriteLine($"Expansion started for [{query}]");
                LibpostalNormalizeOptions normalizeoptions = libpostal.LibpostalGetDefaultOptions();
                normalizeoptions.AddressComponents = LibpostalNormalizeOptions.LIBPOSTAL_ADDRESS_ALL;
                LibpostalNormalizeResponse expansion = libpostal.LibpostalExpandAddress(query, normalizeoptions);
                if (expansion != null)
                {
                    foreach (string s in expansion.Expansions)
                    {
                        Console.WriteLine(s);
                    }
                }
                else
                {
                    Console.WriteLine("Expand address returned null");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LibPostal Exceptions:{Environment.NewLine}{ex}");
            }
            finally
            {
                // Tear down the components
                try { libpostal.LibpostalTeardown(); } catch { Console.WriteLine("Failed to tear down lib postal"); }
                try { libpostal.LibpostalTeardownParser(); } catch { Console.WriteLine("Failed to tear down lib postal parser"); }
                try { libpostal.LibpostalTeardownLanguageClassifier(); } catch { Console.WriteLine("Failed to tear down lib postal language classifier"); }
            }
        }

        #endregion

    }
}
