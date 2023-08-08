using Corely.Data.Culture;
using Corely.Data.Dates;
using Corely.Data.Delimited;
using Corely.Data.Serialization;
using Corely.Distribution;
using Corely.FTP;
using Corely.Helpers;
using Corely.Imaging.Converters;
using Corely.Imaging.Core;
using Corely.Logging;
using Corely.Security;
using Corely.Security.Authentication;
using Corely.UI;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;

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

    }
}
