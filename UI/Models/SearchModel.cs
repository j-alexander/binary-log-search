using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;

using Algorithm;
using System.Windows.Input;

namespace UI.Models {

    public class SearchModel : DependencyObject {

        public static readonly DependencyProperty LogNameProperty =
            DependencyProperty.Register("LogName", typeof(string), typeof(SearchModel));
        public string LogName {
            get { return (string)GetValue(LogNameProperty); }
            set { SetValue(LogNameProperty, value); }
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(DateTime), typeof(SearchModel));
        public DateTime Target {
            get { return (DateTime)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly DependencyProperty TargetResultProperty =
            DependencyProperty.Register("TargetResult", typeof(DateTime?), typeof(SearchModel));
        public DateTime? TargetResult {
            get { return (DateTime?)GetValue(TargetResultProperty); }
            set { SetValue(TargetResultProperty, value); }
        }

        public static readonly DependencyProperty LowerBoundProperty =
            DependencyProperty.Register("LowerBound", typeof(long), typeof(SearchModel));
        public long LowerBound {
            get { return (long)GetValue(LowerBoundProperty); }
            set { SetValue(LowerBoundProperty, value); }
        }

        public static readonly DependencyProperty UpperBoundProperty =
            DependencyProperty.Register("UpperBound", typeof(long), typeof(SearchModel));
        public long UpperBound {
            get { return (long)GetValue(UpperBoundProperty); }
            set { SetValue(UpperBoundProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(long), typeof(SearchModel), new UIPropertyMetadata(0L));
        public long Minimum {
            get { return (long)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(long), typeof(SearchModel), new UIPropertyMetadata(100L));
        public long Maximum {
            get { return (long)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty RangeProperty =
            DependencyProperty.Register("Range", typeof(long), typeof(SearchModel));
        public long Range {
            get { return (long)GetValue(RangeProperty); }
            set { SetValue(RangeProperty, value); }
        }

        public static readonly DependencyProperty QueryPositionProperty =
            DependencyProperty.Register("QueryPosition", typeof(long), typeof(SearchModel), new UIPropertyMetadata(0L));
        public long QueryPosition {
            get { return (long)GetValue(QueryPositionProperty); }
            set { SetValue(QueryPositionProperty, value); }
        }

        public static readonly DependencyProperty CurrentPositionProperty =
            DependencyProperty.Register("CurrentPosition", typeof(long), typeof(SearchModel), new UIPropertyMetadata(0L));
        public long CurrentPosition {
            get { return (long)GetValue(CurrentPositionProperty); }
            set { SetValue(CurrentPositionProperty, value); }
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(SearchModel));
        public string Status {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public static readonly DependencyProperty IsExecutingProperty =
            DependencyProperty.Register("IsExecuting", typeof(bool), typeof(SearchModel));
        public bool IsExecuting {
            get { return (bool)GetValue(IsExecutingProperty); }
            set { SetValue(IsExecutingProperty, value); }
        }

        public static readonly DependencyProperty IsCancellingProperty =
            DependencyProperty.Register("IsCancelling", typeof(bool), typeof(SearchModel));
        public bool IsCancelling {
            get { return (bool)GetValue(IsCancellingProperty); }
            set { SetValue(IsCancellingProperty, value); }
        }

        public bool CanExecute { get { return !IsExecuting && Status != "Found"; } }

        public bool CanCancel { get { return IsExecuting && !IsCancelling; } }

        private CancellationTokenSource TokenSource { get; set; }

        private SearchState State { get; set; }

        private BinaryLogSearch Search { get; set; }

        public SearchModel(BinaryLogSearch search, DateTime target) {
            LogName = search.Name;
            Search = search;
            Target = target;
            State = new SearchState(Target, Algorithm.Status.Idle, FSharpOption<Position>.None);
        }

        public void Cancel() {
            if (IsExecuting && TokenSource != null) {
                IsCancelling = true;
                TokenSource.Cancel(false);
            }
        }

        public void Execute() {
            if (!IsExecuting) {
                IsExecuting = true;
                TokenSource = new CancellationTokenSource();
                
                var worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;
                worker.DoWork += (sender, e) => {
                    Search.Execute(TokenSource.Token, State, (state) => {
                        worker.ReportProgress(0, state);
                    });
                };
                worker.ProgressChanged += (sender, e) => {
                    State = (SearchState)e.UserState;
                    
                    if (FSharpOption<Position>.get_IsSome(State.Position)) {
                        var position = State.Position.Value;
                        if (Maximum != position.Maximum) {
                            Maximum = position.Maximum;
                        }
                        if (Minimum != position.Minimum) {
                            Minimum = position.Minimum;
                        }
                        if (LowerBound != position.LowerBound) {
                            LowerBound = position.LowerBound;
                        }
                        if (UpperBound != position.UpperBound) {
                            UpperBound = position.UpperBound;
                        }
                        if (QueryPosition != position.QueryAt) {
                            QueryPosition = position.QueryAt;
                        }
                        if (CurrentPosition != position.Current) {
                            CurrentPosition = position.Current;
                        }
                        var range = position.UpperBound - position.LowerBound;
                        if (Range != range) {
                            Range = range;
                        }
                    }

                    if (State.Status.IsIdle) {
                        Status = "Idle";
                    } else if (State.Status.IsScan) {
                        Status = "Scan";
                    } else if (State.Status.IsSeek) {
                        Status = "Seek";
                    } else if (State.Status.IsCancelled) {
                        Status = "Cancelled";
                    } else if (State.Status.IsNotFound) {
                        Status = "Not Found";
                    } else if (State.Status.IsFound) {
                        Status = "Found";
                        var found = (Algorithm.Status.Found)State.Status;
                        TargetResult = found.Item1;
                        UpperBound = found.Item2;
                        LowerBound = found.Item2;
                        QueryPosition = found.Item2;
                        CurrentPosition = found.Item2;
                        Range = 0;
                    }
                };
                worker.RunWorkerCompleted += (sender, e) => {
                    IsCancelling = false;
                    IsExecuting = false;
                    CommandManager.InvalidateRequerySuggested();
                };
                worker.RunWorkerAsync();
            }
        }
    }
}
