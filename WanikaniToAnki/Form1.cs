﻿using Newtonsoft.Json;
using QuickType;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace WanikaniToAnki
{
    public partial class WanikaniToAnki : Form
    {
        int newkanji = 0;
        int newvocs = 0;
        static readonly string jisho = "http://classic.jisho.org/";
        bool loadKanji = false;
        bool loadVoks = false;
        bool loadImgs = false;
        Collection<char> fetchedKanjiDiagrams;
        Collection<char> addedKanji;
        Dictionary<long, KanjiOrVoc> kanji;
        Dictionary<long, KanjiOrVoc> voc;
        string token = "";
        static int requestcounter = 59;
        static int timeuntilreset = 60;
        static System.Timers.Timer aTimer;
        Thread workerthread;
        string imgPath;
        string[] configlines;

        private static void SetTimer()
        {
            // Create a timer with a one second interval.
            aTimer = new System.Timers.Timer(1000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            
        }

        private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            if (timeuntilreset < 1)
            {
                requestcounter = 59;
                timeuntilreset = 60;
            }
            else
            {
                timeuntilreset--;
            }
        }
        enum t
        {
            assignement, subject
        }
        class KanjiOrVoc
        {
            public string reading;
            public string meaning;
            public Collection<string> meanings;
            public Collection<string> readings;
            public string characters;
            public string imgName = "";

            public KanjiOrVoc(string meaning, string reading, Meaning[] me, Reading[] re, string chars)
            {

                this.meaning = meaning;
                this.reading = reading;
                characters = chars;
                meanings = new Collection<string>();
                readings = new Collection<string>();
                foreach (Meaning mea in me)
                {
                    meanings.Add(mea.meaning);
                }
                meanings.Remove(this.meaning);
                foreach (Reading rea in re)
                {
                    readings.Add(rea.reading);
                }
                readings.Remove(this.reading);
            }

            public KanjiOrVoc(string meaning, string reading, Meaning2[] me, Reading2[] re, string chars)
            {
                this.meaning = meaning;
                this.reading = reading;
                characters = chars;
                meanings = new Collection<string>();
                readings = new Collection<string>();
                foreach (Meaning2 mea in me)
                {
                    meanings.Add(mea.meaning);
                }
                meanings.Remove(this.meaning);
                foreach (Reading2 rea in re)
                {
                    readings.Add(rea.reading);
                }
                readings.Remove(this.reading);
            }

            public KanjiOrVoc(string meaning, string reading, string[] meanings, string[] readings, string characters, string imgName)
            {
                this.meanings = new Collection<string>();
                this.readings = new Collection<string>();
                this.reading = reading;
                this.meaning = meaning;
                foreach (string mea in meanings)
                {
                    this.meanings.Add(mea);
                }
                foreach (string rea in readings)
                {
                    this.readings.Add(rea);
                }
                this.characters = characters;
                this.imgName = imgName;
            }
        }

        string buildCSV(Dictionary<long, KanjiOrVoc> a)
        {
            string res = "";
            int counter = 0;
            foreach (KeyValuePair<long, KanjiOrVoc> b in a)
            {
                counter++;
                label3.Invoke((MethodInvoker)(() => label3.Text = "line :" + counter));
                if (b.Value.characters.Length < 1 || b.Value.meaning.Length < 1 || b.Value.reading.Length < 1 || b.Value.imgName.Length < 1)
                {
                    Console.WriteLine("check line " + counter);
                }
                res += b.Key + "\t" + b.Value.characters + "\t" + b.Value.meaning + "\t";
                foreach (string m in b.Value.meanings)
                {
                    res += m + ", ";
                }
                if (res.EndsWith(", "))
                {
                    res = res.Remove(res.Length - 2);
                }
                res += "\t" + b.Value.reading + "\t";
                foreach (string m in b.Value.readings)
                {
                    res += m + ", ";
                }
                if (res.EndsWith(", "))
                {
                    res = res.Remove(res.Length - 2);
                }
                res += "\t";
                res += b.Value.imgName;
                res += "\n";
            }
            return res.Remove(res.Length - 1);
        }

        public WanikaniToAnki()
        {
            InitializeComponent();
            FileStream fs;
            StreamWriter wr;
            TextWriter tw = Console.Out;
            try
            {
                fs = new FileStream("log.txt", FileMode.OpenOrCreate, FileAccess.Write);
                wr = new StreamWriter(fs);
                wr.AutoFlush = true;
                Console.SetOut(wr);
            } catch (Exception e)
            {

            }
            
            this.Shown += new System.EventHandler(this.Form1_Shown);

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            SetTimer();
            Directory.CreateDirectory("img");
            Directory.CreateDirectory("csv");
            configlines = new string[] { "", "", "" };
            label2.Text = "ready";
            label3.Text = "ready";
            ToolTip toolTip1 = new ToolTip();
            toolTip1.ShowAlways = true;
            toolTip1.SetToolTip(useCsvsCheckbox, "if checked, will fetch all kanji and vocabulary from wanikani,\n ignoring the already downloaded data in the genereated csv files.\n will take more time depending on the amount of unlocked lessons.");
            /*if (File.Exists(Directory.GetCurrentDirectory() + "/apitoken.txt"))
            {
                ApiToken.Text = File.ReadAllText(Directory.GetCurrentDirectory() + "/apitoken.txt");
                saveTokenCheckbox.Checked = true;
            }
            if (File.Exists(Directory.GetCurrentDirectory() + "/imgpath.txt"))
            {
                imgPath = File.ReadAllText(Directory.GetCurrentDirectory() + "/imgpath.txt");
                textBox3.Text = imgPath;
            }
            else
            {
                imgPath = Directory.GetCurrentDirectory() + "\\img\\";
                textBox3.Text = imgPath;
            }
            if (File.Exists(Directory.GetCurrentDirectory() + "/initiate.txt"))
            {
                string initiate = File.ReadAllText(Directory.GetCurrentDirectory() + "/initiate.txt");
                includeAssignementsCheckbox.Checked = Boolean.Parse(initiate);
            }*/
            if (File.Exists(Directory.GetCurrentDirectory() + "/config.txt"))
            {
                configlines = File.ReadAllText(Directory.GetCurrentDirectory() + "/config.txt").Split(new string[] { "\n" }, StringSplitOptions.None);
            }
            else
            {
                makeDefaultConfigString();
                writeConfigToFile();
            }
            token = configlines[0];
            if (token.Length>0)
            {
                saveTokenCheckbox.Checked = true;
            }
            ApiToken.Text = token;
            imgPath = configlines[1];
            textBox3.Text = imgPath;
            includeAssignementsCheckbox.Checked = Boolean.Parse(configlines[2]);
        }
        void makeDefaultConfigString()
        {
            configlines = new string[] { "", Directory.GetCurrentDirectory() + "\\img\\", "False"};
        }

        void writeConfigToFile()
        {
            File.WriteAllText(Directory.GetCurrentDirectory() + "/config.txt", getConfigString());
        }

        string getConfigString()
        {
            string ret = "";
            foreach(string a in configlines)
            {
                ret += a + "\n";
            }
            return ret.Remove(ret.Length-1);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {

            if (workerthread != null)
            {
                workerthread.Abort();
            }
            base.OnFormClosing(e);


        }
        void init()
        {
            loadImgs = false;
            loadKanji = false;
            loadVoks = false;
            newkanji = 0;
            newvocs = 0;
            kanji = new Dictionary<long, KanjiOrVoc>();
            voc = new Dictionary<long, KanjiOrVoc>();
            addedKanji = new Collection<char>();
            fetchedKanjiDiagrams = new Collection<char>();
            if (File.Exists(Directory.GetCurrentDirectory() + "/csv/Kanji.csv") && !useCsvsCheckbox.Checked)
            {
                loadKanji = true;
            }
            if (File.Exists(Directory.GetCurrentDirectory() + "/csv/Vokabeln.csv") && !useCsvsCheckbox.Checked)
            {
                loadVoks = true;
            }
            if (!CheckDirectoryEmpty_Fast(imgPath))
            {
                loadImgs = true;
            }
            loadFromFiles();
        }
        private void loadFromFiles()
        {
            if (loadKanji)
            {
                Console.WriteLine("loading Kanji");
                label2.Invoke((MethodInvoker)(() => label2.Text = "loading Kanji from File"));
                label3.Invoke((MethodInvoker)(() => label3.Text = "starting"));
                string[] lines = File.ReadAllText(Directory.GetCurrentDirectory() + "/csv/Kanji.csv").Split(new string[] { "\n" }, StringSplitOptions.None);
                int counter = 1;
                foreach (string line in lines)
                {
                    label3.Invoke((MethodInvoker)(() => label3.Text = "current line: " + counter));
                    string[] fields = line.Split(new string[] { "\t" }, StringSplitOptions.None);
                    string[] meanings = fields[3].Split(new string[] { ", " }, StringSplitOptions.None);
                    string[] readings = fields[5].Split(new string[] { ", " }, StringSplitOptions.None);
                    kanji.Add(long.Parse(fields[0].Trim()), new KanjiOrVoc(fields[2], fields[4], meanings, readings, fields[1], fields[6]));
                    counter++;
                }
                label3.Invoke((MethodInvoker)(() => label3.Text = "done"));
                Console.WriteLine("loaded Kanji");
            }
            if (loadVoks)
            {
                Console.WriteLine("loading Voks");
                label2.Invoke((MethodInvoker)(() => label2.Text = "loading Voks from File"));
                label3.Invoke((MethodInvoker)(() => label3.Text = "starting"));
                string[] lines = File.ReadAllText(Directory.GetCurrentDirectory() + "/csv/Vokabeln.csv").Split(new string[] { "\n" }, StringSplitOptions.None);
                int counter = 1;
                foreach (string line in lines)
                {
                    label3.Invoke((MethodInvoker)(() => label3.Text = "current line: " + counter));
                    string[] fields = line.Split(new string[] { "\t" }, StringSplitOptions.None);
                    string[] meanings = fields[3].Split(new string[] { ", " }, StringSplitOptions.None);
                    string[] readings = fields[5].Split(new string[] { ", " }, StringSplitOptions.None);
                    voc.Add(long.Parse(fields[0].Trim()), new KanjiOrVoc(fields[2], fields[4], meanings, readings, fields[1], ""));
                    counter++;
                }
                label3.Invoke((MethodInvoker)(() => label3.Text = "done"));
                Console.WriteLine("loaded Voks");
            }

            if (loadImgs)
            {
                Console.WriteLine("loading imgs");
                string[] fileArray = Directory.GetFiles(imgPath, "*.png");
                foreach (string a in fileArray)
                {
                    Console.WriteLine("loaded "+ Path.GetFileName(a));
                    fetchedKanjiDiagrams.Add(Path.GetFileName(a).Remove(Path.GetFileName(a).Length - 4)[0]);
                }
                Console.WriteLine("loaded imgs");
            }
            label2.Invoke((MethodInvoker)(() => label2.Text = "Initializing done"));
            //button1.Invoke((MethodInvoker)(() => button1.Enabled = true));
        }

        string[] listAnkiProfiles()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            string res = "";
            foreach (DriveInfo d in allDrives)
            {
                res += DirSearch(d.Name, "Anki2");
            }
            return res.Remove(res.Length- "SplitString1234555".Length).Split(new string[] { "SplitString1234555" }, StringSplitOptions.None);
        }

        string DirSearch(string sDir, string dirname)
        {
            string res = "";
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    if (d.Contains(dirname))
                    {
                        res += d + "SplitString1234555";
                    }
                    res +=DirSearch(d,dirname);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
            return res;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            //Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            
            //C:\Users\Tazua\AppData\Roaming\Anki2
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            saveTokenCheckbox.Enabled = false;
            useCsvsCheckbox.Enabled = false;
            includeAssignementsCheckbox.Enabled = false;
            label2.Text = "initializing";
            Match m = Regex.Match(ApiToken.Text, "[a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12}");
            if (m.Success)
            {
                token = m.Value;
                if (saveTokenCheckbox.Checked)
                {
                    configlines[0] = token;
                    writeConfigToFile();
                    //File.WriteAllText(Directory.GetCurrentDirectory() + "/apitoken.txt", token);
                }
            }
            else
            {
                label2.Text = "invalid token";
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
                saveTokenCheckbox.Enabled = true;
                useCsvsCheckbox.Enabled = true;
                includeAssignementsCheckbox.Enabled = true;
                return;
            }
            textBox1.Clear();
            textBox2.Clear();
            workerthread = new Thread(apistuff);
            workerthread.Start();
        }

        void apistuff()
        {
            init();
            aTimer.Enabled = true;
            try
            {
                Console.WriteLine("starting first request");
                WebResponse resp = fireRequest(t.assignement, null);
                Stream stream = resp.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                String responseString = reader.ReadToEnd();
                Assignements assign = JsonConvert.DeserializeObject<Assignements>(responseString);
                Console.WriteLine("assignments: "+ assign.TotalCount);
                int numberofkanji = 0;
                bool first = true;
                while (assign.Pages.NextUrl != null || first)
                {
                    if (!first)
                    {
                        resp = fireRequest(assign.Pages.NextUrl);
                        stream = resp.GetResponseStream();
                        reader = new StreamReader(stream, Encoding.UTF8);
                        responseString = reader.ReadToEnd();
                        assign = JsonConvert.DeserializeObject<Assignements>(responseString);
                    }
                    first = false;
                    foreach (Datum2 a in assign.Data)
                    {
                        if (a.Data.SrsStage > 0 || includeAssignementsCheckbox.Checked)
                        {
                            if (a.Data.SubjectType == "kanji")
                            {
                                if (!kanji.ContainsKey(a.Data.SubjectId))
                                {
                                    resp = fireRequest(t.subject, a.Data.SubjectId);
                                }
                                else
                                {
                                    Console.WriteLine("double, not requesting");
                                    textBox1.Invoke((MethodInvoker)(() => textBox1.Text += kanji[a.Data.SubjectId].characters + ", "));
                                    addedKanji.Add(kanji[a.Data.SubjectId].characters[0]);
                                    continue;
                                }

                                if (resp == null) return;
                                Console.WriteLine(((HttpWebResponse)resp).StatusCode + ": Kan");
                                stream = resp.GetResponseStream();
                                reader = new StreamReader(stream, Encoding.UTF8);
                                String responseString2 = reader.ReadToEnd();
                                KanjiJSON kan = JsonConvert.DeserializeObject<KanjiJSON>(responseString2);
                                string me = "";
                                string re = "";
                                foreach (Meaning m in kan.data.meanings)
                                {
                                    if (m.primary) me = m.meaning;
                                    break;
                                }
                                foreach (Reading r in kan.data.readings)
                                {
                                    if (r.primary) re = r.reading;
                                    break;
                                }
                                if (!kanji.ContainsKey(a.Data.SubjectId))
                                {
                                    KanjiOrVoc kov = new KanjiOrVoc(me, re, kan.data.meanings, kan.data.readings, kan.data.characters);
                                    kanji.Add(a.Data.SubjectId, kov);
                                    newkanji++;
                                    addedKanji.Add(kov.characters[0]);
                                    label2.Invoke((MethodInvoker)(() => label2.Text = "added to collection: " + kov.characters));
                                    numberofkanji++;
                                }
                                else
                                {
                                    Console.WriteLine("DOUBLE : " + a.Data.SubjectId);
                                }
                                textBox1.Invoke((MethodInvoker)(() => textBox1.AppendText(kanji[a.Data.SubjectId].characters + ", ")));

                            }
                            else if (a.Data.SubjectType == "vocabulary")
                            {
                                if (!voc.ContainsKey(a.Data.SubjectId))
                                {
                                    resp = fireRequest(t.subject, a.Data.SubjectId);
                                }
                                else
                                {
                                    Console.WriteLine("double, not requesting");
                                    textBox2.Invoke((MethodInvoker)(() => textBox2.Text += voc[a.Data.SubjectId].characters + ", "));
                                    continue;
                                }
                                if (resp == null) return;
                                Console.WriteLine(((HttpWebResponse)resp).StatusCode + ": Vok");
                                stream = resp.GetResponseStream();
                                reader = new StreamReader(stream, Encoding.UTF8);
                                String responseString2 = reader.ReadToEnd();
                                VocabJSON vocab = JsonConvert.DeserializeObject<VocabJSON>(responseString2);
                                string me = "";
                                string re = "";
                                foreach (Meaning2 m in vocab.data.meanings)
                                {
                                    if (m.primary) me = m.meaning;
                                    break;
                                }
                                foreach (Reading2 r in vocab.data.readings)
                                {
                                    if (r.primary) re = r.reading;
                                    break;
                                }
                                if (!voc.ContainsKey(a.Data.SubjectId))
                                {
                                    KanjiOrVoc kov = new KanjiOrVoc(me, re, vocab.data.meanings, vocab.data.readings, vocab.data.characters);
                                    voc.Add(a.Data.SubjectId, kov);
                                    newvocs++;
                                    label2.Invoke((MethodInvoker)(() => label2.Text = "added to collection: " + kov.characters));
                                }
                                else {
                                    Console.WriteLine("DOUBLE " + vocab.data.characters);
                                }
                                textBox2.Invoke((MethodInvoker)(() => textBox2.AppendText(voc[a.Data.SubjectId].characters + ", ")));
                            }
                        }
                    }
                }
                Console.WriteLine("kanji added: " + numberofkanji);
                reader.Dispose();
                stream.Close();
                label2.Invoke((MethodInvoker)(() => label2.Text = "downloading images"));
                foreach (KeyValuePair<long, KanjiOrVoc> a in kanji)
                {
                    foreach (char c in a.Value.characters)
                    {
                        if (!fetchedKanjiDiagrams.Contains(c))
                        {
                            label3.Invoke((MethodInvoker)(() => label3.Text = "current char: " + c));
                            HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create(jisho + "kanji/details/" + c);
                            resp = hwr.GetResponse();
                            Console.WriteLine("jisho.org: " + c + " - HTTP Response " + ((HttpWebResponse)resp).StatusCode);
                            stream = resp.GetResponseStream();
                            reader = new StreamReader(stream, Encoding.UTF8);
                            responseString = reader.ReadToEnd();
                            Match match = Regex.Match(responseString, "static/images/stroke_diagrams/[0-9]+_frames\\.png");

                            if (match.Success)
                            {
                                using (WebClient client = new WebClient())
                                {
                                    int filecount = Directory.GetFiles(Path.GetDirectoryName(imgPath)).Length;
                                    client.DownloadFile(new Uri(jisho + match.Value), imgPath + c + ".png");
                                    if (filecount == Directory.GetFiles(Path.GetDirectoryName(imgPath)).Length)
                                    {
                                        Console.WriteLine("no image downloaded for: " + c);
                                    }
                                    else
                                    {
                                        fetchedKanjiDiagrams.Add(c);
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("no image found on jisho for: " + c);
                            }
                        }
                        if (a.Value.imgName.Length < 2 && fetchedKanjiDiagrams.Contains(c))
                        {
                            a.Value.imgName = c + ".png";
                        }
                    }

                }
                reader.Dispose();
                stream.Close();
                label2.Invoke((MethodInvoker)(() => label2.Text = "writing Kanji.csv"));
                File.WriteAllText(Directory.GetCurrentDirectory() + "/csv/Kanji.csv", buildCSV(kanji));
                label2.Invoke((MethodInvoker)(() => label2.Text = "writing Vokabeln.csv"));
                File.WriteAllText(Directory.GetCurrentDirectory() + "/csv/Vokabeln.csv", buildCSV(voc));
                label2.Invoke((MethodInvoker)(() => label2.Text = "done. added " + newkanji + " new kanji and " + newvocs + " new vocabulary."));
                label3.Invoke((MethodInvoker)(() => label3.Text = "done"));
                button1.Invoke((MethodInvoker)(() => button1.Enabled = true));
                button2.Invoke((MethodInvoker)(() => button2.Enabled = true));
                button3.Invoke((MethodInvoker)(() => button3.Enabled = true));
                saveTokenCheckbox.Invoke((MethodInvoker)(() => saveTokenCheckbox.Enabled = true));
                useCsvsCheckbox.Invoke((MethodInvoker)(() => useCsvsCheckbox.Enabled = true));
                includeAssignementsCheckbox.Invoke((MethodInvoker)(() => includeAssignementsCheckbox.Enabled = true));
            }
            catch (Exception e)
            {

            }
        }

        WebResponse fireRequest(t type, long? id)
        {
            int i = 0;
            while (requestcounter < 1)
            {
                Thread.Sleep(10);

                if (i != timeuntilreset)
                {
                    label3.Invoke((MethodInvoker)(() => label3.Text = "rate limit reached. waiting for another " + timeuntilreset + " seconds."));
                    i = timeuntilreset;
                }

            }
            requestcounter--;
            label3.Invoke((MethodInvoker)(() => label3.Text = "remaining requests this minute: " + requestcounter + "/59"));
            switch (type)
            {
                case t.assignement:
                    HttpWebRequest assRequest = (HttpWebRequest)WebRequest.Create("https://api.wanikani.com/v2/assignments");
                    assRequest.Headers.Add("Authorization: Bearer " + token);
                    try
                    {
                        WebResponse resp = assRequest.GetResponse();
                        return resp;
                    }
                    catch (WebException e)
                    {
                        if (((HttpWebResponse)e.Response).StatusCode.Equals((HttpStatusCode)429))
                        {
                            Console.WriteLine("too many requests");
                            requestcounter = 0;
                            return fireRequest(type, id);
                        }
                        else
                        {
                            Console.WriteLine("something went wrong when trying to reach wanikani, not rate limiting tho..\n" + e.ToString());
                            label3.Invoke((MethodInvoker)(() => label3.Text = "something went wrong when trying to reach wanikani. Please try again later and restart the app."));
                            return null;
                        }
                    }
                case t.subject:
                    HttpWebRequest kanjiRequest = (HttpWebRequest)WebRequest.Create("https://api.wanikani.com/v2/subjects/" + id);
                    kanjiRequest.Headers.Add("Authorization: Bearer " + token);
                    try
                    {
                        WebResponse respo = kanjiRequest.GetResponse();
                        return respo;
                    }
                    catch (WebException e)
                    {
                        if (((HttpWebResponse)e.Response).StatusCode.Equals((HttpStatusCode)429))
                        {
                            Console.WriteLine("too many requests");
                            requestcounter = 0;
                            return fireRequest(type, id);
                        }
                        else
                        {
                            Console.WriteLine("something went wrong when trying to reach wanikani, not rate limiting tho..\n" + e.ToString());
                            label3.Invoke((MethodInvoker)(() => label3.Text = "something went wrong when trying to reach wanikani. Please try again later and restart the app."));
                            return null;
                        }
                    }
                default:
                    requestcounter++;
                    return null;
            }

        }

        WebResponse fireRequest(Uri nextUrl)
        {
            int i = 0;
            while (requestcounter < 1)
            {
                Thread.Sleep(10);

                if (i != timeuntilreset)
                {
                    label3.Invoke((MethodInvoker)(() => label3.Text = "rate limit reached. waiting for another " + timeuntilreset + " seconds."));
                    i = timeuntilreset;
                }

            }
            requestcounter--;
            label3.Invoke((MethodInvoker)(() => label3.Text = "remaining requests this minute: " + requestcounter + "/60"));
            HttpWebRequest assRequest = (HttpWebRequest)WebRequest.Create(nextUrl);
            assRequest.Headers.Add("Authorization: Bearer " + token);
            try
            {
                WebResponse resp = assRequest.GetResponse();
                return resp;
            }
            catch (WebException e)
            {
                if (((HttpWebResponse)e.Response).StatusCode.Equals((HttpStatusCode)429))
                {
                    Console.WriteLine("too many requests");
                    requestcounter = 0;
                    return fireRequest(nextUrl);
                }
                else
                {
                    Console.WriteLine("something went wrong when trying to reach wanikani, not rate limiting tho..\n" + e.ToString());
                    return null;
                }
            }
        }

        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct WIN32_FIND_DATA
        {
            public uint dwFileAttributes;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            public uint dwReserved0;
            public uint dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll")]
        private static extern bool FindClose(IntPtr hFindFile);
        // https://stackoverflow.com/a/757925
        public static bool CheckDirectoryEmpty_Fast(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(path);
            }

            if (Directory.Exists(path))
            {
                if (path.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    path += "*";
                else
                    path += Path.DirectorySeparatorChar + "*";

                WIN32_FIND_DATA findData;
                var findHandle = FindFirstFile(path, out findData);

                if (findHandle != INVALID_HANDLE_VALUE)
                {
                    try
                    {
                        bool empty = true;
                        do
                        {
                            if (findData.cFileName != "." && findData.cFileName != "..")
                                empty = false;
                        } while (empty && FindNextFile(findHandle, out findData));

                        return empty;
                    }
                    finally
                    {
                        FindClose(findHandle);
                    }
                }

                throw new Exception("Failed to get directory first file",
                    Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
            }
            throw new DirectoryNotFoundException();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            using (var imgFolderDialog = new FolderBrowserDialog())
            {
                DialogResult result = imgFolderDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(imgFolderDialog.SelectedPath))
                {
                    imgPath = imgFolderDialog.SelectedPath+"\\";
                    textBox3.Text = imgPath;
                }
            }
        }

        private void TextBox3_TextChanged(object sender, EventArgs e)
        {
            if (!textBox3.Text.EndsWith("\\")) textBox3.Text += "\\";
            imgPath = textBox3.Text;
            configlines[1] = imgPath;
            writeConfigToFile();
            //File.WriteAllText(Directory.GetCurrentDirectory() + "/imgpath.txt", imgPath);
        }

        private void CheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            configlines[2] = includeAssignementsCheckbox.Checked.ToString();
            writeConfigToFile();
            //File.WriteAllText(Directory.GetCurrentDirectory() + "/initiate.txt", includeAssignementsCheckbox.Checked.ToString());
        }

        private void SaveTokenCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (saveTokenCheckbox.Checked)
            {
                configlines[0] = ApiToken.Text;
            } else
            {
                configlines[0] = "";
            }
            writeConfigToFile();
        }

        private void ApiToken_TextChanged(object sender, EventArgs e)
        {
            if (saveTokenCheckbox.Checked)
            {
                configlines[0] = ApiToken.Text;
            }
            else
            {
                configlines[0] = "";
            }
            writeConfigToFile();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Form2 a = new Form2();
            a.ShowDialog();
            if (a.DialogResult == DialogResult.OK)
            {
                textBox3.Text = a.ReturnValue1;
            }
        }
    }
}
