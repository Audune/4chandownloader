using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Downloader
{
    public class Board
    {
        public string Name { get; set; }
        public bool Repeat { get; set; }
        public int MaxPage { get; set; }
        public bool CreateFolders { get; set; }
        public long? Ticks { get; set; }
        public List<BoardThread> BoardThreads { get; set; }
        public ConcurrentBag<string> Threads { get; set; }
        public int? MinImages { get; set; }
        public int? Sleep { get; set; }

        private DirectoryInfo _directoryInfo;
        public DirectoryInfo DirectoryInfo
        {
            get
            {
                if(!_directoryInfo.Exists)
                {
                    _directoryInfo.Create();
                }
                return _directoryInfo;
            }
            set { _directoryInfo = value; }
        }

        private const string ThreadImageRegex = "\\d{13}.(jpg|png|gif)"; // contains 13 digits followed by a . and ends in a jpg or png or gif - All images currently are 13 digits long, adjust later if it changes
        private const string ThreadLinkRegex = "<a href=\"res/\\d{1,12}\" class=\"replylink\">Reply</a>"; //Finds Threads via the "Reply" button on the page
        private const string ThreadNameRegex = "\\d{1,12}"; // contains digits 1 to 12 long
        private string ImageLinkRegex
        {
            get { return "<a class=\"fileThumb\" href=\"//i.4cdn.org/" + Name + "/src/\\d{10,16}.(jpg|png|gif|jpeg)\" target=\"_blank\">"; } //I dont know why it searches for 10-16 digits when we already search for 13...
        }

        public Board(string name = "", bool repeat = false, int maxPage = 5, bool createFolders = true)
        {
            Name = name;
            Repeat = repeat;
            MaxPage = maxPage;
            CreateFolders = createFolders;
            BoardThreads = new List<BoardThread>();
            Threads = new ConcurrentBag<string>();
        }

        public void GatherThreads()  //Reads threads up to MaxThread variable amount.  Takes Fourm threads it finds via "Reply" and adds them to the list
        {
            // Page 0 -> Page 15
            Parallel.For(fromInclusive: 0, toExclusive: MaxPage, body: (i, a) =>
            {
                try
                {
                    Console.WriteLine(string.Format("STGR http://boards.4chan.org/{0}/{1}", Name, i));  //Text to Console telling you wtf its doing
                    string html = new WebClient().DownloadString(string.Format("http://boards.4chan.org/{0}/{1}", Name, i));  //goes to Board, finds thread
                    var matchCollection = Regex.Matches(html, ThreadLinkRegex);  //Finds shit defined under ThreadLinkRegex
                    matchCollection.Cast<Match>().ToList().ForEach(thread => Threads.Add(Regex.Match(thread.Value, ThreadNameRegex).Value));  //Adds Threads found to list
                    Console.WriteLine(string.Format("FNGT http://boards.4chan.org/{0}/{1}", Name, i));  //Writes to Console it is/was reading the Board index of threads
                }
                catch(Exception e)
                {
                    Console.WriteLine(string.Format("FTGT http://boards.4chan.org/{0}/{1}][{2}", Name, i, e.Message));  //Exception to catch something, some Error thing, prolly 404 stuff
                }     
            });
        }

        public void ProcessThreads()  //I think this is to make Folders in the download directory, where Images will end up going???  Based on Board/Thread name.
        {
            var threadFolders = Threads
                .Select(thread => new BoardThread()
                                      {
                                          Parent = this,
                                          Name = thread,
                                          DirectoryInfo = CreateFolders ? DirectoryInfo.CreateSubdirectory(thread) : DirectoryInfo,
                                      });

            BoardThreads = new List<BoardThread>(threadFolders);

            var groups = BoardThreads.Select((x, i) => new { Index = i, Value = x })
                    .GroupBy(x => x.Index / 5) // Change value to change how many folders it makes at a time?  Currently set to 5.
                    .Select(x => x.Select(v => v.Value).ToList()).ToList();

            foreach(var group in groups)
            {
                Parallel.ForEach(source: group, body: ProcessThread);
            }

            
        }


        private void ProcessThread(BoardThread thread)  //Reads the Boards thread looking at images to download
        {
            try
            {
                string html = new WebClient().DownloadString(address: new Uri(string.Format(@"http://boards.4chan.org/{0}/res/{1}", Name, thread.Name)));  //Reading....

                MatchCollection links = Regex.Matches(html, ImageLinkRegex, RegexOptions.IgnoreCase);  //Find stuff that matches rules in ImageLinkRegex

                if(MinImages.HasValue)  //If enabled, wont download from threads with less than a certain number of images
                {
                    if(links.Count < MinImages.Value)
                    {
                        Console.WriteLine(string.Format(@"DNMF http://i.4cdn.org/{0}/src/{1}", Name, thread.Name));

                        if(!thread.DirectoryInfo.Equals(DirectoryInfo))
                            thread.DirectoryInfo.Delete();
                            
                        return;
                    }
                }

                var groups = links.Cast<Match>().AsParallel().Select((x, i) => new { Index = i, Value = x })
                    .GroupBy(x => x.Index / 10)
                    .Select(x => x.Select(v => v.Value).ToList()).ToList();

                foreach (var group in groups)
                {
                    Console.WriteLine("Processing the next 10 images...");
                    // Send in a set of 10 at once...
                    Parallel.ForEach(group, link =>
                    {
                        string image = Regex.Match(link.Value, ThreadImageRegex).Value;

                        try
                        {
                            Console.WriteLine(string.Format(@"DLNG http://boards.4chan.org/{0}/src/{1}/{2}", Name, thread.Name, image));
                            new WebClient().DownloadFile(new Uri(string.Format(@"http://i.4cdn.org/{0}/src/{1}", Name, image)), thread.DirectoryInfo + "\\" + image);
                        }
                        catch (WebException e)  //Example:404 prints to the console that the image could not be downloaded in time
                        {
                            var httpWebResponse = e.Response as HttpWebResponse;

                            if (httpWebResponse != null)
                            {
                                Console.WriteLine(string.Format(@"FTDL http://i.4cdn.org/{0}/src/{1} - {2}", Name, image, (int)(httpWebResponse).StatusCode));
                            }
                        }
                    });
                }
            }
            catch (WebException e) //Example:404 prints to the console that the image could not be downloaded in time
            {
                var httpWebResponse = e.Response as HttpWebResponse;

                if (httpWebResponse != null)
                {
                    Console.WriteLine(string.Format(@"FTDL http://i.4cdn.org/{0}/src/{1} - {2}", Name, thread.Name, (int)(httpWebResponse).StatusCode));
                }
            }

        }

    }
}
