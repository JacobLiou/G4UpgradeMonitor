using System.Collections.ObjectModel;

namespace Sofar.G4MultiUpgrade.Control
{
#if DEBUG
    public class UC_Base : UserControl
#else
    public abstract class UC_Base : UserControl
#endif
    {
        protected UC_Base()
        {

        }

#if DEBUG
        public virtual void Init()
        {
            throw new NotImplementedException();
        }
#else
        public abstract void Init();
#endif



        #region Check if Busy

        public bool IsBusy { get; protected set; }

        private string _busyWarning = "正在执行xx";

        public virtual string? GetBusyMessage()
        {
            if (IsBusy)
            {
                return _busyWarning;

            }
            return null;
        }

        protected void StartBusy(string busyWarning)
        {
            IsBusy = true;
            _busyWarning = busyWarning;
        }

        protected void StopBusy()
        {
            IsBusy = false;
            _busyWarning = string.Empty;
        }

        #endregion


        #region Utils

        protected void RefreshBoundDataGrid<TData>(DataGridView dataGrid, ObservableCollection<TData>? dataSource)
        {
            BeginInvoke(() =>
            {
                dataGrid.AutoGenerateColumns = false;

                if (dataSource == null || dataSource.Count == 0)
                {
                    dataGrid.DataSource = null;
                    return;
                }

                if (dataGrid.DataSource == null)
                {
                    dataGrid.DataSource = dataSource;
                    return;
                }

                if (dataGrid.Rows.Count != dataSource.Count)
                {
                    dataGrid.DataSource = null;
                    dataGrid.DataSource = dataSource;
                }
                else
                {
                    dataGrid.Invalidate();
                }
            });

        }


        protected void UpdateMsgLabel(Label lblControl, string msg, int durationMs = 2000, KnownColor foreColor = KnownColor.WindowText)
        {
            BeginInvoke(async () =>
            {
                lblControl.Text = msg;
                lblControl.ForeColor = Color.FromKnownColor(foreColor);
                lblControl.Visible = true;

                if (durationMs <= 0) return;

                await Task.Delay(durationMs);
                if (lblControl.Text == msg)
                {
                    lblControl.Text = string.Empty;
                    lblControl.Visible = false;
                }
            });
        }

        protected static DialogResult ShowMsgBoxError(string msg, MessageBoxButtons msgBoxButtons = MessageBoxButtons.OK)
        {
            return MessageBox.Show(msg, "Error", msgBoxButtons, MessageBoxIcon.Error);
        }

        protected static DialogResult ShowMsgBoxInfo(string msg, MessageBoxButtons msgBoxButtons = MessageBoxButtons.OK)
        {
            return MessageBox.Show(msg, "Information", msgBoxButtons, MessageBoxIcon.Information);
        }

        protected static DialogResult ShowMsgBoxWarning(string msg, MessageBoxButtons msgBoxButtons = MessageBoxButtons.OK)
        {
            return MessageBox.Show(msg, "Warning", msgBoxButtons, MessageBoxIcon.Warning);
        }



        // 解析号码范围表达式
        protected static bool TryParseRangeExpression(string rangeExpression, out int[] numbers, int min = 1, int max = 247)
        {
            numbers = Array.Empty<int>();
            var numbersSet = new SortedSet<int>();
            if (string.IsNullOrEmpty(rangeExpression))
            {
                return false;
            }

            string[] ranges = rangeExpression.Contains(',') ?
                rangeExpression.Trim().Split(',') : new string[] { rangeExpression.Trim() };

            for (int i = 0; i < ranges.Length; i++)
            {
                if (ranges[i].Contains('-'))
                {
                    var subRange = ranges[i].Split('-');
                    if (subRange.Length != 2
                        || !int.TryParse(subRange[0], out int subMin)
                        || !int.TryParse(subRange[1], out int subMax))
                        return false;

                    for (int j = subMin; j <= subMax; j++)
                    {
                        if (j >= min && j <= max)
                            numbersSet.Add(j);
                        else
                            return false;
                    }
                }
                else
                {
                    if (int.TryParse(ranges[i], out int num) && num >= min && num <= max)
                        numbersSet.Add(num);
                    else
                        return false;

                }
            }

            numbers = numbersSet.ToArray();

            return true;
        }


        #endregion

    }
}
