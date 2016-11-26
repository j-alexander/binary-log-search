using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Algorithm;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace UI.Models {

    public class QueryModel : DependencyObject {

        public static readonly DependencyProperty LogStoresProperty =
            DependencyProperty.Register("LogStores", typeof(LogStore[]), typeof(QueryModel));
        public LogStore[] LogStores {
            get { return (LogStore[])GetValue(LogStoresProperty); }
            set { SetValue(LogStoresProperty, value); }
        }

        public static readonly DependencyProperty SelectedLogStoreProperty =
            DependencyProperty.Register("SelectedLogStore", typeof(LogStore), typeof(QueryModel));
        public LogStore SelectedLogStore {
            get { return (LogStore)GetValue(SelectedLogStoreProperty); }
            set { SetValue(SelectedLogStoreProperty, value); }
        }

        public static readonly DependencyProperty HostProperty =
            DependencyProperty.Register("Host", typeof(string), typeof(QueryModel));
        public string Host {
            get { return (string)GetValue(HostProperty); }
            set { SetValue(HostProperty, value); }
        }

        public static readonly DependencyProperty ChannelProperty =
            DependencyProperty.Register("Channel", typeof(string), typeof(QueryModel));
        public string Channel {
            get { return (string)GetValue(ChannelProperty); }
            set { SetValue(ChannelProperty, value); }
        }

        public static readonly DependencyProperty StartAtProperty =
            DependencyProperty.Register("StartAt", typeof(int), typeof(QueryModel), new UIPropertyMetadata(0));
        public int StartAt {
            get { return (int)GetValue(StartAtProperty); }
            set { SetValue(StartAtProperty, value); }
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(DateTime), typeof(QueryModel));
        public DateTime Target {
            get { return (DateTime)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly DependencyProperty TargetPathProperty =
            DependencyProperty.Register("TargetPath", typeof(string), typeof(QueryModel), new UIPropertyMetadata("$.timestamp"));
        public string TargetPath {
            get { return (string)GetValue(TargetPathProperty); }
            set { SetValue(TargetPathProperty, value); }
        }

        public static readonly DependencyProperty IsConnectingProperty =
            DependencyProperty.Register("IsConnecting", typeof(bool), typeof(QueryModel), new UIPropertyMetadata(false));
        public bool IsConnecting {
            get { return (bool)GetValue(IsConnectingProperty); }
            set { SetValue(IsConnectingProperty, value); }
        }

        public static readonly DependencyProperty IsConnectedProperty =
            DependencyProperty.Register("IsConnected", typeof(bool), typeof(QueryModel), new UIPropertyMetadata(false));
        public bool IsConnected {
            get { return (bool)GetValue(IsConnectedProperty); }
            set { SetValue(IsConnectedProperty, value); }
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(QueryModel));
        public string Status {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public static readonly DependencyProperty SearchesProperty =
            DependencyProperty.Register("Searches", typeof(ObservableCollection<SearchModel>), typeof(QueryModel));
        public ObservableCollection<SearchModel> Searches {
            get { return (ObservableCollection<SearchModel>)GetValue(SearchesProperty); }
        }
        
        public QueryModel() {
            SetValue(SearchesProperty, new ObservableCollection<SearchModel>());

            LogStores = Algorithm.Connections.all;
            SelectedLogStore = Algorithm.Connections.kafka;

            if (Debugger.IsAttached) {
                Host = "tcp://guardians-kafka-cluster.qa.jet.com:9092";
                Channel = "nova-retailskus-profx";
                StartAt = 33;
                Target = new DateTime(2016,11,20);
            }
        }

        public void Connect() {
            if (!IsConnecting) {
                IsConnecting = true;
                IsConnected = false;
                Status = "Connecting...";

                var store = SelectedLogStore;
                var host = Host;
                var channel = Channel;
                var startAt = StartAt;
                var target = Target;
                var targetPath = TargetPath;

                var worker = new BackgroundWorker();
                worker.DoWork += (sender, e) => {
                    var codec = Codec.create(startAt, targetPath);
                    var connections = store.connect(host, channel, codec);
                    var searches = connections.Select(x => new BinaryLogSearch(x)).ToArray();
                    e.Result = searches;
                };
                worker.RunWorkerCompleted += (sender, e) => {
                    IsConnecting = false;
                    if (e.Error != null) {
                        IsConnected = false;
                        Status = String.Format("Error: {0}", e.Error.Message);
                    } else if (e.Cancelled) {
                        IsConnected = false;
                        Status = "Your connection request was cancelled.";
                    } else {
                        IsConnected = true;
                        Status = "Connected successfully.";
                        Searches.Clear();
                        foreach (var search in (BinaryLogSearch[])e.Result) {
                            Searches.Add(new SearchModel(search, target));
                        }
                        CommandManager.InvalidateRequerySuggested();
                    }
                };
                worker.RunWorkerAsync();
            }
        }
    }
}
