﻿using NSIT_Connect.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Mvvm;
using Windows.Data.Json;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace NSIT_Connect.ViewModels
{
    public class MyFeedPageViewModel : ViewModelBase
    {
        public List<ChooseFeedItem> ChooseSelectedItem;
        private string next = "https://graph.facebook.com/" + Constants.id_nsitonline + "/posts?limit=20&fields=id,picture,from,shares,message," +
            "object_id,link,created_time,comments.limit(0).summary(true),likes.limit(0).summary(true)" +
            "&access_token=" + Constants.common_access;
        private Dictionary<string, string> nextdic = new Dictionary<string, string>();
        private string result = null;
        private Dictionary<string, string> resultdic = new Dictionary<string, string>();
        private string pictureresult = null;
        private bool refresh = false;
        public Feed temp;
        private Dictionary<string, bool> refreshdic = new Dictionary<string, bool>();
        private string PictureUri = null;
        private string hobject, hmessage, hpicture, hlink, hlikes, htime;
        private string[] ChooseSource = {
            "ms-appx:///Assets/ChooseFeedItem/collegespace.png",
            "ms-appx:///Assets/ChooseFeedItem/crosslinks.png",
            "ms-appx:///Assets/ChooseFeedItem/csi.png",
            "ms-appx:///Assets/ChooseFeedItem/ieee.png",
            "ms-appx:///Assets/ChooseFeedItem/ashwamedh.png",
            "ms-appx:///Assets/ChooseFeedItem/junoon.png",
            "ms-appx:///Assets/ChooseFeedItem/debsoc.png",
            "ms-appx:///Assets/ChooseFeedItem/quiz.png",
            "ms-appx:///Assets/ChooseFeedItem/bullethawk.png",
            "ms-appx:///Assets/ChooseFeedItem/rotaract.png",
            "ms-appx:///Assets/ChooseFeedItem/enactus.png",
            "ms-appx:///Assets/ChooseFeedItem/aagaz.png"

        };

        private string[] ids = {
            Constants.id_collegespace,
            Constants.id_crosslinks,
            Constants.id_csi,
            Constants.id_ieee,
            Constants.id_ashwa,
            Constants.id_junoon,
            Constants.id_debsoc,
            Constants.id_quiz,
            Constants.id_bullet,
            Constants.id_rotaract,
            Constants.id_enactus,
            Constants.id_aagaz
        };

        private string[] ChooseTitle = {
            "COLLEGESPACE","CROSSLINKS","CSI NSIT","IEEE NSIT","ASHWAMEDH","JUNOON" ,"DEBSOC","QUIZ CLUB","BULLETHAWK","ROTARACT","ENACTUS","AAGAZ"
        };

        private ObservableCollection<ChooseFeedItem> item = new ObservableCollection<ChooseFeedItem>();
        public ObservableCollection<ChooseFeedItem> Item { get { return item; } set { item = value; } }

        public MyFeedPageViewModel()
        {
            ChooseSelectedItem = new List<ChooseFeedItem>();
            HomeFeed = new ObservableCollection<Feed>();
            for (int i = 0; i < 12; i++)
            {
                Item.Add(new ChooseFeedItem() {ImageSource = new Uri(ChooseSource[i]),Title = ChooseTitle[i] ,ID = ids[i] });
            } 

            foreach(string item in ids)
            {
                resultdic.Add(item,null);
                refreshdic.Add(item, false);
                nextdic.Add(item, "https://graph.facebook.com/" + item + "/posts?limit=20&fields=id,picture,from,shares,message," +
            "object_id,link,created_time,comments.limit(0).summary(true),likes.limit(0).summary(true)" +
            "&access_token=" + Constants.common_access);
            }
        }


        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            RefreshCommand.Execute();
            return Task.CompletedTask;
        }



        ObservableCollection<Feed> _homefeed = default(ObservableCollection<Feed>);

        public ObservableCollection<Feed> HomeFeed
        {
            get { return _homefeed; }
            private set { Set(ref _homefeed, value); }
        }

        //string _searchText = default(string);

        //public string SearchText
        //{
        //    get { return _searchText; }
        //    set { Set(ref _searchText, value); }
        //}

        //public readonly DelegateCommand SwitchToPageCommand =
        //    new DelegateCommand(() => BootStrapper.Current.NavigationService.Navigate(typeof(MainPage)));

        public DelegateCommand RefreshCommand => new DelegateCommand(() =>
        {
            IsMasterLoading = true;
            HomeFeed.Clear();
            Selected = null;
            foreach(string item in ids)
            {
                refreshdic[item] = true;
            }
            WindowWrapper.Current().Dispatcher.Dispatch(() =>
            {
                Chooseinfo();
                Selected = HomeFeed?.FirstOrDefault();
                IsMasterLoading = false;

            }, 2000);
        });

        Feed _selected = default(Feed);

        public object Selected
        {
            get { return _selected; }
            set
            {
                var message = value as Feed;
                Set(ref _selected, message);
                //NextCommand.RaiseCanExecuteChanged();
                //PreviousCommand.RaiseCanExecuteChanged();
                if (message == null) return;
                message.IsRead = true;
                IsDetailsLoading = true;
                WindowWrapper.Current().Dispatcher.Dispatch(() =>
                {
                    IsDetailsLoading = false;
                }, 1000);
            }
        }

        //private DelegateCommand _nextCommand;

        //public DelegateCommand NextCommand
        //{
        //    get
        //    {
        //        return _nextCommand ??
        //            (_nextCommand = new DelegateCommand(ExecuteNextCommand, CanExecuteNextCommand));
        //    }
        //    set { Set(ref _nextCommand, value); }
        //}

        //private void ExecuteNextCommand()
        //{
        //    if (Selected == null)
        //        return;
        //    var index = HomeFeed.IndexOf(_selected);
        //    if (index == -1)
        //        return;
        //    var next = index + 1;
        //    Selected = HomeFeed[next];
        //}

        //private bool CanExecuteNextCommand()
        //{
        //    if (Selected == null)
        //        return false;
        //    var index = HomeFeed.IndexOf(_selected);
        //    if (index == -1)
        //        return false;
        //    if (index == HomeFeed.Count - 2)
        //        getinfo();
        //    return index < (HomeFeed.Count - 1);
        //}

        //private DelegateCommand _previousCommand;

        //public DelegateCommand PreviousCommand
        //{
        //    get
        //    {
        //        return _previousCommand ??
        //               (_previousCommand = new DelegateCommand(ExecutePreviousCommand, CanExecutePreviousCommand));
        //    }
        //    set { Set(ref _previousCommand, value); }
        //}

        //private bool CanExecutePreviousCommand()
        //{
        //    if (Selected == null)
        //        return false;
        //    var index = HomeFeed.IndexOf(_selected);
        //    return index > 0;
        //}

        //private void ExecutePreviousCommand()
        //{
        //    if (Selected == null)
        //        return;
        //    var index = HomeFeed.IndexOf(_selected);
        //    if (index == -1)
        //        return;
        //    var previous = index - 1;
        //    Selected = HomeFeed[previous];
        //}

        private bool _isDetailsLoading;

        public bool IsDetailsLoading
        {
            get { return _isDetailsLoading; }
            set { Set(ref _isDetailsLoading, value); }
        }

        private bool _isMasterLoading;

        public bool IsMasterLoading
        {
            get { return _isMasterLoading; }
            set { Set(ref _isMasterLoading, value); }
        }

        public async void Chooseinfo()
        {
            foreach(ChooseFeedItem item in ChooseSelectedItem)
            {
                getinfo(item.ID);
            }
        }

        public async void getinfo(string id)
        {
            if (refreshdic[id])
                nextdic[id] = "https://graph.facebook.com/" + id + "/posts?limit=10&fields=picture,shares,message,object_id," +
                    "link,comments.limit(0).summary(true),to,created_time,likes.limit(0).summary(true)&access_token=" + Constants.common_access;
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                var httpClient = new HttpClient();
                var uri = new Uri(nextdic[id]);

                try
                {
                    HttpResponseMessage responseMessage = await httpClient.GetAsync(uri);
                    responseMessage.EnsureSuccessStatusCode();
                    resultdic[id] = await responseMessage.Content.ReadAsStringAsync();
                }
                catch (Exception ex) { }


                httpClient.Dispose();
            }
            else
            {
                var mssg = new MessageDialog("No Internet");
                await mssg.ShowAsync();
            }
            JsonObject ob, ob2;
            JsonArray arr;
            if (resultdic[id] != null && resultdic[id] != string.Empty)
            {
                ob = JsonObject.Parse(resultdic[id]);
                arr = ob.GetNamedArray("data");


                int len = arr.Count;
                for (uint i = 0; i < len; i++)
                {

                    if (arr.GetObjectAt(i).ContainsKey("message"))
                        hmessage = arr.GetObjectAt(i).GetNamedString("message");
                    else
                        hmessage = "No Description";

                    if (!(arr.GetObjectAt(i).ContainsKey("object_id")))
                        hobject = string.Empty;
                    else
                        hobject = arr.GetObjectAt(i).GetNamedString("object_id");

                    if (arr.GetObjectAt(i).ContainsKey("picture"))
                    {
                        hpicture = arr.GetObjectAt(i).GetNamedString("picture");
                    }
                    else
                        hpicture = string.Empty;
                    if (arr.GetObjectAt(i).ContainsKey("link"))
                    {
                        hlink = arr.GetObjectAt(i).GetNamedString("link");
                    }
                    else
                        hlink = string.Empty;
                    if (arr.GetObjectAt(i).ContainsKey("likes"))
                    {

                        JsonObject o = arr.GetObjectAt(i).GetNamedObject("likes");
                        JsonObject o2 = o.GetNamedObject("summary");

                        hlikes = o2.GetNamedNumber("total_count").ToString();   //No of likes
                    }
                    else
                        hlikes = "0";

                    if (arr.GetObjectAt(i).ContainsKey("created_time"))
                        htime = arr.GetObjectAt(i).GetNamedString("created_time");
                    else
                        htime = string.Empty;
                    DateTime dt = Convert.ToDateTime(htime);
                    HomeFeed.Add(new Feed() { Object_ID = hobject, Likes = hlikes, Link = hlink, Message = hmessage, Picture = hpicture, Time_Created = dt.ToString("d MMM , h:mm tt") });

                }
                ob = ob.GetNamedObject("paging");
                nextdic[id] = ob.GetNamedString("next");

            }

            foreach (Feed item in HomeFeed)
            {

            }

            if (refreshdic[id] && HomeFeed.Count > 0)
            {
                Selected = HomeFeed[0];
                getpicture(Selected as Feed);
            }
            refreshdic[id] = false;
        }

        public async void getpicture(Feed message)
        {
            pictureresult = null;
            if (message.Object_ID != null)
            {
                PictureUri = "https://graph.facebook.com/" + message.Object_ID + "?fields=images&access_token=" + Constants.common_access;
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    var httpClient = new HttpClient();
                    var uri = new Uri(PictureUri);

                    try
                    {
                        HttpResponseMessage responseMessage = await httpClient.GetAsync(uri);
                        responseMessage.EnsureSuccessStatusCode();
                        pictureresult = await responseMessage.Content.ReadAsStringAsync();
                    }
                    catch (Exception ex) { }


                    httpClient.Dispose();
                }
                if (pictureresult != null)
                {
                    JsonObject obt = JsonObject.Parse(pictureresult);
                    JsonArray array = obt.GetNamedArray("images");
                    if (array.GetObjectAt(0).ContainsKey("source"))
                    {
                        message.PictureUri = new Uri(array.GetObjectAt(0).GetNamedString("source"));
                    }
                    
                }
                if(message.PictureUri == null && message.Picture!=null && message.Picture != string.Empty)
                {
                    message.PictureUri = new Uri(message.Picture);
                }
            }
            else
            {
                message.PictureUri = new Uri(message.Picture);
            }
        }
    }
}
